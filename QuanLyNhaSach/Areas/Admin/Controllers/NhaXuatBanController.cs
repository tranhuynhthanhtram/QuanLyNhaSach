using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Areas.Admin.Controllers
{
    public class NhaXuatBanController : Controller
    {
        // GET: Admin/NhaXuatBan
        QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        public ActionResult List()
        {
            if (Session["TenNhanVien"] == null)
                return RedirectToAction("Login", "Home");
            List<NhaXuatBan> dsnxb = da.NhaXuatBans.Select(s => s).ToList();
            return View(dsnxb);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection fcollection, NhaXuatBan nhaXuatBan)
        {
            var tenNXB = fcollection["TenNXB"];
            if (String.IsNullOrEmpty(tenNXB))
            {
                ViewData["Loi"] = "Tên NXB không được trống";
            }
            else
            {
                da.NhaXuatBans.Add(nhaXuatBan);
                da.SaveChanges();
                return RedirectToAction("List");
            }
            return this.Create();
        }
        public ActionResult Delete(int id)
        {
            NhaXuatBan nhaXuatBan = da.NhaXuatBans.FirstOrDefault(s => s.MaNXB == id);
            da.NhaXuatBans.Remove(nhaXuatBan);
            da.SaveChanges();
            return RedirectToAction("List");
        }
        public ActionResult Edit(int id)
        {
            NhaXuatBan nhaXuatBan = da.NhaXuatBans.FirstOrDefault(s => s.MaNXB == id);
            return View(nhaXuatBan);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection fcollection, int id)
        {
            NhaXuatBan nhaXuatBan = da.NhaXuatBans.FirstOrDefault(s => s.MaNXB == id);
            nhaXuatBan.TenNXB = fcollection["TenNXB"];
            nhaXuatBan.SĐT = fcollection["SĐT"];
            UpdateModel(nhaXuatBan);
            da.SaveChanges();
            return RedirectToAction("List");
        }
    }
}