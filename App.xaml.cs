﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PictureFixer.Services;
using PictureFixer.Validators;
using PictureFixer.ViewModels;
using System.Windows;
using System.Windows.Threading;

namespace PictureFixer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c =>
            {
                //c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); 
            })
            .ConfigureServices((context, services) =>
            {
                services.AddTransient<MainWindow>();
                services.AddTransient<PictureTransformViewModel>();
                services.AddTransient<IPictureValidator, PictureValidator>();
                services.AddTransient<IFolderPickerDialog, FolderPickerDialog>();
                services.AddTransient<IPictureConverter, PictureConverter>();
                services.AddTransient<IMessageBoxService, MessageBoxService>();
                //throw new NotImplementedException("No service or window was registered.");
            }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T? GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = GetService<MainWindow>();

            if (mainWindow == null)
            {
                Shutdown();
                return;
            }

            _host.Start();

            mainWindow.Show();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}
