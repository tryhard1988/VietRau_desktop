using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{
    public partial class ExportCodes : Form
    {        public ExportCodes()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;
            delete_btn.Enabled = false;

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGV_CellClick);
            this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
        }

        public async void ShowData()
        {
            

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                // Chạy truy vấn trên thread riêng
                DataTable dt = await SQLManager.Instance.getExportCodesAsync();
                dataGV.DataSource = dt;

                dataGV.Columns["ExportCode"].HeaderText = "Mã Xuất Cảng";
                dataGV.Columns["ExportDate"].HeaderText = "Ngày Xuất Cảng";                
                dataGV.Columns["Complete"].HeaderText = "Hoàn Thành";
                dataGV.Columns["ModifiedAt"].HeaderText = "Ngày Thay đổi";
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

        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
            }
        }
        
        private int createNewID()
        {
            var existingIds = dataGV.Rows
            .Cast<DataGridViewRow>()
            .Where(r => !r.IsNewRow && r.Cells["ExportCodeID"].Value != null)
            .Select(r => Convert.ToInt32(r.Cells["ExportCodeID"].Value))
            .ToList();

            int newCustomerID = existingIds.Count > 0 ? existingIds.Max() + 1 : 1;

            return newCustomerID;
        }

        private void dataGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;            
            var cells = dataGV.Rows[e.RowIndex].Cells;
            int exportCodeID  = Convert.ToInt32(cells["ExportCodeID"].Value);
            string exportCode = cells["ExportCode"].Value.ToString();
            DateTime exportDate = Convert.ToDateTime(cells["ExportDate"].Value.ToString());
            bool complete = Convert.ToBoolean(cells["Complete"].Value);

            exportCodeId_tb.Text = exportCodeID.ToString();
            exportCode_tb.Text = exportCode;
            exportdate_dtp.Value = exportDate;
            complete_cb.Checked = complete;
            complete_cb.AutoCheck = !complete;
            if (complete)
            {
                complete_cb.ForeColor = Color.Red; // chỉ tác dụng khi Enabled = true trên nhiều hệ thống
                complete_cb.BackColor = Color.Yellow; // background
            }
            else
            {
                complete_cb.ForeColor = Color.Black; // chỉ tác dụng khi Enabled = true trên nhiều hệ thống
                complete_cb.BackColor = this.BackColor; // background
            }

            delete_btn.Enabled = true;

        }

       

        private async void updateExportCode(int exportCodeID, string exportCode, DateTime exportDate, bool complete)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["ExportCodeID"].Value);
                if (id.CompareTo(exportCodeID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Thay Đổi Thông Tin Khách Hàng", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateExportCodeAsync(exportCodeID, exportCode, exportDate, complete);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["ExportCode"].Value = exportCode;
                                row.Cells["ExportDate"].Value = exportDate;
                                row.Cells["ModifiedAt"].Value = DateTime.Now;
                                row.Cells["Complete"].Value = complete;
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }




                    }
                    break;
                }
            }
        }

        private async void createExportCode(string exportCode)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Tạo Mới Thông Tin Khách Hàng", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    bool isScussess = await SQLManager.Instance.insertExportCodeAsync(exportCode);
                    if (isScussess == true)
                    {

                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        int newCustomerID = createNewID();
                        drToAdd["ExportCodeID"] = newCustomerID;
                        drToAdd["ExportCode"] = exportCode;
                        drToAdd["ExportDate"] = DateTime.Now;
                        drToAdd["ModifiedAt"] = DateTime.Now;
                        drToAdd["Complete"] = false;
                        exportCodeId_tb.Text = newCustomerID.ToString();


                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }

            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (exportCodeId_tb.Text.Length != 0)
                updateExportCode(Convert.ToInt32(exportCodeId_tb.Text), exportCode_tb.Text, exportdate_dtp.Value, complete_cb.Checked);
            else
                createExportCode(exportCode_tb.Text);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            string id = exportCodeId_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string exportCodeID = row.Cells["ExportCodeID"].Value.ToString();
                if (exportCodeID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN KHÁCH HÀNG ĐÓ NHA \n Chắc chắn chưa ?", " Xóa Thông Tin Khách Hàng", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deleteExportCodeAsync(Convert.ToInt32(exportCodeID));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                dataGV.Rows.Remove(row);
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }




                    }
                    break;
                }
            }




            delete_btn.Enabled = false;
        }

        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            exportCodeId_tb.Text = "";
            exportCode_tb.Text = "";
            exportdate_dtp.Value = DateTime.Now;
            complete_cb.Checked = false;
        }
    }
}
