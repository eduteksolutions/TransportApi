namespace TransportApi.Models
{
    public class TransportRouteMaster
    {
        public int rCd { get; set; }
        public string routeCode { get; set; }
        public string routeName { get; set; }
        public string description { get; set; }
        public string loginName { get; set; }
        public DateTime lUserDt { get; set; }
        public int UserID { get; set; }
    }
}
