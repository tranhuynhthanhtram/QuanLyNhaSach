using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        // GET: Admin/Order
        QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        public ActionResult List()
        {
            if (Session["TenNhanVien"] == null)
                return RedirectToAction("Login", "Home");
            List<DonHang> dss = da.DonHangs.Select(s => s).ToList();
            return View(dss);
        }
    }
}