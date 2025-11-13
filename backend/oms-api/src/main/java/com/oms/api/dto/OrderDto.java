package com.oms.api.dto;

import com.oms.api.model.*;
import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDateTime;

/**
 * 発注レスポンスDTO
 */
@Data
public class OrderDto {
    private Long orderId;
    private Long userId;
    private Long securityId;
    private String orderNo;
    private OrderSide side;
    private OrderType orderType;
    private BigDecimal quantity;
    private BigDecimal price;
    private BigDecimal stopPrice;
    private TimeInForce timeInForce;
    private OrderStatus status;
    private BigDecimal filledQuantity;
    private BigDecimal averagePrice;
    private BigDecimal commission;
    private LocalDateTime orderDate;
    private LocalDateTime validUntil;
    private String notes;
    private LocalDateTime createdAt;
    private LocalDateTime updatedAt;

    // 関連データ（JOIN結果）
    private String securityCode;
    private String securityName;
    private String username;

    // 計算フィールド
    private BigDecimal remainingQuantity;
    private BigDecimal fillRate;
}
