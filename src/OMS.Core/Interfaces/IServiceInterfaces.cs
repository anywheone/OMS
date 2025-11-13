using OMS.Core.DTOs;
using OMS.Core.Models;

namespace OMS.Core.Interfaces;

/// <summary>
/// 発注サービスインターフェース
/// </summary>
public interface IOrderService
{
    Task<ApiResponse<OrderDto>> CreateOrderAsync(CreateOrderDto dto);
    Task<ApiResponse<PagedResponse<OrderDto>>> GetOrdersAsync(OrderFilterDto filter);
    Task<ApiResponse<OrderDto>> GetOrderByIdAsync(long orderId);
    Task<ApiResponse<OrderDto>> UpdateOrderAsync(long orderId, UpdateOrderDto dto);
    Task<ApiResponse<bool>> CancelOrderAsync(long orderId);
}

/// <summary>
/// 約定サービスインターフェース
/// </summary>
public interface IExecutionService
{
    Task<ApiResponse<PagedResponse<ExecutionDto>>> GetExecutionsAsync(OrderFilterDto filter);
    Task<ApiResponse<ExecutionDto>> GetExecutionByIdAsync(long executionId);
    Task<ApiResponse<List<ExecutionDto>>> GetExecutionsByOrderIdAsync(long orderId);
}

/// <summary>
/// 銘柄サービスインターフェース
/// </summary>
public interface ISecurityService
{
    Task<ApiResponse<List<SecurityDto>>> SearchSecuritiesAsync(string query);
    Task<ApiResponse<SecurityDto>> GetSecurityByIdAsync(long securityId);
    Task<ApiResponse<SecurityDto>> GetSecurityByCodeAsync(string code);
}

/// <summary>
/// ポジションサービスインターフェース
/// </summary>
public interface IPositionService
{
    Task<ApiResponse<List<PositionDto>>> GetPositionsAsync(long userId);
    Task<ApiResponse<PositionDto>> GetPositionBySecurityAsync(long userId, long securityId);
}

/// <summary>
/// ポートフォリオサービスインターフェース
/// </summary>
public interface IPortfolioService
{
    Task<ApiResponse<PortfolioSummary>> GetPortfolioSummaryAsync(long userId);
    Task<ApiResponse<List<AllocationItem>>> GetAssetAllocationAsync(long userId);
}

/// <summary>
/// 通知サービスインターフェース
/// </summary>
public interface INotificationService
{
    void PublishNotification(Notification notification);
    Task<List<Notification>> GetNotificationsAsync(long userId);
    Task MarkAsReadAsync(long notificationId);
}
