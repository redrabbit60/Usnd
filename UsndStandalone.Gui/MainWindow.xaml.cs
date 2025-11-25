using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using NAudio.Wave;
using UsndStandalone;

namespace UsndStandalone.Gui;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}

public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;
    private readonly string _xmlDir;
    private readonly string _masterPath;
    private readonly string _categoryPath;
    private readonly string _labelPath;

    private MasterSettings _master;
    private CategorySettings _category;
    private LabelSettings _labels;
    private readonly AudioPlayer _player = new();

    private LabelItemViewModel? _selectedLabel;
    private CategoryItemViewModel? _selectedCategory;
    private MasterItemViewModel? _selectedMaster;
    private string _statusMessage = "XML を読み込みました。";
    private int _selectedBrowserTabIndex = 2; // デフォルトはLabel

    public ObservableCollection<LabelItemViewModel> LabelItems { get; } = new();
    public ObservableCollection<CategoryItemViewModel> CategoryItems { get; } = new();
    public ObservableCollection<MasterItemViewModel> MasterItems { get; } = new();
    public PointCollection WaveformPoints { get; private set; } = new();
    public IReadOnlyList<string> CategoryBehaviorOptions { get; } = new[] { "STEAL_OLDEST", "JUST_FAIL", "QUEUE" };

    public int SelectedBrowserTabIndex
    {
        get => _selectedBrowserTabIndex;
        set
        {
            if (_selectedBrowserTabIndex == value) return;
            _selectedBrowserTabIndex = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedBrowserTabName));
            OnPropertyChanged(nameof(MasterDetailVisibility));
            OnPropertyChanged(nameof(CategoryDetailVisibility));
            OnPropertyChanged(nameof(LabelDetailVisibility));
        }
    }

    public string SelectedBrowserTabName
    {
        get
        {
            return _selectedBrowserTabIndex switch
            {
                0 => "Master",
                1 => "Category",
                2 => "Label",
                _ => "Label"
            };
        }
    }

    public Visibility MasterDetailVisibility => _selectedBrowserTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
    public Visibility CategoryDetailVisibility => _selectedBrowserTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
    public Visibility LabelDetailVisibility => _selectedBrowserTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;

    public LabelItemViewModel? SelectedLabel
    {
        get => _selectedLabel;
        set
        {
            if (_selectedLabel == value) return;
            _selectedLabel = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedLabelLoop));
            OnPropertyChanged(nameof(SelectedLabelIsStealOldest));
            UpdateStatus(value != null ? $"選択: {value.LabelName}" : "ラベル未選択");
            PlayCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
        }
    }

    public CategoryItemViewModel? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (_selectedCategory == value) return;
            _selectedCategory = value;
            OnPropertyChanged();
            RemoveCategoryCommand.RaiseCanExecuteChanged();
        }
    }

    public MasterItemViewModel? SelectedMaster
    {
        get => _selectedMaster;
        set
        {
            if (_selectedMaster == value) return;
            _selectedMaster = value;
            OnPropertyChanged();
            RemoveMasterCommand.RaiseCanExecuteChanged();
        }
    }

    public bool SelectedLabelLoop
    {
        get => SelectedLabel?.LoopBool ?? false;
        set
        {
            if (SelectedLabel == null) return;
            SelectedLabel.LoopBool = value;
            OnPropertyChanged();
        }
    }

    public bool SelectedLabelIsStealOldest
    {
        get => SelectedLabel?.IsStealOldestBool ?? false;
        set
        {
            if (SelectedLabel == null) return;
            SelectedLabel.IsStealOldestBool = value;
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (_statusMessage == value) return;
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand PlayCommand { get; }
    public RelayCommand StopCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand AddMasterCommand { get; }
    public RelayCommand RemoveMasterCommand { get; }
    public RelayCommand AddCategoryCommand { get; }
    public RelayCommand RemoveCategoryCommand { get; }

    public MainViewModel()
    {
        _xmlDir = Path.GetFullPath(Path.Combine(_baseDir, "..", "..", "..", "xml"));
        _masterPath = Path.Combine(_xmlDir, "MasterSettings.xml");
        _categoryPath = Path.Combine(_xmlDir, "CategorySettings.xml");
        _labelPath = Path.Combine(_xmlDir, "LabelSettings.xml");

        _master = XmlStore.Load<MasterSettings>(_masterPath);
        _category = XmlStore.Load<CategorySettings>(_categoryPath);
        _labels = XmlStore.Load<LabelSettings>(_labelPath);
        RefreshCollections();

        PlayCommand = new RelayCommand(_ => PlaySelected(), _ => SelectedLabel != null);
        StopCommand = new RelayCommand(_ => StopAudio(), _ => true);
        SaveCommand = new RelayCommand(_ => SaveXml());
        AddMasterCommand = new RelayCommand(_ => AddMaster());
        RemoveMasterCommand = new RelayCommand(_ => RemoveMaster(), _ => SelectedMaster != null);
        AddCategoryCommand = new RelayCommand(_ => AddCategory());
        RemoveCategoryCommand = new RelayCommand(_ => RemoveCategory(), _ => SelectedCategory != null);
    }

    private void RefreshCollections()
    {
        LabelItems.Clear();
        foreach (var item in _labels.Items)
        {
            LabelItems.Add(new LabelItemViewModel(item));
        }

        if (LabelItems.Count > 0)
        {
            SelectedLabel = LabelItems[0];
        }

        CategoryItems.Clear();
        foreach (var c in _category.Items)
        {
            CategoryItems.Add(new CategoryItemViewModel(c));
        }
        if (CategoryItems.Count > 0)
        {
            SelectedCategory = CategoryItems[0];
        }

        MasterItems.Clear();
        foreach (var m in _master.Items)
        {
            MasterItems.Add(new MasterItemViewModel(m));
        }
        if (MasterItems.Count > 0)
        {
            SelectedMaster = MasterItems[0];
        }
    }

    private void PlaySelected()
    {
        if (SelectedLabel == null)
        {
            UpdateStatus("ラベルが選択されていません。");
            return;
        }

        var fileCandidates = new[]
        {
            Path.Combine(_baseDir, SelectedLabel.FileName ?? string.Empty),
            Path.Combine(_baseDir, "Audio", SelectedLabel.FileName ?? string.Empty),
            Path.GetFullPath(Path.Combine(_xmlDir, "..", "Audio", SelectedLabel.FileName ?? string.Empty))
        };

        var path = fileCandidates.FirstOrDefault(File.Exists);
        if (path == null)
        {
            UpdateStatus($"音声ファイルが見つかりません: {SelectedLabel.FileName}");
            return;
        }

        LoadWaveform(path);
        _player.Play(path, SelectedLabel.LoopBool);
        UpdateStatus($"再生中: {SelectedLabel.LabelName} ({path})");
    }

    private void StopAudio()
    {
        _player.Stop();
        UpdateStatus("再生を停止しました。");
    }

    private void SaveXml()
    {
        _master.Items = MasterItems.Select(m => m.ToMasterSet()).ToList();
        _category.Items = CategoryItems.Select(c => c.ToCategorySet()).ToList();
        _labels.Items = LabelItems.Select(l => l.ToLabelSet()).ToList();

        XmlStore.Save(_masterPath, _master);
        XmlStore.Save(_categoryPath, _category);
        XmlStore.Save(_labelPath, _labels);

        UpdateStatus("XML を保存しました。");
    }

    private void AddMaster()
    {
        var newMaster = new MasterItemViewModel(new MasterSet
        {
            MasterName = $"Master_{MasterItems.Count + 1}",
            Volume = "1"
        });
        MasterItems.Add(newMaster);
        SelectedMaster = newMaster;
        UpdateStatus($"Master を追加: {newMaster.MasterName}");
    }

    private void RemoveMaster()
    {
        if (SelectedMaster == null) return;

        var name = SelectedMaster.MasterName;
        var index = MasterItems.IndexOf(SelectedMaster);
        MasterItems.Remove(SelectedMaster);

        if (MasterItems.Count > 0)
        {
            SelectedMaster = MasterItems[Math.Min(index, MasterItems.Count - 1)];
        }
        else
        {
            SelectedMaster = null;
        }

        UpdateStatus($"Master を削除: {name}");
        RemoveMasterCommand.RaiseCanExecuteChanged();
    }

    private void AddCategory()
    {
        var newCategory = new CategoryItemViewModel(new CategorySet
        {
            CategoryName = $"Category_{CategoryItems.Count + 1}",
            Volume = "1",
            MaxNum = "0",
            MasterName = ""
        });
        CategoryItems.Add(newCategory);
        SelectedCategory = newCategory;
        UpdateStatus($"Category を追加: {newCategory.CategoryName}");
    }

    private void RemoveCategory()
    {
        if (SelectedCategory == null) return;

        var name = SelectedCategory.CategoryName;
        var index = CategoryItems.IndexOf(SelectedCategory);
        CategoryItems.Remove(SelectedCategory);

        if (CategoryItems.Count > 0)
        {
            SelectedCategory = CategoryItems[Math.Min(index, CategoryItems.Count - 1)];
        }
        else
        {
            SelectedCategory = null;
        }

        UpdateStatus($"Category を削除: {name}");
        RemoveCategoryCommand.RaiseCanExecuteChanged();
    }

    private void UpdateStatus(string message) => StatusMessage = $"{DateTime.Now:HH:mm:ss} - {message}";

    private void LoadWaveform(string path)
    {
        try
        {
            using var reader = new AudioFileReader(path);
            int sampleCount = (int)(reader.Length / (reader.WaveFormat.BitsPerSample / 8));
            int channels = reader.WaveFormat.Channels;
            int points = 400;

            float[] buffer = new float[sampleCount];
            int read = reader.Read(buffer, 0, sampleCount);
            if (read == 0)
            {
                WaveformPoints = new PointCollection();
                OnPropertyChanged(nameof(WaveformPoints));
                return;
            }

            // モノラル化＆ピークサンプリング
            double[] mono = new double[read / channels];
            for (int i = 0, m = 0; i < read; i += channels, m++)
            {
                double sum = 0;
                for (int ch = 0; ch < channels; ch++)
                    sum += Math.Abs(buffer[i + ch]);
                mono[m] = sum / channels;
            }

            int step = Math.Max(1, mono.Length / points);
            var pc = new PointCollection();
            double width = 240.0;
            double height = 110.0;

            for (int i = 0, idx = 0; i < mono.Length && idx < points; i += step, idx++)
            {
                double x = width * idx / (points - 1);
                double y = height * (0.5 - mono[i] * 0.45);
                pc.Add(new Point(x, y + height / 2));
            }

            WaveformPoints = pc;
            OnPropertyChanged(nameof(WaveformPoints));
        }
        catch
        {
            WaveformPoints = new PointCollection();
            OnPropertyChanged(nameof(WaveformPoints));
        }
    }

    public void Dispose()
    {
        _player.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class LabelItemViewModel : INotifyPropertyChanged
{
    private readonly LabelSet _inner;

    public LabelItemViewModel(LabelSet inner)
    {
        _inner = inner;
    }

    public string Initial => string.IsNullOrWhiteSpace(LabelName) ? "?" : LabelName!.Substring(0, 1).ToUpperInvariant();

    public string? LabelName
    {
        get => _inner.LabelName;
        set { _inner.LabelName = value; OnPropertyChanged(); }
    }

    public string? FileName
    {
        get => _inner.FileName;
        set { _inner.FileName = value; OnPropertyChanged(); }
    }

    public string? Volume
    {
        get => _inner.Volume;
        set { _inner.Volume = value; OnPropertyChanged(); }
    }

    public string? CategoryBehavior
    {
        get => _inner.CategoryBehavior;
        set { _inner.CategoryBehavior = value; OnPropertyChanged(); }
    }

    public string? Priority
    {
        get => _inner.Priority;
        set { _inner.Priority = value; OnPropertyChanged(); }
    }

    public string? CategoryName
    {
        get => _inner.CategoryName;
        set { _inner.CategoryName = value; OnPropertyChanged(); }
    }

    public string? SingleGroup
    {
        get => _inner.SingleGroup;
        set { _inner.SingleGroup = value; OnPropertyChanged(); }
    }

    public string? MaxNum
    {
        get => _inner.MaxNum;
        set { _inner.MaxNum = value; OnPropertyChanged(); }
    }

    public string? UnityMixerName
    {
        get => _inner.UnityMixerName;
        set { _inner.UnityMixerName = value; OnPropertyChanged(); }
    }

    public string? SpatialGroup
    {
        get => _inner.SpatialGroup;
        set { _inner.SpatialGroup = value; OnPropertyChanged(); }
    }

    public string? Delay
    {
        get => _inner.Delay;
        set { _inner.Delay = value; OnPropertyChanged(); }
    }

    public string? Interval
    {
        get => _inner.Interval;
        set { _inner.Interval = value; OnPropertyChanged(); }
    }

    public string? Pan
    {
        get => _inner.Pan;
        set { _inner.Pan = value; OnPropertyChanged(); }
    }

    public string? Pitch
    {
        get => _inner.Pitch;
        set { _inner.Pitch = value; OnPropertyChanged(); }
    }

    public string? FadeInTime
    {
        get => _inner.FadeInTime;
        set { _inner.FadeInTime = value; OnPropertyChanged(); }
    }

    public string? FadeOutTime
    {
        get => _inner.FadeOutTime;
        set { _inner.FadeOutTime = value; OnPropertyChanged(); }
    }

    public string? FadeInOldSample
    {
        get => _inner.FadeInOldSample;
        set { _inner.FadeInOldSample = value; OnPropertyChanged(); }
    }

    public string? FadeOutOnPause
    {
        get => _inner.FadeOutOnPause;
        set { _inner.FadeOutOnPause = value; OnPropertyChanged(); }
    }

    public string? FadeInOffPause
    {
        get => _inner.FadeInOffPause;
        set { _inner.FadeInOffPause = value; OnPropertyChanged(); }
    }

    public string? VolRndMin
    {
        get => _inner.VolRndMin;
        set { _inner.VolRndMin = value; OnPropertyChanged(); }
    }

    public string? VolRndMax
    {
        get => _inner.VolRndMax;
        set { _inner.VolRndMax = value; OnPropertyChanged(); }
    }

    public string? VolRndUnit
    {
        get => _inner.VolRndUnit;
        set { _inner.VolRndUnit = value; OnPropertyChanged(); }
    }

    public string? PitchRndMin
    {
        get => _inner.PitchRndMin;
        set { _inner.PitchRndMin = value; OnPropertyChanged(); }
    }

    public string? PitchRndMax
    {
        get => _inner.PitchRndMax;
        set { _inner.PitchRndMax = value; OnPropertyChanged(); }
    }

    public string? PitchRndUnit
    {
        get => _inner.PitchRndUnit;
        set { _inner.PitchRndUnit = value; OnPropertyChanged(); }
    }

    public string? PanRndMin
    {
        get => _inner.PanRndMin;
        set { _inner.PanRndMin = value; OnPropertyChanged(); }
    }

    public string? PanRndMax
    {
        get => _inner.PanRndMax;
        set { _inner.PanRndMax = value; OnPropertyChanged(); }
    }

    public string? PanRndUnit
    {
        get => _inner.PanRndUnit;
        set { _inner.PanRndUnit = value; OnPropertyChanged(); }
    }

    public string? RndSrc
    {
        get => _inner.RndSrc;
        set { _inner.RndSrc = value; OnPropertyChanged(); }
    }

    public string? PitchStart
    {
        get => _inner.PitchStart;
        set { _inner.PitchStart = value; OnPropertyChanged(); }
    }

    public string? PitchEnd
    {
        get => _inner.PitchEnd;
        set { _inner.PitchEnd = value; OnPropertyChanged(); }
    }

    public string? PitchMoveTime
    {
        get => _inner.PitchMoveTime;
        set { _inner.PitchMoveTime = value; OnPropertyChanged(); }
    }

    public string? PanStart
    {
        get => _inner.PanStart;
        set { _inner.PanStart = value; OnPropertyChanged(); }
    }

    public string? PanEnd
    {
        get => _inner.PanEnd;
        set { _inner.PanEnd = value; OnPropertyChanged(); }
    }

    public string? PanMoveTime
    {
        get => _inner.PanMoveTime;
        set { _inner.PanMoveTime = value; OnPropertyChanged(); }
    }

    public string? DuckingCategory
    {
        get => _inner.DuckingCategory;
        set { _inner.DuckingCategory = value; OnPropertyChanged(); }
    }

    public string? DuckStart
    {
        get => _inner.DuckStart;
        set { _inner.DuckStart = value; OnPropertyChanged(); }
    }

    public string? DuckEnd
    {
        get => _inner.DuckEnd;
        set { _inner.DuckEnd = value; OnPropertyChanged(); }
    }

    public string? DuckVol
    {
        get => _inner.DuckVol;
        set { _inner.DuckVol = value; OnPropertyChanged(); }
    }

    public string? RestoreTime
    {
        get => _inner.RestoreTime;
        set { _inner.RestoreTime = value; OnPropertyChanged(); }
    }

    public bool LoopBool
    {
        get => IsTruthy(_inner.Loop);
        set
        {
            _inner.Loop = value ? "true" : "false";
            OnPropertyChanged(nameof(LoopBool));
            OnPropertyChanged(nameof(_inner.Loop));
        }
    }

    public bool IsStealOldestBool
    {
        get => IsTruthy(_inner.IsStealOldest, true);
        set
        {
            _inner.IsStealOldest = value ? "true" : "false";
            OnPropertyChanged(nameof(IsStealOldestBool));
        }
    }

    public bool IsLastSamplesBool
    {
        get => IsTruthy(_inner.IsLastSamples);
        set
        {
            _inner.IsLastSamples = value ? "true" : "false";
            OnPropertyChanged(nameof(IsLastSamplesBool));
        }
    }

    public bool IsVolRndBool
    {
        get => IsTruthy(_inner.IsVolRnd);
        set
        {
            _inner.IsVolRnd = value ? "true" : "false";
            OnPropertyChanged(nameof(IsVolRndBool));
        }
    }

    public bool IncVolBool
    {
        get => IsTruthy(_inner.IncVol);
        set
        {
            _inner.IncVol = value ? "true" : "false";
            OnPropertyChanged(nameof(IncVolBool));
        }
    }

    public bool IsPitchRndBool
    {
        get => IsTruthy(_inner.IsPitchRnd);
        set
        {
            _inner.IsPitchRnd = value ? "true" : "false";
            OnPropertyChanged(nameof(IsPitchRndBool));
        }
    }

    public bool IncPitchBool
    {
        get => IsTruthy(_inner.IncPitch);
        set
        {
            _inner.IncPitch = value ? "true" : "false";
            OnPropertyChanged(nameof(IncPitchBool));
        }
    }

    public bool IsPanRndBool
    {
        get => IsTruthy(_inner.IsPanRnd);
        set
        {
            _inner.IsPanRnd = value ? "true" : "false";
            OnPropertyChanged(nameof(IsPanRndBool));
        }
    }

    public bool IncPanBool
    {
        get => IsTruthy(_inner.IncPan);
        set
        {
            _inner.IncPan = value ? "true" : "false";
            OnPropertyChanged(nameof(IncPanBool));
        }
    }

    public bool IsRndSrcBool
    {
        get => IsTruthy(_inner.IsRndSrc);
        set
        {
            _inner.IsRndSrc = value ? "true" : "false";
            OnPropertyChanged(nameof(IsRndSrcBool));
        }
    }

    public bool IncSrcBool
    {
        get => IsTruthy(_inner.IncSrc);
        set
        {
            _inner.IncSrc = value ? "true" : "false";
            OnPropertyChanged(nameof(IncSrcBool));
        }
    }

    public bool IsMovePitchBool
    {
        get => IsTruthy(_inner.IsMovePitch);
        set
        {
            _inner.IsMovePitch = value ? "true" : "false";
            OnPropertyChanged(nameof(IsMovePitchBool));
        }
    }

    public bool IsMovePanBool
    {
        get => IsTruthy(_inner.IsMovePan);
        set
        {
            _inner.IsMovePan = value ? "true" : "false";
            OnPropertyChanged(nameof(IsMovePanBool));
        }
    }

    public bool AutoRestoreBool
    {
        get => IsTruthy(_inner.AutoRestore, true);
        set
        {
            _inner.AutoRestore = value ? "true" : "false";
            OnPropertyChanged(nameof(AutoRestoreBool));
        }
    }

    public bool IsAndroidNativeBool
    {
        get => IsTruthy(_inner.IsAndroidNative);
        set
        {
            _inner.IsAndroidNative = value ? "true" : "false";
            OnPropertyChanged(nameof(IsAndroidNativeBool));
        }
    }

    public LabelSet ToLabelSet() => _inner;

    private static bool IsTruthy(string? value, bool defaultValue = false)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
               value == "1";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class CategoryItemViewModel : INotifyPropertyChanged
{
    private readonly CategorySet _inner;

    public CategoryItemViewModel(CategorySet inner)
    {
        _inner = inner;
    }

    public string? CategoryName
    {
        get => _inner.CategoryName;
        set { _inner.CategoryName = value; OnPropertyChanged(); }
    }

    public string? Volume
    {
        get => _inner.Volume;
        set { _inner.Volume = value; OnPropertyChanged(); }
    }

    public string? MaxNum
    {
        get => _inner.MaxNum;
        set { _inner.MaxNum = value; OnPropertyChanged(); }
    }

    public string? MasterName
    {
        get => _inner.MasterName;
        set { _inner.MasterName = value; OnPropertyChanged(); }
    }

    public CategorySet ToCategorySet() => _inner;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class MasterItemViewModel : INotifyPropertyChanged
{
    private readonly MasterSet _inner;

    public MasterItemViewModel(MasterSet inner)
    {
        _inner = inner;
    }

    public string? MasterName
    {
        get => _inner.MasterName;
        set { _inner.MasterName = value; OnPropertyChanged(); }
    }

    public string? Volume
    {
        get => _inner.Volume;
        set { _inner.Volume = value; OnPropertyChanged(); }
    }

    public MasterSet ToMasterSet() => _inner;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute(parameter);

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}


