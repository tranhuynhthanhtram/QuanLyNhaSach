using QuanLyNhaSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace QuanLyNhaSach.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        private QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        // GET: Admin/Cart
        public List<Cart> GetListCarts() //Lấy danh sách giỏ hàng
        {
            List<Cart> carts = Session["Cart"] as List<Cart>;
            if (carts == null) //Nếu chưa có SP trong giỏ hàng
            {
                carts = new List<Cart>(); //Tạo mới giỏ hàng
                Session["Cart"] = carts;
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
            if (carts.Count == 0) //Nếu giỏ hàng chưa có SP
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
                if (c.SoLuong == 0)
                {
                    carts.RemoveAll(s => s.MaSach == id);
                }
                return RedirectToAction("List");
            }
            if (carts.Count == 0)
            {
                return RedirectToAction("List", "Menu");
            }
            return RedirectToAction("List");
        }
        public ActionResult Order(FormCollection formCollection, string Email)
        {
            List<Cart> carts = GetListCarts();
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    DonHang donHang = new DonHang();
                    if (Session["TenKhachHang"] != null)
                    {
                        KhachHang kh = (KhachHang)Session["TenKhachHang"];

                        donHang.NgayLapDH = DateTime.Now;
                        donHang.MaKH = kh.MaKhachHang;
                        da.DonHangs.Add(donHang);
                        da.SaveChanges();
                    }
                    if (Session["TenKhachHang"] == null)
                    {
                        return RedirectToAction("Login", "Home");

                    }

                    //List<Cart> carts = GetListCarts();
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
                        //da.SaveChanges();
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
                //return RedirectToAction("Menu", "Home");
                string sMsg = "<html>" +
                "           <body>  " +
                "               <table>" +
                "                   <caption>Thông tin đơn hàng</caption>" +
                "                   <tr>" +
                    "                   <th>STT</th>" +
                    "                   <th>Tên sách</th>" +
                    "                   <th>Số lượng</th>" +
                    "                   <th>Đơn giá</th>" +
                    "                   <th>Thành tiền</th>" +
                "                   </tr>";
                int i = 0;
                decimal tongTien = 0;
                foreach (var item in carts)
                {
                    i++;
                    sMsg += "<tr>";
                    sMsg += "<td>" + i.ToString() + "</td>";
                    sMsg += "<td>" + item.TenSach + "</td>";
                    sMsg += "<td>" + item.SoLuong + "</td>";
                    sMsg += "<td>" + item.GiaTien.ToString() + "</td>";
                    sMsg += "<td>" + String.Format("{0:#,###}", item.SoLuong * item.GiaTien) + "</td>";
                    sMsg += "<tr>";
                    tongTien += item.SoLuong * item.GiaTien;
                }//Gửi mail cho KH
                sMsg += "<tr>" +
                    "       <th>Tổng cộng:" + String.Format("{0:#,###}", tongTien) + "</th></tr></table>";
                MailMessage mail = new MailMessage("diachiemailnguoigui@gmail.com", Email, "Thông tin đơn hàng", sMsg);
                mail.To.Add(new MailAddress("tram.tran.1112001@gmail.com"));//địa chỉ mail người nhận
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                //client.Credentials = new NetworkCredential("1951052209tram@ou.edu.vn", "thanhtram");
                client.Credentials = new NetworkCredential(Email, "nqhfxwcunohifqak");
                mail.IsBodyHtml = true;
                client.Send(mail);
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
            //    sMsg += "<tr>";
            //    tongTien += item.SoLuong * item.GiaTien;
            //}//Gửi mail cho KH
            //sMsg += "<tr>" +
            //    "       <th>Tổng cộng:" + String.Format("{0:#,###}", tongTien) + "</th></tr></table>";
            //MailMessage mail = new MailMessage("diachiemailnguoigui@gmail.com", Email, "Thông tin đơn hàng", sMsg);
            //mail.To.Add(new MailAddress("tram.tran.1112001@gmail.com"));//địa chỉ mail người nhận
            //SmtpClient client = new SmtpClient("smpt.gmail.com", 587);
            //client.EnableSsl = true;
            //client.UseDefaultCredentials = false;
            //client.Credentials = new NetworkCredential("1951052209tram@ou.edu.vn", "thanhtram");
            //mail.IsBodyHtml = true;
            //client.Send(mail);
            //return RedirectToAction("Menu", "Home");




        }
    }
}