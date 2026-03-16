# VehicleVision.Pleasanter.CalendarHolidayStyleGenerator

Pleasanter のカレンダー表示に祝日・週末のスタイルを適用する CSS ファイルを自動生成するツールです。

## 機能

- 内閣府が公開している祝日 CSV データを取得し、祝日の背景色を設定する CSS を生成
- 土曜日・日曜日の背景色を設定する CSS を生成
- 年ごとに CSS ファイルを分割して出力

## ビルド

```bash
dotnet build
```

## 使い方

```bash
dotnet run --project src/VehicleVision.Pleasanter.CalendarHolidayStyleGenerator -- /p <Pleasanterルートパス>
```

### コマンドライン引数

| 引数 | 説明                                                                   |
| ---- | ---------------------------------------------------------------------- |
| `/p` | Pleasanter のルートパス（省略時: `../Implem.Pleasanter`）              |
| `/a` | 全年度を強制更新（省略時: 現在年度以降のみ更新）                       |

## プロジェクト構成

```text
VehicleVision.Pleasanter.CalendarHolidayStyleGenerator/
├── .github/                    # GitHub設定（セキュリティポリシー等）
├── .vscode/                    # VS Code設定
├── LICENSES/                   # サードパーティライセンス
├── src/
│   └── VehicleVision.Pleasanter.CalendarHolidayStyleGenerator/
│       ├── Parameters/         # パラメータ定義
│       ├── Calendar.cs         # カレンダーモデル
│       ├── Program.cs          # エントリーポイント
│       └── nlog.config         # ログ設定
├── .editorconfig
├── .gitignore
├── .markdownlint-cli2.jsonc
├── .prettierignore
├── .prettierrc
├── AUTHORS
├── CONTRIBUTING.md
├── Directory.Build.props
├── LICENSE
├── README.md
├── VehicleVision.Pleasanter.CalendarHolidayStyleGenerator.slnx
└── package.json
```

## サードパーティライセンス

このプロジェクトは以下のサードパーティライブラリを使用しています：

| ライブラリ              | ライセンス    | 著作権                                            |
| ----------------------- | ------------- | ------------------------------------------------- |
| CsvHelper               | MS-PL / Apache-2.0 | Copyright (c) Josh Close                   |
| Newtonsoft.Json          | MIT           | Copyright (c) James Newton-King                   |
| NLog                     | BSD-3-Clause  | Copyright (c) Jaroslaw Kowalski, Kim Christensen  |
| NLog.Extensions.Logging  | BSD-2-Clause  | Copyright (c) NLog                                |

ライセンスファイルの全文は [LICENSES](./LICENSES/) フォルダを参照してください。

## セキュリティ

セキュリティ上の脆弱性を発見された場合は、[セキュリティポリシー](.github/SECURITY.md)をご確認の上、ご報告ください。

## ライセンス

このプロジェクトは AGPL-3.0 ライセンスの下で公開されています。詳細は [LICENSE](LICENSE) を参照してください。