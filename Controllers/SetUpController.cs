using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TManager.Models;
using TManager.DAL;

namespace TManager.Controllers
{
    public class SetUpController : Controller
    {
        private GenericUnitOfWork unitOfWork = null;
        // GET: SetUp
        public SetUpController()
        {
            unitOfWork = new GenericUnitOfWork();
           
        }

        // Constructor for Dummy Data
        public SetUpController(GenericUnitOfWork uow)
        {
            this.unitOfWork = uow;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Filldev(string tos)
        {
            try { 
            int ID = Convert.ToInt32(tos);
            var dev = unitOfWork.Repository<Team>().Get().Where(a => a.Role == TManager.Models.Consts.ActorRole.DEVELOPER && a.TeamID != ID).ToList().Select(e => new { label = e.Fullname, value = e.TeamID });
            if (dev.Count() == 0)
                dev = unitOfWork.Repository<Team>().Get(b => b.Role == TManager.Models.Consts.ActorRole.DEVELOPER).ToList().Select(e => new { label =  e.Fullname, value = e.TeamID });
            return Json(dev, JsonRequestBehavior.AllowGet);
           
           }
                catch (Exception) { }

            return Json(new { FULL_NAME = "Unknown" }, JsonRequestBehavior.AllowGet);
        
        }
        [HttpPost]
        public ActionResult DeleteDoc(string id)
        {

            if (!(string.IsNullOrEmpty(id)))
            {

                int delID = int.Parse(id);
                UploadDoc doc = unitOfWork.Repository<UploadDoc>().GetByID(delID);
                string fullPath = Request.MapPath("~/App_Data/UploadedDoc/" + doc.FileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                unitOfWork.Repository<UploadDoc>().Delete(doc);
                unitOfWork.Save();
                return Json(new { msg = "Document Remove successfully", D = true });
            }
            return Json(new { msg = "error in removing doc", D = false });
        }


        [HttpPost]
        public ActionResult DeleteTempDoc(string id)
        {

            if (!(string.IsNullOrEmpty(id)))
            {

                int delID = int.Parse(id);
                TempDoc doc = unitOfWork.Repository<TempDoc>().GetByID(delID);
                string fullPath = Request.MapPath("~/App_Data/UploadedDoc/" + doc.FileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                unitOfWork.Repository<TempDoc>().Delete(doc);
                unitOfWork.Save();
                return Json(new { msg = "Document Remove successfully", D = true });
            }
            return Json(new { msg = "error in removing doc", D = false });
        }
    }
}