using System;
using System.Collections.Generic;
using System.Data;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class EditExcelFormProductMain : Form
    {
        public EditExcelFormProductMain()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            openFileExcel_mi.Click += openFileExcel_mi_Click;
            save_mi.Click += save_mi_Click;
            importToSQL_productMain_mi.Click += importToSQL_mi_Click;
            EditProductName_btn.Click += EditProductName_btn_Click;
            editpackage_btn.Click += editpackage_btn_Click;
            checkSKU_btn.Click += checkSKU_btn_Click;
        }

        private void importToSQL_mi_Click(object sender, EventArgs e)
        {
            ImportDataGridViewToSql(dataGV, SQLManager.Instance.ql_NhanSu_conStr(), "ProductSKU");
        }

        private void importToSQL_ProductPacking_mi_Click(object sender, EventArgs e)
        {
            ImportDataGridViewToSql(dataGV, SQLManager.Instance.ql_NhanSu_conStr(), "ProductPacking");
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
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để import.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tạo 1 DataTable chỉ chứa schema và thêm 1 dòng đầu tiên
           // DataTable oneRowTable = dt.Clone();
          //  oneRowTable.ImportRow(dt.Rows[0]); // chỉ lấy dòng đầu tiên

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Bật IDENTITY_INSERT
                        using (SqlCommand cmd = new SqlCommand($"SET IDENTITY_INSERT {tableName} ON", conn, tran))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran))
                        {
                            bulkCopy.DestinationTableName = tableName;

                            // Mapping tất cả cột, bao gồm SKU
                            foreach (DataColumn col in dt.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }

                            bulkCopy.WriteToServer(dt);
                        }

                        // Tắt IDENTITY_INSERT
                        using (SqlCommand cmd = new SqlCommand($"SET IDENTITY_INSERT {tableName} OFF", conn, tran))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();

                        // Thông báo kết quả
                        MessageBox.Show(
                            $"Đã import thành công {dt.Rows.Count} dòng vào bảng {tableName}.",
                            "Import thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show(
                            "Lỗi khi import dữ liệu: " + ex.Message,
                            "Lỗi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }



        private void DeleteSKUDuplicate_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            dt.DefaultView.Sort = "";
            // ===== Hàm chuẩn hoá PackingList =====
            string NormalizePacking(string input)
            {
                if (string.IsNullOrWhiteSpace(input)) return "";

                string text = input.Trim().ToLower();

                // Regex lấy đơn vị ở cuối
                var match = System.Text.RegularExpressions.Regex.Match(text, @"(gr|g|kg|weight)$");

                if (match.Success)
                {
                    string unit = match.Groups[1].Value;
                    if (unit == "g") unit = "gr"; // đổi g -> gr
                    return unit;
                }

                return "";
            }

            // ===== Bước 1: Gán SKU nếu trống =====
            for (int i = dataGV.Rows.Count - 1; i >= 0; i--)
            {
                DataGridViewRow row = dataGV.Rows[i];
                if (row.IsNewRow) continue;

                string PLU = row.Cells["PLU"].Value?.ToString().Trim() ?? "";
                string SKU = row.Cells["SKU"].Value?.ToString().Trim() ?? "";

                if (string.IsNullOrEmpty(SKU))
                {
                    if (string.IsNullOrEmpty(PLU))
                    {
                        // SKU trống và PLU trống -> xóa luôn
                        dataGV.Rows.RemoveAt(i);
                        continue;
                    }
                    else
                    {
                        // SKU trống nhưng có PLU -> cấp SKU mới (max + 1)
                        int maxSku = 0;
                        foreach (DataGridViewRow r in dataGV.Rows)
                        {
                            if (r.IsNewRow) continue;
                            string skuVal = r.Cells["SKU"].Value?.ToString().Trim();
                            if (int.TryParse(skuVal, out int skuNum))
                                if (skuNum > maxSku) maxSku = skuNum;
                        }

                        row.Cells["SKU"].Value = (maxSku + 1).ToString();
                    }
                }
            }

            // ===== Bước 2: Xử lý trùng SKU =====
            Dictionary<string, DataGridViewRow> firstRowMap = new Dictionary<string, DataGridViewRow>();
            Dictionary<string, HashSet<string>> packingMap = new Dictionary<string, HashSet<string>>();
            HashSet<string> seen = new HashSet<string>();

            for (int i = dataGV.Rows.Count - 1; i >= 0; i--)
            {
                DataGridViewRow row = dataGV.Rows[i];
                if (row.IsNewRow) continue;

                string sku = row.Cells["SKU"].Value?.ToString().Trim() ?? "";
                if (string.IsNullOrEmpty(sku)) continue;

                string currentPacking = NormalizePacking(row.Cells["PackingList"].Value?.ToString());

                if (seen.Contains(sku))
                {
                    // SKU đã gặp -> merge PackingList
                    DataGridViewRow keptRow = firstRowMap[sku];

                    if (!string.IsNullOrEmpty(currentPacking))
                    {
                        if (!packingMap[sku].Contains(currentPacking))
                        {
                            packingMap[sku].Add(currentPacking);
                            keptRow.Cells["PackingList"].Value = string.Join("-", packingMap[sku]);
                        }
                    }

                    // Xóa row hiện tại
                    dataGV.Rows.RemoveAt(i);
                }
                else
                {
                    seen.Add(sku);
                    firstRowMap[sku] = row;

                    packingMap[sku] = new HashSet<string>();
                    if (!string.IsNullOrEmpty(currentPacking))
                    {
                        packingMap[sku].Add(currentPacking);
                        row.Cells["PackingList"].Value = currentPacking;
                    }
                    else
                    {
                        row.Cells["PackingList"].Value = "";
                    }
                }
            }
        }



        private void checkSKU_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            dt.DefaultView.Sort = "";
            HashSet<string> seen = new HashSet<string>();
            List<string> duplicates = new List<string>();
            List<string> invalids = new List<string>(); // chứa SKU không phải int

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                string sku = row.Cells["SKU"].Value?.ToString().Trim() ?? "";
                if (string.IsNullOrEmpty(sku)) continue;

                // Check không phải số nguyên
                if (!int.TryParse(sku, out _))
                {
                    if (!invalids.Contains(sku))
                        invalids.Add(sku);
                    continue; // bỏ qua, không check trùng nữa
                }

                // Check trùng
                if (!seen.Add(sku))
                {
                    if (!duplicates.Contains(sku))
                        duplicates.Add(sku);
                }
            }

            // Kết quả
            if (duplicates.Count > 0 || invalids.Count > 0)
            {
                string msg = "";
                if (duplicates.Count > 0)
                    msg += "Có SKU trùng nhau:\n" + string.Join(", ", duplicates) + "\n\n";
                if (invalids.Count > 0)
                    msg += "Có SKU không phải số nguyên:\n" + string.Join(", ", invalids);

                MessageBox.Show(msg, "Check SKU", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Tất cả SKU hợp lệ, không trùng và đều là số nguyên.",
                                "Check SKU", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void editpackage_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            dt.DefaultView.Sort = "";
            // mapping từ khóa -> giá trị chuẩn hóa
            var keywordMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "box", "box" },
                { "hộp", "box" },
                { "bottle", "bottle" },
                { "bag", "bag" },
                { "pc", "pc" },
                { "set", "set" },
                { "weight", "weight" },
                { "gói", "pack" },
                { "kg", "kg" },
                { "bó", "bó" },
                { "pack", "pack" }
            };

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                string package = row.Cells["Package"].Value?.ToString() ?? "";
                string packingList = row.Cells["PackingList"].Value?.ToString().Trim().ToLower() ?? "";

                if (!string.IsNullOrEmpty(package))
                {
                    // Ưu tiên: nếu PackingList là "weight" -> Package = "weight"
                    if (packingList.Contains("weight"))
                    {
                        row.Cells["Package"].Value = "weight";
                        row.Cells["PackingList"].Value = "";
                    }
                    else
                    {
                        string lower = package.ToLower();

                        foreach (var kv in keywordMap)
                        {
                            if (lower.Contains(kv.Key.ToLower()))
                            {
                                row.Cells["Package"].Value = kv.Value.ToLower();
                                break; // match 1 từ khóa là đủ
                            }
                        }
                    }
                }
            }
        }


        private void Xoa_SP_Khong_Cần_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            dt.DefaultView.Sort = "";
            for (int i = dataGV.Rows.Count - 1; i >= 0; i--)
            {
                DataGridViewRow row = dataGV.Rows[i];
                if (row.IsNewRow) continue;

                // --- CHECK ProductName (normalize: xóa " và ' rồi toLower để check)
                string productNameRaw = row.Cells["ProductName"].Value?.ToString() ?? "";
                string PLU = row.Cells["PLU"].Value?.ToString() ?? "";
                string SKU = row.Cells["SKU"].Value?.ToString() ?? "";
                string productNorm = productNameRaw
                    .Replace("\"", "")
                    .Replace("'", "")
                    .Replace("“", "")
                    .Replace("”", "")
                    .Trim()
                    .ToLowerInvariant();


                if (PLU.Replace(" ", "") == "")
                {
                    //Mứt khoai lang Box 200 gr     Mứt khoai lang Box 300 gr
                    if (productNorm.Contains("sugar-coated sweet potato") || productNorm.Contains("sugar-coated lotus seeds"))
                    {
                        dataGV.Rows.RemoveAt(i);
                        continue;
                    }
                }

                //Mứt gừng dẻo Box 300 gr        Mứt gừng dẻo  Box 180gr  
                if (SKU == "1019" || SKU == "708") 
                {
                    dataGV.Rows.RemoveAt(i);
                    continue;
                }

                // Bánh trung thu Girval
                if (productNorm.Contains("givral vegetarian") || productNorm.Contains("givral moon") || productNorm.Contains("givral sticky"))
                {
                    dataGV.Rows.RemoveAt(i);
                    continue; // tiếp tục vòng lặp (dong đã bị xóa)
                }

                // Trà Sữa Wangcha      Trà Sữa HILLWAY
                if (productNorm.Contains("wangcha") || productNorm.Contains("hillway"))
                {
                    dataGV.Rows.RemoveAt(i);
                    continue; // tiếp tục vòng lặp (dong đã bị xóa)
                }

                // Lá dong weight   -   Set trang trí Tết T07 set30 -   Set Lì Xì L05 set30
                if (productNorm.Contains("dong leaf") || productNorm.Contains("tet decoration set t") || productNorm.Contains("set of red envelope l"))
                {
                    dataGV.Rows.RemoveAt(i);
                    continue; // tiếp tục vòng lặp (dong đã bị xóa)
                }

                // Lê Hàn Quốc pc   -   Cam Xoàn pc -   Dưa hoàng kim pc
                if (productNorm.Contains("korean pear") || productNorm.Contains("king mandarin") || productNorm.Contains("golden melon"))
                {
                    dataGV.Rows.RemoveAt(i);
                    continue; // tiếp tục vòng lặp (dong đã bị xóa)
                }
            }
        }

        private void EditProductName_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            dt.DefaultView.Sort = "";
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                string nameVN = row.Cells["ProductNameVN"].Value?.ToString() ?? "";
                string nameEN = row.Cells["ProductNameEN"].Value?.ToString() ?? "";
                string name = row.Cells["ProductName"].Value?.ToString() ?? "";
                string plu = row.Cells["PLU"].Value?.ToString() ?? "";
                string package = row.Cells["Package"].Value?.ToString() ?? "";

                // Nếu ProductNameEN bắt đầu bằng ProductName -> cắt phần dư
                if (package == "kg")
                {
                    if (nameEN.StartsWith(name))
                    {
                        string extra = nameEN.Substring(name.Length).Trim(); // phần dư thừa, ví dụ "200g"
                        nameEN = name; // giữ lại phần chuẩn

                        // Nếu VN cũng chứa phần dư này thì cắt luôn
                        if (!string.IsNullOrEmpty(extra) && nameVN.EndsWith(extra))
                        {
                            nameVN = nameVN.Substring(0, nameVN.Length - extra.Length).Trim();
                        }
                    }

                    // cập nhật lại DataGridView
                    row.Cells["ProductNameEN"].Value = nameEN;
                    row.Cells["ProductNameVN"].Value = nameVN;
                }
            }
        }

        private void checkDuplicateProductNameEN_btn_Click(object sender, EventArgs e)
        {
            // Nếu DataGridView có AllowUserToAddRows = true thì hàng cuối là hàng trống → cần trừ đi 1
            int rowCount = dataGV.AllowUserToAddRows
                ? dataGV.Rows.Count - 1
                : dataGV.Rows.Count;

            MessageBox.Show("Tổng số dòng: " + rowCount.ToString());
        }

        private void normalizeProductNameEN_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            dt.DefaultView.Sort = "";
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                string nameEN = row.Cells["ProductNameEN"].Value?.ToString().Trim() ?? "";

                if (string.IsNullOrEmpty(nameEN) || nameEN.ToLower().Contains("unknow"))
                {
                    row.Cells["ProductNameEN"].Value = "Unknow";
                }
            }

        }

        private void normalizeBotanicalName_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            dt.DefaultView.Sort = "";
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                string nameEN = row.Cells["BotanicalName"].Value?.ToString().Trim() ?? "";

                if (string.IsNullOrEmpty(nameEN) || nameEN.ToLower().Contains("unknow"))
                {
                    row.Cells["BotanicalName"].Value = "Unknow";
                }
            }
        }

        private void DeleteSelectedRows_btn_Click(object sender, EventArgs e)
        {
            if (dataGV.CurrentCell != null) // kiểm tra có cell nào đang được chọn không
            {
                int colIndex = dataGV.CurrentCell.ColumnIndex; // lấy index cột
                dataGV.Columns.RemoveAt(colIndex); // xóa cột theo index
            }
        }

        private void chuanhoa_packingList_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            dt.DefaultView.Sort = "";
            // Danh sách từ khóa (chuyển về chữ thường để dễ so sánh)
            string[] keywords = { "box", "bag", "hộp", "hũ", "set30", "xấp", "tách", "hạt", "(", ")", "bó", "bottle", "gói", "set", "pc" };

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.Cells["PackingList"].Value != null)
                {
                    string originalText = row.Cells["PackingList"].Value.ToString();
                    string packingText = originalText.ToLower();

                    // 1) Xóa keyword
                    var foundWords = keywords
                        .Where(k => packingText.Contains(k))
                        .ToArray();

                    string cleaned = originalText;
                    foreach (var word in foundWords)
                    {
                        cleaned = System.Text.RegularExpressions.Regex.Replace(
                                        cleaned,
                                        System.Text.RegularExpressions.Regex.Escape(word),
                                        "",
                                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                                    );
                    }

                    // 2) Về chữ thường
                    cleaned = cleaned.ToLower();

                    // 3) Chuẩn hóa khoảng trắng thừa
                    cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\s+", " ").Trim();

                    // 4) Chuẩn hóa gr: "100g" hoặc "100gr" -> "100 gr"
                    cleaned = System.Text.RegularExpressions.Regex.Replace(
                        cleaned,
                        @"(\d+)\s*(g|gr)\b",
                        "$1 gr",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );

                    // 5) Chuẩn hóa kg: "100kg" -> "100 kg"
                    cleaned = System.Text.RegularExpressions.Regex.Replace(
                        cleaned,
                        @"(\d+)\s*kg\b",
                        "$1 kg",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );

                    // 6) Nếu chỉ còn "g" hoặc "gr" -> "gr"
                    if (cleaned.Trim() == "g" || cleaned.Trim() == "gr")
                    {
                        cleaned = "gr";
                    }
                    // Nếu chỉ còn "kg"
                    else if (cleaned.Trim() == "kg")
                    {
                        cleaned = "kg";
                    }

                    row.Cells["PackingList"].Value = cleaned;
                }
            }
        }

        private void edit_price_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            dt.DefaultView.Sort = "";

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                object value = row.Cells["PriceCNF"].Value;

                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    row.Cells["PriceCNF"].Value = 0;
                }
                else
                {
                    decimal tmp;
                    if (!decimal.TryParse(value.ToString(), out tmp))
                    {
                        row.Cells["PriceCNF"].Value = 0;
                    }
                }
            }
        }

        private void khongGia_khongPLU_btn_Click(object sender, EventArgs e)
        {
            // Danh sách log các dòng bị xóa
            List<string> logs = new List<string>();

            // Duyệt ngược để tránh lỗi khi xóa dòng
            for (int i = dataGV.Rows.Count - 1; i >= 0; i--)
            {
                DataGridViewRow row = dataGV.Rows[i];
                if (row.IsNewRow) continue;

                string plu = row.Cells["PLU"].Value?.ToString().Trim();
                string priceStr = row.Cells["PriceCNF"].Value?.ToString().Trim();
                string sku = row.Cells["SKU"].Value?.ToString().Trim();
                string productName = row.Cells["ProductNameVN"].Value?.ToString().Trim();

                bool removeRow = false;

                // Nếu cả PLU và PriceCNF đều rỗng thì xóa
                if (string.IsNullOrEmpty(plu))
                {
                    if(string.IsNullOrEmpty(priceStr))
                    {
                        removeRow = true;
                    }
                    else if (decimal.TryParse(priceStr, out decimal priceValue))
                    {
                        if (priceValue <= 0)
                        {
                            removeRow = true;
                        }
                    }
                }

                // Nếu SKU không phải số nguyên thì xóa
                if (!string.IsNullOrEmpty(sku) && !int.TryParse(sku, out _))
                {
                    removeRow = true;
                }

                if (removeRow)
                {
                    logs.Add($"Xóa SKU: {sku}, PLU: {plu}, Product: {productName}, Price: {priceStr}");
                    dataGV.Rows.RemoveAt(i);
                }
            }

            // Hiển thị log
            if (logs.Count > 0)
            {
                string message = string.Join(Environment.NewLine, logs);
                MessageBox.Show(message, "Danh sách dòng bị xóa",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Không có dòng nào bị xóa.", "Kết quả",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }




        private void changeSKU_diff_kg_btn_Click(object sender, EventArgs e)
        {
            // Dictionary để đếm số lần gặp SKU
            Dictionary<string, int> skuCounter = new Dictionary<string, int>();

            // Danh sách log các SKU bị đổi
            List<string> logs = new List<string>();

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                string sku = row.Cells["SKU"].Value?.ToString().Trim();
                string package = row.Cells["Package"].Value?.ToString().Trim().ToLower();
                string productName = row.Cells["ProductNameVN"].Value?.ToString().Trim(); // giả sử có cột ProductName

                if (string.IsNullOrEmpty(sku)) continue;

                if (!skuCounter.ContainsKey(sku))
                {
                    // SKU lần đầu tiên -> giữ nguyên
                    skuCounter[sku] = 0;
                }
                else
                {
                    // SKU bị trùng -> xử lý
                    if (!string.Equals(package, "kg", StringComparison.OrdinalIgnoreCase))
                    {
                        skuCounter[sku]++; // tăng số đếm
                        string newSku = sku + skuCounter[sku];

                        // Ghi log trước khi đổi
                        logs.Add($"SKU đổi: {sku} → {newSku} | Product: {productName}");

                        // Đổi SKU trong DataGridView
                        row.Cells["SKU"].Value = newSku;
                    }
                    // nếu Package = "kg" thì giữ nguyên
                }
            }

            // Hiển thị log nếu có thay đổi
            if (logs.Count > 0)
            {
                string message = string.Join(Environment.NewLine, logs);
                MessageBox.Show(message, "Danh sách SKU bị đổi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Không có SKU nào bị đổi.", "Kết quả",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



    }
}

