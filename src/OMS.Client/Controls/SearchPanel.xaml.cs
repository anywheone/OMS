using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OMS.Client.Controls
{
    public partial class SearchPanel : UserControl
    {
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(SearchPanel),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register(nameof(SearchCommand), typeof(ICommand), typeof(SearchPanel));

        public static readonly DependencyProperty ShowFiltersProperty =
            DependencyProperty.Register(nameof(ShowFilters), typeof(bool), typeof(SearchPanel),
                new PropertyMetadata(false));

        public static readonly DependencyProperty FilterContentProperty =
            DependencyProperty.Register(nameof(FilterContent), typeof(object), typeof(SearchPanel));

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public ICommand? SearchCommand
        {
            get => (ICommand?)GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        public bool ShowFilters
        {
            get => (bool)GetValue(ShowFiltersProperty);
            set => SetValue(ShowFiltersProperty, value);
        }

        public object? FilterContent
        {
            get => GetValue(FilterContentProperty);
            set => SetValue(FilterContentProperty, value);
        }

        public SearchPanel()
        {
            InitializeComponent();
        }
    }
}
