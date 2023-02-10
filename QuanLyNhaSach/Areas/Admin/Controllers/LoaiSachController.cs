using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Areas.Admin.Controllers
{
    public class LoaiSachController : Controller
    {
        // GET: Admin/LoaiSach
        QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        public ActionResult List()
        {
            if (Session["TenNhanVien"] == null)
                return RedirectToAction("Login", "Home");
            List<LoaiSach> dsLs = da.LoaiSaches.Select(s => s).ToList();
            return View(dsLs);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection fcollection, LoaiSach loaiSach)
        {
            var tenLoaiSach = fcollection["TenLoaiSach"];
            if (String.IsNullOrEmpty(tenLoaiSach))
            {
                ViewData["Loi"] = "Tên loại sáchkhông được trống";
            }
            else
            {
                da.LoaiSaches.Add(loaiSach);
                da.SaveChanges();
                return RedirectToAction("List");
            }
            return this.Create();
        }
        public ActionResult Edit(int id)
        {
            LoaiSach loaiSach = da.LoaiSaches.FirstOrDefault(s => s.MaLoaiSach == id);
            return View(loaiSach);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection fcollection, int id)
        {
            LoaiSach loaiSach = da.LoaiSaches.FirstOrDefault(s => s.MaLoaiSach == id);
            loaiSach.TenLoaiSach = fcollection["TenLoaiSach"];
            UpdateModel(loaiSach);
            da.SaveChanges();
            return RedirectToAction("List");
        }
        public ActionResult Delete(int id)
        {
            LoaiSach loaiSach = da.LoaiSaches.FirstOrDefault(s => s.MaLoaiSach == id);
            da.LoaiSaches.Remove(loaiSach);
            da.SaveChanges();
            return RedirectToAction("List");
        }
    }
}