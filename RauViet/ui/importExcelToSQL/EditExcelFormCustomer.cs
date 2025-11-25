using System;
using System.Data;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class EditExcelFormCustomer : Form
    {
        public EditExcelFormCustomer()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            openFileExcel_mi.Click += openFileExcel_mi_Click;
            save_mi.Click += save_mi_Click;
            importToSQL_mi.Click += importToSQL_mi_Click;
        }

        private void importToSQL_mi_Click(object sender, EventArgs e)
        {
            ImportDataGridViewToSql(dataGV, SQLManager.Instance.ql_NhanSu_conStr(), "Customers");
        }

        private void save_mi_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Files|*.xlsx";
                sfd.Title = "Chọn nơi lưu file Excel";
                sfd.FileName = "Export.xlsx"; // tên mặc định

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveDataGridViewToExcel(dataGV, sfd.FileName);
                    MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void openFileExcel_mi_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file Excel";
                ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
                ofd.Multiselect = false; // chỉ cho chọn 1 file

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    // TODO: gọi hàm đọc excel -> DataGridView
                    LoadExcelClosedXML(filePath);
                }
            }
        }

        private void LoadExcelClosedXML(string filePath)
        {
            var dt = new DataTable();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheets.First(); // lấy sheet đầu tiên
                bool firstRow = true;

                foreach (var row in worksheet.RowsUsed())
                {
                    var cellValues = row.Cells().Select(c => c.GetValue<string>().Trim()).ToList();

                    if (firstRow)
                    {
                        // Tạo cột từ header
                        foreach (var colName in cellValues)
                        {
                            if (!string.IsNullOrWhiteSpace(colName))
                                dt.Columns.Add(colName);
                            else
                                dt.Columns.Add("Column" + dt.Columns.Count); // tránh null header
                        }
                        firstRow = false;
                    }
                    else
                    {
                        // Nếu số cell < số cột thì bổ sung cho đủ
                        while (cellValues.Count < dt.Columns.Count)
                            cellValues.Add("");

                        // Nếu số cell > số cột thì cắt bớt
                        if (cellValues.Count > dt.Columns.Count)
                            cellValues = cellValues.Take(dt.Columns.Count).ToList();

                        // Kiểm tra dòng có toàn ô trống không
                        bool allEmpty = cellValues.All(string.IsNullOrWhiteSpace);
                        if (!allEmpty)
                            dt.Rows.Add(cellValues.ToArray());
                    }
                }
            }

            dataGV.DataSource = dt;
        }

        private void SaveDataGridViewToExcel(DataGridView dgv, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                int colIndex = 1;

                // Ghi header
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    worksheet.Cell(1, colIndex).Value = col.HeaderText;
                    worksheet.Cell(1, colIndex).Style.Font.Bold = true;
                    worksheet.Cell(1, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    colIndex++;
                }

                // Ghi dữ liệu
                int rowIndex = 2;
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue; // bỏ dòng trống cuối cùng

                    colIndex = 1;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        worksheet.Cell(rowIndex, colIndex).Value = cell.Value?.ToString();
                        colIndex++;
                    }
                    rowIndex++;
                }

                // Auto fit cột
                worksheet.Columns().AdjustToContents();

                // Lưu file
                workbook.SaveAs(filePath);
            }
        }

        private DataTable GetDataTableFromDataGridView(DataGridView dgv)
        {
            var dt = new DataTable();

            // Tạo cột
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                dt.Columns.Add(col.HeaderText, typeof(string));
            }

            // Thêm dữ liệu
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    var values = new object[dgv.Columns.Count];
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        values[i] = row.Cells[i].Value?.ToString();
                    }
                    dt.Rows.Add(values);
                }
            }

            return dt;
        }

        private void ImportDataGridViewToSql(DataGridView dgv, string connectionString, string tableName)
        {
            DataTable dt = GetDataTableFromDataGridView(dgv);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = tableName;

                    // Ánh xạ cột DataGridView -> SQL
                    foreach (DataColumn col in dt.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.WriteToServer(dt);
                }
            }
        }
    }
}
