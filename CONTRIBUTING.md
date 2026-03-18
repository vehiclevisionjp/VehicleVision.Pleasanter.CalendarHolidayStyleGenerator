# コントリビューションガイド

VehicleVision.Pleasanter.CalendarHolidayStyleGenerator へのコントリビューションに感謝します。
このドキュメントでは、プロジェクトへの貢献方法について説明します。

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->

- [はじめに](#はじめに)
- [ガイドライン一覧](#ガイドライン一覧)
- [クイックスタート](#クイックスタート)
    - [1. リポジトリをフォーク・クローン](#1-リポジトリをフォーククローン)
    - [2. サブモジュールの初期化](#2-サブモジュールの初期化)
    - [3. ブランチを作成](#3-ブランチを作成)
    - [4. 変更を実装](#4-変更を実装)
    - [5. コミット・プッシュ](#5-コミットプッシュ)
    - [6. プルリクエストを作成](#6-プルリクエストを作成)
- [Pleasanter リファレンス](#pleasanter-リファレンス)
- [ビルド](#ビルド)
- [コミットメッセージ](#コミットメッセージ)
- [Issue報告](#issue報告)
- [ライセンス](#ライセンス)
- [質問・サポート](#質問サポート)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## はじめに

コントリビューションを行う前に、以下のガイドラインをご確認ください。

## ガイドライン一覧

| ガイドライン                                                              | 内容                                   |
| ------------------------------------------------------------------------- | -------------------------------------- |
| [ドキュメントガイドライン](docs/contributing/documentation-guidelines.md) | Markdown記法、ファイル構成、同期ルール |

## クイックスタート

### 1. リポジトリをフォーク・クローン

```bash
git clone https://github.com/your-username/VehicleVision.Pleasanter.CalendarHolidayStyleGenerator.git
cd VehicleVision.Pleasanter.CalendarHolidayStyleGenerator
```

### 2. サブモジュールの初期化

本プロジェクトは [Implem.Pleasanter](https://github.com/Implem/Implem.Pleasanter) を
サブモジュールとして参照しています。クローン後にサブモジュールを初期化してください。

```bash
git submodule update --init --recursive
```

### 3. ブランチを作成

```bash
git checkout -b feature/your-feature-name
```

### 4. 変更を実装

- ドキュメントを更新（[ドキュメントガイドライン](docs/contributing/documentation-guidelines.md)参照）

### 5. コミット・プッシュ

```bash
git add .
git commit -m "feat: 変更の概要"
git push origin feature/your-feature-name
```

### 6. プルリクエストを作成

GitHub上でプルリクエストを作成してください。

## Pleasanter リファレンス

本プロジェクトは [Implem.Pleasanter](https://github.com/Implem/Implem.Pleasanter) を
サブモジュールとして参照しています。
CSS セレクタの検証や Pleasanter のカレンダー HTML 構造の確認に使用します。

## ビルド

```bash
dotnet build
```

## コミットメッセージ

以下のプレフィックスを使用：

| プレフィックス | 用途               |
| -------------- | ------------------ |
| `feat:`        | 新機能             |
| `fix:`         | バグ修正           |
| `docs:`        | ドキュメント更新   |
| `refactor:`    | リファクタリング   |
| `test:`        | テスト追加・修正   |
| `chore:`       | その他（ビルド等） |

## Issue報告

バグ報告や機能要望は、GitHubのIssueで受け付けています。

- **バグ報告**: 再現手順、期待される動作、実際の動作を記載
- **機能要望**: ユースケースと期待される動作を記載

## ライセンス

このプロジェクトは AGPL-3.0 ライセンスの下で公開されています。コントリビューションは同じライセンスの下で提供されます。

## 質問・サポート

質問がある場合は、GitHubのDiscussionsまたはIssueでお気軽にお問い合わせください。
