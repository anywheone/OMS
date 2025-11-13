using System.ComponentModel;

namespace OMS.Core.Enums;

/// <summary>
/// 売買区分
/// </summary>
public enum OrderSide
{
    /// <summary>
    /// 買い
    /// </summary>
    [Description("買い")]
    BUY,

    /// <summary>
    /// 売り
    /// </summary>
    [Description("売り")]
    SELL
}
