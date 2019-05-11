using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Push
{
    public class DroidPushNotifyFCM
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="server_api_key"></param>
        /// <param name="server_id"></param>
        /// <param name="alarm_tag">Android의 알림 창에 각 알림이 새로 입력되는지 여부를 나타냅니다.</param>
        public DroidPushNotifyFCM(string server_api_key, string server_id, string alarm_tag)
        {
            FCMServerApiKey = server_api_key;
            FCMServerId = server_id;

            AlarmTag = alarm_tag;
        }

        /// <summary>
        ///
        /// </summary>
        public string FCMServerApiKey
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public string FCMServerId
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="device_token"></param>
        /// <param name="priority">high,normal</param>
        /// <param name="title"></param>
        /// <param name="click_action"></param>
        /// <param name="message"></param>
        /// <param name="badge"></param>
        /// <param name="icon_name"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public async Task<(bool success, string message)> SendNotification(string device_token, string priority, string title, string click_action, string message, int badge, string icon_name, string color)
        {
            var _result = (success: false, message: "ok");

            try
            {
                var _pusher = new DroidPushProtocol();
                {
                    _pusher.to = device_token;
                    _pusher.priority = priority;

                    _pusher.notification = new DroidNotification();
                    {
                        _pusher.notification.title = title;
                        _pusher.notification.click_action = click_action;

                        _pusher.notification.body = message;
                        _pusher.notification.icon = icon_name;
                        _pusher.notification.color = color;
                        _pusher.notification.tag = AlarmTag;

                        _pusher.data = new DroidNotifyData();
                        {
                            _pusher.data.title = title;
                            _pusher.data.badge = badge;
                            _pusher.data.message = message;
                        }
                    }
                }

                var _content = JsonConvert.SerializeObject(_pusher);

                using (var _http_client = new HttpClient())
                {
                    var _request = new HttpRequestMessage
                    {
                        RequestUri = new Uri("https://fcm.googleapis.com/fcm/send"),
                        Method = HttpMethod.Post,
                        Content = new StringContent(_content, Encoding.UTF8, "application/json")
                    };

                    _request.Headers.Authorization = new AuthenticationHeaderValue("key", "=" + FCMServerApiKey);
                    _request.Headers.Add("Sender", "id=" + FCMServerId);

                    var _response = await _http_client.SendAsync(_request);
                    if (_response.StatusCode == HttpStatusCode.OK)
                        _result.success = true;

                    _result.message = _response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                _result.message = ex.Message;
            }

            return _result;
        }
    }
}