using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaSach.Models;
using PagedList;

namespace QuanLyNhaSach.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Admin/Home
        QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        public ActionResult Menu(string currentFilter, int?page, string Search="")
        {
            if (Session["TenNhanVien"] == null)
                return RedirectToAction("Login", "Home");
            if (Search != "")
            {
                page = 1;
                List<Sach> tk = da.Saches.Select(s => s).Where(s => s.TenSach.ToUpper().Contains(Search.ToUpper())).ToList();
                return View(tk);
            }
            else
            {
                Search = currentFilter;
            }
            ViewBag.CurrentFilter = currentFilter;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            List<Sach> dss = da.Saches.Select(s => s).OrderBy(s=>s.TenSach).ToList();
            return View(dss.ToPagedList(pageNumber, pageSize));

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
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login(FormCollection formCollection)
        {
            var ten = formCollection["TenNhanVien"];
            var matKhau = GetMD5(formCollection["MatKhau"]);
            var sdt = formCollection["SĐT"];
            if (String.IsNullOrEmpty(ten))
            {
                ViewData["Loi1"] = "Tên nhân viên trống";
            }
            else
            {
                if (String.IsNullOrEmpty(sdt))
                {

                    ViewData["Loi2"] = "SĐT trống";
                }
                else
                {
                    NhanVien nv = da.NhanViens.SingleOrDefault(n => n.TenNhanVien.Equals(ten) && n.SĐT.Equals(sdt) && n.MatKhau.Equals(matKhau));
                    if (nv != null)
                    {
                        ViewBag.ThongBao = "Đăng nhập thành công";
                        Session["TenNhanVien"] = nv;
                        return RedirectToAction("Menu");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Đăng nhập thất bại";
                        return this.Login();
                    }
                }
            }
            return this.Login();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(FormCollection formCollection)
        {
            var tenNhanVien = formCollection["TenNhanVien"];
            if (String.IsNullOrEmpty(tenNhanVien))
            {
                ViewData["Loi"] = "Tên nhân viên không được trống";
            }
            else
            {
                if (String.IsNullOrEmpty(formCollection["MatKhau"]))
                    ViewData["Loi1"] = "Vui lòng nhập mất khẩu mới";
                else
                {
                    NhanVien nhanVien = da.NhanViens.SingleOrDefault(n => n.TenNhanVien.Equals(tenNhanVien));
                    if (nhanVien != null)
                    {
                        nhanVien.MatKhau = GetMD5(formCollection["MatKhau"]);
                        da.SaveChanges();
                    }
                    return RedirectToAction("Login");
                }
            }
            return this.ResetPassword();
        }
        
    }
}