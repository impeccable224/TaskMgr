using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TManager.Models;
using TManager.DAL;
using System.IO;
using TManager.Models.ViewModels;
using System.Data.Entity.Validation;
 using TManager.Models.Business;

namespace TManager.Controllers
{
    public class HomeController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;
         private DataLogic dl = null;
        public HomeController()
        {
            unitOfWork = new GenericUnitOfWork();
        }
        // Constructor for Dummy Data
        public HomeController(GenericUnitOfWork uow)
        {
            this.unitOfWork = uow;
        }
        private string BasePath()
        {
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DecisionPage()
        {
            ViewBag.ProjectRegisterID = new SelectList(unitOfWork.Repository<ProjectReg>().Get().Where(p => p.ProjectTaskType != "Change Request" && p.ProjectStatus != TManager.Models.Consts.ApprovalConsts.COMPLETED).OrderBy(n => n.ProjectName), "ProjectRegisterID", "ProjectName");
           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RedirectTask(ProjectVM pvm)
        {
            try
            {
                string message = null;
                if( pvm.ProjectTaskType == "Change Request" && pvm.ProjectRegisterID!= null )
                {
                    var projectData = unitOfWork.Repository<ProjectReg>().GetByID(pvm.ProjectRegisterID);
                    if (projectData != null)
                    {
                        int? ID = projectData.ProjectRegisterID;
                        return RedirectToAction("ChangeRequest", new { ID = ID });
                        }
                    //else
                    //{
                    //    ViewBag.message =   " does not Exist!";
                    //    return View("DecisionPage");
                    //}
                }
                else if (pvm.ProjectTaskType == "Documentation")
                {
                    var projectData = unitOfWork.Repository<ProjectReg>().GetByID(pvm.ProjectRegisterID);
                    if (projectData != null)
                    {
                        int? ID = projectData.ProjectRegisterID;
                        return RedirectToAction("Documentation", new { ID = ID });
                    }
                                       
                }
                else if(pvm.ProjectTaskType == "Application")
                {
                    
                        return RedirectToAction("AddProject");
                   
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }

            return View("DecisionPage");
        }
        public ActionResult Projects()
        {
            //var me = User.Identity.Name.Split('\\')[1].ToUpper(); //try and find out how window will align with ethe authenticated  persson to know who log in so that you can show him only  his own  project
            var projects = (from v in unitOfWork.Repository<ProjectReg>().Get(e => e.ProjectRegisterID != null && (e.ProjectStatus == TManager.Models.Consts.ApprovalConsts.PENDING))//&& (e.prjectRequester == me) // am the projectmanager or assigned as on of the developers
                           orderby v.ProjectRegisterID descending
                           select new TManager.Models.ViewModels.ProjectVM
                           {
                                ProjectRegisterID = v.ProjectRegisterID,
                              ProjectTaskType = v.ProjectTaskType,
                              ProjectManager = unitOfWork.Repository<Team>().GetByID(int.Parse(v.ProjectManager)).Fullname,
                               ProjectName=v.ProjectName,
                               ProjectStatus= v.ProjectStatus,
                               LeadDeveloper = unitOfWork.Repository<Team>().GetByID(int.Parse(v.LeadDeveloper)).Fullname,
                               DateRecieved = v.DateRecieved,
                               ProjectAttachments = unitOfWork.Repository<UploadDoc>().Get(f => f.ProjectID == v.ProjectRegisterID).Select(b => new TManager.Models.ViewModels.ProjectDocVM
                               {
                                   FileID =b.FileID,
                                   ProjectID = b.ProjectID,
                                   FileName = b.FileName,
                                   FileSystemName = b.FileSystemName,
                                   Description = b.Description,

                               }).ToList()
                           }).Distinct();

            return View(projects);
        }

        public ActionResult AddProject()
        {
            //ViewBag.CompanyName = unitOfWork.Repository<Company>().Get().OrderBy(o => o.CompanyName).Select(p => new SelectListItem
            //{
            //    Text = p.CompanyName,
            //    Value = p.CompanyName
            //});
            var company = unitOfWork.Repository<Company>().Get().OrderBy(o => o.CompanyName); 

            var LeadDev = unitOfWork.Repository<Team>().Get(); 
            var projectMgr = unitOfWork.Repository<Team>().Get(e=>e.Role == TManager.Models.Consts.ActorRole.PROJECT_MANAGER);
            ViewBag.LeadDeveloper = new SelectList(LeadDev, "TeamID", "Fullname");
            ViewBag.ProjectManager = new SelectList(projectMgr, "TeamID", "Fullname");
            ViewBag.CompanyID = new SelectList(company, "CompanyID", "CompanyName");
            return View();
        }

        [HttpPost]
         public ActionResult AddProject(ProjectReg preg)
        {
            var insertProject = new ProjectReg();
            List<string> arr = new List<string>();
            string tid = null;
            int CompID = 0;
            var additionalFile = false;
            try
            {
                 insertProject.ProjectName = preg.ProjectName;
                 insertProject.Priority = preg.Priority;
                 insertProject.ProjectManager = preg.ProjectManager;
                 insertProject.DeptResponsible = preg.DeptResponsible;
                 insertProject.LeadDeveloper = preg.LeadDeveloper;

                 var otherDev = preg.multiSelectLob.Split(',');//string.Join(",", preg.multiSelectLob.ToArray()).Split(',');
                 foreach(var o in otherDev.ToList())
                 {
                     var od = unitOfWork.Repository<Team>().Get(f => f.Fullname == o);
                     if (od.Count() > 0)//.FirstOrDefault().TeamID;
                     {
                         tid = od.FirstOrDefault().TeamID.ToString();

                         arr.Add(tid);
                         od = null;
                     }
                 }
                 CompID = int.Parse(preg.CompanyID);
                 var com = unitOfWork.Repository<Company>().GetByID(CompID);
                 insertProject.OtherDeveloper = string.Join(",", arr.ToArray());
                 insertProject.NumberOfUsers = preg.NumberOfUsers;
                 insertProject.oDeveloper = preg.oDeveloper;
                 insertProject.oProjectManager = preg.oProjectManager;
                 insertProject.CompanyID = preg.CompanyID;
                 insertProject.CompanyName = com.CompanyName;
                 insertProject.BusinessNeed = preg.BusinessNeed;
                 insertProject.BizContact =com.ContactPerson;
                 insertProject.DateRecieved = DateTime.Now;

               // insertProject.RegisteredBy 
                 insertProject.ProjectRemark = preg.ProjectRemark;
                 insertProject.ProjectStatus = preg.ProjectStatus;
                 insertProject.ProjectTaskType = preg.ProjectTaskType;
                 insertProject.ProjectObjective = preg.ProjectObjective;
                 insertProject.ProjectStatus = TManager.Models.Consts.ApprovalConsts.PENDING;
                 bool hasv = unitOfWork.Repository<ProjectReg>().Get().Any(a => a.ProjectName == insertProject.ProjectName);
                if (hasv == false)
                {
                    unitOfWork.Repository<ProjectReg>().Insert(insertProject);
                    unitOfWork.Save();
                    var docs = unitOfWork.Repository<TempDoc>().Get().Where(d => d.Description == insertProject.ProjectName && d.FileStatus == 1 );
                   UploadDoc doc = new UploadDoc(); 
                    if (docs != null && docs.ToList().Count != 0)
                    {
                        foreach (var f in docs)
                        {
                            doc.FileName = f.FileName;
                            doc.FileSystemName = f.FileSystemName;
                            doc.Description = f.Description;
                            doc.ProjectID = insertProject.ProjectRegisterID;
                            doc.ContentType = f.ContentType;
                            unitOfWork.Repository<UploadDoc>().Insert(doc);
                            unitOfWork.Save();
                        }

                        // we have to set the file status to 2 to differentiate the ones taken from tempDoc to vendorDoc tables 
                        foreach (var t in docs)
                        {
                            TempDoc td = unitOfWork.Repository<TempDoc>().GetByID(t.TempID);
                            td.FileStatus = 2;
                            td.ProjectID =  insertProject.ProjectRegisterID;
                            unitOfWork.Repository<TempDoc>().Update(td);

                        }
                        unitOfWork.Save();

                    }
                }
                return RedirectToAction("Projects");
            }
                                            
                    catch (DbEntityValidationException e)
                    {
                        ViewBag.Error = e.StackTrace;
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            ViewBag.Error += eve.Entry.Entity.GetType().Name + ", " + eve.Entry.State;
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                ViewBag.Error += ve.PropertyName + " :: " + ve.ErrorMessage;
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                    }
                      // set notification for approval 
                //    string TargetMsg = PCMS_v1.Models.Consts.Message.RequesterLp1VendorMsg(insertVendor.VendorName, newaction.ActionID, BasePath());
                //    string subject = "#" + insertVendor.VendorID + " / " + insertVendor.VendorType + "/" + insertVendor.VendorName + " Registration Approval";
                //    var lp1Data = unitOfWork.Repository<ConsolStaffData>().Get(e => e.POSITION_NBR == RequesterData.ReportsTo).FirstOrDefault();
                //    // initiator's message
                //    string initiatorMsg = PCMS_v1.Models.Consts.Message.initiatorMsg(insertVendor);
                //    //insertVendor.RequesterLp1Data.FullName = lp1Data.FULL_NAME;
                //    insertVendor.RequesterData = DataHelper.GetRequesterInfo(unitOfWork, insertVendor.VendorRequester);
                //    insertVendor.RequesterData.FullName = RequesterData.LastName + " " + RequesterData.OtherNames;
                //    insertVendor.RequesterData.EmailAddress = RequesterData.EmailAddress;
                //    string TargetMail = lp1Data.EMAIL_ADDR;
                //    // we can fire mail to both requester and initiator
                //      SendEmail(TargetMsg, TargetMail, subject, newaction.ActionID, newaction, PCMS_v1.Models.Consts.ProcessAndFlowName.VENDOR_PROCESS);
                //    SendEmail(initiatorMsg, RequesterData.EmailAddress, subject, newaction.ActionID, newaction, PCMS_v1.Models.Consts.ProcessAndFlowName.VENDOR_PROCESS);
                //    // NotifyNextActor(insertVendor, insertVendor.RequesterLp1, insertVendor.RequesterLp1, PCMS_v1.Models.Consts.ActorRole.REQUESTER_LP1, "ReviewVendor", PCMS_v1.Models.Consts.ActorRole.VENDOR_PROCESS, sequenceNumber, insertVendor.VendorID, targetMsg, initiatorMsg, insertVendor.RequesterLp1Data.EmailAddress, insertVendor.RequesterData.EmailAddress);
                
                //TempData["CssClass"] = "success";
                //TempData["Notify"] = "Vendor" + insertVendor.VendorName + " created successfully !";
                
                 var company = unitOfWork.Repository<Company>().Get().OrderBy(o => o.CompanyName);

                 var LeadDev = unitOfWork.Repository<Team>().Get();
                 var projectMgr = unitOfWork.Repository<Team>().Get(e => e.Role == TManager.Models.Consts.ActorRole.PROJECT_MANAGER);
                 ViewBag.LeadDeveloper = new SelectList(LeadDev, "TeamID", "Fullname");
                 ViewBag.ProjectManager = new SelectList(projectMgr, "TeamID", "Fullname");
                 ViewBag.CompanyID = new SelectList(company, "CompanyID", "CompanyName");
                 return View();
            }
               
          public ActionResult PDetails(int? id)
        {
            if (null != id)
            {
                var ProjectApproval = dl.FetchCompleteProjectDetails(unitOfWork, id);

                return View(ProjectApproval);
            }
            else
                return View(new ProjectVM());
        }
        public ActionResult ProjectSchedule(int? id)
        {
            var DevList = new List<int>();
           
            var v =  unitOfWork.Repository<ProjectReg>().GetByID(id.Value );
                             ProjectVM pvm = new TManager.Models.ViewModels.ProjectVM();
                               pvm.ProjectRegisterID = v.ProjectRegisterID;
                                pvm.ProjectTaskType = v.ProjectTaskType;
                                pvm.OtherDeveloper = v.OtherDeveloper;
                                pvm.OtherDeveloper += (v.OtherDeveloper != null) ?  ',' + v.LeadDeveloper + ',' + v.ProjectManager : v.LeadDeveloper + ',' + v.ProjectManager;
                                pvm.ProjectManager = unitOfWork.Repository<Team>().GetByID(int.Parse(v.ProjectManager)).Fullname;
                                pvm.ProjectName = v.ProjectName;
                                pvm.ProjectStatus = v.ProjectStatus;
                                pvm.LeadDeveloper = unitOfWork.Repository<Team>().GetByID(int.Parse(v.LeadDeveloper)).Fullname;
                                pvm.Priority = v.Priority;
                                pvm.DeptResponsible = v.DeptResponsible;
                                pvm.CompanyName = v.CompanyName;
                                pvm.DateRecieved = v.DateRecieved;
                                pvm.ProjectObjective = v.ProjectObjective;
                                pvm.ProjectAttachments = unitOfWork.Repository<UploadDoc>().Get(f => f.ProjectID == v.ProjectRegisterID).Select(b => new TManager.Models.ViewModels.ProjectDocVM
                                {
                                    FileID = b.FileID,
                                    ProjectID = b.ProjectID,
                                    FileName = b.FileName,
                                    FileSystemName = b.FileSystemName,
                                    Description = b.Description,

                                }).ToList();
                                
            var Devs =( pvm.OtherDeveloper != null)? pvm.OtherDeveloper.Split(',') : null;
            var selectList = new List<SelectListItem>();
                                foreach (var f in Devs.ToList())
                                {
                                    int Devid = int.Parse(f);
                                    var DList = unitOfWork.Repository<Team>().GetByID(Devid);
                                   selectList.Add(new SelectListItem
                                    {
                                        Value = DList.TeamID.ToString(),
                                        Text = DList.Fullname,
                                        //Selected = false
                                    }
                            );
                                }
                                pvm.Developers = selectList;
            //var ProjectApproval = dl.FetchCompleteProjectDetails(unitOfWork, id);
            return View(pvm);
        }
          
        public ActionResult  PJquery()
        {
            var p = unitOfWork.Repository<ProjectReg>().GetByID(26);
            ProjectVM pvm = new ProjectVM();
            pvm.ProjectRegisterID = p.ProjectRegisterID;
            pvm.ProjectName = p.ProjectName;
            pvm.BusinessNeed = p.BusinessNeed;
            return View(pvm);
        }
        [HttpPost]
        public ActionResult UploadDoc(TManager.Models.ViewModels.AttachedFileVM uploadhelper)
        {
            if (ModelState.IsValid)
            {
                if (uploadhelper.AttachedFile != null && uploadhelper.AttachedFile.ContentLength > 0)
                {
                    var realFileName = System.IO.Path.GetFileName(uploadhelper.AttachedFile.FileName);
                    var filePath = Server.MapPath("~/App_Data/UploadedDoc");
                    var fileName = DateTime.Now.Ticks + System.IO.Path.GetExtension(uploadhelper.AttachedFile.FileName);
                    string savedFileName = System.IO.Path.Combine(filePath, fileName);
                    uploadhelper.AttachedFile.SaveAs(savedFileName);
                    // init object here
                    var evd = new TempDoc();
                    evd.FileName = realFileName;
                    evd.FileSystemName = fileName;
                    evd.ContentType = uploadhelper.AttachedFile.ContentType;
                    evd.FileExtension = Path.GetExtension(fileName);
                     // for now
                    evd.Description = uploadhelper.FDescription;
                    // evd.WorkFlowName = "Vendor Registration";
                    evd.FileStatus = 1;
                    unitOfWork.Repository<TempDoc>().Insert(evd);
                    unitOfWork.Save();
                    // we shall send back the id for this file
                    return Json(new { ID = evd.TempID, R = true, FileName = evd.FileName, FileSystemName = evd.FileSystemName, Description = evd.Description, DLink = Url.Action("DownloadTemp", "Home", new { id = evd.TempID }) });

                }
            }

            return Json(new { R = false });

        }
        [HttpPost]
        public ActionResult UploadDocEdit(TManager.Models.ViewModels.AttachedFileVM uploadhelper, int ProjectID)
        {
            if (ModelState.IsValid)
            {
                if (uploadhelper.AttachedFile != null && uploadhelper.AttachedFile.ContentLength > 0)
                {
                    var realFileName = System.IO.Path.GetFileName(uploadhelper.AttachedFile.FileName);
                    var filePath = Server.MapPath("~/App_Data/UploadedDoc");
                    var fileName = DateTime.Now.Ticks + System.IO.Path.GetExtension(uploadhelper.AttachedFile.FileName);
                    string savedFileName = System.IO.Path.Combine(filePath, fileName);
                    uploadhelper.AttachedFile.SaveAs(savedFileName);
                    // init object here
                    var evd = new UploadDoc();
                    evd.FileName = realFileName;
                    evd.FileSystemName = fileName;
                    evd.ContentType = uploadhelper.AttachedFile.ContentType;
                    evd.FileExtension = Path.GetExtension(fileName);
                    evd.ProjectID = ProjectID;
                    // for now
                    evd.Description = uploadhelper.FDescription;
                   
                    unitOfWork.Repository<UploadDoc>().Insert(evd);
                    unitOfWork.Save();
                    // we shall send back the id for this file
                    return Json(new { ID = evd.FileID, R = true, FileName = evd.FileName, FileSystemName = evd.FileSystemName, Description = evd.Description, DLink = Url.Action("Download", "Home", new { id = evd.FileID }) });

                }
            }

            return Json(new { R = false });

        }

        public ActionResult DownloadTemp(int id)
        {

            try
            {
                var evd = unitOfWork.Repository<TempDoc>().GetByID(id);
                var doc = "~/App_Data/UploadedDoc/" + evd.FileSystemName;
                var contentType = evd.ContentType;// "application/pdf";
                var filePath = System.IO.Path.Combine(Server.MapPath("~/App_Data/UploadedDoc"), evd.FileSystemName);
                if (System.IO.File.Exists(filePath))
                {
                    return File(doc, contentType);
                }
            }
            catch (System.IO.DirectoryNotFoundException) { }
            catch (Exception) { }

            return Content("File could not be found!");
        }
        public ActionResult Download(int id)
        {

            try
            {
                var evd = unitOfWork.Repository<UploadDoc>().GetByID(id);
                var doc = "~/App_Data/UploadedDoc/" + evd.FileSystemName;
                var contentType = evd.ContentType;// "application/pdf";
                var filePath = System.IO.Path.Combine(Server.MapPath("~/App_Data/UploadedDoc"), evd.FileSystemName);
                if (System.IO.File.Exists(filePath))
                {
                    return File(doc, contentType);
                }
            }
            catch (System.IO.DirectoryNotFoundException) { }
            catch (Exception) { }

            return Content("File could not be found!");
        }
        public ActionResult Team()
        {
            var srms = unitOfWork.Repository<Team>().Get();
            return View(srms);
        }

       
        public ActionResult AddTeamUser()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult AddTeamUser(Team team)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Repository<Team>().Insert(team);
               
                unitOfWork.Save();
                return RedirectToAction("Team");
            }

            return View(team);
        }
       
        public ActionResult RemoveTMUser(FormCollection form)
        {
            var me = User.Identity.Name.Split('\\')[1];
            
            string ID = form["ID"];
            if (!(string.IsNullOrEmpty(ID)))
            {
                Team tm = unitOfWork.Repository<Team>().GetByID(Int32.Parse(ID));
                unitOfWork.Repository<Team>().Delete(tm);
               
                unitOfWork.Save();
            }


            return RedirectToAction("SRMTeam");
        }


        public JsonResult AddMilestone(Milestone frmValue)
        {
           //r me = UserIGGs();// User.Identity.Name.Split('\\')[1];
            var newMilestone = new Milestone();
            newMilestone.MilestoneName = frmValue.MilestoneName;
            newMilestone.MilestoneDescr = frmValue.MilestoneDescr;
           // newMilestone.EnteredBy = mee;
            newMilestone.DateEntered = DateTime.Now;
            newMilestone.weight = frmValue.weight;
            newMilestone.StartDate = frmValue.StartDate;
            newMilestone.EndDate = frmValue.EndDate;
            newMilestone.MilestoneStatus = TManager.Models.Consts.ApprovalConsts.PENDING;
            newMilestone.ProjectID = frmValue.ProjectID;
            try
            {
                unitOfWork.Repository<Milestone>().Insert(newMilestone);
                unitOfWork.Save();
                var msg = frmValue.MilestoneName + " have been added as milestone!";
              //prepare on the original               
                var TabRow = "<tr>" +
                 "<td><table><tr><td>MILE-" + newMilestone.MilestoneID + "</td> <td> " + frmValue.MilestoneName + " </td> <td>  " + frmValue.MilestoneDescr + " </td> <td> " + frmValue.StartDate.Value.ToString("dd-MM-yyyy") + " </td>" +
                 "<td> " + ("dd-MM-yyyy") + " </td> <td> </td> <td> <a projid='" + newMilestone.ProjectID + "' msid='" + newMilestone.MilestoneID + "' projname='" + newMilestone.ProjectReg.ProjectName + "' actname='" + newMilestone.MilestoneName + "' href='javascript::;' class='btn btn-sm btn-warning btn-flat pull-right' onclick='EditMilestone(this)'><i class='fa fa-edit'></i> Edit</a>  </td>" +
                  "</tr></table></td>"+
                "<td><table id='TaskM-"+newMilestone.MilestoneID + "'></table> </tr>";
                 return Json(new { msg = msg, MilestoneID = newMilestone.MilestoneID, TabRow = TabRow});
              
            }
            catch (Exception E)
            {
                var msg = E.Message;
                return Json(new { msg = msg });
            }
            // return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPerAdded(int ProjID,  int Weight)
        {
            decimal total=0;
            var MWeight = unitOfWork.Repository<Milestone>().Get(mr => mr.ProjectID == ProjID).Select(ac => ac.weight).Sum();
            if (MWeight == null || MWeight == 0)
            {
                MWeight = MWeight + Weight;
                return Json(new { res = "Overall Milestone Percentage  for this project is " + " <b>" + MWeight + "%</b>", msgtype = "s" });
            }
            else
            {
                 MWeight = MWeight + Weight;
            if (MWeight > 100)
            {
                return Json(new { Accom = "Overall Percentage for the milestones cannot exceed  <b>100%</b>", msgtype = "e" });
            }
            return Json(null, JsonRequestBehavior.AllowGet);

            }

        }


                
    }
}