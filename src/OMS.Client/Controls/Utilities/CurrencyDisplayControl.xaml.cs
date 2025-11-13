using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OMS.Client.Controls.Utilities;

/// <summary>
/// 通貨表示用コントロール
/// 金額を指定フォーマットで表示し、プラス/マイナスで色分け可能
///
/// 使用例:
/// <local:CurrencyDisplayControl Value="{Binding PnL}"
///                              Currency="JPY"
///                              ShowCurrencySymbol="True"
///                              ColorizeBySign="True"
///                              DecimalPlaces="2"/>
/// </summary>
public partial class CurrencyDisplayControl : UserControl
{
    public CurrencyDisplayControl()
    {
        InitializeComponent();
    }

    #region Dependency Properties

    /// <summary>
    /// 金額値
    /// </summary>
    public decimal Value
    {
        get => (decimal)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(decimal),
            typeof(CurrencyDisplayControl),
            new PropertyMetadata(0m, OnValueChanged));

    /// <summary>
    /// 通貨コード（ISO 4217）
    /// </summary>
    public string Currency
    {
        get => (string)GetValue(CurrencyProperty);
        set => SetValue(CurrencyProperty, value);
    }

    public static readonly DependencyProperty CurrencyProperty =
        DependencyProperty.Register(
            nameof(Currency),
            typeof(string),
            typeof(CurrencyDisplayControl),
            new PropertyMetadata("JPY", OnCurrencyChanged));

    /// <summary>
    /// 通貨記号を表示するか
    /// </summary>
    public bool ShowCurrencySymbol
    {
        get => (bool)GetValue(ShowCurrencySymbolProperty);
        set => SetValue(ShowCurrencySymbolProperty, value);
    }

    public static readonly DependencyProperty ShowCurrencySymbolProperty =
        DependencyProperty.Register(
            nameof(ShowCurrencySymbol),
            typeof(bool),
            typeof(CurrencyDisplayControl),
            new PropertyMetadata(true));

    /// <summary>
    /// 符号によって色を変えるか
    /// </summary>
    public bool ColorizeBySign
    {
        get => (bool)GetValue(ColorizeBySignProperty);
        set => SetValue(ColorizeBySignProperty, value);
    }

    public static readonly DependencyProperty ColorizeBySignProperty =
        DependencyProperty.Register(
            nameof(ColorizeBySign),
            typeof(bool),
            typeof(CurrencyDisplayControl),
            new PropertyMetadata(false, OnColorizeBySignChanged));

    /// <summary>
    /// 小数点以下桁数
    /// </summary>
    public int DecimalPlaces
    {
        get => (int)GetValue(DecimalPlacesProperty);
        set => SetValue(DecimalPlacesProperty, value);
    }

    public static readonly DependencyProperty DecimalPlacesProperty =
        DependencyProperty.Register(
            nameof(DecimalPlaces),
            typeof(int),
            typeof(CurrencyDisplayControl),
            new PropertyMetadata(0, OnDecimalPlacesChanged));

    /// <summary>
    /// プラスの色
    /// </summary>
    public Brush PositiveColor
    {
        get => (Brush)GetValue(PositiveColorProperty);
        set => SetValue(PositiveColorProperty, value);
    }

    public static readonly DependencyProperty PositiveColorProperty =
        DependencyProperty.Register(
            nameof(PositiveColor),
            typeof(Brush),
            typeof(CurrencyDisplayControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(76, 175, 80)), OnColorChanged)); // Green

    /// <summary>
    /// マイナスの色
    /// </summary>
    public Brush NegativeColor
    {
        get => (Brush)GetValue(NegativeColorProperty);
        set => SetValue(NegativeColorProperty, value);
    }

    public static readonly DependencyProperty NegativeColorProperty =
        DependencyProperty.Register(
            nameof(NegativeColor),
            typeof(Brush),
            typeof(CurrencyDisplayControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(244, 67, 54)), OnColorChanged)); // Red

    /// <summary>
    /// ゼロの色
    /// </summary>
    public Brush NeutralColor
    {
        get => (Brush)GetValue(NeutralColorProperty);
        set => SetValue(NeutralColorProperty, value);
    }

    public static readonly DependencyProperty NeutralColorProperty =
        DependencyProperty.Register(
            nameof(NeutralColor),
            typeof(Brush),
            typeof(CurrencyDisplayControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(158, 158, 158)), OnColorChanged)); // Gray

    /// <summary>
    /// 通貨記号（内部使用）
    /// </summary>
    public string CurrencySymbol
    {
        get => (string)GetValue(CurrencySymbolProperty);
        private set => SetValue(CurrencySymbolProperty, value);
    }

    public static readonly DependencyProperty CurrencySymbolProperty =
        DependencyProperty.Register(
            nameof(CurrencySymbol),
            typeof(string),
            typeof(CurrencyDisplayControl),
            new PropertyMetadata("¥"));

    /// <summary>
    /// フォーマット済み値（内部使用）
    /// </summary>
    public string FormattedValue
    {
        get => (string)GetValue(FormattedValueProperty);
        private set => SetValue(FormattedValueProperty, value);
    }

    public static readonly DependencyProperty FormattedValueProperty =
        DependencyProperty.Register(
            nameof(FormattedValue),
            typeof(string),
            typeof(CurrencyDisplayControl),
            new PropertyMetadata("0"));

    /// <summary>
    /// 計算された前景色（内部使用）
    /// </summary>
    public Brush ComputedForeground
    {
        get => (Brush)GetValue(ComputedForegroundProperty);
        private set => SetValue(ComputedForegroundProperty, value);
    }

    public static readonly DependencyProperty ComputedForegroundProperty =
        DependencyProperty.Register(
            nameof(ComputedForeground),
            typeof(Brush),
            typeof(CurrencyDisplayControl),
            new PropertyMetadata(Brushes.Black));

    #endregion

    #region Property Changed Callbacks

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CurrencyDisplayControl control)
        {
            control.UpdateDisplay();
        }
    }

    private static void OnCurrencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CurrencyDisplayControl control)
        {
            control.UpdateCurrencySymbol();
            control.UpdateDisplay();
        }
    }

    private static void OnColorizeBySignChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CurrencyDisplayControl control)
        {
            control.UpdateForeground();
        }
    }

    private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CurrencyDisplayControl control)
        {
            control.UpdateDisplay();
        }
    }

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CurrencyDisplayControl control)
        {
            control.UpdateForeground();
        }
    }

    #endregion

    #region Methods

    private void UpdateDisplay()
    {
        // 金額をフォーマット
        FormattedValue = FormatValue(Value);

        // 色を更新
        UpdateForeground();
    }

    private string FormatValue(decimal value)
    {
        var formatString = DecimalPlaces > 0
            ? $"N{DecimalPlaces}"
            : "N0";

        return value.ToString(formatString, CultureInfo.CurrentCulture);
    }

    private void UpdateCurrencySymbol()
    {
        CurrencySymbol = Currency switch
        {
            "JPY" => "¥",
            "USD" => "$",
            "EUR" => "€",
            "GBP" => "£",
            "CNY" => "¥",
            "KRW" => "₩",
            _ => Currency
        };
    }

    private void UpdateForeground()
    {
        if (!ColorizeBySign)
        {
            ComputedForeground = Foreground ?? Brushes.Black;
            return;
        }

        ComputedForeground = Value switch
        {
            > 0 => PositiveColor,
            < 0 => NegativeColor,
            _ => NeutralColor
        };
    }

    #endregion
}
