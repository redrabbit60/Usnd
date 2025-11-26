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
        try
        {
            InitializeComponent();
            var viewModel = new MainViewModel();
            DataContext = viewModel;
            viewModel.SetTabControls(SettingsTabControl, LabelGroupTabControl);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"初期化エラー:\n{ex.Message}\n\nSaveDataフォルダーを削除して再起動してください。",
                "エラー",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
            System.Windows.Application.Current.Shutdown();
        }
    }

    private void SettingsTabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.OnSettingsTabSelectionChanged();
        }
    }

    private void LabelGroupTabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.OnLabelGroupTabSelectionChanged();
        }
    }

    private void LabelGroupTabControl_DragEnter(object sender, System.Windows.DragEventArgs e)
    {
        if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
        {
            var paths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            if (paths != null && paths.Length > 0)
            {
                // WAVファイルまたはフォルダーがあればドロップ可能
                bool hasValidItem = paths.Any(p => 
                    p.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) || 
                    Directory.Exists(p));
                
                e.Effects = hasValidItem ? System.Windows.DragDropEffects.Copy : System.Windows.DragDropEffects.None;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
        }
        else
        {
            e.Effects = System.Windows.DragDropEffects.None;
        }
        e.Handled = true;
    }

    private void LabelGroupTabControl_Drop(object sender, System.Windows.DragEventArgs e)
    {
        if (DataContext is MainViewModel vm && e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
        {
            var paths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            if (paths != null && paths.Length > 0)
            {
                // WAVファイルまたはフォルダーをすべて渡す
                vm.HandleWavFileDrop(paths);
            }
        }
    }
}

public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;
    private string _xmlDir;
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
    private readonly List<string> _statusHistory = new();
    private string _statusMessage = "XML を読み込みました。";
    private int _selectedSettingsTabIndex = 0;
    private int _selectedLabelGroupTabIndex = -1;
    private string _currentSaveFileName = "default";
    private bool _hasUnsavedChanges = false;
    private System.Windows.Controls.TabControl? _settingsTabControl;
    private System.Windows.Controls.TabControl? _labelGroupTabControl;
    private List<LabelGroupData> _labelGroups = new();
    private string? _currentLabelGroupName;
    private const int MaxStatusHistory = 20;

    public ObservableCollection<LabelItemViewModel> LabelItems { get; } = new();
    public ObservableCollection<CategoryItemViewModel> CategoryItems { get; } = new();
    public ObservableCollection<MasterItemViewModel> MasterItems { get; } = new();
    public ObservableCollection<string> SaveFileList { get; } = new();
    public ObservableCollection<LabelItemViewModel> SelectedLabels { get; } = new();
    public PointCollection WaveformPoints { get; private set; } = new();
    public IReadOnlyList<string> CategoryBehaviorOptions { get; } = new[] { "STEAL_OLDEST", "JUST_FAIL", "QUEUE" };

    public int SelectedSettingsTabIndex
    {
        get => _selectedSettingsTabIndex;
        set
        {
            if (_selectedSettingsTabIndex == value) return;
            _selectedSettingsTabIndex = value;
            OnPropertyChanged();
        }
    }

    public int SelectedLabelGroupTabIndex
    {
        get => _selectedLabelGroupTabIndex;
        set
        {
            if (_selectedLabelGroupTabIndex == value) return;
            _selectedLabelGroupTabIndex = value;
            OnPropertyChanged();
        }
    }

    public void SetTabControls(System.Windows.Controls.TabControl settingsTabControl, System.Windows.Controls.TabControl labelGroupTabControl)
    {
        _settingsTabControl = settingsTabControl;
        _labelGroupTabControl = labelGroupTabControl;
        
        // 最後に開いたファイルを読み込む
        if (!string.IsNullOrEmpty(_currentSaveFileName))
        {
            try
            {
                var data = BinaryStore.Load(_currentSaveFileName);
                _master = data.Master;
                _category = data.Category;
                _labels = data.Labels;
                _labelGroups = data.LabelGroups ?? new List<LabelGroupData>();
                RefreshCollections();
            }
            catch (Exception ex)
            {
                // 読み込みエラー時は新規データを作成
                UpdateStatus($"ファイル読み込みエラー: {ex.Message}");
                _master = new MasterSettings();
                _category = new CategorySettings();
                _labels = new LabelSettings();
                _labelGroups = new List<LabelGroupData>();
                RefreshCollections();
            }
        }
        
        RebuildTabs();
        _hasUnsavedChanges = false;
    }

    public string CurrentSaveFileName
    {
        get => _currentSaveFileName;
        set
        {
            if (_currentSaveFileName == value) return;
            
            // 未保存の変更がある場合、確認ダイアログを表示
            if (_hasUnsavedChanges)
            {
                var result = System.Windows.MessageBox.Show(
                    $"'{_currentSaveFileName}' に未保存の変更があります。保存しますか？",
                    "未保存の変更",
                    System.Windows.MessageBoxButton.YesNoCancel,
                    System.Windows.MessageBoxImage.Question);
                
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    SaveToBinary();
                }
                else if (result == System.Windows.MessageBoxResult.Cancel)
                {
                    // キャンセルされた場合は切り替えを中止
                    OnPropertyChanged(); // ComboBoxの選択を元に戻すため
                    return;
                }
                // Noの場合は保存せずに続行
            }
            
            _currentSaveFileName = value;
            OnPropertyChanged();
            LoadFromBinary(value);
            
            // 最後に開いたファイルを保存
            AppSettingsStore.Save(new AppSettings { LastOpenedFile = value });
        }
    }

    public string SelectedBrowserTabName
    {
        get
        {
            if (_selectedLabelGroupTabIndex >= 0)
            {
                return "Label";
            }
            return _selectedSettingsTabIndex switch
            {
                0 => "Master",
                1 => "Category",
                _ => "Settings"
            };
        }
    }

    public string DetailPanelTitle
    {
        get
        {
            if (_selectedLabelGroupTabIndex >= 0)
            {
                return "Label Info";
            }
            return _selectedSettingsTabIndex switch
            {
                0 => "Master Detail",
                1 => "Category Detail",
                _ => "Detail"
            };
        }
    }

    public Visibility MasterDetailVisibility => _selectedSettingsTabIndex == 0 && _selectedLabelGroupTabIndex < 0 ? Visibility.Visible : Visibility.Collapsed;
    public Visibility CategoryDetailVisibility => _selectedSettingsTabIndex == 1 && _selectedLabelGroupTabIndex < 0 ? Visibility.Visible : Visibility.Collapsed;
    public Visibility LabelDetailVisibility => _selectedLabelGroupTabIndex >= 0 ? Visibility.Visible : Visibility.Collapsed;

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
            TogglePlayCommand.RaiseCanExecuteChanged();
            
            // Label選択時に自動的にオーディオ情報を読み込む
            if (value != null)
            {
                LoadAudioInfo(value.FileName);
            }
            else
            {
                AudioInfoText = "Audio file info will appear here";
                WaveformPoints = new PointCollection();
                OnPropertyChanged(nameof(WaveformPoints));
            }
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

    private string _audioInfoText = "Audio file info will appear here";
    public string AudioInfoText
    {
        get => _audioInfoText;
        private set
        {
            if (_audioInfoText == value) return;
            _audioInfoText = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand PlayCommand { get; }
    public RelayCommand StopCommand { get; }
    public RelayCommand TogglePlayCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand ImportXmlCommand { get; }
    public RelayCommand SelectXmlPathCommand { get; }
    public RelayCommand AddMasterCommand { get; }
    public RelayCommand RemoveMasterCommand { get; }
    public RelayCommand AddCategoryCommand { get; }
    public RelayCommand RemoveCategoryCommand { get; }
    public RelayCommand AddLabelCommand { get; }
    public RelayCommand RemoveLabelCommand { get; }
    public RelayCommand BulkEditCommand { get; }
    public RelayCommand AddLabelGroupCommand { get; }
    public RelayCommand RemoveLabelGroupCommand { get; }
    public RelayCommand NewFileCommand { get; }
    public RelayCommand SaveFileCommand { get; }
    public RelayCommand DeleteFileCommand { get; }

    public MainViewModel()
    {
        try
        {
            _xmlDir = Path.GetFullPath(Path.Combine(_baseDir, "..", "..", "..", "xml"));
            _masterPath = Path.Combine(_xmlDir, "MasterSettings.xml");
            _categoryPath = Path.Combine(_xmlDir, "CategorySettings.xml");
            _labelPath = Path.Combine(_xmlDir, "LabelSettings.xml");

            _master = XmlStore.Load<MasterSettings>(_masterPath);
            _category = XmlStore.Load<CategorySettings>(_categoryPath);
            _labels = XmlStore.Load<LabelSettings>(_labelPath);
            
            RefreshSaveFileList();
            
            // 最後に開いたファイルを自動的に読み込む（SetTabControls呼び出し後に実行するため、フラグだけ設定）
            try
            {
                var appSettings = AppSettingsStore.Load();
                if (SaveFileList.Contains(appSettings.LastOpenedFile))
                {
                    _currentSaveFileName = appSettings.LastOpenedFile;
                }
                else if (SaveFileList.Count > 0)
                {
                    _currentSaveFileName = SaveFileList[0];
                }
            }
            catch
            {
                // エラー時はデフォルトファイルを使用
                _currentSaveFileName = SaveFileList.Count > 0 ? SaveFileList[0] : "default";
            }
            
            RefreshCollections();
            SetupChangeTracking();
        }
        catch
        {
            // 初期化エラー時は最小限のデータで起動
            _xmlDir = Path.Combine(_baseDir, "xml");
            _masterPath = Path.Combine(_xmlDir, "MasterSettings.xml");
            _categoryPath = Path.Combine(_xmlDir, "CategorySettings.xml");
            _labelPath = Path.Combine(_xmlDir, "LabelSettings.xml");
            
            _master = new MasterSettings();
            _category = new CategorySettings();
            _labels = new LabelSettings();
            _currentSaveFileName = "default";
            
            SaveFileList.Add("default");
        }

        PlayCommand = new RelayCommand(_ => PlaySelected(), _ => SelectedLabel != null);
        StopCommand = new RelayCommand(_ => StopAudio(), _ => true);
        TogglePlayCommand = new RelayCommand(_ => TogglePlay(), _ => SelectedLabel != null);
        SaveCommand = new RelayCommand(_ => SaveXml());
        ImportXmlCommand = new RelayCommand(_ => ImportXml());
        SelectXmlPathCommand = new RelayCommand(_ => SelectXmlPath());
        AddMasterCommand = new RelayCommand(_ => AddMaster());
        RemoveMasterCommand = new RelayCommand(_ => RemoveMaster(), _ => SelectedMaster != null);
        AddCategoryCommand = new RelayCommand(_ => AddCategory());
        RemoveCategoryCommand = new RelayCommand(_ => RemoveCategory(), _ => SelectedCategory != null);
        AddLabelCommand = new RelayCommand(_ => AddLabel());
        RemoveLabelCommand = new RelayCommand(_ => RemoveLabel(), _ => SelectedLabel != null);
        BulkEditCommand = new RelayCommand(_ => BulkEditLabels(), _ => SelectedLabels.Count > 0);
        AddLabelGroupCommand = new RelayCommand(_ => AddLabelGroup());
        RemoveLabelGroupCommand = new RelayCommand(_ => RemoveLabelGroup(), _ => CanRemoveLabelGroup());
        NewFileCommand = new RelayCommand(_ => CreateNewFile());
        SaveFileCommand = new RelayCommand(_ => SaveToBinary());
        DeleteFileCommand = new RelayCommand(_ => DeleteCurrentFile());
    }

    private void SetupChangeTracking()
    {
        // コレクションの変更を監視
        LabelItems.CollectionChanged += (s, e) => MarkAsChanged();
        CategoryItems.CollectionChanged += (s, e) => MarkAsChanged();
        MasterItems.CollectionChanged += (s, e) => MarkAsChanged();
        
        // 各アイテムのPropertyChanged を監視
        foreach (var item in LabelItems)
        {
            item.PropertyChanged += (s, e) => MarkAsChanged();
        }
        foreach (var item in CategoryItems)
        {
            item.PropertyChanged += (s, e) => MarkAsChanged();
        }
        foreach (var item in MasterItems)
        {
            item.PropertyChanged += (s, e) => MarkAsChanged();
        }
    }

    private void MarkAsChanged()
    {
        _hasUnsavedChanges = true;
    }

    private void RefreshCollections()
    {
        LabelItems.Clear();
        foreach (var item in _labels.Items)
        {
            var vm = new LabelItemViewModel(item);
            vm.PropertyChanged += (s, e) => MarkAsChanged();
            LabelItems.Add(vm);
        }

        if (LabelItems.Count > 0)
        {
            SelectedLabel = LabelItems[0];
        }

        CategoryItems.Clear();
        foreach (var c in _category.Items)
        {
            var vm = new CategoryItemViewModel(c);
            vm.PropertyChanged += (s, e) => MarkAsChanged();
            CategoryItems.Add(vm);
        }
        if (CategoryItems.Count > 0)
        {
            SelectedCategory = CategoryItems[0];
        }

        MasterItems.Clear();
        foreach (var m in _master.Items)
        {
            var vm = new MasterItemViewModel(m);
            vm.PropertyChanged += (s, e) => MarkAsChanged();
            MasterItems.Add(vm);
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

        var fileCandidates = new List<string>
        {
            Path.Combine(_baseDir, SelectedLabel.FileName ?? string.Empty),
            Path.Combine(_baseDir, "Audio", SelectedLabel.FileName ?? string.Empty),
            Path.GetFullPath(Path.Combine(_xmlDir, "..", "Audio", SelectedLabel.FileName ?? string.Empty))
        };

        // 各Label Groupフォルダー内も検索
        foreach (var group in _labelGroups)
        {
            fileCandidates.Add(Path.Combine(_baseDir, "Audio", group.GroupName, SelectedLabel.FileName ?? string.Empty));
        }

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

    private void TogglePlay()
    {
        if (_player.IsPlaying)
        {
            StopAudio();
        }
        else
        {
            PlaySelected();
        }
    }

    private void SelectXmlPath()
    {
        var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "XML書き出し先フォルダーを選択してください",
            ShowNewFolderButton = true,
            SelectedPath = _xmlDir
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            _xmlDir = dialog.SelectedPath;
            UpdateStatus($"書き出し先を設定: {_xmlDir}");
        }
    }

    private void SaveXml()
    {
        try
        {
            UpdateStatus($"書き出し先: {_xmlDir}");

            // MasterSettings.xml
            _master.Items = MasterItems.Select(m => m.ToMasterSet()).ToList();
            var masterPath = Path.Combine(_xmlDir, "MasterSettings.xml");
            XmlStore.Save(masterPath, _master);
            UpdateStatus("MasterSettings.xml を保存しました。");

            // CategorySettings.xml
            _category.Items = CategoryItems.Select(c => c.ToCategorySet()).ToList();
            var categoryPath = Path.Combine(_xmlDir, "CategorySettings.xml");
            XmlStore.Save(categoryPath, _category);
            UpdateStatus("CategorySettings.xml を保存しました。");

            // 現在のLabel Group名を記録（大文字小文字を区別）
            var currentGroupNames = new HashSet<string>(_labelGroups.Select(g => g.GroupName));

            // 既存のXMLファイルをチェックして、Label Groupsに存在しないものを削除
            if (Directory.Exists(_xmlDir))
            {
                var existingXmlFiles = Directory.GetFiles(_xmlDir, "*.xml");
                foreach (var xmlFile in existingXmlFiles)
                {
                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(xmlFile);
                    
                    // MasterSettings.xmlとCategorySettings.xmlは削除対象外
                    if (fileNameWithoutExt.Equals("MasterSettings", StringComparison.Ordinal) ||
                        fileNameWithoutExt.Equals("CategorySettings", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    // 現在のLabel Groupsに存在しない場合は削除（大文字小文字を区別）
                    if (!currentGroupNames.Contains(fileNameWithoutExt))
                    {
                        File.Delete(xmlFile);
                        UpdateStatus($"削除: {fileNameWithoutExt}.xml (Label Groupsに存在しないため)");
                    }
                }
            }

            // 各Label Groupごとに個別のXMLファイルを書き出す
            foreach (var group in _labelGroups)
            {
                var groupLabels = LabelItems
                    .Where(l => group.LabelNames.Contains(l.LabelName ?? string.Empty))
                    .Select(l => l.ToLabelSet())
                    .ToList();

                var labelSettings = new LabelSettings { Items = groupLabels };
                // 大文字小文字を保持したままファイル名を生成
                var groupPath = Path.Combine(_xmlDir, $"{group.GroupName}.xml");
                XmlStore.Save(groupPath, labelSettings);
                UpdateStatus($"{group.GroupName}.xml を保存しました ({groupLabels.Count} labels)。");
            }

            UpdateStatus("すべてのXMLファイルを保存しました。");
        }
        catch (Exception ex)
        {
            UpdateStatus($"XML保存エラー: {ex.Message}");
        }
    }

    private void ImportXml()
    {
        try
        {
            var dialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Multiselect = true,
                InitialDirectory = _xmlDir,
                Title = "インポートするXMLファイルを選択してください"
            };

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                UpdateStatus("XMLインポートをキャンセルしました。");
                return;
            }

            int importedCount = 0;
            foreach (var filePath in dialog.FileNames)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);

                // MasterSettings.xml の処理
                if (fileName.Equals("MasterSettings", StringComparison.OrdinalIgnoreCase))
                {
                    var masterSettings = XmlStore.Load<MasterSettings>(filePath);
                    MasterItems.Clear();
                    foreach (var item in masterSettings.Items)
                    {
                        var vm = new MasterItemViewModel(item);
                        vm.PropertyChanged += (s, e) => MarkAsChanged();
                        MasterItems.Add(vm);
                    }
                    if (MasterItems.Count > 0)
                    {
                        SelectedMaster = MasterItems[0];
                    }
                    UpdateStatus($"MasterSettings.xml をインポートしました ({masterSettings.Items.Count} items)。");
                    importedCount++;
                    continue;
                }

                // CategorySettings.xml の処理
                if (fileName.Equals("CategorySettings", StringComparison.OrdinalIgnoreCase))
                {
                    var categorySettings = XmlStore.Load<CategorySettings>(filePath);
                    CategoryItems.Clear();
                    foreach (var item in categorySettings.Items)
                    {
                        var vm = new CategoryItemViewModel(item);
                        vm.PropertyChanged += (s, e) => MarkAsChanged();
                        CategoryItems.Add(vm);
                    }
                    if (CategoryItems.Count > 0)
                    {
                        SelectedCategory = CategoryItems[0];
                    }
                    UpdateStatus($"CategorySettings.xml をインポートしました ({categorySettings.Items.Count} items)。");
                    importedCount++;
                    continue;
                }

                // Label Group XMLファイルの処理
                var labelSettings = XmlStore.Load<LabelSettings>(filePath);
                if (labelSettings.Items.Count > 0)
                {
                    // 既存のLabel Groupを検索
                    var existingGroup = _labelGroups.FirstOrDefault(g => 
                        g.GroupName.Equals(fileName, StringComparison.OrdinalIgnoreCase));

                    if (existingGroup == null)
                    {
                        // 新しいLabel Groupを作成
                        var newGroup = new LabelGroupData
                        {
                            GroupName = fileName,
                            LabelNames = new List<string>()
                        };
                        _labelGroups.Add(newGroup);
                        UpdateStatus($"新しいLabel Group '{fileName}' を作成しました。");
                        existingGroup = newGroup;
                    }
                    else
                    {
                        // 既存のグループのラベルをクリア
                        var labelsToRemove = LabelItems
                            .Where(l => existingGroup.LabelNames.Contains(l.LabelName ?? string.Empty))
                            .ToList();
                        foreach (var label in labelsToRemove)
                        {
                            LabelItems.Remove(label);
                        }
                        existingGroup.LabelNames.Clear();
                        UpdateStatus($"既存のLabel Group '{fileName}' をクリアしました。");
                    }

                    // Labelをインポート
                    foreach (var labelSet in labelSettings.Items)
                    {
                        var vm = new LabelItemViewModel(labelSet);
                        vm.PropertyChanged += (s, e) => MarkAsChanged();
                        LabelItems.Add(vm);
                        existingGroup.LabelNames.Add(labelSet.LabelName ?? string.Empty);
                    }

                    UpdateStatus($"{fileName}.xml をインポートしました ({labelSettings.Items.Count} labels)。");
                    importedCount++;
                }
            }

            if (importedCount > 0)
            {
                // UIタブを再構築
                RebuildTabs();
                
                UpdateStatus($"合計 {importedCount} 個のXMLファイルをインポートしました。");
                MarkAsChanged();
            }
            else
            {
                UpdateStatus("インポート可能なデータが見つかりませんでした。");
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"XMLインポートエラー: {ex.Message}");
            System.Windows.MessageBox.Show($"XMLインポート中にエラーが発生しました:\n{ex.Message}", 
                "エラー", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private void AddMaster()
    {
        var newMaster = new MasterItemViewModel(new MasterSet
        {
            MasterName = $"Master_{MasterItems.Count + 1}",
            Volume = "1"
        });
        newMaster.PropertyChanged += (s, e) => MarkAsChanged();
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
        newCategory.PropertyChanged += (s, e) => MarkAsChanged();
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

    private void AddLabel()
    {
        // 現在のLabelグループを取得
        if (_currentLabelGroupName == null)
        {
            UpdateStatus("Label Groupを選択してください");
            return;
        }
        
        var group = _labelGroups.FirstOrDefault(g => g.GroupName == _currentLabelGroupName);
        if (group == null) return;
        
        // ユニークなIDを生成してLabelNameとして一時的に使用
        var tempId = $"_temp_{Guid.NewGuid():N}";
        
        var newLabel = new LabelItemViewModel(new LabelSet
        {
            LabelName = string.Empty,
            FileName = string.Empty,
            Volume = "1",
            Loop = "FALSE",
            CategoryBehavior = "STEAL_OLDEST",
            Priority = "64",
            CategoryName = string.Empty,
            SingleGroup = string.Empty,
            MaxNum = "0",
            IsStealOldest = "FALSE",
            UnityMixerName = string.Empty,
            SpatialGroup = string.Empty,
            Delay = "0",
            Interval = "0",
            Pan = "0",
            Pitch = "0",
            IsLastSamples = "FALSE",
            FadeInTime = "0",
            FadeOutTime = "0",
            FadeInOldSample = "0",
            FadeOutOnPause = "0",
            FadeInOffPause = "0",
            IsVolRnd = "FALSE",
            IncVol = "FALSE",
            VolRndMin = "0",
            VolRndMax = "0",
            VolRndUnit = "0",
            IsPitchRnd = "FALSE",
            IncPitch = "FALSE",
            PitchRndMin = "0",
            PitchRndMax = "0",
            PitchRndUnit = "0",
            IsPanRnd = "FALSE",
            IncPan = "FALSE",
            PanRndMin = "0",
            PanRndMax = "0",
            PanRndUnit = "0",
            IsRndSrc = "FALSE",
            IncSrc = "FALSE",
            RndSrc = string.Empty,
            IsMovePitch = "FALSE",
            PitchStart = "0",
            PitchEnd = "0",
            PitchMoveTime = "0",
            IsMovePan = "FALSE",
            PanStart = "0",
            PanEnd = "0",
            PanMoveTime = "0",
            DuckingCategory = string.Empty,
            DuckStart = "0",
            DuckEnd = "0",
            DuckVol = "1",
            AutoRestore = "FALSE",
            RestoreTime = "0",
            IsAndroidNative = "FALSE"
        });
        
        // このLabel専用の追跡用クラスを作成
        var tracker = new LabelNameTracker { CurrentName = tempId, GroupName = _currentLabelGroupName };
        
        newLabel.PropertyChanged += (s, e) => 
        {
            MarkAsChanged();
            
            // LabelNameが変更されたときにグループを更新
            if (e.PropertyName == nameof(LabelItemViewModel.LabelName))
            {
                var currentGroup = _labelGroups.FirstOrDefault(g => g.GroupName == tracker.GroupName);
                if (currentGroup != null)
                {
                    // 古いLabelNameを削除
                    currentGroup.LabelNames.Remove(tracker.CurrentName);
                    
                    // 新しいLabelNameを追加
                    var newLabelName = newLabel.LabelName ?? string.Empty;
                    currentGroup.LabelNames.Add(newLabelName);
                    tracker.CurrentName = newLabelName;
                    
                    RebuildTabs();
                }
            }
        };
        
        LabelItems.Add(newLabel);
        
        // グループに一時IDを追加
        group.LabelNames.Add(tempId);
        
        // タブを再構築してリストを更新
        RebuildTabs();
        
        SelectedLabel = newLabel;
        UpdateStatus($"Label を '{_currentLabelGroupName}' グループに追加しました");
    }

    private void RemoveLabel()
    {
        if (SelectedLabel == null) return;

        var name = SelectedLabel.LabelName ?? string.Empty;
        var index = LabelItems.IndexOf(SelectedLabel);
        
        // 全てのグループから削除
        foreach (var group in _labelGroups)
        {
            if (group.LabelNames.Contains(name))
            {
                group.LabelNames.Remove(name);
            }
            // 一時IDの場合も削除（_temp_で始まる）
            var tempIds = group.LabelNames.Where(ln => ln.StartsWith("_temp_")).ToList();
            foreach (var tempId in tempIds)
            {
                var matchingLabel = LabelItems.FirstOrDefault(l => 
                    string.IsNullOrEmpty(l.LabelName) && group.LabelNames.Contains(tempId));
                if (matchingLabel == SelectedLabel)
                {
                    group.LabelNames.Remove(tempId);
                }
            }
        }
        
        LabelItems.Remove(SelectedLabel);

        if (LabelItems.Count > 0)
        {
            SelectedLabel = LabelItems[Math.Min(index, LabelItems.Count - 1)];
        }
        else
        {
            SelectedLabel = null;
        }
        
        // タブを再構築
        RebuildTabs();
        
        UpdateStatus($"Label を削除: {name}");
        RemoveLabelCommand.RaiseCanExecuteChanged();
    }

    private void BulkEditLabels()
    {
        if (SelectedLabels.Count == 0)
        {
            UpdateStatus("編集するラベルを選択してください。");
            return;
        }

        var dialog = new BulkEditDialog
        {
            Owner = System.Windows.Application.Current.MainWindow,
            SelectionCount = SelectedLabels.Count
        };

        if (dialog.ShowDialog() == true)
        {
            int count = 0;
            foreach (var label in SelectedLabels.ToList())
            {
                if (dialog.ApplyVolume && !string.IsNullOrWhiteSpace(dialog.VolumeValue))
                {
                    label.Volume = dialog.VolumeValue;
                }
                if (dialog.ApplyCategoryName)
                {
                    label.CategoryName = dialog.CategoryNameValue;
                }
                if (dialog.ApplyPriority && !string.IsNullOrWhiteSpace(dialog.PriorityValue))
                {
                    label.Priority = dialog.PriorityValue;
                }
                if (dialog.ApplyLoop && !string.IsNullOrWhiteSpace(dialog.LoopValue))
                {
                    label.Loop = dialog.LoopValue;
                }
                if (dialog.ApplyPan && !string.IsNullOrWhiteSpace(dialog.PanValue))
                {
                    label.Pan = dialog.PanValue;
                }
                if (dialog.ApplyPitch && !string.IsNullOrWhiteSpace(dialog.PitchValue))
                {
                    label.Pitch = dialog.PitchValue;
                }
                count++;
            }

            MarkAsChanged();
            UpdateStatus($"{count} 個のラベルを一括編集しました。");
        }
    }

    private void RefreshSaveFileList()
    {
        SaveFileList.Clear();
        var files = BinaryStore.GetSaveFileList();
        
        if (files.Count == 0)
        {
            // デフォルトファイルを作成
            var defaultData = new SaveData
            {
                Master = _master,
                Category = _category,
                Labels = _labels,
                LabelGroups = new List<LabelGroupData>()
            };
            BinaryStore.Save("default", defaultData);
            SaveFileList.Add("default");
        }
        else
        {
            foreach (var file in files)
            {
                SaveFileList.Add(file);
            }
        }
        
        CurrentSaveFileName = SaveFileList.FirstOrDefault() ?? "default";
    }

    private void LoadFromBinary(string fileName)
    {
        var data = BinaryStore.Load(fileName);
        _master = data.Master;
        _category = data.Category;
        _labels = data.Labels;
        _labelGroups = data.LabelGroups ?? new List<LabelGroupData>();
        
        RefreshCollections();
        RebuildTabs();
        _hasUnsavedChanges = false; // 読み込み後は未保存フラグをリセット
        UpdateStatus($"ファイルを読み込みました: {fileName}");
    }

    private void SaveToBinary()
    {
        var data = new SaveData
        {
            Master = new MasterSettings { Items = MasterItems.Select(m => m.ToMasterSet()).ToList() },
            Category = new CategorySettings { Items = CategoryItems.Select(c => c.ToCategorySet()).ToList() },
            Labels = new LabelSettings { Items = LabelItems.Select(l => l.ToLabelSet()).ToList() },
            LabelGroups = _labelGroups
        };
        
        BinaryStore.Save(CurrentSaveFileName, data);
        _hasUnsavedChanges = false; // 保存後は未保存フラグをリセット
        UpdateStatus($"ファイルを保存しました: {CurrentSaveFileName}");
    }

    private void CreateNewFile()
    {
        var window = new NewFileDialog { Owner = System.Windows.Application.Current.MainWindow };
        if (window.ShowDialog() == true)
        {
            var fileName = window.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                UpdateStatus("ファイル名が無効です。");
                return;
            }
            
            if (SaveFileList.Contains(fileName))
            {
                UpdateStatus($"ファイル '{fileName}' は既に存在します。");
                return;
            }
            
            var newData = new SaveData
            {
                Master = new MasterSettings(),
                Category = new CategorySettings(),
                Labels = new LabelSettings(),
                LabelGroups = new List<LabelGroupData>()
            };
            
            BinaryStore.Save(fileName, newData);
            SaveFileList.Add(fileName);
            CurrentSaveFileName = fileName;
            UpdateStatus($"新規ファイルを作成: {fileName}");
        }
    }

    private void DeleteCurrentFile()
    {
        if (SaveFileList.Count <= 1)
        {
            UpdateStatus("最後のファイルは削除できません。");
            return;
        }
        
        var result = System.Windows.MessageBox.Show(
            $"ファイル '{CurrentSaveFileName}' を削除してもよろしいですか？",
            "削除確認",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);
        
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            var fileName = CurrentSaveFileName;
            BinaryStore.Delete(fileName);
            SaveFileList.Remove(fileName);
            CurrentSaveFileName = SaveFileList.FirstOrDefault() ?? "default";
            UpdateStatus($"ファイルを削除しました: {fileName}");
        }
    }

    private void RebuildTabs()
    {
        if (_labelGroupTabControl == null) return;

        // 現在選択されているタブのインデックスを保存
        var currentSelectedIndex = _labelGroupTabControl.SelectedIndex;
        var currentGroupName = _currentLabelGroupName;

        _labelGroupTabControl.Items.Clear();

        // 動的 Label グループタブ
        foreach (var group in _labelGroups)
        {
            _labelGroupTabControl.Items.Add(CreateLabelGroupTab(group));
        }

        // タブ選択を復元
        if (currentSelectedIndex >= 0 && currentSelectedIndex < _labelGroupTabControl.Items.Count)
        {
            _labelGroupTabControl.SelectedIndex = currentSelectedIndex;
        }
        
        // _currentLabelGroupNameを復元
        _currentLabelGroupName = currentGroupName;
    }

    private System.Windows.Controls.TabItem CreateLabelGroupTab(LabelGroupData group)
    {
        var tab = new System.Windows.Controls.TabItem();
        
        // 編集可能なヘッダー（ダブルクリックで編集）
        var headerPanel = new System.Windows.Controls.StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal };
        
        var headerTextBlock = new System.Windows.Controls.TextBlock
        {
            Text = group.GroupName,
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(33, 33, 33)),
            MinWidth = 50,
            Padding = new Thickness(2),
            VerticalAlignment = VerticalAlignment.Center
        };
        
        var headerTextBox = new System.Windows.Controls.TextBox
        {
            Text = group.GroupName,
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            BorderThickness = new Thickness(1),
            BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 100, 100)),
            Background = System.Windows.Media.Brushes.White,
            Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(33, 33, 33)),
            MinWidth = 50,
            Padding = new Thickness(2),
            VerticalAlignment = VerticalAlignment.Center,
            Visibility = Visibility.Collapsed
        };
        
        // ダブルクリックで編集モードに切り替え
        headerTextBlock.MouseLeftButtonDown += (s, e) =>
        {
            if (e.ClickCount == 2)
            {
                headerTextBlock.Visibility = Visibility.Collapsed;
                headerTextBox.Visibility = Visibility.Visible;
                headerTextBox.Focus();
                headerTextBox.SelectAll();
            }
        };
        
        // フォーカスが外れたら表示モードに戻す
        headerTextBox.LostFocus += (s, e) =>
        {
            group.GroupName = headerTextBox.Text;
            headerTextBlock.Text = headerTextBox.Text;
            headerTextBox.Visibility = Visibility.Collapsed;
            headerTextBlock.Visibility = Visibility.Visible;
            MarkAsChanged();
        };
        
        // Enterキーで確定
        headerTextBox.KeyDown += (s, e) =>
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                group.GroupName = headerTextBox.Text;
                headerTextBlock.Text = headerTextBox.Text;
                headerTextBox.Visibility = Visibility.Collapsed;
                headerTextBlock.Visibility = Visibility.Visible;
                MarkAsChanged();
            }
        };
        
        headerPanel.Children.Add(headerTextBlock);
        headerPanel.Children.Add(headerTextBox);
        tab.Header = headerPanel;

        var scrollViewer = new System.Windows.Controls.ScrollViewer { VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto };
        var stackPanel = new System.Windows.Controls.StackPanel();

        // Header with +/- buttons
        var buttonPanel = new System.Windows.Controls.StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
        var labelText = new System.Windows.Controls.TextBlock
        {
            Text = "Label List",
            FontSize = 14,
            Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(245, 246, 255)),
            VerticalAlignment = VerticalAlignment.Center
        };
        var addBtn = new System.Windows.Controls.Button
        {
            Content = "+",
            Width = 28,
            Height = 28,
            Margin = new Thickness(12, 0, 4, 0),
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            Padding = new Thickness(0),
            VerticalAlignment = VerticalAlignment.Center,
            Command = AddLabelCommand
        };
        var removeBtn = new System.Windows.Controls.Button
        {
            Content = "-",
            Width = 28,
            Height = 28,
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            Padding = new Thickness(0),
            VerticalAlignment = VerticalAlignment.Center,
            Command = RemoveLabelCommand
        };
        buttonPanel.Children.Add(labelText);
        buttonPanel.Children.Add(addBtn);
        buttonPanel.Children.Add(removeBtn);

        // ListView - このグループに属するLabelのみ表示
        var listView = new System.Windows.Controls.ListView
        {
            Background = System.Windows.Media.Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Foreground = System.Windows.Media.Brushes.White,
            SelectionMode = System.Windows.Controls.SelectionMode.Extended // 複数選択を有効化
        };
        
        // ItemTemplate を作成
        var dataTemplate = new System.Windows.DataTemplate();
        var borderFactory = new System.Windows.FrameworkElementFactory(typeof(System.Windows.Controls.Border));
        borderFactory.SetValue(System.Windows.Controls.Border.BackgroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromRgb(35, 47, 65)));
        borderFactory.SetValue(System.Windows.Controls.Border.CornerRadiusProperty, new CornerRadius(8));
        borderFactory.SetValue(System.Windows.Controls.Border.MarginProperty, new Thickness(0, 2, 0, 0));
        borderFactory.SetValue(System.Windows.Controls.Border.PaddingProperty, new Thickness(8, 6, 8, 6));
        
        var textBlockFactory = new System.Windows.FrameworkElementFactory(typeof(System.Windows.Controls.TextBlock));
        textBlockFactory.SetValue(System.Windows.Controls.TextBlock.ForegroundProperty, System.Windows.Media.Brushes.White);
        textBlockFactory.SetValue(System.Windows.Controls.TextBlock.FontSizeProperty, 13.0);
        
        // Run要素を使った複雑な表示を実現するため、Inlinesを構築
        var run1Factory = new System.Windows.FrameworkElementFactory(typeof(System.Windows.Documents.Run));
        run1Factory.SetBinding(System.Windows.Documents.Run.TextProperty, new System.Windows.Data.Binding("DisplayName") { Mode = System.Windows.Data.BindingMode.OneWay });
        run1Factory.SetValue(System.Windows.Documents.Run.FontWeightProperty, FontWeights.SemiBold);
        
        var run2Factory = new System.Windows.FrameworkElementFactory(typeof(System.Windows.Documents.Run));
        run2Factory.SetValue(System.Windows.Documents.Run.TextProperty, " / ");
        run2Factory.SetValue(System.Windows.Documents.Run.ForegroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 144, 168)));
        
        var run3Factory = new System.Windows.FrameworkElementFactory(typeof(System.Windows.Documents.Run));
        run3Factory.SetBinding(System.Windows.Documents.Run.TextProperty, new System.Windows.Data.Binding("DisplayFileName") { Mode = System.Windows.Data.BindingMode.OneWay });
        run3Factory.SetValue(System.Windows.Documents.Run.ForegroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromRgb(182, 192, 216)));
        
        textBlockFactory.AppendChild(run1Factory);
        textBlockFactory.AppendChild(run2Factory);
        textBlockFactory.AppendChild(run3Factory);
        
        borderFactory.AppendChild(textBlockFactory);
        dataTemplate.VisualTree = borderFactory;
        listView.ItemTemplate = dataTemplate;
        
        // フィルター済みのLabelItemsをバインド
        // このグループに属するLabelを表示
        var filteredLabels = new ObservableCollection<LabelItemViewModel>();
        
        // group.LabelNamesのインデックスを保持してLabelItemsをマッチング
        foreach (var labelName in group.LabelNames)
        {
            LabelItemViewModel? matchingLabel = null;
            
            if (labelName.StartsWith("_temp_"))
            {
                // 一時IDの場合、まだLabelNameが設定されていないLabelを探す
                matchingLabel = LabelItems.FirstOrDefault(l => string.IsNullOrEmpty(l.LabelName) && !filteredLabels.Contains(l));
            }
            else
            {
                // 通常のLabelName
                matchingLabel = LabelItems.FirstOrDefault(l => l.LabelName == labelName);
            }
            
            if (matchingLabel != null && !filteredLabels.Contains(matchingLabel))
            {
                filteredLabels.Add(matchingLabel);
            }
        }
        
        listView.ItemsSource = filteredLabels;
        listView.SetBinding(System.Windows.Controls.ListView.SelectedItemProperty, new System.Windows.Data.Binding("SelectedLabel") { Mode = System.Windows.Data.BindingMode.TwoWay });
        
        // 複数選択の変更を監視
        listView.SelectionChanged += (s, e) =>
        {
            SelectedLabels.Clear();
            foreach (var item in listView.SelectedItems)
            {
                if (item is LabelItemViewModel labelItem)
                {
                    SelectedLabels.Add(labelItem);
                }
            }
            BulkEditCommand.RaiseCanExecuteChanged();
        };

        stackPanel.Children.Add(buttonPanel);
        stackPanel.Children.Add(listView);
        scrollViewer.Content = stackPanel;
        tab.Content = scrollViewer;
        return tab;
    }

    public void OnSettingsTabSelectionChanged()
    {
        // Label Groupタブの選択を解除
        _selectedLabelGroupTabIndex = -1;
        if (_labelGroupTabControl != null)
        {
            _labelGroupTabControl.SelectedIndex = -1;
        }
        
        _currentLabelGroupName = null;
        
        OnPropertyChanged(nameof(MasterDetailVisibility));
        OnPropertyChanged(nameof(CategoryDetailVisibility));
        OnPropertyChanged(nameof(LabelDetailVisibility));
        OnPropertyChanged(nameof(DetailPanelTitle));
    }

    public void OnLabelGroupTabSelectionChanged()
    {
        // Settingsタブの選択を解除
        if (_settingsTabControl != null)
        {
            // 選択を解除しない（視覚的には残す）
        }
        
        if (_labelGroupTabControl == null) return;
        
        var selectedIndex = _labelGroupTabControl.SelectedIndex;
        if (selectedIndex >= 0 && selectedIndex < _labelGroups.Count)
        {
            _currentLabelGroupName = _labelGroups[selectedIndex].GroupName;
        }
        else
        {
            _currentLabelGroupName = null;
        }
        
        OnPropertyChanged(nameof(MasterDetailVisibility));
        OnPropertyChanged(nameof(CategoryDetailVisibility));
        OnPropertyChanged(nameof(LabelDetailVisibility));
        OnPropertyChanged(nameof(DetailPanelTitle));
    }

    private void AddLabelGroup()
    {
        var window = new NewFileDialog { Owner = System.Windows.Application.Current.MainWindow, Title = "新規Label Group作成" };
        if (window.ShowDialog() == true)
        {
            var groupName = window.FileName;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                UpdateStatus("グループ名が無効です。");
                return;
            }
            
            if (_labelGroups.Any(g => g.GroupName == groupName))
            {
                UpdateStatus($"グループ '{groupName}' は既に存在します。");
                return;
            }
            
            _labelGroups.Add(new LabelGroupData { GroupName = groupName, LabelNames = new List<string>() });
            RebuildTabs();
            MarkAsChanged();
            UpdateStatus($"Label Group '{groupName}' を追加しました");
        }
    }

    private void RemoveLabelGroup()
    {
        if (_labelGroupTabControl == null || _currentLabelGroupName == null) return;
        
        var group = _labelGroups.FirstOrDefault(g => g.GroupName == _currentLabelGroupName);
        if (group == null) return;
        
        var result = System.Windows.MessageBox.Show(
            $"Label Group '{group.GroupName}' を削除してもよろしいですか？\n(グループ内のLabelは削除されません)",
            "削除確認",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);
        
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            _labelGroups.Remove(group);
            RebuildTabs();
            MarkAsChanged();
            UpdateStatus($"Label Group '{group.GroupName}' を削除しました");
        }
    }

    private bool CanRemoveLabelGroup()
    {
        return _currentLabelGroupName != null;
    }

    public void HandleWavFileDrop(string[] paths)
    {
        // 現在選択されているグループに追加
        if (string.IsNullOrEmpty(_currentLabelGroupName))
        {
            UpdateStatus("Label Groupを選択してください");
            return;
        }

        var group = _labelGroups.FirstOrDefault(g => g.GroupName == _currentLabelGroupName);
        if (group == null) return;

        var groupName = _currentLabelGroupName;

        // Audio/[GroupName]フォルダーを作成
        var audioFolder = Path.Combine(_baseDir, "Audio", groupName);
        Directory.CreateDirectory(audioFolder);

        // ドロップされたパスからすべてのWAVファイルを収集
        var allWavFiles = new List<string>();
        foreach (var path in paths)
        {
            if (Directory.Exists(path))
            {
                // フォルダーの場合：サブフォルダーも含めてすべての.wavファイルを取得
                var wavFilesInFolder = Directory.GetFiles(path, "*.wav", SearchOption.AllDirectories);
                allWavFiles.AddRange(wavFilesInFolder);
                UpdateStatus($"フォルダー検出: {Path.GetFileName(path)} ({wavFilesInFolder.Length} files)");
            }
            else if (File.Exists(path) && path.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                // ファイルの場合：.wavファイルのみ追加
                allWavFiles.Add(path);
            }
        }

        if (allWavFiles.Count == 0)
        {
            UpdateStatus("WAVファイルが見つかりませんでした。");
            return;
        }

        UpdateStatus($"{allWavFiles.Count} 個のWAVファイルを処理します...");

        foreach (var wavFile in allWavFiles)
        {
            // ファイル名（拡張子なし）を取得
            var fileName = Path.GetFileName(wavFile);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(wavFile);

            // Audio/[GroupName]フォルダーにコピー
            var destPath = Path.Combine(audioFolder, fileName);
            try
            {
                File.Copy(wavFile, destPath, overwrite: true);
            }
            catch (Exception ex)
            {
                UpdateStatus($"ファイルコピーエラー: {ex.Message}");
                continue;
            }

            // WAVファイルのループ情報を検知
            bool hasLoop = ReadWavLoopInfo(destPath);

            UpdateStatus($"追加: {fileName} (Loop: {(hasLoop ? "Yes" : "No")})");

            // 一時的なユニークIDを生成
            var tempId = $"_temp_{Guid.NewGuid():N}";

            var newLabel = new LabelItemViewModel(new LabelSet
            {
                LabelName = string.Empty, // LabelNameは空（Non）
                FileName = fileName, // FileNameはファイル名のみ
                Volume = "1",
                Loop = hasLoop ? "true" : "false", // ループ情報を自動設定
                CategoryBehavior = "STEAL_OLDEST",
                Priority = "64",
                CategoryName = string.Empty,
                SingleGroup = string.Empty,
                MaxNum = "0",
                IsStealOldest = "FALSE",
                UnityMixerName = string.Empty,
                SpatialGroup = string.Empty,
                Delay = "0",
                Interval = "0",
                Pan = "0",
                Pitch = "0",
                IsLastSamples = "FALSE",
                FadeInTime = "0",
                FadeOutTime = "0",
                FadeInOldSample = "0",
                FadeOutOnPause = "0",
                FadeInOffPause = "0",
                IsVolRnd = "FALSE",
                IncVol = "FALSE",
                VolRndMin = "0",
                VolRndMax = "0",
                VolRndUnit = "0",
                IsPitchRnd = "FALSE",
                IncPitch = "FALSE",
                PitchRndMin = "0",
                PitchRndMax = "0",
                PitchRndUnit = "0",
                IsPanRnd = "FALSE",
                IncPan = "FALSE",
                PanRndMin = "0",
                PanRndMax = "0",
                PanRndUnit = "0",
                IsRndSrc = "FALSE",
                IncSrc = "FALSE",
                RndSrc = string.Empty,
                IsMovePitch = "FALSE",
                PitchStart = "0",
                PitchEnd = "0",
                PitchMoveTime = "0",
                IsMovePan = "FALSE",
                PanStart = "0",
                PanEnd = "0",
                PanMoveTime = "0",
                DuckingCategory = string.Empty,
                DuckStart = "0",
                DuckEnd = "0",
                DuckVol = "1",
                AutoRestore = "FALSE",
                RestoreTime = "0",
                IsAndroidNative = "FALSE"
            });

            // このLabel専用の追跡用クラスを作成
            var tracker = new LabelNameTracker { CurrentName = tempId, GroupName = groupName };

            newLabel.PropertyChanged += (s, e) =>
            {
                MarkAsChanged();

                // LabelNameが変更されたときにグループを更新
                if (e.PropertyName == nameof(LabelItemViewModel.LabelName))
                {
                    var currentGroup = _labelGroups.FirstOrDefault(g => g.GroupName == tracker.GroupName);
                    if (currentGroup != null)
                    {
                        // 古いLabelNameを削除
                        currentGroup.LabelNames.Remove(tracker.CurrentName);

                        // 新しいLabelNameを追加
                        var newLabelName = newLabel.LabelName ?? string.Empty;
                        currentGroup.LabelNames.Add(newLabelName);
                        tracker.CurrentName = newLabelName;

                        RebuildTabs();
                    }
                }
            };

            LabelItems.Add(newLabel);

            // グループに一時IDを追加
            group.LabelNames.Add(tempId);
        }

        // タブを再構築してリストを更新
        RebuildTabs();

        // 該当するグループタブを選択
        if (_labelGroupTabControl != null)
        {
            var groupIndex = _labelGroups.IndexOf(group);
            if (groupIndex >= 0)
            {
                _labelGroupTabControl.SelectedIndex = groupIndex;
                _currentLabelGroupName = groupName;
            }
        }

        UpdateStatus($"{allWavFiles.Count}個の.wavファイルを '{groupName}' グループに追加しました");
        MarkAsChanged();
    }

    private void UpdateStatus(string message)
    {
        var timestampedMessage = $"{DateTime.Now:HH:mm:ss} - {message}";
        
        // 履歴に追加
        _statusHistory.Add(timestampedMessage);
        
        // 最大数を超えたら古いものを削除
        if (_statusHistory.Count > MaxStatusHistory)
        {
            _statusHistory.RemoveAt(0);
        }
        
        // 改行で結合して表示（新しいものが下）
        StatusMessage = string.Join("\n", _statusHistory);
    }

    private void LoadAudioInfo(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            AudioInfoText = "No audio file selected";
            WaveformPoints = new PointCollection();
            OnPropertyChanged(nameof(WaveformPoints));
            return;
        }

        // ファイルパスを解決
        var fileCandidates = new List<string>
        {
            Path.Combine(_baseDir, fileName),
            Path.Combine(_baseDir, "Audio", fileName),
            Path.GetFullPath(Path.Combine(_xmlDir, "..", "Audio", fileName))
        };

        // 各Label Groupフォルダー内も検索
        foreach (var group in _labelGroups)
        {
            fileCandidates.Add(Path.Combine(_baseDir, "Audio", group.GroupName, fileName));
        }

        var path = fileCandidates.FirstOrDefault(File.Exists);
        if (path == null)
        {
            AudioInfoText = $"File not found: {fileName}";
            WaveformPoints = new PointCollection();
            OnPropertyChanged(nameof(WaveformPoints));
            return;
        }

        LoadWaveform(path);
    }

    private void LoadWaveform(string path)
    {
        try
        {
            using var reader = new AudioFileReader(path);
            
            int sampleCount = (int)(reader.Length / (reader.WaveFormat.BitsPerSample / 8));
            int channels = reader.WaveFormat.Channels;
            int points = 400;

            // WAVファイル情報を取得
            var format = reader.WaveFormat;
            var duration = reader.TotalTime;
            var fileName = Path.GetFileName(path);
            
            // ループ情報を取得
            var (hasLoop, loopStart, loopEnd) = ReadWavLoopInfoDetails(path);
            
            // Audio情報テキストを作成
            var infoText = $"File: {fileName}\n";
            infoText += $"Duration: {duration:mm\\:ss\\.ff}\n";
            infoText += $"Sample Rate: {format.SampleRate} Hz\n";
            infoText += $"Channels: {(channels == 1 ? "Mono" : channels == 2 ? "Stereo" : $"{channels}ch")}\n";
            infoText += $"Bit Depth: {format.BitsPerSample} bit\n";
            infoText += $"Loop: {(hasLoop ? $"Yes (Start: {loopStart}, End: {loopEnd})" : "No")}";
            
            AudioInfoText = infoText;

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
                pc.Add(new System.Windows.Point(x, y + height / 2));
            }

            WaveformPoints = pc;
            OnPropertyChanged(nameof(WaveformPoints));
        }
        catch (Exception ex)
        {
            WaveformPoints = new PointCollection();
            OnPropertyChanged(nameof(WaveformPoints));
            AudioInfoText = $"Error reading audio file:\n{ex.Message}";
        }
    }

    private bool ReadWavLoopInfo(string path)
    {
        try
        {
            using var fs = File.OpenRead(path);
            using var br = new BinaryReader(fs);
            
            // RIFFヘッダー検証
            var riff = new string(br.ReadChars(4));
            if (riff != "RIFF") return false;
            
            br.ReadInt32(); // ファイルサイズ
            var wave = new string(br.ReadChars(4));
            if (wave != "WAVE") return false;
            
            // チャンクを検索
            while (fs.Position < fs.Length - 8)
            {
                var chunkId = new string(br.ReadChars(4));
                var chunkSize = br.ReadInt32();
                
                if (chunkId == "smpl" && chunkSize >= 36)
                {
                    br.ReadBytes(28); // スキップ
                    var numLoops = br.ReadInt32();
                    return numLoops > 0;
                }
                else
                {
                    fs.Seek(chunkSize, SeekOrigin.Current);
                }
            }
        }
        catch
        {
            // エラー時はループなし
        }
        return false;
    }

    private (bool hasLoop, uint loopStart, uint loopEnd) ReadWavLoopInfoDetails(string path)
    {
        try
        {
            using var fs = File.OpenRead(path);
            using var br = new BinaryReader(fs);
            
            // RIFFヘッダー検証
            var riff = new string(br.ReadChars(4));
            if (riff != "RIFF") return (false, 0, 0);
            
            br.ReadInt32(); // ファイルサイズ
            var wave = new string(br.ReadChars(4));
            if (wave != "WAVE") return (false, 0, 0);
            
            // チャンクを検索
            while (fs.Position < fs.Length - 8)
            {
                var chunkId = new string(br.ReadChars(4));
                var chunkSize = br.ReadInt32();
                
                if (chunkId == "smpl" && chunkSize >= 60)
                {
                    br.ReadBytes(28); // スキップ
                    var numLoops = br.ReadInt32();
                    
                    if (numLoops > 0)
                    {
                        br.ReadBytes(4); // サンプラーデータサイズをスキップ
                        br.ReadInt32(); // CuePointIDをスキップ
                        br.ReadInt32(); // Typeをスキップ
                        var start = br.ReadUInt32();
                        var end = br.ReadUInt32();
                        return (true, start, end);
                    }
                }
                else
                {
                    fs.Seek(chunkSize, SeekOrigin.Current);
                }
            }
        }
        catch
        {
            // エラー時はループなし
        }
        return (false, 0, 0);
    }

    public void Dispose()
    {
        _player.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

// Label名の追跡用ヘルパークラス
internal class LabelNameTracker
{
    public string CurrentName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
}

public sealed class LabelItemViewModel : INotifyPropertyChanged
{
    private readonly LabelSet _inner;

    public LabelItemViewModel(LabelSet inner)
    {
        _inner = inner;
    }

    public string Initial => string.IsNullOrWhiteSpace(LabelName) ? "?" : LabelName!.Substring(0, 1).ToUpperInvariant();

    public string DisplayName => string.IsNullOrWhiteSpace(LabelName) ? "Non" : LabelName;
    
    public string DisplayFileName => string.IsNullOrWhiteSpace(FileName) ? "Non" : FileName;

    public string? LabelName
    {
        get => _inner.LabelName;
        set 
        { 
            _inner.LabelName = value; 
            OnPropertyChanged(); 
            OnPropertyChanged(nameof(DisplayName));
            OnPropertyChanged(nameof(Initial));
        }
    }

    public string? FileName
    {
        get => _inner.FileName;
        set 
        { 
            _inner.FileName = value; 
            OnPropertyChanged(); 
            OnPropertyChanged(nameof(DisplayFileName));
        }
    }

    public string? Volume
    {
        get => _inner.Volume;
        set 
        { 
            // Volume: 0.0 ~ 2.0 の範囲にクランプ
            if (!string.IsNullOrWhiteSpace(value) && double.TryParse(value, out double vol))
            {
                vol = Math.Max(0.0, Math.Min(2.0, vol));
                _inner.Volume = vol.ToString("0.###");
            }
            else
            {
                _inner.Volume = value;
            }
            OnPropertyChanged(); 
        }
    }

    public string? CategoryBehavior
    {
        get => _inner.CategoryBehavior;
        set { _inner.CategoryBehavior = value; OnPropertyChanged(); }
    }

    public string? Priority
    {
        get => _inner.Priority;
        set 
        { 
            // Priority: 0 ~ 255 の範囲にクランプ
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out int priority))
            {
                priority = Math.Max(0, Math.Min(255, priority));
                _inner.Priority = priority.ToString();
            }
            else
            {
                _inner.Priority = value;
            }
            OnPropertyChanged(); 
        }
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
        set 
        { 
            // Interval: 0.0 以上
            if (!string.IsNullOrWhiteSpace(value) && double.TryParse(value, out double interval))
            {
                interval = Math.Max(0.0, interval);
                _inner.Interval = interval.ToString("0.###");
            }
            else
            {
                _inner.Interval = value;
            }
            OnPropertyChanged(); 
        }
    }

    public string? Pan
    {
        get => _inner.Pan;
        set 
        { 
            // Pan: -1.0 ~ 1.0 の範囲にクランプ
            if (!string.IsNullOrWhiteSpace(value) && double.TryParse(value, out double pan))
            {
                pan = Math.Max(-1.0, Math.Min(1.0, pan));
                _inner.Pan = pan.ToString("0.###");
            }
            else
            {
                _inner.Pan = value;
            }
            OnPropertyChanged(); 
        }
    }

    public string? Pitch
    {
        get => _inner.Pitch;
        set 
        { 
            // Pitch: -3.0 ~ 3.0 の範囲にクランプ
            if (!string.IsNullOrWhiteSpace(value) && double.TryParse(value, out double pitch))
            {
                pitch = Math.Max(-3.0, Math.Min(3.0, pitch));
                _inner.Pitch = pitch.ToString("0.###");
            }
            else
            {
                _inner.Pitch = value;
            }
            OnPropertyChanged(); 
        }
    }

    public string? Loop
    {
        get => _inner.Loop;
        set { _inner.Loop = value; OnPropertyChanged(); }
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
        set 
        { 
            // Volume: 0.0 ~ 2.0 の範囲にクランプ
            if (!string.IsNullOrWhiteSpace(value) && double.TryParse(value, out double vol))
            {
                vol = Math.Max(0.0, Math.Min(2.0, vol));
                _inner.Volume = vol.ToString("0.###");
            }
            else
            {
                _inner.Volume = value;
            }
            OnPropertyChanged(); 
        }
    }

    public string? MaxNum
    {
        get => _inner.MaxNum;
        set 
        { 
            // MaxNum: 0 以上
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out int maxNum))
            {
                maxNum = Math.Max(0, maxNum);
                _inner.MaxNum = maxNum.ToString();
            }
            else
            {
                _inner.MaxNum = value;
            }
            OnPropertyChanged(); 
        }
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
        set 
        { 
            // Volume: 0.0 ~ 2.0 の範囲にクランプ
            if (!string.IsNullOrWhiteSpace(value) && double.TryParse(value, out double vol))
            {
                vol = Math.Max(0.0, Math.Min(2.0, vol));
                _inner.Volume = vol.ToString("0.###");
            }
            else
            {
                _inner.Volume = value;
            }
            OnPropertyChanged(); 
        }
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


