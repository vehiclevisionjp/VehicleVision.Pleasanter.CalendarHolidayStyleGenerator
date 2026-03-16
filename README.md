# VehicleVision.Pleasanter.CalendarHolidayStyleGenerator

Pleasanter のカレンダー表示に祝日・週末のスタイルを適用する CSS ファイルを自動生成するツールです。
Pleasanter v1.5.2.0 の標準カレンダーおよび FullCalendar の両方に対応しています。

## 機能

- 内閣府が公開している祝日 CSV データを取得し、祝日の背景色を設定する CSS を生成
- 土曜日・日曜日の背景色を設定する CSS を生成
- 年ごとに CSS ファイルを分割して出力
- 標準カレンダーと FullCalendar の両方に対応した CSS を生成
- カレンダータイプごとに出力ディレクトリを分割
- API モードで Pleasanter の Extensions テーブルに直接書き込み可能

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

### ファイルモード（デフォルト）

```bash
dotnet run --project src/VehicleVision.Pleasanter.CalendarHolidayStyleGenerator -- /p <Pleasanterルートパス>
```

### API モード

```bash
dotnet run --project src/VehicleVision.Pleasanter.CalendarHolidayStyleGenerator -- /m api
```

API モードを使用する場合は、事前に `Parameters/Calendar.json` に `ApiUrl` と `ApiKey` を設定してください。

### コマンドライン引数

| 引数 | 説明                                                                   |
| ---- | ---------------------------------------------------------------------- |
| `/p` | Pleasanter のルートパス（省略時: `../Implem.Pleasanter`）              |
| `/a` | 全年度を強制更新（省略時: 現在年度以降のみ更新）                       |
| `/m` | 出力モード: `file`（デフォルト）または `api`                           |

### CSS 出力先（ファイルモード）

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

### Extensions テーブル登録（API モード）

API モードでは、CSS を Pleasanter の Extensions テーブルに `Style` タイプとして登録します。
以下の Extension 名で登録されます:

| Extension 名                                 | 内容                           |
| -------------------------------------------- | ------------------------------ |
| `CalendarStyle-Standard-Holiday{年}`         | 標準カレンダー用 祝日スタイル  |
| `CalendarStyle-Standard-Weekend`             | 標準カレンダー用 週末スタイル  |
| `CalendarStyle-FullCalendar-Holiday{年}`     | FullCalendar 用 祝日スタイル   |
| `CalendarStyle-FullCalendar-Weekend`         | FullCalendar 用 週末スタイル   |

同名の Extension が既に存在する場合は更新、存在しない場合は新規作成されます。

### パラメータ設定

`Parameters/Calendar.json` で以下のパラメータを設定できます:

| パラメータ               | 説明                                     | デフォルト値                                            |
| ------------------------ | ---------------------------------------- | ------------------------------------------------------- |
| `CalendarUrl`            | 祝日 CSV データの URL                    | `https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv` |
| `SaturdayIndex`          | 標準カレンダーの土曜日列インデックス     | `6`                                                     |
| `SundayIndex`            | 標準カレンダーの日曜日列インデックス     | `7`                                                     |
| `SaturdayBackgroundColor`| 土曜日の背景色                           | `#add8e6`                                               |
| `SundayBackgroundColor`  | 日曜日の背景色                           | `#ffc0cb`                                               |
| `HolidayBackgroundColor` | 祝日の背景色                             | `#ffc0cb`                                               |
| `ApiUrl`                 | Pleasanter API の URL（API モード時必須）| `""`                                                    |
| `ApiKey`                 | Pleasanter API キー（API モード時必須）  | `""`                                                    |

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