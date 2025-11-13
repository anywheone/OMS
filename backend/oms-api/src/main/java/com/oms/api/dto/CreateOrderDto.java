package com.oms.api.dto;

import com.oms.api.model.*;
import lombok.Data;
import jakarta.validation.constraints.*;
import java.math.BigDecimal;
import java.time.LocalDateTime;

/**
 * 発注作成DTO
 */
@Data
public class CreateOrderDto {
    @NotNull(message = "銘柄IDは必須です")
    private Long securityId;

    @NotNull(message = "売買区分は必須です")
    private OrderSide side;

    @NotNull(message = "注文タイプは必須です")
    private OrderType orderType;

    @NotNull(message = "数量は必須です")
    @DecimalMin(value = "0.0001", message = "数量は0より大きい必要があります")
    private BigDecimal quantity;

    private BigDecimal price;

    private BigDecimal stopPrice;

    @NotNull
    private TimeInForce timeInForce = TimeInForce.DAY;

    private LocalDateTime validUntil;

    private String notes;
}
