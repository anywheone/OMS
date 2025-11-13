using System;

namespace OMS.Client.Models;

/// <summary>
/// 注文モデル（UI用）
/// </summary>
public class OrderModel
{
    public long? OrderId { get; set; }
    public string OrderNo { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public long UserId { get; set; }
    public long SecurityId { get; set; }
    public string Side { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? StopPrice { get; set; }
    public string TimeInForce { get; set; } = string.Empty;
    public DateTime? ValidUntil { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal FilledQuantity { get; set; }
    public decimal? AveragePrice { get; set; }
    public decimal? Commission { get; set; }
    public string? Notes { get; set; }
}
