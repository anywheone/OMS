using System.Windows;
using System.Windows.Controls;

namespace OMS.Client.Controls
{
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

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(nameof(Increment), typeof(decimal), typeof(NumericUpDown),
                new PropertyMetadata(1m));

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

        public decimal Increment
        {
            get => (decimal)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        public NumericUpDown()
        {
            InitializeComponent();
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value + Increment <= Maximum)
            {
                Value += Increment;
            }
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value - Increment >= Minimum)
            {
                Value -= Increment;
            }
        }
    }
}
