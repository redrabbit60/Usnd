using System.Windows;

namespace UsndStandalone.Gui;

public partial class NewFileDialog : Window
{
    public string FileName => FileNameTextBox.Text;

    public NewFileDialog()
    {
        InitializeComponent();
        FileNameTextBox.Focus();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FileNameTextBox.Text))
        {
            System.Windows.MessageBox.Show("ファイル名を入力してください。", "エラー", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
