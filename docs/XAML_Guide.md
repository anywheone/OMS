# XAML (Extensible Application Markup Language) 完全ガイド

## 目次
1. [XAMLとは](#xamlとは)
2. [基本構文](#基本構文)
3. [マークアップ拡張](#マークアップ拡張)
4. [名前空間](#名前空間)
5. [添付プロパティ](#添付プロパティ)
6. [コレクション構文](#コレクション構文)
7. [リソース](#リソース)
8. [ベストプラクティス](#ベストプラクティス)

---

## XAMLとは

### 定義
**XAML (ザムル)** は、UIを宣言的に定義するためのXMLベースの言語です。

### HTMLとの比較

| XAML | HTML |
|------|------|
| `<Button Content="クリック"/>` | `<button>クリック</button>` |
| `<TextBox Text="入力"/>` | `<input type="text" value="入力"/>` |
| `<StackPanel>` | `<div style="display:flex; flex-direction:column">` |

### XAMLの特徴

1. **宣言的**: UIを「何を」表示するかで記述
2. **デザイナー対応**: Visual Studioでビジュアル編集可能
3. **データバインディング**: `{Binding}` でデータと連携
4. **リソース**: スタイルや色を再利用
5. **型安全**: コンパイル時にエラー検出

---

## 基本構文

### 要素（Element）

```xml
<!-- 自己終了タグ -->
<Button Content="クリック"/>

<!-- 開始タグと終了タグ -->
<Button>
    <Button.Content>クリック</Button.Content>
</Button>
```

### 属性（Attribute）

```xml
<!-- プロパティを属性で設定 -->
<Button Content="クリック"
        Width="100"
        Height="30"
        Background="Blue"
        Foreground="White"/>
```

### プロパティ要素構文

複雑な値は要素として設定。

```xml
<!-- 属性構文（シンプル） -->
<Button Background="Blue"/>

<!-- プロパティ要素構文（複雑） -->
<Button>
    <Button.Background>
        <LinearGradientBrush>
            <GradientStop Color="Blue" Offset="0"/>
            <GradientStop Color="LightBlue" Offset="1"/>
        </LinearGradientBrush>
    </Button.Background>
</Button>
```

### コンテンツプロパティ

デフォルトのプロパティは省略可能。

```xml
<!-- Content プロパティ（明示的） -->
<Button>
    <Button.Content>クリック</Button.Content>
</Button>

<!-- Content プロパティ（省略） -->
<Button>クリック</Button>

<!-- StackPanel の Children プロパティ -->
<StackPanel>
    <!-- StackPanel.Children は省略 -->
    <Button Content="ボタン1"/>
    <Button Content="ボタン2"/>
</StackPanel>
```

---

## マークアップ拡張

### Binding

データバインディング。

```xml
<!-- 基本 -->
<TextBox Text="{Binding PropertyName}"/>

<!-- モード指定 -->
<TextBox Text="{Binding PropertyName, Mode=TwoWay}"/>

<!-- 更新タイミング -->
<TextBox Text="{Binding PropertyName, UpdateSourceTrigger=PropertyChanged}"/>

<!-- フォーマット -->
<TextBlock Text="{Binding Price, StringFormat='¥{0:N0}'}"/>

<!-- ElementName -->
<TextBlock Text="{Binding ElementName=MyTextBox, Path=Text}"/>

<!-- RelativeSource -->
<TextBlock Text="{Binding RelativeSource={RelativeSource AncestorType=Window}, Path=Title}"/>
```

### StaticResource

静的リソース参照（一度だけ解決）。

```xml
<Window.Resources>
    <SolidColorBrush x:Key="PrimaryBrush" Color="Blue"/>
</Window.Resources>

<Button Background="{StaticResource PrimaryBrush}"/>
```

### DynamicResource

動的リソース参照（実行時に解決）。

```xml
<Button Background="{DynamicResource ThemeBrush}"/>
```

### TemplateBinding

コントロールテンプレート内で使用。

```xml
<ControlTemplate TargetType="Button">
    <Border Background="{TemplateBinding Background}"
            BorderBrush="{TemplateBinding BorderBrush}">
        <ContentPresenter/>
    </Border>
</ControlTemplate>
```

### x:Static

静的メンバー参照。

```xml
<TextBlock FontFamily="{x:Static SystemFonts.MessageFontFamily}"/>
<Rectangle Fill="{x:Static SystemColors.ControlBrush}"/>
```

```csharp
// カスタム静的メンバー
public static class Constants
{
    public static string CompanyName => "My Company";
}
```

```xml
<TextBlock Text="{x:Static local:Constants.CompanyName}"/>
```

### x:Type

型参照。

```xml
<Style TargetType="{x:Type Button}">
    <!-- スタイル定義 -->
</Style>
```

### x:Null

null値。

```xml
<Button Background="{x:Null}"/>
```

---

## 名前空間

### デフォルト名前空間

```xml
<!-- WPFコントロール -->
xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"

<!-- XAML言語機能（x:Name, x:Key など） -->
xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
```

### カスタム名前空間

```xml
<!-- ローカルクラス -->
xmlns:local="clr-namespace:OMS.Client"
xmlns:controls="clr-namespace:OMS.Client.Controls"
xmlns:viewmodels="clr-namespace:OMS.Client.ViewModels"

<!-- 他のアセンブリ -->
xmlns:prism="http://prismlibrary.com/"
xmlns:materialDesign="http://materialdesigninxaml.net/winfx/xaml/themes"
```

**使用例:**

```xml
<Window xmlns:local="clr-namespace:OMS.Client"
        xmlns:controls="clr-namespace:OMS.Client.Controls">

    <!-- ローカルコントロール -->
    <controls:OrderTicket/>

    <!-- ローカルコンバーター -->
    <Window.Resources>
        <local:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter"/>
    </Window.Resources>
</Window>
```

---

## 添付プロパティ

### 添付プロパティとは

**親要素が子要素に対して設定するプロパティ**

### Grid の添付プロパティ

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>

    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="200"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>

    <!-- Grid.Row, Grid.Column は添付プロパティ -->
    <TextBlock Grid.Row="0" Grid.Column="0" Text="ヘッダー"/>
    <TextBox Grid.Row="1" Grid.Column="0"/>
    <DataGrid Grid.Row="1" Grid.Column="1"/>
</Grid>
```

### DockPanel の添付プロパティ

```xml
<DockPanel>
    <!-- DockPanel.Dock は添付プロパティ -->
    <Menu DockPanel.Dock="Top"/>
    <StatusBar DockPanel.Dock="Bottom"/>
    <TreeView DockPanel.Dock="Left" Width="200"/>
    <TextBox/>
</DockPanel>
```

### Canvas の添付プロパティ

```xml
<Canvas>
    <!-- Canvas.Left, Canvas.Top は添付プロパティ -->
    <Button Canvas.Left="50" Canvas.Top="100" Content="ボタン"/>
    <Ellipse Canvas.Left="200" Canvas.Top="150" Width="100" Height="100"/>
</Canvas>
```

### カスタム添付プロパティ

```csharp
public class TextBoxHelper
{
    // 添付プロパティの定義
    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.RegisterAttached(
            "Placeholder",
            typeof(string),
            typeof(TextBoxHelper),
            new PropertyMetadata(string.Empty));

    // Getter
    public static string GetPlaceholder(DependencyObject obj)
    {
        return (string)obj.GetValue(PlaceholderProperty);
    }

    // Setter
    public static void SetPlaceholder(DependencyObject obj, string value)
    {
        obj.SetValue(PlaceholderProperty, value);
    }
}
```

```xml
<TextBox local:TextBoxHelper.Placeholder="名前を入力してください"/>
```

---

## コレクション構文

### ItemsControl

```xml
<ListBox>
    <ListBox.Items>
        <ListBoxItem Content="項目1"/>
        <ListBoxItem Content="項目2"/>
        <ListBoxItem Content="項目3"/>
    </ListBox.Items>
</ListBox>

<!-- コンテンツプロパティで省略 -->
<ListBox>
    <ListBoxItem Content="項目1"/>
    <ListBoxItem Content="項目2"/>
    <ListBoxItem Content="項目3"/>
</ListBox>
```

### Grid定義

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
        <RowDefinition Height="100"/>
    </Grid.RowDefinitions>

    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="200"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
</Grid>
```

### リソース辞書

```xml
<Window.Resources>
    <!-- ResourceDictionary.Add の省略形 -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="Blue"/>
    <SolidColorBrush x:Key="AccentBrush" Color="Red"/>

    <Style x:Key="MyButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="{StaticResource PrimaryBrush}"/>
    </Style>
</Window.Resources>
```

---

## リソース

### リソースの定義場所

#### 1. アプリケーションリソース（App.xaml）

```xml
<Application.Resources>
    <!-- アプリ全体で使用可能 -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="#3F51B5"/>

    <Style x:Key="PrimaryButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="{StaticResource PrimaryBrush}"/>
        <Setter Property="Foreground" Value="White"/>
    </Style>
</Application.Resources>
```

#### 2. ウィンドウリソース

```xml
<Window.Resources>
    <!-- このウィンドウ内で使用可能 -->
    <local:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter"/>

    <Style x:Key="HeaderTextStyle" TargetType="TextBlock">
        <Setter Property="FontSize" Value="20"/>
        <Setter Property="FontWeight" Value="Bold"/>
    </Style>
</Window.Resources>
```

#### 3. コントロールリソース

```xml
<StackPanel>
    <StackPanel.Resources>
        <!-- このStackPanel内で使用可能 -->
        <Style TargetType="Button">
            <Setter Property="Margin" Value="5"/>
        </Style>
    </StackPanel.Resources>

    <Button Content="ボタン1"/>
    <Button Content="ボタン2"/>
</StackPanel>
```

### リソース辞書の分離

```xml
<!-- Themes/Colors.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <SolidColorBrush x:Key="PrimaryBrush" Color="#3F51B5"/>
    <SolidColorBrush x:Key="AccentBrush" Color="#FF4081"/>
</ResourceDictionary>
```

```xml
<!-- Themes/Styles.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Style x:Key="PrimaryButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="{StaticResource PrimaryBrush}"/>
    </Style>
</ResourceDictionary>
```

```xml
<!-- App.xaml -->
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Themes/Colors.xaml"/>
            <ResourceDictionary Source="Themes/Styles.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

---

## ベストプラクティス

### 1. 名前付け規則

```xml
<!-- x:Name: キャメルケース -->
<Button x:Name="submitButton"/>
<TextBox x:Name="userNameTextBox"/>

<!-- x:Key: パスカルケース -->
<SolidColorBrush x:Key="PrimaryBrush"/>
<Style x:Key="HeaderTextStyle"/>
```

### 2. インデント

```xml
<!-- ✅ 良い例 -->
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>

    <StackPanel Grid.Row="0">
        <TextBlock Text="ヘッダー"/>
    </StackPanel>
</Grid>

<!-- ❌ 悪い例 -->
<Grid><Grid.RowDefinitions><RowDefinition Height="Auto"/><RowDefinition Height="*"/></Grid.RowDefinitions><StackPanel Grid.Row="0"><TextBlock Text="ヘッダー"/></StackPanel></Grid>
```

### 3. プロパティの順序

```xml
<!-- 推奨順序: 名前 → レイアウト → 見た目 → データバインディング → イベント -->
<Button x:Name="submitButton"
        Grid.Row="0" Grid.Column="0"
        Width="100" Height="30"
        Background="Blue" Foreground="White"
        Content="{Binding ButtonText}"
        Command="{Binding SubmitCommand}"/>
```

### 4. リソースの再利用

```xml
<!-- ✅ 良い例 -->
<Window.Resources>
    <SolidColorBrush x:Key="PrimaryBrush" Color="Blue"/>
</Window.Resources>

<Button Background="{StaticResource PrimaryBrush}"/>
<Border BorderBrush="{StaticResource PrimaryBrush}"/>

<!-- ❌ 悪い例（重複） -->
<Button Background="Blue"/>
<Border BorderBrush="Blue"/>
```

### 5. データバインディングの活用

```xml
<!-- ✅ 良い例 -->
<TextBox Text="{Binding UserName, UpdateSourceTrigger=PropertyChanged}"/>

<!-- ❌ 悪い例（イベントハンドラ） -->
<TextBox TextChanged="TextBox_TextChanged"/>
```

### 6. コメント

```xml
<!-- メインコンテンツエリア -->
<Grid Grid.Row="1">
    <!-- 左サイドバー -->
    <Border Grid.Column="0">
        <!-- ... -->
    </Border>

    <!-- 中央コンテンツ -->
    <StackPanel Grid.Column="1">
        <!-- ... -->
    </StackPanel>
</Grid>
```

### 7. マジックナンバーを避ける

```xml
<!-- ✅ 良い例 -->
<Window.Resources>
    <System:Double x:Key="StandardMargin">10</System:Double>
    <System:Double x:Key="StandardPadding">15</System:Double>
</Window.Resources>

<Button Margin="{StaticResource StandardMargin}"
        Padding="{StaticResource StandardPadding}"/>

<!-- ❌ 悪い例 -->
<Button Margin="10" Padding="15"/>
<TextBox Margin="10" Padding="15"/>
<Border Margin="10" Padding="15"/>
```

---

## まとめ

### XAMLの主要構文

| 構文 | 説明 | 例 |
|------|------|-----|
| **要素** | コントロール定義 | `<Button/>` |
| **属性** | プロパティ設定 | `Content="クリック"` |
| **プロパティ要素** | 複雑な値 | `<Button.Background>` |
| **マークアップ拡張** | 動的な値 | `{Binding}`, `{StaticResource}` |
| **添付プロパティ** | 親が子に設定 | `Grid.Row="0"` |
| **名前空間** | 外部型参照 | `xmlns:local="..."` |

### マークアップ拡張まとめ

| 拡張 | 用途 | 例 |
|------|------|-----|
| `{Binding}` | データバインディング | `{Binding PropertyName}` |
| `{StaticResource}` | 静的リソース | `{StaticResource PrimaryBrush}` |
| `{DynamicResource}` | 動的リソース | `{DynamicResource ThemeBrush}` |
| `{TemplateBinding}` | テンプレート内 | `{TemplateBinding Background}` |
| `{x:Static}` | 静的メンバー | `{x:Static SystemColors.ControlBrush}` |
| `{x:Type}` | 型参照 | `{x:Type Button}` |
| `{x:Null}` | null値 | `{x:Null}` |

### XAML学習の流れ

```
1. 基本要素と属性
   ↓
2. レイアウト（Grid, StackPanel）
   ↓
3. データバインディング
   ↓
4. リソース
   ↓
5. スタイル
   ↓
6. テンプレート
```

---

**作成日:** 2025-11-16
**プロジェクト:** OMS (Order Management System)
