using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TransportApi.Models
{
    [Table("AdmissionDTL")]
    public class AdmissionDtl
    {
        [Key] // Primary or Composite Key element
        [Column("admCd")]
        public int? AdmCd { get; set; }

        [Column("regCd")]
        public int? RegCd { get; set; }

        [Column("fName")]
        [StringLength(50)]
        public string? FName { get; set; }

        [Column("fDob")]
        public DateTime? FDob { get; set; }

        [Column("fQualification")]
        [StringLength(50)]
        public string? FQualification { get; set; }

        [Column("fOccupation")]
        [StringLength(50)]
        public string? FOccupation { get; set; }

        [Column("fAddress")]
        [StringLength(200)]
        public string? FAddress { get; set; }

        [Column("fOfficePhone")]
        [StringLength(50)]
        public string? FOfficePhone { get; set; }

        [Column("fIncome")]
        public double? FIncome { get; set; } // float maps to double in C#

        [Column("fMailId")]
        [StringLength(50)]
        public string? FMailId { get; set; }

        [Column("fatherPic")]
        public string? FatherPic { get; set; } // Length -1 maps to varchar(max)

        [Column("fatherSign")]
        public string? FatherSign { get; set; } // Length -1 maps to varchar(max)

        [Column("mName")]
        [StringLength(50)]
        public string? MName { get; set; }

        [Column("mDOB")]
        public DateTime? MDOB { get; set; }

        [Column("mQualification")]
        [StringLength(50)]
        public string? MQualification { get; set; }

        [Column("mAddress")]
        [StringLength(210)]
        public string? MAddress { get; set; }

        [Column("mOccupation")]
        [StringLength(50)]
        public string? MOccupation { get; set; }

        [Column("mOfficePhone")]
        [StringLength(50)]
        public string? MOfficePhone { get; set; }

        [Column("mIncome")]
        public double? MIncome { get; set; } // float maps to double

        [Column("mEmailId")]
        [StringLength(50)]
        public string? MEmailId { get; set; }

        [Column("motherPic")]
        public string? MotherPic { get; set; } // Length -1 maps to varchar(max)

        [Column("motherSign")]
        public string? MotherSign { get; set; } // Length -1 maps to varchar(max)

        [Column("corrAdd")]
        [StringLength(150)]
        public string? CorrAdd { get; set; }

        [Column("cCity")]
        public int? CCity { get; set; }

        [Column("cState")]
        public int? CState { get; set; }

        [Column("cContactNo")]
        [StringLength(50)]
        public string? CContactNo { get; set; }

        [Column("Address")]
        [StringLength(8000)]
        public string? Address { get; set; }

        [Column("City")]
        public int? City { get; set; }

        [Column("State")]
        public int? State { get; set; }

        [Column("contactNo")]
        public string? ContactNo { get; set; } // Length -1 maps to varchar(max)

        [Column("eMailId")]
        [StringLength(50)]
        public string? EMailId { get; set; }

        [Column("livingAway")]
        [StringLength(1)]
        public string? LivingAway { get; set; }

        [Column("oAddress")]
        [StringLength(100)]
        public string? OAddress { get; set; }

        [Column("cPerson")]
        [StringLength(30)]
        public string? CPerson { get; set; }

        [Column("cPerson1")]
        [StringLength(30)]
        public string? CPerson1 { get; set; }

        [Column("cPerson2")]
        [StringLength(30)]
        public string? CPerson2 { get; set; }

        [Column("LoginName")]
        [StringLength(50)]
        public string? LoginName { get; set; }

        [Column("lUserDt")]
        public DateTime? LUserDt { get; set; }

        [Column("schPrefix")]
        [StringLength(30)]
        public string? SchPrefix { get; set; }

        [Column("sCode")]
        [StringLength(30)]
        public string? SCode { get; set; }

        [Column("UserID")]
        public int? UserID { get; set; }

        [Column("admSCode")]
        [StringLength(100)]
        public string? AdmSCode { get; set; }
    }
}
