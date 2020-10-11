using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Barely.GoogleAnalyticsV4
{

    /// <summary>
    /// A very simple wrapper around the google analytics measurement protocol.
    /// </summary>
    public class GoogleAnalytics
    {
        private readonly HttpClient _httpClient;
        private string _tid;
        private string _cid;
        private GoogleAnalyticsProtocolVersion _protocolVersion;
        private string _basePayload;

        /// <summary>
        /// Create a new google analytics client.
        /// </summary>
        /// <param name="tid">required: The google analytics Tracking ID (e.g. UA-XXXXXX-Y)</param>
        /// <param name="cid">optional: A unique client id to identify the user (optional. If not provided one is generated.</param>
        /// <param name="protocolVersion">optional: Version of the google analytics measurement protocol being used (default: V1)</param>
        public GoogleAnalytics(string tid, string cid = null, GoogleAnalyticsProtocolVersion protocolVersion = GoogleAnalyticsProtocolVersion.V1)
        {
            if (!IsValidTid(tid))
                throw new ArgumentException($"{nameof(GoogleAnalytics)}: Attempted to create GoogleAnalytics client with invalid tracking id, {tid}");

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://google-analytics.com")
            };

            _tid = tid;
            _cid = string.IsNullOrEmpty(cid) ? Guid.NewGuid().ToString() : cid;
            _protocolVersion = protocolVersion;
            _basePayload = $"/collect?v={(int)_protocolVersion}&tid={_tid}&cid={_cid}";
        }

        /// <summary>
        /// Send a Pageview
        /// </summary>
        /// <param name="dp">required: Document page (e.g. /home).</param>
        /// <param name="dh">optional: Document hostname (e.g. example.com)</param>
        /// <param name="dt">optional: Document title (e.g. homepage)</param>
        /// <returns></returns>
        public async Task SendPageview(string dp, string dh, string dt)
        {
            if (string.IsNullOrEmpty(dp))
                throw new ArgumentException($"{nameof(SendPageview)}: document page (dp) required.");


            if (!dp.StartsWith("/"))
                dp = "/" + dp;

            var pageviewPayload = $"t=pageview";
            pageviewPayload += $"&dp={HttpUtility.UrlEncode(dp)}";
            pageviewPayload += string.IsNullOrEmpty(dh) ? string.Empty : $"&dh={HttpUtility.UrlEncode(dh)}";
            pageviewPayload += string.IsNullOrEmpty(dt) ? string.Empty : $"&dt={HttpUtility.UrlEncode(dt)}";

            var payload = _basePayload + "&" + pageviewPayload;

            var req = new HttpRequestMessage(HttpMethod.Post, payload);
            var rsp = await _httpClient.SendAsync(req);
            rsp.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Send an Event
        /// </summary>
        /// <param name="ec">required: Event category (e.g. video)</param>
        /// <param name="ea">required: Event action (e.g. /play).</param>
        /// <param name="el">optional: Event label (e.g. holiday)</param>
        /// <param name="ev">optional: Event value (e.g. 300)</param>
        /// <returns></returns>
        public async Task SendEvent(string ec, string ea, string el, string ev)
        {
            if (string.IsNullOrEmpty(ec))
                throw new ArgumentException($"{nameof(SendEvent)}: event category (ec) required.");

            if (string.IsNullOrEmpty(ea))
                throw new ArgumentException($"{nameof(SendEvent)}: event action (ea) required.");

            var eventPayload = $"t=event";
            eventPayload += $"&ec={HttpUtility.UrlEncode(ec)}";
            eventPayload += $"&ea={HttpUtility.UrlEncode(ea)}";
            eventPayload += string.IsNullOrEmpty(el) ? string.Empty : $"&el={HttpUtility.UrlEncode(el)}";
            eventPayload += string.IsNullOrEmpty(ev) ? string.Empty : $"&ev={HttpUtility.UrlEncode(ev)}";

            var req = new HttpRequestMessage(HttpMethod.Post, _basePayload + "&" + eventPayload);
            var rsp = await _httpClient.SendAsync(req);
            rsp.EnsureSuccessStatusCode();
        }

        private static Regex _validTid = new Regex(@"\bUA-\d{4,10}-\d{1,4}\b", RegexOptions.Compiled);
        public static bool IsValidTid(string tid)
        {
            return _validTid.IsMatch(tid);
        }

    }
}
