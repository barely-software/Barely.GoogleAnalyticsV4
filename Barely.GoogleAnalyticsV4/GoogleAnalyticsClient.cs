using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Barely.GoogleAnalyticsV4
{
    public class GoogleAnalyticsClient
    {
        private readonly HttpClient _httpClient;
        private string _tid;
        private string _cid;

        public GoogleAnalyticsClient(string tid)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://google-analytics.com")
            };

            _tid = tid;
            _cid = Guid.NewGuid().ToString();
        }

        public async Task Send(string dp)
        {
            dp = HttpUtility.UrlEncode(dp);
            var req = new HttpRequestMessage(HttpMethod.Post, $"/collect?v=1&tid={_tid}&cid={_cid}&dp={dp}");
            await _httpClient.SendAsync(req);
        }
    }
}
