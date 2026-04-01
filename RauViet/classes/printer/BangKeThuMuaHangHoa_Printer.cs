using ClosedXML.Excel;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class BangKeThuMuaHangHoa_Printer
{
    private DataTable mData_dt;
    private List<KeyValuePair<DateTime, DataTable>> mDataGroup;
    private string mReceiveName;
    private int rowIndex = 0, groupIndex = 0; // để phân trang
    private int lineHeight = 24;
    private int mCountSTT;
    private int mSellerID;
    private int total = 0;
    DataRow mSellerRow;
    private DateTime mFromDate, mToDate;
    public BangKeThuMuaHangHoa_Printer(int month, int year, int sellerID, DataTable data_dt)
    {
        mFromDate = new DateTime(year, month, 1);
        mToDate = mFromDate.AddMonths(1).AddDays(-1);

        DataView dv = new DataView(data_dt);
        dv.RowFilter = $@"SellerID = {sellerID} AND TransactionDate >= #{mFromDate:MM/dd/yyyy}# AND TransactionDate <= #{mToDate:MM/dd/yyyy}#";
        dv.Sort = "TransactionDate ASC";

        this.mData_dt = dv.ToTable();

        mDataGroup = mData_dt.AsEnumerable().GroupBy(r => r.Field<DateTime>("TransactionDate").Date)
                                        .Select(g =>
                                        {
                                            DataTable dt = mData_dt.Clone();
                                            foreach (var row in g)
                                            {
                                                dt.ImportRow(row);
                                            }
                                            return new KeyValuePair<DateTime, DataTable>(g.Key, dt);
                                        }).ToList();

        this.mCountSTT = 1;
        mSellerID = sellerID;
        mReceiveName = UserManager.Instance.fullName;
        total = 0;
    }

    public async Task loadData()
    {
        var Seller_dt = await SQLStore_Kho.Instance.GetSupplierAsync();
        mSellerRow = Seller_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("SupplierID") == mSellerID);
    }

    public void PrintPreview(Form owner)
    {
        rowIndex = 0; // reset trước khi in
        groupIndex = 0;
        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.Margins = new Margins(50, 50, 0, 0);
        pd.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
        pd.DefaultPageSettings.Landscape = false;
        pd.PrintPage += Pd_PrintPage;
        PrintPreviewDialog preview = new PrintPreviewDialog();
        preview.Document = pd;
        preview.WindowState = FormWindowState.Maximized;  // phóng to toàn màn hình
        preview.StartPosition = FormStartPosition.CenterScreen;

        foreach (Control c in preview.Controls)
        {
            if (c is ToolStrip ts)
            {
                foreach (ToolStripItem item in ts.Items)
                {
                    if (
                        item.Name.Equals("print", StringComparison.OrdinalIgnoreCase) ||
                        item.Name.Equals("printButton", StringComparison.OrdinalIgnoreCase) ||
                        item.Name.Equals("toolStripButton1", StringComparison.OrdinalIgnoreCase) ||
                        (item.ToolTipText?.Contains("Print") ?? false) ||
                        (item.Text?.Contains("Print") ?? false)
                       )
                    {
                        item.Visible = false;
                    }
                }
            }
        }

        preview.ShowDialog(owner);
    }

    public void PrintDirect()
    {
        rowIndex = 0; // reset trước khi in
        groupIndex = 0;
        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.Margins = new Margins(50, 50, 0, 0);
        pd.DefaultPageSettings.Landscape = false;
        pd.PrintPage += Pd_PrintPage;
        if (pd.PrinterSettings.CanDuplex)
        {
            pd.PrinterSettings.Duplex = Duplex.Vertical;
        }

        // 🔹 Hộp thoại chọn máy in
        PrintDialog printDialog = new PrintDialog();
        printDialog.Document = pd;
        printDialog.AllowSomePages = false;
        printDialog.AllowSelection = false;
        printDialog.UseEXDialog = true;

        if (printDialog.ShowDialog() == DialogResult.OK)
        {
            // Máy in đã được gán tự động cho pd
            pd.Print();
        }
    }

    private void Pd_PrintPage(object sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics;
        int pageWidth = e.MarginBounds.Width;
        int pageHeight = e.MarginBounds.Height;

        // Cột
        int colWidth_1 = 85, colWidth_2 = 220, colWidth_3 = 80, colWidth_4 = 95, colWidth_5 = 110;
        int col_1 = e.MarginBounds.Left;
        int col_2 = col_1 + colWidth_1;
        int col_3 = col_2 + colWidth_2;
        int col_4 = col_3 + colWidth_3;
        int col_5 = col_4 + colWidth_4;
        int col_6 = col_5 + colWidth_5;
        int colWidth_6 = e.MarginBounds.Right - col_6;

        Font titleFont = new Font("Times New Roman", 18, FontStyle.Bold);

        Font normalFont = new Font("Times New Roman", 12);
        Font tableHeaderFont = new Font("Times New Roman", 12, FontStyle.Bold);

        int y = e.MarginBounds.Top;

        SolidBrush bgBrush_LightGray = new SolidBrush(Color.FromArgb(217, 217, 217));
        // Header chỉ in 1 lần ở đầu trang
        if (groupIndex == 0)
        {
            Utils.DrawCellText(g, $"BẢNG KÊ THU MUA HÀNG HÓA, DỊCH VỤ MUA VÀO KHÔNG CÓ HÓA ĐƠN", titleFont, new Rectangle(col_1, y, col_6 + colWidth_6 - col_1, lineHeight * 3), StringAlignment.Center);
            y += (lineHeight * 3);
            Utils.DrawCellText(g, $"Đơn vị thu mua: Công Ty Cổ Phần Việt Rau", normalFont, new Rectangle(col_1, y, col_6 + colWidth_6 - col_1, lineHeight), StringAlignment.Near);
            y += lineHeight;
            Utils.DrawCellText(g, $"Địa chỉ: Tổ 1, Ấp 4, xã Phước Thái, Tỉnh Đồng Nai", normalFont, new Rectangle(col_1, y, col_6 + colWidth_6 - col_1, lineHeight), StringAlignment.Near);
            y += lineHeight;
            Utils.DrawCellText(g, $"Nơi tổ chức thu mua: Tổ 1, Ấp 4, xã Phước Thái, Tỉnh Đồng Nai", normalFont, new Rectangle(col_1, y, col_6 + colWidth_6 - col_1, lineHeight), StringAlignment.Near);
            y += lineHeight;
            Utils.DrawCellText(g, $"Người phụ trách thu mua: {mReceiveName}", normalFont, new Rectangle(col_1, y, col_6 + colWidth_6 - col_1, lineHeight), StringAlignment.Near);
            y += lineHeight;
            
            g.FillRectangle(bgBrush_LightGray, new Rectangle(col_1, y, col_6 + colWidth_6 - col_1, lineHeight*2));

            g.DrawRectangle(Pens.Gray, col_1, y, col_6 + colWidth_6 - col_1, lineHeight * 2);
            g.DrawLine(Pens.Gray, col_2, y + lineHeight, col_6 + colWidth_6, y + lineHeight);
            g.DrawLine(Pens.Gray, col_2, y, col_2, y + lineHeight * 2);
            g.DrawLine(Pens.Gray, col_3, y + lineHeight, col_3, y + lineHeight * 2);
            g.DrawLine(Pens.Gray, col_4, y + lineHeight, col_4, y + lineHeight * 2);
            g.DrawLine(Pens.Gray, col_5, y + lineHeight, col_5, y + lineHeight * 2);
            g.DrawLine(Pens.Gray, col_6, y + lineHeight, col_6, y + lineHeight * 2);

            Utils.DrawCellText(g, "Thời gian mua hàng", tableHeaderFont, new Rectangle(col_1, y, colWidth_1, lineHeight * 2), StringAlignment.Center);
            Utils.DrawCellText(g, "Người Bán", tableHeaderFont, new Rectangle(col_2, y, col_6 + colWidth_6 - col_2, lineHeight), StringAlignment.Center);
            y += lineHeight;
            Utils.DrawCellText(g, "Họ tên người bán", tableHeaderFont, new Rectangle(col_2, y, colWidth_2, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, "Số CCCD", tableHeaderFont, new Rectangle(col_3, y, colWidth_3, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, "Số ĐT", tableHeaderFont, new Rectangle(col_4, y, colWidth_4, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, "Địa chỉ", tableHeaderFont, new Rectangle(col_5, y, colWidth_5, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, "Số TK", tableHeaderFont, new Rectangle(col_6, y, colWidth_6, lineHeight), StringAlignment.Center);
            y += lineHeight;

            g.DrawRectangle(Pens.Gray, col_1, y, col_6 + colWidth_6 - col_1, lineHeight * 3);
            g.DrawLine(Pens.Gray, col_2, y, col_2, y + lineHeight * 3);
            g.DrawLine(Pens.Gray, col_3, y, col_3, y + lineHeight * 3);
            g.DrawLine(Pens.Gray, col_4, y, col_4, y + lineHeight * 3);
            g.DrawLine(Pens.Gray, col_5, y, col_5, y + lineHeight * 3);
            g.DrawLine(Pens.Gray, col_6, y, col_6, y + lineHeight * 3);

            Utils.DrawCellText(g, $"{mFromDate.ToString("dd/MM/yyyy")}\nđến\n{mToDate.ToString("dd/MM/yyyy")}", normalFont, new Rectangle(col_1, y, colWidth_1, lineHeight * 3), StringAlignment.Center);
            Utils.DrawCellText(g, mSellerRow["SupplierName"].ToString().Trim(), normalFont, new Rectangle(col_2, y, colWidth_2, lineHeight*3), StringAlignment.Center);
            Utils.DrawCellText(g, mSellerRow["Citizen"].ToString().Trim(), normalFont, new Rectangle(col_3, y, colWidth_3, lineHeight*3), StringAlignment.Center);
            Utils.DrawCellText(g, mSellerRow["Phone"].ToString().Trim(), normalFont, new Rectangle(col_4, y, colWidth_4, lineHeight*3), StringAlignment.Center);
            Utils.DrawCellText(g, mSellerRow["Address"].ToString().Trim(), normalFont, new Rectangle(col_5, y, colWidth_5, lineHeight*3), StringAlignment.Center);
            Utils.DrawCellText(g, $"{mSellerRow["BankName"].ToString().Trim()}\n{mSellerRow["BankAccount"].ToString().Trim()}", normalFont, new Rectangle(col_6, y, colWidth_6, lineHeight*3), StringAlignment.Center);
            y += lineHeight*3;

            Utils.DrawCellText(g, "Hàng hóa mua vào", tableHeaderFont, new Rectangle(col_1, y, col_6 + colWidth_6 - col_1, lineHeight), StringAlignment.Center);
            y += lineHeight;

        }

        g.FillRectangle(bgBrush_LightGray, new Rectangle(col_1, y, col_6 + colWidth_6 - col_1, lineHeight*2));

        g.DrawRectangle(Pens.Gray, col_1, y, col_6 + colWidth_6 - col_1, lineHeight * 2);
        g.DrawLine(Pens.Gray, col_2, y, col_2, y + lineHeight * 2);
        g.DrawLine(Pens.Gray, col_3, y, col_3, y + lineHeight * 2);
        g.DrawLine(Pens.Gray, col_4, y, col_4, y + lineHeight * 2);
        g.DrawLine(Pens.Gray, col_5, y, col_5, y + lineHeight * 2);
        g.DrawLine(Pens.Gray, col_6, y, col_6, y + lineHeight * 2);

        Utils.DrawCellText(g, "Thời gian mua hàng", tableHeaderFont, new Rectangle(col_1, y, colWidth_1, lineHeight * 2), StringAlignment.Center);        
        Utils.DrawCellText(g, "Tên mặt hàng", tableHeaderFont, new Rectangle(col_2, y, colWidth_2, lineHeight * 2), StringAlignment.Center);
        Utils.DrawCellText(g, "ĐVT", tableHeaderFont, new Rectangle(col_3, y, colWidth_3, lineHeight * 2), StringAlignment.Center);
        Utils.DrawCellText(g, "Số lượng", tableHeaderFont, new Rectangle(col_4, y, colWidth_4, lineHeight * 2), StringAlignment.Center);
        Utils.DrawCellText(g, "Đơn giá", tableHeaderFont, new Rectangle(col_5, y, colWidth_5, lineHeight * 2), StringAlignment.Center);
        Utils.DrawCellText(g, "Tổng giá trị thanh toán", tableHeaderFont, new Rectangle(col_6, y, colWidth_6, lineHeight * 2), StringAlignment.Center);
        y += lineHeight*2;


        while (groupIndex < mDataGroup.Count)
        {
            var kv = mDataGroup[groupIndex];
            DateTime date = kv.Key;
            DataTable data = kv.Value;
            int count = data.Rows.Count;

            if (y + lineHeight * count > pageHeight)
            {
                e.HasMorePages = true;
                return; // sang trang tiếp theo
            }

            g.DrawRectangle(Pens.Gray, col_1, y, col_6 + colWidth_6 - col_1, lineHeight * count);
            g.DrawLine(Pens.Gray, col_2, y, col_2, y + lineHeight * count);
            g.DrawLine(Pens.Gray, col_3, y, col_3, y + lineHeight * count);
            g.DrawLine(Pens.Gray, col_4, y, col_4, y + lineHeight * count);
            g.DrawLine(Pens.Gray, col_5, y, col_5, y + lineHeight * count);
            g.DrawLine(Pens.Gray, col_6, y, col_6, y + lineHeight * count);

            Utils.DrawCellText(g, date.ToString("dd/MM/yyyy"), normalFont, new Rectangle(col_1, y, colWidth_1, lineHeight * count), StringAlignment.Center);

            while (rowIndex < data.Rows.Count)
            {      
                DataRow row = data.Rows[rowIndex];
                string spName = Convert.ToString(row["Name_VN"]);
                string note = Convert.ToString(row["Note"]);
                string unit = Convert.ToString(row["Package"]);
                DateTime transactionDate = Convert.ToDateTime(row["TransactionDate"]);
                decimal quantity = Convert.ToDecimal(row["Quantity"]);
                int price = Convert.ToInt32(row["Price"]);

                string rename = note?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                                    .FirstOrDefault(l => Utils.RemoveVietnameseSigns(l).ToLower().Contains("rename"))
                                                    ?.Split(':')
                                                    .Skip(1)
                                                    .FirstOrDefault()
                                                    ?.Trim() ?? "";
                if (string.IsNullOrEmpty(rename) == false)
                    spName = rename;

                if (rowIndex > 0)
                    g.DrawLine(Pens.Gray, col_2, y, col_6 + colWidth_6, y);

                int thanhtien = Convert.ToInt32(price * quantity);
                total += thanhtien;

                Utils.DrawCellText(g, spName, normalFont, new Rectangle(col_2, y, colWidth_2, lineHeight), StringAlignment.Near);
                Utils.DrawCellText(g, unit, normalFont, new Rectangle(col_3, y, colWidth_3, lineHeight), StringAlignment.Center);
                Utils.DrawCellText(g, quantity.ToString("N1"), normalFont, new Rectangle(col_4, y, colWidth_4, lineHeight), StringAlignment.Far);
                Utils.DrawCellText(g, price.ToString("N0"), normalFont, new Rectangle(col_5, y, colWidth_5, lineHeight), StringAlignment.Far);
                Utils.DrawCellText(g, thanhtien.ToString("N0"), normalFont, new Rectangle(col_6, y, colWidth_6, lineHeight), StringAlignment.Far);
                y += lineHeight;
                rowIndex++;
                mCountSTT++;
            }

            groupIndex++;
            rowIndex = 0;
        }

        if (y + lineHeight * 10 > pageHeight)
        {
            e.HasMorePages = true;
            return; // sang trang tiếp theo
        }

        g.DrawRectangle(Pens.Gray, col_1, y, col_6 + colWidth_6 - col_1, lineHeight);
        g.DrawLine(Pens.Gray, col_6, y, col_6, y + lineHeight);
        Utils.DrawCellText(g, $"Tổng giá trị hàng hóa mua vào", new Font("Times New Roman", 11, FontStyle.Bold), new Rectangle(col_1, y, col_6 - col_1, lineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, total.ToString("N0"), new Font("Times New Roman", 11, FontStyle.Bold), new Rectangle(col_6, y, colWidth_6, lineHeight), StringAlignment.Far);
        y += lineHeight;
        g.DrawRectangle(Pens.Gray, col_1, y, col_6 + colWidth_6 - col_1, lineHeight*2);
        Utils.DrawCellText(g,$"Số tiền viết bằng chữ: {Utils.NumberToText(total)}" , new Font("Times New Roman", 11, FontStyle.Italic), new Rectangle(col_1, y, col_6 + colWidth_6 - col_1, lineHeight*2), StringAlignment.Near);
        y += lineHeight;
        Utils.DrawCellText(g, $"ngày {mToDate:dd}, tháng {mToDate:MM}, năm {mToDate:yyyy}\n Người bán hàng hóa\n\n\n\n\n\n{mSellerRow["SupplierName"].ToString()}", new Font("Times New Roman", 11, FontStyle.Italic), new Rectangle(col_5, y, col_6 + colWidth_6 - col_5, lineHeight * 8), StringAlignment.Center);
        Utils.DrawCellText(g, $"\n\n Người mua hàng hóa\n\n\n\n\n\n{mReceiveName}", new Font("Times New Roman", 11, FontStyle.Italic), new Rectangle(col_1, y, col_3 - col_1, lineHeight * 8), StringAlignment.Center);
        e.HasMorePages = false;

    }



    public void ExportExcel()
    {
        //try
        //{
        //    using (var wb = new XLWorkbook())
        //    {
        //        var ws = wb.Worksheets.Add("HoaDon");
        //        ws.Style.Font.FontName = "Times New Roman";
        //        ws.Style.Font.FontSize = 16;

        //        int row = 1;

        //        // Header
        //        ws.Cell(row, 1).Value = $"NGƯỜI BÁN: {SellerName}";
        //        ws.Range(row, 1, row, 5).Merge().Style.Font.Bold = true;
        //        row += 2;

        //        ws.Cell(row, 1).Value = "DANH SÁCH HÀNG MUA NGOÀI";
        //        ws.Range(row, 1, row, 4).Merge().Style.Font.Bold = true;                
        //        ws.Cell(row, 5).Value = mMaDon;
        //        ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //        row++;

        //        // Table header
        //        ws.Cell(row, 1).Value = "STT";
        //        ws.Cell(row, 2).Value = "TÊN SẢN PHẨM";
        //        ws.Cell(row, 3).Value = "ĐƠN VỊ TÍNH";
        //        ws.Cell(row, 4).Value = "SỐ LƯỢNG";
        //        ws.Cell(row, 5).Value = "GHI CHÚ";

        //        ws.Range(row, 1, row, 5).Style.Font.Bold = true;
        //        ws.Range(row, 1, row, 5).Style.Fill.BackgroundColor = XLColor.LightGray;

        //        row++;

        //        int stt = 1;

        //        // Data
        //        foreach (DataRow dr in mData_dt.Rows)
        //        {
        //            ws.Cell(row, 1).Value = stt++;
        //            ws.Cell(row, 2).Value = dr["Name_VN"].ToString();
        //            ws.Cell(row, 3).Value = dr["Package"].ToString();
        //            ws.Cell(row, 4).Value = Convert.ToDecimal(dr["Quantity"]);
        //            ws.Cell(row, 5).Value = dr["Note"].ToString();
        //            row++;
        //        }

        //        row += 2;

        //        // Footer
        //        ws.Cell(row, 3).Value = $"Công ty cổ phần Việt Rau, ngày {saleDate:dd}, tháng {saleDate:MM}, năm {saleDate:yyyy}";
        //        ws.Range(row, 3, row, 5).Merge();
        //        ws.Cell(row, 3).Style.Font.FontSize = 13;
        //        row += 2;

        //        ws.Cell(row, 1).Value = "Người giao hàng";
        //        ws.Range(row, 1, row, 2).Merge();
        //        ws.Range(row, 1, row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        ws.Range(row, 1, row, 2).Style.Font.FontSize = 12;
        //        ws.Cell(row, 3).Value = "Người nhận hàng";
        //        ws.Range(row, 3, row, 5).Merge();
        //        ws.Range(row, 3, row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        ws.Range(row, 3, row, 5).Style.Font.FontSize = 12;

        //        row += 4;

        //        ws.Cell(row, 1).Value = SellerName;
        //        ws.Range(row, 1, row, 2).Merge();
        //        ws.Range(row, 1, row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        ws.Range(row, 1, row, 2).Style.Font.FontSize = 12;
        //        ws.Cell(row, 3).Value = mReceiveName;
        //        ws.Range(row, 3, row, 5).Merge();
        //        ws.Range(row, 3, row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        ws.Range(row, 3, row, 5).Style.Font.FontSize = 12;
        //        // Auto width
        //        ws.Range(4, 1, 4 + mData_dt.Rows.Count, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        ws.Range(4, 1, 4 + mData_dt.Rows.Count, 5).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //        ws.Range(4, 1, 4, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


        //        ws.Columns().AdjustToContents();

        //        using (SaveFileDialog sfd = new SaveFileDialog())
        //        {
        //            sfd.Filter = "Excel Workbook|*.xlsx";
        //            sfd.FileName = $"BangKeThuMua";
        //            if (sfd.ShowDialog() == DialogResult.OK)
        //            {
        //                wb.SaveAs(sfd.FileName);
        //                DialogResult result = MessageBox.Show("Bạn có muốn mở file này không?", "Lưu file thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //                if (result == DialogResult.Yes)
        //                {
        //                    try
        //                    {
        //                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
        //                        {
        //                            FileName = sfd.FileName,
        //                            UseShellExecute = true
        //                        });
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        MessageBox.Show("Không thể mở file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message);
        //}
    }
}
