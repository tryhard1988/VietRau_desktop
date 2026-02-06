
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

        public bool hasRoles(params string[] roleCodes)
        {
            foreach (string roleCode in roleCodes)
            {
                if (this.roleCodes.Contains(roleCode, StringComparer.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public bool hasRole_User(){return hasRole("ql_user");}
        public bool hasRole_NhanSu(){ return hasRole("qlns");}
        public bool hasRole_KhachHang() { return hasRole("qlkh"); }
        public bool hasRole_SanPhamChinh() { return hasRole("spc"); }
        public bool hasRole_SanPhamQuyCach() { return hasRole("spqc"); }
        public bool hasRole_NhapKhoRauCuQua() { return hasRole("nk_rcq"); }
        public bool hasRole_NhapKhoHangKho() { return hasRole("nk_hkho"); }
        public bool hasRole_AnGiaSanPham() { return hasRole("agsp"); }
        public bool hasRole_HoanThanhDonHang() { return hasRole("htdh"); }
        public bool hasRole_NhapDonNuocNgoai() { return hasRole("nldh"); }
        public bool hasRole_BanThanhLiChoNV() { return hasRole("btlcnv"); }
        public bool hasRole_SuaDonNuocNgoai() { return hasRole("csdnn"); }
        public bool hasRole_DangKyKiemDich() { return hasRole("xuat_dkkd"); }
        public bool hasRole_PHYTO() { return hasRole("xuat_phyto"); }
        public bool hasRole_Chot_PHYTO() { return hasRole("xuat_c_phyto"); }
        public bool hasRole_Invoice() { return hasRole("xuat_invoice"); }
        public bool hasRole_PackingTotal() { return hasRole("xuat_pt"); }
        public bool hasRole_CustomerDetailPacking() { return hasRole("xuat_cdp"); }
        public bool hasRole_ChamCongHanhChanh() { return hasRole("cc"); }
        public bool hasRole_ChamCongTangCa() { return hasRoles("cctc_28", "cctc_29", "cctc_26", "cctc_24", "cctc_25", "cctc_27"); }
        public bool hasRole_CacKhoanTru() { return hasRoles("tt_rau", "tt_ULuong", "tt_CCan", "tt_CEP", "tt_Other"); }
        public bool hasRole_TruTienRau() { return hasRole("tt_rau"); }
        public bool hasRole_TruUngLuong() { return hasRole("tt_ULuong"); }
        public bool hasRole_TruChuyenCan() { return hasRole("tt_CCan"); }
        public bool hasRole_TruCEP() { return hasRole("tt_CEP"); }
        public bool hasRole_TruKhac() { return hasRole("tt_Other"); }
        public bool hasRole_ThongKe() { return hasRole("tknsvk"); }
        public bool hasRole_CreateQR() { return hasRole("tqr"); }
        public bool hasRole_NhapDonTrongNuoc() { return hasRole("ndtn"); }
        public bool hasRole_QLK_VatTu() { return hasRole("qlk_vt"); }
        public bool hasRole_SuaDonTrongNuoc() { return hasRole("csdtn"); }
        public int[] get_ChamCongTangCa_Departments()
        {
            int[] departmentIDs = roleCodes.Where(r => r.StartsWith("cctc_", StringComparison.OrdinalIgnoreCase))
                                        .Select(r =>
                                        {
                                            string numberPart = r.Substring(5); // bỏ "cctc_"
                                            return int.TryParse(numberPart, out int n) ? n : (int?)null;
                                        })
                                        .Where(n => n.HasValue)
                                        .Select(n => n.Value)
                                        .ToArray();

            return departmentIDs;
        }
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
