using ClosedXML.Excel;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

public class HoaDonMuaHang_Printer
{
    private DataTable mData_dt;
    private string mMaDon, SellerName, mReceiveName;
    private int rowIndex = 0; // để phân trang
    private int startX = 0;
    private int lineHeight = 27;
    private int mCountSTT;
    private DateTime saleDate = DateTime.Now;
    public HoaDonMuaHang_Printer(string maDon, DataTable data_dt, string receiveName)
    {
        DataView dv = new DataView(data_dt);
        dv.RowFilter = $"FarmSourceCode = '{maDon}'";
        dv.Sort = "TransactionDate ASC";

        this.mData_dt = dv.ToTable();
        this.mCountSTT = 1;
        mMaDon = maDon;
        mReceiveName = receiveName;

        DataRow firstRow = mData_dt.Rows.Count > 0 ? mData_dt.Rows[0] : null;
        if (firstRow != null)
        {
            SellerName = firstRow["SellerName"].ToString();
            saleDate = Convert.ToDateTime(firstRow["TransactionDate"]);
        }
    }

    public void PrintPreview(Form owner)
    {
        rowIndex = 0; // reset trước khi in
        PrintDocument pd = new PrintDocument();
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

        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.Landscape = false;
        pd.PrintPage += Pd_PrintPage;

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
        int colWidth_STT = 60, colWidth_SP = 150, colWidth_Unit = 110, colWidth_SL = 110;
        int col_STT = e.MarginBounds.Left;
        int col_SP = col_STT + colWidth_STT;
        int col_Unit = col_SP + colWidth_SP;
        int col_SL = col_Unit + colWidth_Unit;
        int col_Note = col_SL + colWidth_SL;
        int colWidth_Note = e.MarginBounds.Right - col_Note;

        Font titleFont = new Font("Times New Roman", 18, FontStyle.Bold);

        Font normalFont = new Font("Times New Roman", 12);
        Font tableHeaderFont = new Font("Times New Roman", 11, FontStyle.Bold);

        int y = e.MarginBounds.Top - lineHeight * 2;

        // Header chỉ in 1 lần ở đầu trang
        if (rowIndex == 0)
        {
            Utils.DrawCellText(g, $"NGƯỜI BÁN: {SellerName.ToUpper()}", titleFont, new Rectangle(col_STT, y, col_Note + colWidth_Note - col_STT, lineHeight * 2), StringAlignment.Near);
            y += (lineHeight + 13);
            Utils.DrawCellText(g, $"DANH SÁCH HÀNG MUA NGOÀI", new Font("Times New Roman", 13), new Rectangle(col_STT, y, col_Note - col_STT, lineHeight * 2), StringAlignment.Near);
            Utils.DrawCellText(g, mMaDon, new Font("Times New Roman", 13), new Rectangle(col_Note, y, colWidth_Note, lineHeight * 2), StringAlignment.Far);
            y += Convert.ToInt32(lineHeight * 1.5);
        }

        int tableHeaderLineHeight = Convert.ToInt32(lineHeight * 1.5);
        // Table Gray

        SolidBrush bgBrush_LightGray = new SolidBrush(Color.FromArgb(217, 217, 217));
        g.FillRectangle(bgBrush_LightGray, new Rectangle(col_STT, y, col_Note + colWidth_Note - col_STT, tableHeaderLineHeight));

        g.DrawRectangle(Pens.Gray, col_STT, y, col_Note + colWidth_Note - col_STT, tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_SP, y, col_SP, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_Unit, y, col_Unit, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_SL, y, col_SL, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_Note, y, col_Note, y + tableHeaderLineHeight);

        Utils.DrawCellText(g, "STT", tableHeaderFont, new Rectangle(col_STT, y, colWidth_STT, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "TÊN SẢN PHẨM", tableHeaderFont, new Rectangle(col_SP, y, colWidth_SP, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "ĐƠN VỊ TÍNH", tableHeaderFont, new Rectangle(col_Unit, y, colWidth_Unit, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "SỐ LƯỢNG", tableHeaderFont, new Rectangle(col_SL, y, colWidth_SL, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "GHI CHÚ", tableHeaderFont, new Rectangle(col_Note, y, colWidth_Note, tableHeaderLineHeight), StringAlignment.Center);

        y += tableHeaderLineHeight;

        // Table Data với phân trang
        while (rowIndex < mData_dt.Rows.Count)
        {
            if (y + lineHeight > pageHeight)
            {
                e.HasMorePages = true;
                return; // sang trang tiếp theo
            }


            DataRow row = mData_dt.Rows[rowIndex];
            string spName = Convert.ToString(row["Name_VN"]);
            string unit = Convert.ToString(row["Package"]);
            string Note = Convert.ToString(row["Note"]);
            decimal quantity = Convert.ToDecimal(row["Quantity"]);


            g.DrawRectangle(Pens.Gray, col_STT, y, col_Note + colWidth_Note - col_STT, lineHeight);
            g.DrawLine(Pens.Gray, col_SP, y, col_SP, y + lineHeight);
            g.DrawLine(Pens.Gray, col_Unit, y, col_Unit, y + lineHeight);
            g.DrawLine(Pens.Gray, col_SL, y, col_SL, y + lineHeight);
            g.DrawLine(Pens.Gray, col_Note, y, col_Note, y + lineHeight);

            Utils.DrawCellText(g, mCountSTT.ToString(), normalFont, new Rectangle(col_STT, y, colWidth_STT, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, spName, normalFont, new Rectangle(col_SP, y, colWidth_SP, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, unit, normalFont, new Rectangle(col_Unit, y, colWidth_Unit, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, quantity.ToString("N1"), normalFont, new Rectangle(col_SL, y, colWidth_SL, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, Note, normalFont, new Rectangle(col_Note, y, colWidth_Note, lineHeight), StringAlignment.Center);

            y += lineHeight;
            rowIndex++;
            mCountSTT++;
        }


        DateTime now = DateTime.Now;
        Utils.DrawCellText(g, $"Công ty cổ phần Việt Rau, ngày {saleDate:dd}, tháng {saleDate:MM}, năm {saleDate:yyyy}\n\n Người nhận hàng\n\n\n\n\n\n{mReceiveName}", new Font("Times New Roman", 11, FontStyle.Italic), new Rectangle(col_Unit, y, col_Note + colWidth_Note - col_Unit, lineHeight * 8), StringAlignment.Center);
        Utils.DrawCellText(g, $"\n\n Người Giao hàng\n\n\n\n\n\n{SellerName}", new Font("Times New Roman", 11, FontStyle.Italic), new Rectangle(col_STT, y, col_Unit - col_STT, lineHeight * 8), StringAlignment.Center);
        e.HasMorePages = false;

    }



    public void ExportExcel()
    {
        try
        {
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("HoaDon");
                ws.Style.Font.FontName = "Times New Roman";
                ws.Style.Font.FontSize = 16;

                int row = 1;

                // Header
                ws.Cell(row, 1).Value = $"NGƯỜI BÁN: {SellerName}";
                ws.Range(row, 1, row, 5).Merge().Style.Font.Bold = true;
                row += 2;

                ws.Cell(row, 1).Value = "DANH SÁCH HÀNG MUA NGOÀI";
                ws.Range(row, 1, row, 4).Merge().Style.Font.Bold = true;                
                ws.Cell(row, 5).Value = mMaDon;
                ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                row++;

                // Table header
                ws.Cell(row, 1).Value = "STT";
                ws.Cell(row, 2).Value = "TÊN SẢN PHẨM";
                ws.Cell(row, 3).Value = "ĐƠN VỊ TÍNH";
                ws.Cell(row, 4).Value = "SỐ LƯỢNG";
                ws.Cell(row, 5).Value = "GHI CHÚ";

                ws.Range(row, 1, row, 5).Style.Font.Bold = true;
                ws.Range(row, 1, row, 5).Style.Fill.BackgroundColor = XLColor.LightGray;

                row++;

                int stt = 1;

                // Data
                foreach (DataRow dr in mData_dt.Rows)
                {
                    ws.Cell(row, 1).Value = stt++;
                    ws.Cell(row, 2).Value = dr["Name_VN"].ToString();
                    ws.Cell(row, 3).Value = dr["Package"].ToString();
                    ws.Cell(row, 4).Value = Convert.ToDecimal(dr["Quantity"]);
                    ws.Cell(row, 5).Value = dr["Note"].ToString();
                    row++;
                }

                row += 2;

                // Footer
                ws.Cell(row, 3).Value = $"Công ty cổ phần Việt Rau, ngày {saleDate:dd}, tháng {saleDate:MM}, năm {saleDate:yyyy}";
                ws.Range(row, 3, row, 5).Merge();
                ws.Cell(row, 3).Style.Font.FontSize = 13;
                row += 2;

                ws.Cell(row, 1).Value = "Người giao hàng";
                ws.Range(row, 1, row, 2).Merge();
                ws.Range(row, 1, row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range(row, 1, row, 2).Style.Font.FontSize = 12;
                ws.Cell(row, 3).Value = "Người nhận hàng";
                ws.Range(row, 3, row, 5).Merge();
                ws.Range(row, 3, row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range(row, 3, row, 5).Style.Font.FontSize = 12;

                row += 4;

                ws.Cell(row, 1).Value = SellerName;
                ws.Range(row, 1, row, 2).Merge();
                ws.Range(row, 1, row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range(row, 1, row, 2).Style.Font.FontSize = 12;
                ws.Cell(row, 3).Value = mReceiveName;
                ws.Range(row, 3, row, 5).Merge();
                ws.Range(row, 3, row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range(row, 3, row, 5).Style.Font.FontSize = 12;
                // Auto width
                ws.Range(4, 1, 4 + mData_dt.Rows.Count, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range(4, 1, 4 + mData_dt.Rows.Count, 5).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                ws.Range(4, 1, 4, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                ws.Columns().AdjustToContents();

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel Workbook|*.xlsx";
                    sfd.FileName = $"BangKeThuMua";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        wb.SaveAs(sfd.FileName);
                        DialogResult result = MessageBox.Show("Bạn có muốn mở file này không?", "Lưu file thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                                {
                                    FileName = sfd.FileName,
                                    UseShellExecute = true
                                });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Không thể mở file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message);
        }
    }
}
