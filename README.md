# VehicleVision.Pleasanter.CalendarHolidayStyleGenerator

Pleasanter のカレンダー表示に祝日・週末のスタイルを適用する CSS ファイルを自動生成するツールです。
Pleasanter v1.5.2.0 の標準カレンダーおよび FullCalendar の両方に対応しています。

## 機能

- 内閣府が公開している祝日 CSV データを取得し、祝日の背景色を設定する CSS を生成
- 土曜日・日曜日の背景色を設定する CSS を生成
- 年ごとに CSS ファイルを分割して出力
- 標準カレンダーと FullCalendar の両方に対応した CSS を生成
- カレンダータイプごとに出力ディレクトリを分割

## Pleasanter リファレンス

本プロジェクトは [Implem.Pleasanter](https://github.com/Implem/Implem.Pleasanter) を
サブモジュールとして参照しています。
CSS セレクタの検証や Pleasanter のカレンダー HTML 構造の確認に使用します。

サブモジュールの初期化:

```bash
git submodule update --init --recursive
```

## 対応カレンダータイプ

### 標準カレンダー（Standard）

Pleasanter の標準カレンダー（`calendarType: "Standard"`）に対応します。

- コンテナ: `#CalendarBody #Grid`
- 日付セル: `td[data-id="yyyy/M/d"]`
- 他月セル: `.other-month`
- 日付表示: `.day`
- 週末: `nth-child` による土曜・日曜の指定

### FullCalendar

Pleasanter の FullCalendar（`calendarType: "FullCalendar"`）に対応します。

- コンテナ: `#FullCalendar .fc`
- 日付セル: `td.fc-daygrid-day[data-date="yyyy-MM-dd"]`
- 他月セル: `.fc-day-other`
- 日付表示: `.fc-daygrid-day-top`
- 土曜: `.fc-day-sat`
- 日曜: `.fc-day-sun`

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

### CSS 出力先

CSS ファイルはカレンダータイプごとに分割して出力されます:

```text
<Pleasanterルートパス>/App_Data/Parameters/ExtendedStyles/CalendarStyle/
├── Standard/                              # 標準カレンダー用
│   ├── CalendarStyle-Holiday2025.css
│   ├── CalendarStyle-Holiday2026.css
│   └── CalendarStyle-Weekend.css
└── FullCalendar/                          # FullCalendar用
    ├── CalendarStyle-Holiday2025.css
    ├── CalendarStyle-Holiday2026.css
    └── CalendarStyle-Weekend.css
```

## プロジェクト構成

```text
VehicleVision.Pleasanter.CalendarHolidayStyleGenerator/
├── .github/                    # GitHub設定（セキュリティポリシー等）
├── .vscode/                    # VS Code設定
├── Implem.Pleasanter/          # Pleasanter本体（サブモジュール）
├── LICENSES/                   # サードパーティライセンス
├── src/
│   └── VehicleVision.Pleasanter.CalendarHolidayStyleGenerator/
│       ├── Parameters/         # パラメータ定義
│       ├── Calendar.cs         # カレンダーモデル
│       ├── Program.cs          # エントリーポイント
│       └── nlog.config         # ログ設定
├── .editorconfig
├── .gitignore
├── .gitmodules
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
| CsvHelper               | MS-PL OR Apache-2.0 | Copyright (c) Josh Close                  |
| Newtonsoft.Json          | MIT           | Copyright (c) James Newton-King                   |
| NLog                     | BSD-3-Clause  | Copyright (c) Jaroslaw Kowalski, Kim Christensen  |
| NLog.Extensions.Logging  | BSD-2-Clause  | Copyright (c) NLog                                |

ライセンスファイルの全文は [LICENSES](./LICENSES/) フォルダを参照してください。

## セキュリティ

セキュリティ上の脆弱性を発見された場合は、[セキュリティポリシー](.github/SECURITY.md)をご確認の上、ご報告ください。

## ライセンス

このプロジェクトは AGPL-3.0 ライセンスの下で公開されています。詳細は [LICENSE](LICENSE) を参照してください。