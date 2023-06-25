﻿using Newtonsoft.Json;
using Quick.SL651;
using System.Net;
using System.Net.Sockets;

var port = 13210;
var test_data = new byte[] 
{
    0x7E,0x7E,
    0x01,
    0x11,0x22,0x33,0x44,0x55,
    0xA0,0x00,
    0x33,0x00,
    0x35,
    0x02,
    0x00,0x0C,
    0x23,0x06,0x19,0x10,0x36,0x05,
    0xF1,0xF1,
    0x11,0x22,0x33,0x44,0x55,
    0x48,
    0xF0,0xF0,
    0x23,0x06,0x19,0x10,0x35,
    0x22,0x19,
    0x00,0x00,0x00,
    0x1A,0x19,
    0x00,0x00,0x00,
    0x20,0x19,
    0x00,0x00,0x00,
    0x26,0x19,
    0x00,0x00,0x00,
    0x39,0x23,
    0x00,0x00,0x00,0x00,
    0x38,0x12,
    0x11,0x30,
    0x03,
    0x76,0x35
};

var centralStation = new CentralStation(new CentralStationOptions()
{
    IPAddress = IPAddress.Loopback,
    Port = port
});
centralStation.TelemetryStationConnected += (sender, telemetryStation) =>
{
    Console.WriteLine($"遥测站[端点：{telemetryStation.RemoteEndPoint}]已连接！遥测站地址：{telemetryStation.TelemetryStationInfo.TelemetryStationAddress}");
    telemetryStation.Disconnected += (sender2, e) =>
    {
        Console.WriteLine($"遥测站[端点：{telemetryStation.RemoteEndPoint}]的连接已断开！");
    };
    telemetryStation.MessageFrameArrived += (sender2, e) =>
    {
        Console.WriteLine($"遥测站[端点：{telemetryStation.RemoteEndPoint}]接收到报文帧：{e.UpgoingMessage.GetType().FullName}");
    };
};
centralStation.Start();
Console.WriteLine("中心站已启动！");
_ = Task.Run(async () =>
{
    var client = new TcpClient();
    client.Connect(IPAddress.Loopback, port);
    var stream = client.GetStream();
    for (var i = 0; i < 10; i++)
    {
        await Task.Delay(5000);
        await stream.WriteAsync(test_data);
    }
});
Console.ReadLine();
centralStation.Stop();