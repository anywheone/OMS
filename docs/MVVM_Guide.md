# MVVM パターン 完全ガイド

## 目次
1. [MVVMとは](#mvvmとは)
2. [MVVMの構成要素](#mvvmの構成要素)
3. [データバインディング](#データバインディング)
4. [INotifyPropertyChanged](#inotifypropertychanged)
5. [コマンド](#コマンド)
6. [実装パターン](#実装パターン)
7. [ベストプラクティス](#ベストプラクティス)
8. [アンチパターン](#アンチパターン)

---

## MVVMとは

### 定義
**MVVM (Model-View-ViewModel)** は、UIとビジネスロジックを分離する設計パターンです。

### MVCとの違い

| パターン | Controller/ViewModel | データフロー |
|---------|---------------------|------------|
| **MVC** | Controller が View を直接操作 | View ← Controller → Model |
| **MVVM** | ViewModel は View を知らない | View ⇄ ViewModel ⇄ Model |

### MVVMのメリット

1. **テスタビリティ**: ViewModelは単体テスト可能
2. **保守性**: UIとロジックが分離されている
3. **再利用性**: ViewModelは複数のViewで使える
4. **デザイナーとの協業**: XAMLとC#を分離できる

### MVVMが適している場面

- WPF、UWP、Xamarin Forms、.NET MAUI
- データバインディングが利用可能な環境
- 複雑なUIロジックを持つアプリケーション

---

## MVVMの構成要素

### 1. Model（データ層）

**役割:** データとビジネスロジック

```csharp
// OMS.Core/Models/OrderModel.cs
public class OrderModel
{
    // データプロパティのみ
    public long OrderNo { get; set; }
    public long UserId { get; set; }
    public long SecurityId { get; set; }
    public string Side { get; set; }
    public string OrderType { get; set; }
    public decimal Quantity { get; set; }
    public decimal? Price { get; set; }
    public string TimeInForce { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
}

// ビジネスロジック
public class OrderService
{
    public async Task<OrderModel> CreateOrderAsync(OrderModel order)
    {
        // API呼び出し、バリデーション、データ保存など
        var response = await _httpClient.PostAsJsonAsync("/api/orders", order);
        return await response.Content.ReadFromJsonAsync<OrderModel>();
    }

    public async Task<List<OrderModel>> GetOrdersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<OrderModel>>("/api/orders");
    }
}
```

### 2. View（表示層）

**役割:** UIの定義と表示

```xml
<!-- Views/MainWindow.xaml -->
<Window x:Class="OMS.Client.Views.MainWindow"
        xmlns:prism="http://prismlibrary.com/"
        prism:ViewModelLocator.AutoWireViewModel="True">

    <Grid>
        <!-- データバインディングでViewModelと連携 -->
        <StackPanel>
            <TextBlock Text="銘柄コード"/>
            <TextBox Text="{Binding SecurityId, UpdateSourceTrigger=PropertyChanged}"/>

            <TextBlock Text="数量"/>
            <TextBox Text="{Binding Quantity}"/>

            <Button Content="発注" Command="{Binding PlaceOrderCommand}"/>
        </StackPanel>

        <!-- データグリッド -->
        <DataGrid ItemsSource="{Binding Orders}" AutoGenerateColumns="False">
            <DataGrid.Columns>
                <DataGridTextColumn Header="注文番号" Binding="{Binding OrderNo}"/>
                <DataGridTextColumn Header="銘柄" Binding="{Binding SecurityId}"/>
                <DataGridTextColumn Header="数量" Binding="{Binding Quantity}"/>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>
</Window>
```

```csharp
// Views/MainWindow.xaml.cs
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // ViewはViewModelを知らない（Prismが自動設定）
    }
}
```

### 3. ViewModel（プレゼンテーション層）

**役割:** UIロジックとデータの変換

```csharp
// ViewModels/MainWindowViewModel.cs
public class MainWindowViewModel : BindableBase
{
    private readonly OrderService _orderService;
    private string _securityId = "1001";
    private string _quantity = "100";

    // プロパティ（変更通知付き）
    public string SecurityId
    {
        get => _securityId;
        set => SetProperty(ref _securityId, value);
    }

    public string Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    // コレクション
    public ObservableCollection<OrderModel> Orders { get; } = new();

    // コマンド
    public DelegateCommand PlaceOrderCommand { get; }

    // コンストラクタ（DI）
    public MainWindowViewModel(OrderService orderService)
    {
        _orderService = orderService;
        PlaceOrderCommand = new DelegateCommand(async () => await PlaceOrderAsync());

        // 初期データロード
        _ = LoadOrdersAsync();
    }

    // ビジネスロジック
    private async Task PlaceOrderAsync()
    {
        var order = new OrderModel
        {
            SecurityId = long.Parse(SecurityId),
            Quantity = decimal.Parse(Quantity),
            // ...
        };

        var result = await _orderService.CreateOrderAsync(order);

        // 一覧を更新
        await LoadOrdersAsync();
    }

    private async Task LoadOrdersAsync()
    {
        var orders = await _orderService.GetOrdersAsync();

        Orders.Clear();
        foreach (var order in orders)
        {
            Orders.Add(order);
        }
    }
}
```

---

## データバインディング

### バインディングの方向

#### 1. OneWay（一方向: ViewModel → View）
```xml
<!-- ViewModelの変更がViewに反映される -->
<TextBlock Text="{Binding OrderCount, Mode=OneWay}"/>
```

#### 2. TwoWay（双方向: ViewModel ⇄ View）
```xml
<!-- Viewの入力がViewModelに反映され、ViewModelの変更もViewに反映される -->
<TextBox Text="{Binding SecurityId, Mode=TwoWay}"/>
```

#### 3. OneWayToSource（ViewModel ← View）
```xml
<!-- Viewの変更のみViewModelに反映 -->
<TextBox Text="{Binding UserInput, Mode=OneWayToSource}"/>
```

#### 4. OneTime（一度だけ）
```xml
<!-- 初期値のみ設定 -->
<TextBlock Text="{Binding InitialMessage, Mode=OneTime}"/>
```

### UpdateSourceTrigger

| 値 | 意味 | 使用場面 |
|----|------|---------|
| `PropertyChanged` | 入力の度に更新 | リアルタイム検証、検索ボックス |
| `LostFocus` | フォーカスを失った時 | 入力フォーム（デフォルト） |
| `Explicit` | 明示的に更新 | 手動でバインディング更新したい時 |

```xml
<!-- リアルタイム検索 -->
<TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}"/>

<!-- フォーカスを失った時に更新 -->
<TextBox Text="{Binding Email, UpdateSourceTrigger=LostFocus}"/>
```

---

## INotifyPropertyChanged

### 変更通知の仕組み

ViewModelのプロパティが変更されたことをViewに通知する仕組み。

### 実装方法

#### 方法1: 手動実装
```csharp
public class MainWindowViewModel : INotifyPropertyChanged
{
    private string _securityId;

    public string SecurityId
    {
        get => _securityId;
        set
        {
            if (_securityId != value)
            {
                _securityId = value;
                OnPropertyChanged(nameof(SecurityId));  // 通知
            }
        }
    }

    // INotifyPropertyChangedの実装
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

#### 方法2: BindableBase（Prism推奨）
```csharp
public class MainWindowViewModel : BindableBase
{
    private string _securityId;

    public string SecurityId
    {
        get => _securityId;
        set => SetProperty(ref _securityId, value);  // 自動で通知
    }
}
```

### 計算プロパティの通知

```csharp
public class OrderViewModel : BindableBase
{
    private decimal _price;
    private decimal _quantity;

    public decimal Price
    {
        get => _price;
        set
        {
            SetProperty(ref _price, value);
            RaisePropertyChanged(nameof(TotalAmount));  // 関連プロパティも通知
        }
    }

    public decimal Quantity
    {
        get => _quantity;
        set
        {
            SetProperty(ref _quantity, value);
            RaisePropertyChanged(nameof(TotalAmount));
        }
    }

    // 計算プロパティ
    public decimal TotalAmount => Price * Quantity;
}
```

---

## コマンド

### コマンドとは

**ユーザーアクションをカプセル化するオブジェクト**

### ICommandインターフェース

```csharp
public interface ICommand
{
    // コマンドが実行可能かどうか
    bool CanExecute(object parameter);

    // コマンドの実行
    void Execute(object parameter);

    // 実行可否の変更通知
    event EventHandler CanExecuteChanged;
}
```

### DelegateCommand（Prism）

#### 基本的な使い方
```csharp
public class MainWindowViewModel : BindableBase
{
    public DelegateCommand PlaceOrderCommand { get; }

    public MainWindowViewModel()
    {
        // コマンドの作成
        PlaceOrderCommand = new DelegateCommand(PlaceOrder, CanPlaceOrder);
    }

    // 実行処理
    private void PlaceOrder()
    {
        // 注文処理
    }

    // 実行可否
    private bool CanPlaceOrder()
    {
        return !string.IsNullOrWhiteSpace(SecurityId);
    }
}
```

#### 非同期コマンド
```csharp
public DelegateCommand RefreshCommand { get; }

public MainWindowViewModel()
{
    RefreshCommand = new DelegateCommand(async () => await RefreshAsync());
}

private async Task RefreshAsync()
{
    var orders = await _orderService.GetOrdersAsync();
    // ...
}
```

#### パラメータ付きコマンド
```csharp
public DelegateCommand<OrderModel> DeleteOrderCommand { get; }

public MainWindowViewModel()
{
    DeleteOrderCommand = new DelegateCommand<OrderModel>(DeleteOrder);
}

private void DeleteOrder(OrderModel order)
{
    Orders.Remove(order);
}
```

```xml
<Button Command="{Binding DeleteOrderCommand}"
        CommandParameter="{Binding}"/>
```

#### CanExecuteの更新
```csharp
private string _securityId;

public string SecurityId
{
    get => _securityId;
    set
    {
        SetProperty(ref _securityId, value);
        PlaceOrderCommand.RaiseCanExecuteChanged();  // 実行可否を再評価
    }
}
```

---

## 実装パターン

### パターン1: シンプルなフォーム

```csharp
public class LoginViewModel : BindableBase
{
    private readonly IAuthService _authService;
    private string _username;
    private string _password;
    private bool _isLoading;

    public string Username
    {
        get => _username;
        set
        {
            SetProperty(ref _username, value);
            LoginCommand.RaiseCanExecuteChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
            LoginCommand.RaiseCanExecuteChanged();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public DelegateCommand LoginCommand { get; }

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        LoginCommand = new DelegateCommand(async () => await LoginAsync(), CanLogin);
    }

    private bool CanLogin()
    {
        return !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !IsLoading;
    }

    private async Task LoginAsync()
    {
        IsLoading = true;
        try
        {
            await _authService.LoginAsync(Username, Password);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

### パターン2: マスター/詳細

```csharp
public class OrderListViewModel : BindableBase
{
    private readonly OrderService _orderService;
    private OrderModel _selectedOrder;

    // マスター（一覧）
    public ObservableCollection<OrderModel> Orders { get; } = new();

    // 詳細（選択された注文）
    public OrderModel SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            SetProperty(ref _selectedOrder, value);
            RaisePropertyChanged(nameof(IsOrderSelected));
            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    public bool IsOrderSelected => SelectedOrder != null;

    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public OrderListViewModel(OrderService orderService)
    {
        _orderService = orderService;
        DeleteCommand = new DelegateCommand(DeleteOrder, () => SelectedOrder != null);
        RefreshCommand = new DelegateCommand(async () => await LoadOrdersAsync());

        _ = LoadOrdersAsync();
    }

    private async Task LoadOrdersAsync()
    {
        var orders = await _orderService.GetOrdersAsync();
        Orders.Clear();
        foreach (var order in orders)
        {
            Orders.Add(order);
        }
    }

    private void DeleteOrder()
    {
        if (SelectedOrder != null)
        {
            Orders.Remove(SelectedOrder);
        }
    }
}
```

```xml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="2*"/>
    </Grid.ColumnDefinitions>

    <!-- マスター -->
    <ListBox Grid.Column="0"
             ItemsSource="{Binding Orders}"
             SelectedItem="{Binding SelectedOrder}"
             DisplayMemberPath="OrderNo"/>

    <!-- 詳細 -->
    <StackPanel Grid.Column="1" DataContext="{Binding SelectedOrder}">
        <TextBlock Text="{Binding OrderNo, StringFormat='注文番号: {0}'}"/>
        <TextBlock Text="{Binding SecurityId, StringFormat='銘柄: {0}'}"/>
        <TextBlock Text="{Binding Quantity, StringFormat='数量: {0}'}"/>
    </StackPanel>
</Grid>
```

### パターン3: バリデーション

```csharp
public class OrderInputViewModel : BindableBase, IDataErrorInfo
{
    private string _securityId;
    private string _quantity;

    public string SecurityId
    {
        get => _securityId;
        set => SetProperty(ref _securityId, value);
    }

    public string Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    // IDataErrorInfo実装
    public string Error => null;

    public string this[string columnName]
    {
        get
        {
            switch (columnName)
            {
                case nameof(SecurityId):
                    if (string.IsNullOrWhiteSpace(SecurityId))
                        return "銘柄コードを入力してください";
                    if (!long.TryParse(SecurityId, out _))
                        return "数値を入力してください";
                    break;

                case nameof(Quantity):
                    if (string.IsNullOrWhiteSpace(Quantity))
                        return "数量を入力してください";
                    if (!decimal.TryParse(Quantity, out var qty) || qty <= 0)
                        return "正の数値を入力してください";
                    break;
            }
            return null;
        }
    }
}
```

```xml
<TextBox Text="{Binding SecurityId, ValidatesOnDataErrors=True, UpdateSourceTrigger=PropertyChanged}"/>
<TextBox Text="{Binding Quantity, ValidatesOnDataErrors=True, UpdateSourceTrigger=PropertyChanged}"/>
```

---

## ベストプラクティス

### 1. ViewはViewModelを知らない

```csharp
// ✅ 良い例
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Prismが自動でViewModelを設定
    }
}

// ❌ 悪い例
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();  // Viewが直接ViewModelを生成
    }
}
```

### 2. ViewModelはViewを知らない

```csharp
// ❌ 悪い例
public class MainWindowViewModel
{
    private void ShowMessage()
    {
        MessageBox.Show("メッセージ");  // View依存
    }
}

// ✅ 良い例
public class MainWindowViewModel
{
    private readonly IDialogService _dialogService;

    public MainWindowViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    private void ShowMessage()
    {
        _dialogService.ShowMessage("メッセージ");  // サービス経由
    }
}
```

### 3. ObservableCollectionを使う

```csharp
// ✅ 良い例（変更が自動でViewに反映）
public ObservableCollection<OrderModel> Orders { get; } = new();

private void AddOrder(OrderModel order)
{
    Orders.Add(order);  // 自動でUIが更新される
}

// ❌ 悪い例（変更が反映されない）
public List<OrderModel> Orders { get; set; } = new();

private void AddOrder(OrderModel order)
{
    Orders.Add(order);  // UIは更新されない
}
```

### 4. async/awaitを正しく使う

```csharp
// ✅ 良い例
public DelegateCommand LoadCommand { get; }

public MainWindowViewModel()
{
    LoadCommand = new DelegateCommand(async () => await LoadDataAsync());
}

private async Task LoadDataAsync()
{
    var data = await _service.GetDataAsync();
    // ...
}

// ❌ 悪い例
public DelegateCommand LoadCommand { get; }

public MainWindowViewModel()
{
    LoadCommand = new DelegateCommand(() => LoadDataAsync().Wait());  // デッドロックの危険
}
```

### 5. プロパティ名は文字列を避ける

```csharp
// ✅ 良い例（リファクタリングセーフ）
RaisePropertyChanged(nameof(SecurityId));

// ❌ 悪い例（タイポのリスク）
RaisePropertyChanged("SecurityId");
```

---

## アンチパターン

### 1. コードビハインドにロジックを書く

```csharp
// ❌ 悪い例
public partial class MainWindow : Window
{
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        // ビジネスロジックをコードビハインドに書く
        var order = new OrderModel { /* ... */ };
        var service = new OrderService();
        service.CreateOrder(order);
    }
}

// ✅ 良い例（ViewModelに書く）
public class MainWindowViewModel : BindableBase
{
    public DelegateCommand PlaceOrderCommand { get; }

    private void PlaceOrder()
    {
        var order = new OrderModel { /* ... */ };
        _orderService.CreateOrder(order);
    }
}
```

### 2. ViewModelがViewを直接操作

```csharp
// ❌ 悪い例
public class MainWindowViewModel
{
    private MainWindow _view;

    public void SetView(MainWindow view)
    {
        _view = view;
    }

    private void ShowMessage()
    {
        _view.MessageTextBlock.Text = "メッセージ";  // Viewを直接操作
    }
}

// ✅ 良い例
public class MainWindowViewModel : BindableBase
{
    private string _message;

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    private void ShowMessage()
    {
        Message = "メッセージ";  // プロパティ経由
    }
}
```

### 3. ModelがViewModelを参照

```csharp
// ❌ 悪い例
public class OrderModel
{
    public MainWindowViewModel ViewModel { get; set; }  // 循環参照
}

// ✅ 良い例
public class OrderModel
{
    // データのみ
    public long OrderNo { get; set; }
    public string Side { get; set; }
}
```

---

## まとめ

### MVVMの責務

| 層 | 責務 | 例 |
|----|------|-----|
| **Model** | データとビジネスロジック | `OrderModel`, `OrderService` |
| **View** | UIの定義と表示 | `MainWindow.xaml` |
| **ViewModel** | UIロジックとデータ変換 | `MainWindowViewModel` |

### MVVMのデータフロー

```
User Input
   ↓
View (XAML)
   ↓ (DataBinding)
ViewModel
   ↓ (Method Call)
Service/Model
   ↓ (Data)
ViewModel
   ↓ (PropertyChanged)
View (XAML)
   ↓
UI Update
```

### テストのしやすさ

```csharp
// ViewModelは単体テスト可能
[Test]
public void PlaceOrder_SetsCorrectValues()
{
    var mockService = new Mock<IOrderService>();
    var viewModel = new MainWindowViewModel(mockService.Object);

    viewModel.SecurityId = "1001";
    viewModel.Quantity = "100";
    viewModel.PlaceOrderCommand.Execute();

    mockService.Verify(s => s.CreateOrderAsync(It.IsAny<OrderModel>()), Times.Once);
}
```

---

**作成日:** 2025-11-16
**プロジェクト:** OMS (Order Management System)
