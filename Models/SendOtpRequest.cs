namespace TransportApi.Models
{
    public class SendOtpRequest
    {
        public string MobileNo { get; set; }
        public string UserID { get; set; }

        public string UserType { get; set; }

    }
}
