namespace TransportApi.Models
{
    public class VehicleStudentAttendance
    {
        public int AttendanceID { get; set; }
        public int AdmCd { get; set; }
        public int VehicleID { get; set; }
        public int? RouteID { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? DropTime { get; set; }

        public string AttendanceStatus { get; set; }

        public string LoginName { get; set; }
        public int UserID { get; set; }
    }
}
