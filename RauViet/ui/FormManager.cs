using MySql.Data.MySqlClient;
using RauViet.classes;
using RauViet.ui.PhanQuyen;
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
using static RauViet.ui.FormManager;

namespace RauViet.ui
{
    public partial class FormManager : Form
    {
        private Timer checkLoginTimer = new Timer { Interval = 20000 };
        public FormManager()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
            this.Text = "Quản Lý Việt Rau - NV: " + UserManager.Instance.fullName + " [" + UserManager.Instance.employeeCode + "]";            
            this.Load += FormManager_LoadAsync;
            
        }

        private async void FormManager_LoadAsync(object sender, EventArgs e)
        {
            title_lb.Text = "Đã Sẵn Sàng Rồi";
            SetupMenus();

            checkLoginTimer.Tick += checkLoginTimer_Tick;
            checkLoginTimer.Start();
        }

        private void SetupMenus()
        {
            if (UserManager.Instance.hasRole_User())
                user_mi.Click += user_mi_Click;
            else
                user_mi.Visible = false;

            if (UserManager.Instance.hasRole_NhanSu())
            {
                employee_mi.Click += employee_mi_Click;
                employeeBH_mi.Click += EmployeeBH_mi_Click;
                employeeBank_mi.Click += EmployeeBank_mi_Click;
                employeeWork_mi.Click += EmployeeWork_mi_Click;
                department_mi.Click += department_mi_Click;
                position_mi.Click += position_mi_Click;
                overtimeType_mi.Click += OvertimeType_mi_Click;
                holiday_mi.Click += Holiday_mi_Click;
                allowanceType_mi.Click += AllowanceType_mi_Click;
                salaryGrade_mi.Click += SalaryGrade_mi_Click;
                employeeSalaryInfo_mi.Click += EmployeeSalaryInfo_mi_Click;
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
                doCBM_mi.Click += DoCBM_mi_Click;
            }
            else if (UserManager.Instance.hasRole_HoanThanhDonHang())
            {
                exportCode_menuItem.Click += exportCode_btn_Click;
                order_menuitem.Visible = false;
                dsDongThung_menuitem.Visible = false;
                do417_menuitem.Visible = false;
                lotCode_menuitem.Visible = false;
                doCBM_mi.Visible = false;
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
                chotphyto_mi.Click += ChotPhyto_mi_Click;
            }
            else
                xuatExcelGuiKH_Group_mi.Visible = false;

            if (UserManager.Instance.hasRole_ChamCong())
            {
                attendanceHC_mi.Click += attendanceHC_mi_Click;
                overtimeAttendace_mi.Click += OvertimeAttendace_mi_Click;
                annualLeaveBalance_mi.Click += AnnualLeaveBalance_mi_Click;
                leaveAttendance_mi.Click += LeaveAttendance_mi_Click;
                //departmentAllowance_mi.Click += DepartmentAllowance_mi_Click;
                //positionAllowance_mi.Click += PositionAllowance_mi_Click;
                employeeAllowance_mi.Click += EmployeeAllowance_mi_Click;
                monthlyAllowance_mi.Click += MonthlyAllowance_mi_Click;
                deduction_VEG_mi.Click += deduction_VEG_mi_Click;
                deduction_OTH_mi.Click += Deduction_OTH_mi_Click;
                deduction_CEP_mi.Click += Deduction_CEP_mi_Click;
                deduction_ADV_mi.Click += Deduction_ADV_mi_Click;
                deduction_ATT_mi.Click += Deduction_ATT_mi_Click;
                salaryCaculator_mi.Click += SalaryCaculator_mi_Click;
            }
            else
                chamcong_pmi.Visible = false;

            if (UserManager.Instance.hasRole_ThongKe())
            {
                reportSalary_Month_mi.Click += ReportSalary_Month_mi_Click;
                reportExportYear_mi.Click += ReportExportYear_mi_Click;
            }
            else
            {
                thongke_main_mi.Visible = false;
            }

            string saved = Properties.Settings.Default.current_form;
            if (Enum.TryParse(saved, out EForm status))
            {
                openCurrentForm(status);
            }
        }

        

        private async void SwitchChildForm<T>(string title) where T : Form, new()
        {
            // Lưu form hiện tại nếu có
            if (this.content_panel.Controls.Count > 0)
            {
                if (this.content_panel.Controls[0] is ICanSave currentForm)
                    currentForm.SaveData();

                await Task.Delay(300);
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
        public enum EForm
        {
            ProductSKU,
            ProductList,
            ReportSalary_Year,
            ReportExport_Year,
            SalaryCaculator,
            EmployeeDeduction_ATT,
            EmployeeDeduction_ADV,
            EmployeeDeduction_CEP,
            EmployeeDeduction_OTH,
            EmployeeDeduction_VEG,
            Employee_POS_DEP_CON,
            EmployeeNganHang,
            EmployeeBaoHiem,
            EmployeeSalaryInfo,
            SalaryGrade,
            MonthlyAllowance,
            EmployeeAllowance,
            PositionAllowance,
            DepartmentAllowance,
            AllowanceType,
            LeaveAttendance,
            AnnualLeaveBalance,
            OvertimeAttendace,
            OvertimeType,
            Holidays,
            Attendance,
            CustomerDetailPackingTotal,
            DetailPackingTotal,
            LOTCode,
            INVOICE,
            Phyto,
            DangKyKiemDinh,
            Do417,
            OrderPackingList,
            ExportCodes,
            Customers,
            OrdersList,
            User,
            Employee,
            Department,
            Position,
            DoCBM,
            ChotPHYTO
        }

        private void openCurrentForm(EForm status)
        {            
            switch (status)
            {
                case EForm.ProductSKU:
                    SwitchChildForm<ProductSKU>("Danh Sách Sản Phẩm Chính");
                    break;

                case EForm.ProductList:
                    SwitchChildForm<ProductList>("Danh Sách Sản Phẩm Quy Cách");
                    break;
                case EForm.ReportSalary_Year:
                    SwitchChildForm<ReportSalary_Year>("Báo Cáo Chi Lương Theo Năm");
                    break;
                case EForm.ReportExport_Year:
                    SwitchChildForm<ReportExport_Year>("Báo Cáo Đơn Hàng Theo Năm");
                    break;
                case EForm.SalaryCaculator:
                    SwitchChildForm<SalaryCaculator>("Bảng Tính Lương Nhân Viên");
                    break;
                case EForm.EmployeeDeduction_ATT:
                    SwitchChildForm<EmployeeDeduction_ATT>("Trừ Chuyên Cần");
                    break;
                case EForm.EmployeeDeduction_ADV:
                    SwitchChildForm<EmployeeDeduction_ADV>("Ứng Lương Cho Nhân Viên");
                    break;
                case EForm.EmployeeDeduction_CEP:
                    SwitchChildForm<EmployeeDeduction_CEP>("Thu Hộ CEP");
                    break;
                case EForm.EmployeeDeduction_OTH:
                    SwitchChildForm<EmployeeDeduction_OTH>("Các Khoản Trừ Khác Của Nhân Viên");
                    break;
                case EForm.EmployeeDeduction_VEG:
                    SwitchChildForm<EmployeeDeduction_VEG>("Tiền Rau Của Nhân Viên");
                    break;
                case EForm.Employee_POS_DEP_CON:
                    SwitchChildForm<Employee_POS_DEP_CON>("Thông Tin Công Việc Nhân Viên");
                    break;
                case EForm.EmployeeNganHang:
                    SwitchChildForm<EmployeeNganHang>("Thông Tin Tài Khoản Ngân Hàng Của Nhân Viên");
                    break;
                case EForm.EmployeeBaoHiem:
                    SwitchChildForm<EmployeeBaoHiem>("Thông Tin Bảo Hiểm");
                    break;
                case EForm.EmployeeSalaryInfo:
                    SwitchChildForm<EmployeeSalaryInfo>("Chi Tiết Thay Đổi Lương");
                    break;
                case EForm.SalaryGrade:
                    SwitchChildForm<SalaryGrade>("Bảng Bậc Lương");
                    break;
                case EForm.MonthlyAllowance:
                    SwitchChildForm<MonthlyAllowance>("Phụ Cấp Phát Sinh Trong Tháng");
                    break;
                case EForm.EmployeeAllowance:
                    SwitchChildForm<EmployeeAllowance>("Phụ Cấp Theo Từng Nhân Viên");
                    break;
                case EForm.PositionAllowance:
                    SwitchChildForm<PositionAllowance>("Phụ Cấp Theo Chức Vụ");
                    break;
                case EForm.DepartmentAllowance:
                    SwitchChildForm<DepartmentAllowance>("Phụ Cấp Theo Phòng Ban");
                    break;
                case EForm.AllowanceType:
                    SwitchChildForm<AllowanceType>("Loại Phụ Cấp");
                    break;
                case EForm.LeaveAttendance:
                    SwitchChildForm<LeaveAttendance>("Bảng Đơn Nghỉ Phép");
                    break;
                case EForm.AnnualLeaveBalance:
                    SwitchChildForm<AnnualLeaveBalance>("Bảng Tồn Phép Nghỉ");
                    break;
                case EForm.OvertimeAttendace:
                    SwitchChildForm<OvertimeAttendace>("Bảng Chấm Công Tăng Ca");
                    break;
                case EForm.OvertimeType:
                    SwitchChildForm<OvertimeType>("Bảng Loại Tăng Ca");
                    break;
                case EForm.Holidays:
                    SwitchChildForm<Holidays>("Lập Ngày Nghỉ Lễ");
                    break;
                case EForm.Attendance:
                    SwitchChildForm<Attendance>("Chấm Công Hành Chính");
                    break;
                case EForm.CustomerDetailPackingTotal:
                    SwitchChildForm<CustomerDetailPackingTotal>("Customer Detail Packing");
                    break;
                case EForm.DetailPackingTotal:
                    SwitchChildForm<DetailPackingTotal>("Packing Total");
                    break;
                case EForm.LOTCode:
                    SwitchChildForm<LOTCode>("Nhập Mã LOT");
                    break;
                case EForm.INVOICE:
                    SwitchChildForm<INVOICE>("INVOICE");
                    break;
                case EForm.Phyto:
                    SwitchChildForm<Phyto>("PHYTO");
                    break;
                case EForm.DangKyKiemDinh:
                    SwitchChildForm<DangKyKiemDinh>("Đăng Ký Kiểm Dịch");
                    break;
                case EForm.Do417:
                    SwitchChildForm<Do417>("Dò 417");
                    break;
                case EForm.OrderPackingList:
                    SwitchChildForm<OrderPackingList>("Danh Sách Đóng Thùng");
                    break;
                case EForm.ExportCodes:
                    SwitchChildForm<ExportCodes>("Danh Sách Mã Xuất Cảng");
                    break;
                case EForm.Customers:
                    SwitchChildForm<Customers>("Danh Sách Khách Hàng");
                    break;
                case EForm.OrdersList:
                    SwitchChildForm<OrdersList>("Danh Sách Đơn Hàng");
                    break;
                case EForm.User:
                    SwitchChildForm<User>("Danh Sách User");
                    break;
                case EForm.Employee:
                    SwitchChildForm<Employee>("Danh Sách Nhân Viên");
                    break;
                case EForm.Department:
                    SwitchChildForm<Department>("Danh Sách Phòng Ban");
                    break;
                case EForm.Position:
                    SwitchChildForm<Position>("Danh Sách Vị Trí Nhân Viên");
                    break;
                case EForm.DoCBM:
                    SwitchChildForm<Do_CBM>("Do CBM");
                    break;
                case EForm.ChotPHYTO:
                    SwitchChildForm<ChotPhyto>("Chốt PHYTO");
                    break;

            }
            
            Properties.Settings.Default.current_form = status.ToString();
            Properties.Settings.Default.Save();
        }
        
        private void ReportSalary_Month_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.ReportSalary_Year); }
        private void ReportExportYear_mi_Click(object sender, EventArgs e){ openCurrentForm(EForm.ReportExport_Year); }
        private void SalaryCaculator_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.SalaryCaculator); }
        private void Deduction_ATT_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.EmployeeDeduction_ATT); }
        private void Deduction_ADV_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.EmployeeDeduction_ADV); }
        private void Deduction_CEP_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.EmployeeDeduction_CEP); }
        private void Deduction_OTH_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.EmployeeDeduction_OTH); }
        private void deduction_VEG_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.EmployeeDeduction_VEG); }
        private void EmployeeWork_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.Employee_POS_DEP_CON); }
        private void EmployeeBank_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.EmployeeNganHang); }
        private void EmployeeBH_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.EmployeeBaoHiem); }
        private void EmployeeSalaryInfo_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.EmployeeSalaryInfo); }
        private void SalaryGrade_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.SalaryGrade); }
        private void MonthlyAllowance_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.MonthlyAllowance); }
        private void EmployeeAllowance_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.EmployeeAllowance); }
        private void PositionAllowance_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.PositionAllowance); }
        private void DepartmentAllowance_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.DepartmentAllowance); }
        private void AllowanceType_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.AllowanceType); }
        private void LeaveAttendance_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.LeaveAttendance); }
        private void AnnualLeaveBalance_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.AnnualLeaveBalance); }
        private void OvertimeAttendace_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.OvertimeAttendace); }
        private void OvertimeType_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.OvertimeType); }
        private void Holiday_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.Holidays); }
        private void attendanceHC_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.Attendance); }
        private void customerDetailPacking_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.CustomerDetailPackingTotal); }
        private void packingTotal_btn_Click(object sender, EventArgs e) { openCurrentForm(EForm.DetailPackingTotal); }
        private void lotCode_btn_Click(object sender, EventArgs e) { openCurrentForm(EForm.LOTCode); }
        private void invoice_btn_Click(object sender, EventArgs e) { openCurrentForm(EForm.INVOICE); }
        private void phyto_btn_Click(object sender, EventArgs e) { openCurrentForm(EForm.Phyto); }
        private void dkkd_btn_Click(object sender, EventArgs e) { openCurrentForm(EForm.DangKyKiemDinh); }
        private void orderTotal_btn_Click(object sender, EventArgs e) { openCurrentForm(EForm.Do417); }
        private void orderpackingList_btn_Click(object sender, EventArgs e) { openCurrentForm(EForm.OrderPackingList); }
        private void exportCode_btn_Click(object sender, EventArgs e) { openCurrentForm(EForm.ExportCodes); }
        private void Products_SKU_btn_Click(object sender, EventArgs e)  { openCurrentForm(EForm.ProductSKU);  }
        private void productPacking_btn_Click(object sender, EventArgs e) { openCurrentForm(EForm.ProductList); }
        private void khachhang_btn_Click(object sender, EventArgs e) { openCurrentForm(EForm.Customers); }
        private void others_btn_Click(object sender, EventArgs e) { openCurrentForm(EForm.OrdersList); }
        private void user_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.User); }
        private void employee_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.Employee); }
        private void department_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.Department); }
        private void position_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.Position); }
        private void DoCBM_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.DoCBM); }
        private void ChotPhyto_mi_Click(object sender, EventArgs e) { openCurrentForm(EForm.ChotPHYTO); }
        private async void checkLoginTimer_Tick(object sender, EventArgs e)
        {
            var isHave = await SQLManager.Instance.HaveOtherComputerLoginAsync();
            if (isHave)
            {
                this.Hide();
                using (var loginForm = new LoginForm())
                {
                    checkLoginTimer.Stop(); // dừng timer
                    MessageBox.Show("Tài khoản của bạn đã đăng nhập ở máy khác.\nChương trình sẽ tự thoát.",
                                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    loginForm.ShowDialog();
                }
                this.Close();
            }

        }

    }
}
