using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace DotNet.Push.Core
{
    public class PushNotifyFCM
    {
        public PushNotifyFCM(string server_apikey, string server_id)
        {
            FCMServerApiKey = server_apikey;
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

        public bool SendNotification(string title, string message, string topic)
        {
            try
            {
                this.Successful = true;
                this.Error = null;

                var _request_uri = "https://fcm.googleapis.com/fcm/send";

                var _web_request = WebRequest.Create(_request_uri);
                _web_request.Method = "POST";
                _web_request.Headers.Add(string.Format("Authorization: key={0}", FCMServerApiKey));
                _web_request.Headers.Add(string.Format("Sender: id={0}", FCMServerId));
                _web_request.ContentType = "application/json";

                var _data = new
                {
                    // to = YOUR_FCM_DEVICE_ID, // Uncoment this if you want to test for single device
                    to = "/topics/" + topic, // this is for topic 
                    notification = new
                    {
                        title = title,
                        body = message,
                        //icon="myicon"
                    }
                };

                var _post_data = JsonConvert.SerializeObject(_data);
                var _post_byte = Encoding.UTF8.GetBytes(_post_data);

                _web_request.ContentLength = _post_byte.Length;
                using (var _data_stream = _web_request.GetRequestStream())
                {
                    _data_stream.Write(_post_byte, 0, _post_byte.Length);

                    using (var _web_response = _web_request.GetResponse())
                    {
                        using (var _response = _web_response.GetResponseStream())
                        {
                            using (var _reader = new StreamReader(_response))
                            {
                                var _responseFromServer = _reader.ReadToEnd();
                                this.Response = _responseFromServer;
                            }
                        }
                    }
                }
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