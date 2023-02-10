using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Areas.Admin.Controllers
{
    public class NhanVienController : BaseController
    {
        // GET: Admin/NhanVien
        QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        public ActionResult List()
        {
            if (Session["TenNhanVien"] == null)
                return RedirectToAction("Login", "Home");
            List<NhanVien> dskh = da.NhanViens.Select(s => s).ToList();
            return View(dskh);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection fcollection, NhanVien nhanVien)
        {
            var tenNhanVien = fcollection["TenNhanVien"];
            if (String.IsNullOrEmpty(tenNhanVien))
            {
                ViewData["Loi"] = "Tên nhân viên không được trống";
            }
            else
            {
                nhanVien.MatKhau = GetMD5(fcollection["MatKhau"]);
                da.NhanViens.Add(nhanVien);
                da.SaveChanges();
                return RedirectToAction("List");
            }
            return this.Create();
        }
        public ActionResult Delete(int id)
        {
            NhanVien nhanVien = da.NhanViens.FirstOrDefault(s => s.MaNhanVien == id);
            da.NhanViens.Remove(nhanVien);
            da.SaveChanges();
            return RedirectToAction("List");
        }
        //public static string GetMD5(string str)
        //{
        //    MD5 md5 = new MD5CryptoServiceProvider();
        //    byte[] fromData = Encoding.UTF8.GetBytes(str);
        //    byte[] targetData = md5.ComputeHash(fromData);
        //    string byteToString = null;

        //    for (int i = 0; i < targetData.Length; i++)
        //    {
        //        byteToString += targetData[i].ToString("x2");
        //    }
        //    return byteToString;
        //}
        public ActionResult Edit(int id)
        {
            NhanVien nhanVien = da.NhanViens.FirstOrDefault(s => s.MaNhanVien == id);
            return View(nhanVien);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection fcollection, int id, NhanVien nhanVien)
        {
            nhanVien = da.NhanViens.FirstOrDefault(s => s.MaNhanVien == id);
            nhanVien.TenNhanVien = fcollection["TenNhanVien"];
            nhanVien.MatKhau = GetMD5(fcollection["MatKhau"]);
            nhanVien.SĐT = fcollection["SĐT"];
            da.Entry(nhanVien).State=System.Data.Entity.EntityState.Modified;
            da.SaveChanges();
            return RedirectToAction("List");
        }
        
    }
}