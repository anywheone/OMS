using OMS.Core.Enums;

namespace OMS.Core.Models;

/// <summary>
/// 銘柄モデル
/// </summary>
public class Security
{
    public long SecurityId { get; set; }
    public string SecurityCode { get; set; } = string.Empty;
    public string SecurityName { get; set; } = string.Empty;
    public SecurityType SecurityType { get; set; }
    public string Market { get; set; } = string.Empty;
    public string? Sector { get; set; }
    public string Currency { get; set; } = "JPY";
    public int LotSize { get; set; } = 100;
    public decimal TickSize { get; set; } = 1.0m;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public override string ToString() => $"{SecurityCode} - {SecurityName}";
}

/// <summary>
/// 約定モデル
/// </summary>
public class Execution
{
    public long ExecutionId { get; set; }
    public long OrderId { get; set; }
    public long SecurityId { get; set; }
    public string ExecutionNo { get; set; } = string.Empty;
    public decimal ExecutionPrice { get; set; }
    public decimal ExecutionQuantity { get; set; }
    public decimal Commission { get; set; }
    public DateTime ExecutionDate { get; set; }
    public DateTime SettlementDate { get; set; }
    public string? ContraBroker { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    // ナビゲーションプロパティ
    public Order? Order { get; set; }
    public Security? Security { get; set; }

    /// <summary>
    /// 約定金額を計算
    /// </summary>
    public decimal ExecutionAmount => ExecutionPrice * ExecutionQuantity;

    /// <summary>
    /// 純額（手数料込み）
    /// </summary>
    public decimal NetAmount => ExecutionAmount + Commission;
}

/// <summary>
/// ポジションモデル
/// </summary>
public class Position
{
    public long PositionId { get; set; }
    public long UserId { get; set; }
    public long SecurityId { get; set; }
    public decimal Quantity { get; set; }
    public decimal AverageCost { get; set; }
    public decimal? CurrentPrice { get; set; }
    public decimal? UnrealizedPnL { get; set; }
    public decimal RealizedPnL { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // ナビゲーションプロパティ
    public User? User { get; set; }
    public Security? Security { get; set; }

    /// <summary>
    /// 時価評価額
    /// </summary>
    public decimal MarketValue => Quantity * (CurrentPrice ?? 0);

    /// <summary>
    /// 評価損益率（%）
    /// </summary>
    public decimal PnLPercent => AverageCost > 0 ? ((CurrentPrice ?? 0) - AverageCost) / AverageCost * 100 : 0;
}

/// <summary>
/// ユーザーモデル
/// </summary>
public class User
{
    public long UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 取引履歴モデル
/// </summary>
public class Trade
{
    public long TradeId { get; set; }
    public long OrderId { get; set; }
    public long? ExecutionId { get; set; }
    public long UserId { get; set; }
    public long SecurityId { get; set; }
    public DateTime TradeDate { get; set; }
    public OrderSide Side { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public decimal Commission { get; set; }
    public decimal NetAmount { get; set; }
    public decimal? PnL { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    // ナビゲーションプロパティ
    public Security? Security { get; set; }
    public User? User { get; set; }
}

/// <summary>
/// 通知モデル
/// </summary>
public class Notification
{
    public long NotificationId { get; set; }
    public long UserId { get; set; }
    public NotificationType NotificationType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationSeverity Severity { get; set; }
    public bool IsRead { get; set; }
    public long? RelatedOrderId { get; set; }
    public long? RelatedExecutionId { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// ポートフォリオサマリー
/// </summary>
public class PortfolioSummary
{
    public long UserId { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal DailyPnL { get; set; }
    public decimal DailyPnLPercent { get; set; }
    public decimal UnrealizedPnL { get; set; }
    public decimal UnrealizedPnLPercent { get; set; }
    public decimal RealizedPnL { get; set; }
    public DateTime AsOfDate { get; set; }
}

/// <summary>
/// アセットアロケーション項目
/// </summary>
public class AllocationItem
{
    public string Category { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal Percent { get; set; }
    public string? Color { get; set; }
}

/// <summary>
/// ポートフォリオスナップショット（時系列データ）
/// </summary>
public class PortfolioSnapshot
{
    public DateTime Date { get; set; }
    public decimal TotalValue { get; set; }
    public decimal DailyPnL { get; set; }
}

/// <summary>
/// 価格データポイント
/// </summary>
public class PricePoint
{
    public DateTime Timestamp { get; set; }
    public decimal Price { get; set; }
    public long? Volume { get; set; }
}

/// <summary>
/// 市場指数
/// </summary>
public class Index
{
    public string IndexCode { get; set; } = string.Empty;
    public string IndexName { get; set; } = string.Empty;
    public decimal CurrentValue { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
    public DateTime LastUpdated { get; set; }
}
