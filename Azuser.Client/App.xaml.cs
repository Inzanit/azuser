﻿using System;
using System.Threading.Tasks;
using System.Windows;
using Azuser.Client.Framework.Resolver;
using Azuser.Client.Views.Shell;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Json;

namespace Azuser.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Logger = CreateLogger();

            Log.Information($"Initializing Azuser with version {Version}");

            Log.Debug("Created logger");

            SubscribeForUnhandledExceptions();

            Log.Debug("Subscribed to unhandled exceptions");

            Resolver.Initialise();

            Log.Debug("Resolver initialized");

            var shell = Resolver.Get<Shell>();

            shell.Show();

            Log.Debug("Shell initialized");
        }

        private static Logger CreateLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.RollingFile(new JsonFormatter(), "logs\\log-{Date}.txt")
                .CreateLogger();
        }

        private static void SubscribeForUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledDomainException;
            TaskScheduler.UnobservedTaskException += OnUnhandledTaskSchedulerException;
        }

        private static void OnUnhandledDomainException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            Log.Fatal(exception, "Client experienced unhandled exception");
        }

        private static void OnUnhandledTaskSchedulerException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.Fatal(e.Exception, "Client experienced unhandled exception in task scheduler");
        }
    }
}
