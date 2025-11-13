using System.ComponentModel;

namespace OMS.Core.Enums;

/// <summary>
/// 注文タイプ
/// </summary>
public enum OrderType
{
    /// <summary>
    /// 成行注文
    /// </summary>
    [Description("成行")]
    MARKET,

    /// <summary>
    /// 指値注文
    /// </summary>
    [Description("指値")]
    LIMIT,

    /// <summary>
    /// 逆指値注文
    /// </summary>
    [Description("逆指値")]
    STOP,

    /// <summary>
    /// 逆指値付指値注文
    /// </summary>
    [Description("逆指値付指値")]
    STOP_LIMIT
}
