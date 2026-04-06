using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public class DeNghiThanhToan_Printer
{
    private DataRow mSellerRow;
    private int lineHeight = 25;
    private int mCountSTT;
    private int mSoTien = 0;
    private decimal mSoLuong = 0;
    private DateTime mNgayMua;
    private string mNoiDung, mNote, mNguoiDeNghi;
    int mSellerID;
    public DeNghiThanhToan_Printer(string nguoiDeNghi, int supplierID, DateTime ngayMua, int soLuong, string noidung,  int soTien, string ghiChu)
    {
        this.mCountSTT = 1;
        mNgayMua = ngayMua;
        mSoTien = soTien;
        mSoLuong = soLuong;
        mNote = ghiChu;
        mNoiDung = noidung;
        mNguoiDeNghi = nguoiDeNghi;
        mSellerID = supplierID;
    }

    public async Task loadData()
    {
        var seller_dt = await SQLStore_Kho.Instance.GetSupplierAsync();
        mSellerRow = seller_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("SupplierID") == mSellerID);
    }

    public void PrintPreview(Form owner)
    {
        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.Margins = new Margins(25, 50, 25, 25);
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
        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.Margins = new Margins(25, 50, 25, 25);
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
        //int colWidth_1 = 35, colWidth_2 = 150, colWidth_3 = 40, colWidth_4 = 45, colWidth_5 = 50, colWidth_6 = 70, colWidth_7 = 70, colWidth_8 = 110;
        //int col_1 = e.MarginBounds.Left;
        //int col_2 = col_1 + colWidth_1;
        //int col_3 = col_2 + colWidth_2;
        //int col_4 = col_3 + colWidth_3;
        //int col_5 = col_4 + colWidth_4;
        //int col_6 = col_5 + colWidth_5;
        //int col_7 = col_6 + colWidth_6;
        //int col_8 = col_7 + colWidth_7;
        //int col_9 = col_8 + colWidth_8;
        //int colWidth_9 = e.MarginBounds.Right - col_9;

        //Font titleFont = new Font("Times New Roman", 14, FontStyle.Bold);

        //Font normalFont = new Font("Times New Roman", 9);
        //Font tableHeaderFont = new Font("Times New Roman", 9, FontStyle.Bold);

        //int y = e.MarginBounds.Top;

        //SolidBrush bgBrush_LightGray = new SolidBrush(Color.FromArgb(217, 217, 217));
        //// Header chỉ in 1 lần ở đầu trang
        //if (groupIndex == 0)
        //{
        //    Utils.DrawCellText(g, $"Công Ty Cổ Phần Việt Rau", normalFont, new Rectangle(col_1, y, col_6 + colWidth_6 - col_1, lineHeight), StringAlignment.Near);
        //    y += lineHeight;
        //    Utils.DrawCellText(g, $"BẢNG KÊ HÀNG MUA NGOÀI {mDate.ToString("dd/MM/yyyy")} - {mCode}", titleFont, new Rectangle(col_1, y, col_9 + colWidth_9 - col_1, lineHeight * 2), StringAlignment.Center);
        //    y += (lineHeight * 2);
            
            
        //}

        //g.FillRectangle(bgBrush_LightGray, new Rectangle(col_1, y, col_9 + colWidth_9 - col_1, lineHeight * 2));

        //g.DrawRectangle(Pens.Gray, col_1, y, col_9 + colWidth_9 - col_1, lineHeight * 2);
        //g.DrawLine(Pens.Gray, col_2, y, col_2, y + lineHeight * 2);
        //g.DrawLine(Pens.Gray, col_3, y, col_3, y + lineHeight * 2);
        //g.DrawLine(Pens.Gray, col_4, y, col_4, y + lineHeight * 2);
        //g.DrawLine(Pens.Gray, col_5, y, col_5, y + lineHeight * 2);
        //g.DrawLine(Pens.Gray, col_6, y, col_6, y + lineHeight * 2);
        //g.DrawLine(Pens.Gray, col_7, y, col_7, y + lineHeight * 2);
        //g.DrawLine(Pens.Gray, col_8, y, col_8, y + lineHeight * 2);
        //g.DrawLine(Pens.Gray, col_9, y, col_9, y + lineHeight * 2);

        //Utils.DrawCellText(g, "STT", tableHeaderFont, new Rectangle(col_1, y, colWidth_1, lineHeight * 2), StringAlignment.Center);
        //Utils.DrawCellText(g, "Sản Phẩm", tableHeaderFont, new Rectangle(col_2, y, colWidth_2, lineHeight * 2), StringAlignment.Center);
        //Utils.DrawCellText(g, "ĐVT", tableHeaderFont, new Rectangle(col_3, y, colWidth_3, lineHeight * 2), StringAlignment.Center);
        //Utils.DrawCellText(g, "Số Lượng", tableHeaderFont, new Rectangle(col_4, y, colWidth_4, lineHeight * 2), StringAlignment.Center);
        //Utils.DrawCellText(g, "Đơn Giá", tableHeaderFont, new Rectangle(col_5, y, colWidth_5, lineHeight * 2), StringAlignment.Center);
        //Utils.DrawCellText(g, "Thành Tiền", tableHeaderFont, new Rectangle(col_6, y, colWidth_6, lineHeight * 2), StringAlignment.Center);
        //Utils.DrawCellText(g, "Tổng", tableHeaderFont, new Rectangle(col_7, y, colWidth_7, lineHeight * 2), StringAlignment.Center);
        //Utils.DrawCellText(g, "TT CK", tableHeaderFont, new Rectangle(col_8, y, colWidth_8, lineHeight * 2), StringAlignment.Center);
        //Utils.DrawCellText(g, "ghi chú", tableHeaderFont, new Rectangle(col_9, y, colWidth_9, lineHeight * 2), StringAlignment.Center);
        //y += lineHeight*2;


        //while (groupIndex < mDataGroup.Count)
        //{
        //    var kv = mDataGroup[groupIndex];
        //    int sellerID = kv.Key;
        //    DataTable data = kv.Value;
        //    int count = data.Rows.Count;

        //    if (y + lineHeight * count > pageHeight)
        //    {
        //        e.HasMorePages = true;
        //        return; // sang trang tiếp theo
        //    }

        //    g.DrawRectangle(Pens.Gray, col_1, y, col_9 + colWidth_9 - col_1, lineHeight * count);
        //    g.DrawLine(Pens.Gray, col_2, y, col_2, y + lineHeight * count);
        //    g.DrawLine(Pens.Gray, col_3, y, col_3, y + lineHeight * count);
        //    g.DrawLine(Pens.Gray, col_4, y, col_4, y + lineHeight * count);
        //    g.DrawLine(Pens.Gray, col_5, y, col_5, y + lineHeight * count);
        //    g.DrawLine(Pens.Gray, col_6, y, col_6, y + lineHeight * count);
        //    g.DrawLine(Pens.Gray, col_7, y, col_7, y + lineHeight * count);
        //    g.DrawLine(Pens.Gray, col_8, y, col_8, y + lineHeight * count);
        //    g.DrawLine(Pens.Gray, col_9, y, col_9, y + lineHeight * count);

        //    decimal tongtien_seller = 0;
        //    int y_seller = y;
        //    while (rowIndex < data.Rows.Count)
        //    {      
        //        DataRow row = data.Rows[rowIndex];
        //        string spName = Convert.ToString(row["Name_VN"]);
        //        string note = Convert.ToString(row["Note"]);
        //        string unit = Convert.ToString(row["unit"]);
        //        DateTime transactionDate = Convert.ToDateTime(row["TransactionDate"]);
        //        decimal quantity = Convert.ToDecimal(row["Quantity"]);
        //        int price = Convert.ToInt32(row["Price"]);

        //        var lines = note?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

        //        // lấy dòng rename
        //        string renameLine = lines
        //            .FirstOrDefault(l => Utils.RemoveVietnameseSigns(l).ToLower().Contains("rename"));

        //        // lấy giá trị rename
        //        string rename = renameLine?
        //            .Split(':')
        //            .Skip(1)
        //            .FirstOrDefault()
        //            ?.Trim() ?? "";

        //        // phần ghi chú = bỏ dòng rename
        //        string ghichu = string.Join(Environment.NewLine,
        //            lines.Where(l => l != renameLine));

        //        if (string.IsNullOrEmpty(rename) == false)
        //            spName = rename;

        //        if (rowIndex > 0)
        //        {
        //            g.DrawLine(Pens.Gray, col_1, y, col_7, y);
        //            g.DrawLine(Pens.Gray, col_9, y, col_9 + colWidth_9, y);
        //        }

        //        int thanhtien = Convert.ToInt32(price * quantity);
        //        total_thanhtien += thanhtien;
        //        tongtien_seller += thanhtien;
        //        total_soLuong += quantity;
        //        Utils.DrawCellText(g, mCountSTT.ToString(), normalFont, new Rectangle(col_1, y, colWidth_1, lineHeight), StringAlignment.Center);
        //        Utils.DrawCellText(g, spName, normalFont, new Rectangle(col_2, y, colWidth_2, lineHeight), StringAlignment.Near);
        //        Utils.DrawCellText(g, unit, normalFont, new Rectangle(col_3, y, colWidth_3, lineHeight), StringAlignment.Center);
        //        Utils.DrawCellText(g, quantity.ToString("N1"), normalFont, new Rectangle(col_4, y, colWidth_4, lineHeight), StringAlignment.Far);
        //        Utils.DrawCellText(g, price.ToString("N0"), normalFont, new Rectangle(col_5, y, colWidth_5, lineHeight), StringAlignment.Far);
        //        Utils.DrawCellText(g, thanhtien.ToString("N0"), normalFont, new Rectangle(col_6, y, colWidth_6, lineHeight), StringAlignment.Far);                
        //        Utils.DrawCellText(g, ghichu, normalFont, new Rectangle(col_9, y, colWidth_9, lineHeight), StringAlignment.Center);
        //        y += lineHeight;
        //        rowIndex++;
        //        mCountSTT++;
        //    }

        //    DataRow[] rows = mSeller_dt.Select($"SupplierID = {sellerID}");
        //    string ttck = "";
        //    if (rows.Length > 0)
        //    {
        //        DataRow row = rows[0];
        //        string supplierName = row["SupplierName"].ToString();
        //        supplierName = supplierName.Split('-')[0].Trim();
        //        ttck = $"{row["BankAccount"]}\n{row["BankName"]} {supplierName}";
        //    }

        //    Utils.DrawCellText(g, tongtien_seller.ToString("N0"), normalFont, new Rectangle(col_7, y_seller, colWidth_7, lineHeight * data.Rows.Count), StringAlignment.Center);
        //    Utils.DrawCellText(g, ttck, normalFont, new Rectangle(col_8, y_seller, colWidth_8, lineHeight * data.Rows.Count), StringAlignment.Center);

        //    groupIndex++;
        //    rowIndex = 0;
        //}

        //if (y + lineHeight > pageHeight)
        //{
        //    e.HasMorePages = true;
        //    return; // sang trang tiếp theo
        //}

        //g.DrawRectangle(Pens.Gray, col_1, y, col_9 + colWidth_9 - col_1, lineHeight);
        //g.DrawLine(Pens.Gray, col_4, y, col_4, y + lineHeight);
        //g.DrawLine(Pens.Gray, col_5, y, col_5, y + lineHeight);
        //g.DrawLine(Pens.Gray, col_6, y, col_6, y + lineHeight);
        //g.DrawLine(Pens.Gray, col_7, y, col_7, y + lineHeight);
        //g.DrawLine(Pens.Gray, col_8, y, col_8, y + lineHeight);
        //g.DrawLine(Pens.Gray, col_9, y, col_9, y + lineHeight);
        //Utils.DrawCellText(g, $"CỘNG TIỀN MUA HÀNG", tableHeaderFont, new Rectangle(col_1, y, col_4 - col_1, lineHeight), StringAlignment.Center);
        //Utils.DrawCellText(g, total_soLuong.ToString("N1"), tableHeaderFont, new Rectangle(col_4, y, colWidth_4, lineHeight), StringAlignment.Far);
        //Utils.DrawCellText(g, total_thanhtien.ToString("N0"), tableHeaderFont, new Rectangle(col_6, y, colWidth_6, lineHeight), StringAlignment.Far);
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
