using System.Windows;
using System.Windows.Controls;

namespace OMS.Client.Controls
{
    public partial class ValidatedTextBox : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ValidatedTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        public static readonly DependencyProperty ValidationRuleProperty =
            DependencyProperty.Register(nameof(ValidationRule), typeof(Func<string, (bool IsValid, string ErrorMessage)>),
                typeof(ValidatedTextBox));

        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.Register(nameof(HasError), typeof(bool), typeof(ValidatedTextBox),
                new PropertyMetadata(false));

        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register(nameof(ErrorMessage), typeof(string), typeof(ValidatedTextBox),
                new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Func<string, (bool IsValid, string ErrorMessage)>? ValidationRule
        {
            get => (Func<string, (bool IsValid, string ErrorMessage)>?)GetValue(ValidationRuleProperty);
            set => SetValue(ValidationRuleProperty, value);
        }

        public bool HasError
        {
            get => (bool)GetValue(HasErrorProperty);
            private set => SetValue(HasErrorProperty, value);
        }

        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            private set => SetValue(ErrorMessageProperty, value);
        }

        public ValidatedTextBox()
        {
            InitializeComponent();
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ValidatedTextBox)d;
            var text = (string)e.NewValue;

            if (control.ValidationRule != null)
            {
                var (isValid, errorMessage) = control.ValidationRule(text);
                control.HasError = !isValid;
                control.ErrorMessage = errorMessage;
            }
            else
            {
                control.HasError = false;
                control.ErrorMessage = string.Empty;
            }
        }
    }
}
