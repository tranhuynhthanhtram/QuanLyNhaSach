using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyNhaSach.Models
{
    public class ViewModel
    {
        public KhachHang khachHang { get; set; }
        public ChiTietDonHang chiTietDonHang { get; set; }
        public DonHang donHang { get; set; }    
        public LoaiSach loaiSach { get; set; }
        public NhanVien nhanVien { get; set; }
        public Sach sach { get; set; }
        [DisplayFormat (DataFormatString="{0:0.##0}")]
        public double thanhTien { get; set; }
    }
}