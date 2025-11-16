using System.Windows;

namespace OMS.Client.Models;

/// <summary>
/// コントロールタイプ
/// </summary>
public enum ControlType
{
    /// <summary>OMS特有</summary>
    OMS,
    /// <summary>一般業務アプリ</summary>
    General
}

/// <summary>
/// コントロール情報モデル
/// </summary>
public class ControlInfo
{
    /// <summary>コントロール名</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>説明</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>カテゴリ</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>コントロールタイプ</summary>
    public ControlType Type { get; set; } = ControlType.OMS;

    /// <summary>優先度（1-5、5が最高）</summary>
    public int Priority { get; set; }

    /// <summary>使用頻度</summary>
    public string UsageFrequency { get; set; } = string.Empty;

    /// <summary>主な用途</summary>
    public string PrimaryUse { get; set; } = string.Empty;

    /// <summary>プレビューコントロール</summary>
    public UIElement? PreviewControl { get; set; }

    /// <summary>アイコン</summary>
    public string Icon { get; set; } = "📊";

    /// <summary>タグ</summary>
    public List<string> Tags { get; set; } = new();
}
