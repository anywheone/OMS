package com.oms.api.model;

/**
 * 注文ステータス
 */
public enum OrderStatus {
    NEW,       // 新規
    PARTIAL,   // 一部約定
    FILLED,    // 全量約定
    CANCELED,  // キャンセル
    REJECTED,  // 拒否
    EXPIRED    // 期限切れ
}
