using System;

namespace DotNet.Push.Core.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var _test_phone = "IX";

            // IPHONE
            if (_test_phone == "IA")
            {
                var _apns = new IosPushNotifyAPNs("<key-id>", "<team-id>", "<app-id>", "<key-file-path>");
                var _result = _apns
                                .JwtAPNsPush("<device-token>", "Json Web Token(JWT): Apple Push Notification Service(APNs)", 1, "<sound>")
                                .GetAwaiter()
                                .GetResult();

                Console.WriteLine($"result: {_result.success}, '{_result.message}'");
            }

            // ANDROID
            if (_test_phone == "AF")
            {
                var _fcm = new DroidPushNotifyFCM("<server-api-key>", "<server-id>", "<alarm-tag>");
                var _result = _fcm
                                .SendNotification("<to>", "<priority>", "<title>", "<click-action>", "<message>", 1, "<icon-name>", "<color>")
                                .GetAwaiter()
                                .GetResult();

                Console.WriteLine($"result: {_result.success}, '{_result.message}'");
            }

            Console.ReadLine();
        }
    }
}