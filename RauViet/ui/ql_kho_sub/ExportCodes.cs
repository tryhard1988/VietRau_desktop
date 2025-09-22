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
using DocumentFormat.OpenXml.Wordprocessing;
using RauViet.classes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Color = System.Drawing.Color;

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

            autoCreateExportId_btn.Enabled = false;

            autoCreateExportId_btn.Click += autoCreateExportId_btn_Click;
            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            complete_cb.CheckedChanged += completeCB_CheckedChanged;
        }

        private void autoCreateExportId_btn_Click(object sender, EventArgs e)
        {
            int maxExportIndex = 0;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue; // bỏ dòng mới chưa nhập dữ liệu

                if (row.Cells["exportCodeIndex"].Value != null &&
                    int.TryParse(row.Cells["exportCodeIndex"].Value.ToString(), out int value))
                {
                    if (value > maxExportIndex)
                        maxExportIndex = value;
                }
            }

            exportCode_tb.Text = (maxExportIndex + 1).ToString();
        }

        public async void ShowData()
        {
            

            this.FormBorderStyle = FormBorderStyle.None;
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

                dataGV.Columns["ExportCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["ExportDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["Complete"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["Packing"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                dataGV.Columns["ModifiedAt"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

                dataGV.Columns["ExportCodeID"].Visible = false;
                dataGV.Columns["ExportCodeIndex"].Visible = false;
                dataGV.AutoResizeColumns();

                if (dataGV.SelectedRows.Count > 0)
                {
                    updateRightUI(0);
                }
                
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

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            updateRightUI(rowIndex);
        }

        private void updateRightUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            int exportCodeID = Convert.ToInt32(cells["ExportCodeID"].Value);
            int exportCodeIndex = Convert.ToInt32(cells["ExportCodeIndex"].Value);
            DateTime exportDate = Convert.ToDateTime(cells["ExportDate"].Value.ToString());
            bool complete = Convert.ToBoolean(cells["Complete"].Value);

            exportCodeId_tb.Text = exportCodeID.ToString();
            exportCode_tb.Text = exportCodeIndex.ToString();
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
                complete_cb.BackColor = Color.DarkGray; // background
            }

            delete_btn.Enabled = true;
            complete_cb.Visible = true;
            autoCreateExportId_btn.Enabled = false;

            info_gb.BackColor = Color.DarkGray;
            completeCB_CheckedChanged(null, null);
        }

       

        private async void updateExportCode(int exportCodeID, string exportCode, int exportCodeIndex, DateTime exportDate, bool complete)
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
                            bool isScussess = await SQLManager.Instance.updateExportCodeAsync(exportCodeID, exportCode, exportCodeIndex, exportDate, complete);

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

        private async void createExportCode(string exportCode, int exportCodeIndex, DateTime exportDate)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Tạo Mới Thông Tin Khách Hàng", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    bool isScussess = await SQLManager.Instance.insertExportCodeAsync(exportCode, exportCodeIndex, exportDate);
                    if (isScussess == true)
                    {

                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        int newCustomerID = createNewID();
                        drToAdd["ExportCodeID"] = newCustomerID;
                        drToAdd["ExportCode"] = exportCode;
                        drToAdd["ExportCodeIndex"] = exportCodeIndex;
                        drToAdd["ExportDate"] = exportDate;
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
            if(exportCode_tb.Text.CompareTo("") == 0)
            {
                MessageBox.Show(
                                "Dữ Liệu Không hợp Lệ, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            if (complete_cb.Checked == true) return;

            DateTime exportDate = Convert.ToDateTime(exportdate_dtp.Value.ToString("dd/MM/yyyy"));
            int exportCodeIndex = Convert.ToInt32(exportCode_tb.Text);
            string exportCode = "MXC" + exportCodeIndex + "_"+ exportDate.Day + exportDate.Month + exportDate.Year;
            if (exportCodeId_tb.Text.Length != 0)
                updateExportCode(Convert.ToInt32(exportCodeId_tb.Text), exportCode, exportCodeIndex, exportDate, complete_cb.Checked);
            else
                createExportCode(exportCode, exportCodeIndex, exportDate);

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
            complete_cb.Visible = false;
            info_gb.BackColor = Color.Green;
            autoCreateExportId_btn.Enabled = true;
        }

        private void completeCB_CheckedChanged(object sender, EventArgs e)
        {
            exportCode_tb.Enabled = !complete_cb.Checked;
            exportdate_dtp.Enabled = !complete_cb.Checked;
        }
    }
}
