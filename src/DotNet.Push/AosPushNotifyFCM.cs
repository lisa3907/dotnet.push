using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotNet.Push
{
    public class AosPushNotifyFCM
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="serverKey"></param>
        /// <param name="serverId"></param>
        /// <param name="alarmTag">Android의 알림 창에 각 알림이 새로 입력되는지 여부를 나타냅니다.</param>
        public AosPushNotifyFCM(string serverKey, string serverId, string alarmTag)
        {
            ServerKey = serverKey;
            ServerId = serverId;
            AlarmTag = alarmTag;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="priority">high,normal</param>
        /// <param name="title"></param>
        /// <param name="clickAction"></param>
        /// <param name="message"></param>
        /// <param name="badge"></param>
        /// <param name="iconName"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public async Task<(bool success, string message)> SendNotificationAsync(string deviceToken, string priority, string title, string clickAction, string message, int badge, string iconName, string color)
        {
            var result = (success: false, message: "ok");

            try
            {
                var pusher = new AosPushProtocol
                {
                    to = deviceToken,
                    priority = priority,

                    notification = new AosNotification
                    {
                        title = title,
                        click_action = clickAction,

                        body = message,
                        icon = iconName,
                        color = color,
                        tag = AlarmTag
                    },
                    data = new AosNotifyData
                    {
                        title = title,
                        badge = badge,
                        message = message
                    }
                };

                var content = JsonSerializer.Serialize(pusher);

                using (var http_client = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri("https://fcm.googleapis.com/fcm/send"),
                        Method = HttpMethod.Post,
                        Content = new StringContent(content, Encoding.UTF8, "application/json")
                    };

                    request.Headers.Authorization = new AuthenticationHeaderValue("key", "=" + ServerKey);
                    request.Headers.Add("Sender", "id=" + ServerId);

                    var response = await http_client.SendAsync(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        result.message = $"success";
                        result.success = true;
                    }
                    else
                    {
                        result.message = $"{response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                result.message = $"exception: '{ex.Message}'";
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        public string ServerKey
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public string ServerId
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public string AlarmTag
        {
            get;
            set;
        }
    }
}