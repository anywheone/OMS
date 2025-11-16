# WPF ユーザーコントロール 完全ガイド

## 目次
1. [ユーザーコントロールとは](#ユーザーコントロールとは)
2. [作成方法](#作成方法)
3. [依存関係プロパティ](#依存関係プロパティ)
4. [データバインディング](#データバインディング)
5. [イベント処理](#イベント処理)
6. [実装パターン](#実装パターン)
7. [ベストプラクティス](#ベストプラクティス)
8. [実装例](#実装例)

---

## ユーザーコントロールとは

### 定義
**ユーザーコントロール**は、複数のWPFコントロールを組み合わせて、再利用可能な独自のコントロールを作成する仕組みです。

### メリット
- **再利用性**: 一度作れば何度でも使える
- **保守性**: 変更が一箇所で済む
- **カプセル化**: 内部構造を隠蔽できる
- **読みやすさ**: XAMLがシンプルになる

### 使用場面
- 同じUIパターンが複数回登場する
- 複雑なUIをモジュール化したい
- チーム間でコントロールを共有したい

---

## 作成方法

### 1. Visual Studioでの作成

**手順:**
1. プロジェクトを右クリック → 追加 → 新しい項目
2. 「ユーザー コントロール (WPF)」を選択
3. 名前を入力（例: `OrderTicket.xaml`）

### 2. 基本構造

#### OrderTicket.xaml
```xml
<UserControl x:Class="OMS.Client.Controls.OrderTicket"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d"
             d:DesignHeight="300" d:DesignWidth="400">
    <Grid>
        <!-- コントロールの内容 -->
        <StackPanel>
            <TextBlock Text="銘柄コード"/>
            <TextBox x:Name="SecurityIdTextBox"/>

            <TextBlock Text="数量"/>
            <TextBox x:Name="QuantityTextBox"/>

            <Button Content="発注" Click="PlaceOrder_Click"/>
        </StackPanel>
    </Grid>
</UserControl>
```

#### OrderTicket.xaml.cs
```csharp
using System.Windows;
using System.Windows.Controls;

namespace OMS.Client.Controls
{
    public partial class OrderTicket : UserControl
    {
        public OrderTicket()
        {
            InitializeComponent();
        }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            // ボタンクリック時の処理
            var securityId = SecurityIdTextBox.Text;
            var quantity = QuantityTextBox.Text;

            // イベントを発行するか、コマンドを実行
        }
    }
}
```

### 3. 使用方法

```xml
<Window xmlns:controls="clr-namespace:OMS.Client.Controls">
    <StackPanel>
        <!-- ユーザーコントロールの使用 -->
        <controls:OrderTicket/>
        <controls:OrderTicket/>
    </StackPanel>
</Window>
```

---

## 依存関係プロパティ

### 依存関係プロパティとは

**通常のプロパティ + データバインディング、アニメーション、スタイル設定などの機能**

### 通常のプロパティ vs 依存関係プロパティ

#### 通常のプロパティ（使えない機能が多い）
```csharp
public class OrderTicket : UserControl
{
    // データバインディングが効かない
    public string SecurityId { get; set; }
}
```

```xml
<!-- これは動かない -->
<controls:OrderTicket SecurityId="{Binding SelectedSecurityId}"/>
```

#### 依存関係プロパティ（推奨）
```csharp
public class OrderTicket : UserControl
{
    // 依存関係プロパティの定義
    public static readonly DependencyProperty SecurityIdProperty =
        DependencyProperty.Register(
            nameof(SecurityId),           // プロパティ名
            typeof(string),               // プロパティの型
            typeof(OrderTicket),          // 所有者の型
            new PropertyMetadata(string.Empty, OnSecurityIdChanged));  // デフォルト値と変更時コールバック

    // CLRプロパティのラッパー
    public string SecurityId
    {
        get => (string)GetValue(SecurityIdProperty);
        set => SetValue(SecurityIdProperty, value);
    }

    // 値が変更された時の処理
    private static void OnSecurityIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (OrderTicket)d;
        var newValue = (string)e.NewValue;
        var oldValue = (string)e.OldValue;

        // 変更時の処理
        control.UpdateSecurityIdDisplay();
    }

    private void UpdateSecurityIdDisplay()
    {
        // UI更新処理
    }
}
```

```xml
<!-- これは動く -->
<controls:OrderTicket SecurityId="{Binding SelectedSecurityId}"/>
```

### 依存関係プロパティの作り方（スニペット）

Visual Studioで `propdp` と入力してTabキーを押すと自動生成されます:

```csharp
public int MyProperty
{
    get { return (int)GetValue(MyPropertyProperty); }
    set { SetValue(MyPropertyProperty, value); }
}

public static readonly DependencyProperty MyPropertyProperty =
    DependencyProperty.Register("MyProperty", typeof(int), typeof(ownerclass), new PropertyMetadata(0));
```

---

## データバインディング

### コントロール内部でのバインディング

#### 方法1: ElementName（要素名でバインド）
```xml
<UserControl>
    <StackPanel>
        <TextBox x:Name="InputTextBox"/>
        <TextBlock Text="{Binding ElementName=InputTextBox, Path=Text}"/>
    </StackPanel>
</UserControl>
```

#### 方法2: RelativeSource（相対的にバインド）
```xml
<UserControl>
    <StackPanel>
        <!-- 親UserControlのプロパティにバインド -->
        <TextBlock Text="{Binding Path=SecurityId,
                          RelativeSource={RelativeSource AncestorType=UserControl}}"/>
    </StackPanel>
</UserControl>
```

#### 方法3: DataContextを使う
```xml
<UserControl x:Name="Root">
    <StackPanel DataContext="{Binding ElementName=Root}">
        <!-- Rootの依存関係プロパティにバインド -->
        <TextBlock Text="{Binding SecurityId}"/>
        <TextBlock Text="{Binding Quantity}"/>
    </StackPanel>
</UserControl>
```

---

## イベント処理

### 方法1: コードビハインドでイベント処理

```xml
<UserControl>
    <Button Content="発注" Click="PlaceOrder_Click"/>
</UserControl>
```

```csharp
private void PlaceOrder_Click(object sender, RoutedEventArgs e)
{
    // ルーティングイベントを発行
    RaiseEvent(new RoutedEventArgs(OrderPlacedEvent));
}
```

### 方法2: カスタムルーティングイベント

```csharp
public class OrderTicket : UserControl
{
    // ルーティングイベントの定義
    public static readonly RoutedEvent OrderPlacedEvent =
        EventManager.RegisterRoutedEvent(
            "OrderPlaced",                    // イベント名
            RoutingStrategy.Bubble,           // バブリング戦略
            typeof(RoutedEventHandler),       // ハンドラの型
            typeof(OrderTicket));             // 所有者の型

    // CLRイベントのラッパー
    public event RoutedEventHandler OrderPlaced
    {
        add { AddHandler(OrderPlacedEvent, value); }
        remove { RemoveHandler(OrderPlacedEvent, value); }
    }

    private void PlaceOrder_Click(object sender, RoutedEventArgs e)
    {
        // イベントを発行
        RaiseEvent(new RoutedEventArgs(OrderPlacedEvent));
    }
}
```

**使用側:**
```xml
<controls:OrderTicket OrderPlaced="OrderTicket_OrderPlaced"/>
```

```csharp
private void OrderTicket_OrderPlaced(object sender, RoutedEventArgs e)
{
    MessageBox.Show("注文が発注されました");
}
```

### 方法3: コマンドを使う（MVVM推奨）

```csharp
public class OrderTicket : UserControl
{
    // コマンドの依存関係プロパティ
    public static readonly DependencyProperty PlaceOrderCommandProperty =
        DependencyProperty.Register(
            nameof(PlaceOrderCommand),
            typeof(ICommand),
            typeof(OrderTicket));

    public ICommand PlaceOrderCommand
    {
        get => (ICommand)GetValue(PlaceOrderCommandProperty);
        set => SetValue(PlaceOrderCommandProperty, value);
    }
}
```

```xml
<!-- コントロール内部 -->
<Button Content="発注" Command="{Binding PlaceOrderCommand,
                                 RelativeSource={RelativeSource AncestorType=UserControl}}"/>

<!-- 使用側 -->
<controls:OrderTicket PlaceOrderCommand="{Binding PlaceOrderCommand}"/>
```

---

## 実装パターン

### パターン1: シンプルな表示専用コントロール

**用途:** データを表示するだけ

```csharp
public partial class OrderCard : UserControl
{
    public static readonly DependencyProperty OrderNoProperty =
        DependencyProperty.Register(nameof(OrderNo), typeof(long), typeof(OrderCard));

    public static readonly DependencyProperty SecurityIdProperty =
        DependencyProperty.Register(nameof(SecurityId), typeof(long), typeof(OrderCard));

    public long OrderNo
    {
        get => (long)GetValue(OrderNoProperty);
        set => SetValue(OrderNoProperty, value);
    }

    public long SecurityId
    {
        get => (long)GetValue(SecurityIdProperty);
        set => SetValue(SecurityIdProperty, value);
    }

    public OrderCard()
    {
        InitializeComponent();
    }
}
```

```xml
<UserControl x:Name="Root">
    <Border BorderBrush="Gray" BorderThickness="1" Padding="10">
        <StackPanel DataContext="{Binding ElementName=Root}">
            <TextBlock Text="{Binding OrderNo, StringFormat='注文番号: {0}'}"/>
            <TextBlock Text="{Binding SecurityId, StringFormat='銘柄: {0}'}"/>
        </StackPanel>
    </Border>
</UserControl>
```

### パターン2: 入力フォームコントロール

**用途:** データ入力

```csharp
public partial class OrderInputForm : UserControl
{
    // 依存関係プロパティ
    public static readonly DependencyProperty SecurityIdProperty =
        DependencyProperty.Register(nameof(SecurityId), typeof(string), typeof(OrderInputForm),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string SecurityId
    {
        get => (string)GetValue(SecurityIdProperty);
        set => SetValue(SecurityIdProperty, value);
    }

    // コマンドプロパティ
    public static readonly DependencyProperty SubmitCommandProperty =
        DependencyProperty.Register(nameof(SubmitCommand), typeof(ICommand), typeof(OrderInputForm));

    public ICommand SubmitCommand
    {
        get => (ICommand)GetValue(SubmitCommandProperty);
        set => SetValue(SubmitCommandProperty, value);
    }

    public OrderInputForm()
    {
        InitializeComponent();
    }
}
```

```xml
<UserControl x:Name="Root">
    <StackPanel DataContext="{Binding ElementName=Root}">
        <TextBox Text="{Binding SecurityId, UpdateSourceTrigger=PropertyChanged}"/>
        <Button Content="送信" Command="{Binding SubmitCommand}"/>
    </StackPanel>
</UserControl>
```

### パターン3: ViewModelを持つコントロール

**用途:** 複雑なロジックを持つコントロール

```csharp
// ViewModel
public class MarketDepthViewModel : BindableBase
{
    private ObservableCollection<PriceLevel> _bids = new();
    private ObservableCollection<PriceLevel> _asks = new();

    public ObservableCollection<PriceLevel> Bids
    {
        get => _bids;
        set => SetProperty(ref _bids, value);
    }

    public ObservableCollection<PriceLevel> Asks
    {
        get => _asks;
        set => SetProperty(ref _asks, value);
    }

    public void LoadMarketData(long securityId)
    {
        // データ取得処理
    }
}

// UserControl
public partial class MarketDepth : UserControl
{
    private readonly MarketDepthViewModel _viewModel;

    public MarketDepth()
    {
        InitializeComponent();
        _viewModel = new MarketDepthViewModel();
        DataContext = _viewModel;
    }

    public static readonly DependencyProperty SecurityIdProperty =
        DependencyProperty.Register(nameof(SecurityId), typeof(long), typeof(MarketDepth),
            new PropertyMetadata(0L, OnSecurityIdChanged));

    public long SecurityId
    {
        get => (long)GetValue(SecurityIdProperty);
        set => SetValue(SecurityIdProperty, value);
    }

    private static void OnSecurityIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (MarketDepth)d;
        control._viewModel.LoadMarketData((long)e.NewValue);
    }
}
```

---

## ベストプラクティス

### 1. 依存関係プロパティを使う

```csharp
// ✅ 良い例
public static readonly DependencyProperty TitleProperty =
    DependencyProperty.Register(nameof(Title), typeof(string), typeof(MyControl));

public string Title
{
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
}

// ❌ 悪い例（バインディングできない）
public string Title { get; set; }
```

### 2. x:Nameを使って内部要素を参照

```xml
<UserControl>
    <TextBox x:Name="InputTextBox"/>
</UserControl>
```

```csharp
private void SomeMethod()
{
    var text = InputTextBox.Text;  // 直接アクセス可能
}
```

### 3. DataContextの設定は慎重に

```csharp
// ❌ 悪い例（親のDataContextを上書きしてしまう）
public MyControl()
{
    InitializeComponent();
    DataContext = this;  // 親のバインディングが効かなくなる
}

// ✅ 良い例
public MyControl()
{
    InitializeComponent();
    // DataContextは設定しない
    // 代わりに ElementName や RelativeSource を使う
}
```

### 4. FrameworkPropertyMetadataOptions.BindsTwoWayByDefault

双方向バインディングが必要なプロパティには設定:

```csharp
public static readonly DependencyProperty ValueProperty =
    DependencyProperty.Register(nameof(Value), typeof(string), typeof(MyControl),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));  // 双方向デフォルト
```

### 5. 命名規則

| 種類 | 命名規則 | 例 |
|------|---------|-----|
| ファイル名 | パスカルケース | `OrderTicket.xaml` |
| クラス名 | パスカルケース | `OrderTicket` |
| 依存関係プロパティ | `{名前}Property` | `SecurityIdProperty` |
| ルーティングイベント | `{名前}Event` | `OrderPlacedEvent` |

---

## 実装例

### 例1: バリデーション付きテキスト入力

```csharp
public partial class ValidatedTextBox : UserControl
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(ValidatedTextBox),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnTextChanged));

    public static readonly DependencyProperty ValidationRuleProperty =
        DependencyProperty.Register(nameof(ValidationRule), typeof(Func<string, bool>), typeof(ValidatedTextBox));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public Func<string, bool> ValidationRule
    {
        get => (Func<string, bool>)GetValue(ValidationRuleProperty);
        set => SetValue(ValidationRuleProperty, value);
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ValidatedTextBox)d;
        var text = (string)e.NewValue;

        if (control.ValidationRule != null)
        {
            var isValid = control.ValidationRule(text);
            control.UpdateValidationState(isValid);
        }
    }

    private void UpdateValidationState(bool isValid)
    {
        ErrorBorder.Visibility = isValid ? Visibility.Collapsed : Visibility.Visible;
    }

    public ValidatedTextBox()
    {
        InitializeComponent();
    }
}
```

```xml
<UserControl x:Name="Root">
    <Grid>
        <TextBox Text="{Binding Text, ElementName=Root, UpdateSourceTrigger=PropertyChanged}"/>
        <Border x:Name="ErrorBorder"
                BorderBrush="Red"
                BorderThickness="2"
                Visibility="Collapsed"/>
    </Grid>
</UserControl>
```

**使用例:**
```xml
<controls:ValidatedTextBox
    Text="{Binding Email}"
    ValidationRule="{Binding EmailValidationRule}"/>
```

### 例2: 数値入力コントロール

```csharp
public partial class NumericUpDown : UserControl
{
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(decimal), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(0m, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(decimal), typeof(NumericUpDown),
            new PropertyMetadata(decimal.MinValue));

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(decimal), typeof(NumericUpDown),
            new PropertyMetadata(decimal.MaxValue));

    public decimal Value
    {
        get => (decimal)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public decimal Minimum
    {
        get => (decimal)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public decimal Maximum
    {
        get => (decimal)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public NumericUpDown()
    {
        InitializeComponent();
    }

    private void UpButton_Click(object sender, RoutedEventArgs e)
    {
        if (Value < Maximum)
            Value++;
    }

    private void DownButton_Click(object sender, RoutedEventArgs e)
    {
        if (Value > Minimum)
            Value--;
    }
}
```

```xml
<UserControl x:Name="Root">
    <StackPanel Orientation="Horizontal">
        <TextBox Text="{Binding Value, ElementName=Root}" Width="100"/>
        <Button Content="▲" Click="UpButton_Click"/>
        <Button Content="▼" Click="DownButton_Click"/>
    </StackPanel>
</UserControl>
```

---

## まとめ

### ユーザーコントロールの要点

1. **再利用性**: 同じUIを何度も使える
2. **依存関係プロパティ**: データバインディングを有効にする
3. **カスタムイベント**: 外部とのコミュニケーション
4. **カプセル化**: 内部実装を隠蔽

### 作成フロー

```
1. UserControlを作成
   ↓
2. UIをXAMLで定義
   ↓
3. 依存関係プロパティを定義
   ↓
4. イベントまたはコマンドを定義
   ↓
5. 内部バインディングを設定
   ↓
6. 使用側でバインディング
```

### 参考リンク

- [依存関係プロパティの概要](https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/properties/dependency-properties-overview)
- [ルーティングイベントの概要](https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/events/routed-events-overview)

---

**作成日:** 2025-11-16
**プロジェクト:** OMS (Order Management System)
