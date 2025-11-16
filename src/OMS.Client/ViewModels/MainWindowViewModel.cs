using System;
using System.Collections.ObjectModel;
using System.Windows;
using Prism.Mvvm;
using Prism.Commands;
using Prism.Ioc;
using Microsoft.Extensions.Logging;
using OMS.Client.Models;
using OMS.Client.Services;

namespace OMS.Client.ViewModels;

/// <summary>
/// MainWindowのViewModel
/// </summary>
public class MainWindowViewModel : BindableBase
{
    private readonly ILogger<MainWindowViewModel>? _logger;
    private readonly OrderService? _orderService;
    private readonly IContainerProvider? _container;

    private string _securityId = "1001";
    private string _side = "BUY";
    private string _orderType = "MARKET";
    private string _quantity = "100";
    private string _price = "";
    private string _timeInForce = "DAY";

    public MainWindowViewModel()
    {
        // デザインタイム用のコンストラクタ
        Orders = new ObservableCollection<OrderModel>();
        PlaceOrderCommand = new DelegateCommand(async () => await PlaceOrderAsync());
        RefreshOrdersCommand = new DelegateCommand(async () => await LoadOrdersAsync());
        ClearCommand = new DelegateCommand(ClearForm);
        ShowControlLibraryCommand = new DelegateCommand(ShowControlLibrary);
    }

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, OrderService orderService, IContainerProvider container)
    {
        _logger = logger;
        _orderService = orderService;
        _container = container;
        Orders = new ObservableCollection<OrderModel>();

        PlaceOrderCommand = new DelegateCommand(async () => await PlaceOrderAsync());
        RefreshOrdersCommand = new DelegateCommand(async () => await LoadOrdersAsync());
        ClearCommand = new DelegateCommand(ClearForm);
        ShowControlLibraryCommand = new DelegateCommand(ShowControlLibrary);

        _logger?.LogInformation("MainWindowViewModel initialized");

        // 初期データロード
        _ = LoadOrdersAsync();
    }

    #region Properties

    public string SecurityId
    {
        get => _securityId;
        set => SetProperty(ref _securityId, value);
    }

    public string Side
    {
        get => _side;
        set => SetProperty(ref _side, value);
    }

    public string OrderType
    {
        get => _orderType;
        set
        {
            SetProperty(ref _orderType, value);
            RaisePropertyChanged(nameof(IsPriceEnabled));
        }
    }

    public string Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    public string Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }

    public string TimeInForce
    {
        get => _timeInForce;
        set => SetProperty(ref _timeInForce, value);
    }

    public bool IsPriceEnabled => OrderType == "LIMIT" || OrderType == "STOP_LIMIT";

    #endregion

    #region Collections

    /// <summary>
    /// 注文一覧
    /// </summary>
    public ObservableCollection<OrderModel> Orders { get; }

    /// <summary>
    /// 約定履歴
    /// </summary>
    public ObservableCollection<object> Executions { get; } = new();

    /// <summary>
    /// ポジション一覧
    /// </summary>
    public ObservableCollection<object> Positions { get; } = new();

    #endregion

    #region Commands

    public DelegateCommand PlaceOrderCommand { get; }
    public DelegateCommand RefreshOrdersCommand { get; }
    public DelegateCommand ClearCommand { get; }
    public DelegateCommand ShowControlLibraryCommand { get; }

    #endregion

    #region Methods

    private async System.Threading.Tasks.Task PlaceOrderAsync()
    {
        try
        {
            if (_orderService == null)
            {
                MessageBox.Show("OrderServiceが初期化されていません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // バリデーション
            if (!long.TryParse(SecurityId, out var securityId))
            {
                MessageBox.Show("銘柄コードは数値で入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(Quantity, out var quantity) || quantity <= 0)
            {
                MessageBox.Show("数量は正の数値で入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal? price = null;
            if (IsPriceEnabled && !string.IsNullOrWhiteSpace(Price))
            {
                if (!decimal.TryParse(Price, out var p))
                {
                    MessageBox.Show("価格は数値で入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                price = p;
            }

            var order = new OrderModel
            {
                UserId = 1, // テスト用の固定値
                SecurityId = securityId,
                Side = Side,
                OrderType = OrderType,
                Quantity = quantity,
                Price = price,
                TimeInForce = TimeInForce,
                OrderDate = DateTime.Now
            };

            _logger?.LogInformation("注文を送信します: {Order}", order);

            var result = await _orderService.CreateOrderAsync(order);

            MessageBox.Show($"注文を受け付けました。\n注文番号: {result.OrderNo}", "注文完了", MessageBoxButton.OK, MessageBoxImage.Information);

            // 注文一覧を更新
            await LoadOrdersAsync();

            // フォームをクリア
            ClearForm();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "注文の送信に失敗しました");
            MessageBox.Show($"注文の送信に失敗しました。\n{ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async System.Threading.Tasks.Task LoadOrdersAsync()
    {
        try
        {
            if (_orderService == null) return;

            _logger?.LogInformation("注文一覧を取得します");

            var orders = await _orderService.GetOrdersAsync();

            Orders.Clear();
            foreach (var order in orders)
            {
                Orders.Add(order);
            }

            _logger?.LogInformation("注文一覧を取得しました: {Count}件", orders.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "注文一覧の取得に失敗しました");
            MessageBox.Show($"注文一覧の取得に失敗しました。\n{ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ClearForm()
    {
        SecurityId = "1001";
        Side = "BUY";
        OrderType = "MARKET";
        Quantity = "100";
        Price = "";
        TimeInForce = "DAY";
    }

    private void ShowControlLibrary()
    {
        try
        {
            if (_container == null)
            {
                MessageBox.Show("コンテナが初期化されていません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var window = _container.Resolve<Views.ControlLibraryWindow>();
            window.Show();
            _logger?.LogInformation("コントロールライブラリウィンドウを表示しました");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "コントロールライブラリウィンドウの表示に失敗しました");
            MessageBox.Show($"コントロールライブラリウィンドウの表示に失敗しました。\n{ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    #endregion
}
