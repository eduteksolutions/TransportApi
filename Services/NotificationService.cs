using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System.Text;

namespace TransportApi.Services
{


        public class NotificationService
        {
            private readonly IHttpClientFactory _httpClientFactory;

            // Updated with your actual Firebase Project ID
            private const string FirebaseProjectId = "eduschool-a6040";

            // Structured cleanly from your precise Firebase Service Account JSON payload
            private const string FirebaseCredentialsJson = @"
        {
          ""type"": ""service_account"",
          ""project_id"": ""eduschool-a6040"",
          ""private_key_id"": ""83bb3e93a4ba0193a0a2286d41dda470698292ac"",
          ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCv0sofQ34wXtAo\njNOp2+r0oh4ZycFePOLWazCcK/x1gN7mStBJm2JV08YrI82RzVatel2RDPY+q29y\nqiskTU6kj5p69xH3N/Q8Ggj4836uU1RLQJ/E0nbxez322O+JHr12jyxTdAQ6Xw5o\nHGT1gbdYBk+J0uwC84UfwORgGuKKixzxwGCA8h7ykF5dGMMnclVmMIUN9sMSuMs0\nxZHer2NWiHh3HRvqOselJpCkI2usBB/KZA8ZuOQoojKFm1vrPHpM/ZPxGq40Nf88\nCd4JXY431AopOZMo2pVi841hp+eIWSb+9b8YbXLDqsze1O9Ko/P8DtpJmcWfzGj6\nzslq3m4vAgMBAAECggEAN8BiQQmYwOO/o6wMDedjGFEztD+qZiRuVUlF769u7R+B\nDosYgs6Xntl5FALVU29GFdogQzdV+cdInpZg+bDdyAaVGHqxoq+GbWxf/fjP6Z0Y\ntIf4YWPPPYEzw8tEITmzIB4EEy5NjOjJW6/tP3/zUCEoJW7RUZpAUzJJ+ENuJDmy\n3fm0hl9M+zKFlVqCAgHY7uIb5qNFmEUWh7ynYFxBRdL+cjPwNNXgxXORDtLMZnY2\nFdoPciK9KmOAhduUKLXnoEkdnikEu3/T2Hf+fn0LfNg5ULTdjjDjL6FioR9/eohV\nyo3p79/vbiLZwGUz4VU0AfO2gyfamDjEpJ6MCCDhoQKBgQDvPI8qEgFZTanCjVqK\njvEa3HTj2psrICo6b7ogPTMbkI3Pn1yF/yhWIlqYwuOdPyv7xCqajJ/Lf9J1OTT0\nHqvudl6G66FO1eSjyH5hs9ldt4L3Y1xo/NcdjhayqLOZ7IhfPbj4WbdxJO9JlbXd\naYH1VEcLOb6tNuCz4jZ2/d9RHwKBgQC8JLiOAf5xFK099HoeFDnKtb98+UWWy48c\nKSVsUje0j5zC3stC15HzBzvLG6e2wdy0TZLvudNqqkHDwu6BRWZ/XobuerSnEhTO\nLI2Bpl8MUmUtBoLzKsncLNy6Xd3QTMqz+/pPQPkalvSsk2FqkblN3UKvQYvDsQgH\nw4Sdr6rw8QKBgHd0sbe8Ddhdoyp2EoOlJFbZRbSQb3J1OSC8sR9v75kP8P9+/2AZ\nLDb2KG6mwai+5mNsbCmHljKyvI+z5PEQb6E+kayk84K94tC1SAXZG+xjuY11m6sA\ntRkY3dSP0D5HkUCdPUMk6Sgh3SnNC2ZnTHEHiAHe74rmIAUyKyf3VA7ZAoGAOmr4\ndvd9hqUR6aPIayCClUN7kpVfmyPXaroH6atZqpHQ7dz5peaDZb+APF944tEs8zsG\n/bJoR71XsC4NmkvvOGskYFXvU9TUavQAeBotdwj6a1GTRWeqx9eSLbfJkMzXP61k\nmkklT7FBhXg5jjpzHA8wjnQ+rmOAh9CYk9GgtTECgYAoVrOrwJF/slu7z7GvRSd0\nlvYjjksfCfMAGl9X04LiYjua4m6zsMYFNApRUY4gHL4ZuZF0gFacE7qWGzFkT9bY\nCmQWliSMuWLsiHsFUjBmOHH0nCva9jL8nRoO5cKWL3gDNxjvYySgpJkyNCqAg69Y\nx4KnCPxyA2G6yFBcp/p+lw==\n-----END PRIVATE KEY-----\n"",
          ""client_email"": ""eduschool-a6040@appspot.gserviceaccount.com"",
          ""client_id"": ""104601322485996437210"",
          ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
          ""token_uri"": ""https://oauth2.googleapis.com/token"",
          ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
          ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/eduschool-a6040%40appspot.gserviceaccount.com""
        }";

            public NotificationService(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            // =====================================
            // GENERATE OAUTH 2.0 ACCESS TOKEN (NO FILES)
            // =====================================
            public async Task<string> GetAccessTokenAsync()
            {
                try
                {
                    // Convert the raw string configuration safely into an in-memory stream
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(FirebaseCredentialsJson);

                    using (var stream = new MemoryStream(jsonBytes))
                    {
                        var credential = GoogleCredential.FromStream(stream)
                            .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

                        return await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to retrieve FCM access token from embedded credentials: {ex.Message}", ex);
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

