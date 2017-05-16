using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Jose;
using Newtonsoft.Json.Linq;

namespace DotNet.Push.Core
{
    public class IosPushNotifyAPNs
    {
        public string Algorithm { get; }

        public string HostServerUrl { get; }
        public int HostPort { get; }

        public string APNsKeyId { get; }
        public string TeamId { get; }

        public string BundleAppId { get; }
        public CngKey PrivateKey { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key_id"></param>
        /// <param name="team_id"></param>
        /// <param name="app_id"></param>
        /// <param name="auth_key_path"></param>
        /// <param name="production"></param>
        public IosPushNotifyAPNs(string key_id, string team_id, string app_id, string auth_key_path, string algorithm = "ES256", bool production = false, int port = 443)
        {
            Algorithm = algorithm;
            if (production == false)
                HostServerUrl = "api.development.push.apple.com";
            else
                HostServerUrl = "api.push.apple.com";

            HostPort = port;

            APNsKeyId = key_id;
            TeamId = team_id;
            BundleAppId = app_id;

            // 다운로드 읽기 암호화 된 개인 키 (.p8)
            var _private_key_content = System.IO.File.ReadAllText(auth_key_path);
            var _private_key = _private_key_content.Split('\n')[1];

            var _secret_key_blob = Convert.FromBase64String(_private_key);
            PrivateKey = CngKey.Import(_secret_key_blob, CngKeyBlobFormat.Pkcs8PrivateBlob);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        ///<returns>Date converted to seconds since Unix epoch(Jan 1, 1970, midnight UTC).</returns>        
        private long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host_uri"></param>
        /// <param name="access_token"></param>
        /// <param name="payload_bytes"></param>
        /// <returns></returns>
        private async Task<(bool success, string message)> JwtAPNsPush(Uri host_uri, string access_token, byte[] payload_bytes)
        {
            var _result = (success: false, message: "ok");

            try
            {
                using (var _handler = new Http2CustomHandler())
                {
                    using (var _http_client = new HttpClient(_handler))
                    {
                        var _request_message = new HttpRequestMessage();
                        {
                            _request_message.RequestUri = host_uri;
                            _request_message.Headers.Add("authorization", string.Format("bearer {0}", access_token));
                            _request_message.Headers.Add("apns-id", Guid.NewGuid().ToString());
                            _request_message.Headers.Add("apns-expiration", "0");
                            _request_message.Headers.Add("apns-priority", "10");
                            _request_message.Headers.Add("apns-topic", BundleAppId);
                            _request_message.Method = HttpMethod.Post;
                            _request_message.Content = new ByteArrayContent(payload_bytes);
                        }

                        var _response_message = await _http_client.SendAsync(_request_message);
                        if (_response_message.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var _response_uuid = "";

                            IEnumerable<string> values;
                            if (_response_message.Headers.TryGetValues("apns-id", out values))
                            {
                                _response_uuid = values.First();

                                _result.message = $"success: '{_response_uuid}'";
                                _result.success = true;
                            }
                            else
                                _result.message = "failure";
                        }
                        else
                        {
                            var _response_body = await _response_message.Content.ReadAsStringAsync();
                            var _response_json = JObject.Parse(_response_body);

                            var _reason_str = _response_json.Value<string>("reason");
                            _result.message = $"failure: '{_reason_str}'";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _result.message = $"exception: '{ex.Message}'";
            }

            return _result;
        }

        /// <summary>
        /// Token Based Authentication APNs push
        /// </summary>
        /// <param name="device_token">device token</param>
        /// <param name="message">push message</param>
        /// <param name="badge">badge number</param>
        /// <param name="sound">sound file name</param>
        /// <param name="production">development or production</param>
        public async Task<(bool success, string message)> JwtAPNsPush(string device_token, string message, int badge, string sound)
        {
            var _host_uri = new Uri($"https://{HostServerUrl}:{HostPort}/3/device/{device_token}");

            var _access_token = "";
            {
                var payload = new Dictionary<string, object>()
                    {
                        { "iss", TeamId },
                        { "iat", ToUnixEpochDate(DateTime.Now) }
                    };

                var header = new Dictionary<string, object>()
                    {
                        { "alg", Algorithm },
                        { "kid", APNsKeyId }
                    };

                _access_token = Jose.JWT.Encode(payload, PrivateKey, JwsAlgorithm.ES256, header);
            }

            var _payload = new byte[0];
            {
                var _data = JObject.FromObject(new
                {
                    aps = new
                    {
                        alert = message,
                        badge = badge,
                        sound = sound
                    }
                });

                _payload = System.Text.Encoding.UTF8.GetBytes(_data.ToString());
            }

            return await JwtAPNsPush(_host_uri, _access_token, _payload);
        }
    }
}