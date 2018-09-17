namespace TManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Company")]
    public partial class Company
    {
        public int CompanyID { get; set; }

        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(150)]
        public string ContactPerson { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(50)]
        public string EmailAddress { get; set; }

        public string CompanyDescription { get; set; }
    }
}
