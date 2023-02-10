using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaSach.Models;
using System.Transactions;
using System.Net.Mail;
using System.Net;

namespace QuanLyNhaSach.Areas.Admin.Controllers
{
    public class CartController : Controller
    {
        private QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        // GET: Admin/Cart
        public List<Cart> GetListCarts() //Lấy danh sách giỏ hàng
        {
            List<Cart> carts = Session["Cart"] as List<Cart>;
            if (carts == null) //Nếu chưa có SP trong giỏ hàng
            {
                carts = new List<Cart>(); //Tạo mới giỏ hàng
                Session["Cart"]=carts;
            }
            return carts;
        }
        public ActionResult AddCart(int id)
        {
            //Kiểm tra giỏ hàng có SP chưa
            List<Cart> carts = GetListCarts(); //Lấy danh sách giỏ hàng
            Cart c = carts.Find(s => s.MaSach == id);
            if (c == null) //Nếu giỏ hàng chưa có SP
            {
                c = new Cart(id); //Thêm SP và giỏ hàng
                carts.Add(c);
            }
            else //đã có
            {
                c.SoLuong++;
            }
            return RedirectToAction("List");
        }
        public ActionResult List() //Hiển thị giỏ hàng
        {
            List<Cart> carts = GetListCarts();
            if(carts.Count == 0) //Nếu giỏ hàng chưa có SP
            {
                return RedirectToAction("Menu", "Home");
            }
            ViewBag.CountProduct = carts.Sum(s => s.SoLuong);
            ViewBag.Total = carts.Sum(s => s.ThanhTien);
            return View(carts);
        }
        public ActionResult Delete(int id)
        {
            List<Cart> carts = GetListCarts(); //Lấy danh sách giỏ hàng
            Cart c = carts.Find(s => s.MaSach == id); //Tìm SP muốn xóa trong giỏ hàng
            if (c != null) //Nếu tìm thấy SP
            {
               c.SoLuong--;
                if(c.SoLuong == 0)
                {
                    carts.RemoveAll(s=>s.MaSach==id);
                }
                return RedirectToAction("List");
            }
            if (carts.Count == 0)
            {
                return RedirectToAction("List", "Menu");
            }
            return RedirectToAction("List");
        }
        public ActionResult Order(FormCollection form) 
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    DonHang donHang = new DonHang();
                    if (Session["TenNhanVien"] != null)
                    {
                        NhanVien nv = (NhanVien)Session["TenNhanVien"];
                        donHang.NgayLapDH = DateTime.Now;
                        donHang.MaNhanVien = nv.MaNhanVien;
                        da.DonHangs.Add(donHang);
                        da.SaveChanges();
                    }
                    if (Session["TenNhanVien"] == null)
                    {
                        return RedirectToAction("Login", "Home");
                    }

                    List<Cart> carts = GetListCarts();
                    foreach (var item in carts)
                    {
                        ChiTietDonHang chiTietDonHang = new ChiTietDonHang();
                        chiTietDonHang.MaDH = donHang.MaDH;
                        chiTietDonHang.MaSach = item.MaSach;
                        chiTietDonHang.SoLuong = short.Parse(item.SoLuong.ToString());
                        chiTietDonHang.GiaTien = item.GiaTien;
                        da.ChiTietDonHangs.Add(chiTietDonHang);

                        Sach sach = da.Saches.Find(item.MaSach);
                        sach.SoLuong -= item.SoLuong;
                        da.SaveChanges();
                    }
                    da.SaveChanges();
                    transactionScope.Complete();
                    Session["Cart"] = null;
                }
                catch
                {
                    transactionScope.Dispose();
                    return RedirectToAction("List");
                }
                return RedirectToAction("Menu", "Home");
            }


            //List<Cart> carts = GetListCarts();
            //string sMsg = "<html>" +
            //    "           <body>  " +
            //    "               <table>" +
            //    "                   <caption>Thông tin đơn hàng</caption>" +
            //    "                   <tr>" +
            //        "                   <th>STT</th>" +
            //        "                   <th>Tên sách</th>" +
            //        "                   <th>Số lượng</th>" +
            //        "                   <th>Đơn giá</th>" +
            //        "                   <th>Thành tiền</th>" +
            //    "                   </tr>";
            //int i = 0;
            //decimal tongTien = 0;
            //foreach (var item in carts)
            //{
            //    i++;
            //    sMsg += "<tr>";
            //    sMsg += "<td>" + i.ToString() + "</td>";
            //    sMsg += "<td>" + item.TenSach + "</td>";
            //    sMsg += "<td>" + item.SoLuong + "</td>";
            //    sMsg += "<td>" + item.GiaTien.ToString() + "</td>";
            //    sMsg += "<td>" + String.Format("{0:#,###}", item.SoLuong * item.GiaTien) + "</td>";
            //    sMsg += "</tr>";
            //    tongTien += item.SoLuong * item.GiaTien;
            //}//Gửi mail cho KH
            //sMsg += "<tr>" +
            //            "       <th>Tổng cộng:" + String.Format("{0:#,###}", tongTien) + "</th></tr></table>";
            //MailMessage mail = new MailMessage();
            //mail.From = new MailAddress(form["Email"]);//địa chỉ mail người gửi
            //mail.Subject = "Thông tin đơn hàng";
            //mail.To.Add(new MailAddress("1951052209tram@ou.edu.vn"));//địa chỉ mail người nhận
            //mail.Body = sMsg;
            //mail.IsBodyHtml = true;
            //SmtpClient client = new SmtpClient("smpt.gmail.com")
            //{
            //    Port = 587,
            //    //client.UseDefaultCredentials = false;
            //    Credentials = new NetworkCredential(form["Email"], "nqhfxwcunohifqak"),//người gửi 
            //    EnableSsl = true,
            //};
            //client.Send(mail);
        }
    }
}