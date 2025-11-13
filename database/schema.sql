-- ================================================================
-- OMS (Order Management System) Database Schema
-- データベース: MySQL 8.0+
-- 文字コード: utf8mb4
-- ================================================================

-- データベース作成（既存の場合は削除して再作成）
DROP DATABASE IF EXISTS oms_db;
CREATE DATABASE oms_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE oms_db;

-- ================================================================
-- 1. ユーザーマスタ (users)
-- ================================================================
CREATE TABLE users (
    user_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'ユーザーID',
    username VARCHAR(50) NOT NULL UNIQUE COMMENT 'ユーザー名',
    email VARCHAR(100) NOT NULL UNIQUE COMMENT 'メールアドレス',
    password_hash VARCHAR(255) NOT NULL COMMENT 'パスワードハッシュ',
    full_name VARCHAR(100) NOT NULL COMMENT '氏名',
    department VARCHAR(50) COMMENT '部署',
    role ENUM('TRADER', 'MANAGER', 'ADMIN') NOT NULL DEFAULT 'TRADER' COMMENT '権限',
    is_active BOOLEAN NOT NULL DEFAULT TRUE COMMENT '有効フラグ',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    INDEX idx_username (username),
    INDEX idx_email (email),
    INDEX idx_is_active (is_active)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='ユーザーマスタ';

-- ================================================================
-- 2. 銘柄マスタ (securities)
-- ================================================================
CREATE TABLE securities (
    security_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '銘柄ID',
    security_code VARCHAR(20) NOT NULL UNIQUE COMMENT '銘柄コード',
    security_name VARCHAR(200) NOT NULL COMMENT '銘柄名',
    security_type ENUM('STOCK', 'BOND', 'ETF', 'REIT', 'FUND', 'DERIVATIVE') NOT NULL COMMENT '銘柄種別',
    market VARCHAR(50) COMMENT '市場（東証プライム、東証スタンダード等）',
    sector VARCHAR(50) COMMENT 'セクター（自動車、金融等）',
    currency VARCHAR(3) NOT NULL DEFAULT 'JPY' COMMENT '通貨',
    lot_size INT NOT NULL DEFAULT 100 COMMENT '売買単位',
    tick_size DECIMAL(10,4) NOT NULL DEFAULT 1.0000 COMMENT '呼値の単位',
    is_active BOOLEAN NOT NULL DEFAULT TRUE COMMENT '取引可能フラグ',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    INDEX idx_security_code (security_code),
    INDEX idx_security_name (security_name),
    INDEX idx_security_type (security_type),
    INDEX idx_market (market),
    INDEX idx_sector (sector),
    INDEX idx_is_active (is_active),
    FULLTEXT INDEX ft_idx_security_name (security_name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='銘柄マスタ';

-- ================================================================
-- 3. 発注テーブル (orders)
-- ================================================================
CREATE TABLE orders (
    order_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '発注ID',
    user_id BIGINT NOT NULL COMMENT 'ユーザーID',
    security_id BIGINT NOT NULL COMMENT '銘柄ID',
    order_no VARCHAR(50) NOT NULL UNIQUE COMMENT '注文番号',
    side ENUM('BUY', 'SELL') NOT NULL COMMENT '売買区分',
    order_type ENUM('MARKET', 'LIMIT', 'STOP', 'STOP_LIMIT') NOT NULL COMMENT '注文タイプ',
    quantity DECIMAL(18,4) NOT NULL COMMENT '数量',
    price DECIMAL(18,4) COMMENT '指値価格',
    stop_price DECIMAL(18,4) COMMENT '逆指値価格',
    time_in_force ENUM('DAY', 'GTC', 'IOC', 'FOK') NOT NULL DEFAULT 'DAY' COMMENT '有効期限',
    status ENUM('NEW', 'PARTIAL', 'FILLED', 'CANCELED', 'REJECTED', 'EXPIRED') NOT NULL DEFAULT 'NEW' COMMENT 'ステータス',
    filled_quantity DECIMAL(18,4) NOT NULL DEFAULT 0 COMMENT '約定済数量',
    average_price DECIMAL(18,4) COMMENT '平均約定価格',
    commission DECIMAL(18,4) COMMENT '手数料',
    order_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '発注日時',
    valid_until DATETIME COMMENT '有効期限日時',
    notes TEXT COMMENT '備考',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (security_id) REFERENCES securities(security_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_user_id (user_id),
    INDEX idx_security_id (security_id),
    INDEX idx_order_no (order_no),
    INDEX idx_status (status),
    INDEX idx_order_date (order_date),
    INDEX idx_side (side),
    INDEX idx_composite (user_id, order_date, status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='発注テーブル';

-- ================================================================
-- 4. 約定テーブル (executions)
-- ================================================================
CREATE TABLE executions (
    execution_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '約定ID',
    order_id BIGINT NOT NULL COMMENT '発注ID',
    security_id BIGINT NOT NULL COMMENT '銘柄ID',
    execution_no VARCHAR(50) NOT NULL UNIQUE COMMENT '約定番号',
    execution_price DECIMAL(18,4) NOT NULL COMMENT '約定価格',
    execution_quantity DECIMAL(18,4) NOT NULL COMMENT '約定数量',
    commission DECIMAL(18,4) NOT NULL DEFAULT 0 COMMENT '手数料',
    execution_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '約定日時',
    settlement_date DATE NOT NULL COMMENT '受渡日',
    contra_broker VARCHAR(100) COMMENT '相手方証券会社',
    notes TEXT COMMENT '備考',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    FOREIGN KEY (order_id) REFERENCES orders(order_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (security_id) REFERENCES securities(security_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_order_id (order_id),
    INDEX idx_security_id (security_id),
    INDEX idx_execution_no (execution_no),
    INDEX idx_execution_date (execution_date),
    INDEX idx_settlement_date (settlement_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='約定テーブル';

-- ================================================================
-- 5. ポジションテーブル (positions)
-- ================================================================
CREATE TABLE positions (
    position_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'ポジションID',
    user_id BIGINT NOT NULL COMMENT 'ユーザーID',
    security_id BIGINT NOT NULL COMMENT '銘柄ID',
    quantity DECIMAL(18,4) NOT NULL DEFAULT 0 COMMENT '保有数量',
    average_cost DECIMAL(18,4) NOT NULL COMMENT '平均取得単価',
    current_price DECIMAL(18,4) COMMENT '現在価格',
    unrealized_pnl DECIMAL(18,4) COMMENT '評価損益',
    realized_pnl DECIMAL(18,4) NOT NULL DEFAULT 0 COMMENT '実現損益',
    last_updated DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '最終更新日時',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (security_id) REFERENCES securities(security_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    UNIQUE KEY uk_user_security (user_id, security_id),
    INDEX idx_user_id (user_id),
    INDEX idx_security_id (security_id),
    INDEX idx_last_updated (last_updated)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='ポジションテーブル';

-- ================================================================
-- 6. 取引履歴テーブル (trade_history)
-- ================================================================
CREATE TABLE trade_history (
    trade_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '取引ID',
    order_id BIGINT NOT NULL COMMENT '発注ID',
    execution_id BIGINT COMMENT '約定ID',
    user_id BIGINT NOT NULL COMMENT 'ユーザーID',
    security_id BIGINT NOT NULL COMMENT '銘柄ID',
    trade_date DATE NOT NULL COMMENT '取引日',
    side ENUM('BUY', 'SELL') NOT NULL COMMENT '売買区分',
    quantity DECIMAL(18,4) NOT NULL COMMENT '数量',
    price DECIMAL(18,4) NOT NULL COMMENT '価格',
    amount DECIMAL(18,4) NOT NULL COMMENT '金額',
    commission DECIMAL(18,4) NOT NULL DEFAULT 0 COMMENT '手数料',
    net_amount DECIMAL(18,4) NOT NULL COMMENT '純額（手数料込み）',
    pnl DECIMAL(18,4) COMMENT '損益',
    notes TEXT COMMENT '備考',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    FOREIGN KEY (order_id) REFERENCES orders(order_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (execution_id) REFERENCES executions(execution_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (security_id) REFERENCES securities(security_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_user_id (user_id),
    INDEX idx_security_id (security_id),
    INDEX idx_trade_date (trade_date),
    INDEX idx_order_id (order_id),
    INDEX idx_composite (user_id, trade_date, side)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='取引履歴テーブル';

-- ================================================================
-- 7. お気に入り銘柄 (favorite_securities)
-- ================================================================
CREATE TABLE favorite_securities (
    favorite_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'お気に入りID',
    user_id BIGINT NOT NULL COMMENT 'ユーザーID',
    security_id BIGINT NOT NULL COMMENT '銘柄ID',
    sort_order INT NOT NULL DEFAULT 0 COMMENT '表示順序',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (security_id) REFERENCES securities(security_id) ON DELETE CASCADE ON UPDATE CASCADE,
    UNIQUE KEY uk_user_security (user_id, security_id),
    INDEX idx_user_id (user_id),
    INDEX idx_sort_order (sort_order)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='お気に入り銘柄';

-- ================================================================
-- 8. 注文履歴ログ (order_audit_log)
-- ================================================================
CREATE TABLE order_audit_log (
    audit_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '監査ログID',
    order_id BIGINT NOT NULL COMMENT '発注ID',
    user_id BIGINT NOT NULL COMMENT 'ユーザーID',
    action ENUM('CREATE', 'UPDATE', 'CANCEL', 'FILL', 'PARTIAL_FILL', 'REJECT', 'EXPIRE') NOT NULL COMMENT 'アクション',
    old_status VARCHAR(50) COMMENT '変更前ステータス',
    new_status VARCHAR(50) COMMENT '変更後ステータス',
    change_details JSON COMMENT '変更詳細（JSON形式）',
    ip_address VARCHAR(45) COMMENT 'IPアドレス',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    FOREIGN KEY (order_id) REFERENCES orders(order_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_order_id (order_id),
    INDEX idx_user_id (user_id),
    INDEX idx_action (action),
    INDEX idx_created_at (created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='注文履歴ログ';

-- ================================================================
-- 9. 検索条件プリセット (search_presets)
-- ================================================================
CREATE TABLE search_presets (
    preset_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'プリセットID',
    user_id BIGINT NOT NULL COMMENT 'ユーザーID',
    preset_name VARCHAR(100) NOT NULL COMMENT 'プリセット名',
    search_criteria JSON NOT NULL COMMENT '検索条件（JSON形式）',
    is_default BOOLEAN NOT NULL DEFAULT FALSE COMMENT 'デフォルト設定フラグ',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE,
    INDEX idx_user_id (user_id),
    INDEX idx_is_default (is_default)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='検索条件プリセット';

-- ================================================================
-- 10. ユーザー設定 (user_settings)
-- ================================================================
CREATE TABLE user_settings (
    setting_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '設定ID',
    user_id BIGINT NOT NULL UNIQUE COMMENT 'ユーザーID',
    theme VARCHAR(20) NOT NULL DEFAULT 'light' COMMENT 'テーマ（light/dark）',
    language VARCHAR(10) NOT NULL DEFAULT 'ja' COMMENT '言語',
    default_market VARCHAR(50) COMMENT 'デフォルト市場',
    notification_enabled BOOLEAN NOT NULL DEFAULT TRUE COMMENT '通知有効化',
    sound_enabled BOOLEAN NOT NULL DEFAULT TRUE COMMENT '音声有効化',
    grid_page_size INT NOT NULL DEFAULT 50 COMMENT 'グリッドページサイズ',
    auto_refresh_interval INT NOT NULL DEFAULT 30 COMMENT '自動更新間隔（秒）',
    settings_json JSON COMMENT 'その他設定（JSON形式）',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新日時',
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='ユーザー設定';

-- ================================================================
-- 11. 市場データ（価格履歴） (market_data)
-- ================================================================
CREATE TABLE market_data (
    market_data_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '市場データID',
    security_id BIGINT NOT NULL COMMENT '銘柄ID',
    data_date DATE NOT NULL COMMENT '日付',
    open_price DECIMAL(18,4) COMMENT '始値',
    high_price DECIMAL(18,4) COMMENT '高値',
    low_price DECIMAL(18,4) COMMENT '安値',
    close_price DECIMAL(18,4) COMMENT '終値',
    volume BIGINT COMMENT '出来高',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    FOREIGN KEY (security_id) REFERENCES securities(security_id) ON DELETE CASCADE ON UPDATE CASCADE,
    UNIQUE KEY uk_security_date (security_id, data_date),
    INDEX idx_security_id (security_id),
    INDEX idx_data_date (data_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='市場データ（価格履歴）';

-- ================================================================
-- 12. 通知履歴 (notifications)
-- ================================================================
CREATE TABLE notifications (
    notification_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '通知ID',
    user_id BIGINT NOT NULL COMMENT 'ユーザーID',
    notification_type ENUM('ORDER', 'EXECUTION', 'SYSTEM', 'ALERT') NOT NULL COMMENT '通知タイプ',
    title VARCHAR(200) NOT NULL COMMENT 'タイトル',
    message TEXT NOT NULL COMMENT 'メッセージ',
    severity ENUM('INFO', 'WARNING', 'ERROR', 'SUCCESS') NOT NULL DEFAULT 'INFO' COMMENT '重要度',
    is_read BOOLEAN NOT NULL DEFAULT FALSE COMMENT '既読フラグ',
    related_order_id BIGINT COMMENT '関連注文ID',
    related_execution_id BIGINT COMMENT '関連約定ID',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (related_order_id) REFERENCES orders(order_id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (related_execution_id) REFERENCES executions(execution_id) ON DELETE SET NULL ON UPDATE CASCADE,
    INDEX idx_user_id (user_id),
    INDEX idx_notification_type (notification_type),
    INDEX idx_is_read (is_read),
    INDEX idx_created_at (created_at),
    INDEX idx_composite (user_id, is_read, created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='通知履歴';

-- ================================================================
-- トリガー: 約定時のポジション更新
-- ================================================================
DELIMITER $$

CREATE TRIGGER trg_after_execution_insert
AFTER INSERT ON executions
FOR EACH ROW
BEGIN
    DECLARE v_user_id BIGINT;
    DECLARE v_side ENUM('BUY', 'SELL');
    DECLARE v_current_qty DECIMAL(18,4);
    DECLARE v_current_avg_cost DECIMAL(18,4);
    DECLARE v_new_qty DECIMAL(18,4);
    DECLARE v_new_avg_cost DECIMAL(18,4);

    -- 注文情報取得
    SELECT user_id, side INTO v_user_id, v_side
    FROM orders WHERE order_id = NEW.order_id;

    -- 既存ポジション取得
    SELECT quantity, average_cost INTO v_current_qty, v_current_avg_cost
    FROM positions
    WHERE user_id = v_user_id AND security_id = NEW.security_id;

    -- ポジション計算
    IF v_side = 'BUY' THEN
        SET v_new_qty = COALESCE(v_current_qty, 0) + NEW.execution_quantity;
        SET v_new_avg_cost = ((COALESCE(v_current_qty, 0) * COALESCE(v_current_avg_cost, 0)) +
                               (NEW.execution_quantity * NEW.execution_price)) / v_new_qty;
    ELSE -- SELL
        SET v_new_qty = COALESCE(v_current_qty, 0) - NEW.execution_quantity;
        SET v_new_avg_cost = COALESCE(v_current_avg_cost, 0);
    END IF;

    -- ポジション更新または挿入
    INSERT INTO positions (user_id, security_id, quantity, average_cost)
    VALUES (v_user_id, NEW.security_id, v_new_qty, v_new_avg_cost)
    ON DUPLICATE KEY UPDATE
        quantity = v_new_qty,
        average_cost = v_new_avg_cost,
        updated_at = CURRENT_TIMESTAMP;
END$$

DELIMITER ;

-- ================================================================
-- ビュー: 発注詳細ビュー
-- ================================================================
CREATE VIEW v_order_details AS
SELECT
    o.order_id,
    o.order_no,
    u.user_id,
    u.username,
    u.full_name,
    s.security_id,
    s.security_code,
    s.security_name,
    s.security_type,
    s.market,
    o.side,
    o.order_type,
    o.quantity,
    o.price,
    o.stop_price,
    o.time_in_force,
    o.status,
    o.filled_quantity,
    o.average_price,
    o.commission,
    o.order_date,
    o.valid_until,
    o.notes,
    o.created_at,
    o.updated_at
FROM orders o
INNER JOIN users u ON o.user_id = u.user_id
INNER JOIN securities s ON o.security_id = s.security_id;

-- ================================================================
-- ビュー: 約定詳細ビュー
-- ================================================================
CREATE VIEW v_execution_details AS
SELECT
    e.execution_id,
    e.execution_no,
    e.order_id,
    o.order_no,
    u.user_id,
    u.username,
    s.security_id,
    s.security_code,
    s.security_name,
    o.side,
    e.execution_price,
    e.execution_quantity,
    e.commission,
    e.execution_date,
    e.settlement_date,
    e.contra_broker,
    e.notes,
    e.created_at
FROM executions e
INNER JOIN orders o ON e.order_id = o.order_id
INNER JOIN users u ON o.user_id = u.user_id
INNER JOIN securities s ON e.security_id = s.security_id;

-- ================================================================
-- ビュー: ポジションサマリービュー
-- ================================================================
CREATE VIEW v_position_summary AS
SELECT
    p.position_id,
    p.user_id,
    u.username,
    u.full_name,
    p.security_id,
    s.security_code,
    s.security_name,
    s.security_type,
    s.market,
    s.sector,
    p.quantity,
    p.average_cost,
    p.current_price,
    p.unrealized_pnl,
    p.realized_pnl,
    (p.quantity * p.current_price) AS market_value,
    p.last_updated,
    p.created_at,
    p.updated_at
FROM positions p
INNER JOIN users u ON p.user_id = u.user_id
INNER JOIN securities s ON p.security_id = s.security_id
WHERE p.quantity > 0;

-- ================================================================
-- インデックス最適化のためのコメント
-- ================================================================
-- パフォーマンスチューニング用の複合インデックスは、実際のクエリパターンに応じて追加
-- 例: CREATE INDEX idx_orders_user_date ON orders(user_id, order_date DESC);
