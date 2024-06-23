using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using UpSub.UI.ViewModels;

namespace UpSub.UI.Views;

public partial class ConfigCard : UserControl
{
    public ConfigCard()
    {
        InitializeComponent();
    }
    
    private void TitleOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not StyledElement { DataContext: SubConfigViewModel config }) return;
        config.EditingName = true;
    }

    private void TitleOnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is not StyledElement { DataContext: SubConfigViewModel config }) return;
        config.EditingName = false;
    }
}