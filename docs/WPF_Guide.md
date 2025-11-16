# WPF (Windows Presentation Foundation) 完全ガイド

## 目次
1. [WPFとは](#wpfとは)
2. [WPFの基本概念](#wpfの基本概念)
3. [レイアウト](#レイアウト)
4. [コントロール](#コントロール)
5. [リソース](#リソース)
6. [スタイルとテンプレート](#スタイルとテンプレート)
7. [アニメーション](#アニメーション)
8. [ベストプラクティス](#ベストプラクティス)

---

## WPFとは

### 定義
**WPF (Windows Presentation Foundation)** は、Windowsデスクトップアプリケーションを構築するための.NETフレームワークです。

### WinFormsとの違い

| 特徴 | WinForms | WPF |
|------|----------|-----|
| **描画** | GDI/GDI+ | DirectX |
| **UI定義** | C#コード | XAML + C# |
| **レイアウト** | 絶対座標 | 動的レイアウト |
| **デザイン** | 制限あり | 自由度高い |
| **データバインディング** | 基本的 | 強力 |
| **パフォーマンス** | 標準 | 高速（GPU活用） |

### WPFの特徴

1. **XAML**: 宣言的なUI定義
2. **データバインディング**: Model-Viewの自動同期
3. **スタイルとテンプレート**: 柔軟なカスタマイズ
4. **リソース**: 再利用可能な定義
5. **アニメーション**: 組み込みアニメーション機能
6. **DirectX**: GPU加速による高速描画

---

## WPFの基本概念

### 1. ビジュアルツリー

WPFのUIは**ビジュアルツリー**として構築されます。

```xml
<Window>
    <Grid>
        <StackPanel>
            <TextBlock/>
            <Button/>
        </StackPanel>
    </Grid>
</Window>
```

```
Window
└── Grid
    └── StackPanel
        ├── TextBlock
        └── Button
```

### 2. 論理ツリーと視覚ツリー

- **論理ツリー**: XAMLで定義した要素
- **視覚ツリー**: 実際に描画される要素（テンプレートを含む）

### 3. 依存関係プロパティ

WPFコントロールのプロパティは**依存関係プロパティ**として実装されています。

**特徴:**
- データバインディング対応
- アニメーション対応
- スタイル設定可能
- 値の継承

```csharp
// 依存関係プロパティの定義
public static readonly DependencyProperty TitleProperty =
    DependencyProperty.Register("Title", typeof(string), typeof(MyControl));

public string Title
{
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
}
```

### 4. ルーティングイベント

イベントがビジュアルツリーを伝播する仕組み。

**3つの戦略:**
- **Bubbling**: 子 → 親へ伝播（デフォルト）
- **Tunneling**: 親 → 子へ伝播（PreviewXXX）
- **Direct**: 発生元のみ

```xml
<StackPanel Button.Click="StackPanel_Click">
    <Button Content="ボタン1" Click="Button_Click"/>
    <Button Content="ボタン2" Click="Button_Click"/>
</StackPanel>
```

---

## レイアウト

### レイアウトパネルの種類

#### 1. Grid（グリッド）

行と列で領域を分割。最も汎用的。

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>    <!-- 内容に応じた高さ -->
        <RowDefinition Height="*"/>       <!-- 残りすべて -->
        <RowDefinition Height="100"/>     <!-- 固定100px -->
    </Grid.RowDefinitions>

    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="200"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>

    <TextBlock Grid.Row="0" Grid.Column="0" Text="ヘッダー"/>
    <TextBox Grid.Row="1" Grid.Column="0"/>
    <DataGrid Grid.Row="1" Grid.Column="1"/>
</Grid>
```

**使用場面:** 複雑なレイアウト全般

#### 2. StackPanel（スタックパネル）

要素を縦または横に並べる。

```xml
<!-- 縦方向 -->
<StackPanel Orientation="Vertical">
    <TextBlock Text="ラベル1"/>
    <TextBox/>
    <TextBlock Text="ラベル2"/>
    <TextBox/>
</StackPanel>

<!-- 横方向 -->
<StackPanel Orientation="Horizontal">
    <Button Content="OK"/>
    <Button Content="キャンセル"/>
</StackPanel>
```

**使用場面:** シンプルな縦/横並び

#### 3. DockPanel（ドックパネル）

要素を上下左右に配置。

```xml
<DockPanel>
    <Menu DockPanel.Dock="Top"/>
    <StatusBar DockPanel.Dock="Bottom"/>
    <TreeView DockPanel.Dock="Left" Width="200"/>
    <TextBox/>  <!-- 残りすべて -->
</DockPanel>
```

**使用場面:** 上下左右の固定領域 + メインコンテンツ

#### 4. WrapPanel（ラップパネル）

要素を折り返しながら配置。

```xml
<WrapPanel>
    <Button Content="ボタン1"/>
    <Button Content="ボタン2"/>
    <Button Content="ボタン3"/>
    <!-- ウィンドウ幅に応じて折り返す -->
</WrapPanel>
```

**使用場面:** タグ、ボタン群

#### 5. Canvas（キャンバス）

絶対座標で配置。

```xml
<Canvas>
    <Button Canvas.Left="50" Canvas.Top="100" Content="ボタン"/>
    <Ellipse Canvas.Left="200" Canvas.Top="150" Width="100" Height="100" Fill="Blue"/>
</Canvas>
```

**使用場面:** 図形描画、ゲーム

#### 6. UniformGrid（均等グリッド）

すべてのセルが同じサイズ。

```xml
<UniformGrid Rows="2" Columns="3">
    <Button Content="1"/>
    <Button Content="2"/>
    <Button Content="3"/>
    <Button Content="4"/>
    <Button Content="5"/>
    <Button Content="6"/>
</UniformGrid>
```

**使用場面:** 電卓、カレンダー

---

## コントロール

### 基本コントロール

#### テキスト表示

```xml
<!-- 読み取り専用テキスト -->
<TextBlock Text="テキスト" FontSize="16" FontWeight="Bold"/>

<!-- テキスト入力（単行） -->
<TextBox Text="{Binding Name}" PlaceholderText="名前を入力"/>

<!-- テキスト入力（複数行） -->
<TextBox AcceptsReturn="True" TextWrapping="Wrap" Height="100"/>

<!-- パスワード入力 -->
<PasswordBox/>

<!-- ラベル -->
<Label Content="ラベル" Target="{Binding ElementName=TextBox1}"/>
```

#### ボタン

```xml
<!-- 通常のボタン -->
<Button Content="クリック" Command="{Binding ClickCommand}"/>

<!-- トグルボタン -->
<ToggleButton Content="トグル" IsChecked="{Binding IsEnabled}"/>

<!-- ラジオボタン -->
<RadioButton Content="オプション1" GroupName="Group1"/>
<RadioButton Content="オプション2" GroupName="Group1"/>

<!-- チェックボックス -->
<CheckBox Content="同意する" IsChecked="{Binding IsAgreed}"/>
```

#### 選択

```xml
<!-- コンボボックス -->
<ComboBox ItemsSource="{Binding Items}"
          SelectedItem="{Binding SelectedItem}"
          DisplayMemberPath="Name"/>

<!-- リストボックス -->
<ListBox ItemsSource="{Binding Orders}"
         SelectionMode="Multiple"/>
```

#### データ表示

```xml
<!-- データグリッド -->
<DataGrid ItemsSource="{Binding Orders}"
          AutoGenerateColumns="False"
          CanUserAddRows="False">
    <DataGrid.Columns>
        <DataGridTextColumn Header="注文番号" Binding="{Binding OrderNo}"/>
        <DataGridTextColumn Header="数量" Binding="{Binding Quantity}"/>
        <DataGridCheckBoxColumn Header="完了" Binding="{Binding IsCompleted}"/>
    </DataGrid.Columns>
</DataGrid>

<!-- ツリービュー -->
<TreeView ItemsSource="{Binding RootNodes}">
    <TreeView.ItemTemplate>
        <HierarchicalDataTemplate ItemsSource="{Binding Children}">
            <TextBlock Text="{Binding Name}"/>
        </HierarchicalDataTemplate>
    </TreeView.ItemTemplate>
</TreeView>

<!-- リストビュー -->
<ListView ItemsSource="{Binding Products}">
    <ListView.View>
        <GridView>
            <GridViewColumn Header="名前" DisplayMemberBinding="{Binding Name}"/>
            <GridViewColumn Header="価格" DisplayMemberBinding="{Binding Price}"/>
        </GridView>
    </ListView.View>
</ListView>
```

#### その他

```xml
<!-- プログレスバー -->
<ProgressBar Value="{Binding Progress}" Maximum="100"/>

<!-- スライダー -->
<Slider Minimum="0" Maximum="100" Value="{Binding Volume}"/>

<!-- 日付選択 -->
<DatePicker SelectedDate="{Binding OrderDate}"/>

<!-- カレンダー -->
<Calendar SelectedDate="{Binding SelectedDate}"/>

<!-- 画像 -->
<Image Source="/Images/logo.png" Stretch="Uniform"/>

<!-- 境界線 -->
<Border BorderBrush="Gray" BorderThickness="1" CornerRadius="4" Padding="10">
    <TextBlock Text="内容"/>
</Border>

<!-- スクロールビューア -->
<ScrollViewer VerticalScrollBarVisibility="Auto">
    <StackPanel>
        <!-- 長いコンテンツ -->
    </StackPanel>
</ScrollViewer>
```

---

## リソース

### リソースの定義

#### アプリケーションリソース（App.xaml）

```xml
<Application.Resources>
    <!-- 色 -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="#3F51B5"/>
    <SolidColorBrush x:Key="AccentBrush" Color="#FF4081"/>

    <!-- フォントサイズ -->
    <System:Double x:Key="TitleFontSize">24</System:Double>
    <System:Double x:Key="BodyFontSize">14</System:Double>

    <!-- スタイル -->
    <Style x:Key="PrimaryButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="{StaticResource PrimaryBrush}"/>
        <Setter Property="Foreground" Value="White"/>
        <Setter Property="Padding" Value="15,8"/>
    </Style>
</Application.Resources>
```

#### ウィンドウリソース

```xml
<Window.Resources>
    <local:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter"/>

    <Style x:Key="HeaderTextStyle" TargetType="TextBlock">
        <Setter Property="FontSize" Value="20"/>
        <Setter Property="FontWeight" Value="Bold"/>
    </Style>
</Window.Resources>
```

### リソースの使用

```xml
<!-- StaticResource（一度だけ解決） -->
<Button Background="{StaticResource PrimaryBrush}"/>
<TextBlock Style="{StaticResource HeaderTextStyle}"/>

<!-- DynamicResource（動的に解決） -->
<TextBlock Foreground="{DynamicResource ThemeForeground}"/>
```

---

## スタイルとテンプレート

### スタイル

#### 基本的なスタイル

```xml
<Style x:Key="MyButtonStyle" TargetType="Button">
    <Setter Property="Background" Value="Blue"/>
    <Setter Property="Foreground" Value="White"/>
    <Setter Property="FontSize" Value="14"/>
    <Setter Property="Padding" Value="10,5"/>
    <Setter Property="Margin" Value="5"/>
</Style>

<!-- 使用 -->
<Button Content="ボタン" Style="{StaticResource MyButtonStyle}"/>
```

#### 暗黙的なスタイル

```xml
<!-- x:Keyなし = すべてのButtonに適用 -->
<Style TargetType="Button">
    <Setter Property="Margin" Value="5"/>
</Style>
```

#### スタイルの継承

```xml
<Style x:Key="BaseButtonStyle" TargetType="Button">
    <Setter Property="Padding" Value="10,5"/>
</Style>

<Style x:Key="PrimaryButtonStyle" TargetType="Button" BasedOn="{StaticResource BaseButtonStyle}">
    <Setter Property="Background" Value="Blue"/>
</Style>
```

#### トリガー

```xml
<Style TargetType="Button">
    <Setter Property="Background" Value="LightGray"/>

    <Style.Triggers>
        <!-- マウスオーバー時 -->
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Background" Value="Gray"/>
        </Trigger>

        <!-- 無効時 -->
        <Trigger Property="IsEnabled" Value="False">
            <Setter Property="Opacity" Value="0.5"/>
        </Trigger>

        <!-- データトリガー -->
        <DataTrigger Binding="{Binding IsImportant}" Value="True">
            <Setter Property="Foreground" Value="Red"/>
        </DataTrigger>
    </Style.Triggers>
</Style>
```

### コントロールテンプレート

完全にカスタマイズしたコントロールを作成。

```xml
<Style TargetType="Button">
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Border x:Name="border"
                        Background="{TemplateBinding Background}"
                        BorderBrush="{TemplateBinding BorderBrush}"
                        BorderThickness="1"
                        CornerRadius="4">
                    <ContentPresenter HorizontalAlignment="Center"
                                     VerticalAlignment="Center"/>
                </Border>

                <ControlTemplate.Triggers>
                    <Trigger Property="IsMouseOver" Value="True">
                        <Setter TargetName="border" Property="Background" Value="LightBlue"/>
                    </Trigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

### データテンプレート

データの表示方法を定義。

```xml
<ListBox ItemsSource="{Binding Orders}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <Border BorderBrush="Gray" BorderThickness="1" Padding="10" Margin="5">
                <StackPanel>
                    <TextBlock Text="{Binding OrderNo}" FontWeight="Bold"/>
                    <TextBlock Text="{Binding SecurityId}"/>
                    <TextBlock Text="{Binding Quantity, StringFormat='数量: {0}'}"/>
                </StackPanel>
            </Border>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

---

## アニメーション

### ストーリーボード

```xml
<Window.Resources>
    <Storyboard x:Key="FadeInAnimation">
        <DoubleAnimation Storyboard.TargetProperty="Opacity"
                        From="0" To="1" Duration="0:0:0.5"/>
    </Storyboard>

    <Storyboard x:Key="SlideInAnimation">
        <DoubleAnimation Storyboard.TargetProperty="(Canvas.Left)"
                        From="-200" To="0" Duration="0:0:0.3">
            <DoubleAnimation.EasingFunction>
                <QuadraticEase EasingMode="EaseOut"/>
            </DoubleAnimation.EasingFunction>
        </DoubleAnimation>
    </Storyboard>
</Window.Resources>

<Button Content="アニメーション">
    <Button.Triggers>
        <EventTrigger RoutedEvent="Button.Click">
            <BeginStoryboard Storyboard="{StaticResource FadeInAnimation}"/>
        </EventTrigger>
    </Button.Triggers>
</Button>
```

### コードからアニメーション

```csharp
private void AnimateButton()
{
    var animation = new DoubleAnimation
    {
        From = 0,
        To = 1,
        Duration = TimeSpan.FromSeconds(0.5)
    };

    myButton.BeginAnimation(OpacityProperty, animation);
}
```

---

## ベストプラクティス

### 1. レイアウトは動的に

```xml
<!-- ✅ 良い例（ウィンドウサイズに応じて調整） -->
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
</Grid>

<!-- ❌ 悪い例（固定サイズ） -->
<Canvas>
    <Button Canvas.Left="100" Canvas.Top="200"/>
</Canvas>
```

### 2. リソースを活用

```xml
<!-- ✅ 良い例 -->
<Button Background="{StaticResource PrimaryBrush}"/>
<Button Background="{StaticResource PrimaryBrush}"/>

<!-- ❌ 悪い例（重複） -->
<Button Background="#3F51B5"/>
<Button Background="#3F51B5"/>
```

### 3. スタイルで統一

```xml
<!-- ✅ 良い例 -->
<Style TargetType="TextBox">
    <Setter Property="Margin" Value="5"/>
    <Setter Property="Padding" Value="5"/>
</Style>

<!-- ❌ 悪い例（個別に設定） -->
<TextBox Margin="5" Padding="5"/>
<TextBox Margin="5" Padding="5"/>
<TextBox Margin="5" Padding="5"/>
```

### 4. 名前付け規則

```xml
<!-- コントロール名: キャメルケース -->
<Button x:Name="submitButton"/>
<TextBox x:Name="userNameTextBox"/>

<!-- リソースキー: パスカルケース -->
<SolidColorBrush x:Key="PrimaryBrush"/>
<Style x:Key="HeaderTextStyle"/>
```

### 5. コードとXAMLの分離

```xml
<!-- ✅ 良い例（データバインディング） -->
<Button Content="送信" Command="{Binding SubmitCommand}"/>

<!-- ❌ 悪い例（イベントハンドラ） -->
<Button Content="送信" Click="Button_Click"/>
```

---

## まとめ

### WPFの主要機能

| 機能 | 説明 | 用途 |
|------|------|------|
| **XAML** | 宣言的UI定義 | UI構築 |
| **データバインディング** | データとUIの同期 | MVVM |
| **スタイル** | 見た目の定義 | デザイン統一 |
| **テンプレート** | カスタマイズ | 独自デザイン |
| **リソース** | 再利用可能な定義 | 保守性向上 |
| **アニメーション** | 滑らかな動き | UX向上 |

### WPF学習の流れ

```
1. 基本コントロール
   ↓
2. レイアウト
   ↓
3. データバインディング
   ↓
4. MVVM
   ↓
5. スタイル・テンプレート
   ↓
6. アニメーション
```

---

**作成日:** 2025-11-16
**プロジェクト:** OMS (Order Management System)
