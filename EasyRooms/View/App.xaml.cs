﻿using EasyRooms.Implementations;
using EasyRooms.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace EasyRooms
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                   {
                       ConfigureServices(context.Configuration, services);
                   })
                   .Build();
        }

        private static void ConfigureServices(IConfiguration _, IServiceCollection services)
        {
            services.AddTransient<IXpsWordsExtractor, XpsWordsExtractor>()
                .AddTransient<IRowsCreator, RowsCreator>()
                .AddTransient<IDayPlanParser, DayPlanParser>()
                .AddTransient<IRoomOccupationsFiller, RoomOccupationsFiller>()
                .AddSingleton<MainWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }

            base.OnExit(e);
        }
    }
}