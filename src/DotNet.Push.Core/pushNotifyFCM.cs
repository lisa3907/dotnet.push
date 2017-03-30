using System;
using System.Net;
using System.Net.Http;
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

        public bool Successful
        {
            get;
            set;
        }

        public string Response
        {
            get;
            set;
        }
        public Exception Error
        {
            get;
            set;
        }

        public async Task<bool> SendNotification(string title, string message, string topic)
        {
            try
            {
                //var _data = new
                //{
                //    // to = YOUR_FCM_DEVICE_ID, // Uncoment this if you want to test for single device
                //    to = "/topics/" + topic, // this is for topic 
                //    notification = new
                //    {
                //        title = title,
                //        body = message,
                //        //icon="myicon"
                //    }
                //};

                var _push = new PushProtocol();
                _push.to = "c1Rp3_jocws:APA91bHh5cpODs_F95hNYRx57fXXBS0efySmWsGE8XLlafvrMeEsLBN9KfkbjaGBxkHcqw1itG493Iwr2gXfzoL2grz8BaE80IpGWHr-xsHESCRSQs2382bdb2kB5SCNeSr8dyu4R2uO";
                _push.priority = "high";
                _push.notification = new Notification();
                _push.notification.title = "IDTECKM3S";
                _push.notification.click_action = "OPEN_ACTIVITY_1";
                _push.notification.body = "새로운 이벤트가 발생되었습니다.";
                _push.notification.icon = "alarm";
                _push.notification.color = "#d32121";

                _push.data = new NotifiData();
                _push.data.badge = 1;
                _push.data.title = "token test";

                _push.data.sub = new SubData();
                _push.data.sub.sub = "sub test";

                var _content = JsonConvert.SerializeObject(_push);

                var _request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send");
                {
                    _request.Headers.Add("Authorization", $"key {FCMServerApiKey}");
                    _request.Headers.Add("Sender", $"id {FCMServerId}");
                    _request.Headers.Add("ContentType", $"application/json");
                    _request.Content = new StringContent(_content);
                }

                var _client = new HttpClient();
                var _response = await _client.SendAsync(_request);

                this.Successful = _response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                this.Successful = false;

                this.Response = null;
                this.Error = ex;
            }

            return this.Successful;
        }
    }
}