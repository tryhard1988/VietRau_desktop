using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class FormManager : Form
    {
        public FormManager()
        {
            InitializeComponent();

            Task.Run(() =>SQLManager.Instance.AutoUpsertAnnualLeaveMonthListAsync());


            this.WindowState = FormWindowState.Maximized;
            this.Text = "Quản Lý Việt Rau - NV: " + UserManager.Instance.fullName + " [" + UserManager.Instance.employeeCode + "]";

            if (UserManager.Instance.hasRole_User())
                user_mi.Click += user_mi_Click;
            else
                user_mi.Visible = false;

            if (UserManager.Instance.hasRole_NhanSu())
            {
                employee_mi.Click += employee_mi_Click;
                department_mi.Click += department_mi_Click;
                position_mi.Click += position_mi_Click;
            }
            else
            {
                nhansu_group_mi.Visible = false;
            }

            if (UserManager.Instance.hasRole_KhachHang())
                KhachHang_menuItem.Click += khachhang_btn_Click;
            else
                KhachHang_menuItem.Visible = false;

            if (UserManager.Instance.hasRole_SanPham())
            {
                productMain_menuitem.Click += Products_SKU_btn_Click;
                productPacking_meniitem.Click += productPacking_btn_Click;
            }
            else
                sanpham_group_mi.Visible = false;

            if (UserManager.Instance.hasRole_NhapLieuDonHang())
            {
                exportCode_menuItem.Click += exportCode_btn_Click;
                order_menuitem.Click += others_btn_Click;
                dsDongThung_menuitem.Click += orderpackingList_btn_Click;
                do417_menuitem.Click += orderTotal_btn_Click;
                lotCode_menuitem.Click += lotCode_btn_Click;
            }
            else if (UserManager.Instance.hasRole_HoanThanhDonHang())
            {
                exportCode_menuItem.Click += exportCode_btn_Click;
                order_menuitem.Visible = false;
                dsDongThung_menuitem.Visible = false;
                do417_menuitem.Visible = false;
                lotCode_menuitem.Visible = false;
            }
            else
            {
                donhang_group_mi.Visible = false;
            }

            if (UserManager.Instance.hasRole_XuatExcelGuiKH())
            {
                dkkd_menuitem.Click += dkkd_btn_Click;
                phyto_menuitem.Click += phyto_btn_Click;
                invoice_menuitem.Click += invoice_btn_Click;
                packingTotal_menuitem.Click += packingTotal_btn_Click;
                customerDetailPacking_mi.Click += customerDetailPacking_mi_Click;
            }
            else
                xuatExcelGuiKH_Group_mi.Visible = false;

            if (UserManager.Instance.hasRole_ChamCong())
            {
                attendanceHC_mi.Click += attendanceHC_mi_Click;
                holiday_mi.Click += Holiday_mi_Click;
                overtimeType_mi.Click += OvertimeType_mi_Click;
                overtimeAttendace_mi.Click += OvertimeAttendace_mi_Click;
                annualLeaveBalance_mi.Click += AnnualLeaveBalance_mi_Click;
                leaveAttendance_mi.Click += LeaveAttendance_mi_Click;
            }
            else
                chamcong_pmi.Visible = false;
            
            title_lb.Text = "";
        }

        private void SwitchChildForm<T>(string title) where T : Form, new()
        {
            // Lưu form hiện tại nếu có
            if (this.content_panel.Controls.Count > 0)
            {
                if (this.content_panel.Controls[0] is ICanSave currentForm)
                    currentForm.SaveData();

                this.content_panel.Controls.Clear();
            }

            // Tạo form mới
            var form = new T();
            title_lb.Text = title;
            form.TopLevel = false;
            form.Parent = this.content_panel;
            form.Dock = DockStyle.Fill;

            // Nếu form con có method ShowData, gọi bằng reflection hoặc interface
            var showDataMethod = form.GetType().GetMethod("ShowData");
            showDataMethod?.Invoke(form, null);

            form.Show();
        }

        private void LeaveAttendance_mi_Click(object sender, EventArgs e)
        {
            SwitchChildForm<LeaveAttendance>("Bảng Đơn Nghỉ Phép");
        }

        private void AnnualLeaveBalance_mi_Click(object sender, EventArgs e)
        {
            SwitchChildForm<AnnualLeaveBalance>("Bảng Tồn Phép Nghỉ");
        }

        private void OvertimeAttendace_mi_Click(object sender, EventArgs e)
        {
            SwitchChildForm<OvertimeAttendace>("Bảng Chấm Công Tăng Ca");
        }
        private void OvertimeType_mi_Click(object sender, EventArgs e)
        {
            SwitchChildForm<OvertimeType>("Bảng Loại Tăng Ca");
        }

        private void Holiday_mi_Click(object sender, EventArgs e)
        {
            SwitchChildForm<Holidays>("Lập Ngày Nghỉ Lễ");
        }

        private void attendanceHC_mi_Click(object sender, EventArgs e)
        {
            SwitchChildForm<Attendance>("Nhóm Ca Làm Của Nhân Viên");
        }
        private void customerDetailPacking_mi_Click(object sender, EventArgs e)
        {
            SwitchChildForm<CustomerDetailPackingTotal>("Customer Detail Packing");
        }

        private void packingTotal_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<DetailPackingTotal>("Packing Total");
        }

        private void lotCode_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<LOTCode>("Nhập Mã LOT");
        }

        private void invoice_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<INVOICE>("INVOICE");
        }

        private void phyto_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<Phyto>("PHYTO");
        }

        private void dkkd_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<DangKyKiemDinh>("Đăng Ký Kiểm Dịch");
        }

        private void orderTotal_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<Do417>("Dò 417");
        }

        private void orderpackingList_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<OrderPackingList>("Danh Sách Đóng Thùng");
        }

        private void exportCode_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<ExportCodes>("Danh Sách Mã Xuất Cảng");
        }

        private void Products_SKU_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<ProductSKU>("Danh Sách Sản Phẩm Chính");
        }

        private void productPacking_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<ProductList>("Danh Sách Sản Phẩm Quy Cách");
        }

        private void khachhang_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<Customers>("Danh Sách Khách Hàng");
        }


        private void others_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<OrdersList>("Danh Sách Đơn Hàng");
        }

        private void user_mi_Click(object sender, EventArgs e)
        {
            SwitchChildForm<User>("Danh Sách User");
        }

        private void employee_mi_Click(object sender, EventArgs e)
        {
            SwitchChildForm<Employee>("Danh Sách Nhân Viên");
        }

        private void department_mi_Click(object sender, EventArgs e)
        {
            SwitchChildForm<Department>("Danh Sách Phòng Ban");
        }

        private void position_mi_Click(object sender, EventArgs e)
        {
            SwitchChildForm<Position>("Danh Sách Vị Trí Nhân Viên");
        }
    }
}
