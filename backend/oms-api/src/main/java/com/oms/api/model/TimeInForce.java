package com.oms.api.model;

/**
 * 執行条件
 */
public enum TimeInForce {
    DAY,  // 当日中
    GTC,  // 無期限
    IOC,  // 即座or取消
    FOK   // 全量約定or取消
}
