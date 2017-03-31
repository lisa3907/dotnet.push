using System;

namespace DotNet.Push.Sample
{
    public class Program
    {
        static void Main(string[] args)
        {
            var _apns = new IosPushNotifyAPNs("<key-id>", "<team-id>", "<app-id>", "<key-file-path>");
            var _result = _apns
                            .JwtAPNsPush("<device-token>", "Json Web Token(JWT): Apple Push Notification Service(APNs)", 1, "<sound>")
                            .GetAwaiter()
                            .GetResult();

            Console.WriteLine($"result: {_result.success}, '{_result.message}'");
            Console.ReadLine();
        }

    }
}