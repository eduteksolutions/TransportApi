using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System.Text;

namespace TransportApi.Services
{
    public class NotificationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string FirebaseProjectId = "schoolapp-3410c";

        public NotificationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // =====================================
        // GENERATE OAUTH 2.0 ACCESS TOKEN
        // =====================================
        public async Task<string> GetAccessTokenAsync()
        {
            try
            {
                string jsonPath = Path.Combine(AppContext.BaseDirectory, "schoolapp-service-account.json");

                if (!File.Exists(jsonPath))
                {
                    throw new FileNotFoundException("Firebase credentials file missing from application folder.", jsonPath);
                }

                using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                {
                    var credential = GoogleCredential.FromStream(stream)
                        .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

                    return await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve FCM access token: {ex.Message}", ex);
            }
        }

        // =====================================
        // SEND PUSH NOTIFICATION (FCM v1 HTTP)
        // =====================================
        public async Task<string> SendMessageAsync(string deviceToken, string title, string body)
        {
            string accessToken = await GetAccessTokenAsync();
            string uniqueId = Guid.NewGuid().ToString();

            using (var client = _httpClientFactory.CreateClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var payload = new
                {
                    message = new
                    {
                        token = deviceToken,
                        notification = new { title, body },
                        android = new
                        {
                            notification = new
                            {
                                sound = "default",
                                channel_id = "high_importance_channel",
                                tag = uniqueId
                            }
                        },
                        apns = new
                        {
                            headers = new { apns_priority = "10" },
                            payload = new
                            {
                                aps = new
                                {
                                    alert = new { title, body },
                                    sound = "default"
                                }
                            }
                        },
                        data = new
                        {
                            click_action = "FLUTTER_NOTIFICATION_CLICK",
                            messageId = Guid.NewGuid().ToString()
                        }
                    }
                };

                var jsonMessage = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                string url = $"https://fcm.googleapis.com/v1/projects/{FirebaseProjectId}/messages:send";

                var response = await client.PostAsync(url, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
