namespace TManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Milestone")]
    public partial class Milestone
    {
        public Milestone()
        {
            PTasks = new HashSet<PTask>();
        }

        public int MilestoneID { get; set; }

        [Required]
        [StringLength(50)]
        public string MilestoneName { get; set; }

       [System.ComponentModel.DataAnnotations.DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int ProjectID { get; set; }

        public decimal weight { get; set; }

        public decimal? PercentageDone { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        public DateTime? DateEntered { get; set; }

        public string MilestoneDescr { get; set; }

        [StringLength(50)]
        public string MilestoneStatus { get; set; }

        public int? TaskNumber { get; set; }

        public DateTime? ActualDateCompleted { get; set; }

        [StringLength(50)]
        public string ValidationStatus { get; set; }

        [StringLength(50)]
        public string ValidatedBy { get; set; }

        public DateTime? ValidationDate { get; set; }

        public virtual ProjectReg ProjectReg { get; set; }

        public virtual ICollection<PTask> PTasks { get; set; }
    }
}
