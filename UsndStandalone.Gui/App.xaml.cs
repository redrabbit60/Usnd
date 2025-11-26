namespace UsndStandalone.Gui;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(System.Windows.StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // グローバルエラーハンドラー
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            System.Windows.MessageBox.Show(
                $"エラーが発生しました:\n{ex?.Message}\n\n{ex?.StackTrace}",
                "エラー",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        };
        
        DispatcherUnhandledException += (sender, args) =>
        {
            System.Windows.MessageBox.Show(
                $"エラーが発生しました:\n{args.Exception.Message}\n\n{args.Exception.StackTrace}",
                "エラー",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
            args.Handled = true;
        };
    }
}



