namespace TManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProjectReg")]
    public partial class ProjectReg
    {
        public ProjectReg()
        {
            Milestones = new HashSet<Milestone>();
            ProjectAssignments = new HashSet<ProjectAssignment>();
            UploadDocs = new HashSet<UploadDoc>();
            WorkDones = new HashSet<WorkDone>();
        }

        [Key]
        public int ProjectRegisterID { get; set; }

        [StringLength(450)]
        public string ProjectTaskType { get; set; }

        public string ProjectName { get; set; }

        //[StringLength(50)]
        public string CompanyID { get; set; }

        [StringLength(50)]
        public string BizContact { get; set; }

        [StringLength(50)]
        public string Priority { get; set; }

        [StringLength(50)]
        public string BAInCharge { get; set; }

        [StringLength(50)]
        public string ProjectManager { get; set; }

        [StringLength(50)]
        public string LeadDeveloper { get; set; }

        public string OtherDeveloper { get; set; }

        public string ProjectObjective { get; set; }

        public DateTime? DateRecieved { get; set; }

        [StringLength(50)]
        public string ProjectStatus { get; set; }

        public DateTime? statusUpdatedDate { get; set; }

        public DateTime? DevStartDate { get; set; }

        public DateTime? DevEndDate { get; set; }

        [StringLength(50)]
        public string oDeveloper { get; set; }

        public string BusinessNeed { get; set; }

        [StringLength(50)]
        public string RegisteredBy { get; set; }

        public int? NumberofMilestones { get; set; }

        public int? ParentProjectID { get; set; }

        public DateTime? UATDate { get; set; }

        public DateTime? ImplementationDate { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public string ScopeTask { get; set; }

        [StringLength(50)]
        public string ScheduledBy { get; set; }

        public string ExpectedBenefits { get; set; }

        public string ProjectRemark { get; set; }

        public int? Duration { get; set; }

        public DateTime? DateRequestFreezed { get; set; }

        [StringLength(150)]
        public string RequestFreezedBy { get; set; }

        public decimal? PercentageDone { get; set; }

        public DateTime? ScheduleModifiedDate { get; set; }

        public DateTime? ProjectModifiedDate { get; set; }

        public DateTime? oDevStartDatre { get; set; }

        public DateTime? oDevEndDate { get; set; }

        public short? IsMilestone { get; set; }

        public short? IsTask { get; set; }

        [StringLength(50)]
        public string UATUrl { get; set; }

        public int? YearCompleted { get; set; }

        public string TestServerName { get; set; }

        public string LiveServerName { get; set; }

        public string LiveURL { get; set; }

        public string FilePathUrl { get; set; }

        [StringLength(50)]
        public string oProjectManager { get; set; }

        public DateTime? ExpectedDateCompleted { get; set; }

        public DateTime? ActualDateCompleted { get; set; }

        public short? requestFreezed { get; set; }

        public string DeptResponsible { get; set; }

        public string NumberOfUsers { get; set; }

        public decimal? EstimatedCost { get; set; }
        public string  CompanyName {get;set;}
        public string ExpectedDeliveryTimeQ { get; set; }
        public string ExpectedDeliveryTimeYr { get; set; }

        [StringLength(150)]
        public string Approval { get; set; }

        public virtual ICollection<Milestone> Milestones { get; set; }

        public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; }

        public virtual ICollection<UploadDoc> UploadDocs { get; set; }

        public virtual ICollection<WorkDone> WorkDones { get; set; }
    }
}
