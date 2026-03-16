using CsvHelper;
using Newtonsoft.Json;
using NLog;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace VehicleVision.Pleasanter.CalendarHolidayStyleGenerator
{
    class Program
    {
        private static string currentPath => Directory.GetCurrentDirectory();

        private static readonly HttpClient httpClient = new HttpClient();

        private static Logger logger = LogManager.GetCurrentClassLogger();

        static async Task Main(string[] args)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                //パラメータを取得する
                var argsDic = ArgsType(args);

                //全更新するかどうか
                var allRefresh = argsDic.ContainsKey("a");

                //出力モードの取得（file または api）
                if (!argsDic.TryGetValue("m", out var mode))
                {
                    mode = "file";
                }

                mode = mode.ToLowerInvariant();

                //パラメータ読み取り
                //ジェネレータのカレンダー設定
                var paramCalendar = DeserializeFromFile<Parameters.Calendar>(Path.Combine(currentPath, "Parameters", "Calendar.json"));

                var calendarStylePath = string.Empty;

                if (mode == "api")
                {
                    //APIモードの場合、APIのURLとAPIキーが必要
                    if (string.IsNullOrWhiteSpace(paramCalendar.ApiUrl) || string.IsNullOrWhiteSpace(paramCalendar.ApiKey))
                    {
                        logger.Fatal("API mode requires ApiUrl and ApiKey to be set in Parameters/Calendar.json.");
                        return;
                    }
                }
                else if (mode == "file")
                {
                    //ファイルモードの場合、ベースパスの有無を取得する
                    if (!argsDic.TryGetValue("p", out var rootPath))
                    {
                        rootPath = Path.Combine(currentPath, "..", "Implem.Pleasanter");
                    }

                    //ベースのパスが存在しない時は落とす
                    if (!Directory.Exists(rootPath))
                    {
                        logger.Fatal("Root path does not exist. Please check the path.");
                        return;
                    }

                    var exStylePath = Path.Combine(rootPath, "App_Data", "Parameters", "ExtendedStyles");

                    //拡張スタイルのパスが存在しない時は落とす
                    if (!Directory.Exists(exStylePath))
                    {
                        logger.Fatal("ExtendedStyles path does not exist. Please check the path.");
                        return;
                    }

                    //ベースパスから出力先のパスを取得する
                    calendarStylePath = Path.Combine(exStylePath, "CalendarStyle");
                }
                else
                {
                    logger.Fatal($"Unknown mode: {mode}. Use 'file' or 'api'.");
                    return;
                }

                //内閣府のサイトより公示された祝日データを取得する
                using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(paramCalendar.CalendarUrl)))
                using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        logger.Fatal("Unable to retrieve the CSV file for the calendar.");
                        return;
                    }

                    using (var content = response.Content)
                    using (var stream = await content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream, Encoding.GetEncoding("Shift_JIS")))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<Calendar>().ToList();

                        if (mode == "api")
                        {
                            //APIモード：Extensionsテーブルに書き込む
                            await GenerateCalendarCssViaApiAsync(
                                records, paramCalendar, allRefresh);
                        }
                        else
                        {
                            //ファイルモード：CSSファイルを生成する
                            //標準カレンダー用CSS生成
                            GenerateStandardCalendarCss(
                                records, calendarStylePath, paramCalendar, allRefresh);

                            //FullCalendar用CSS生成
                            GenerateFullCalendarCss(
                                records, calendarStylePath, paramCalendar, allRefresh);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// 標準カレンダー用のCSSファイルを生成する
        /// </summary>
        private static void GenerateStandardCalendarCss(
            List<Calendar> records,
            string calendarStylePath,
            Parameters.Calendar paramCalendar,
            bool allRefresh)
        {
            var outputPath = Path.Combine(calendarStylePath, "Standard");

            //出力先が存在しない時は作る
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            //祝日
            foreach (var recordsYear in records.GroupBy(record => record.Date.Year))
            {
                var outputFile = Path.Combine(outputPath, $"CalendarStyle-Holiday{recordsYear.Key}.css");

                //既に出力済みのファイルがある場合は削除する
                if (File.Exists(outputFile))
                {
                    //存在するファイルが過去のものである場合は更新
                    if (recordsYear.Key < DateTime.Today.Year && !allRefresh)
                    {
                        continue;
                    }

                    File.Delete(outputFile);
                    logger.Info($"{Path.GetFileName(outputFile)} Deleted.");
                }

                if (recordsYear.Any())
                {
                    foreach (var record in recordsYear)
                    {
                        File.AppendAllText(
                            outputFile,
                            @$"#CalendarBody #Grid tbody tr td[data-id=""{record.Date:yyyy/M/d}""]:not(.other-month){{background-color:{paramCalendar.HolidayBackgroundColor} !important;}}"
                            + Environment.NewLine
                            + $@"#CalendarBody #Grid tbody tr td[data-id=""{record.Date:yyyy/M/d}""] div .day:after{{content:""{record.Title}"";margin-left:5px;}}"
                            + Environment.NewLine
                        );
                    }

                    logger.Info($"Standard/{Path.GetFileName(outputFile)} Created.");
                }
            }

            //週末
            {
                var outputFile = Path.Combine(outputPath, $"CalendarStyle-Weekend.css");

                //既に出力済みのファイルがある場合は削除する
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                    logger.Info($"{Path.GetFileName(outputFile)} Deleted.");
                }

                File.AppendAllText(
                outputFile,
                    @$"#CalendarBody #Grid tbody tr td:nth-child({paramCalendar.SaturdayIndex}):not(.other-month){{background-color:{paramCalendar.SaturdayBackgroundColor};}}"
                    + Environment.NewLine
                    + @$"#CalendarBody #Grid tbody tr td:nth-child({paramCalendar.SundayIndex}):not(.other-month){{background-color:{paramCalendar.SundayBackgroundColor};}}"
                    + Environment.NewLine
                );

                logger.Info($"Standard/{Path.GetFileName(outputFile)} Created.");
            }
        }

        /// <summary>
        /// FullCalendar用のCSSファイルを生成する
        /// </summary>
        private static void GenerateFullCalendarCss(
            List<Calendar> records,
            string calendarStylePath,
            Parameters.Calendar paramCalendar,
            bool allRefresh)
        {
            var outputPath = Path.Combine(calendarStylePath, "FullCalendar");

            //出力先が存在しない時は作る
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            //祝日
            foreach (var recordsYear in records.GroupBy(record => record.Date.Year))
            {
                var outputFile = Path.Combine(outputPath, $"CalendarStyle-Holiday{recordsYear.Key}.css");

                //既に出力済みのファイルがある場合は削除する
                if (File.Exists(outputFile))
                {
                    //存在するファイルが過去のものである場合は更新
                    if (recordsYear.Key < DateTime.Today.Year && !allRefresh)
                    {
                        continue;
                    }

                    File.Delete(outputFile);
                    logger.Info($"{Path.GetFileName(outputFile)} Deleted.");
                }

                if (recordsYear.Any())
                {
                    foreach (var record in recordsYear)
                    {
                        File.AppendAllText(
                            outputFile,
                            @$"#FullCalendar .fc td.fc-daygrid-day[data-date=""{record.Date:yyyy-MM-dd}""]:not(.fc-day-other){{background-color:{paramCalendar.HolidayBackgroundColor} !important;}}"
                            + Environment.NewLine
                            + @$"#FullCalendar .fc td.fc-daygrid-day[data-date=""{record.Date:yyyy-MM-dd}""] .fc-daygrid-day-top:after{{content:""{record.Title}"";margin-left:5px;}}"
                            + Environment.NewLine
                        );
                    }

                    logger.Info($"FullCalendar/{Path.GetFileName(outputFile)} Created.");
                }
            }

            //週末
            {
                var outputFile = Path.Combine(outputPath, $"CalendarStyle-Weekend.css");

                //既に出力済みのファイルがある場合は削除する
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                    logger.Info($"{Path.GetFileName(outputFile)} Deleted.");
                }

                File.AppendAllText(
                outputFile,
                    @$"#FullCalendar .fc td.fc-day-sat:not(.fc-day-other){{background-color:{paramCalendar.SaturdayBackgroundColor};}}"
                    + Environment.NewLine
                    + @$"#FullCalendar .fc td.fc-day-sun:not(.fc-day-other){{background-color:{paramCalendar.SundayBackgroundColor};}}"
                    + Environment.NewLine
                );

                logger.Info($"FullCalendar/{Path.GetFileName(outputFile)} Created.");
            }
        }

        /// <summary>
        /// APIを使用してExtensionsテーブルにカレンダースタイルを書き込む
        /// </summary>
        private static async Task GenerateCalendarCssViaApiAsync(
            List<Calendar> records,
            Parameters.Calendar paramCalendar,
            bool allRefresh)
        {
            //既存のExtensionを取得する
            var existingExtensions = await GetExtensionsAsync(paramCalendar);

            //標準カレンダー用CSS生成・登録
            await GenerateStandardCalendarCssViaApiAsync(
                records, paramCalendar, allRefresh, existingExtensions);

            //FullCalendar用CSS生成・登録
            await GenerateFullCalendarCssViaApiAsync(
                records, paramCalendar, allRefresh, existingExtensions);
        }

        /// <summary>
        /// 標準カレンダー用のCSSをAPIで登録する
        /// </summary>
        private static async Task GenerateStandardCalendarCssViaApiAsync(
            List<Calendar> records,
            Parameters.Calendar paramCalendar,
            bool allRefresh,
            List<ExtensionData> existingExtensions)
        {
            //祝日
            foreach (var recordsYear in records.GroupBy(record => record.Date.Year))
            {
                //過去の年のデータである場合はスキップ
                if (recordsYear.Key < DateTime.Today.Year && !allRefresh)
                {
                    continue;
                }

                if (recordsYear.Any())
                {
                    var extensionName = $"CalendarStyle-Standard-Holiday{recordsYear.Key}";
                    var sb = new StringBuilder();

                    foreach (var record in recordsYear)
                    {
                        sb.Append(
                            @$"#CalendarBody #Grid tbody tr td[data-id=""{record.Date:yyyy/M/d}""]:not(.other-month){{background-color:{paramCalendar.HolidayBackgroundColor} !important;}}"
                            + Environment.NewLine
                            + $@"#CalendarBody #Grid tbody tr td[data-id=""{record.Date:yyyy/M/d}""] div .day:after{{content:""{record.Title}"";margin-left:5px;}}"
                            + Environment.NewLine
                        );
                    }

                    await CreateOrUpdateExtensionAsync(
                        paramCalendar, extensionName, sb.ToString(), existingExtensions);
                }
            }

            //週末
            {
                var extensionName = "CalendarStyle-Standard-Weekend";
                var body =
                    @$"#CalendarBody #Grid tbody tr td:nth-child({paramCalendar.SaturdayIndex}):not(.other-month){{background-color:{paramCalendar.SaturdayBackgroundColor};}}"
                    + Environment.NewLine
                    + @$"#CalendarBody #Grid tbody tr td:nth-child({paramCalendar.SundayIndex}):not(.other-month){{background-color:{paramCalendar.SundayBackgroundColor};}}"
                    + Environment.NewLine;

                await CreateOrUpdateExtensionAsync(
                    paramCalendar, extensionName, body, existingExtensions);
            }
        }

        /// <summary>
        /// FullCalendar用のCSSをAPIで登録する
        /// </summary>
        private static async Task GenerateFullCalendarCssViaApiAsync(
            List<Calendar> records,
            Parameters.Calendar paramCalendar,
            bool allRefresh,
            List<ExtensionData> existingExtensions)
        {
            //祝日
            foreach (var recordsYear in records.GroupBy(record => record.Date.Year))
            {
                //過去の年のデータである場合はスキップ
                if (recordsYear.Key < DateTime.Today.Year && !allRefresh)
                {
                    continue;
                }

                if (recordsYear.Any())
                {
                    var extensionName = $"CalendarStyle-FullCalendar-Holiday{recordsYear.Key}";
                    var sb = new StringBuilder();

                    foreach (var record in recordsYear)
                    {
                        sb.Append(
                            @$"#FullCalendar .fc td.fc-daygrid-day[data-date=""{record.Date:yyyy-MM-dd}""]:not(.fc-day-other){{background-color:{paramCalendar.HolidayBackgroundColor} !important;}}"
                            + Environment.NewLine
                            + @$"#FullCalendar .fc td.fc-daygrid-day[data-date=""{record.Date:yyyy-MM-dd}""] .fc-daygrid-day-top:after{{content:""{record.Title}"";margin-left:5px;}}"
                            + Environment.NewLine
                        );
                    }

                    await CreateOrUpdateExtensionAsync(
                        paramCalendar, extensionName, sb.ToString(), existingExtensions);
                }
            }

            //週末
            {
                var extensionName = "CalendarStyle-FullCalendar-Weekend";
                var body =
                    @$"#FullCalendar .fc td.fc-day-sat:not(.fc-day-other){{background-color:{paramCalendar.SaturdayBackgroundColor};}}"
                    + Environment.NewLine
                    + @$"#FullCalendar .fc td.fc-day-sun:not(.fc-day-other){{background-color:{paramCalendar.SundayBackgroundColor};}}"
                    + Environment.NewLine;

                await CreateOrUpdateExtensionAsync(
                    paramCalendar, extensionName, body, existingExtensions);
            }
        }

        /// <summary>
        /// Extensions APIから既存のExtensionを取得する
        /// </summary>
        private static async Task<List<ExtensionData>> GetExtensionsAsync(
            Parameters.Calendar paramCalendar)
        {
            var apiUrl = paramCalendar.ApiUrl.TrimEnd('/');
            var url = $"{apiUrl}/api/extensions/Get";

            var requestBody = new ExtensionApiRequest
            {
                ApiKey = paramCalendar.ApiKey
            };

            var json = JsonConvert.SerializeObject(requestBody);
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
            {
                requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var response = await httpClient.SendAsync(requestMessage))
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        logger.Warn($"Failed to get extensions. StatusCode: {response.StatusCode}");
                        return new List<ExtensionData>();
                    }

                    var result = JsonConvert.DeserializeObject<ExtensionGetResponse>(responseBody);
                    return result?.Response?.Data ?? new List<ExtensionData>();
                }
            }
        }

        /// <summary>
        /// Extensionを作成または更新する
        /// </summary>
        private static async Task CreateOrUpdateExtensionAsync(
            Parameters.Calendar paramCalendar,
            string extensionName,
            string body,
            List<ExtensionData> existingExtensions)
        {
            var existing = existingExtensions
                .FirstOrDefault(e => e.ExtensionName == extensionName);

            if (existing != null)
            {
                await UpdateExtensionAsync(paramCalendar, existing.ExtensionId, extensionName, body);
            }
            else
            {
                await CreateExtensionAsync(paramCalendar, extensionName, body);
            }
        }

        /// <summary>
        /// Extensions APIで新規Extensionを作成する
        /// </summary>
        private static async Task CreateExtensionAsync(
            Parameters.Calendar paramCalendar,
            string extensionName,
            string body)
        {
            var apiUrl = paramCalendar.ApiUrl.TrimEnd('/');
            var url = $"{apiUrl}/api/extensions/Create";

            var requestBody = new ExtensionApiRequest
            {
                ApiKey = paramCalendar.ApiKey,
                ExtensionType = "Style",
                ExtensionName = extensionName,
                Body = body,
                Description = extensionName,
                Disabled = false
            };

            var json = JsonConvert.SerializeObject(requestBody);
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
            {
                requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var response = await httpClient.SendAsync(requestMessage))
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        logger.Info($"Extension '{extensionName}' Created.");
                    }
                    else
                    {
                        logger.Error($"Failed to create extension '{extensionName}'. StatusCode: {response.StatusCode}, Response: {responseBody}");
                    }
                }
            }
        }

        /// <summary>
        /// Extensions APIで既存Extensionを更新する
        /// </summary>
        private static async Task UpdateExtensionAsync(
            Parameters.Calendar paramCalendar,
            int extensionId,
            string extensionName,
            string body)
        {
            var apiUrl = paramCalendar.ApiUrl.TrimEnd('/');
            var url = $"{apiUrl}/api/extensions/{extensionId}/Update";

            var requestBody = new ExtensionApiRequest
            {
                ApiKey = paramCalendar.ApiKey,
                ExtensionType = "Style",
                ExtensionName = extensionName,
                Body = body,
                Description = extensionName,
                Disabled = false
            };

            var json = JsonConvert.SerializeObject(requestBody);
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
            {
                requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var response = await httpClient.SendAsync(requestMessage))
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        logger.Info($"Extension '{extensionName}' Updated.");
                    }
                    else
                    {
                        logger.Error($"Failed to update extension '{extensionName}'. StatusCode: {response.StatusCode}, Response: {responseBody}");
                    }
                }
            }
        }

        public static Dictionary<string, string> ArgsType(string[] args)
        {
            var argsDic = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                //'/'始まりの場合はそれをパラメータと識別する
                if (Regex.IsMatch(args[i], "^/"))
                {
                    string key = Regex.Replace(args[i], "^/", "");
                    string value = string.Empty;

                    //今のサーチ場所が最末尾でない
                    //次の場所がパラメータでない
                    if (i != args.Length - 1 && !Regex.IsMatch(args[i + 1], "^/"))
                    {
                        value = args[i + 1];
                        i++;
                    }

                    argsDic.Add(key, value);
                }
            }

            return argsDic;
        }

        public static T DeserializeFromFile<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default;
            }

            try
            {
                //JSONファイルを開く
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    //JSONファイルを読み出す
                    using (var sr = new StreamReader(stream))
                    {
                        //デシリアライズオブジェクト関数に読み込んだデータを渡して、
                        //指定されたデータ用のクラス型で値を返す。
                        return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return default;
            }
        }
    }
}