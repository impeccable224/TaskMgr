namespace TManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PTask")]
    public partial class PTask
    {
        [Key]
        public int TaskID { get; set; }

        public int ProjectID { get; set; }

        public int? TeamID { get; set; }

        [StringLength(230)]
        public string TaskDescr { get; set; }

        [StringLength(50)]
        public string TaskStatus { get; set; }

        public int MilestoneID { get; set; }

        [StringLength(150)]
        public string AssignedTo { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        public DateTime? DateEntered { get; set; }

        public DateTime? DateCompleted { get; set; }

        [StringLength(50)]
        public string TaskRemark { get; set; }

        public decimal? weight { get; set; }

        public short? isAssigned { get; set; }

        public virtual Milestone Milestone { get; set; }

        public virtual PTask PTask1 { get; set; }

        public virtual PTask PTask2 { get; set; }
    }
}
