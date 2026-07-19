using Microsoft.EntityFrameworkCore;

namespace TransportApi.Models
{
    [Keyless]
    public class TodayBirthReport
    {
        public int AdmCd { get; set; }
        public string? Name { get; set; }
        public string? ClassName { get; set; }
        public string? SecName { get; set; }
        public string? Pic { get; set; }
        public int? AClass { get; set; }      // int because SQL value is 18
        public string? StrPwd { get; set; }
        public int? RollNo { get; set; }      // int because SQL value is 3
        public string? FName { get; set; }
        public string? MName { get; set; }
        public string? Address { get; set; }
        public string? ContactNo { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ICardPhoto { get; set; }
        public string? mbl_Device_ID { get; set; }

    }
}
