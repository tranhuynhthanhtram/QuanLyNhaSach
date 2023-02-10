using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaSach.Models;
using PagedList;
using SelectPdf;

namespace QuanLyNhaSach.Areas.Admin.Controllers
{
    public class SachController : BaseController
    {
        // GET: Admin/Sach
        QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        public ActionResult List(string orderSort, int?page)
        {
            if (Session["TenNhanVien"] == null)
                return RedirectToAction("Login", "Home");
            ViewBag.SortName = String.IsNullOrEmpty(orderSort) ? "ten_desc" : "";
            ViewBag.SortPrice = orderSort == "giatien" ? "giatien_desc" : "giatien";
            ViewBag.CurrentSort=orderSort;
            List<Sach> dss = da.Saches.Select(s => s).ToList();
            switch(orderSort)
            {
                case "ten_desc":
                    dss = dss.OrderByDescending(s => s.TenSach).ToList();
                    break;
                case "giatien_desc":
                    dss = dss.OrderByDescending(s => s.GiaTien).ToList();
                    break;
                case "giatien":
                    dss = dss.OrderBy(s => s.GiaTien).ToList();
                    break;
                default: //Mặc định sắp xếp theo tên sách
                    dss = dss.OrderBy(s => s.TenSach).ToList();
                    break;
            }
            int pageSize = 10;
            int pageNumber = page ?? 1;
            return View(dss.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            ViewData["TenTacGia"] = new SelectList(da.TacGias, "MaTacGia", "TenTacGia");
            ViewData["TenLoaiSach"] = new SelectList(da.LoaiSaches, "MaLoaiSach", "TenLoaiSach");
            ViewData["TenNXB"] = new SelectList(da.NhaXuatBans, "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection fcollection, Sach sach, HttpPostedFileBase upHinh)
        {
            var tenSach = fcollection["TenSach"];
            if (String.IsNullOrEmpty(tenSach))
            {
                ViewData["Loi"] = "Tên sach không được trống";
            }
            else
            {
                sach.MaTacGia = int.Parse(fcollection["TenTacGia"]);
                sach.MaLoaiSach = int.Parse(fcollection["TenLoaiSach"]);
                sach.MaNXB = int.Parse(fcollection["TenNXB"]);
                da.Saches.Add(sach);
                da.SaveChanges();

                if (upHinh != null && upHinh.ContentLength > 0)
                {
                    int id=int.Parse(da.Saches.ToList().Last().MaSach.ToString());

                    string fileName = "";
                    int index=upHinh.FileName.IndexOf('.');
                    fileName = "sach" + id.ToString() + "." + upHinh.FileName.Substring(index + 1);
                    string path=Path.Combine(Server.MapPath("~/Images"),fileName);
                    upHinh.SaveAs(path);

                    sach = da.Saches.FirstOrDefault(S => S.MaSach == id);
                    sach.Anh = fileName;
                    da.SaveChanges();

                }
                return RedirectToAction("List");
            }
            return this.Create();
        }
        public ActionResult Details(int id)
        {
            Sach sach = da.Saches.Where(s => s.MaSach == id).FirstOrDefault();
            var bestSales = da.Pro_BestSales().Take(3);
            ViewBag.Sub = bestSales;
            return View(sach);
        }
        public ActionResult Delete(int id)
        {
            Sach sach = da.Saches.FirstOrDefault(s => s.MaSach == id);
            da.Saches.Remove(sach);
            da.SaveChanges();
            return RedirectToAction("List");
        }
        public ActionResult Edit(int id)
        {
            Sach sach = da.Saches.FirstOrDefault(s => s.MaSach == id);
            ViewData["TenTacGia"] = new SelectList(da.TacGias, "MaTacGia", "TenTacGia");
            ViewData["TenLoaiSach"] = new SelectList(da.LoaiSaches, "MaLoaiSach", "TenLoaiSach");
            ViewData["TenNXB"] = new SelectList(da.NhaXuatBans, "MaNXB", "TenNXB");
            return View(sach);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection fcollection, int id, HttpPostedFileBase upHinh, Sach sach, int SoLuong)
        {
            //soLuong = (int)(sach.SoLuong + int.Parse(fcollection["SoLuong"]));
            //soLuong = int.Parse(fcollection["SoLuong"]);
            //if (int.Parse(fcollection["SoLuong"]) < 0)
            //{
            //    ViewData["LoiNhapSoLuong"] = "Số lượng phải lớn hơn 0";
            //    return this.Edit(id);
            //}
            //else
            //{
            //    sach.SoLuong = soLuong;
            //}
                
            
            sach = da.Saches.FirstOrDefault(s => s.MaSach == id);
            sach.TenSach = fcollection["TenSach"];
            sach.MaTacGia = int.Parse(fcollection["TenTacGia"]);
            sach.MaLoaiSach = int.Parse(fcollection["TenLoaiSach"]);
            sach.MaNXB = int.Parse(fcollection["TenNXB"]);
            sach.GiaTien = Decimal.Parse(fcollection["GiaTien"]);
            //sach.SoLuong += int.Parse(fcollection["SoLuong"]);
            sach.SoLuong += SoLuong;

            if (upHinh != null && upHinh.ContentLength > 0)
            {
                int id1 = sach.MaSach;

                string fileName = "";
                int index = upHinh.FileName.IndexOf('.');
                fileName = "sach" + id1.ToString() + "." + upHinh.FileName.Substring(index + 1);
                string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                sach.Anh = fileName;
                upHinh.SaveAs(path);
            }

            UpdateModel(sach);
            da.SaveChanges();
            return RedirectToAction("List");
        }
        //public List<Pro_ThongKeDoanhThuTheoThang_Result> GetReportByMonth(int month)
        //{
        //    var lsData = da.Pro_ThongKeDoanhThuTheoThang(month);
        //    return lsData.ToList();
        //}
        public ActionResult GetReportByMonth(int month)
        {
            var lsData = da.Pro_ThongKeDoanhThuTheoThang(month);
            return Json(lsData, JsonRequestBehavior.AllowGet);
        }   
        public ActionResult ExportPdf()
        {
            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = PdfPageSize.A4; // mặc định in trang A4
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait; // chiều trang giấy _ thẳng
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;

            List<Sach> dss = da.Saches.Select(s => s).ToList();
            var ls = dss.ToPagedList(1, 1000000);
            var htmlPdf = base.RenderPartialToString("~/Areas/Admin/Views/Sach/PatialViewPdfResult.cshtml", ls, ControllerContext);

            // create a new pdf document converting an html string
            PdfDocument doc = converter.ConvertHtmlString(htmlPdf);

            string fileName = string.Format("{0}.pdf", DateTime.Now.Ticks);
            string path = string.Format("{0}/{1}", Server.MapPath("~/File"), fileName);

            // save pdf document
            doc.Save(path);

            // close pdf document
            doc.Close();
            return Json(1, JsonRequestBehavior.AllowGet);
        }
      
    }
}