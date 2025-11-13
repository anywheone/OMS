package com.oms.api.service;

import com.oms.api.dto.*;
import com.oms.api.model.*;
import com.oms.api.repository.OrderRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.modelmapper.ModelMapper;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.List;
import java.util.stream.Collectors;

/**
 * 発注サービス
 */
@Service
@RequiredArgsConstructor
@Slf4j
public class OrderService {

    private final OrderRepository orderRepository;
    private final ModelMapper modelMapper;

    /**
     * 発注作成
     */
    @Transactional
    public OrderDto createOrder(Long userId, CreateOrderDto dto) {
        log.info("Creating order for user: {}, security: {}", userId, dto.getSecurityId());

        // バリデーション
        validateOrder(dto);

        // エンティティ作成
        Order order = new Order();
        order.setUserId(userId);
        order.setSecurityId(dto.getSecurityId());
        order.setOrderNo(generateOrderNo());
        order.setSide(dto.getSide());
        order.setOrderType(dto.getOrderType());
        order.setQuantity(dto.getQuantity());
        order.setPrice(dto.getPrice());
        order.setStopPrice(dto.getStopPrice());
        order.setTimeInForce(dto.getTimeInForce());
        order.setValidUntil(dto.getValidUntil());
        order.setNotes(dto.getNotes());
        order.setStatus(OrderStatus.NEW);
        order.setFilledQuantity(BigDecimal.ZERO);

        // 保存
        Order savedOrder = orderRepository.save(order);
        log.info("Order created successfully: {}", savedOrder.getOrderNo());

        return convertToDto(savedOrder);
    }

    /**
     * 注文取得（ID指定）
     */
    public OrderDto getOrderById(Long orderId) {
        Order order = orderRepository.findById(orderId)
                .orElseThrow(() -> new RuntimeException("Order not found: " + orderId));
        return convertToDto(order);
    }

    /**
     * ユーザーの注文一覧取得
     */
    public List<OrderDto> getOrdersByUserId(Long userId) {
        List<Order> orders = orderRepository.findByUserIdOrderByOrderDateDesc(userId);
        return orders.stream()
                .map(this::convertToDto)
                .collect(Collectors.toList());
    }

    /**
     * 全注文一覧取得
     */
    public List<OrderDto> getAllOrders() {
        List<Order> orders = orderRepository.findAll();
        return orders.stream()
                .map(this::convertToDto)
                .collect(Collectors.toList());
    }

    /**
     * フィルター条件で注文検索
     */
    public List<OrderDto> findOrdersByFilters(
            Long userId,
            Long securityId,
            List<OrderStatus> statuses,
            LocalDateTime startDate,
            LocalDateTime endDate) {

        List<Order> orders = orderRepository.findByFilters(
                userId, securityId, statuses, startDate, endDate);

        return orders.stream()
                .map(this::convertToDto)
                .collect(Collectors.toList());
    }

    /**
     * 注文更新
     */
    @Transactional
    public OrderDto updateOrder(Long orderId, UpdateOrderDto dto) {
        log.info("Updating order: {}", orderId);

        Order order = orderRepository.findById(orderId)
                .orElseThrow(() -> new RuntimeException("Order not found: " + orderId));

        // ステータスチェック（約定済み・キャンセル済みは更新不可）
        if (order.getStatus() == OrderStatus.FILLED ||
            order.getStatus() == OrderStatus.CANCELED) {
            throw new RuntimeException("Cannot update order in status: " + order.getStatus());
        }

        // 更新
        if (dto.getQuantity() != null) order.setQuantity(dto.getQuantity());
        if (dto.getPrice() != null) order.setPrice(dto.getPrice());
        if (dto.getStopPrice() != null) order.setStopPrice(dto.getStopPrice());
        if (dto.getTimeInForce() != null) order.setTimeInForce(dto.getTimeInForce());
        if (dto.getValidUntil() != null) order.setValidUntil(dto.getValidUntil());
        if (dto.getNotes() != null) order.setNotes(dto.getNotes());

        Order updatedOrder = orderRepository.save(order);
        log.info("Order updated successfully: {}", updatedOrder.getOrderNo());

        return convertToDto(updatedOrder);
    }

    /**
     * 注文取消
     */
    @Transactional
    public OrderDto cancelOrder(Long orderId) {
        log.info("Canceling order: {}", orderId);

        Order order = orderRepository.findById(orderId)
                .orElseThrow(() -> new RuntimeException("Order not found: " + orderId));

        // ステータスチェック
        if (order.getStatus() == OrderStatus.FILLED) {
            throw new RuntimeException("Cannot cancel filled order");
        }
        if (order.getStatus() == OrderStatus.CANCELED) {
            throw new RuntimeException("Order is already canceled");
        }

        order.setStatus(OrderStatus.CANCELED);
        Order canceledOrder = orderRepository.save(order);
        log.info("Order canceled successfully: {}", canceledOrder.getOrderNo());

        return convertToDto(canceledOrder);
    }

    /**
     * アクティブな注文取得
     */
    public List<OrderDto> getActiveOrders(Long userId) {
        List<Order> orders = orderRepository.findActiveOrdersByUserId(userId);
        return orders.stream()
                .map(this::convertToDto)
                .collect(Collectors.toList());
    }

    // ===== Private Methods =====

    /**
     * 注文番号生成（ORD + YYYYMMDD + 連番）
     */
    private String generateOrderNo() {
        LocalDateTime now = LocalDateTime.now();
        String dateStr = now.format(DateTimeFormatter.ofPattern("yyyyMMdd"));

        LocalDateTime startOfDay = now.toLocalDate().atStartOfDay();
        LocalDateTime endOfDay = now.toLocalDate().atTime(23, 59, 59);
        long todayCount = orderRepository.countByOrderDateBetween(startOfDay, endOfDay);

        return String.format("ORD%s-%04d", dateStr, todayCount + 1);
    }

    /**
     * バリデーション
     */
    private void validateOrder(CreateOrderDto dto) {
        // 指値・逆指値の場合は価格必須
        if (dto.getOrderType() == OrderType.LIMIT && dto.getPrice() == null) {
            throw new IllegalArgumentException("Price is required for LIMIT orders");
        }
        if (dto.getOrderType() == OrderType.STOP && dto.getStopPrice() == null) {
            throw new IllegalArgumentException("Stop price is required for STOP orders");
        }
        if (dto.getOrderType() == OrderType.STOP_LIMIT &&
            (dto.getPrice() == null || dto.getStopPrice() == null)) {
            throw new IllegalArgumentException("Both price and stop price are required for STOP_LIMIT orders");
        }
    }

    /**
     * Entity -> DTO変換
     */
    private OrderDto convertToDto(Order order) {
        OrderDto dto = modelMapper.map(order, OrderDto.class);
        dto.setRemainingQuantity(order.getRemainingQuantity());
        dto.setFillRate(order.getFillRate());
        return dto;
    }
}
