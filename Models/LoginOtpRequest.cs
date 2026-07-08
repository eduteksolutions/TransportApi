namespace TransportApi.Models
{
    public class LoginOtpRequest
    {
        public string MobileNo { get; set; }
        public string OTP { get; set; }
        public int UserId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
    }
}
