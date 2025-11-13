# OMS (Order Management System)

資産運用会社向けの本格的な発注管理システム

## 🎯 プロジェクトの状態

このプロジェクトは**実務で即使えるスターターキット**として設計されています。

### ✅ 完成済みコンポーネント

1. **設計ドキュメント**
   - PlantUML ER図、クラス図、シーケンス図（完全版）
   - 実装ガイド（`docs/IMPLEMENTATION_GUIDE.md`）

2. **データベース**
   - 完全なMySQLスキーマ（12テーブル + ビュー + トリガー）
   - リアルなサンプルデータ

3. **.NET 8 WPF クライアント**
   - Prismフレームワーク設定
   - Enums、Models、DTOs、Interfaces（完全版）
   - テーマとスタイル
   - 完全実装済みUserControl:
     - ✅ `NumericUpDownControl` - 数値入力（上下ボタン付き）
     - ✅ `CurrencyDisplayControl` - 通貨表示（色分け対応）

4. **Spring Boot 3 バックエンドAPI**
   - Order機能（完全実装）:
     - Entity、Repository、Service、Controller
     - REST API（発注作成・更新・取消・検索）
   - Swagger UI統合
   - ModelMapper設定

### 🚧 実装テンプレート提供

以下のコンポーネントは実装パターンとサンプルコードを提供：

- 残り26個のUserControl（実装ガイド付き）
- 他のSpring Boot API（Execution、Security、Position等）
- AvalonDockレイアウト
- ReoGridグリッド実装

詳細は **`docs/IMPLEMENTATION_GUIDE.md`** を参照してください。

## 技術スタック

### フロントエンド
- **フレームワーク**: WPF (C#) + .NET 8
- **MVVMフレームワーク**: Prism
- **UIコンポーネント**:
  - AvalonDock (ドッキングレイアウト)
  - unvell.ReoGrid (高機能グリッド)
- **設計パターン**: MVVM, Dependency Injection, Repository Pattern

### バックエンド
- **フレームワーク**: Spring Boot 3.x (Java 17+)
- **API**: RESTful API
- **データベース**: MySQL 8.0+
- **ORM**: Spring Data JPA

### ドキュメント
- **設計図**: PlantUML
- **外部設計書**: Markdown + Excel

## プロジェクト構造

```
OMS/
├── docs/                          # ドキュメント
│   ├── plantuml/                  # PlantUML設計図
│   │   ├── class-diagrams/        # クラス図
│   │   ├── sequence-diagrams/     # シーケンス図
│   │   └── database-schema/       # データベーススキーマ図
│   └── external-design/           # 外部設計書
│
├── src/                           # WPFクライアントソースコード
│   ├── OMS.Client/                # メインクライアントアプリケーション
│   │   ├── Controls/              # 再利用可能UserControls
│   │   │   ├── Orders/            # 発注関連コントロール
│   │   │   ├── Grids/             # グリッドコントロール
│   │   │   ├── Portfolio/         # ポートフォリオコントロール
│   │   │   ├── Search/            # 検索・フィルターコントロール
│   │   │   ├── Status/            # ステータス・通知コントロール
│   │   │   ├── Details/           # 詳細表示コントロール
│   │   │   └── Utilities/         # ユーティリティコントロール
│   │   ├── ViewModels/            # ViewModels
│   │   ├── Views/                 # Views (Window/Page)
│   │   ├── Services/              # アプリケーションサービス
│   │   ├── Converters/            # ValueConverters
│   │   ├── Behaviors/             # Attachedビヘイビア
│   │   ├── Themes/                # テーマ・スタイル
│   │   └── App.xaml               # アプリケーションエントリーポイント
│   │
│   ├── OMS.Core/                  # コアライブラリ
│   │   ├── Models/                # ドメインモデル
│   │   ├── DTOs/                  # データ転送オブジェクト
│   │   ├── Interfaces/            # インターフェース
│   │   ├── Enums/                 # 列挙型
│   │   └── Constants/             # 定数
│   │
│   └── OMS.Infrastructure/        # インフラストラクチャ層
│       ├── Services/              # API通信サービス
│       ├── Repositories/          # リポジトリ実装
│       └── Http/                  # HTTP クライアント
│
├── backend/                       # Spring Boot バックエンド
│   └── oms-api/                   # API アプリケーション
│       ├── src/main/java/com/oms/api/
│       │   ├── controller/        # REST Controllers
│       │   ├── service/           # ビジネスロジック
│       │   ├── repository/        # JPA Repositories
│       │   ├── model/             # JPA Entities
│       │   ├── dto/               # DTOs
│       │   └── config/            # 設定クラス
│       ├── src/main/resources/
│       │   ├── application.properties
│       │   └── db/migration/      # Flyway マイグレーション
│       └── pom.xml
│
├── database/                      # データベース関連
│   ├── schema.sql                 # DDL (テーブル定義)
│   ├── seed-data.sql              # サンプルデータ
│   └── migrations/                # マイグレーションスクリプト
│
└── tests/                         # テストプロジェクト
    ├── OMS.Client.Tests/
    └── OMS.Core.Tests/
```

## 主要な UserControl 一覧

### 1. ユーティリティコントロール (Utilities/)
- `NumericUpDownControl` - 数値入力（上下ボタン付き）
- `DateRangePickerControl` - 日付範囲選択
- `ValidationSummaryControl` - バリデーションサマリー
- `LoadingOverlay` - ローディングオーバーレイ
- `ConfirmDialogControl` - 確認ダイアログ

### 2. データ表示ヘルパー (Utilities/)
- `CurrencyDisplayControl` - 通貨表示
- `PercentageDisplayControl` - パーセンテージ表示
- `StatusBadgeControl` - ステータスバッジ

### 3. 発注関連コントロール (Orders/)
- `OrderEntryControl` - 発注入力フォーム
- `QuickOrderPanel` - クイック発注パネル
- `OrderValidationControl` - 発注前確認ダイアログ

### 4. グリッドコントロール (Grids/)
- `OrderListGrid` - 発注一覧グリッド
- `ExecutionListGrid` - 約定結果一覧グリッド
- `TradeHistoryGrid` - 取引履歴グリッド

### 5. 検索・フィルターコントロール (Search/)
- `SecuritySearchControl` - 銘柄検索パネル
- `OrderFilterPanel` - 発注フィルターパネル
- `AdvancedSearchControl` - 高度な検索パネル

### 6. ポートフォリオ表示コントロール (Portfolio/)
- `PortfolioSummaryControl` - ポートフォリオサマリー
- `PositionListControl` - 保有ポジション一覧
- `PortfolioChartControl` - ポートフォリオチャート

### 7. ステータス・通知コントロール (Status/)
- `OrderStatusControl` - 注文ステータス表示
- `NotificationPanel` - 通知パネル
- `MarketStatusBar` - マーケット情報バー

### 8. 詳細表示コントロール (Details/)
- `OrderDetailControl` - 発注詳細パネル
- `SecurityDetailControl` - 銘柄詳細パネル
- `ExecutionDetailControl` - 約定詳細パネル

## データベース設計

### 主要テーブル
- `orders` - 発注テーブル
- `executions` - 約定テーブル
- `securities` - 銘柄マスタ
- `positions` - ポジションテーブル
- `trade_history` - 取引履歴
- `users` - ユーザーマスタ

## REST API エンドポイント

### 発注管理
- `POST /api/orders` - 新規発注
- `GET /api/orders` - 発注一覧取得
- `GET /api/orders/{id}` - 発注詳細取得
- `PUT /api/orders/{id}` - 発注修正
- `DELETE /api/orders/{id}` - 発注取消

### 約定管理
- `GET /api/executions` - 約定一覧取得
- `GET /api/executions/{id}` - 約定詳細取得

### ポートフォリオ
- `GET /api/positions` - ポジション一覧取得
- `GET /api/portfolio/summary` - ポートフォリオサマリー取得

### 銘柄検索
- `GET /api/securities/search` - 銘柄検索
- `GET /api/securities/{code}` - 銘柄詳細取得

### 取引履歴
- `GET /api/trades/history` - 取引履歴取得

## セットアップ手順

### 前提条件
- .NET 8 SDK
- Java 17以上
- MySQL 8.0以上
- Visual Studio 2022 以上 または Rider
- Maven 3.6以上

### 1. データベースセットアップ

```bash
# MySQLにログイン
mysql -u root -p

# スキーマ適用（データベース作成を含む）
mysql -u root -p < database/schema.sql

# サンプルデータ投入
mysql -u root -p < database/seed-data.sql
```

### 2. バックエンドセットアップ

```bash
cd backend/oms-api

# application.propertiesを編集してDB接続情報を設定
# spring.datasource.password=your_password_here

# ビルドと起動
mvn clean install
mvn spring-boot:run

# 確認: http://localhost:8080/swagger-ui.html
```

### 3. フロントエンドセットアップ

```bash
cd src/OMS.Client

# appsettings.jsonを確認（必要に応じて編集）

# パッケージ復元・ビルド・起動
dotnet restore
dotnet build
dotnet run
```

### 4. 動作確認

1. バックエンドAPI: http://localhost:8080/swagger-ui.html
2. WPFアプリケーション: 自動起動
3. テストユーザー: username=`trader001`, password=`password123`

## 開発ガイドライン

### UserControl開発の原則
1. **独立性**: 各コントロールは独立して動作可能
2. **再利用性**: 依存性注入でViewModelを設定
3. **柔軟性**: DependencyPropertyで柔軟にカスタマイズ可能
4. **通信**: イベントとコマンドで親コントロールと通信
5. **バインディング**: MVVMパターンに従ったデータバインディング

### コーディング規約
- C#: Microsoft C# Coding Conventions に準拠
- Java: Google Java Style Guide に準拠
- XAML: インデント4スペース、属性は1行に1つ推奨

## ライセンス
Proprietary - 社内使用のみ

## 作成者
Asset Management Company Development Team
