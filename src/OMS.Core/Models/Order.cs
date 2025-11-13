using OMS.Core.Enums;

namespace OMS.Core.Models;

/// <summary>
/// 発注モデル
/// </summary>
public class Order
{
    /// <summary>
    /// 発注ID
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// ユーザーID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 銘柄ID
    /// </summary>
    public long SecurityId { get; set; }

    /// <summary>
    /// 注文番号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 売買区分
    /// </summary>
    public OrderSide Side { get; set; }

    /// <summary>
    /// 注文タイプ
    /// </summary>
    public OrderType OrderType { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// 価格（指値の場合）
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// 逆指値価格
    /// </summary>
    public decimal? StopPrice { get; set; }

    /// <summary>
    /// 有効期限
    /// </summary>
    public TimeInForce TimeInForce { get; set; }

    /// <summary>
    /// ステータス
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// 約定済み数量
    /// </summary>
    public decimal FilledQuantity { get; set; }

    /// <summary>
    /// 平均約定価格
    /// </summary>
    public decimal? AveragePrice { get; set; }

    /// <summary>
    /// 手数料
    /// </summary>
    public decimal? Commission { get; set; }

    /// <summary>
    /// 発注日時
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 有効期限日時
    /// </summary>
    public DateTime? ValidUntil { get; set; }

    /// <summary>
    /// 備考
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // ナビゲーションプロパティ
    public Security? Security { get; set; }
    public User? User { get; set; }
    public ICollection<Execution>? Executions { get; set; }

    /// <summary>
    /// 残数量を計算
    /// </summary>
    public decimal RemainingQuantity => Quantity - FilledQuantity;

    /// <summary>
    /// 約定率を計算（%）
    /// </summary>
    public decimal FillRate => Quantity > 0 ? (FilledQuantity / Quantity) * 100 : 0;

    /// <summary>
    /// 注文が完全に約定したかどうか
    /// </summary>
    public bool IsFullyFilled => Status == OrderStatus.FILLED;

    /// <summary>
    /// 注文がアクティブかどうか
    /// </summary>
    public bool IsActive => Status == OrderStatus.NEW || Status == OrderStatus.PARTIAL;
}
