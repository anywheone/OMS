# OMS クイックスタートガイド

## 5分で始めるOMS開発

### Step 1: データベース起動 (1分)

```bash
# MySQLサービス起動（Windows）
net start MySQL80

# スキーマ適用
cd C:\Users\tatsu\source\repos\ClaudeCode\OMS
mysql -u root -p < database/schema.sql

# サンプルデータ投入
mysql -u root -p < database/seed-data.sql

# 確認
mysql -u root -p
> USE oms_db;
> SELECT COUNT(*) FROM orders;  # 8件
> SELECT COUNT(*) FROM securities;  # 20件
> exit
```

### Step 2: バックエンドAPI起動 (2分)

```bash
cd backend/oms-api

# application.propertiesを編集
# spring.datasource.password=your_password

# 起動
mvn spring-boot:run

# 別ターミナルで確認
curl http://localhost:8080/api/orders?userId=1
```

**Swagger UI**: http://localhost:8080/swagger-ui.html

### Step 3: WPFアプリ起動 (2分)

```bash
cd src/OMS.Client

# 起動
dotnet run
```

---

## 最初の開発タスク

### タスク1: StatusBadgeControlを実装する

**場所**: `src/OMS.Client/Controls/Utilities/StatusBadgeControl.xaml`

**参考**: `docs/IMPLEMENTATION_GUIDE.md` の「StatusBadgeControl の実装例」

**手順**:
1. XAMLファイルを作成
2. Code-behindを作成
3. テスト用Viewで使用
4. 動作確認

**所要時間**: 30分

---

### タスク2: Execution APIを実装する

**場所**: `backend/oms-api/src/main/java/com/oms/api/`

**参考**: 既存の`Order.java`、`OrderService.java`、`OrderController.java`

**手順**:
1. `Execution.java` Entity作成
2. `ExecutionRepository.java` 作成
3. `ExecutionService.java` 作成
4. `ExecutionController.java` 作成
5. Swaggerで動作確認

**所要時間**: 45分

---

## よくある質問

### Q: データベースに接続できません

**A**: `backend/oms-api/src/main/resources/application.properties`を確認してください。

```properties
spring.datasource.url=jdbc:mysql://localhost:3306/oms_db?useSSL=false&serverTimezone=Asia/Tokyo
spring.datasource.username=root
spring.datasource.password=your_password_here  # ここを変更
```

### Q: WPFアプリがビルドエラーになります

**A**: NuGetパッケージを復元してください。

```bash
cd src/OMS.Client
dotnet restore
dotnet clean
dotnet build
```

### Q: APIのエンドポイントが404エラーになります

**A**: コンテキストパスを確認してください。

- 正しい: `http://localhost:8080/api/orders`
- 誤り: `http://localhost:8080/orders`

---

## 次のステップ

1. **設計ドキュメントを確認**
   - `docs/plantuml/` - PlantUML図
   - `docs/IMPLEMENTATION_GUIDE.md` - 実装ガイド

2. **残りのUserControlを実装**
   - OrderEntryControl（発注入力フォーム）
   - OrderListGrid（発注一覧グリッド）
   - PortfolioSummaryControl（ポートフォリオサマリー）

3. **AvalonDockレイアウトを構築**
   - MainWindow.xaml にドッキングレイアウトを追加

4. **リアルタイム更新機能を追加**
   - WebSocketでサーバー→クライアント通知
   - Prism EventAggregatorで画面間通信

---

## サポート

- 実装ガイド: `docs/IMPLEMENTATION_GUIDE.md`
- メインREADME: `README.md`
- PlantUML図: `docs/plantuml/`

**Happy Coding! 🚀**
