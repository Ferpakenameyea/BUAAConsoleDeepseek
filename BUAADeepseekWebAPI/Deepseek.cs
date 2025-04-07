using BUAADeepseekWebAPI.Credentials;
using BUAADeepseekWebAPI.Exceptions;
using Newtonsoft.Json;
using System.Text;

namespace BUAADeepseekWebAPI
{
    public sealed class BUAADeepseek
    {
        private readonly ICredentialProvider _credentialProvider;
        private readonly HttpClient _httpClient = new();
        private readonly IContextManager _contextManager;

        public BUAADeepseek(
            ICredentialProvider credentialProvider,
            IContextManager contextManager)
        {
            _credentialProvider = credentialProvider;
            _contextManager = contextManager;
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

            var form = new MultipartFormDataContent
            {
                { new StringContent(content), "content" }
            };

            if (useContext)
            {
                int index = 0;
                foreach (var (user, system) in this._contextManager.Enumerate())
                {
                    form.Add(new StringContent(user.Role), $"history[{index}][role]");
                    form.Add(new StringContent(user.Content), $"history[{index}][content]");
                    index++;
                    form.Add(new StringContent(system.Role), $"history[{index}][role]");
                    form.Add(new StringContent(system.Content), $"history[{index}][content]");
                    index++;
                }
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

            if (useContext)
            {
                _contextManager.AddHistory((
                    new History() { Role = Roles.USER, Content = content },
                    new History() { Role = Roles.ASSISTANT, Content = builder.ToString() }));
            }
        }
    }
}
