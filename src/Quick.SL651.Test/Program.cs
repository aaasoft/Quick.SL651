using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Quick.SL651;
using Quick.SL651.Messages;
using System.Net;

var ipadress = IPAddress.Any;
var port = 22222;
var jsonSerializerSettings = new JsonSerializerSettings()
{
    Formatting = Formatting.Indented,
    Converters = new List<JsonConverter>()
    {
        new StringEnumConverter()
    }
};
var centralStation = new CentralStation(new CentralStationOptions()
{
    IPAddress = ipadress,
    Port = port
});
centralStation.TelemetryStationConnected += (sender, e) =>
{
    var telemetryStation = e.TelemetryStation;
    Console.WriteLine($"[{DateTime.Now}] 遥测站[端点：{telemetryStation.RemoteEndPoint}]已连接！遥测站地址：{telemetryStation.TelemetryStationInfo.TelemetryStationAddress_Text}");
    telemetryStation.Disconnected += (sender2, e) =>
    {
        Console.WriteLine($"[{DateTime.Now}] 遥测站[端点：{telemetryStation.RemoteEndPoint}]的连接已断开！");
    };
    telemetryStation.RawDataArrived += (sender2, e) =>
    {
        Console.WriteLine($"[{DateTime.Now}] [{telemetryStation.TelemetryStationInfo.TelemetryStationAddress_Text}][RX]{BitConverter.ToString(e.ToArray())}");
    };
    telemetryStation.RawDataSent += (sender2, e) =>
    {
        Console.WriteLine($"[{DateTime.Now}] [{telemetryStation.TelemetryStationInfo.TelemetryStationAddress_Text}][TX]{BitConverter.ToString(e.ToArray())}");
    };
    telemetryStation.MessageFrameArrived += (sender2, e) =>
    {
        Console.WriteLine($"[{DateTime.Now}] [{telemetryStation.TelemetryStationInfo.TelemetryStationAddress_Text}][上行报文帧] 功能码：[{e.FrameInfo.FunctionCode}]，结束符：[{e.FrameInfo.EndMark}]，报文：{JsonConvert.SerializeObject(e.Message, jsonSerializerSettings)}");
    };
    telemetryStation.SendDowngoingMessage(Quick.SL651.Enums.FunctionCodes.M37, new Message(), Quick.SL651.Enums.EndMarks.ENQ);
};
centralStation.Start();
Console.WriteLine($"中心站已启动！监听端点：{ipadress}:{port}");
Console.ReadLine();
centralStation.Stop();