using QuanLyNhaSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyNhaSach.Areas.Admin.Controllers
{
    public class StatisticController : Controller
    {
        // GET: Admin/Statistic
        QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
       
        public ActionResult Year()
        {
            if (Session["TenNhanVien"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }
        public ActionResult GetReportByYear(int year)
        {
            
            var lsData = da.Pro_ThongKeDSTungThang(year);
            return Json(lsData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Month()
        {
            if (Session["TenNhanVien"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }
        public ActionResult GetReportBestSalesByMonth( int month)
        {
           
            var lsData = da.Pro_BestSalesByMonth(month);
            return Json(lsData, JsonRequestBehavior.AllowGet);
        }


    }
}