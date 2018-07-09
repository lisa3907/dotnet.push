using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNet.Push
{
    /// <summary>
    /// 
    /// </summary>
    public class Http2CustomHandler : WinHttpHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            request.Version = new Version("2.0");
            return base.SendAsync(request, cancellationToken);
        }
    }
}