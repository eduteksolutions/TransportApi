namespace TransportApi.Models
{
    public class UpdateStationRequest
    {
        public int UserId { get; set; }
        public int Code { get; set; }
        public int AdmCd { get; set; }
        public int PickupStationCd { get; set; }
        public int DropStationCd { get; set; }

    }
}
