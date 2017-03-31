using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNet.Push.Core
{
    public class PushNotifyFCM
    {
        public PushNotifyFCM(string server_api_key, string server_id)
        {
            FCMServerApiKey = server_api_key;
            FCMServerId = server_id;
        }

        public string FCMServerApiKey
        {
            get;
            set;
        }

        public string FCMServerId
        {
            get;
            set;
        }

        public async Task<(bool success, string message)> SendNotification(string device_token, string title, string message, int badge)
        {
            (bool success, string message) _result = (false, "ok");

            try
            {
                var _pusher = new PushProtocol();
                {
                    _pusher.to = device_token;
                    _pusher.priority = "high";

                    _pusher.notification = new Notification();
                    {
                        _pusher.notification.title = title;
                        _pusher.notification.click_action = "PUSH_EVENT_ALARM";
                        _pusher.notification.body = message;
                        _pusher.notification.icon = "alarm";
                        _pusher.notification.color = "#d32121";
                        _pusher.notification.tag = "PUSH_EVENT_ALARM";

                        _pusher.data = new NotifiData();
                        {
                            _pusher.data.title = title;
                            _pusher.data.badge = badge;
                            _pusher.data.text = message;
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

                    _request.Headers.Authorization = new AuthenticationHeaderValue("key" , "=" + FCMServerApiKey);
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