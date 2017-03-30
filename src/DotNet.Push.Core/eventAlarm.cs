using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace DotNet.Push.Core
{
    public class PushNotifyFCM2
    {
        public bool EventAlarm(string p_to, string p_title, string p_text, int p_badge)
        {
            // Create a request for the URL. 
            var request = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            request.Headers.Add("Authorization", "key=" + __cconfig.PushKey);
            request.ContentType = "application/json";

            var _push = new PushProtocol();
            {
                _push.to = p_to;
                _push.priority = "high";
                _push.notification = new Notification();
                _push.notification.title = p_title;
                _push.notification.click_action = "OPEN_ACTIVITY_1";
                _push.notification.body = p_text;

                _push.data = new NotifiData();
                _push.data.badge = p_badge;
                _push.data.title = p_title;
                _push.data.text = p_text;
                _push.data.sub = new SubData();
                _push.data.sub.sub = "sub test";
            }

            var postData = JsonConvert.SerializeObject(_push);
            var byteArray = Encoding.UTF8.GetBytes(postData);

            request.Method = "POST";
            request.ContentLength = byteArray.Length;
            
            // Get the request stream.
            var requestData = request.GetRequestStream();
            // Write the data to the request stream.
            requestData.Write(byteArray, 0, byteArray.Length);
            
            // Close the Stream object.
            //requestData.Close();

            // Get the response.
            var response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.
            var responseStream = response.GetResponseStream();
            
            // Open the stream using a StreamReader for easy access.
            var reader = new StreamReader(responseStream);
            
            // Read the content.
            var responseFromServer = reader.ReadToEnd();
            
            // Display the content.
            // Clean up the streams and the response.
            //reader.Close();
            response.Close();

            return ((HttpWebResponse)response).StatusCode == HttpStatusCode.OK;
        }
    }
}