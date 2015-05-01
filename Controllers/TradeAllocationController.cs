using DashboardTest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DashboardTest.Controllers
{
    public class TradeAllocationController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            //return View("AllocationInstruction");
            return PartialView("AllocationInstruction");
          
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AllocationInstruction()
        {
            NorthwindEntities model = new NorthwindEntities();
            List<Order_Detail> viewModel = model.Order_Details.Take(1).ToList();
            int totCount = viewModel.Count;

            var list = JsonConvert.SerializeObject(viewModel, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(new { totCount = totCount, allocData = list }, JsonRequestBehavior.AllowGet);
            //return Json(new { allocData = viewModel }, JsonRequestBehavior.AllowGet);
        }

    }

}
