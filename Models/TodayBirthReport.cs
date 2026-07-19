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
        public string? AClass { get; set; }
        public string? StrPwd { get; set; }
        public string? RollNo { get; set; }
        public string? FName { get; set; }
        public string? MName { get; set; }
        public string? Address { get; set; }
        public string? ContactNo { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ICardPhoto { get; set; }
    }
}
