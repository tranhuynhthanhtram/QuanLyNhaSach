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
    public class KhachHangController : BaseController
    {
        // GET: Admin/KhachHang
        QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        public ActionResult List()
        {
            if (Session["TenNhanVien"] == null)
                return RedirectToAction("Login", "Home");
            List<KhachHang> dskh = da.KhachHangs.Select(s => s).ToList();
            return View(dskh);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection fcollection, KhachHang khachHang)
        {
            var tenkhachHang = fcollection["TenKhachHang"];
            if (String.IsNullOrEmpty(tenkhachHang))
            {
                ViewData["Loi"] = "Tên khách hàng không được trống";
            }
            else
            {
                khachHang.MatKhau = GetMD5(fcollection["MatKhau"]);
                da.KhachHangs.Add(khachHang);
                da.SaveChanges();
                return RedirectToAction("List");
            }
            return this.Create();
        }
        public ActionResult Delete(int id)
        {
            KhachHang khachHang = da.KhachHangs.FirstOrDefault(s => s.MaKhachHang == id);
            da.KhachHangs.Remove(khachHang);
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
            KhachHang khachHang = da.KhachHangs.FirstOrDefault(s => s.MaKhachHang == id);
            return View(khachHang);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection fcollection, int id, KhachHang khachHang)
        {
            khachHang = da.KhachHangs.FirstOrDefault(s => s.MaKhachHang == id);
            khachHang.TenKhachHang = fcollection["TenKhachHang"];
            khachHang.MatKhau = GetMD5(fcollection["MatKhau"]);
            khachHang.SĐT = fcollection["SĐT"];
            khachHang.Email = fcollection["Email"];
            khachHang.DiaChi = fcollection["DiaChi"];
            da.Entry(khachHang).State = System.Data.Entity.EntityState.Modified;
            da.SaveChanges();
            return RedirectToAction("List");
        }
    }
}