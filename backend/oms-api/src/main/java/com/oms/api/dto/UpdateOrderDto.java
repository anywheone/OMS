package com.oms.api.dto;

import com.oms.api.model.TimeInForce;
import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDateTime;

/**
 * 発注更新DTO
 */
@Data
public class UpdateOrderDto {
    private BigDecimal quantity;
    private BigDecimal price;
    private BigDecimal stopPrice;
    private TimeInForce timeInForce;
    private LocalDateTime validUntil;
    private String notes;
}
