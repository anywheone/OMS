package com.oms.api.repository;

import com.oms.api.model.Order;
import com.oms.api.model.OrderStatus;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

/**
 * 発注リポジトリ
 */
@Repository
public interface OrderRepository extends JpaRepository<Order, Long> {

    /**
     * 注文番号で検索
     */
    Optional<Order> findByOrderNo(String orderNo);

    /**
     * ユーザーIDで検索（注文日降順）
     */
    List<Order> findByUserIdOrderByOrderDateDesc(Long userId);

    /**
     * ユーザーID + ステータスで検索
     */
    List<Order> findByUserIdAndStatusInOrderByOrderDateDesc(Long userId, List<OrderStatus> statuses);

    /**
     * ユーザーID + 日付範囲で検索
     */
    @Query("SELECT o FROM Order o WHERE o.userId = :userId " +
           "AND o.orderDate BETWEEN :startDate AND :endDate " +
           "ORDER BY o.orderDate DESC")
    List<Order> findByUserIdAndDateRange(
            @Param("userId") Long userId,
            @Param("startDate") LocalDateTime startDate,
            @Param("endDate") LocalDateTime endDate);

    /**
     * 高度なフィルター検索
     */
    @Query("SELECT o FROM Order o WHERE " +
           "(:userId IS NULL OR o.userId = :userId) AND " +
           "(:securityId IS NULL OR o.securityId = :securityId) AND " +
           "(:statuses IS NULL OR o.status IN :statuses) AND " +
           "(:startDate IS NULL OR o.orderDate >= :startDate) AND " +
           "(:endDate IS NULL OR o.orderDate <= :endDate) " +
           "ORDER BY o.orderDate DESC")
    List<Order> findByFilters(
            @Param("userId") Long userId,
            @Param("securityId") Long securityId,
            @Param("statuses") List<OrderStatus> statuses,
            @Param("startDate") LocalDateTime startDate,
            @Param("endDate") LocalDateTime endDate);

    /**
     * アクティブな注文を取得
     */
    @Query("SELECT o FROM Order o WHERE o.userId = :userId " +
           "AND o.status IN ('NEW', 'PARTIAL') " +
           "ORDER BY o.orderDate DESC")
    List<Order> findActiveOrdersByUserId(@Param("userId") Long userId);

    /**
     * 次の注文番号を生成するためのカウント
     */
    long countByOrderDateBetween(LocalDateTime start, LocalDateTime end);
}
