﻿using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Controls;
using UpSub.Service;
using UpSub.Service.Services;
using UpSub.UI.ViewModels;

namespace UpSub.UI;

public partial class MainWindow : SukiWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        var web = new Core();
        await web.Build(6043);
        await web.Start();
        var vm = new MainViewModel
        {
            RequestService = web.ServiceProvider.GetRequiredService<ConfigRequestService>(),
            ConfigService  = web.ServiceProvider.GetRequiredService<SubConfigService>(),
            Core           = web
        };
        DataContext = vm;
        await vm.Load();
    }


    
}