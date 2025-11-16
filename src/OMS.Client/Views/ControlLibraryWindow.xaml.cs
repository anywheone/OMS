using System.Windows;
using System.Windows.Media;
using OMS.Client.Models;
using OMS.Client.ViewModels;

namespace OMS.Client.Views;

/// <summary>
/// ControlLibraryWindow.xaml の相互作用ロジック
/// </summary>
public partial class ControlLibraryWindow : Window
{
    public ControlLibraryWindow(ControlLibraryViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void BtnOmsTab_Click(object sender, RoutedEventArgs e)
    {
        // ViewModelをOMS特有コントロールに切り替え
        if (DataContext is ControlLibraryViewModel viewModel)
        {
            viewModel.SelectedType = ControlType.OMS;
        }

        // ボタンの見た目を更新
        BtnOmsTab.BorderThickness = new Thickness(0, 0, 0, 3);
        BtnOmsTab.BorderBrush = (SolidColorBrush)FindResource("PrimaryBrush");
        BtnOmsTab.Foreground = (SolidColorBrush)FindResource("PrimaryBrush");
        BtnOmsTab.FontWeight = FontWeights.Medium;

        BtnGeneralTab.BorderThickness = new Thickness(0);
        BtnGeneralTab.BorderBrush = Brushes.Transparent;
        BtnGeneralTab.Foreground = Brushes.Gray;
        BtnGeneralTab.FontWeight = FontWeights.Medium;
    }

    private void BtnGeneralTab_Click(object sender, RoutedEventArgs e)
    {
        // ViewModelを一般業務アプリコントロールに切り替え
        if (DataContext is ControlLibraryViewModel viewModel)
        {
            viewModel.SelectedType = ControlType.General;
        }

        // ボタンの見た目を更新
        BtnGeneralTab.BorderThickness = new Thickness(0, 0, 0, 3);
        BtnGeneralTab.BorderBrush = (SolidColorBrush)FindResource("PrimaryBrush");
        BtnGeneralTab.Foreground = (SolidColorBrush)FindResource("PrimaryBrush");
        BtnGeneralTab.FontWeight = FontWeights.Medium;

        BtnOmsTab.BorderThickness = new Thickness(0);
        BtnOmsTab.BorderBrush = Brushes.Transparent;
        BtnOmsTab.Foreground = Brushes.Gray;
        BtnOmsTab.FontWeight = FontWeights.Medium;
    }
}
