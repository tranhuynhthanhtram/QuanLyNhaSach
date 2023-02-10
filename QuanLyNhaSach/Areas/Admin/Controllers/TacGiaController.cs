using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Areas.Admin.Controllers
{
    public class TacGiaController : Controller
    {
        // GET: Admin/TacGia
        QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        public ActionResult List()
        {
            if (Session["TenNhanVien"] == null)
                return RedirectToAction("Login", "Home");
            List<TacGia> dstg = da.TacGias.Select(s => s).ToList();
            return View(dstg);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection fcollection, TacGia tacGia)
        {
            var tenTacGia = fcollection["TenTacGia"];
            if (String.IsNullOrEmpty(tenTacGia))
            {
                ViewData["Loi"] = "Tên NXB không được trống";
            }
            else
            {

                da.TacGias.Add(tacGia);
                da.SaveChanges();
                return RedirectToAction("List");
            }
            return this.Create();
        }
        public ActionResult Delete(int id)
        {
            TacGia tacGia = da.TacGias.FirstOrDefault(s => s.MaTacGia == id);
            da.TacGias.Remove(tacGia);
            da.SaveChanges();
            return RedirectToAction("List");
        }
        public ActionResult Edit(int id)
        {
            TacGia tacGia = da.TacGias.FirstOrDefault(s => s.MaTacGia == id);
            return View(tacGia);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection fcollection, int id)
        {
            TacGia tacGia = da.TacGias.FirstOrDefault(s => s.MaTacGia == id);
            tacGia.TenTacGia = fcollection["TenTacGia"];
            tacGia.QueQuan = fcollection["QueQuan"];
            UpdateModel(tacGia);
            da.SaveChanges();
            return RedirectToAction("List");
        }
    }
}