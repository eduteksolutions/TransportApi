using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TransportApi.Models
{

    [Table("Admission")]
    public class Admission
    {
        [Column("admSCode")]
        [StringLength(50)]
        public string? AdmSCode { get; set; }

        [Key] // Assuming this is your primary key based on previous contexts
        [Column("admCd")]
        public int? AdmCd { get; set; }

        [Column("rollNo")]
        public int? RollNo { get; set; }

        [Column("admDate")]
        public DateTime? AdmDate { get; set; }

        [Column("regCd")]
        public int? RegCd { get; set; }

        [Column("regDate")]
        public DateTime? RegDate { get; set; }

        [Column("lastName")]
        [StringLength(200)]
        public string? LastName { get; set; }

        [Column("middleName")]
        [StringLength(200)]
        public string? MiddleName { get; set; }

        [Column("firstName")]
        [StringLength(200)]
        public string? FirstName { get; set; }

        [Column("nickName")]
        [StringLength(200)]
        public string? NickName { get; set; }

        [Column("Sex")]
        [StringLength(1)]
        public string? Sex { get; set; }

        [Column("birthDate")]
        public DateTime? BirthDate { get; set; }

        [Column("ambition")]
        [StringLength(200)]
        public string? Ambition { get; set; }

        [Column("identityMark")]
        [StringLength(200)]
        public string? IdentityMark { get; set; }

        [Column("infoMedia")]
        public int? InfoMedia { get; set; }

        [Column("Category")]
        public int? Category { get; set; }

        [Column("caste")]
        public int? Caste { get; set; }

        [Column("bloodCd")]
        public int? BloodCd { get; set; }

        [Column("Nationality")]
        public int? Nationality { get; set; }

        [Column("Religion")]
        public int? Religion { get; set; }

        [Column("Quota")]
        public int? Quota { get; set; }

        [Column("PrevClass")]
        public int? PrevClass { get; set; }

        [Column("marksPer")]
        public double? MarksPer { get; set; } // float maps to double in C#

        [Column("grade")]
        [StringLength(10)]
        public string? Grade { get; set; }

        [Column("sLanguage")]
        public int? SLanguage { get; set; }

        [Column("prevSchool")]
        [StringLength(500)]
        public string? PrevSchool { get; set; }

        [Column("reasonForLeaving")]
        [StringLength(500)]
        public string? ReasonForLeaving { get; set; }

        [Column("hostel")]
        [StringLength(500)]
        public string? Hostel { get; set; }

        [Column("trans")]
        [StringLength(5000)]
        public string? Trans { get; set; }

        [Column("transCd")]
        public int? TransCd { get; set; }

        [Column("tMode")]
        [StringLength(20)]
        public string? TMode { get; set; }

        [Column("VehicleNo")]
        [StringLength(500)]
        public string? VehicleNo { get; set; }

        [Column("HealthDetail")]
        public string? HealthDetail { get; set; } // length -1 indicates varchar(max)

        [Column("firstAid")]
        public string? FirstAid { get; set; } // length -1 indicates varchar(max)

        [Column("handiCapped")]
        [StringLength(1)]
        public string? HandiCapped { get; set; }

        [Column("handiDesc")]
        [StringLength(100)]
        public string? HandiDesc { get; set; }

        [Column("handiPer")]
        public int? HandiPer { get; set; }

        [Column("doctorName")]
        [StringLength(50)]
        public string? DoctorName { get; set; }

        [Column("doctorAdd")]
        [StringLength(100)]
        public string? DoctorAdd { get; set; }

        [Column("doctorCity")]
        public int? DoctorCity { get; set; }

        [Column("doctorContact")]
        [StringLength(50)]
        public string? DoctorContact { get; set; }

        [Column("docClicName")]
        [StringLength(50)]
        public string? DocClicName { get; set; }

        [Column("docClicAdd")]
        [StringLength(100)]
        public string? DocClicAdd { get; set; }

        [Column("docClicCity")]
        public int? DocClicCity { get; set; }

        [Column("docClicContact")]
        [StringLength(50)]
        public string? DocClicContact { get; set; }

        [Column("pic")]
        public string? Pic { get; set; } // length -1 indicates varchar(max)

        [Column("mTongue")]
        public int? MTongue { get; set; }

        [Column("reasonJoin")]
        [StringLength(50)]
        public string? ReasonJoin { get; set; }

        [Column("description")]
        [StringLength(50)]
        public string? Description { get; set; }

        [Column("hobbies")]
        [StringLength(50)]
        public string? Hobbies { get; set; }

        [Column("games")]
        [StringLength(50)]
        public string? Games { get; set; }

        [Column("Activities")]
        [StringLength(50)]
        public string? Activities { get; set; }

        [Column("aClass")]
        public int? AClass { get; set; }

        [Column("cSection")]
        public int? CSection { get; set; }

        [Column("cAdmNo")]
        public int? CAdmNo { get; set; }

        [Column("cCurrClass")]
        public int? CCurrClass { get; set; }

        [Column("status")]
        [StringLength(1)]
        public string? Status { get; set; }

        [Column("Result")]
        [StringLength(1)]
        public string? Result { get; set; }

        [Column("libCard")]
        [StringLength(2)]
        public string? LibCard { get; set; }

        [Column("studentDeposit")]
        public double? StudentDeposit { get; set; } // float maps to double

        [Column("FormNo")]
        [StringLength(10)]
        public string? FormNo { get; set; }

        [Column("BoardRollNo")]
        [StringLength(20)]
        public string? BoardRollNo { get; set; }

        [Column("District")]
        public int? District { get; set; }

        [Column("Tehsil")]
        public int? Tehsil { get; set; }

        [Column("Village")]
        public int? Village { get; set; }

        [Column("Mohalla")]
        public int? Mohalla { get; set; }

        [Column("SchoolAdm")]
        public int? SchoolAdm { get; set; }

        [Column("HCode")]
        public int? HCode { get; set; }

        [Column("ClassCat")]
        public int? ClassCat { get; set; }

        [Column("aStatus")]
        [StringLength(5)]
        public string? AStatus { get; set; }

        [Column("LoginName")]
        [StringLength(50)]
        public string? LoginName { get; set; }

        [Column("lUserDt")]
        public DateTime? LUserDt { get; set; }

        [Column("CourseID")]
        public int? CourseID { get; set; }

        [Column("sCode")]
        [StringLength(30)]
        public string? SCode { get; set; }

        [Column("UserLoginName")]
        [StringLength(30)]
        public string? UserLoginName { get; set; }

        [Column("strPwd")]
        [StringLength(30)]
        public string? StrPwd { get; set; }

        [Column("PsebCode")]
        [StringLength(200)]
        public string? PsebCode { get; set; }

        [Column("schPrefix")]
        [StringLength(30)]
        public string? SchPrefix { get; set; }

        [Column("sUpdated")]
        [StringLength(30)]
        public string? SUpdated { get; set; }

        [Column("UserID")]
        public int? UserID { get; set; }

        [Column("prtybasis")]
        [StringLength(100)]
        public string? Prtybasis { get; set; }

        [Column("LedgerMasterID")]
        public int? LedgerMasterID { get; set; }

        [Column("ICardPhoto")]
        public string? ICardPhoto { get; set; } // length -1 indicates nvarchar(max)

        [Column("ExamMarkSheetUrl")]
        [StringLength(200)]
        public string? ExamMarkSheetUrl { get; set; }

        [Column("RegNo")]
        [StringLength(40)]
        public string? RegNo { get; set; }

        [Column("Email")]
        [StringLength(400)]
        public string? Email { get; set; }

        [Column("StateCd")]
        public int? StateCd { get; set; }

        [Column("CityCd")]
        public int? CityCd { get; set; }

        [Column("CountryCd")]
        public int? CountryCd { get; set; }

        [Column("FeeCategory")]
        public int? FeeCategory { get; set; }

        [Column("AadharNo")]
        [StringLength(40)]
        public string? AadharNo { get; set; }

        [Column("SearchText")]
        [StringLength(1000)]
        public string? SearchText { get; set; }

        // Notice these match your specific station routing codes!
        [Column("PickupStationCd")]
        public int? PickupStationCd { get; set; }

        [Column("DropStationCd")]
        public int? DropStationCd { get; set; }
    }
}
