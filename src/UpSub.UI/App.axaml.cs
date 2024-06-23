using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using UpSub.Service;

namespace UpSub.UI
{
    public partial class App : Application
    {
        

        public App()
        {
            
        }
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            RequestedThemeVariant = ThemeVariant.Dark;
            if (Design.IsDesignMode) { }
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
