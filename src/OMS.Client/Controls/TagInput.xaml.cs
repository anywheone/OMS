using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OMS.Client.Controls
{
    public partial class TagInput : UserControl
    {
        public static readonly DependencyProperty TagsProperty =
            DependencyProperty.Register(nameof(Tags), typeof(ObservableCollection<string>), typeof(TagInput),
                new PropertyMetadata(null));

        public ObservableCollection<string> Tags
        {
            get
            {
                var tags = (ObservableCollection<string>?)GetValue(TagsProperty);
                if (tags == null)
                {
                    tags = new ObservableCollection<string>();
                    SetValue(TagsProperty, tags);
                }
                return tags;
            }
            set => SetValue(TagsProperty, value);
        }

        public TagInput()
        {
            InitializeComponent();
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                var tag = InputTextBox.Text.Trim();
                if (!Tags.Contains(tag))
                {
                    Tags.Add(tag);
                }
                InputTextBox.Text = string.Empty;
                e.Handled = true;
            }
        }

        private void RemoveTag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string tag)
            {
                Tags.Remove(tag);
            }
        }
    }
}
