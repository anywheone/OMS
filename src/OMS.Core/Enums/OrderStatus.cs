using System.ComponentModel;

namespace OMS.Core.Enums;

/// <summary>
/// 注文ステータス
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 新規（未約定）
    /// </summary>
    [Description("新規")]
    NEW,

    /// <summary>
    /// 部分約定
    /// </summary>
    [Description("部分約定")]
    PARTIAL,

    /// <summary>
    /// 全約定
    /// </summary>
    [Description("全約定")]
    FILLED,

    /// <summary>
    /// 取消済み
    /// </summary>
    [Description("取消済み")]
    CANCELED,

    /// <summary>
    /// 拒否
    /// </summary>
    [Description("拒否")]
    REJECTED,

    /// <summary>
    /// 期限切れ
    /// </summary>
    [Description("期限切れ")]
    EXPIRED
}
