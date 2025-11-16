using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Prism.Mvvm;
using OMS.Client.Models;

namespace OMS.Client.ViewModels;

/// <summary>
/// コントロールライブラリViewModel
/// </summary>
public class ControlLibraryViewModel : BindableBase
{
    private ControlInfo? _selectedControl;
    private string _searchText = string.Empty;
    private string _selectedCategory = "すべて";
    private ControlType _selectedType = ControlType.OMS;

    public ControlLibraryViewModel()
    {
        LoadControls();
        LoadGeneralControls();
        UpdateFilteredControls();
    }

    /// <summary>コントロール一覧</summary>
    public ObservableCollection<ControlInfo> Controls { get; } = new();

    /// <summary>フィルタ済みコントロール一覧</summary>
    public ObservableCollection<ControlInfo> FilteredControls { get; } = new();

    /// <summary>カテゴリ一覧</summary>
    public ObservableCollection<string> Categories { get; } = new()
    {
        "すべて", "基本コントロール", "組み合わせパターン", "トレーディング", "リスク管理", "分析・レポート"
    };

    /// <summary>選択中のコントロール</summary>
    public ControlInfo? SelectedControl
    {
        get => _selectedControl;
        set => SetProperty(ref _selectedControl, value);
    }

    /// <summary>検索テキスト</summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                UpdateFilteredControls();
            }
        }
    }

    /// <summary>選択中のカテゴリ</summary>
    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                UpdateFilteredControls();
            }
        }
    }

    /// <summary>選択中のコントロールタイプ</summary>
    public ControlType SelectedType
    {
        get => _selectedType;
        set
        {
            if (SetProperty(ref _selectedType, value))
            {
                UpdateFilteredControls();
            }
        }
    }

    /// <summary>コントロール一覧を読み込み</summary>
    private void LoadControls()
    {
        // 基本コントロール
        Controls.Add(new ControlInfo
        {
            Name = "注文チケット (OrderTicket)",
            Description = "クイック発注用の注文入力パネル。銘柄、売買、数量、価格を素早く入力して発注できます。",
            Category = "基本コントロール",
            Priority = 5,
            UsageFrequency = "非常に高い",
            PrimaryUse = "トレーダーが最も頻繁に使用する発注インターフェース",
            Icon = "📝",
            Tags = new List<string> { "注文", "発注", "トレーディング" },
            PreviewControl = CreateOrderTicketPreview()
        });

        Controls.Add(new ControlInfo
        {
            Name = "板情報 (MarketDepth)",
            Description = "買い気配と売り気配を表示する板情報。価格帯別の注文数量を視覚的に確認できます。",
            Category = "基本コントロール",
            Priority = 5,
            UsageFrequency = "非常に高い",
            PrimaryUse = "市場の需給バランスを把握",
            Icon = "📊",
            Tags = new List<string> { "板", "気配", "マーケットデータ" },
            PreviewControl = CreateMarketDepthPreview()
        });

        Controls.Add(new ControlInfo
        {
            Name = "ポジションサマリー (PositionSummary)",
            Description = "現在のポジション状況を一覧表示。銘柄別の保有数量、平均取得価格、損益を確認できます。",
            Category = "基本コントロール",
            Priority = 5,
            UsageFrequency = "高い",
            PrimaryUse = "ポジション管理とリスク監視",
            Icon = "💼",
            Tags = new List<string> { "ポジション", "保有", "損益" },
            PreviewControl = CreatePositionSummaryPreview()
        });

        Controls.Add(new ControlInfo
        {
            Name = "リスク指標 (RiskMeter)",
            Description = "VaR、エクスポージャー、レバレッジ等のリスク指標をリアルタイム表示。",
            Category = "基本コントロール",
            Priority = 4,
            UsageFrequency = "中〜高",
            PrimaryUse = "リスク管理とコンプライアンス監視",
            Icon = "⚠️",
            Tags = new List<string> { "リスク", "VaR", "コンプライアンス" },
            PreviewControl = CreateRiskMeterPreview()
        });

        Controls.Add(new ControlInfo
        {
            Name = "歩み値 (TimeAndSales)",
            Description = "直近の約定履歴を時系列で表示。価格、数量、時刻を確認できます。",
            Category = "基本コントロール",
            Priority = 3,
            UsageFrequency = "中",
            PrimaryUse = "市場の勢いとトレンド把握",
            Icon = "⏱️",
            Tags = new List<string> { "約定", "履歴", "タイムアンドセールス" },
            PreviewControl = CreateTimeAndSalesPreview()
        });

        Controls.Add(new ControlInfo
        {
            Name = "価格チャート (PriceChart)",
            Description = "ローソク足チャートとテクニカル指標。トレンド分析に使用します。",
            Category = "基本コントロール",
            Priority = 4,
            UsageFrequency = "高い",
            PrimaryUse = "テクニカル分析とトレンド把握",
            Icon = "📈",
            Tags = new List<string> { "チャート", "分析", "テクニカル" },
            PreviewControl = CreateChartPreview()
        });

        Controls.Add(new ControlInfo
        {
            Name = "アラート通知 (AlertPanel)",
            Description = "価格アラート、約定通知、システムメッセージを表示。",
            Category = "基本コントロール",
            Priority = 3,
            UsageFrequency = "中",
            PrimaryUse = "重要なイベント通知",
            Icon = "🔔",
            Tags = new List<string> { "アラート", "通知", "イベント" },
            PreviewControl = CreateAlertPanelPreview()
        });

        Controls.Add(new ControlInfo
        {
            Name = "注文管理台帳 (OrderBlotter)",
            Description = "全注文の状況を一覧管理。フィルタリング、ソート、キャンセル機能付き。",
            Category = "基本コントロール",
            Priority = 5,
            UsageFrequency = "非常に高い",
            PrimaryUse = "注文状況の監視と管理",
            Icon = "📋",
            Tags = new List<string> { "注文", "管理", "台帳" },
            PreviewControl = CreateOrderBlotterPreview()
        });

        // 組み合わせパターン
        Controls.Add(new ControlInfo
        {
            Name = "トレーディングワークスペース",
            Description = "注文チケット + 板情報 + チャートを統合したトレーディング環境。",
            Category = "組み合わせパターン",
            Priority = 5,
            UsageFrequency = "非常に高い",
            PrimaryUse = "デイトレーダー向けオールインワン画面",
            Icon = "🖥️",
            Tags = new List<string> { "トレーディング", "統合", "ワークスペース" },
            PreviewControl = CreateTradingWorkspacePreview()
        });

        Controls.Add(new ControlInfo
        {
            Name = "リスクダッシュボード",
            Description = "リスク指標 + ポジション + エクスポージャーを統合したリスク管理画面。",
            Category = "組み合わせパターン",
            Priority = 4,
            UsageFrequency = "高い",
            PrimaryUse = "リスク管理者向けモニタリング",
            Icon = "🎛️",
            Tags = new List<string> { "リスク", "ダッシュボード", "管理" },
            PreviewControl = CreateRiskDashboardPreview()
        });

        Controls.Add(new ControlInfo
        {
            Name = "執行モニター",
            Description = "注文台帳 + 約定台帳 + 歩み値を統合した執行管理画面。",
            Category = "組み合わせパターン",
            Priority = 4,
            UsageFrequency = "高い",
            PrimaryUse = "注文執行状況の詳細監視",
            Icon = "📊",
            Tags = new List<string> { "執行", "モニタリング", "台帳" },
            PreviewControl = CreateExecutionMonitorPreview()
        });

        Controls.Add(new ControlInfo
        {
            Name = "マーケット概要",
            Description = "複数銘柄の価格 + 板 + ミニチャートを一覧表示。",
            Category = "組み合わせパターン",
            Priority = 3,
            UsageFrequency = "中",
            PrimaryUse = "複数銘柄の同時監視",
            Icon = "🌐",
            Tags = new List<string> { "マーケット", "一覧", "マルチ銘柄" },
            PreviewControl = CreateMarketOverviewPreview()
        });
    }

    /// <summary>フィルタ済みコントロールを更新</summary>
    private void UpdateFilteredControls()
    {
        FilteredControls.Clear();

        var filtered = Controls.AsEnumerable();

        // タイプフィルタ
        filtered = filtered.Where(c => c.Type == SelectedType);

        // カテゴリフィルタ
        if (SelectedCategory != "すべて")
        {
            filtered = filtered.Where(c => c.Category == SelectedCategory);
        }

        // 検索フィルタ
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var searchLower = SearchText.ToLower();
            filtered = filtered.Where(c =>
                c.Name.ToLower().Contains(searchLower) ||
                c.Description.ToLower().Contains(searchLower) ||
                c.Tags.Any(t => t.ToLower().Contains(searchLower)));
        }

        // 優先度順にソート
        foreach (var control in filtered.OrderByDescending(c => c.Priority))
        {
            FilteredControls.Add(control);
        }
    }

    // プレビュー作成メソッド群
    private UIElement CreateOrderTicketPreview()
    {
        var grid = new Grid { Margin = new System.Windows.Thickness(10) };
        grid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });

        var title = new TextBlock { Text = "注文チケット", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) };
        Grid.SetRow(title, 0);
        grid.Children.Add(title);

        var stack = new StackPanel { Orientation = Orientation.Vertical };
        Grid.SetRow(stack, 1);

        stack.Children.Add(new TextBlock { Text = "銘柄: 1234 (サンプル株)", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "売買: 買い", Foreground = new SolidColorBrush(Colors.Green), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "数量: 1,000株", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "価格: 2,500円 (指値)", Margin = new System.Windows.Thickness(0, 3, 0, 3) });

        grid.Children.Add(stack);

        var button = new Button { Content = "発注実行", Margin = new System.Windows.Thickness(0, 10, 0, 0), Padding = new System.Windows.Thickness(20, 5, 20, 5) };
        Grid.SetRow(button, 2);
        grid.Children.Add(button);

        return grid;
    }

    private UIElement CreateMarketDepthPreview()
    {
        var grid = new Grid { Margin = new System.Windows.Thickness(10) };
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());

        var title = new TextBlock { Text = "板情報", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) };
        Grid.SetColumnSpan(title, 2);
        grid.Children.Add(title);

        var sellStack = new StackPanel { Margin = new System.Windows.Thickness(0, 30, 5, 0) };
        sellStack.Children.Add(new TextBlock { Text = "売り気配", FontWeight = System.Windows.FontWeights.SemiBold, Foreground = new SolidColorBrush(Colors.Red) });
        sellStack.Children.Add(new TextBlock { Text = "2,510 - 500株", Foreground = new SolidColorBrush(Colors.DarkRed), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        sellStack.Children.Add(new TextBlock { Text = "2,505 - 1,200株", Foreground = new SolidColorBrush(Colors.Red), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        sellStack.Children.Add(new TextBlock { Text = "2,501 - 800株", Foreground = new SolidColorBrush(Colors.OrangeRed), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        Grid.SetColumn(sellStack, 0);
        grid.Children.Add(sellStack);

        var buyStack = new StackPanel { Margin = new System.Windows.Thickness(5, 30, 0, 0) };
        buyStack.Children.Add(new TextBlock { Text = "買い気配", FontWeight = System.Windows.FontWeights.SemiBold, Foreground = new SolidColorBrush(Colors.Green) });
        buyStack.Children.Add(new TextBlock { Text = "2,499 - 900株", Foreground = new SolidColorBrush(Colors.LightGreen), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        buyStack.Children.Add(new TextBlock { Text = "2,495 - 1,500株", Foreground = new SolidColorBrush(Colors.Green), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        buyStack.Children.Add(new TextBlock { Text = "2,490 - 600株", Foreground = new SolidColorBrush(Colors.DarkGreen), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        Grid.SetColumn(buyStack, 1);
        grid.Children.Add(buyStack);

        return grid;
    }

    private UIElement CreatePositionSummaryPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "ポジションサマリー", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "サンプル株 (1234): +1,000株", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "平均取得: 2,450円", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "現在値: 2,500円", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "評価損益: +50,000円 (+2.04%)", Foreground = new SolidColorBrush(Colors.Green), FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        return stack;
    }

    private UIElement CreateRiskMeterPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "リスク指標", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "VaR (1日, 99%): ¥123,456", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "エクスポージャー: ¥2,500,000", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "レバレッジ: 1.2倍", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "リスク状態: 正常", Foreground = new SolidColorBrush(Colors.Green), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        return stack;
    }

    private UIElement CreateTimeAndSalesPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "歩み値", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "14:32:15 - 2,500円 x 100株", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "14:32:10 - 2,499円 x 200株", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "14:32:05 - 2,501円 x 150株", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "14:32:00 - 2,500円 x 300株", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        return stack;
    }

    private UIElement CreateChartPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "価格チャート", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "📈 ローソク足チャート表示エリア", Margin = new System.Windows.Thickness(0, 10, 0, 10), FontSize = 24, HorizontalAlignment = System.Windows.HorizontalAlignment.Center });
        stack.Children.Add(new TextBlock { Text = "移動平均線、ボリンジャーバンド等のインジケーター表示", Margin = new System.Windows.Thickness(0, 3, 0, 3), FontSize = 10 });
        return stack;
    }

    private UIElement CreateAlertPanelPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "アラート通知", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "🔔 価格アラート: サンプル株が2,500円に到達", Foreground = new SolidColorBrush(Colors.Orange), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "✅ 注文約定: ORD20251114-0001", Foreground = new SolidColorBrush(Colors.Green), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "ℹ️ システム: 市場開始まで5分", Foreground = new SolidColorBrush(Colors.Blue), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        return stack;
    }

    private UIElement CreateOrderBlotterPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "注文管理台帳", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "ORD-0001 | サンプル株 | 買 1,000株 @ 2,500 | NEW", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "ORD-0002 | テスト株 | 売 500株 @ 1,200 | PARTIAL", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "ORD-0003 | サンプル株 | 買 2,000株 @ 2,480 | FILLED", Foreground = new SolidColorBrush(Colors.Green), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        return stack;
    }

    private UIElement CreateTradingWorkspacePreview()
    {
        var grid = new Grid { Margin = new System.Windows.Thickness(10) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(2, System.Windows.GridUnitType.Star) });

        var stack = new StackPanel();
        stack.Children.Add(new TextBlock { Text = "トレーディングワークスペース", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new Border { BorderBrush = new SolidColorBrush(Colors.Gray), BorderThickness = new System.Windows.Thickness(1), Padding = new System.Windows.Thickness(5), Margin = new System.Windows.Thickness(0, 5, 0, 5), Child = new TextBlock { Text = "注文チケット" } });
        stack.Children.Add(new Border { BorderBrush = new SolidColorBrush(Colors.Gray), BorderThickness = new System.Windows.Thickness(1), Padding = new System.Windows.Thickness(5), Margin = new System.Windows.Thickness(0, 5, 0, 5), Child = new TextBlock { Text = "板情報" } });
        Grid.SetColumn(stack, 0);
        grid.Children.Add(stack);

        var chartArea = new Border
        {
            BorderBrush = new SolidColorBrush(Colors.Gray),
            BorderThickness = new System.Windows.Thickness(1),
            Padding = new System.Windows.Thickness(10),
            Margin = new System.Windows.Thickness(5, 25, 0, 0),
            Child = new TextBlock { Text = "📈 チャートエリア", FontSize = 20, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center }
        };
        Grid.SetColumn(chartArea, 1);
        grid.Children.Add(chartArea);

        return grid;
    }

    private UIElement CreateRiskDashboardPreview()
    {
        var grid = new Grid { Margin = new System.Windows.Thickness(10) };
        grid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
        grid.RowDefinitions.Add(new RowDefinition());

        var title = new TextBlock { Text = "リスクダッシュボード", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) };
        Grid.SetRow(title, 0);
        grid.Children.Add(title);

        var stack = new StackPanel();
        Grid.SetRow(stack, 1);

        stack.Children.Add(new Border { BorderBrush = new SolidColorBrush(Colors.Orange), BorderThickness = new System.Windows.Thickness(1), Padding = new System.Windows.Thickness(5), Margin = new System.Windows.Thickness(0, 5, 0, 5), Child = new TextBlock { Text = "リスク指標パネル" } });
        stack.Children.Add(new Border { BorderBrush = new SolidColorBrush(Colors.Blue), BorderThickness = new System.Windows.Thickness(1), Padding = new System.Windows.Thickness(5), Margin = new System.Windows.Thickness(0, 5, 0, 5), Child = new TextBlock { Text = "ポジションサマリー" } });
        stack.Children.Add(new Border { BorderBrush = new SolidColorBrush(Colors.Green), BorderThickness = new System.Windows.Thickness(1), Padding = new System.Windows.Thickness(5), Margin = new System.Windows.Thickness(0, 5, 0, 5), Child = new TextBlock { Text = "エクスポージャーグラフ" } });

        grid.Children.Add(stack);
        return grid;
    }

    private UIElement CreateExecutionMonitorPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "執行モニター", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new Border { BorderBrush = new SolidColorBrush(Colors.Blue), BorderThickness = new System.Windows.Thickness(1), Padding = new System.Windows.Thickness(5), Margin = new System.Windows.Thickness(0, 5, 0, 5), Child = new TextBlock { Text = "注文台帳" } });
        stack.Children.Add(new Border { BorderBrush = new SolidColorBrush(Colors.Green), BorderThickness = new System.Windows.Thickness(1), Padding = new System.Windows.Thickness(5), Margin = new System.Windows.Thickness(0, 5, 0, 5), Child = new TextBlock { Text = "約定台帳" } });
        stack.Children.Add(new Border { BorderBrush = new SolidColorBrush(Colors.Gray), BorderThickness = new System.Windows.Thickness(1), Padding = new System.Windows.Thickness(5), Margin = new System.Windows.Thickness(0, 5, 0, 5), Child = new TextBlock { Text = "歩み値" } });
        return stack;
    }

    private UIElement CreateMarketOverviewPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "マーケット概要", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "サンプル株 | 2,500 (+50) | 📊 | 板...", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "テスト株 | 1,200 (-10) | 📉 | 板...", Foreground = new SolidColorBrush(Colors.Red), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "デモ株 | 3,450 (+120) | 📈 | 板...", Foreground = new SolidColorBrush(Colors.Green), Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        return stack;
    }

    /// <summary>一般的な業務アプリコントロールを読み込み</summary>
    private void LoadGeneralControls()
    {
        // 入力フォーム系コントロール（優先度：最重要）
        Controls.Add(new ControlInfo
        {
            Name = "バリデーション付きテキスト入力",
            Description = "入力値の検証機能を備えたテキストボックス。必須チェック、形式チェック、範囲チェックなどを標準装備。",
            Category = "入力フォーム系",
            Type = ControlType.General,
            Priority = 5,
            UsageFrequency = "非常に高い",
            PrimaryUse = "業務アプリの各種入力画面で最も頻繁に使用",
            Icon = "📝",
            Tags = new List<string> { "入力", "バリデーション", "TextBox" },
            PreviewControl = new OMS.Client.Controls.ValidatedTextBox
            {
                Text = "サンプルテキスト",
                ValidationRule = (text) =>
                {
                    if (string.IsNullOrWhiteSpace(text))
                        return (false, "入力してください");
                    if (text.Length < 3)
                        return (false, "3文字以上入力してください");
                    return (true, string.Empty);
                }
            }
        });

        Controls.Add(new ControlInfo
        {
            Name = "数値入力コントロール",
            Description = "数値専用の入力コントロール。小数点、桁区切り、範囲制限に対応。",
            Category = "入力フォーム系",
            Type = ControlType.General,
            Priority = 5,
            UsageFrequency = "非常に高い",
            PrimaryUse = "金額、数量などの数値入力",
            Icon = "🔢",
            Tags = new List<string> { "数値", "入力", "NumericTextBox" },
            PreviewControl = new OMS.Client.Controls.NumericUpDown
            {
                Value = 100,
                Minimum = 0,
                Maximum = 1000,
                Increment = 10
            }
        });

        Controls.Add(new ControlInfo
        {
            Name = "検索可能コンボボックス",
            Description = "大量のデータから検索・選択できるコンボボックス。コード+名称表示、インクリメンタルサーチ対応。",
            Category = "入力フォーム系",
            Type = ControlType.General,
            Priority = 5,
            UsageFrequency = "非常に高い",
            PrimaryUse = "マスタ選択、商品選択、顧客選択など",
            Icon = "🔍",
            Tags = new List<string> { "検索", "選択", "ComboBox", "AutoComplete" },
            PreviewControl = CreateSearchableComboPreview()
        });

        Controls.Add(new ControlInfo
        {
            Name = "日付範囲選択",
            Description = "開始日と終了日を一度に選択できるコントロール。期間検索やレポート生成に便利。",
            Category = "入力フォーム系",
            Type = ControlType.General,
            Priority = 4,
            UsageFrequency = "高い",
            PrimaryUse = "期間指定検索、レポート条件入力",
            Icon = "📅",
            Tags = new List<string> { "日付", "期間", "DatePicker", "Range" },
            PreviewControl = new OMS.Client.Controls.DateRangePicker
            {
                StartDate = DateTime.Now.AddDays(-7),
                EndDate = DateTime.Now
            }
        });

        Controls.Add(new ControlInfo
        {
            Name = "ファイル選択",
            Description = "ファイル参照ダイアログを開いてファイルを選択できるコントロール。フィルター設定可能。",
            Category = "入力フォーム系",
            Type = ControlType.General,
            Priority = 4,
            UsageFrequency = "高い",
            PrimaryUse = "ファイルアップロード、インポート機能",
            Icon = "📁",
            Tags = new List<string> { "ファイル", "参照", "アップロード" },
            PreviewControl = new OMS.Client.Controls.FilePicker
            {
                FilePath = "C:\\example\\file.xlsx",
                Filter = "Excelファイル (*.xlsx)|*.xlsx|すべてのファイル (*.*)|*.*"
            }
        });

        Controls.Add(new ControlInfo
        {
            Name = "タグ入力",
            Description = "Enterキーでタグを追加、×ボタンで削除できる入力コントロール。キーワード入力に最適。",
            Category = "入力フォーム系",
            Type = ControlType.General,
            Priority = 3,
            UsageFrequency = "中",
            PrimaryUse = "キーワード入力、カテゴリ選択",
            Icon = "🏷️",
            Tags = new List<string> { "タグ", "キーワード", "複数入力" },
            PreviewControl = new OMS.Client.Controls.TagInput
            {
                Tags = new System.Collections.ObjectModel.ObservableCollection<string> { "タグ1", "タグ2", "タグ3" }
            }
        });

        // データ一覧表示系（優先度：最重要）
        Controls.Add(new ControlInfo
        {
            Name = "拡張DataGrid",
            Description = "ソート、フィルタ、ページング、行色変更、固定列に対応した高機能グリッド。",
            Category = "データ一覧表示系",
            Type = ControlType.General,
            Priority = 5,
            UsageFrequency = "非常に高い",
            PrimaryUse = "検索結果一覧、マスタ一覧など",
            Icon = "📊",
            Tags = new List<string> { "DataGrid", "一覧", "ソート", "フィルタ" },
            PreviewControl = CreateDataGridPreview()
        });

        // ダイアログ・メッセージ系（優先度：最重要）
        Controls.Add(new ControlInfo
        {
            Name = "共通メッセージダイアログ",
            Description = "統一デザインのメッセージボックス。情報、警告、エラー、確認の4種類に対応。",
            Category = "ダイアログ・メッセージ系",
            Type = ControlType.General,
            Priority = 5,
            UsageFrequency = "非常に高い",
            PrimaryUse = "ユーザーへの通知、確認ダイアログ",
            Icon = "💬",
            Tags = new List<string> { "ダイアログ", "メッセージ", "確認" },
            PreviewControl = CreateMessageDialogPreview()
        });

        Controls.Add(new ControlInfo
        {
            Name = "進捗インジケーター",
            Description = "処理中を示すプログレスダイアログ。パーセンテージ表示とメッセージ表示機能付き。",
            Category = "ダイアログ・メッセージ系",
            Type = ControlType.General,
            Priority = 4,
            UsageFrequency = "高い",
            PrimaryUse = "長時間処理の進捗表示",
            Icon = "⏳",
            Tags = new List<string> { "ローディング", "プログレス", "処理中" },
            PreviewControl = new OMS.Client.Controls.ProgressIndicator
            {
                Message = "データを読み込み中...",
                Progress = 65
            }
        });

        // 検索条件パネル（優先度：高）
        Controls.Add(new ControlInfo
        {
            Name = "検索パネル",
            Description = "検索条件を入力するためのパネル。検索テキストとオプショナルなフィルター機能付き。",
            Category = "検索条件パネル",
            Type = ControlType.General,
            Priority = 4,
            UsageFrequency = "高い",
            PrimaryUse = "一覧画面の検索機能",
            Icon = "🔎",
            Tags = new List<string> { "検索", "フィルタ", "条件" },
            PreviewControl = new OMS.Client.Controls.SearchPanel
            {
                SearchText = "サンプル検索",
                ShowFilters = false
            }
        });

        // ナビゲーション（優先度：高）
        Controls.Add(new ControlInfo
        {
            Name = "サイドバーメニュー",
            Description = "アイコン+テキストのサイドバー型ナビゲーション。折りたたみ可能。",
            Category = "ナビゲーション",
            Type = ControlType.General,
            Priority = 4,
            UsageFrequency = "高い",
            PrimaryUse = "アプリケーションのメインメニュー",
            Icon = "📂",
            Tags = new List<string> { "メニュー", "ナビゲーション", "サイドバー" },
            PreviewControl = CreateSidebarPreview()
        });

        // カード/パネル系（優先度：中）
        Controls.Add(new ControlInfo
        {
            Name = "情報カード",
            Description = "タイトル+内容をカード形式で表示。ダッシュボードやサマリー表示に最適。",
            Category = "カード/パネル系",
            Type = ControlType.General,
            Priority = 3,
            UsageFrequency = "中",
            PrimaryUse = "ダッシュボード、KPI表示",
            Icon = "📋",
            Tags = new List<string> { "カード", "パネル", "ダッシュボード" },
            PreviewControl = CreateCardPreview()
        });

        // ページング（優先度：中）
        Controls.Add(new ControlInfo
        {
            Name = "ページングコントロール",
            Description = "前へ/次へ、ページ番号ボタンを備えたページングUI。DataGridと連携。",
            Category = "ページングコントロール",
            Type = ControlType.General,
            Priority = 3,
            UsageFrequency = "中",
            PrimaryUse = "大量データの一覧表示",
            Icon = "📄",
            Tags = new List<string> { "ページング", "ページネーション" },
            PreviewControl = CreatePagingPreview()
        });

        // ツリービュー（優先度：中）
        Controls.Add(new ControlInfo
        {
            Name = "拡張ツリービュー",
            Description = "チェックボックス付き、アイコン表示対応のツリービュー。階層データ表示に最適。",
            Category = "ツリービュー",
            Type = ControlType.General,
            Priority = 3,
            UsageFrequency = "中",
            PrimaryUse = "フォルダ構造、カテゴリ管理",
            Icon = "🌲",
            Tags = new List<string> { "ツリー", "階層", "TreeView" },
            PreviewControl = CreateTreeViewPreview()
        });

        // タブ型コンテンツ（優先度：高）
        Controls.Add(new ControlInfo
        {
            Name = "拡張TabControl",
            Description = "タブの開閉制御、非同期ロード対応。複数画面の切り替えに便利。",
            Category = "タブ型コンテンツ",
            Type = ControlType.General,
            Priority = 4,
            UsageFrequency = "高い",
            PrimaryUse = "複数画面の管理、子画面切替",
            Icon = "📑",
            Tags = new List<string> { "タブ", "TabControl", "画面切替" },
            PreviewControl = CreateTabControlPreview()
        });
    }

    // 一般的な業務アプリコントロールのプレビュー作成メソッド
    private UIElement CreateTextInputPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "バリデーション付きテキスト入力", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "例: 名前入力（必須）", Margin = new System.Windows.Thickness(0, 5, 0, 3) });
        var textBox = new TextBox { Text = "山田太郎", Margin = new System.Windows.Thickness(0, 0, 0, 5), Padding = new System.Windows.Thickness(5) };
        stack.Children.Add(textBox);
        stack.Children.Add(new TextBlock { Text = "✓ 入力値が正しい形式です", Foreground = new SolidColorBrush(Colors.Green), FontSize = 11, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "エラー例:", Margin = new System.Windows.Thickness(0, 10, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "× 必須項目です", Foreground = new SolidColorBrush(Colors.Red), FontSize = 11 });
        return stack;
    }

    private UIElement CreateNumericInputPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "数値入力コントロール", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "金額:", Margin = new System.Windows.Thickness(0, 5, 0, 3) });
        stack.Children.Add(new TextBox { Text = "1,234,567", Margin = new System.Windows.Thickness(0, 0, 0, 5), Padding = new System.Windows.Thickness(5) });
        stack.Children.Add(new TextBlock { Text = "数量:", Margin = new System.Windows.Thickness(0, 10, 0, 3) });
        stack.Children.Add(new TextBox { Text = "100.50", Margin = new System.Windows.Thickness(0, 0, 0, 5), Padding = new System.Windows.Thickness(5) });
        stack.Children.Add(new TextBlock { Text = "- 桁区切り表示\n- 小数点対応\n- 範囲制限可能", FontSize = 11, Foreground = new SolidColorBrush(Colors.Gray), Margin = new System.Windows.Thickness(0, 10, 0, 0) });
        return stack;
    }

    private UIElement CreateSearchableComboPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "検索可能コンボボックス", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "顧客選択:", Margin = new System.Windows.Thickness(0, 5, 0, 3) });
        var combo = new ComboBox { Margin = new System.Windows.Thickness(0, 0, 0, 5), Padding = new System.Windows.Thickness(5) };
        combo.Items.Add("1001 - 株式会社サンプル");
        combo.Items.Add("1002 - テスト商事株式会社");
        combo.Items.Add("1003 - デモ工業");
        combo.SelectedIndex = 0;
        stack.Children.Add(combo);
        stack.Children.Add(new TextBlock { Text = "🔍 インクリメンタルサーチ対応\n💡 コード or 名称で検索可能", FontSize = 11, Foreground = new SolidColorBrush(Colors.Gray), Margin = new System.Windows.Thickness(0, 10, 0, 0) });
        return stack;
    }

    private UIElement CreateDataGridPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "拡張DataGrid", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "ID | 名前 | ステータス | 金額", Margin = new System.Windows.Thickness(0, 5, 0, 3), FontWeight = System.Windows.FontWeights.SemiBold });
        stack.Children.Add(new TextBlock { Text = "001 | サンプル | 完了 | ¥10,000", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "002 | テスト | 処理中 | ¥25,000", Margin = new System.Windows.Thickness(0, 3, 0, 3), Background = new SolidColorBrush(Color.FromRgb(255, 255, 200)) });
        stack.Children.Add(new TextBlock { Text = "003 | デモ | 完了 | ¥8,500", Margin = new System.Windows.Thickness(0, 3, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "✓ ソート・フィルタ\n✓ 固定列\n✓ 行色変更", FontSize = 11, Foreground = new SolidColorBrush(Colors.Gray) });
        return stack;
    }

    private UIElement CreateMessageDialogPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "共通メッセージダイアログ", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        var border1 = new Border { BorderBrush = new SolidColorBrush(Colors.Blue), BorderThickness = new System.Windows.Thickness(2), Padding = new System.Windows.Thickness(10), Margin = new System.Windows.Thickness(0, 5, 0, 5) };
        border1.Child = new TextBlock { Text = "ℹ️ 情報: 保存が完了しました", Foreground = new SolidColorBrush(Colors.Blue) };
        stack.Children.Add(border1);

        var border2 = new Border { BorderBrush = new SolidColorBrush(Colors.Orange), BorderThickness = new System.Windows.Thickness(2), Padding = new System.Windows.Thickness(10), Margin = new System.Windows.Thickness(0, 5, 0, 5) };
        border2.Child = new TextBlock { Text = "⚠️ 警告: 入力値を確認してください", Foreground = new SolidColorBrush(Colors.Orange) };
        stack.Children.Add(border2);

        var border3 = new Border { BorderBrush = new SolidColorBrush(Colors.Red), BorderThickness = new System.Windows.Thickness(2), Padding = new System.Windows.Thickness(10), Margin = new System.Windows.Thickness(0, 5, 0, 5) };
        border3.Child = new TextBlock { Text = "❌ エラー: 処理に失敗しました", Foreground = new SolidColorBrush(Colors.Red) };
        stack.Children.Add(border3);
        return stack;
    }

    private UIElement CreateLoadingDialogPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10), HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
        stack.Children.Add(new TextBlock { Text = "ローディングダイアログ", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10), HorizontalAlignment = System.Windows.HorizontalAlignment.Center });
        stack.Children.Add(new TextBlock { Text = "⏳", FontSize = 36, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, Margin = new System.Windows.Thickness(0, 10, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "処理中...", HorizontalAlignment = System.Windows.HorizontalAlignment.Center, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        var progress = new System.Windows.Controls.ProgressBar { Width = 200, Height = 20, Value = 65, Margin = new System.Windows.Thickness(0, 5, 0, 10) };
        stack.Children.Add(progress);
        stack.Children.Add(new TextBlock { Text = "65% 完了", FontSize = 11, HorizontalAlignment = System.Windows.HorizontalAlignment.Center });
        return stack;
    }

    private UIElement CreateSearchPanelPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "検索条件パネル", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        var border = new Border { BorderBrush = new SolidColorBrush(Colors.Gray), BorderThickness = new System.Windows.Thickness(1), Padding = new System.Windows.Thickness(10) };
        var innerStack = new StackPanel();
        innerStack.Children.Add(new TextBlock { Text = "キーワード:", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        innerStack.Children.Add(new TextBox { Margin = new System.Windows.Thickness(0, 0, 0, 10), Padding = new System.Windows.Thickness(5) });
        innerStack.Children.Add(new TextBlock { Text = "日付:", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        innerStack.Children.Add(new TextBox { Text = "2025/01/01 ~ 2025/12/31", Margin = new System.Windows.Thickness(0, 0, 0, 10), Padding = new System.Windows.Thickness(5) });
        var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = System.Windows.HorizontalAlignment.Right };
        buttonPanel.Children.Add(new Button { Content = "検索", Margin = new System.Windows.Thickness(0, 0, 5, 0), Padding = new System.Windows.Thickness(15, 5, 15, 5) });
        buttonPanel.Children.Add(new Button { Content = "クリア", Padding = new System.Windows.Thickness(15, 5, 15, 5) });
        innerStack.Children.Add(buttonPanel);
        border.Child = innerStack;
        stack.Children.Add(border);
        return stack;
    }

    private UIElement CreateSidebarPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "サイドバーメニュー", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "🏠 ホーム", Margin = new System.Windows.Thickness(0, 5, 0, 5), Background = new SolidColorBrush(Color.FromRgb(230, 240, 255)), Padding = new System.Windows.Thickness(5) });
        stack.Children.Add(new TextBlock { Text = "📊 ダッシュボード", Margin = new System.Windows.Thickness(0, 5, 0, 5), Padding = new System.Windows.Thickness(5) });
        stack.Children.Add(new TextBlock { Text = "📁 マスタ管理", Margin = new System.Windows.Thickness(0, 5, 0, 5), Padding = new System.Windows.Thickness(5) });
        stack.Children.Add(new TextBlock { Text = "⚙️ 設定", Margin = new System.Windows.Thickness(0, 5, 0, 5), Padding = new System.Windows.Thickness(5) });
        return stack;
    }

    private UIElement CreateCardPreview()
    {
        var border = new Border { BorderBrush = new SolidColorBrush(Colors.LightGray), BorderThickness = new System.Windows.Thickness(1), Margin = new System.Windows.Thickness(10), Padding = new System.Windows.Thickness(15) };
        var stack = new StackPanel();
        stack.Children.Add(new TextBlock { Text = "売上サマリー", FontWeight = System.Windows.FontWeights.Bold, FontSize = 16, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "今月の売上", Foreground = new SolidColorBrush(Colors.Gray), FontSize = 12, Margin = new System.Windows.Thickness(0, 0, 0, 5) });
        stack.Children.Add(new TextBlock { Text = "¥12,345,678", FontSize = 24, FontWeight = System.Windows.FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Green) });
        stack.Children.Add(new TextBlock { Text = "前月比: +15.3% ↑", Foreground = new SolidColorBrush(Colors.Green), FontSize = 12, Margin = new System.Windows.Thickness(0, 5, 0, 0) });
        border.Child = stack;
        return border;
    }

    private UIElement CreatePagingPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10), HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
        stack.Children.Add(new TextBlock { Text = "ページングコントロール", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10), HorizontalAlignment = System.Windows.HorizontalAlignment.Center });
        var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
        buttonPanel.Children.Add(new Button { Content = "◀", Padding = new System.Windows.Thickness(10, 5, 10, 5), Margin = new System.Windows.Thickness(2, 0, 2, 0) });
        buttonPanel.Children.Add(new Button { Content = "1", Padding = new System.Windows.Thickness(10, 5, 10, 5), Margin = new System.Windows.Thickness(2, 0, 2, 0), Background = new SolidColorBrush(Colors.LightBlue) });
        buttonPanel.Children.Add(new Button { Content = "2", Padding = new System.Windows.Thickness(10, 5, 10, 5), Margin = new System.Windows.Thickness(2, 0, 2, 0) });
        buttonPanel.Children.Add(new Button { Content = "3", Padding = new System.Windows.Thickness(10, 5, 10, 5), Margin = new System.Windows.Thickness(2, 0, 2, 0) });
        buttonPanel.Children.Add(new TextBlock { Text = "...", VerticalAlignment = System.Windows.VerticalAlignment.Center, Margin = new System.Windows.Thickness(5, 0, 5, 0) });
        buttonPanel.Children.Add(new Button { Content = "10", Padding = new System.Windows.Thickness(10, 5, 10, 5), Margin = new System.Windows.Thickness(2, 0, 2, 0) });
        buttonPanel.Children.Add(new Button { Content = "▶", Padding = new System.Windows.Thickness(10, 5, 10, 5), Margin = new System.Windows.Thickness(2, 0, 2, 0) });
        stack.Children.Add(buttonPanel);
        stack.Children.Add(new TextBlock { Text = "1 - 20 / 全200件", FontSize = 11, Foreground = new SolidColorBrush(Colors.Gray), HorizontalAlignment = System.Windows.HorizontalAlignment.Center, Margin = new System.Windows.Thickness(0, 10, 0, 0) });
        return stack;
    }

    private UIElement CreateTreeViewPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "拡張ツリービュー", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        stack.Children.Add(new TextBlock { Text = "☑ 📁 マスタデータ", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "  ☑ 👤 顧客マスタ", Margin = new System.Windows.Thickness(20, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "  ☐ 📦 商品マスタ", Margin = new System.Windows.Thickness(20, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "☐ 📊 トランザクション", Margin = new System.Windows.Thickness(0, 3, 0, 3) });
        stack.Children.Add(new TextBlock { Text = "  ☐ 💰 売上データ", Margin = new System.Windows.Thickness(20, 3, 0, 3) });
        return stack;
    }

    private UIElement CreateTabControlPreview()
    {
        var stack = new StackPanel { Margin = new System.Windows.Thickness(10) };
        stack.Children.Add(new TextBlock { Text = "拡張TabControl", FontWeight = System.Windows.FontWeights.Bold, Margin = new System.Windows.Thickness(0, 0, 0, 10) });
        var tabPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new System.Windows.Thickness(0, 0, 0, 10) };
        tabPanel.Children.Add(new Border { BorderBrush = new SolidColorBrush(Colors.Blue), BorderThickness = new System.Windows.Thickness(0, 0, 0, 2), Padding = new System.Windows.Thickness(10, 5, 10, 5), Margin = new System.Windows.Thickness(0, 0, 5, 0), Child = new TextBlock { Text = "基本情報" } });
        tabPanel.Children.Add(new Border { Padding = new System.Windows.Thickness(10, 5, 10, 5), Margin = new System.Windows.Thickness(0, 0, 5, 0), Child = new TextBlock { Text = "詳細", Foreground = new SolidColorBrush(Colors.Gray) } });
        tabPanel.Children.Add(new Border { Padding = new System.Windows.Thickness(10, 5, 10, 5), Child = new TextBlock { Text = "履歴", Foreground = new SolidColorBrush(Colors.Gray) } });
        stack.Children.Add(tabPanel);
        var content = new Border { BorderBrush = new SolidColorBrush(Colors.LightGray), BorderThickness = new System.Windows.Thickness(1), Padding = new System.Windows.Thickness(15) };
        content.Child = new TextBlock { Text = "タブコンテンツエリア\n\n複数の子画面を切り替えて表示", Foreground = new SolidColorBrush(Colors.Gray) };
        stack.Children.Add(content);
        return stack;
    }
}
