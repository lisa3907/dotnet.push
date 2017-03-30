using System;

namespace DotNet.Push.Core.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //var _apns = new TokenBasedAPNs("<key-id>", "<team-id>", "<app-id>", "<key-file-path>");
            //var _result = _apns
            //                .JwtAPNsPush("<device-token>", "Json Web Token(JWT)을 이용한 Apple Push Notification Service", 1, "ping.aiff")
            //                .GetAwaiter()
            //                .GetResult();

            //Console.WriteLine($"result: {_result.result}, '{_result.message}'");

            var _fcm = new PushNotifyFCM("AIzaSyBzkuTXeuK2H17nONqpp64WtqEdNdkKJjE", "");
            _fcm.SendNotification("", "received new message...", "").GetAwaiter().GetResult();

            Console.ReadLine();
        }
    }
}