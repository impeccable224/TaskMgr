namespace TManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Team")]
    public partial class Team
    {
        public Team()
        {
            ProjectAssignments = new HashSet<ProjectAssignment>();
        }

        public int TeamID { get; set; }

        [StringLength(50)]
        public string Fullname { get; set; }

        [StringLength(50)]
        public string Skillset { get; set; }

        [StringLength(50)]
        public string Role { get; set; }

        public int? isAdmin { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string EmailAddress { get; set; }

        public string ContactAddress { get; set; }

        public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; }
    }
}
