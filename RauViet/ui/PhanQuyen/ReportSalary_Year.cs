using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class ReportSalary_Year : Form
    {
        public ReportSalary_Year()
        {
            InitializeComponent();
            year_tb.Text = DateTime.Now.Year.ToString();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;

            totalNetSalary_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            load_btn.Click += Load_btn_Click;
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;

            try
            {
                int year = Convert.ToInt32(year_tb.Text);
                var salarySummaryByYearTask = SQLStore_QLNS.Instance.GetSalarySummaryByYearAsync(year);

                await Task.WhenAll(salarySummaryByYearTask);
                DataTable salarySummaryByYear_dt = salarySummaryByYearTask.Result;

                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = salarySummaryByYear_dt;


                dataGV.Columns["Month"].Visible = false;
                dataGV.Columns["Year"].Visible = false;

                int count = 0;
                salarySummaryByYear_dt.Columns["MonthYear"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalEmployees"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalBaseSalary"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalNetSalary"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalNetInsuranceSalary"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalInsuranceAllowance"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalNonInsuranceAllowance"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalOvertimeSalary"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalLeaveSalary"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalDeductionAmount"].SetOrdinal(count++);

                dataGV.Columns["MonthYear"].HeaderText = "Tháng/Năm";
                dataGV.Columns["TotalBaseSalary"].HeaderText = "Tổng Lương Cơ Bản";
                dataGV.Columns["TotalNetSalary"].HeaderText = "Tổng Lương Thực Lãnh";
                dataGV.Columns["TotalNetInsuranceSalary"].HeaderText = "Tổng Lương C.S Đóng BH";
                dataGV.Columns["TotalInsuranceAllowance"].HeaderText = "Tổng PC Có Đóng BH";
                dataGV.Columns["TotalNonInsuranceAllowance"].HeaderText = "Tổng PC K.Đóng BH";
                dataGV.Columns["TotalOvertimeSalary"].HeaderText = "Tổng Chi Lương Tăng Ca";
                dataGV.Columns["TotalLeaveSalary"].HeaderText = "Tổng Lương Ngày Nghỉ";
                dataGV.Columns["TotalDeductionAmount"].HeaderText = "Tổng Khoảng Trừ";
                dataGV.Columns["TotalEmployees"].HeaderText = "Tổng NV Làm Việc";

                int sizeWidth = 90;
                dataGV.Columns["MonthYear"].Width = 70;
                dataGV.Columns["TotalBaseSalary"].Width = sizeWidth;
                dataGV.Columns["TotalNetSalary"].Width = sizeWidth;
                dataGV.Columns["TotalNetInsuranceSalary"].Width = sizeWidth;
                dataGV.Columns["TotalInsuranceAllowance"].Width = sizeWidth;
                dataGV.Columns["TotalNonInsuranceAllowance"].Width = sizeWidth;
                dataGV.Columns["TotalOvertimeSalary"].Width = sizeWidth;
                dataGV.Columns["TotalLeaveSalary"].Width = sizeWidth;
                dataGV.Columns["TotalDeductionAmount"].Width = sizeWidth;
                dataGV.Columns["TotalEmployees"].Width = sizeWidth;

                decimal totalNetSalary = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalNetSalary"));
                decimal totalNetInsuranceSalary = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalNetInsuranceSalary"));
                decimal totalInsuranceAllowance = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalInsuranceAllowance"));
                decimal totalNonInsuranceAllowance = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalNonInsuranceAllowance"));
                totalNetSalary_tb.Text = totalNetSalary.ToString();
                totalNetInsuranceSalary_tb.Text = totalNetInsuranceSalary.ToString();
                totalAllowance_tb.Text = (totalInsuranceAllowance + totalNonInsuranceAllowance).ToString();

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            catch
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                loading_lb.Visible = false; // ẩn loading
                loading_lb.Enabled = true; // enable lại button
            }


        }

        private async void Load_btn_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

            // Chỉ cho nhập số, phím điều khiển hoặc dấu chấm
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true; // chặn ký tự không hợp lệ
            }

            // Không cho nhập nhiều dấu chấm
            if (e.KeyChar == '.' && tb.Text.Contains("."))
            {
                e.Handled = true;
            }
        }
    }
}
