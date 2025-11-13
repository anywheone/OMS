using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OMS.Client.Controls.Utilities;

/// <summary>
/// 数値入力用UpDownコントロール
///
/// 使用例:
/// <local:NumericUpDownControl Value="{Binding Quantity}"
///                            Minimum="0"
///                            Maximum="1000000"
///                            Increment="100"
///                            DecimalPlaces="0"
///                            ShowUpDownButtons="True"/>
/// </summary>
public partial class NumericUpDownControl : UserControl
{
    public NumericUpDownControl()
    {
        InitializeComponent();
    }

    #region Dependency Properties

    /// <summary>
    /// 現在の値
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
            typeof(NumericUpDownControl),
            new FrameworkPropertyMetadata(
                0m,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnValueChanged,
                CoerceValue));

    /// <summary>
    /// 最小値
    /// </summary>
    public decimal Minimum
    {
        get => (decimal)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            nameof(Minimum),
            typeof(decimal),
            typeof(NumericUpDownControl),
            new PropertyMetadata(decimal.MinValue, OnMinMaxChanged));

    /// <summary>
    /// 最大値
    /// </summary>
    public decimal Maximum
    {
        get => (decimal)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            nameof(Maximum),
            typeof(decimal),
            typeof(NumericUpDownControl),
            new PropertyMetadata(decimal.MaxValue, OnMinMaxChanged));

    /// <summary>
    /// 増減値
    /// </summary>
    public decimal Increment
    {
        get => (decimal)GetValue(IncrementProperty);
        set => SetValue(IncrementProperty, value);
    }

    public static readonly DependencyProperty IncrementProperty =
        DependencyProperty.Register(
            nameof(Increment),
            typeof(decimal),
            typeof(NumericUpDownControl),
            new PropertyMetadata(1m));

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
            typeof(NumericUpDownControl),
            new PropertyMetadata(0, OnDecimalPlacesChanged));

    /// <summary>
    /// フォーマット文字列
    /// </summary>
    public string FormatString
    {
        get => (string)GetValue(FormatStringProperty);
        set => SetValue(FormatStringProperty, value);
    }

    public static readonly DependencyProperty FormatStringProperty =
        DependencyProperty.Register(
            nameof(FormatString),
            typeof(string),
            typeof(NumericUpDownControl),
            new PropertyMetadata("N0", OnFormatStringChanged));

    /// <summary>
    /// Up/Downボタンを表示するか
    /// </summary>
    public bool ShowUpDownButtons
    {
        get => (bool)GetValue(ShowUpDownButtonsProperty);
        set => SetValue(ShowUpDownButtonsProperty, value);
    }

    public static readonly DependencyProperty ShowUpDownButtonsProperty =
        DependencyProperty.Register(
            nameof(ShowUpDownButtons),
            typeof(bool),
            typeof(NumericUpDownControl),
            new PropertyMetadata(true));

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
            typeof(NumericUpDownControl),
            new PropertyMetadata("0"));

    #endregion

    #region Events

    /// <summary>
    /// 値が変更された時のイベント
    /// </summary>
    public event RoutedPropertyChangedEventHandler<decimal>? ValueChanged;

    #endregion

    #region Property Changed Callbacks

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumericUpDownControl control)
        {
            var oldValue = (decimal)e.OldValue;
            var newValue = (decimal)e.NewValue;

            control.UpdateFormattedValue();
            control.ValueChanged?.Invoke(control, new RoutedPropertyChangedEventArgs<decimal>(oldValue, newValue));
        }
    }

    private static object CoerceValue(DependencyObject d, object baseValue)
    {
        if (d is NumericUpDownControl control && baseValue is decimal value)
        {
            if (value < control.Minimum) return control.Minimum;
            if (value > control.Maximum) return control.Maximum;
            return value;
        }
        return baseValue;
    }

    private static void OnMinMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumericUpDownControl control)
        {
            control.CoerceValue(ValueProperty);
        }
    }

    private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumericUpDownControl control)
        {
            control.FormatString = $"N{control.DecimalPlaces}";
        }
    }

    private static void OnFormatStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumericUpDownControl control)
        {
            control.UpdateFormattedValue();
        }
    }

    #endregion

    #region Methods

    private void UpdateFormattedValue()
    {
        FormattedValue = Value.ToString(FormatString, CultureInfo.CurrentCulture);
    }

    private void IncreaseValue()
    {
        var newValue = Value + Increment;
        if (newValue <= Maximum)
        {
            Value = newValue;
        }
    }

    private void DecreaseValue()
    {
        var newValue = Value - Increment;
        if (newValue >= Minimum)
        {
            Value = newValue;
        }
    }

    private bool ValidateInput(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return true;

        // 数値（小数点含む）と負号、カンマを許可
        var pattern = DecimalPlaces > 0
            ? @"^-?\d{0,15}(,\d{3})*(\.\d{0," + DecimalPlaces + "})?$"
            : @"^-?\d{0,15}(,\d{3})*$";

        return Regex.IsMatch(text, pattern);
    }

    private decimal ParseValue(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return Minimum;

        // カンマを除去して解析
        text = text.Replace(",", "");

        if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out var result))
        {
            return result;
        }

        return Value;
    }

    #endregion

    #region Event Handlers

    private void BtnUp_Click(object sender, RoutedEventArgs e)
    {
        IncreaseValue();
        txtValue.Focus();
    }

    private void BtnDown_Click(object sender, RoutedEventArgs e)
    {
        DecreaseValue();
        txtValue.Focus();
    }

    private void TxtValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // 入力をリアルタイムで検証
        var textBox = (TextBox)sender;
        var newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

        e.Handled = !ValidateInput(newText);
    }

    private void TxtValue_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
                IncreaseValue();
                e.Handled = true;
                break;
            case Key.Down:
                DecreaseValue();
                e.Handled = true;
                break;
            case Key.Enter:
                TxtValue_LostFocus(sender, new RoutedEventArgs());
                e.Handled = true;
                break;
        }
    }

    private void TxtValue_LostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var parsedValue = ParseValue(textBox.Text);

        // 値を設定（自動的にMinMax範囲内に収まる）
        Value = parsedValue;
    }

    private void TxtValue_GotFocus(object sender, RoutedEventArgs e)
    {
        // フォーカス時はフォーマットなしの生の数値を表示
        txtValue.Text = Value.ToString(CultureInfo.CurrentCulture);
        txtValue.SelectAll();
    }

    #endregion
}
