using QuanLyNhaSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyNhaSach.Areas.Admin.Controllers
{
    public class OrderDetailsController : Controller
    {
        // GET: Admin/OrderDetails
       

        QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        public ActionResult List(int MaDH)
        {
            List<KhachHang> khachHang = da.KhachHangs.ToList();
            List<DonHang> donHang = da.DonHangs.ToList();
            List<NhanVien> nhanVien = da.NhanViens.ToList();
            List<Sach> sach = da.Saches.ToList();
            List<ChiTietDonHang> chiTietDonHang = da.ChiTietDonHangs.ToList();
            var m = from d in donHang
                    join k in khachHang on d.MaKH equals k.MaKhachHang 
                    where (d.MaDH == MaDH)
                    select new ViewModel
                    {
                        donHang = d,
                        khachHang = k
                    };
            var s = from c in chiTietDonHang
                    join b in sach on c.MaSach equals b.MaSach
                    where c.MaDH == MaDH
                    select new ViewModel
                    {
                        chiTietDonHang = c,
                        sach = b,
                        thanhTien = Convert.ToDouble(c.GiaTien * c.SoLuong)
                    };
            //Truyền sang View
            ViewBag.Main = m;
            ViewBag.Sub = s;
            return View();
        }
        public ActionResult Edit(int id)
        {
            ChiTietDonHang chiTietDonHang = da.ChiTietDonHangs.FirstOrDefault(s => s.MaSach == id);
            int soLuong = (int)(chiTietDonHang.SoLuong - 1);
            chiTietDonHang.SoLuong = (short)soLuong;
            
                
                Sach sach = da.Saches.FirstOrDefault(s => s.MaSach == id);
                sach.SoLuong++;
            
            UpdateModel(chiTietDonHang);
            da.SaveChanges();
            return RedirectToAction("List", "Order");
        }
    }
}