# OMS 実装ガイド

## 概要

このドキュメントは、資産運用会社向けOMS（Order Management System）の実装ガイドです。
提供されているスターターキットを基に、残りのコンポーネントを実装する方法を説明します。

## プロジェクト構成

```
OMS/
├── docs/              # 設計ドキュメント（完成済み）
├── src/               # WPFクライアント
│   ├── OMS.Client/    # メインアプリケーション
│   ├── OMS.Core/      # コアライブラリ（完成済み）
│   └── OMS.Infrastructure/  # インフラ層
├── backend/           # Spring Boot API
│   └── oms-api/       # REST API（Order機能完成済み）
└── database/          # データベーススキーマ（完成済み）
```

## 完成済みコンポーネント

### 1. 設計ドキュメント ✅
- **ER図**: `docs/plantuml/database-schema/er-diagram.puml`
- **クラス図**: `docs/plantuml/class-diagrams/`
- **シーケンス図**: `docs/plantuml/sequence-diagrams/`

### 2. データベース ✅
- **スキーマ**: `database/schema.sql`
- **サンプルデータ**: `database/seed-data.sql`

### 3. .NET Core (WPF)  ✅
- **Enums**: `src/OMS.Core/Enums/`
- **Models**: `src/OMS.Core/Models/`
- **DTOs**: `src/OMS.Core/DTOs/`
- **Interfaces**: `src/OMS.Core/Interfaces/`
- **Prism設定**: `src/OMS.Client/App.xaml.cs`
- **テーマ**: `src/OMS.Client/Themes/`

### 4. UserControl（完全実装済み） ✅
- **NumericUpDownControl**: `src/OMS.Client/Controls/Utilities/NumericUpDownControl.*`
- **CurrencyDisplayControl**: `src/OMS.Client/Controls/Utilities/CurrencyDisplayControl.*`

### 5. Spring Boot API（Order機能完成済み） ✅
- **Entity**: `Order.java`
- **Repository**: `OrderRepository.java`
- **Service**: `OrderService.java`
- **Controller**: `OrderController.java`

---

## 未実装コンポーネントの実装方法

### 1. 残りのUserControlの実装

完成済みの`NumericUpDownControl`と`CurrencyDisplayControl`をテンプレートとして使用してください。

#### StatusBadgeControl の実装例

**XAML** (`StatusBadgeControl.xaml`)
```xml
<UserControl x:Class="OMS.Client.Controls.Utilities.StatusBadgeControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Name="root">
    <Border Background="{Binding BadgeBackground, ElementName=root}"
            CornerRadius="12"
            Padding="12,4">
        <StackPanel Orientation="Horizontal">
            <!-- アイコン（オプション） -->
            <Path Data="{Binding IconGeometry, ElementName=root}"
                  Fill="{Binding BadgeForeground, ElementName=root}"
                  Width="12" Height="12"
                  Margin="0,0,4,0"
                  Visibility="{Binding ShowIcon, ElementName=root,
                             Converter={StaticResource BooleanToVisibilityConverter}}"/>

            <!-- ステータステキスト -->
            <TextBlock Text="{Binding StatusText, ElementName=root}"
                       Foreground="{Binding BadgeForeground, ElementName=root}"
                       FontSize="12"
                       FontWeight="Medium"/>
        </StackPanel>
    </Border>
</UserControl>
```

**Code-behind** (`StatusBadgeControl.xaml.cs`)
```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OMS.Core.Enums;

namespace OMS.Client.Controls.Utilities;

public partial class StatusBadgeControl : UserControl
{
    public StatusBadgeControl()
    {
        InitializeComponent();
    }

    // DependencyProperty: Status
    public OrderStatus Status
    {
        get => (OrderStatus)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(
            nameof(Status),
            typeof(OrderStatus),
            typeof(StatusBadgeControl),
            new PropertyMetadata(OrderStatus.NEW, OnStatusChanged));

    // DependencyProperty: ShowIcon
    public bool ShowIcon
    {
        get => (bool)GetValue(ShowIconProperty);
        set => SetValue(ShowIconProperty, value);
    }

    public static readonly DependencyProperty ShowIconProperty =
        DependencyProperty.Register(
            nameof(ShowIcon),
            typeof(bool),
            typeof(StatusBadgeControl),
            new PropertyMetadata(true));

    // 計算プロパティ
    public string StatusText
    {
        get => (string)GetValue(StatusTextProperty);
        private set => SetValue(StatusTextProperty, value);
    }

    public static readonly DependencyProperty StatusTextProperty =
        DependencyProperty.Register(nameof(StatusText), typeof(string), typeof(StatusBadgeControl));

    public Brush BadgeBackground
    {
        get => (Brush)GetValue(BadgeBackgroundProperty);
        private set => SetValue(BadgeBackgroundProperty, value);
    }

    public static readonly DependencyProperty BadgeBackgroundProperty =
        DependencyProperty.Register(nameof(BadgeBackground), typeof(Brush), typeof(StatusBadgeControl));

    public Brush BadgeForeground
    {
        get => (Brush)GetValue(BadgeForegroundProperty);
        private set => SetValue(BadgeForegroundProperty, value);
    }

    public static readonly DependencyProperty BadgeForegroundProperty =
        DependencyProperty.Register(nameof(BadgeForeground), typeof(Brush), typeof(StatusBadgeControl));

    private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StatusBadgeControl control)
        {
            control.UpdateBadge();
        }
    }

    private void UpdateBadge()
    {
        switch (Status)
        {
            case OrderStatus.NEW:
                StatusText = "新規";
                BadgeBackground = new SolidColorBrush(Color.FromRgb(33, 150, 243)); // Blue
                BadgeForeground = Brushes.White;
                break;
            case OrderStatus.PARTIAL:
                StatusText = "部分約定";
                BadgeBackground = new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange
                BadgeForeground = Brushes.White;
                break;
            case OrderStatus.FILLED:
                StatusText = "約定済";
                BadgeBackground = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green
                BadgeForeground = Brushes.White;
                break;
            case OrderStatus.CANCELED:
                StatusText = "取消済";
                BadgeBackground = new SolidColorBrush(Color.FromRgb(158, 158, 158)); // Gray
                BadgeForeground = Brushes.White;
                break;
            case OrderStatus.REJECTED:
                StatusText = "拒否";
                BadgeBackground = new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Red
                BadgeForeground = Brushes.White;
                break;
        }
    }
}
```

**使用例**
```xml
<local:StatusBadgeControl Status="{Binding Order.Status}" ShowIcon="True"/>
```

---

### 2. OrderEntryControlの実装

発注入力フォームの実装例です。

#### ViewModel

**OrderEntryViewModel.cs**
```csharp
using Prism.Commands;
using Prism.Mvvm;
using OMS.Core.Models;
using OMS.Core.DTOs;
using OMS.Core.Enums;
using OMS.Core.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace OMS.Client.ViewModels;

public class OrderEntryViewModel : BindableBase
{
    private readonly IOrderService _orderService;
    private readonly ISecurityService _securityService;

    public OrderEntryViewModel(
        IOrderService orderService,
        ISecurityService securityService)
    {
        _orderService = orderService;
        _securityService = securityService;

        SubmitOrderCommand = new DelegateCommand(OnSubmitOrder, CanSubmitOrder)
            .ObservesProperty(() => SelectedSecurity)
            .ObservesProperty(() => Quantity);

        SearchSecuritiesCommand = new DelegateCommand<string>(OnSearchSecurities);

        // 初期値設定
        Side = OrderSide.BUY;
        OrderType = OrderType.LIMIT;
        TimeInForce = TimeInForce.DAY;
    }

    // Properties
    private ObservableCollection<Security> _securities = new();
    public ObservableCollection<Security> Securities
    {
        get => _securities;
        set => SetProperty(ref _securities, value);
    }

    private Security? _selectedSecurity;
    public Security? SelectedSecurity
    {
        get => _selectedSecurity;
        set => SetProperty(ref _selectedSecurity, value);
    }

    private OrderSide _side;
    public OrderSide Side
    {
        get => _side;
        set => SetProperty(ref _side, value);
    }

    private OrderType _orderType;
    public OrderType OrderType
    {
        get => _orderType;
        set => SetProperty(ref _orderType, value);
    }

    private decimal _quantity;
    public decimal Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    private decimal? _price;
    public decimal? Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }

    private TimeInForce _timeInForce;
    public TimeInForce TimeInForce
    {
        get => _timeInForce;
        set => SetProperty(ref _timeInForce, value);
    }

    // Commands
    public ICommand SubmitOrderCommand { get; }
    public ICommand SearchSecuritiesCommand { get; }

    // Methods
    private async void OnSubmitOrder()
    {
        if (SelectedSecurity == null) return;

        var dto = new CreateOrderDto
        {
            SecurityId = SelectedSecurity.SecurityId,
            Side = Side,
            OrderType = OrderType,
            Quantity = Quantity,
            Price = Price,
            TimeInForce = TimeInForce
        };

        var result = await _orderService.CreateOrderAsync(dto);
        if (result.Success)
        {
            // 成功通知
            // イベント発行
            ResetForm();
        }
    }

    private bool CanSubmitOrder()
    {
        return SelectedSecurity != null && Quantity > 0;
    }

    private async void OnSearchSecurities(string? query)
    {
        if (string.IsNullOrWhiteSpace(query)) return;

        var result = await _securityService.SearchSecuritiesAsync(query);
        if (result.Success && result.Data != null)
        {
            Securities.Clear();
            foreach (var security in result.Data)
            {
                Securities.Add(new Security
                {
                    SecurityId = security.SecurityId,
                    SecurityCode = security.SecurityCode,
                    SecurityName = security.SecurityName
                });
            }
        }
    }

    private void ResetForm()
    {
        SelectedSecurity = null;
        Quantity = 0;
        Price = null;
    }
}
```

---

### 3. Spring Boot API - 他のエンティティの実装

`Order`エンティティと同様のパターンで実装してください。

#### Execution（約定）の実装例

**Execution.java** (簡略版)
```java
@Entity
@Table(name = "executions")
@Data
public class Execution {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long executionId;

    private Long orderId;
    private Long securityId;
    private String executionNo;
    private BigDecimal executionPrice;
    private BigDecimal executionQuantity;
    // ... その他のフィールド
}
```

**ExecutionRepository.java**
```java
@Repository
public interface ExecutionRepository extends JpaRepository<Execution, Long> {
    List<Execution> findByOrderId(Long orderId);
    List<Execution> findByExecutionDateBetween(LocalDateTime start, LocalDateTime end);
}
```

**ExecutionService.java**
```java
@Service
@RequiredArgsConstructor
public class ExecutionService {
    private final ExecutionRepository executionRepository;

    public List<ExecutionDto> getExecutionsByOrderId(Long orderId) {
        return executionRepository.findByOrderId(orderId)
            .stream()
            .map(this::convertToDto)
            .collect(Collectors.toList());
    }

    private ExecutionDto convertToDto(Execution execution) {
        // ModelMapper使用
    }
}
```

**ExecutionController.java**
```java
@RestController
@RequestMapping("/api/executions")
public class ExecutionController {
    private final ExecutionService executionService;

    @GetMapping
    public ResponseEntity<ApiResponse<List<ExecutionDto>>> getExecutions(
            @RequestParam(required = false) Long orderId) {
        // 実装
    }
}
```

---

### 4. ReoGridを使用したグリッドControlの実装

#### OrderListGrid の実装例

**OrderListGrid.xaml**
```xml
<UserControl x:Class="OMS.Client.Controls.Grids.OrderListGrid"
             xmlns:reo="clr-namespace:unvell.ReoGrid;assembly=unvell.ReoGrid"
             x:Name="root">
    <Grid>
        <reo:ReoGridControl x:Name="grid"
                           CurrentWorksheetChanged="Grid_CurrentWorksheetChanged"/>
    </Grid>
</UserControl>
```

**OrderListGrid.xaml.cs**
```csharp
using unvell.ReoGrid;
using OMS.Core.Models;

public partial class OrderListGrid : UserControl
{
    public OrderListGrid()
    {
        InitializeComponent();
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        var worksheet = grid.CurrentWorksheet;

        // 列定義
        worksheet.ColumnHeaders[0].Text = "注文番号";
        worksheet.ColumnHeaders[1].Text = "銘柄コード";
        worksheet.ColumnHeaders[2].Text = "銘柄名";
        worksheet.ColumnHeaders[3].Text = "売買";
        worksheet.ColumnHeaders[4].Text = "数量";
        worksheet.ColumnHeaders[5].Text = "価格";
        worksheet.ColumnHeaders[6].Text = "ステータス";

        // 列幅設定
        worksheet.SetColumnsWidth(0, 1, 120);  // 注文番号
        worksheet.SetColumnsWidth(1, 1, 80);   // 銘柄コード
        worksheet.SetColumnsWidth(2, 1, 200);  // 銘柄名
        // ...
    }

    public void LoadOrders(List<Order> orders)
    {
        var worksheet = grid.CurrentWorksheet;
        worksheet.Reset();

        int row = 0;
        foreach (var order in orders)
        {
            worksheet[row, 0] = order.OrderNo;
            worksheet[row, 1] = order.Security?.SecurityCode;
            worksheet[row, 2] = order.Security?.SecurityName;
            worksheet[row, 3] = order.Side.ToString();
            worksheet[row, 4] = order.Quantity;
            worksheet[row, 5] = order.Price;
            worksheet[row, 6] = order.Status.ToString();

            // ステータスに応じて行の色を変更
            var style = new WorksheetRangeStyle();
            style.BackColor = GetStatusColor(order.Status);
            worksheet.SetRangeStyles(new RangePosition(row, 0, 1, 7), style);

            row++;
        }
    }

    private Color GetStatusColor(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.NEW => Color.LightBlue,
            OrderStatus.PARTIAL => Color.LightYellow,
            OrderStatus.FILLED => Color.LightGreen,
            OrderStatus.CANCELED => Color.LightGray,
            _ => Color.White
        };
    }
}
```

---

### 5. AvalonDockを使用したメイン画面の実装

**MainWindow.xaml**
```xml
<Window x:Class="OMS.Client.Views.MainWindow"
        xmlns:avalonDock="https://github.com/Dirkster99/AvalonDock"
        Title="OMS - Order Management System" Height="900" Width="1600">
    <Grid>
        <avalonDock:DockingManager x:Name="dockManager">
            <avalonDock:LayoutRoot>
                <!-- 左側パネル: 発注入力 -->
                <avalonDock:LayoutPanel Orientation="Horizontal">
                    <avalonDock:LayoutAnchorablePane DockWidth="350">
                        <avalonDock:LayoutAnchorable Title="発注入力" CanClose="False">
                            <local:OrderEntryControl/>
                        </avalonDock:LayoutAnchorable>
                    </avalonDock:LayoutAnchorablePane>

                    <!-- 中央・右側 -->
                    <avalonDock:LayoutPanel Orientation="Vertical">
                        <!-- 中央: 発注一覧 -->
                        <avalonDock:LayoutDocumentPane DockHeight="*">
                            <avalonDock:LayoutDocument Title="発注一覧">
                                <local:OrderListGrid/>
                            </avalonDock:LayoutDocument>
                        </avalonDock:LayoutDocumentPane>

                        <!-- 下部: 約定一覧 -->
                        <avalonDock:LayoutAnchorablePane DockHeight="250">
                            <avalonDock:LayoutAnchorable Title="約定一覧">
                                <local:ExecutionListGrid/>
                            </avalonDock:LayoutAnchorable>
                        </avalonDock:LayoutAnchorablePane>
                    </avalonDock:LayoutPanel>

                    <!-- 右側: ポートフォリオ -->
                    <avalonDock:LayoutAnchorablePane DockWidth="350">
                        <avalonDock:LayoutAnchorable Title="ポートフォリオ">
                            <local:PortfolioSummaryControl/>
                        </avalonDock:LayoutAnchorable>
                    </avalonDock:LayoutAnchorablePane>
                </avalonDock:LayoutPanel>
            </avalonDock:LayoutRoot>
        </avalonDock:DockingManager>
    </Grid>
</Window>
```

---

## セットアップ手順

### 1. データベースセットアップ

```bash
# MySQLにログイン
mysql -u root -p

# スキーマ適用
mysql -u root -p < database/schema.sql

# サンプルデータ投入
mysql -u root -p < database/seed-data.sql
```

### 2. バックエンド起動

```bash
cd backend/oms-api

# ビルド
mvn clean install

# 起動
mvn spring-boot:run

# API確認
# Swagger UI: http://localhost:8080/swagger-ui.html
```

### 3. フロントエンド起動

```bash
cd src/OMS.Client

# パッケージ復元
dotnet restore

# ビルド
dotnet build

# 起動
dotnet run
```

---

## 開発のヒント

### 1. UserControlの開発パターン

全てのUserControlは以下のパターンに従ってください：

1. **DependencyProperty**でカスタマイズ可能にする
2. **イベント**で親コントロールと通信
3. **ViewModel**をDataContextとしてバインド
4. **Prism EventAggregator**で疎結合な通信

### 2. バリデーション

- `IDataErrorInfo`または`INotifyDataErrorInfo`を実装
- `ValidationRule`でカスタムバリデーション
- ViewModelでビジネスロジックバリデーション

### 3. 非同期処理

```csharp
private async Task LoadDataAsync()
{
    IsBusy = true;
    try
    {
        var result = await _orderService.GetOrdersAsync(filter);
        if (result.Success)
        {
            Orders = new ObservableCollection<Order>(result.Data);
        }
    }
    finally
    {
        IsBusy = false;
    }
}
```

### 4. エラーハンドリング

```csharp
try
{
    // API呼び出し
}
catch (HttpRequestException ex)
{
    // 通信エラー
    ShowErrorMessage("サーバーと通信できません");
}
catch (Exception ex)
{
    // その他のエラー
    _logger.LogError(ex, "Unexpected error");
    ShowErrorMessage("予期しないエラーが発生しました");
}
```

---

## まとめ

このスターターキットには以下が含まれています：

1. ✅ 完全な設計ドキュメント（PlantUML）
2. ✅ データベーススキーマとサンプルデータ
3. ✅ .NET Core基本構造（Enums、Models、DTOs、Interfaces）
4. ✅ 完全実装済みUserControl（2個）
5. ✅ Spring Boot完全実装（Order機能）
6. ✅ Prism設定とテーマ

残りのコンポーネントは、提供されているテンプレートとパターンに従って実装してください。
各コンポーネントは独立しているため、段階的に開発を進めることができます。
