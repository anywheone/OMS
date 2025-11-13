using System.Windows;

namespace OMS.Client.Views;

/// <summary>
/// MainWindow.xaml の相互作用ロジック
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // ウィンドウ初期化時に中央配置（表示前に実行される）
        this.SourceInitialized += (s, e) =>
        {
            // ウィンドウサイズと画面サイズから中央座標を計算
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = (screenHeight - this.Height) / 2;
        };
    }
}
