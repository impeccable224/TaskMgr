namespace TManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkDone")]
    public partial class WorkDone
    {
        public int WorkDoneID { get; set; }

        public int? ProjectID { get; set; }

        public int? TeamID { get; set; }

        [StringLength(50)]
        public string ProjectName { get; set; }

        public string WorkDoneDescr { get; set; }

        public string issues { get; set; }

        public DateTime? EntryDate { get; set; }

        [StringLength(500)]
        public string Othercomments { get; set; }

        [StringLength(150)]
        public string DoneBy { get; set; }

        public decimal? PercentageWkd { get; set; }

        public int? MilestonesCompleted { get; set; }

        public virtual ProjectReg ProjectReg { get; set; }
    }
}
