using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyNhaSach.Models
{
    public class Cart
    {
        private QuanLyNhaSachEntities da = new QuanLyNhaSachEntities();
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public decimal GiaTien { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien { get { return SoLuong*GiaTien; } }
        public Cart(int maSach)
        {
            this.MaSach = maSach;
            Sach sach=da.Saches.Single(n=>n.MaSach == maSach);
            TenSach = sach.TenSach;
            GiaTien = (decimal)sach.GiaTien;
            SoLuong = 1;
        }
    }
}