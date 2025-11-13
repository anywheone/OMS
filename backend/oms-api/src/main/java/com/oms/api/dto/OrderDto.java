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

/**
 * API レスポンス基底クラス
 */
@Data
public class ApiResponse<T> {
    private boolean success;
    private T data;
    private String message;
    private java.util.List<String> errors;

    public static <T> ApiResponse<T> success(T data) {
        ApiResponse<T> response = new ApiResponse<>();
        response.setSuccess(true);
        response.setData(data);
        return response;
    }

    public static <T> ApiResponse<T> success(T data, String message) {
        ApiResponse<T> response = new ApiResponse<>();
        response.setSuccess(true);
        response.setData(data);
        response.setMessage(message);
        return response;
    }

    public static <T> ApiResponse<T> error(String message) {
        ApiResponse<T> response = new ApiResponse<>();
        response.setSuccess(false);
        response.setMessage(message);
        return response;
    }

    public static <T> ApiResponse<T> error(String message, java.util.List<String> errors) {
        ApiResponse<T> response = new ApiResponse<>();
        response.setSuccess(false);
        response.setMessage(message);
        response.setErrors(errors);
        return response;
    }
}
