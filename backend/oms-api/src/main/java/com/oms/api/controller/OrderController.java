package com.oms.api.controller;

import com.oms.api.dto.*;
import com.oms.api.model.OrderStatus;
import com.oms.api.service.OrderService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.List;

/**
 * 発注API コントローラー
 *
 * エンドポイント:
 * - POST   /api/orders           : 新規発注
 * - GET    /api/orders           : 発注一覧取得（フィルター可）
 * - GET    /api/orders/{id}      : 発注詳細取得
 * - PUT    /api/orders/{id}      : 発注修正
 * - DELETE /api/orders/{id}      : 発注取消
 * - GET    /api/orders/active    : アクティブな発注一覧
 */
@RestController
@RequestMapping("/api/orders")
@RequiredArgsConstructor
@Slf4j
@Tag(name = "Order", description = "発注管理API")
public class OrderController {

    private final OrderService orderService;

    /**
     * 新規発注
     *
     * リクエスト例:
     * POST /api/orders?userId=1
     * {
     *   "securityId": 1,
     *   "side": "BUY",
     *   "orderType": "LIMIT",
     *   "quantity": 1000,
     *   "price": 2500.00,
     *   "timeInForce": "DAY"
     * }
     */
    @PostMapping
    @Operation(summary = "新規発注", description = "新しい注文を作成します")
    public ResponseEntity<ApiResponse<OrderDto>> createOrder(
            @RequestParam(required = false, defaultValue = "1") Long userId,
            @Valid @RequestBody CreateOrderDto dto) {
        try {
            log.info("POST /api/orders - userId: {}", userId);
            OrderDto order = orderService.createOrder(userId, dto);
            return ResponseEntity.status(HttpStatus.CREATED)
                    .body(ApiResponse.success(order, "発注が完了しました"));
        } catch (IllegalArgumentException e) {
            log.error("Validation error: {}", e.getMessage());
            return ResponseEntity.badRequest()
                    .body(ApiResponse.error(e.getMessage()));
        } catch (Exception e) {
            log.error("Error creating order", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                    .body(ApiResponse.error("発注処理中にエラーが発生しました"));
        }
    }

    /**
     * 発注詳細取得
     */
    @GetMapping("/{id}")
    @Operation(summary = "発注詳細取得", description = "指定IDの注文詳細を取得します")
    public ResponseEntity<ApiResponse<OrderDto>> getOrderById(@PathVariable Long id) {
        try {
            log.info("GET /api/orders/{}", id);
            OrderDto order = orderService.getOrderById(id);
            return ResponseEntity.ok(ApiResponse.success(order));
        } catch (RuntimeException e) {
            log.error("Order not found: {}", id);
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body(ApiResponse.error("注文が見つかりません"));
        }
    }

    /**
     * 発注一覧取得（フィルター対応）
     *
     * クエリパラメータ:
     * - userId: ユーザーID
     * - securityId: 銘柄ID
     * - statuses: ステータス（カンマ区切り）
     * - startDate: 開始日時
     * - endDate: 終了日時
     */
    @GetMapping
    @Operation(summary = "発注一覧取得", description = "フィルター条件に合致する注文一覧を取得します")
    public ResponseEntity<ApiResponse<List<OrderDto>>> getOrders(
            @RequestParam(required = false) Long userId,
            @RequestParam(required = false) Long securityId,
            @RequestParam(required = false) List<OrderStatus> statuses,
            @RequestParam(required = false) @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME) LocalDateTime startDate,
            @RequestParam(required = false) @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME) LocalDateTime endDate) {
        try {
            log.info("GET /api/orders - userId: {}, filters: securityId={}, statuses={}, dates={}-{}",
                    userId, securityId, statuses, startDate, endDate);

            List<OrderDto> orders;
            if (userId != null) {
                if (securityId != null || statuses != null || startDate != null || endDate != null) {
                    // フィルター検索
                    orders = orderService.findOrdersByFilters(userId, securityId, statuses, startDate, endDate);
                } else {
                    // 全件取得
                    orders = orderService.getOrdersByUserId(userId);
                }
            } else {
                // userId指定なしの場合は全ユーザーの注文を取得
                orders = orderService.getAllOrders();
            }

            return ResponseEntity.ok(ApiResponse.success(orders));
        } catch (Exception e) {
            log.error("Error fetching orders", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                    .body(ApiResponse.error("注文一覧取得中にエラーが発生しました"));
        }
    }

    /**
     * アクティブな発注一覧取得
     */
    @GetMapping("/active")
    @Operation(summary = "アクティブな発注一覧", description = "NEW/PARTIALステータスの注文を取得します")
    public ResponseEntity<ApiResponse<List<OrderDto>>> getActiveOrders(@RequestParam Long userId) {
        try {
            log.info("GET /api/orders/active - userId: {}", userId);
            List<OrderDto> orders = orderService.getActiveOrders(userId);
            return ResponseEntity.ok(ApiResponse.success(orders));
        } catch (Exception e) {
            log.error("Error fetching active orders", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                    .body(ApiResponse.error("アクティブ注文取得中にエラーが発生しました"));
        }
    }

    /**
     * 発注修正
     */
    @PutMapping("/{id}")
    @Operation(summary = "発注修正", description = "指定IDの注文を修正します")
    public ResponseEntity<ApiResponse<OrderDto>> updateOrder(
            @PathVariable Long id,
            @Valid @RequestBody UpdateOrderDto dto) {
        try {
            log.info("PUT /api/orders/{}", id);
            OrderDto order = orderService.updateOrder(id, dto);
            return ResponseEntity.ok(ApiResponse.success(order, "注文を更新しました"));
        } catch (RuntimeException e) {
            log.error("Error updating order: {}", e.getMessage());
            return ResponseEntity.badRequest()
                    .body(ApiResponse.error(e.getMessage()));
        }
    }

    /**
     * 発注取消
     */
    @DeleteMapping("/{id}")
    @Operation(summary = "発注取消", description = "指定IDの注文をキャンセルします")
    public ResponseEntity<ApiResponse<OrderDto>> cancelOrder(@PathVariable Long id) {
        try {
            log.info("DELETE /api/orders/{}", id);
            OrderDto order = orderService.cancelOrder(id);
            return ResponseEntity.ok(ApiResponse.success(order, "注文を取り消しました"));
        } catch (RuntimeException e) {
            log.error("Error canceling order: {}", e.getMessage());
            return ResponseEntity.badRequest()
                    .body(ApiResponse.error(e.getMessage()));
        }
    }

    /**
     * グローバルエラーハンドラー
     */
    @ExceptionHandler(Exception.class)
    public ResponseEntity<ApiResponse<Void>> handleGlobalException(Exception e) {
        log.error("Unhandled exception", e);
        return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(ApiResponse.error("システムエラーが発生しました: " + e.getMessage()));
    }
}
