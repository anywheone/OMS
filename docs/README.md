# OMS ドキュメント

このディレクトリには、OMSプロジェクトで使用している技術スタックの詳細な解説ドキュメントが含まれています。

## 📚 ドキュメント一覧

### WPF関連

#### 1. [Prism_Guide.md](./Prism_Guide.md) - Prismフレームワーク
- Prismとは
- 依存性注入（DI）
- MVVMパターン
- ViewModelLocator
- コマンド
- このプロジェクトでの実装例
- ベストプラクティス

**推奨読者:** Prism初心者、MVVMを学びたい方

---

#### 2. [MVVM_Guide.md](./MVVM_Guide.md) - MVVMパターン
- MVVMとは
- Model-View-ViewModelの構成要素
- データバインディング
- INotifyPropertyChanged
- コマンド
- 実装パターン
- アンチパターン

**推奨読者:** MVVMパターンを深く理解したい方

---

#### 3. [WPF_Guide.md](./WPF_Guide.md) - WPF基礎
- WPFとは
- レイアウト（Grid, StackPanel, DockPanel等）
- コントロール
- リソース
- スタイルとテンプレート
- アニメーション
- ベストプラクティス

**推奨読者:** WPF初心者、レイアウトを学びたい方

---

#### 4. [UserControl_Guide.md](./UserControl_Guide.md) - ユーザーコントロール
- ユーザーコントロールとは
- 作成方法
- 依存関係プロパティ
- データバインディング
- イベント処理
- 実装パターン
- 実装例

**推奨読者:** 再利用可能なコントロールを作りたい方

---

#### 5. [DataBinding_Guide.md](./DataBinding_Guide.md) - データバインディング
- データバインディングとは
- バインディングモード（OneWay, TwoWay等）
- バインディングソース（DataContext, ElementName等）
- 値の変換（IValueConverter）
- バリデーション
- コレクションバインディング
- トラブルシューティング

**推奨読者:** データバインディングを極めたい方

---

#### 6. [XAML_Guide.md](./XAML_Guide.md) - XAML基礎
- XAMLとは
- 基本構文
- マークアップ拡張（{Binding}, {StaticResource}等）
- 名前空間
- 添付プロパティ
- コレクション構文
- リソース
- ベストプラクティス

**推奨読者:** XAML初心者、構文を体系的に学びたい方

---

### アーキテクチャ関連

#### 7. [DI_Guide.md](./DI_Guide.md) - 依存性注入
- 依存性注入とは
- DIの利点
- DIコンテナ
- ライフタイム管理（Singleton, Transient, Scoped）
- 実装パターン
- PrismでのDI
- ベストプラクティス
- よくある間違い

**推奨読者:** DIを理解したい方、テスタブルなコードを書きたい方

---

## 🎯 学習ロードマップ

### 初心者向け

1. **まずはここから**
   - [WPF_Guide.md](./WPF_Guide.md) - WPFの基礎を理解
   - [XAML_Guide.md](./XAML_Guide.md) - XAML構文を学ぶ

2. **次のステップ**
   - [DataBinding_Guide.md](./DataBinding_Guide.md) - データバインディングを習得
   - [MVVM_Guide.md](./MVVM_Guide.md) - MVVMパターンを理解

3. **発展**
   - [Prism_Guide.md](./Prism_Guide.md) - Prismの使い方を学ぶ
   - [DI_Guide.md](./DI_Guide.md) - 依存性注入を理解
   - [UserControl_Guide.md](./UserControl_Guide.md) - カスタムコントロールを作る

### 中級者向け

1. **MVVMの深掘り**
   - [MVVM_Guide.md](./MVVM_Guide.md) - 実装パターンを学ぶ
   - [Prism_Guide.md](./Prism_Guide.md) - Prismの活用

2. **アーキテクチャ**
   - [DI_Guide.md](./DI_Guide.md) - DIの深い理解
   - [UserControl_Guide.md](./UserControl_Guide.md) - 再利用可能なコンポーネント設計

### 上級者向け

- すべてのドキュメントを読み、ベストプラクティスを実践
- 実装例を参考に独自のアプリケーションを構築

---

## 📖 トピック別索引

### データバインディング関連
- [DataBinding_Guide.md](./DataBinding_Guide.md) - データバインディング詳細
- [MVVM_Guide.md](./MVVM_Guide.md) - MVVMでのバインディング
- [UserControl_Guide.md](./UserControl_Guide.md) - カスタムコントロールでのバインディング

### 設計パターン関連
- [MVVM_Guide.md](./MVVM_Guide.md) - MVVMパターン
- [DI_Guide.md](./DI_Guide.md) - 依存性注入パターン
- [Prism_Guide.md](./Prism_Guide.md) - Prismパターン

### UI構築関連
- [WPF_Guide.md](./WPF_Guide.md) - レイアウトとコントロール
- [XAML_Guide.md](./XAML_Guide.md) - XAML構文
- [UserControl_Guide.md](./UserControl_Guide.md) - カスタムコントロール

### アーキテクチャ関連
- [Prism_Guide.md](./Prism_Guide.md) - Prismアーキテクチャ
- [DI_Guide.md](./DI_Guide.md) - 依存性注入アーキテクチャ
- [MVVM_Guide.md](./MVVM_Guide.md) - MVVMアーキテクチャ

---

## 🔍 よくある質問と該当ドキュメント

### Q: データバインディングが効かない
→ [DataBinding_Guide.md](./DataBinding_Guide.md) の「トラブルシューティング」を参照

### Q: ViewModelとViewの紐付け方がわからない
→ [Prism_Guide.md](./Prism_Guide.md) の「ViewModelLocator」を参照

### Q: 依存性注入の登録方法がわからない
→ [DI_Guide.md](./DI_Guide.md) の「DIコンテナ」を参照

### Q: カスタムコントロールの作り方がわからない
→ [UserControl_Guide.md](./UserControl_Guide.md) の「作成方法」を参照

### Q: レイアウトがうまくいかない
→ [WPF_Guide.md](./WPF_Guide.md) の「レイアウト」を参照

### Q: XAMLの構文がわからない
→ [XAML_Guide.md](./XAML_Guide.md) の「基本構文」を参照

### Q: MVVMパターンがよくわからない
→ [MVVM_Guide.md](./MVVM_Guide.md) を最初から読む

---

## 💡 実装例の場所

各ドキュメントで紹介されている実装例は、このOMSプロジェクトの以下の場所にあります:

### ViewModels
- `src/OMS.Client/ViewModels/MainWindowViewModel.cs`
- `src/OMS.Client/ViewModels/ControlLibraryViewModel.cs`

### Views
- `src/OMS.Client/Views/MainWindow.xaml`
- `src/OMS.Client/Views/ControlLibraryWindow.xaml`

### Models
- `src/OMS.Client/Models/ControlInfo.cs`
- `src/OMS.Core/Models/OrderModel.cs`

### Services
- `src/OMS.Client/Services/OrderService.cs`

### App設定
- `src/OMS.Client/App.xaml.cs` - DI登録、Prism設定

---

## 📝 ドキュメントの更新履歴

| 日付 | 更新内容 |
|------|---------|
| 2025-11-16 | 初版作成 - 全7ファイル |

---

## 🤝 貢献

ドキュメントの改善提案や誤りの報告は、GitHubのIssueで受け付けています。

---

**プロジェクト:** OMS (Order Management System)
**作成日:** 2025-11-16
