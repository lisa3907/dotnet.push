using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using HttpTwo;
using Jose;
using Newtonsoft.Json.Linq;

namespace JwtAPNs
{
    public class TokenBasedAPNs
    {
        private string Algorithm { get; } = "ES256";

        private string HostUrl { get; } = "api.development.push.apple.com";
        private int HostPort { get; } = 443;

        private string APNsKeyId { get; set; }
        private string TeamId { get; set; }

        private string BundleAppId { get; set; }
        private CngKey PrivateKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key_id"></param>
        /// <param name="team_id"></param>
        /// <param name="app_id"></param>
        /// <param name="auth_key_path"></param>
        public TokenBasedAPNs(string key_id, string team_id, string app_id, string auth_key_path)
        {
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
        private async Task<(bool result, string message)> JwtAPNsPush(Uri host_uri, string access_token, byte[] payload_bytes)
        {
            var _result = false;
            var _message = "";

            try
            {
                using (var _handler = new Http2MessageHandler())
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

                                _message = $"success: '{_response_uuid}'";
                                _result = true;
                            }
                            else
                                _message = "failure";
                        }
                        else
                        {
                            var _response_body = await _response_message.Content.ReadAsStringAsync();
                            var _response_json = JObject.Parse(_response_body);

                            var _reason_str = _response_json.Value<string>("reason");
                            _message = $"failure: '{_reason_str}'";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _message = $"exception: '{ex.Message}'";
            }

            return (_result, _message);
        }

        /// <summary>
        /// Token Based Authentication APNs 푸시
        /// </summary>
        /// <param name="device_token">device token</param>
        /// <param name="message">푸시 메시지</param>
        /// <param name="badge">배지 번호</param>
        /// <param name="sound">사운드 파일 이름</param>
        public async Task<(bool result, string message)> JwtAPNsPush(string device_token, string message, int badge, string sound)
        {
            var _host_uri = new Uri(string.Format("https://{0}:{1}/3/device/{2}", HostUrl, HostPort, device_token));

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