using System;

namespace DotNet.Push.Core.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // IPHONE
            {
                var _apns = new TokenBasedAPNs("<key-id>", "<team-id>", "<app-id>", "<key-file-path>");
                var _result = _apns
                                .JwtAPNsPush("<device-token>", "Json Web Token(JWT)을 이용한 Apple Push Notification Service", 1, "ping.aiff")
                                .GetAwaiter()
                                .GetResult();

                Console.WriteLine($"result: {_result.success}, '{_result.message}'");
            }

            // ANDROID
            {
                var _fcm = new PushNotifyFCM("<server-api-key>", "<server-id>");
                var _result = _fcm
                                .SendNotification("<to>", "<title>", "<message>", 0)
                                .GetAwaiter()
                                .GetResult();

                Console.WriteLine($"result: {_result.success}, '{_result.message}'");
            }

            Console.ReadLine();
        }
    }
}