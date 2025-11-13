using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using OMS.Client.Views;
using OMS.Core.Interfaces;
using OMS.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;

namespace OMS.Client;

/// <summary>
/// App.xaml の相互作用ロジック
/// Prismアプリケーションのブートストラッパー
/// </summary>
public partial class App : PrismApplication
{
    private IConfiguration? _configuration;

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        // Serilogの設定
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/oms-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("OMS アプリケーション起動");

        base.OnStartup(e);
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // 設定の登録
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        containerRegistry.RegisterInstance(_configuration);

        // ロギング
        containerRegistry.Register<ILoggerFactory, LoggerFactory>();
        containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

        // HTTP Client
        containerRegistry.RegisterSingleton<HttpClient>(provider =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"] ?? "http://localhost:8080")
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        });

        // サービスの登録
        containerRegistry.Register<IOrderService, OrderService>();
        containerRegistry.Register<IExecutionService, ExecutionService>();
        containerRegistry.Register<ISecurityService, SecurityService>();
        containerRegistry.Register<IPositionService, PositionService>();
        containerRegistry.Register<IPortfolioService, PortfolioService>();
        containerRegistry.RegisterSingleton<INotificationService, NotificationService>();

        // ViewModelsの登録（必要に応じて）
        // containerRegistry.Register<OrderEntryViewModel>();

        // Viewsの登録
        containerRegistry.RegisterForNavigation<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("OMS アプリケーション終了");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
