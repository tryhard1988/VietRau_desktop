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
        public string password = "";
        public int employeeID = -1;
        public string employeeCode = "";
        public string fullName = "";
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
            return roleCodes.Contains(roleCode, StringComparer.OrdinalIgnoreCase);
        }

        public bool hasRole_User(){return hasRole("ql_user");}
        public bool hasRole_NhanSu(){ return true; hasRole("qlns");}
        public bool hasRole_KhachHang() { return hasRole("qlkh"); }
        public bool hasRole_SanPham() { return hasRole("qlsp"); }
        public bool hasRole_HoanThanhDonHang() { return hasRole("htdh"); }
        public bool hasRole_NhapLieuDonHang() { return  hasRole("nldh"); }
        public bool hasRole_XuatExcelGuiKH() { return hasRole("xegkh"); }
        public bool hasRole_ChamCong() { return true; hasRole("xegkh"); }

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
