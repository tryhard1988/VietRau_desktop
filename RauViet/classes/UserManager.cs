using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using RauViet.ui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using DataTable = System.Data.DataTable;

namespace RauViet.classes
{
    public sealed class UserManager
    {
        private static UserManager ins = null;
        private static readonly object padlock = new object();

        public string userName = "";
        public string machineInfo = "";
        public string password = "";
        public int employeeID = -1;
        public string employeeCode = "";
        public string fullName = "";
        public string fullName_NoSign = "";
        public List<string> roleCodes = new List<string>();

        private UserManager() { }

        public static UserManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (ins == null)
                        ins = new UserManager();
                    return ins;
                }
            }
        }

        public void init(string user, string password, DataRow userInfo)
        {
            this.userName = user;
            this.password = password;

            if (userInfo != null)
            {
                this.employeeID = Convert.ToInt32(userInfo["EmployeeID"]);
                this.employeeCode = userInfo["EmployeeCode"].ToString();
                this.fullName = userInfo["FullName"].ToString();
                this.fullName_NoSign = Utils.RemoveVietnameseSigns(this.fullName).Replace(" ", "");
                string roleCodeStr = userInfo["RoleCodes"].ToString();

                if (!string.IsNullOrWhiteSpace(roleCodeStr))
                {
                    roleCodes = roleCodeStr
                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim())
                                .ToList();
                }

            }
        }

        public bool hasRole (string roleCode)
        {
            if (roleCodes.Contains(roleCode, StringComparer.OrdinalIgnoreCase))
                return true;
            return false;
        }

        public bool hasRole_User(){return hasRole("ql_user");}
        public bool hasRole_NhanSu(){ return hasRole("qlns");}
        public bool hasRole_KhachHang() { return hasRole("qlkh"); }
        public bool hasRole_SanPhamChinh() { return hasRole("spc"); }
        public bool hasRole_SanPhamQuyCach() { return hasRole("spqc"); }
        public bool hasRole_AnGiaSanPham() { return hasRole("agsp"); }
        public bool hasRole_HoanThanhDonHang() { return hasRole("htdh"); }
        public bool hasRole_NhapLieuDonHang() { return hasRole("nldh"); }
        public bool hasRole_DangKyKiemDich() { return hasRole("xuat_dkkd"); }
        public bool hasRole_PHYTO() { return hasRole("xuat_phyto"); }
        public bool hasRole_Chot_PHYTO() { return hasRole("xuat_c_phyto"); }
        public bool hasRole_Invoice() { return hasRole("xuat_invoice"); }
        public bool hasRole_PackingTotal() { return hasRole("xuat_pt"); }
        public bool hasRole_CustomerDetailPacking() { return hasRole("xuat_cdp"); }
        public bool hasRole_ChamCong() { return hasRole("cc"); }
        public bool hasRole_ThongKe() { return hasRole("tknsvk"); }
        public bool hasRole_CreateQR() { return hasRole("tqr"); }
        public void reset()
        {
            this.userName = "";
            this.password = "";
        }

        public void setUserName(string userName) { 
            this.userName = userName;           
        }

        public void setPassword(string password)
        {
            this.password = password;
        }
    }
}
