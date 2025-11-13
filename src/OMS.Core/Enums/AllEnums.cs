using System.ComponentModel;

namespace OMS.Core.Enums;

/// <summary>
/// 有効期限タイプ
/// </summary>
public enum TimeInForce
{
    /// <summary>
    /// 当日限り
    /// </summary>
    [Description("当日")]
    DAY,

    /// <summary>
    /// 取消まで有効
    /// </summary>
    [Description("GTC")]
    GTC,

    /// <summary>
    /// 即時約定または取消
    /// </summary>
    [Description("IOC")]
    IOC,

    /// <summary>
    /// 全量即時約定または取消
    /// </summary>
    [Description("FOK")]
    FOK
}

/// <summary>
/// 銘柄種別
/// </summary>
public enum SecurityType
{
    /// <summary>
    /// 株式
    /// </summary>
    [Description("株式")]
    STOCK,

    /// <summary>
    /// 債券
    /// </summary>
    [Description("債券")]
    BOND,

    /// <summary>
    /// ETF
    /// </summary>
    [Description("ETF")]
    ETF,

    /// <summary>
    /// REIT
    /// </summary>
    [Description("REIT")]
    REIT,

    /// <summary>
    /// 投資信託
    /// </summary>
    [Description("投資信託")]
    FUND,

    /// <summary>
    /// デリバティブ
    /// </summary>
    [Description("デリバティブ")]
    DERIVATIVE
}

/// <summary>
/// ユーザー権限
/// </summary>
public enum UserRole
{
    /// <summary>
    /// トレーダー
    /// </summary>
    [Description("トレーダー")]
    TRADER,

    /// <summary>
    /// マネージャー
    /// </summary>
    [Description("マネージャー")]
    MANAGER,

    /// <summary>
    /// 管理者
    /// </summary>
    [Description("管理者")]
    ADMIN
}

/// <summary>
/// 通知タイプ
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// 注文関連
    /// </summary>
    [Description("注文")]
    ORDER,

    /// <summary>
    /// 約定関連
    /// </summary>
    [Description("約定")]
    EXECUTION,

    /// <summary>
    /// システム
    /// </summary>
    [Description("システム")]
    SYSTEM,

    /// <summary>
    /// アラート
    /// </summary>
    [Description("アラート")]
    ALERT
}

/// <summary>
/// 通知の重要度
/// </summary>
public enum NotificationSeverity
{
    /// <summary>
    /// 情報
    /// </summary>
    [Description("情報")]
    INFO,

    /// <summary>
    /// 警告
    /// </summary>
    [Description("警告")]
    WARNING,

    /// <summary>
    /// エラー
    /// </summary>
    [Description("エラー")]
    ERROR,

    /// <summary>
    /// 成功
    /// </summary>
    [Description("成功")]
    SUCCESS
}

/// <summary>
/// 市場状態
/// </summary>
public enum MarketStatus
{
    /// <summary>
    /// 閉場中
    /// </summary>
    [Description("閉場中")]
    CLOSED,

    /// <summary>
    /// プレオープン
    /// </summary>
    [Description("プレオープン")]
    PRE_OPEN,

    /// <summary>
    /// 開場中
    /// </summary>
    [Description("開場中")]
    OPEN,

    /// <summary>
    /// 昼休み
    /// </summary>
    [Description("昼休み")]
    LUNCH_BREAK,

    /// <summary>
    /// ポストクローズ
    /// </summary>
    [Description("ポストクローズ")]
    POST_CLOSE
}

/// <summary>
/// チャートタイプ
/// </summary>
public enum ChartType
{
    /// <summary>
    /// 資産推移
    /// </summary>
    [Description("資産推移")]
    EQUITY_CURVE,

    /// <summary>
    /// セクター構成
    /// </summary>
    [Description("セクター構成")]
    SECTOR_ALLOCATION,

    /// <summary>
    /// パフォーマンス比較
    /// </summary>
    [Description("パフォーマンス")]
    PERFORMANCE
}

/// <summary>
/// ソート順
/// </summary>
public enum SortOrder
{
    /// <summary>
    /// 昇順
    /// </summary>
    [Description("昇順")]
    ASCENDING,

    /// <summary>
    /// 降順
    /// </summary>
    [Description("降順")]
    DESCENDING
}
