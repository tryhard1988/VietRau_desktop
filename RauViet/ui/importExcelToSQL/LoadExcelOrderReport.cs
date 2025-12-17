using ClosedXML.Excel;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class LoadExcelOrderReport : Form
    {
        DataTable mData;
        public LoadExcelOrderReport()
        {
            InitializeComponent();
            dataGV.AllowUserToAddRows = false;

            mData = new DataTable();

            this.WindowState = FormWindowState.Maximized;
            openFileExcel_mi.Click += openFileExcel_mi_Click;
            save_mi.Click += save_mi_Click;
            importToSQL_mi.Click += importToSQL_mi_Click;
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
            mData.Clear();

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
                                mData.Columns.Add(colName);
                            else
                                mData.Columns.Add("Column" + mData.Columns.Count); // tránh null header
                        }
                        firstRow = false;
                    }
                    else
                    {
                        // Nếu số cell < số cột thì bổ sung cho đủ
                        while (cellValues.Count < mData.Columns.Count)
                            cellValues.Add("");

                        // Nếu số cell > số cột thì cắt bớt
                        if (cellValues.Count > mData.Columns.Count)
                            cellValues = cellValues.Take(mData.Columns.Count).ToList();

                        // Kiểm tra dòng có toàn ô trống không
                        bool allEmpty = cellValues.All(string.IsNullOrWhiteSpace);
                        if (!allEmpty)
                            mData.Rows.Add(cellValues.ToArray());
                    }
                }
            }

            dataGV.DataSource = mData;
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


        private void importToSQL_mi_Click(object sender, EventArgs e)
        {
            ImportDataGridViewToSql();
        }

        private void ImportDataGridViewToSql()
        {
            List<(string customerName, string exportCode, DateTime exportDate, string product_VN, string product_EN, string package, int PCS, decimal netWeight, decimal amount, int priority)> orders =
                new List<(string customerName, string exportCode, DateTime exportDate, string product_VN, string product_EN, string package, int PCS, decimal netWeight, decimal amount, int priority)>();

            foreach (DataRow item in mData.Rows)
            {

                orders.Add((
                    customerName: "NULL",
                    exportCode: item["ExportCode"].ToString(),
                    exportDate: Convert.ToDateTime(item["ExportDate"]),
                    product_VN: item["ProductName_VN"].ToString(),
                    product_EN: item["ProductName_EN"].ToString(),
                    package: item["Package"].ToString(),
                    PCS: Convert.ToInt32(item["PCS"]),
                    netWeight: Convert.ToDecimal(item["NetWeight"]),
                    amount: Convert.ToDecimal(item["AmountCHF"]),
                    priority: Convert.ToInt32(item["Priority"])
                ));
            }

            _ = SQLManager_Kho.Instance.CustomerOrderDetailHistory_SaveListAsync(orders);
        }

        private void AddExportCode_btn_Click(object sender, EventArgs e)
        {
            mData.Columns.Add(new DataColumn("ExportCode", typeof(string)));
            foreach (DataRow row in mData.Rows)
            {
                DateTime ExportDate = Convert.ToDateTime(row["ExportDate"]);
                ExportDate = new DateTime(Convert.ToInt32(year_tb.Text), ExportDate.Month, ExportDate.Day);
                row["ExportDate"] = ExportDate.ToString();
                row["ExportCode"] = $"MCX_{ExportDate.Day.ToString("D2")}{ExportDate.Month.ToString("D2")}{ExportDate.Year}";
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var grouped = mData.AsEnumerable()
                                            .GroupBy(r => new
                                            {
                                                ProductName_VN = Utils.RemoveVietnameseSigns(r.Field<string>("ProductName_VN")).Replace(" ", "").Trim(),
                                                ExportCode = r.Field<string>("ExportCode")
                                            })
                                            .Select(g => new
                                            {
                                                ExportCode = g.Key.ExportCode,
                                                // ProductName_VN: phần giống nhau
                                                ProductName_EN = GetCommonPrefixCleaned(g.Select(r => r.Field<string>("ProductName_EN")).ToList(), g.Select(r => r.Field<string>("Package")).FirstOrDefault()),
                                                ProductName_VN = GetCommonPrefixCleaned(g.Select(r => r.Field<string>("ProductName_VN")).ToList(), g.Select(r => r.Field<string>("Package")).FirstOrDefault()),

                                                // Sum
                                                PCS = g.Sum(r =>
                                                {
                                                    var val = r["PCS"];
                                                    if (val == DBNull.Value || val == null) return 0;

                                                    int pcs = 0;
                                                    decimal d;
                                                    if (decimal.TryParse(val.ToString(), out d))
                                                    {
                                                        pcs = (int)d; // cắt phần thập phân
                                                    }
                                                    return pcs;
                                                }),
                                                AmountCHF = g.Sum(r =>
                                                {
                                                    string v = r["AmountCHF"]?.ToString() ?? "0";
                                                    return Convert.ToDecimal(v);
                                                }),

                                                NetWeight = g.Sum(r =>
                                                {
                                                    string v = r["NetWeight"]?.ToString() ?? "0";
                                                    return Convert.ToDecimal(v);
                                                }),

                                                // Lấy giá trị đầu tiên
                                                PLU = g.Select(r => r.Field<string>("PLU")).FirstOrDefault().ToLower().Trim(),
                                                Package = g.Select(r => r.Field<string>("Package")).FirstOrDefault().ToLower().Trim(),
                                                ExportDate = g.Select(r =>
                                                {
                                                    string v = r["ExportDate"]?.ToString();
                                                    return DateTime.TryParse(v, out var dte) ? dte : DateTime.MinValue;
                                                }).FirstOrDefault()
                                            })
                                            .ToList();

            DataTable dt = new DataTable();

            // Tạo đúng các cột giống mData
            dt.Columns.Add("Priority", typeof(int));
            dt.Columns.Add("ProductName_EN", typeof(string));
            dt.Columns.Add("ExportCode", typeof(string));
            dt.Columns.Add("ProductName_VN", typeof(string));
            dt.Columns.Add("PCS", typeof(int));
            dt.Columns.Add("AmountCHF", typeof(decimal));
            dt.Columns.Add("NetWeight", typeof(decimal));
            dt.Columns.Add("Package", typeof(string));
            dt.Columns.Add("ExportDate", typeof(DateTime));

            var productPacking_dt = await SQLStore_Kho.Instance.getProductpackingAsync();

            foreach (var g in grouped)
            {
                var row = dt.NewRow();

                
                DataRow[] rows = productPacking_dt.Select($"PLU = '{g.PLU}'");

                if (rows.Length > 0)
                {
                    row["Priority"] = rows[0]["Priority"];
                }
                else
                {
                    row["Priority"] = 200000;
                }

                
               // row["PLU"] = g.ProductName_EN
                row["ProductName_EN"] = g.ProductName_EN;
                row["ExportCode"] = g.ExportCode;
                row["ProductName_VN"] = g.ProductName_VN;
                row["PCS"] = g.PCS;
                row["AmountCHF"] = g.AmountCHF;
                row["NetWeight"] = g.NetWeight;
                row["Package"] = g.Package;
                row["ExportDate"] = g.ExportDate;
                dt.Rows.Add(row);
            }

            mData = dt;
            dataGV.DataSource = mData;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var groupedByMonth = mData.AsEnumerable()
                                            .GroupBy(r =>
                                            {
                                                DateTime exportDate = DateTime.MinValue;
                                                string dateStr = r["ExportDate"]?.ToString();
                                                if (!string.IsNullOrWhiteSpace(dateStr))
                                                    DateTime.TryParse(dateStr, out exportDate);

                                                return new
                                                {
                                                    ProductName_VN = Utils.RemoveVietnameseSigns(r.Field<string>("ProductName_VN")).Replace(" ","").Trim(),
                                                    Year = exportDate.Year,
                                                    Month = exportDate.Month
                                                };
                                            })
                                            .Select(g => new
                                            {
                                                ExportCode = $"MXC_1_{g.Key.Month}_{g.Key.Year}",
                                                ExportDate = new DateTime(g.Key.Year, g.Key.Month, 1),
                                                // ProductName_VN: phần giống nhau
                                                ProductName_EN = GetCommonPrefixCleaned(
                                                    g.Select(r => r.Field<string>("ProductName_EN")).ToList(),
                                                    g.Select(r => r.Field<string>("Package")).FirstOrDefault()
                                                ),
                                                ProductName_VN = GetCommonPrefixCleaned(
                                                    g.Select(r => r.Field<string>("ProductName_VN")).ToList(),
                                                    g.Select(r => r.Field<string>("Package")).FirstOrDefault()
                                                ),

                                                // Sum các cột
                                                PCS = g.Sum(r =>
                                                {
                                                    var val = r["PCS"];
                                                    if (val == DBNull.Value || val == null) return 0;
                                                    return Convert.ToInt32(val);
                                                }),
                                                AmountCHF = g.Sum(r =>
                                                {
                                                    string v = r["AmountCHF"]?.ToString() ?? "0";
                                                    return Convert.ToDecimal(v);
                                                }),
                                                NetWeight = g.Sum(r =>
                                                {
                                                    string v = r["NetWeight"]?.ToString() ?? "0";
                                                    return Convert.ToDecimal(v);
                                                }),

                                                Priority = g.Select(r => r.Field<int>("Priority")).FirstOrDefault(),
                                                Package = g.Select(r => r.Field<string>("Package")).FirstOrDefault().ToLower().Trim(),                                             
                                            })
                                            .ToList();

            DataTable dt = new DataTable();

            // Tạo đúng các cột giống mData
            dt.Columns.Add("ProductName_EN", typeof(string));
            dt.Columns.Add("ExportCode", typeof(string));
            dt.Columns.Add("ProductName_VN", typeof(string));
            dt.Columns.Add("PCS", typeof(int));
            dt.Columns.Add("AmountCHF", typeof(decimal));
            dt.Columns.Add("NetWeight", typeof(decimal));
            dt.Columns.Add("Package", typeof(string));
            dt.Columns.Add("ExportDate", typeof(DateTime));
            dt.Columns.Add("Priority", typeof(int));

            foreach (var g in groupedByMonth)
            {
                var row = dt.NewRow();
                row["ProductName_EN"] = g.ProductName_EN;
                row["ExportCode"] = g.ExportCode;
                row["ProductName_VN"] = g.ProductName_VN;
                row["PCS"] = g.PCS;
                row["AmountCHF"] = g.AmountCHF;
                row["NetWeight"] = g.NetWeight;
                row["Package"] = g.Package;
                row["ExportDate"] = g.ExportDate;
                row["Priority"] = g.Priority;
                dt.Rows.Add(row);
            }

            mData = dt;
            dataGV.DataSource = mData;
        }

        public string CleanProductName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "";

            // Xóa phần khối lượng: tìm pattern "số + khoảng trắng + (g|gr|kg)" ở cuối chuỗi
            // ví dụ: "galadium 200 gr", "baby corn 1 kg"
            string pattern = @"\s*\d+(\.\d+)?\s*(g|gr|kg)\s*$";
            string cleaned = Regex.Replace(name, pattern, "", RegexOptions.IgnoreCase);

            return cleaned.Trim();
        }

        public string GetCommonPrefixCleaned(List<string> names, string package)
        {
            if (names == null || names.Count == 0)
                return "";

            var cleanedNames = names;
            string pa = package.ToLower().Trim();
            if (pa.CompareTo("kg") == 0 || pa.CompareTo("weight") == 0)
                cleanedNames = names.Select(n => CleanProductName(n)).ToList();

            string prefix = cleanedNames[0];

            foreach (var name in cleanedNames)
            {
                int len = Math.Min(prefix.Length, name.Length);
                int i = 0;

                while (i < len && prefix[i] == name[i])
                {
                    i++;
                }

                prefix = prefix.Substring(0, i);

                if (string.IsNullOrWhiteSpace(prefix))
                    return "";
            }

            if (prefix.Contains("("))
                prefix = prefix.Substring(0, prefix.IndexOf("("));

            return prefix.Trim();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach(DataRow row in mData.Rows)
            {
                string packing = row["Packing"].ToString().ToLower().Trim();
                if (packing.CompareTo("weight") == 0)
                    row["Package"] = packing;

                if (string.IsNullOrWhiteSpace(row["Package"].ToString().Trim()))
                    row["Package"] = packing;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
