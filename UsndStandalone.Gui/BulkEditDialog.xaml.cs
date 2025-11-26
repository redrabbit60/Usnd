using System.Windows;

namespace UsndStandalone.Gui;

public partial class BulkEditDialog : Window
{
    public int SelectionCount { get; set; }

    public bool ApplyVolume => VolumeCheckBox.IsChecked == true;
    public string? VolumeValue => VolumeTextBox.Text;

    public bool ApplyCategoryName => CategoryNameCheckBox.IsChecked == true;
    public string? CategoryNameValue => CategoryNameTextBox.Text;

    public bool ApplyPriority => PriorityCheckBox.IsChecked == true;
    public string? PriorityValue => PriorityTextBox.Text;

    public bool ApplyLoop => LoopCheckBox.IsChecked == true;
    public string? LoopValue => LoopTextBox.Text;

    public bool ApplyPan => PanCheckBox.IsChecked == true;
    public string? PanValue => PanTextBox.Text;

    public bool ApplyPitch => PitchCheckBox.IsChecked == true;
    public string? PitchValue => PitchTextBox.Text;

    public BulkEditDialog()
    {
        InitializeComponent();
        Loaded += (s, e) => SelectionCountText.Text = SelectionCount.ToString();
    }

    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
