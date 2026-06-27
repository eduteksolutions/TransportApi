namespace TransportApi.Models
{
    
    public class TransportVehicleMaster
    {
        public int vCd { get; set; }
        public int vTypeCode { get; set; }
        public string vNo { get; set; }
        public int Capacity { get; set; }
        public DateTime purchDt { get; set; }
        public decimal purchCost { get; set; }
        public string BillNo { get; set; }
        public DateTime BillDate { get; set; }
        public string Model { get; set; }
        public string Milage { get; set; }
        public string ChasisNo { get; set; }
        public string purchFrom { get; set; }
        public string Add2 { get; set; }
        public string Add3 { get; set; }
        public string vDescr { get; set; }
        public string picPath { get; set; }
        public string LoginName { get; set; }
        public int UserID { get; set; }

        public string DeviceID { get; set; }
    }
}
