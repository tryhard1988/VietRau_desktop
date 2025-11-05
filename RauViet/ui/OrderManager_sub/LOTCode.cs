using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class LOTCode : Form, ICanSave
    {
        DataTable mExportCode_dt, mLOTCode_dt;
        private bool _dataChanged = false;
        private LoadingOverlay loadingOverlay;
        public LOTCode()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";


            LuuThayDoiBtn.Click += saveBtn_Click;           
         //   dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            dataGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGV_EditingControlShowing);
            dataGV.CellFormatting += dataGV_CellFormatting;
            dataGV.CellValueChanged += dataGV_CellValueChanged;
            dataGV.KeyDown += dataGV_KeyDown;
            //dataGV.CellEndEdit += dataGV_CellEndEdit;

            exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
        }

        

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);         

            try
            {
                var LOTCodeTask = SQLManager.Instance.GetLOTCodeByExportCode_inCompleteAsync();
                string[] keepColumns = { "ExportCodeID", "ExportCode" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                var exportCodeTask = SQLStore.Instance.getExportCodesAsync(keepColumns, parameters);

                await Task.WhenAll(LOTCodeTask, exportCodeTask);

                mExportCode_dt = exportCodeTask.Result;
                mLOTCode_dt = LOTCodeTask.Result;

                mLOTCode_dt.Columns["ProductNameVN"].SetOrdinal(0);
                mLOTCode_dt.Columns["LotCode"].SetOrdinal(1);
                mLOTCode_dt.Columns["LOTCodeComplete"].SetOrdinal(2);

                dataGV.DataSource = mLOTCode_dt;

                dataGV.Columns["SKU"].Visible = false;
                dataGV.Columns["ExportCode"].Visible = false;
                dataGV.Columns["ExportCodeID"].Visible = false;
                dataGV.Columns["LOTCodeHeader"].Visible = false;

                dataGV.ReadOnly = false;
                dataGV.Columns["LotCode"].ReadOnly = false;

                dataGV.Columns["ProductNameVN"].ReadOnly = true;
                dataGV.Columns["LOTCodeComplete"].ReadOnly = true;
                dataGV.Columns["Priority"].ReadOnly = true;

                dataGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                dataGV.Columns["LOTCodeComplete"].HeaderText = "LOT Code Hoàn Chỉnh";
                dataGV.Columns["LotCode"].HeaderText = "Phần Sau LOT Code";
                dataGV.Columns["Priority"].HeaderText = "Ưu Tiên";

                dataGV.Columns["Priority"].Width = 50;
                dataGV.Columns["LOTCodeComplete"].Width = 120;
                dataGV.Columns["ProductNameVN"].Width = 150; ;
                dataGV.Columns["LotCode"].Width = 50;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


                dataGV.Columns["LotCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                _dataChanged = false;

                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";
                
            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }

        private void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mLOTCode_dt == null || mExportCode_dt.Rows.Count == 0)
                return;

            SaveData();
            string selectedExportCode = ((DataRowView)exportCode_cbb.SelectedItem)["ExportCodeID"].ToString();

            if (!string.IsNullOrEmpty(selectedExportCode))
            {
                // Tạo DataView để filter
                DataView dv = new DataView(mLOTCode_dt);
                dv.RowFilter = $"ExportCodeID = '{selectedExportCode}'";

                // Gán lại cho DataGridView
                dataGV.DataSource = dv;
            }
            else
            {
                // Nếu chưa chọn gì thì hiển thị toàn bộ
                dataGV.DataSource = mLOTCode_dt;
            }
        }

        private void dataGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            _dataChanged = true;
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var columnName = dataGV.Columns[e.ColumnIndex].Name;

                if (columnName == "LOTCode")
                {
                    updateLotCodeComplete(dataGV.Rows[e.RowIndex]);
                }
            }
        }

        private void updateLotCodeComplete(DataGridViewRow row)
        {

            string lotCodeHeader = row.Cells["LOTCodeHeader"].Value.ToString();
            string LotCode = row.Cells["LotCode"].Value.ToString().Replace(" ","");

            string exportCode = exportCode_cbb.Text;
            string lastTwoChars = exportCode.Substring(exportCode.Length - 2);
            row.Cells["LOTCodeComplete"].Value = LotCode.CompareTo("") == 0 ? "" : lotCodeHeader + lastTwoChars + LotCode;            
        }



        private void dataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGV.Columns[e.ColumnIndex].Name == "LOTCode")
            {
                var row = dataGV.Rows[e.RowIndex];
                var lotCodeHeaderValue = row.Cells["LOTCodeHeader"].Value?.ToString();
                if (!string.IsNullOrEmpty(lotCodeHeaderValue))
                {
                    row.Cells["LOTCode"].ReadOnly = false;
                    e.CellStyle.BackColor = Color.LightGray;
                }
                else
                {
                    row.Cells["LOTCode"].ReadOnly = true;
                }
            }
        }
        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
            }
        }        

        private void dataGV_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tb = e.Control as TextBox;
            if (tb == null) return;

            // Gỡ toàn bộ trước để tránh add trùng
            tb.KeyPress -= Tb_KeyPress_OnlyNumber;

            var colName = dataGV.CurrentCell.OwningColumn.Name;

            if (colName == "NetWeightFinal")
            {
                tb.KeyPress += Tb_KeyPress_OnlyNumber;
            }
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

            // Chỉ cho nhập số, dấu chấm hoặc ký tự điều khiển
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Nếu đã có 1 dấu chấm, không cho nhập thêm
            if (e.KeyChar == '.' && tb.Text.Contains("."))
            {
                e.Handled = true;
            }
        }


        private void dataGV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // chặn xuống dòng mặc định

                int row = dataGV.CurrentCell.RowIndex;
                int col = dataGV.CurrentCell.ColumnIndex;

                // Nếu chưa phải dòng cuối thì nhảy xuống
                if (row < dataGV.Rows.Count - 1)
                {
                    dataGV.CurrentCell = dataGV.Rows[row + 1].Cells[col];
                    dataGV.BeginEdit(true); // mở chế độ nhập luôn
                }
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            SaveData(false);
        }

        public async void SaveData(bool ask = true)
        {
            if (!_dataChanged && ask) return;

            DialogResult dialogResult = MessageBox.Show(
                                           "Chắc chắn chưa?",
                                           "Thay đổi",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question // Icon dấu chấm hỏi
                                       );
            if (dialogResult == DialogResult.No)
                return;

            var list = new List<(int ExportCodeID, int SKU, string LOTCode, string LOTCodeComplete)>();

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                int exportCodeID = Convert.ToInt32(row.Cells["ExportCodeID"].Value);
                int sku = Convert.ToInt32(row.Cells["SKU"].Value);
                string lotCode = row.Cells["LOTCode"].Value?.ToString();
                string lotCodeComplete = row.Cells["LOTCodeComplete"].Value?.ToString();

                list.Add((exportCodeID, sku, lotCode, lotCodeComplete));
            }

            try
            {
                // Gọi async
                bool result = await SQLManager.Instance.UpsertOrdersLotCodesBySKUAsync(list);

                if (result)
                {
                    MessageBox.Show("Cập nhật thành công!");
                    _dataChanged = false;
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!");
                }
            }
            catch
            {
                MessageBox.Show("Cập nhật thất bại!");
            }
        }
    }
}
