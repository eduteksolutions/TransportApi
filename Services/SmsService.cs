using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Net.Http;

namespace TransportApi.Services
{
    public class SmsService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public SmsService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task SendSmsAsyncTwilop(string toPhoneNumber, string message)
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromPhoneNumber = _configuration["Twilio:FromPhoneNumber"];

            TwilioClient.Init(accountSid, authToken);

            await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(fromPhoneNumber),
                to: new PhoneNumber(toPhoneNumber)
            );
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            var username = _configuration["SmsSettings:Username"];
            var password = _configuration["SmsSettings:Password"];
            var senderId = _configuration["SmsSettings:SenderId"];
            var entityId = _configuration["SmsSettings:DltPrincipalEntityId"];
            var templateId = _configuration["SmsSettings:DltContentId"];

            var url =
                $"https://sms.omnitechintegrators.com/fe/api/v1/multiSend" +
                $"?username={Uri.EscapeDataString(username)}" +
                $"&password={Uri.EscapeDataString(password)}" +
                $"&unicode=false" +
                $"&from={Uri.EscapeDataString(senderId)}" +
                $"&to={Uri.EscapeDataString(toPhoneNumber)}" +
                $"&dltContentId={Uri.EscapeDataString(templateId)}" +
                $"&dltPrincipalEntityId={Uri.EscapeDataString(entityId)}" +
                $"&text={Uri.EscapeDataString(message)}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"SMS sending failed. Status Code: {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync();

            // Optional: Log or return the response
            Console.WriteLine(result);
        }
    }

}
