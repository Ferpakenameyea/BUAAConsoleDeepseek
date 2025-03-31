using BUAADeepseekWebAPI.Credentials;
using BUAADeepseekWebAPI.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Web;

namespace BUAADeepseekWebAPI
{
    public sealed class BUAADeepseek
    {
        private readonly ICredentialProvider _credentialProvider;
        private readonly HttpClient _httpClient = new();
        private readonly LinkedList<History> histories = new();
        private readonly int maxContext;

        public BUAADeepseek(
            ICredentialProvider credentialProvider,
            int maxContext=50)
        {
            _credentialProvider = credentialProvider;
            this.maxContext = maxContext;
        }

        private async Task<Response<T>> AccessWithCredential<T>(HttpRequestMessage request)
        {
            request.Headers.Add("Cookie", (await _credentialProvider.GetCredentialAsync()).Cookie);
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Response<T>>(content);
            return responseObject ?? throw new OutdatedException($"[uri: {request.RequestUri}]");
        }

        public async Task<UserInfo> GetUserInfoAsync()
        {
            var uri = new UriBuilder(DeepseekURL.BASE + DeepseekURL.USER_INFO).Uri;
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            var responseObject = await AccessWithCredential<UserInfo>(request);

            return responseObject == null ? throw new OutdatedException("user-info") : responseObject.Data!;
        }

        public async Task ChatAsync(
            string content,
            Action<Response<AIChatResponse>> onDataReceive,
            bool useContext=true,
            bool deepSearch=false, 
            bool internetSearch=false)
        {
            StringBuilder builder = new();
            var request = new HttpRequestMessage(HttpMethod.Post, DeepseekURL.BASE + DeepseekURL.COMPOSE_CHAT);

            var form = new MultipartFormDataContent();
            form.Add(new StringContent(content), "content");

            int index = 0;
            foreach (var history in this.histories)
            {
                form.Add(new StringContent(history.Role), $"history[{index}][role]");
                form.Add(new StringContent(history.Content), $"history[{index}][content]");
            }

            form.Add(new StringContent("2"), "compose_id");
            form.Add(new StringContent(deepSearch ? "1" : "2"), "deep_search");
            form.Add(new StringContent(internetSearch ? "1" : "2"), "internet_search");

            request.Content = form;

            request.Headers.Add("Cookie", (await _credentialProvider.GetCredentialAsync()).Cookie);

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while(!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrEmpty(line))
                {
                    var chatResponse = JsonConvert.DeserializeObject<Response<AIChatResponse>>(
                        line["data: ".Length..]);
                    onDataReceive(chatResponse!);
                    builder.Append(chatResponse!.Data!.Answer);
                }
            }

            histories.AddLast(new History() { Role = Roles.USER, Content = content }); 
            histories.AddLast(new History() { Role = Roles.ASSISTANT, Content = builder.ToString() });

            while (histories.Count > this.maxContext)
            {
                histories.RemoveFirst();
            }
        }
    }
}
