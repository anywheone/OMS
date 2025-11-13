package com.oms.api.model;

/**
 * 注文タイプ
 */
public enum OrderType {
    MARKET,      // 成行
    LIMIT,       // 指値
    STOP,        // 逆指値
    STOP_LIMIT   // 逆指値付き指値
}
