using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using UpSub.UI.ViewModels;

namespace UpSub.UI.Views;

public partial class UrlBlockCard : UserControl
{
    public UrlBlockCard()
    {
        InitializeComponent();
    }
    
    private void BlockOnGotFocus(object? sender, GotFocusEventArgs e)
    {
        if (sender is not StyledElement { DataContext: UrlBlockViewModel block }) return;
        block.Focused = true;
    }

    private void BlockOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not StyledElement { DataContext: UrlBlockViewModel block }) return;
        block.Focused = true;
    }

    
}