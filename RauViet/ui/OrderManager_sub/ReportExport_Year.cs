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
    public partial class ReportExport_Year : Form
    {
        public ReportExport_Year()
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

            totalMoney_tb.KeyPress += Tb_KeyPress_OnlyNumber;
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
                var salarySummaryByYearTask = SQLStore.Instance.GetExportHistoryByYear(year);

                await Task.WhenAll(salarySummaryByYearTask);
                DataTable salarySummaryByYear_dt = salarySummaryByYearTask.Result;

                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = salarySummaryByYear_dt;


                dataGV.Columns["ExportHistoryID"].Visible = false;

                int count = 0;
                salarySummaryByYear_dt.Columns["ExportCode"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["ExportDate"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalMoney"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["TotalNW"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["NumberCarton"].SetOrdinal(count++);
                salarySummaryByYear_dt.Columns["FreightCharge"].SetOrdinal(count++);

                dataGV.Columns["ExportCode"].HeaderText = "Mã Xuất Cảng";
                dataGV.Columns["ExportDate"].HeaderText = "Ngày Xuất Cảng";
                dataGV.Columns["TotalMoney"].HeaderText = "Tổng Tiền Hàng";
                dataGV.Columns["TotalNW"].HeaderText = "Tổng Trọng Lượng";
                dataGV.Columns["NumberCarton"].HeaderText = "Tổng Số Thùng";
                dataGV.Columns["FreightCharge"].HeaderText = "Phí Vận Chuyển";

                int sizeWidth = 120;
                dataGV.Columns["ExportCode"].Width = 100;
                dataGV.Columns["ExportDate"].Width = sizeWidth;
                dataGV.Columns["TotalMoney"].Width = sizeWidth;
                dataGV.Columns["TotalNW"].Width = sizeWidth;
                dataGV.Columns["NumberCarton"].Width = sizeWidth;
                dataGV.Columns["FreightCharge"].Width = sizeWidth;

                decimal totalmoney = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalMoney"));
                decimal totalNW = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("TotalNW"));
                int totalCarton = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<int>("NumberCarton"));
                decimal totalFreightCharge = salarySummaryByYear_dt.AsEnumerable().Sum(r => r.Field<decimal>("FreightCharge"));
                totalMoney_tb.Text = totalmoney.ToString();
                totalNW_tb.Text = totalNW.ToString();
                totalCarton_tb.Text = (totalCarton).ToString();
                freightCharge_tb.Text = (totalFreightCharge).ToString();

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            catch (Exception ex)
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
