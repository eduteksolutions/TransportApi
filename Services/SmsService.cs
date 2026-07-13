
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
           
        }

    
    }

}
