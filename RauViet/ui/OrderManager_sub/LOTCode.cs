using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class LOTCode : Form
    {
        DataTable mExportCode_dt, mLOTCode_dt;
        DataView mLotCodeLog_dv;
        private LoadingOverlay loadingOverlay;
        int mCurrentExportID = -1;
        object oldValue;
        public LOTCode()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
          
         //   dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            dataGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGV_EditingControlShowing);
            dataGV.CellFormatting += dataGV_CellFormatting;
            dataGV.CellEndEdit += dataGV_CellValueChanged;
            dataGV.KeyDown += dataGV_KeyDown;
            dataGV.CellBeginEdit += dataGV_CellBeginEdit;
            dataGV.SelectionChanged += DataGV_SelectionChanged; ;
            this.KeyDown += LOTCode_KeyDown;
        }

        

        private void LOTCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (mCurrentExportID <= 0)
                {
                    return;
                }

                SQLStore_Kho.Instance.removeOrdersTotal(mCurrentExportID);
                ShowData();
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);         

            try
            {
                
                string[] keepColumns = { "ExportCodeID", "ExportCode", "InputByName_NoSign" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                mExportCode_dt = await SQLStore_Kho.Instance.getExportCodesAsync(keepColumns, parameters);

                if (mCurrentExportID <= 0 && mExportCode_dt.Rows.Count > 0)
                {
                    mCurrentExportID = Convert.ToInt32(mExportCode_dt.AsEnumerable()
                                   .Max(r => r.Field<int>("ExportCodeID")));
                }

                var LOTCodeTask = SQLStore_Kho.Instance.GetLOTCodeAsync(mCurrentExportID);
                var LOTCodeLogTask = SQLStore_Kho.Instance.GetLotCodeLogAsync(mCurrentExportID);

                await Task.WhenAll(LOTCodeTask, LOTCodeLogTask);
                mLOTCode_dt = LOTCodeTask.Result;

                await FillMissingLotCodeComplete(mLOTCode_dt);
                mLotCodeLog_dv = new DataView(LOTCodeLogTask.Result);
                logGV.DataSource = mLotCodeLog_dv;

                DataView dv = new DataView(mLOTCode_dt);
                dv.RowFilter = $"ExportCodeID = {mCurrentExportID}";
                dataGV.DataSource = dv;

                dataGV.Columns["SKU"].Visible = false;
                dataGV.Columns["ExportCode"].Visible = false;
                dataGV.Columns["ExportCodeID"].Visible = false;
                logGV.Columns["LogID"].Visible = false;
                logGV.Columns["ExportCodeID"].Visible = false;
                logGV.Columns["SKU"].Visible = false;

                dataGV.ReadOnly = false;
                dataGV.Columns["LotCode"].ReadOnly = false;

                dataGV.Columns["ProductNameVN"].ReadOnly = true;
                dataGV.Columns["LOTCodeComplete"].ReadOnly = false;
                dataGV.Columns["Priority"].ReadOnly = true;

                dataGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                dataGV.Columns["LOTCodeComplete"].HeaderText = "LOT Code Hoàn Chỉnh";
                dataGV.Columns["LotCode"].HeaderText = "Phần Sau LOT Code";
                dataGV.Columns["LOTCodeHeader"].HeaderText = "Phần Đầu LOT Code";
                dataGV.Columns["Priority"].HeaderText = "Ưu Tiên";

                dataGV.Columns["Priority"].Width = 50;
                dataGV.Columns["LOTCodeComplete"].Width = 120;
                dataGV.Columns["ProductNameVN"].Width = 150; ;
                dataGV.Columns["LotCode"].Width = 100;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


                dataGV.Columns["LOTCodeHeader"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["LotCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                exportCode_cbb.SelectedIndexChanged -= exportCode_search_cbb_SelectedIndexChanged;
                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";
                exportCode_cbb.SelectedValue = mCurrentExportID;
                exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;

                logGV.Columns["Description"].HeaderText = "Hành động";
                logGV.Columns["LotCode"].HeaderText = "Phần sau LOTCode";
                logGV.Columns["LotCodeHeader"].HeaderText = "Phần đầu LOTCode";
                logGV.Columns["LotCodeComplete"].HeaderText = "LOTCode hoàn chỉnh";
                logGV.Columns["ActionBy"].HeaderText = "Người thay đổi";
                logGV.Columns["CreatedAt"].HeaderText = "Ngày thay đổi";

                logGV.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                logGV.Columns["LotCode"].Width = 100;
                logGV.Columns["LotCodeHeader"].Width = 100;
                logGV.Columns["LotCodeComplete"].Width = 110;
                logGV.Columns["ActionBy"].Width = 150;
                logGV.Columns["CreatedAt"].Width = 110;
                logGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            }
            catch
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

        private async void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;
            mCurrentExportID = exportCodeId;

            var LOTCodeTask = SQLStore_Kho.Instance.GetLOTCodeAsync(mCurrentExportID);
            var LOTCodeLogTask = SQLStore_Kho.Instance.GetLotCodeLogAsync(mCurrentExportID);

            await Task.WhenAll(LOTCodeTask, LOTCodeLogTask);

            mLOTCode_dt = LOTCodeTask.Result;
            mLotCodeLog_dv = new DataView(LOTCodeLogTask.Result);
            logGV.DataSource = mLotCodeLog_dv;

            DataView dv = new DataView(mLOTCode_dt);
            dv.RowFilter = $"ExportCodeID = {exportCodeId}";
            dataGV.DataSource = dv;

            await FillMissingLotCodeComplete(mLOTCode_dt);

            DataRowView dataR = (DataRowView)exportCode_cbb.SelectedItem;
            string staff = dataR["InputByName_NoSign"].ToString();

            dataGV.ReadOnly = !(UserManager.Instance.fullName_NoSign.CompareTo(staff) == 0);
        }

        private async void dataGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                object newValue = dataGV.CurrentCell?.Value;
                if (object.Equals(oldValue, newValue)) return;

                var columnName = dataGV.Columns[e.ColumnIndex].Name;
                var row = dataGV.CurrentRow;
                if (columnName == "LOTCode")
                {
                    updateLotCodeComplete(row);
                }
                
                var list = new List<(int ExportCodeID, int SKU, string LOTCode, string LOTCodeComplete)>();
                int exportCodeID = Convert.ToInt32(row.Cells["ExportCodeID"].Value);
                int sku = Convert.ToInt32(row.Cells["SKU"].Value);
                string lotCodeHeader = row.Cells["LOTCodeHeader"].Value.ToString();
                string lotCode = row.Cells["LOTCode"].Value?.ToString();
                string lotCodeComplete = row.Cells["LOTCodeComplete"].Value?.ToString();

                list.Add((exportCodeID, sku, lotCode, lotCodeComplete));

                try
                {
                    bool result = await SQLManager_Kho.Instance.UpsertOrdersLotCodesBySKUAsync(list);

                    if (result)
                    {
                        _ = SQLManager_Kho.Instance.InsertLotCodeLogAsync(exportCodeID, sku, $"{oldValue}: {columnName} Update Thành Công", lotCode, lotCodeHeader, lotCodeComplete);
                        status_lb.Text = "Thành công.";
                        status_lb.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        _ = SQLManager_Kho.Instance.InsertLotCodeLogAsync(exportCodeID, sku, $"{oldValue}: {columnName} Update Thất Bại", lotCode, lotCodeHeader, lotCodeComplete);
                        MessageBox.Show("Cập nhật thất bại!");
                    }
                }
                catch (Exception ex)
                {
                    _ = SQLManager_Kho.Instance.InsertLotCodeLogAsync(exportCodeID, sku, $"{oldValue}: {columnName} Update Thất Bại do Exception:  {ex.Message}", lotCode, lotCodeHeader, lotCodeComplete);
                    MessageBox.Show("Cập nhật thất bại!");
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
            string columnName = dataGV.Columns[e.ColumnIndex].Name;
            if (columnName == "LOTCode" || columnName == "LOTCodeComplete")
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

        private void dataGV_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            oldValue = dataGV.CurrentCell?.Value;
            if (exportCode_cbb.SelectedItem != null)
            {
                DataRowView dataR = (DataRowView)exportCode_cbb.SelectedItem;
                string staff = dataR["InputByName_NoSign"].ToString();
                if (UserManager.Instance.fullName_NoSign.CompareTo(staff) != 0)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void DataGV_SelectionChanged(object sender, EventArgs e)
        {
            status_lb.Text = "";
            if (dataGV.CurrentRow == null) return;

            if (exportCode_cbb.SelectedValue == null) return;
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return;
            }

            var currentRow = dataGV.CurrentRow;
            int SKU = Convert.ToInt32(currentRow.Cells["SKU"].Value);

            mLotCodeLog_dv.RowFilter = $"SKU = {SKU}";
            mLotCodeLog_dv.Sort = "LogID DESC";
        }

        public async Task FillMissingLotCodeComplete(DataTable sourceTable)
        {
            if (sourceTable == null || sourceTable.Rows.Count == 0)
                return;

            var list = new List<(int ExportCodeID, int SKU, string LOTCode, string LOTCodeComplete)>();
            // Duyệt từng group theo SKU + ExportCodeID
            foreach (var grp in sourceTable.AsEnumerable()
                .GroupBy(r => new
                {
                    SKU = r.Field<int>("SKU"),
                    ExportCodeID = r.Field<int>("ExportCodeID")
                }))
            {
                // Lấy LotCodeComplete hợp lệ
                var validLot = grp.Where(r => !string.IsNullOrWhiteSpace(r.Field<string>("LotCodeComplete")))
                                .Select(r => new
                                {
                                    LotCode = r.Field<string>("LotCode"),
                                    LotCodeComplete = r.Field<string>("LotCodeComplete")
                                })
                                .FirstOrDefault();

                // Nếu group không có LotCodeComplete hợp lệ thì bỏ qua
                if (validLot == null)
                    continue;
                                
                // Fill các row bị rỗng
                foreach (var row in grp.Where(r =>
                    string.IsNullOrWhiteSpace(r.Field<string>("LotCodeComplete"))))
                {
                    row.SetField("LotCodeComplete", validLot.LotCodeComplete);
                    row.SetField("LotCode", validLot.LotCode);

                    int exportCodeID = Convert.ToInt32(row["ExportCodeID"]);

                    list.Add((grp.Key.ExportCodeID, grp.Key.SKU, validLot.LotCode, validLot.LotCodeComplete));
                }
            }

            if (list.Count > 0)
            {
                await SQLManager_Kho.Instance.UpsertOrdersLotCodesBySKUAsync(list);
            }

            var uniqueRows = sourceTable.AsEnumerable()
        .GroupBy(r => new
        {
            SKU = r.Field<int>("SKU"),
            ExportCodeID = r.Field<int>("ExportCodeID"),
            LotCodeComplete = r.Field<string>("LotCodeComplete") ?? ""
        })
        .Select(g => g.First()) // Lấy row đầu tiên của group
        .CopyToDataTable();

            // Gán bảng đã lọc trùng ngược lại cho sourceTable
            sourceTable.Clear();
            foreach (DataRow r in uniqueRows.Rows)
                sourceTable.ImportRow(r);
        }
    }
}
