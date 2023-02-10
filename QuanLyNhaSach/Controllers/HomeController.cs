using QuanLyNhaSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PagedList;
using QuanLyNhaSach.Areas.Admin.Controllers;

namespace QuanLyNhaSach.Controllers
{
    public class HomeController : BaseController
    {
        QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        public ActionResult Menu(string currentFilter, int? page, string Search ="")
        {
            if (Search != "")
            {
                page = 1;
                List<Sach> tk = da.Saches.Select(s => s).Where(s => s.TenSach.ToUpper().Contains(Search.ToUpper())).ToList();
                tk = tk.OrderBy(s => s.TenSach).ToList();
                int pageSize1 = tk.Count();
                int pageNumber1 = (page ?? 1);
                return View(tk.ToPagedList(pageNumber1, pageSize1));
            }
            else
            {
                Search = currentFilter;
            }
            ViewBag.CurrentFilter = currentFilter;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            List<Sach> dss = da.Saches.Select(s => s).OrderBy(s => s.TenSach).ToList();
            return View(dss.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Details(int id)
        {
            Sach sach = da.Saches.Where(s => s.MaSach == id).FirstOrDefault();
            var bestSales = da.Pro_BestSales().Take(3);
            ViewBag.Sub = bestSales;
            return View(sach);
        }
        [HttpPost]
        public ActionResult Details(FormCollection form, int id, string Comment)
        {
            Sach sach = da.Saches.Where(s => s.MaSach == id).FirstOrDefault();
            if (Session["TenKhachHang"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            BinhLuan binhLuan = new BinhLuan();
            binhLuan.MaSach = id;
            binhLuan.NoiDungBinhLuan = Comment;
            binhLuan.NgayBinhLuan = DateTime.Now;
            binhLuan.MaKH = (int)Session["MaKH"];

            da.BinhLuans.Add(binhLuan);
            da.SaveChanges();
            return this.Details(id);
            
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
            var ten = formCollection["TenKhachHang"];
            var matKhau = GetMD5(formCollection["MatKhau"]);
            var sdt = formCollection["SĐT"];
            if (String.IsNullOrEmpty(ten))
            {
                ViewData["Loi1"] = "Tên khách hàng trống";
            }
            else
            {
                if (String.IsNullOrEmpty(sdt))
                {

                    ViewData["Loi2"] = "SĐT trống";
                }
                else
                {
                    KhachHang khachHang = da.KhachHangs.SingleOrDefault(n => n.TenKhachHang.Equals(ten) && n.SĐT.Equals(sdt) && n.MatKhau.Equals(matKhau));
                    if (khachHang != null)
                    {
                        ViewBag.ThongBao = "Đăng nhập thành công";
                        Session["TenKhachHang"] = khachHang;
                        Session["MaKH"] = khachHang.MaKhachHang;
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
            var tenKhachHang = formCollection["TenKhachHang"];
            if (String.IsNullOrEmpty(tenKhachHang))
            {
                ViewData["Loi"] = "Tên khách hàng không được trống";
            }
            else
            {
                if (String.IsNullOrEmpty(formCollection["MatKhau"]))
                    ViewData["Loi1"] = "Vui lòng nhập mất khẩu mới";
                else
                {
                    KhachHang kh = da.KhachHangs.SingleOrDefault(n => n.TenKhachHang.Equals(tenKhachHang));
                    if (kh != null)
                    {
                        kh.MatKhau = GetMD5(formCollection["MatKhau"]);
                        da.SaveChanges();
                    }
                    return RedirectToAction("Login");
                }
            }
            return this.ResetPassword();
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(KhachHang khachHang, FormCollection form)
        {
            var tenKH = form["TenKhachHang"];
            var email = form["Email"];
            var sdt = form["SĐT"];
            var diaChi = form["DiaChi"];
            var matKhau = form["MatKhau"];
            if (String.IsNullOrEmpty(tenKH))
                ViewData["LoiTenKH"] = "Vui lòng nhập tên khách hàng";
           
            if (String.IsNullOrEmpty(email))
                ViewData["LoiEmail"] = "Vui lòng nhập email";
            if (String.IsNullOrEmpty(diaChi))
                ViewData["LoiDiaChi"] = "Vui lòng nhập địa chỉ";
            if (String.IsNullOrEmpty(sdt))
                ViewData["LoiSĐT"] = "Vui lòng nhập SĐT";
            if (String.IsNullOrEmpty(matKhau))
                ViewData["LoiMatKhau"] = "Vui lòng nhập mật khẩu";
            else
            {
                khachHang.MatKhau = matKhau;
                khachHang.SĐT = sdt;
                khachHang.DiaChi = diaChi;
                khachHang.Email = email;
                khachHang.TenKhachHang=tenKH;
                da.KhachHangs.Add(khachHang);
                da.SaveChanges();
                return RedirectToAction("Login");
            }
            return this.Register();
        }

    }
}