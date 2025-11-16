# 依存性注入 (DI: Dependency Injection) 完全ガイド

## 目次
1. [依存性注入とは](#依存性注入とは)
2. [DIの利点](#diの利点)
3. [DIコンテナ](#diコンテナ)
4. [ライフタイム管理](#ライフタイム管理)
5. [実装パターン](#実装パターン)
6. [Prismでの DI](#prismでのdi)
7. [ベストプラクティス](#ベストプラクティス)
8. [よくある間違い](#よくある間違い)

---

## 依存性注入とは

### 定義
**依存性注入 (DI)** は、オブジェクトが必要とする依存関係を外部から注入する設計パターンです。

### 依存関係とは

```csharp
public class OrderService
{
    private readonly HttpClient _httpClient;  // OrderServiceはHttpClientに依存

    public OrderService()
    {
        _httpClient = new HttpClient();  // 直接newしている = 密結合
    }
}
```

### DIなし vs DIあり

#### DIなし（密結合）

```csharp
public class MainWindowViewModel
{
    private readonly OrderService _orderService;

    public MainWindowViewModel()
    {
        // 直接newしている
        _orderService = new OrderService();
    }
}
```

**問題点:**
- テスト時にモックに置き換えられない
- OrderServiceの変更がMainWindowViewModelに影響
- OrderServiceの依存関係も全て知る必要がある
- 変更に弱い

#### DIあり（疎結合）

```csharp
public class MainWindowViewModel
{
    private readonly OrderService _orderService;

    // コンストラクタで受け取る
    public MainWindowViewModel(OrderService orderService)
    {
        _orderService = orderService;
    }
}
```

**利点:**
- テスト時にモックを注入できる
- OrderServiceの実装変更の影響を受けない
- 依存関係はコンテナが解決
- 変更に強い

---

## DIの利点

### 1. テスタビリティ

```csharp
// テストコード
[Test]
public void PlaceOrder_CallsOrderService()
{
    // モックを作成
    var mockOrderService = new Mock<IOrderService>();

    // DIでモックを注入
    var viewModel = new MainWindowViewModel(mockOrderService.Object);

    // テスト実行
    viewModel.PlaceOrderCommand.Execute();

    // モックが呼ばれたか検証
    mockOrderService.Verify(s => s.CreateOrderAsync(It.IsAny<OrderModel>()), Times.Once);
}
```

### 2. 疎結合

```csharp
// インターフェースに依存
public class MainWindowViewModel
{
    private readonly IOrderService _orderService;  // インターフェース

    public MainWindowViewModel(IOrderService orderService)
    {
        _orderService = orderService;
    }
}

// 実装を切り替えられる
containerRegistry.Register<IOrderService, OrderService>();          // 本番用
containerRegistry.Register<IOrderService, MockOrderService>();      // テスト用
containerRegistry.Register<IOrderService, CachedOrderService>();    // キャッシュ付き
```

### 3. 再利用性

```csharp
// OrderServiceは他のクラスでも使える
public class OrderListViewModel
{
    public OrderListViewModel(OrderService orderService) { }
}

public class OrderDetailViewModel
{
    public OrderDetailViewModel(OrderService orderService) { }
}
```

### 4. 保守性

```csharp
// OrderServiceのコンストラクタが変更されても
public class OrderService
{
    // 新しい依存関係を追加
    public OrderService(HttpClient httpClient, ILogger logger, IConfiguration config)
    {
        // DIコンテナが自動で解決するので、使用側の変更は不要
    }
}
```

---

## DIコンテナ

### DIコンテナとは

**オブジェクトの生成と依存関係の解決を自動で行う「箱」**

```
┌──────────────────────────────┐
│      DIコンテナ               │
│                              │
│  ┌──────────────────────┐   │
│  │  OrderService         │   │
│  └──────────────────────┘   │
│           ↑                  │
│  ┌──────────────────────┐   │
│  │  HttpClient           │   │
│  └──────────────────────┘   │
│           ↑                  │
│  ┌──────────────────────┐   │
│  │  ILogger              │   │
│  └──────────────────────┘   │
└──────────────────────────────┘
          ↓
  MainWindowViewModel
```

### .NETのDIコンテナ

| コンテナ | 特徴 | 使用場所 |
|---------|------|---------|
| **DryIoc** | 高速、軽量 | Prism（このプロジェクト） |
| **Unity** | 機能豊富 | Prism |
| **Autofac** | 強力、柔軟 | ASP.NET Core |
| **Microsoft.Extensions.DependencyInjection** | 標準 | ASP.NET Core、.NET Core |

---

## ライフタイム管理

### 3つのライフタイム

#### 1. Transient（都度生成）

**毎回新しいインスタンスを生成**

```csharp
containerRegistry.Register<MainWindowViewModel>();
```

```
リクエスト1 → 新しいMainWindowViewModel
リクエスト2 → 新しいMainWindowViewModel（別インスタンス）
リクエスト3 → 新しいMainWindowViewModel（別インスタンス）
```

**用途:**
- ViewModel
- 軽量なサービス
- ステートレスなオブジェクト

#### 2. Singleton（シングルトン）

**アプリケーション全体で1つだけ**

```csharp
containerRegistry.RegisterSingleton<OrderService>();
```

```
リクエスト1 → OrderService（初回生成）
リクエスト2 → OrderService（同じインスタンス）
リクエスト3 → OrderService（同じインスタンス）
```

**用途:**
- APIクライアント（HttpClient）
- データベース接続
- 設定オブジェクト
- キャッシュ

#### 3. Scoped（スコープ単位）

**スコープ内で1つ**（WPFではあまり使わない）

```csharp
containerRegistry.RegisterScoped<OrderService>();
```

**用途:**
- ASP.NET Coreのリクエストスコープ
- EF CoreのDbContext

### ライフタイムの選択

```csharp
protected override void RegisterTypes(IContainerRegistry containerRegistry)
{
    // シングルトン（1つだけ）
    containerRegistry.RegisterSingleton<HttpClient>();
    containerRegistry.RegisterSingleton<IConfiguration>(_configuration);
    containerRegistry.RegisterSingleton<OrderService>();
    containerRegistry.RegisterSingleton<ILoggerFactory, LoggerFactory>();

    // 都度生成
    containerRegistry.Register<MainWindowViewModel>();
    containerRegistry.Register<OrderDetailViewModel>();
    containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

    // インスタンス登録
    containerRegistry.RegisterInstance(_configuration);
}
```

---

## 実装パターン

### パターン1: コンストラクタ注入（推奨）

**最も一般的な方法**

```csharp
public class MainWindowViewModel
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly OrderService _orderService;
    private readonly IContainerProvider _container;

    // コンストラクタで受け取る
    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        OrderService orderService,
        IContainerProvider container)
    {
        _logger = logger;
        _orderService = orderService;
        _container = container;
    }
}
```

**メリット:**
- 依存関係が明確
- 必須依存関係を保証
- イミュータブル（変更不可）

### パターン2: プロパティ注入

**オプショナルな依存関係**

```csharp
public class MainWindowViewModel
{
    // 自動でセットされる
    [Dependency]
    public ILogger Logger { get; set; }

    public MainWindowViewModel()
    {
        // Loggerはnullの可能性がある
    }
}
```

**使用場面:**
- オプショナルな依存関係
- 基底クラスでの注入

### パターン3: メソッド注入

**メソッド実行時に注入**

```csharp
public class OrderService
{
    public void ProcessOrder([Dependency] ILogger logger, OrderModel order)
    {
        logger.LogInformation($"Processing order {order.OrderNo}");
    }
}
```

**使用場面:**
- 特定のメソッドでのみ必要な依存関係

### パターン4: サービスロケーター（アンチパターン）

**DIコンテナから直接取得**

```csharp
// ❌ 悪い例
public class MainWindowViewModel
{
    private readonly OrderService _orderService;

    public MainWindowViewModel(IContainerProvider container)
    {
        // コンテナから直接取得（避けるべき）
        _orderService = container.Resolve<OrderService>();
    }
}

// ✅ 良い例
public class MainWindowViewModel
{
    private readonly OrderService _orderService;

    public MainWindowViewModel(OrderService orderService)
    {
        _orderService = orderService;
    }
}
```

---

## PrismでのDI

### App.xaml.csでの登録

```csharp
public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        // DIコンテナから解決
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // 設定
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        containerRegistry.RegisterInstance(configuration);

        // ロギング
        containerRegistry.Register<ILoggerFactory, LoggerFactory>();
        containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

        // HTTP Client（シングルトン）
        containerRegistry.RegisterSingleton<HttpClient>(provider =>
        {
            var config = provider.Resolve<IConfiguration>();
            var client = new HttpClient
            {
                BaseAddress = new Uri(config["ApiSettings:BaseUrl"] ?? "http://localhost:8080")
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        });

        // サービス（シングルトン）
        containerRegistry.RegisterSingleton<OrderService>();

        // ViewModels（都度生成）
        containerRegistry.Register<MainWindowViewModel>();
        containerRegistry.Register<ControlLibraryViewModel>();

        // Views（ナビゲーション用）
        containerRegistry.RegisterForNavigation<MainWindow>();
        containerRegistry.RegisterForNavigation<ControlLibraryWindow>();
    }
}
```

### ViewModelでの使用

```csharp
public class MainWindowViewModel : BindableBase
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly OrderService _orderService;
    private readonly IContainerProvider _container;

    // DIコンテナが自動で依存関係を解決して注入
    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        OrderService orderService,
        IContainerProvider container)
    {
        _logger = logger;
        _orderService = orderService;
        _container = container;

        _logger.LogInformation("MainWindowViewModel initialized");
    }

    private void ShowDialog()
    {
        // 動的に解決
        var dialog = _container.Resolve<OrderDetailWindow>();
        dialog.ShowDialog();
    }
}
```

### インターフェースの使用

```csharp
// インターフェース定義
public interface IOrderService
{
    Task<OrderModel> CreateOrderAsync(OrderModel order);
    Task<List<OrderModel>> GetOrdersAsync();
}

// 実装
public class OrderService : IOrderService
{
    public async Task<OrderModel> CreateOrderAsync(OrderModel order)
    {
        // 実装
    }

    public async Task<List<OrderModel>> GetOrdersAsync()
    {
        // 実装
    }
}

// 登録
containerRegistry.RegisterSingleton<IOrderService, OrderService>();

// 使用
public class MainWindowViewModel
{
    public MainWindowViewModel(IOrderService orderService)  // インターフェースを受け取る
    {
        // ...
    }
}
```

---

## ベストプラクティス

### 1. インターフェースに依存する

```csharp
// ✅ 良い例
public class MainWindowViewModel
{
    private readonly IOrderService _orderService;

    public MainWindowViewModel(IOrderService orderService)
    {
        _orderService = orderService;
    }
}

// ❌ 悪い例（具象クラスに依存）
public class MainWindowViewModel
{
    private readonly OrderService _orderService;

    public MainWindowViewModel(OrderService orderService)
    {
        _orderService = orderService;
    }
}
```

### 2. コンストラクタ注入を使う

```csharp
// ✅ 良い例
public MainWindowViewModel(OrderService orderService)
{
    _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
}

// ❌ 悪い例（プロパティ注入）
[Dependency]
public OrderService OrderService { get; set; }
```

### 3. 適切なライフタイムを選択

```csharp
// ✅ 良い例
containerRegistry.RegisterSingleton<HttpClient>();        // 1つだけ
containerRegistry.Register<MainWindowViewModel>();        // 都度生成

// ❌ 悪い例
containerRegistry.RegisterSingleton<MainWindowViewModel>();  // ViewModelをシングルトンにしない
containerRegistry.Register<HttpClient>();                    // HttpClientを都度生成しない
```

### 4. コンテナから直接解決しない

```csharp
// ✅ 良い例
public class MainWindowViewModel
{
    public MainWindowViewModel(OrderService orderService)
    {
        // コンストラクタで受け取る
    }
}

// ❌ 悪い例（サービスロケーター）
public class MainWindowViewModel
{
    public MainWindowViewModel(IContainerProvider container)
    {
        var orderService = container.Resolve<OrderService>();  // 避ける
    }
}
```

### 5. 循環依存を避ける

```csharp
// ❌ 悪い例（循環依存）
public class ServiceA
{
    public ServiceA(ServiceB serviceB) { }
}

public class ServiceB
{
    public ServiceB(ServiceA serviceA) { }  // 循環！
}

// ✅ 良い例（インターフェースで分離）
public class ServiceA
{
    public ServiceA(IServiceB serviceB) { }
}

public class ServiceB : IServiceB
{
    // ServiceAに依存しない
}
```

---

## よくある間違い

### 1. new キーワードを使う

```csharp
// ❌ 悪い例
public class MainWindowViewModel
{
    private readonly OrderService _orderService;

    public MainWindowViewModel()
    {
        _orderService = new OrderService();  // DIを使わない
    }
}

// ✅ 良い例
public class MainWindowViewModel
{
    private readonly OrderService _orderService;

    public MainWindowViewModel(OrderService orderService)
    {
        _orderService = orderService;
    }
}
```

### 2. 静的クラスを使う

```csharp
// ❌ 悪い例
public static class OrderServiceHelper
{
    public static OrderService Instance { get; } = new OrderService();
}

public class MainWindowViewModel
{
    public void PlaceOrder()
    {
        OrderServiceHelper.Instance.CreateOrder(...);  // テスト不可能
    }
}

// ✅ 良い例
public class MainWindowViewModel
{
    private readonly OrderService _orderService;

    public MainWindowViewModel(OrderService orderService)
    {
        _orderService = orderService;
    }

    public void PlaceOrder()
    {
        _orderService.CreateOrder(...);  // テスト可能
    }
}
```

### 3. 依存関係が多すぎる

```csharp
// ❌ 悪い例（コンストラクタが肥大化）
public class MainWindowViewModel
{
    public MainWindowViewModel(
        OrderService orderService,
        UserService userService,
        SecurityService securityService,
        NotificationService notificationService,
        LoggingService loggingService,
        ConfigurationService configurationService,
        CacheService cacheService)
    {
        // 依存関係が多すぎる = 責務が多すぎる
    }
}

// ✅ 良い例（責務を分割）
public class OrderManagementViewModel
{
    public OrderManagementViewModel(OrderService orderService)
    {
        // 注文管理のみ
    }
}

public class UserManagementViewModel
{
    public UserManagementViewModel(UserService userService)
    {
        // ユーザー管理のみ
    }
}
```

### 4. DIコンテナへの登録忘れ

```csharp
// ❌ 登録していない
public class MainWindowViewModel
{
    public MainWindowViewModel(OrderService orderService)
    {
        // OrderServiceが登録されていないとエラー
    }
}

// ✅ App.xaml.csで登録
protected override void RegisterTypes(IContainerRegistry containerRegistry)
{
    containerRegistry.RegisterSingleton<OrderService>();  // 登録必須
}
```

---

## まとめ

### DIの原則

1. **依存関係を外部から注入**
2. **インターフェースに依存**
3. **コンストラクタ注入を優先**
4. **適切なライフタイムを選択**
5. **循環依存を避ける**

### DIのメリット

| メリット | 説明 |
|---------|------|
| **テスタビリティ** | モックを使った単体テスト |
| **疎結合** | 実装の変更に強い |
| **再利用性** | 同じサービスを複数箇所で使用 |
| **保守性** | 依存関係が明確 |

### Prism DI フロー

```
App.xaml.cs
  ↓
RegisterTypes（登録）
  ↓
DIコンテナ
  ↓
Container.Resolve<MainWindow>
  ↓
MainWindowViewModel（依存関係自動注入）
  ↓
OrderService, ILogger, etc...
```

---

**作成日:** 2025-11-16
**プロジェクト:** OMS (Order Management System)
