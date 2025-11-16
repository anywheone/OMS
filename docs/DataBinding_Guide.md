# WPF データバインディング 完全ガイド

## 目次
1. [データバインディングとは](#データバインディングとは)
2. [バインディングモード](#バインディングモード)
3. [バインディングソース](#バインディングソース)
4. [値の変換](#値の変換)
5. [バリデーション](#バリデーション)
6. [コレクションバインディング](#コレクションバインディング)
7. [高度なテクニック](#高度なテクニック)
8. [トラブルシューティング](#トラブルシューティング)

---

## データバインディングとは

### 定義
**データバインディング**は、UIとデータを自動的に同期させる仕組みです。

### メリット

- **自動同期**: データ変更が自動でUIに反映
- **コード削減**: 手動でのUI更新が不要
- **疎結合**: UIとロジックの分離
- **双方向**: UI ⇄ データの相互更新

### 基本構文

```xml
<TextBox Text="{Binding PropertyName}"/>
```

```
┌──────────┐          ┌─────────────┐
│   View   │ ⇄ Binding ⇄ │ ViewModel │
└──────────┘          └─────────────┘
```

---

## バインディングモード

### 4つのモード

#### 1. OneWay（一方向: Source → Target）

```xml
<!-- ViewModelの変更がViewに反映 -->
<TextBlock Text="{Binding OrderCount, Mode=OneWay}"/>
```

**用途:** 読み取り専用の表示

#### 2. TwoWay（双方向: Source ⇄ Target）

```xml
<!-- 相互に反映 -->
<TextBox Text="{Binding UserName, Mode=TwoWay}"/>
```

**用途:** 入力フォーム

#### 3. OneWayToSource（Source ← Target）

```xml
<!-- Viewの変更のみViewModelに反映 -->
<Slider Value="{Binding Volume, Mode=OneWayToSource}"/>
```

**用途:** 入力専用コントロール

#### 4. OneTime（一度だけ）

```xml
<!-- 初期値のみ設定 -->
<TextBlock Text="{Binding WelcomeMessage, Mode=OneTime}"/>
```

**用途:** 静的なテキスト

### デフォルトモード

| コントロール | デフォルトモード |
|-------------|----------------|
| TextBlock | OneWay |
| TextBox.Text | TwoWay |
| CheckBox.IsChecked | TwoWay |
| ComboBox.SelectedItem | TwoWay |
| ListBox.SelectedItem | TwoWay |

---

## バインディングソース

### 1. DataContext

最も一般的な方法。

```xml
<Window DataContext="{Binding MainWindowViewModel}">
    <!-- 自動的にMainWindowViewModelのプロパティにバインド -->
    <TextBox Text="{Binding SecurityId}"/>
</Window>
```

```csharp
public class MainWindowViewModel : BindableBase
{
    private string _securityId;

    public string SecurityId
    {
        get => _securityId;
        set => SetProperty(ref _securityId, value);
    }
}
```

### 2. ElementName

他の要素を参照。

```xml
<StackPanel>
    <TextBox x:Name="SourceTextBox" Text="こんにちは"/>
    <TextBlock Text="{Binding ElementName=SourceTextBox, Path=Text}"/>
</StackPanel>
```

### 3. RelativeSource

相対的な要素を参照。

#### Self（自分自身）
```xml
<TextBox Background="LightBlue"
         Text="{Binding RelativeSource={RelativeSource Self}, Path=Background}"/>
```

#### FindAncestor（祖先を検索）
```xml
<UserControl x:Name="MyControl">
    <StackPanel>
        <!-- 親UserControlのプロパティを参照 -->
        <TextBlock Text="{Binding Path=Title,
                          RelativeSource={RelativeSource AncestorType=UserControl}}"/>
    </StackPanel>
</UserControl>
```

#### TemplatedParent（テンプレート親）
```xml
<ControlTemplate TargetType="Button">
    <Border Background="{TemplateBinding Background}">
        <!-- これと同じ -->
        <ContentPresenter Content="{Binding RelativeSource={RelativeSource TemplatedParent}, Path=Content}"/>
    </Border>
</ControlTemplate>
```

### 4. Source

直接オブジェクトを指定。

```xml
<Window.Resources>
    <local:SettingsManager x:Key="Settings"/>
</Window.Resources>

<TextBox Text="{Binding Source={StaticResource Settings}, Path=UserName}"/>
```

### 5. StaticResource / DynamicResource

リソースを参照。

```xml
<Window.Resources>
    <SolidColorBrush x:Key="PrimaryBrush" Color="Blue"/>
</Window.Resources>

<Button Background="{StaticResource PrimaryBrush}"/>
```

---

## 値の変換

### IValueConverter

異なる型間の変換。

#### コンバーターの実装

```csharp
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        return false;
    }
}
```

#### 使用方法

```xml
<Window.Resources>
    <local:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter"/>
</Window.Resources>

<TextBlock Text="エラー"
           Foreground="Red"
           Visibility="{Binding HasError, Converter={StaticResource BoolToVisibilityConverter}}"/>
```

### よく使うコンバーター

#### 1. BoolToVisibilityConverter

```csharp
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (Visibility)value == Visibility.Visible;
    }
}
```

#### 2. InverseBoolConverter

```csharp
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value;
    }
}
```

#### 3. StringFormatConverter

```csharp
public class StringFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is string format)
        {
            return string.Format(format, value);
        }
        return value?.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

```xml
<TextBlock Text="{Binding Price, Converter={StaticResource StringFormatConverter}, ConverterParameter='¥{0:N0}'}"/>
```

### StringFormat

簡単な書式設定にはStringFormatを使用。

```xml
<!-- 数値フォーマット -->
<TextBlock Text="{Binding Price, StringFormat='¥{0:N0}'}"/>  <!-- ¥1,000 -->
<TextBlock Text="{Binding Quantity, StringFormat='{}{0}株'}"/>  <!-- 100株 -->

<!-- 日付フォーマット -->
<TextBlock Text="{Binding OrderDate, StringFormat='yyyy/MM/dd HH:mm'}"/>

<!-- 複数値 -->
<TextBlock>
    <TextBlock.Text>
        <MultiBinding StringFormat="{}{0} {1}">
            <Binding Path="FirstName"/>
            <Binding Path="LastName"/>
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>
```

### MultiValueConverter

複数の値を組み合わせる。

```csharp
public class AdditionConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        double sum = 0;
        foreach (var value in values)
        {
            if (value is double d)
                sum += d;
        }
        return sum;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

```xml
<TextBlock>
    <TextBlock.Text>
        <MultiBinding Converter="{StaticResource AdditionConverter}">
            <Binding Path="Value1"/>
            <Binding Path="Value2"/>
            <Binding Path="Value3"/>
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>
```

---

## バリデーション

### IDataErrorInfo

プロパティごとのバリデーション。

```csharp
public class OrderInputViewModel : BindableBase, IDataErrorInfo
{
    private string _quantity;

    public string Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    public string Error => null;

    public string this[string columnName]
    {
        get
        {
            switch (columnName)
            {
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
<TextBox Text="{Binding Quantity, ValidatesOnDataErrors=True, UpdateSourceTrigger=PropertyChanged}">
    <TextBox.Style>
        <Style TargetType="TextBox">
            <Style.Triggers>
                <Trigger Property="Validation.HasError" Value="True">
                    <Setter Property="BorderBrush" Value="Red"/>
                    <Setter Property="BorderThickness" Value="2"/>
                </Trigger>
            </Style.Triggers>
        </Style>
    </TextBox.Style>
</TextBox>

<!-- エラーメッセージ表示 -->
<TextBlock Text="{Binding ElementName=QuantityTextBox, Path=(Validation.Errors)[0].ErrorContent}"
           Foreground="Red"/>
```

### INotifyDataErrorInfo

非同期バリデーション対応。

```csharp
public class OrderInputViewModel : BindableBase, INotifyDataErrorInfo
{
    private Dictionary<string, List<string>> _errors = new();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public bool HasErrors => _errors.Any();

    public IEnumerable GetErrors(string propertyName)
    {
        if (_errors.ContainsKey(propertyName))
            return _errors[propertyName];
        return null;
    }

    private void AddError(string propertyName, string error)
    {
        if (!_errors.ContainsKey(propertyName))
            _errors[propertyName] = new List<string>();

        if (!_errors[propertyName].Contains(error))
        {
            _errors[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }
    }

    private void ClearErrors(string propertyName)
    {
        if (_errors.ContainsKey(propertyName))
        {
            _errors.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}
```

---

## コレクションバインディング

### ObservableCollection

変更通知付きコレクション。

```csharp
public class MainWindowViewModel : BindableBase
{
    // 変更が自動でUIに反映される
    public ObservableCollection<OrderModel> Orders { get; } = new();

    public void AddOrder(OrderModel order)
    {
        Orders.Add(order);  // UIが自動更新
    }

    public void RemoveOrder(OrderModel order)
    {
        Orders.Remove(order);  // UIが自動更新
    }
}
```

```xml
<ListBox ItemsSource="{Binding Orders}" DisplayMemberPath="OrderNo"/>

<DataGrid ItemsSource="{Binding Orders}" AutoGenerateColumns="False">
    <DataGrid.Columns>
        <DataGridTextColumn Header="注文番号" Binding="{Binding OrderNo}"/>
        <DataGridTextColumn Header="数量" Binding="{Binding Quantity}"/>
    </DataGrid.Columns>
</DataGrid>
```

### ItemTemplate

各アイテムの表示をカスタマイズ。

```xml
<ListBox ItemsSource="{Binding Orders}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <Border BorderBrush="Gray" BorderThickness="1" Padding="10" Margin="5">
                <StackPanel>
                    <TextBlock Text="{Binding OrderNo, StringFormat='注文番号: {0}'}" FontWeight="Bold"/>
                    <TextBlock Text="{Binding SecurityId, StringFormat='銘柄: {0}'}"/>
                    <TextBlock Text="{Binding Quantity, StringFormat='数量: {0}'}"/>
                </StackPanel>
            </Border>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

### SelectedItem

選択されたアイテムをバインド。

```csharp
public class OrderListViewModel : BindableBase
{
    private OrderModel _selectedOrder;

    public ObservableCollection<OrderModel> Orders { get; } = new();

    public OrderModel SelectedOrder
    {
        get => _selectedOrder;
        set => SetProperty(ref _selectedOrder, value);
    }
}
```

```xml
<ListBox ItemsSource="{Binding Orders}"
         SelectedItem="{Binding SelectedOrder}"/>

<StackPanel DataContext="{Binding SelectedOrder}">
    <TextBlock Text="{Binding OrderNo}"/>
    <TextBlock Text="{Binding SecurityId}"/>
</StackPanel>
```

---

## 高度なテクニック

### UpdateSourceTrigger

いつバインディングを更新するか。

```xml
<!-- 入力の度に更新（リアルタイム検索など） -->
<TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}"/>

<!-- フォーカスを失った時に更新（デフォルト） -->
<TextBox Text="{Binding Email, UpdateSourceTrigger=LostFocus}"/>

<!-- 明示的に更新 -->
<TextBox x:Name="MyTextBox" Text="{Binding Value, UpdateSourceTrigger=Explicit}"/>
```

```csharp
// 明示的更新
var binding = MyTextBox.GetBindingExpression(TextBox.TextProperty);
binding.UpdateSource();
```

### FallbackValue

バインディング失敗時のデフォルト値。

```xml
<TextBlock Text="{Binding MissingProperty, FallbackValue='データなし'}"/>
```

### TargetNullValue

ソースがnullの時の値。

```xml
<TextBlock Text="{Binding OrderNo, TargetNullValue='未設定'}"/>
```

### Delay

入力から更新までの遅延。

```xml
<!-- 500ms後に更新（リアルタイム検索の負荷軽減） -->
<TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged, Delay=500}"/>
```

### PriorityBinding

複数のバインディングを優先順位付けで実行。

```xml
<TextBlock>
    <TextBlock.Text>
        <PriorityBinding>
            <Binding Path="FastProperty" IsAsync="False"/>
            <Binding Path="SlowProperty" IsAsync="True"/>
        </PriorityBinding>
    </TextBlock.Text>
</TextBlock>
```

---

## トラブルシューティング

### バインディングエラーの確認

#### 出力ウィンドウ

Visual Studioの出力ウィンドウにバインディングエラーが表示されます。

```
System.Windows.Data Error: 40 : BindingExpression path error: 'InvalidProperty' property not found
```

#### DebugConverterを使う

```csharp
public class DebugConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Debug.WriteLine($"Convert: {value}");
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Debug.WriteLine($"ConvertBack: {value}");
        return value;
    }
}
```

### よくある問題

#### 1. バインディングが効かない

```xml
<!-- ❌ 悪い例（Pathが間違っている） -->
<TextBox Text="{Binding SecuirtyId}"/>  <!-- タイポ -->

<!-- ✅ 良い例 -->
<TextBox Text="{Binding SecurityId}"/>
```

#### 2. 変更通知が発火しない

```csharp
// ❌ 悪い例
public string SecurityId { get; set; }  // 変更通知なし

// ✅ 良い例
private string _securityId;
public string SecurityId
{
    get => _securityId;
    set => SetProperty(ref _securityId, value);  // 変更通知あり
}
```

#### 3. DataContextが正しく設定されていない

```csharp
// ❌ 悪い例
public MainWindow()
{
    InitializeComponent();
    // DataContextが設定されていない
}

// ✅ 良い例
public MainWindow()
{
    InitializeComponent();
    DataContext = new MainWindowViewModel();
}
```

---

## まとめ

### データバインディングの要点

| 項目 | 説明 |
|------|------|
| **Mode** | OneWay, TwoWay, OneWayToSource, OneTime |
| **Source** | DataContext, ElementName, RelativeSource |
| **Converter** | 型変換 |
| **Validation** | IDataErrorInfo, INotifyDataErrorInfo |
| **UpdateSourceTrigger** | PropertyChanged, LostFocus, Explicit |

### バインディング構文まとめ

```xml
<!-- 基本 -->
<TextBox Text="{Binding PropertyName}"/>

<!-- モード指定 -->
<TextBox Text="{Binding PropertyName, Mode=TwoWay}"/>

<!-- 更新タイミング -->
<TextBox Text="{Binding PropertyName, UpdateSourceTrigger=PropertyChanged}"/>

<!-- コンバーター -->
<TextBox Visibility="{Binding IsVisible, Converter={StaticResource BoolToVisibilityConverter}}"/>

<!-- フォーマット -->
<TextBlock Text="{Binding Price, StringFormat='¥{0:N0}'}"/>

<!-- ElementName -->
<TextBlock Text="{Binding ElementName=MyTextBox, Path=Text}"/>

<!-- RelativeSource -->
<TextBlock Text="{Binding RelativeSource={RelativeSource AncestorType=UserControl}, Path=Title}"/>
```

---

**作成日:** 2025-11-16
**プロジェクト:** OMS (Order Management System)
