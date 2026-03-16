using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleVision.Pleasanter.CalendarHolidayStyleGenerator
{
    /// <summary>
    /// Extensions APIリクエストモデル
    /// </summary>
    public class ExtensionApiRequest
    {
        public decimal ApiVersion { get; set; } = 1.1m;
        public string ApiKey { get; set; } = "";
        public string ExtensionType { get; set; } = "";
        public string ExtensionName { get; set; } = "";
        public string Body { get; set; } = "";
        public string Description { get; set; } = "";
        public bool Disabled { get; set; }
    }

    /// <summary>
    /// Extensions API GETレスポンスモデル
    /// </summary>
    public class ExtensionGetResponse
    {
        public int StatusCode { get; set; }
        public ExtensionGetResponseBody Response { get; set; }
    }

    /// <summary>
    /// Extensions API GETレスポンスボディ
    /// </summary>
    public class ExtensionGetResponseBody
    {
        public List<ExtensionData> Data { get; set; } = new List<ExtensionData>();
    }

    /// <summary>
    /// Extensionデータモデル
    /// </summary>
    public class ExtensionData
    {
        public int ExtensionId { get; set; }
        public string ExtensionType { get; set; } = "";
        public string ExtensionName { get; set; } = "";
        public string Body { get; set; } = "";
        public string Description { get; set; } = "";
        public bool Disabled { get; set; }
    }

    /// <summary>
    /// Extensions API 作成・更新・削除レスポンスモデル
    /// </summary>
    public class ExtensionApiResponse
    {
        public int StatusCode { get; set; }
        public int Id { get; set; }
        public string Message { get; set; } = "";
    }
}
