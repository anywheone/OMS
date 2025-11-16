# Prism フレームワーク 基礎解説

## 目次
1. [Prismとは](#prismとは)
2. [主要な概念](#主要な概念)
3. [依存性注入（DI）](#依存性注入di)
4. [MVVMパターン](#mvvmパターン)
5. [ViewModelLocator](#viewmodellocator)
6. [コマンド](#コマンド)
7. [このプロジェクトでの実装例](#このプロジェクトでの実装例)
8. [ベストプラクティス](#ベストプラクティス)

---

## Prismとは

**Prism**は、WPF、Xamarin Forms、.NET MAUIなどのXAMLベースのアプリケーション開発を支援する、オープンソースのフレームワークです。

### 主な特徴
- **MVVM（Model-View-ViewModel）パターン**の実装支援
- **依存性注入（DI）**コンテナの統合
- **モジュール化**によるアプリケーションの分割
- **ナビゲーション**の簡素化
- **イベント集約**によるコンポーネント間通信

### なぜPrismを使うのか？
- 大規模アプリケーションの構造化が容易
- テストしやすいコードが書ける
- 疎結合な設計を実現できる
- ベストプラクティスに沿った開発が可能

---

## 主要な概念

### 1. 依存性注入（DI: Dependency Injection）
オブジェクトの依存関係を外部から注入する設計パターン。

**メリット:**
- テストが容易（モックに置き換えられる）
- 疎結合な設計
- ライフタイム管理の自動化

### 2. MVVM（Model-View-ViewModel）
UIロジックとビジネスロジックを分離する設計パターン。

```
View (XAML)
    ↕ データバインディング
ViewModel (ロジック)
    ↕
Model (データ)
```

### 3. コンテナ
依存性を管理する「箱」。オブジェクトの生成とライフタイムを管理する。

Prismでサポートされているコンテナ:
- **DryIoc** ← このプロジェクトで使用
- Unity
- Autofac

---

## 依存性注入（DI）

### DIの基本

#### 従来の方法（DIなし）
```csharp
public class MainWindowViewModel
{
    private readonly OrderService _orderService;

    public MainWindowViewModel()
    {
        // 直接newしている = 密結合
        _orderService = new OrderService();
    }
}
```

**問題点:**
- テスト時にモックに置き換えられない
- OrderServiceの依存関係も全て知る必要がある
- 変更に弱い

#### Prismの方法（DIあり）
```csharp
public class MainWindowViewModel
{
    private readonly OrderService _orderService;

    // コンストラクタで受け取る = 疎結合
    public MainWindowViewModel(OrderService orderService)
    {
        _orderService = orderService;
    }
}
```

**メリット:**
- テスト時にモックを注入できる
- 依存関係はコンテナが解決
- 変更に強い

### コンテナへの登録

`App.xaml.cs`の`RegisterTypes`メソッドで登録:

```csharp
protected override void RegisterTypes(IContainerRegistry containerRegistry)
{
    // サービスの登録
    containerRegistry.RegisterSingleton<OrderService>();  // シングルトン（1つだけ）

    // ViewModelの登録
    containerRegistry.Register<MainWindowViewModel>();    // 都度生成

    // Viewの登録
    containerRegistry.RegisterForNavigation<MainWindow>();
}
```

### 登録のスコープ

| メソッド | 意味 | 用途 |
|---------|------|------|
| `RegisterSingleton` | アプリケーション全体で1つだけ | サービス、設定、共有データ |
| `Register` | 必要な時に毎回新しいインスタンス | ViewModel、ダイアログ |
| `RegisterInstance` | 既存のインスタンスを登録 | 設定オブジェクトなど |

---

## MVVMパターン

### MVVMの役割分担

#### Model（データ層）
```csharp
// OMS.Core/Models/OrderModel.cs
public class OrderModel
{
    public long OrderNo { get; set; }
    public long SecurityId { get; set; }
    public string Side { get; set; }
    // ... データのみ、ロジックなし
}
```

#### View（UI層）
```xml
<!-- MainWindow.xaml -->
<Window>
    <TextBox Text="{Binding SecurityId}"/>
    <Button Command="{Binding PlaceOrderCommand}"/>
</Window>
```

#### ViewModel（プレゼンテーション層）
```csharp
// ViewModels/MainWindowViewModel.cs
public class MainWindowViewModel : BindableBase
{
    private string _securityId;

    // プロパティ（Viewとバインド）
    public string SecurityId
    {
        get => _securityId;
        set => SetProperty(ref _securityId, value);  // 変更通知
    }

    // コマンド（Viewのボタンと紐づく）
    public DelegateCommand PlaceOrderCommand { get; }

    public MainWindowViewModel(OrderService orderService)
    {
        PlaceOrderCommand = new DelegateCommand(PlaceOrder);
    }

    private void PlaceOrder()
    {
        // ビジネスロジック
    }
}
```

### データバインディング

Viewとの双方向通信:

```xml
<!-- 一方向: ViewModel → View -->
<TextBlock Text="{Binding OrderCount}"/>

<!-- 双方向: ViewModel ⇄ View -->
<TextBox Text="{Binding SecurityId, UpdateSourceTrigger=PropertyChanged}"/>

<!-- コマンド -->
<Button Command="{Binding PlaceOrderCommand}"/>
```

---

## ViewModelLocator

### ViewModelLocatorとは

**自動的にViewとViewModelを紐付ける機能**

#### 従来の方法（手動）
```csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();  // 手動設定
    }
}
```

#### Prismの方法（自動）
```xml
<!-- MainWindow.xaml -->
<Window x:Class="OMS.Client.Views.MainWindow"
        xmlns:prism="http://prismlibrary.com/"
        prism:ViewModelLocator.AutoWireViewModel="True">
    <!-- これだけでViewModelが自動で紐付けられる -->
</Window>
```

### 命名規則

ViewModelLocatorは命名規則に基づいて自動紐付けします:

| View | ViewModel |
|------|-----------|
| `Views.MainWindow` | `ViewModels.MainWindowViewModel` |
| `Views.ControlLibraryWindow` | `ViewModels.ControlLibraryViewModel` |

**規則:**
- Viewの名前空間: `*.Views.*`
- ViewModelの名前空間: `*.ViewModels.*`
- ViewModelの名前: `{ViewName}ViewModel`

### コンテナからの解決

```csharp
// MainWindowViewModel.cs
public class MainWindowViewModel
{
    private readonly IContainerProvider _container;

    public MainWindowViewModel(IContainerProvider container)
    {
        _container = container;
    }

    private void ShowControlLibrary()
    {
        // コンテナから解決（DIされる）
        var window = _container.Resolve<ControlLibraryWindow>();
        window.Show();
    }
}
```

---

## コマンド

### コマンドとは

**ユーザーアクションをカプセル化するオブジェクト**

### DelegateCommandの使い方

```csharp
public class MainWindowViewModel : BindableBase
{
    // コマンドの宣言
    public DelegateCommand PlaceOrderCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public MainWindowViewModel()
    {
        // コマンドの初期化
        PlaceOrderCommand = new DelegateCommand(PlaceOrder, CanPlaceOrder);
        RefreshCommand = new DelegateCommand(Refresh);
    }

    // コマンドの実装
    private void PlaceOrder()
    {
        // 処理
    }

    // コマンドの実行可否
    private bool CanPlaceOrder()
    {
        return !string.IsNullOrWhiteSpace(SecurityId);
    }

    private void Refresh()
    {
        // 処理
    }
}
```

### XAMLでのバインディング

```xml
<Button Content="注文" Command="{Binding PlaceOrderCommand}"/>
<Button Content="更新" Command="{Binding RefreshCommand}"/>
```

### DelegateCommand<T>（パラメータ付き）

```csharp
// ViewModel
public DelegateCommand<string> DeleteOrderCommand { get; }

public MainWindowViewModel()
{
    DeleteOrderCommand = new DelegateCommand<string>(DeleteOrder);
}

private void DeleteOrder(string orderNo)
{
    // orderNoを使った処理
}
```

```xml
<!-- XAML -->
<Button Command="{Binding DeleteOrderCommand}"
        CommandParameter="{Binding OrderNo}"/>
```

---

## このプロジェクトでの実装例

### 1. App.xaml.cs（エントリーポイント）

```csharp
public partial class App : PrismApplication
{
    // メインウィンドウの指定
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    // DIコンテナへの登録
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // 設定
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        containerRegistry.RegisterInstance(configuration);

        // サービス（シングルトン）
        containerRegistry.RegisterSingleton<HttpClient>();
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

### 2. ViewModel（MainWindowViewModel.cs）

```csharp
public class MainWindowViewModel : BindableBase
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly OrderService _orderService;
    private readonly IContainerProvider _container;

    // DIコンテナから注入される
    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        OrderService orderService,
        IContainerProvider container)
    {
        _logger = logger;
        _orderService = orderService;
        _container = container;

        // コマンドの初期化
        PlaceOrderCommand = new DelegateCommand(async () => await PlaceOrderAsync());
        ShowControlLibraryCommand = new DelegateCommand(ShowControlLibrary);
    }

    // プロパティ（変更通知付き）
    private string _securityId = "1001";
    public string SecurityId
    {
        get => _securityId;
        set => SetProperty(ref _securityId, value);  // INotifyPropertyChanged
    }

    // コマンド
    public DelegateCommand PlaceOrderCommand { get; }
    public DelegateCommand ShowControlLibraryCommand { get; }

    // コマンドの実装
    private async Task PlaceOrderAsync()
    {
        var result = await _orderService.CreateOrderAsync(...);
        // 処理
    }

    private void ShowControlLibrary()
    {
        // コンテナから解決
        var window = _container.Resolve<ControlLibraryWindow>();
        window.Show();
    }
}
```

### 3. View（MainWindow.xaml）

```xml
<Window x:Class="OMS.Client.Views.MainWindow"
        xmlns:prism="http://prismlibrary.com/"
        prism:ViewModelLocator.AutoWireViewModel="True">

    <StackPanel>
        <!-- データバインディング -->
        <TextBox Text="{Binding SecurityId, UpdateSourceTrigger=PropertyChanged}"/>

        <!-- コマンドバインディング -->
        <Button Content="注文" Command="{Binding PlaceOrderCommand}"/>
        <Button Content="コントロール一覧" Command="{Binding ShowControlLibraryCommand}"/>
    </StackPanel>
</Window>
```

### 4. View Code-Behind（MainWindow.xaml.cs）

```csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // DataContextはViewModelLocatorが自動設定
        // 手動設定は不要！
    }
}
```

---

## ベストプラクティス

### 1. ViewModelはViewを知らない

```csharp
// ❌ 悪い例
public class MainWindowViewModel
{
    private void ShowDialog()
    {
        var dialog = new MyDialog();  // Viewを直接生成
        dialog.ShowDialog();
    }
}

// ✅ 良い例
public class MainWindowViewModel
{
    private readonly IContainerProvider _container;

    public MainWindowViewModel(IContainerProvider container)
    {
        _container = container;
    }

    private void ShowDialog()
    {
        var dialog = _container.Resolve<MyDialog>();  // コンテナから解決
        dialog.ShowDialog();
    }
}
```

### 2. コンストラクタ注入を使う

```csharp
// ❌ 悪い例
public class MainWindowViewModel
{
    private OrderService _orderService;

    public MainWindowViewModel()
    {
        _orderService = new OrderService();  // 直接new
    }
}

// ✅ 良い例
public class MainWindowViewModel
{
    private readonly OrderService _orderService;

    public MainWindowViewModel(OrderService orderService)  // 注入
    {
        _orderService = orderService;
    }
}
```

### 3. BindableBaseを継承する

```csharp
// ✅ 良い例
public class MainWindowViewModel : BindableBase
{
    private string _name;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);  // 変更通知自動
    }
}
```

### 4. デザインタイム用コンストラクタを用意

```csharp
public class MainWindowViewModel : BindableBase
{
    // デザインタイム用（パラメータなし）
    public MainWindowViewModel()
    {
        // デザイナで表示するダミーデータ
        SecurityId = "9999";
    }

    // 実行時用（DI）
    public MainWindowViewModel(OrderService orderService)
    {
        _orderService = orderService;
    }
}
```

### 5. AutoWireViewModelを使う

```xml
<!-- ✅ 良い例 -->
<Window prism:ViewModelLocator.AutoWireViewModel="True">
```

```csharp
// ❌ 悪い例（手動設定）
public MainWindow()
{
    InitializeComponent();
    DataContext = new MainWindowViewModel();
}
```

---

## まとめ

### Prismの主要な利点

1. **依存性注入**: テストしやすく、疎結合なコード
2. **MVVM**: UIとロジックの分離
3. **ViewModelLocator**: View-ViewModel自動紐付け
4. **コマンド**: ユーザーアクションのカプセル化
5. **モジュール化**: 大規模アプリの構造化

### このプロジェクトでのPrism利用フロー

```
1. App.xaml.cs で RegisterTypes
   ↓
2. Container.Resolve<MainWindow>() で起動
   ↓
3. ViewModelLocator が MainWindowViewModel を自動設定
   ↓
4. MainWindowViewModel のコンストラクタに依存性が注入される
   ↓
5. View と ViewModel がデータバインディングで連携
```

### 参考リンク

- [Prism 公式ドキュメント](https://prismlibrary.com/)
- [Prism GitHub](https://github.com/PrismLibrary/Prism)
- [DryIoc](https://github.com/dadhi/DryIoc)

---

**作成日:** 2025-11-16
**プロジェクト:** OMS (Order Management System)
