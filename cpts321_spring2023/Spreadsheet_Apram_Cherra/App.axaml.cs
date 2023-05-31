using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Spreadsheet_Apram_Cherra.ViewModels;
using Spreadsheet_Apram_Cherra.Views;

namespace Spreadsheet_Apram_Cherra;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime mw)
            {
                ((MainWindow) mw.MainWindow).ShowGridRows();
            }
        base.OnFrameworkInitializationCompleted();
    }
}