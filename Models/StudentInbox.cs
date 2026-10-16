namespace TransportApi.Models
{
    public class StudentInbox
    {
        public int AdmCd { get; set; }
        public string MsgText { get; set; }
        public string? ImgURL { get; set; }
        public string? DownLink { get; set; }          // <-- Add '?' here
        public string? Description { get; set; }       // <-- Add '?' here
        public DateTime TDate { get; set; }
        public int UserID { get; set; }
        public int? ClassCd { get; set; }
        public int? SecCd { get; set; }
        public string? URL_Date_Sheet { get; set; }    // <-- Add '?' here
        public int ToFaculty { get; set; }
        public string? IsRead { get; set; }
    }
}
