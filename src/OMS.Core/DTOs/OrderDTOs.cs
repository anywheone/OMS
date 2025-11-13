using OMS.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace OMS.Core.DTOs;

/// <summary>
/// 発注作成DTO
/// </summary>
public class CreateOrderDto
{
    [Required(ErrorMessage = "銘柄IDは必須です")]
    public long SecurityId { get; set; }

    [Required(ErrorMessage = "売買区分は必須です")]
    public OrderSide Side { get; set; }

    [Required(ErrorMessage = "注文タイプは必須です")]
    public OrderType OrderType { get; set; }

    [Required(ErrorMessage = "数量は必須です")]
    [Range(1, double.MaxValue, ErrorMessage = "数量は1以上である必要があります")]
    public decimal Quantity { get; set; }

    public decimal? Price { get; set; }

    public decimal? StopPrice { get; set; }

    [Required]
    public TimeInForce TimeInForce { get; set; } = TimeInForce.DAY;

    public DateTime? ValidUntil { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// 発注更新DTO
/// </summary>
public class UpdateOrderDto
{
    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? StopPrice { get; set; }
    public TimeInForce? TimeInForce { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 発注レスポンスDTO
/// </summary>
public class OrderDto
{
    public long OrderId { get; set; }
    public long UserId { get; set; }
    public long SecurityId { get; set; }
    public string OrderNo { get; set; } = string.Empty;
    public OrderSide Side { get; set; }
    public OrderType OrderType { get; set; }
    public decimal Quantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? StopPrice { get; set; }
    public TimeInForce TimeInForce { get; set; }
    public OrderStatus Status { get; set; }
    public decimal FilledQuantity { get; set; }
    public decimal? AveragePrice { get; set; }
    public decimal? Commission { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // 関連データ
    public string? SecurityCode { get; set; }
    public string? SecurityName { get; set; }
    public string? Username { get; set; }
}

/// <summary>
/// 発注フィルタDTO
/// </summary>
public class OrderFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<OrderStatus>? Statuses { get; set; }
    public List<long>? SecurityIds { get; set; }
    public OrderSide? Side { get; set; }
    public long? UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.DESCENDING;
}

/// <summary>
/// 約定DTO
/// </summary>
public class ExecutionDto
{
    public long ExecutionId { get; set; }
    public long OrderId { get; set; }
    public string OrderNo { get; set; } = string.Empty;
    public long SecurityId { get; set; }
    public string SecurityCode { get; set; } = string.Empty;
    public string SecurityName { get; set; } = string.Empty;
    public string ExecutionNo { get; set; } = string.Empty;
    public decimal ExecutionPrice { get; set; }
    public decimal ExecutionQuantity { get; set; }
    public decimal Commission { get; set; }
    public DateTime ExecutionDate { get; set; }
    public DateTime SettlementDate { get; set; }
    public OrderSide Side { get; set; }
}

/// <summary>
/// 銘柄DTO
/// </summary>
public class SecurityDto
{
    public long SecurityId { get; set; }
    public string SecurityCode { get; set; } = string.Empty;
    public string SecurityName { get; set; } = string.Empty;
    public SecurityType SecurityType { get; set; }
    public string Market { get; set; } = string.Empty;
    public string? Sector { get; set; }
    public string Currency { get; set; } = "JPY";
    public int LotSize { get; set; }
    public decimal TickSize { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// ポジションDTO
/// </summary>
public class PositionDto
{
    public long PositionId { get; set; }
    public long SecurityId { get; set; }
    public string SecurityCode { get; set; } = string.Empty;
    public string SecurityName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal AverageCost { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal MarketValue { get; set; }
    public decimal UnrealizedPnL { get; set; }
    public decimal PnLPercent { get; set; }
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// API レスポンス基底クラス
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}

/// <summary>
/// ページングレスポンス
/// </summary>
public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
