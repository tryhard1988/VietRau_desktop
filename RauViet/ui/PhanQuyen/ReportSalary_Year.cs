using RauViet.classes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"MonthYear", "Tháng/Năm" },
                    {"TotalBaseSalary", "Tổng Lương Cơ Bản" },
                    {"TotalNetSalary", "Tổng Lương Thực Lãnh" },
                    {"TotalNetInsuranceSalary", "Tổng Lương C.S Đóng BH" },
                    {"TotalInsuranceAllowance", "Tổng PC Có Đóng BH" },
                    {"TotalNonInsuranceAllowance", "Tổng PC K.Đóng BH" },
                    {"TotalOvertimeSalary", "Tổng Chi Lương Tăng Ca" },
                    {"TotalLeaveSalary", "Tổng Lương Ngày Nghỉ" },
                    {"TotalDeductionAmount", "Tổng Khoảng Trừ" },
                    {"TotalEmployees", "Tổng NV Làm Việc" }
                });

                Utils.SetGridFormat_NO(dataGV, "TotalBaseSalary");
                Utils.SetGridFormat_NO(dataGV, "TotalNetSalary");
                Utils.SetGridFormat_NO(dataGV, "TotalNetInsuranceSalary");
                Utils.SetGridFormat_NO(dataGV, "TotalInsuranceAllowance");
                Utils.SetGridFormat_NO(dataGV, "TotalNonInsuranceAllowance");
                Utils.SetGridFormat_NO(dataGV, "TotalOvertimeSalary");
                Utils.SetGridFormat_NO(dataGV, "TotalLeaveSalary");
                Utils.SetGridFormat_NO(dataGV, "TotalDeductionAmount");
                Utils.SetGridFormat_NO(dataGV, "TotalEmployees");

                dataGV.Columns["Month"].Visible = false;
                dataGV.Columns["Year"].Visible = false;
                Utils.SetGridOrdinal(salarySummaryByYear_dt, new[] { "MonthYear", "TotalEmployees", "TotalBaseSalary", "TotalNetSalary", "TotalNetInsuranceSalary", "TotalInsuranceAllowance", "TotalNonInsuranceAllowance", "TotalOvertimeSalary", "TotalLeaveSalary", "TotalDeductionAmount"});

                int sizeWidth = 90;
                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"MonthYear", 70},
                    {"TotalBaseSalary", sizeWidth},
                    {"TotalNetSalary", sizeWidth},
                    {"TotalNetInsuranceSalary", sizeWidth},
                    {"TotalInsuranceAllowance", sizeWidth},
                    {"TotalNonInsuranceAllowance", sizeWidth},
                    {"TotalOvertimeSalary", sizeWidth},
                    {"TotalLeaveSalary", sizeWidth},
                    {"TotalDeductionAmount", sizeWidth},
                    {"TotalEmployees", sizeWidth}
                });

                decimal totalNetSalary = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalNetSalary"));
                decimal totalNetInsuranceSalary = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalNetInsuranceSalary"));
                decimal totalInsuranceAllowance = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalInsuranceAllowance"));
                decimal totalNonInsuranceAllowance = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalNonInsuranceAllowance"));
                totalNetSalary_tb.Text = totalNetSalary.ToString("N0");
                totalNetInsuranceSalary_tb.Text = totalNetInsuranceSalary.ToString("N0");
                totalAllowance_tb.Text = (totalInsuranceAllowance + totalNonInsuranceAllowance).ToString("N0");

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
