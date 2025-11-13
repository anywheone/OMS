-- ================================================================
-- OMS サンプルデータ (Seed Data)
-- ================================================================
USE oms_db;

-- データ削除（テスト用）
SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE notifications;
TRUNCATE TABLE order_audit_log;
TRUNCATE TABLE trade_history;
TRUNCATE TABLE executions;
TRUNCATE TABLE orders;
TRUNCATE TABLE positions;
TRUNCATE TABLE favorite_securities;
TRUNCATE TABLE search_presets;
TRUNCATE TABLE user_settings;
TRUNCATE TABLE market_data;
TRUNCATE TABLE securities;
TRUNCATE TABLE users;
SET FOREIGN_KEY_CHECKS = 1;

-- ================================================================
-- 1. ユーザーマスタ
-- ================================================================
INSERT INTO users (username, email, password_hash, full_name, department, role, is_active) VALUES
('trader001', 'trader001@example.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', '山田 太郎', '株式運用部', 'TRADER', TRUE),
('trader002', 'trader002@example.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', '佐藤 花子', '株式運用部', 'TRADER', TRUE),
('trader003', 'trader003@example.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', '鈴木 一郎', '債券運用部', 'TRADER', TRUE),
('manager001', 'manager001@example.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', '田中 次郎', '運用部', 'MANAGER', TRUE),
('admin001', 'admin@example.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'システム管理者', 'IT部', 'ADMIN', TRUE);
-- パスワード: すべて "password123"

-- ================================================================
-- 2. 銘柄マスタ（日本株）
-- ================================================================
INSERT INTO securities (security_code, security_name, security_type, market, sector, currency, lot_size, tick_size, is_active) VALUES
-- 自動車
('7203', 'トヨタ自動車', 'STOCK', '東証プライム', '自動車', 'JPY', 100, 1.0000, TRUE),
('7201', '日産自動車', 'STOCK', '東証プライム', '自動車', 'JPY', 100, 1.0000, TRUE),
('7267', '本田技研工業', 'STOCK', '東証プライム', '自動車', 'JPY', 100, 1.0000, TRUE),

-- 電機
('6758', 'ソニーグループ', 'STOCK', '東証プライム', '電機', 'JPY', 100, 1.0000, TRUE),
('6752', 'パナソニック ホールディングス', 'STOCK', '東証プライム', '電機', 'JPY', 100, 1.0000, TRUE),
('6501', '日立製作所', 'STOCK', '東証プライム', '電機', 'JPY', 100, 1.0000, TRUE),

-- 金融
('8306', '三菱UFJフィナンシャル・グループ', 'STOCK', '東証プライム', '金融', 'JPY', 100, 0.5000, TRUE),
('8316', '三井住友フィナンシャルグループ', 'STOCK', '東証プライム', '金融', 'JPY', 100, 1.0000, TRUE),
('8411', 'みずほフィナンシャルグループ', 'STOCK', '東証プライム', '金融', 'JPY', 100, 0.5000, TRUE),

-- 通信
('9432', '日本電信電話', 'STOCK', '東証プライム', '通信', 'JPY', 100, 0.5000, TRUE),
('9433', 'KDDI', 'STOCK', '東証プライム', '通信', 'JPY', 100, 1.0000, TRUE),
('9984', 'ソフトバンクグループ', 'STOCK', '東証プライム', '通信', 'JPY', 100, 1.0000, TRUE),

-- 小売
('9983', 'ファーストリテイリング', 'STOCK', '東証プライム', '小売', 'JPY', 100, 10.0000, TRUE),
('3382', 'セブン&アイ・ホールディングス', 'STOCK', '東証プライム', '小売', 'JPY', 100, 1.0000, TRUE),

-- 製薬
('4502', '武田薬品工業', 'STOCK', '東証プライム', '医薬品', 'JPY', 100, 1.0000, TRUE),
('4503', 'アステラス製薬', 'STOCK', '東証プライム', '医薬品', 'JPY', 100, 1.0000, TRUE),

-- ETF
('1306', 'TOPIX連動型上場投資信託', 'ETF', '東証', 'インデックス', 'JPY', 10, 0.5000, TRUE),
('1321', '日経225連動型上場投資信託', 'ETF', '東証', 'インデックス', 'JPY', 1, 1.0000, TRUE),

-- REIT
('8951', '日本ビルファンド投資法人', 'REIT', '東証', '不動産', 'JPY', 1, 100.0000, TRUE),
('8952', 'ジャパンリアルエステイト投資法人', 'REIT', '東証', '不動産', 'JPY', 1, 100.0000, TRUE);

-- ================================================================
-- 3. 市場データ（直近10営業日分）
-- ================================================================
INSERT INTO market_data (security_id, data_date, open_price, high_price, low_price, close_price, volume) VALUES
-- トヨタ自動車 (security_id=1)
(1, '2024-01-22', 2450.00, 2480.00, 2445.00, 2470.00, 15234500),
(1, '2024-01-23', 2475.00, 2490.00, 2460.00, 2485.00, 16453200),
(1, '2024-01-24', 2480.00, 2510.00, 2475.00, 2500.00, 18765400),
(1, '2024-01-25', 2505.00, 2520.00, 2490.00, 2495.00, 14523600),
(1, '2024-01-26', 2490.00, 2505.00, 2480.00, 2500.00, 13245700),

-- ソニーグループ (security_id=4)
(4, '2024-01-22', 12500.00, 12650.00, 12480.00, 12600.00, 3456700),
(4, '2024-01-23', 12620.00, 12700.00, 12580.00, 12680.00, 3654200),
(4, '2024-01-24', 12670.00, 12750.00, 12650.00, 12720.00, 3823400),
(4, '2024-01-25', 12730.00, 12800.00, 12700.00, 12750.00, 3234500),
(4, '2024-01-26', 12740.00, 12780.00, 12710.00, 12760.00, 2987600);

-- ================================================================
-- 4. ユーザー設定
-- ================================================================
INSERT INTO user_settings (user_id, theme, language, default_market, notification_enabled, sound_enabled, grid_page_size, auto_refresh_interval) VALUES
(1, 'light', 'ja', '東証プライム', TRUE, TRUE, 50, 30),
(2, 'dark', 'ja', '東証プライム', TRUE, FALSE, 100, 60),
(3, 'light', 'ja', '東証プライム', TRUE, TRUE, 50, 30),
(4, 'light', 'ja', '東証プライム', TRUE, TRUE, 100, 30),
(5, 'dark', 'en', NULL, TRUE, FALSE, 50, 30);

-- ================================================================
-- 5. お気に入り銘柄
-- ================================================================
INSERT INTO favorite_securities (user_id, security_id, sort_order) VALUES
(1, 1, 1),  -- トヨタ自動車
(1, 4, 2),  -- ソニーグループ
(1, 7, 3),  -- 三菱UFJ
(1, 10, 4), -- NTT
(2, 1, 1),  -- トヨタ自動車
(2, 13, 2), -- ファーストリテイリング
(2, 15, 3); -- 武田薬品

-- ================================================================
-- 6. 発注データ（サンプル）
-- ================================================================
INSERT INTO orders (user_id, security_id, order_no, side, order_type, quantity, price, time_in_force, status, filled_quantity, average_price, order_date, valid_until) VALUES
-- 約定済み
(1, 1, 'ORD20240126-0001', 'BUY', 'LIMIT', 1000, 2500.00, 'DAY', 'FILLED', 1000, 2498.50, '2024-01-26 09:15:23', '2024-01-26 15:00:00'),
(1, 4, 'ORD20240126-0002', 'BUY', 'MARKET', 100, NULL, 'DAY', 'FILLED', 100, 12750.00, '2024-01-26 10:30:45', '2024-01-26 15:00:00'),
(2, 1, 'ORD20240126-0003', 'SELL', 'LIMIT', 500, 2505.00, 'DAY', 'FILLED', 500, 2505.00, '2024-01-26 11:20:15', '2024-01-26 15:00:00'),

-- 部分約定
(1, 7, 'ORD20240126-0004', 'BUY', 'LIMIT', 2000, 1250.00, 'GTC', 'PARTIAL', 1200, 1249.50, '2024-01-26 13:45:30', NULL),

-- 新規（未約定）
(1, 10, 'ORD20240126-0005', 'BUY', 'LIMIT', 500, 180.00, 'DAY', 'NEW', 0, NULL, '2024-01-26 14:10:00', '2024-01-26 15:00:00'),
(2, 13, 'ORD20240126-0006', 'BUY', 'LIMIT', 10, 42000.00, 'DAY', 'NEW', 0, NULL, '2024-01-26 14:25:00', '2024-01-26 15:00:00'),
(3, 15, 'ORD20240126-0007', 'SELL', 'LIMIT', 100, 4200.00, 'DAY', 'NEW', 0, NULL, '2024-01-26 14:35:00', '2024-01-26 15:00:00'),

-- キャンセル済み
(1, 4, 'ORD20240125-0001', 'BUY', 'LIMIT', 50, 12500.00, 'DAY', 'CANCELED', 0, NULL, '2024-01-25 10:00:00', '2024-01-25 15:00:00');

-- ================================================================
-- 7. 約定データ
-- ================================================================
INSERT INTO executions (order_id, security_id, execution_no, execution_price, execution_quantity, commission, execution_date, settlement_date) VALUES
-- ORD20240126-0001の約定（2回に分けて約定）
(1, 1, 'EXE20240126-0001', 2498.00, 500, 150.00, '2024-01-26 09:15:25', '2024-01-29'),
(1, 1, 'EXE20240126-0002', 2499.00, 500, 150.00, '2024-01-26 09:16:10', '2024-01-29'),

-- ORD20240126-0002の約定（一括約定）
(2, 4, 'EXE20240126-0003', 12750.00, 100, 380.00, '2024-01-26 10:30:47', '2024-01-29'),

-- ORD20240126-0003の約定（一括約定）
(3, 1, 'EXE20240126-0004', 2505.00, 500, 150.00, '2024-01-26 11:20:18', '2024-01-29'),

-- ORD20240126-0004の約定（部分約定、3回に分けて約定）
(4, 7, 'EXE20240126-0005', 1249.00, 500, 75.00, '2024-01-26 13:45:35', '2024-01-29'),
(4, 7, 'EXE20240126-0006', 1250.00, 400, 60.00, '2024-01-26 14:02:12', '2024-01-29'),
(4, 7, 'EXE20240126-0007', 1249.50, 300, 45.00, '2024-01-26 14:30:45', '2024-01-29');

-- ================================================================
-- 8. 取引履歴
-- ================================================================
INSERT INTO trade_history (order_id, execution_id, user_id, security_id, trade_date, side, quantity, price, amount, commission, net_amount, pnl) VALUES
(1, 1, 1, 1, '2024-01-26', 'BUY', 500, 2498.00, 1249000.00, 150.00, 1249150.00, NULL),
(1, 2, 1, 1, '2024-01-26', 'BUY', 500, 2499.00, 1249500.00, 150.00, 1249650.00, NULL),
(2, 3, 1, 4, '2024-01-26', 'BUY', 100, 12750.00, 1275000.00, 380.00, 1275380.00, NULL),
(3, 4, 2, 1, '2024-01-26', 'SELL', 500, 2505.00, 1252500.00, 150.00, 1252350.00, 3500.00),
(4, 5, 1, 7, '2024-01-26', 'BUY', 500, 1249.00, 624500.00, 75.00, 624575.00, NULL),
(4, 6, 1, 7, '2024-01-26', 'BUY', 400, 1250.00, 500000.00, 60.00, 500060.00, NULL),
(4, 7, 1, 7, '2024-01-26', 'BUY', 300, 1249.50, 374850.00, 45.00, 374895.00, NULL);

-- ================================================================
-- 9. ポジション（トリガーにより自動計算されるが、初期データとして手動投入）
-- ================================================================
INSERT INTO positions (user_id, security_id, quantity, average_cost, current_price, unrealized_pnl, realized_pnl) VALUES
(1, 1, 1000, 2498.50, 2500.00, 1500.00, 0.00),    -- トヨタ: +1,500円
(1, 4, 100, 12750.00, 12760.00, 1000.00, 0.00),   -- ソニー: +1,000円
(1, 7, 1200, 1249.50, 1250.00, 600.00, 0.00),     -- 三菱UFJ: +600円
(2, 1, -500, 2505.00, 2500.00, 2500.00, 3500.00); -- トヨタ売建: +2,500円（含み益） + 3,500円（実現益）

-- ================================================================
-- 10. 監査ログ
-- ================================================================
INSERT INTO order_audit_log (order_id, user_id, action, old_status, new_status, change_details, ip_address) VALUES
(1, 1, 'CREATE', NULL, 'NEW', '{"quantity": 1000, "price": 2500.00}', '192.168.1.100'),
(1, 1, 'PARTIAL_FILL', 'NEW', 'PARTIAL', '{"filled_quantity": 500, "execution_price": 2498.00}', '192.168.1.100'),
(1, 1, 'FILL', 'PARTIAL', 'FILLED', '{"filled_quantity": 1000, "average_price": 2498.50}', '192.168.1.100'),
(2, 1, 'CREATE', NULL, 'NEW', '{"quantity": 100, "order_type": "MARKET"}', '192.168.1.100'),
(2, 1, 'FILL', 'NEW', 'FILLED', '{"filled_quantity": 100, "average_price": 12750.00}', '192.168.1.100'),
(3, 2, 'CREATE', NULL, 'NEW', '{"quantity": 500, "price": 2505.00}', '192.168.1.105'),
(3, 2, 'FILL', 'NEW', 'FILLED', '{"filled_quantity": 500, "average_price": 2505.00}', '192.168.1.105'),
(8, 1, 'CREATE', NULL, 'NEW', '{"quantity": 50, "price": 12500.00}', '192.168.1.100'),
(8, 1, 'CANCEL', 'NEW', 'CANCELED', '{"reason": "user_cancelled"}', '192.168.1.100');

-- ================================================================
-- 11. 通知履歴
-- ================================================================
INSERT INTO notifications (user_id, notification_type, title, message, severity, is_read, related_order_id, related_execution_id) VALUES
(1, 'ORDER', '発注完了', '注文番号: ORD20240126-0001\n銘柄: トヨタ自動車\n数量: 1,000株', 'SUCCESS', TRUE, 1, NULL),
(1, 'EXECUTION', '約定通知', '銘柄: トヨタ自動車\n数量: 500株\n価格: ¥2,498', 'SUCCESS', TRUE, 1, 1),
(1, 'EXECUTION', '約定通知（全約定完了）', '銘柄: トヨタ自動車\n数量: 500株\n価格: ¥2,499\n全数量が約定しました。', 'SUCCESS', TRUE, 1, 2),
(1, 'EXECUTION', '約定通知', '銘柄: ソニーグループ\n数量: 100株\n価格: ¥12,750', 'SUCCESS', FALSE, 2, 3),
(2, 'EXECUTION', '約定通知', '銘柄: トヨタ自動車\n数量: 500株\n価格: ¥2,505', 'SUCCESS', TRUE, 3, 4),
(1, 'EXECUTION', '部分約定通知', '銘柄: 三菱UFJ\n数量: 500株 / 2,000株\n価格: ¥1,249', 'INFO', FALSE, 4, 5),
(1, 'ORDER', '注文取消完了', '注文番号: ORD20240125-0001\n銘柄: ソニーグループ', 'INFO', TRUE, 8, NULL),
(1, 'SYSTEM', 'マーケットクローズ', '本日の取引が終了しました。', 'INFO', TRUE, NULL, NULL);

-- ================================================================
-- 12. 検索条件プリセット
-- ================================================================
INSERT INTO search_presets (user_id, preset_name, search_criteria, is_default) VALUES
(1, '本日の注文', '{"dateRange": {"start": "today", "end": "today"}, "status": ["NEW", "PARTIAL", "FILLED"]}', TRUE),
(1, '未約定注文', '{"status": ["NEW", "PARTIAL"], "sortBy": "order_date", "sortOrder": "DESC"}', FALSE),
(1, 'トヨタ関連注文', '{"securityCode": "7203", "dateRange": {"start": "-30d", "end": "today"}}', FALSE),
(2, '今週の約定済み', '{"dateRange": {"start": "thisWeek", "end": "today"}, "status": ["FILLED"]}', TRUE);

-- ================================================================
-- データ確認用クエリ（コメント）
-- ================================================================
-- SELECT * FROM v_order_details;
-- SELECT * FROM v_execution_details;
-- SELECT * FROM v_position_summary;
-- SELECT COUNT(*) AS order_count, status FROM orders GROUP BY status;
