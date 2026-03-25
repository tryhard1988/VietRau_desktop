using RauViet.classes;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

public class TheoDoiSuatAn_Printer
{
    private DataTable mMealOrder_dt;
    private int rowIndex = 0; // để phân trang
    private int startX = 0;
    private int lineHeight = 24;
    private int mCountSTT;
    DateTime mStartDate, mEndDate;

    public TheoDoiSuatAn_Printer(DateTime startDate, DateTime endDate, DataTable mealOrder_dt)
    {
        DataView dv = new DataView(mealOrder_dt);
        dv.RowFilter = $"OrderDate >= #{startDate:MM/dd/yyyy}# AND OrderDate <= #{endDate:MM/dd/yyyy}#";
        dv.Sort = "OrderDate ASC";

        this.mMealOrder_dt = dv.ToTable();
        this.mStartDate = startDate;
        this.mEndDate = endDate;
        this.mCountSTT = 1;
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
        int colWidth_STT = 60, colWidth_Ngay = 110, colWidth_SA = 110, colWidth_DG = 110, colWidth_TT = 110;
        int col_STT = e.MarginBounds.Left;
        int col_Ngay = col_STT + colWidth_STT;
        int col_Note = col_Ngay + colWidth_Ngay;
        int colWidth_Note = e.MarginBounds.Right - col_Note - (colWidth_SA + colWidth_DG + colWidth_TT);
        int col_SA = col_Note + colWidth_Note;
        int col_DG = col_SA + colWidth_SA;
        int col_TT = col_DG + colWidth_DG;        

        Font titleFont = new Font("Times New Roman", 18, FontStyle.Bold);

        Font normalFont = new Font("Times New Roman", 11);
        Font tableHeaderFont = new Font("Times New Roman", 11, FontStyle.Bold);

        int y = e.MarginBounds.Top - lineHeight*2;

        // Header chỉ in 1 lần ở đầu trang
        if (rowIndex == 0)
        {
            Utils.DrawCellText(g, $"BẢNG THEO DÕI NHẬN SUẤT ĂN TRƯA T{mStartDate.ToString("MM/yyyy")}", titleFont, new Rectangle(col_STT, y, col_TT + colWidth_TT - col_STT, lineHeight * 2), StringAlignment.Center);
            y += (lineHeight + 13);
            Utils.DrawCellText(g, $"từ {mStartDate.ToString("dd/MM/yyyy")} đến {mEndDate.ToString("dd/MM/yyyy")}", normalFont, new Rectangle(col_STT, y, col_TT + colWidth_TT - col_STT, lineHeight), StringAlignment.Center);
            y += lineHeight + 15;
            Utils.DrawCellText(g, $"DỊCH VỤ NẤU ĂN LOAN", new Font("Times New Roman", 13), new Rectangle(col_STT, y, col_TT + colWidth_TT - col_STT, lineHeight * 2), StringAlignment.Near);
            y += lineHeight*2;
        }
        
        

        int tableHeaderLineHeight = Convert.ToInt32(lineHeight * 1.5);
        // Table Gray

        SolidBrush bgBrush_LightGray = new SolidBrush(Color.FromArgb(217, 217, 217));
        g.FillRectangle(bgBrush_LightGray, new Rectangle(col_STT, y, col_TT + colWidth_TT - col_STT, tableHeaderLineHeight));

        g.DrawRectangle(Pens.Gray, col_STT, y, col_TT + colWidth_TT - col_STT, tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_Ngay, y, col_Ngay, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_Note, y, col_Note, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_SA, y, col_SA, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_DG, y, col_DG, y + tableHeaderLineHeight);
        g.DrawLine(Pens.Gray, col_TT, y, col_TT, y + tableHeaderLineHeight);

        Utils.DrawCellText(g, "STT", tableHeaderFont, new Rectangle(col_STT, y, colWidth_STT, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Ngày", tableHeaderFont, new Rectangle(col_Ngay, y, colWidth_Ngay, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Diễn Giải", tableHeaderFont, new Rectangle(col_Note, y, colWidth_Note, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Suất Ăn", tableHeaderFont, new Rectangle(col_SA, y, colWidth_SA, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Đơn Giá", tableHeaderFont, new Rectangle(col_DG, y, colWidth_DG, tableHeaderLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, "Thành Tiền", tableHeaderFont, new Rectangle(col_TT, y, colWidth_TT, tableHeaderLineHeight), StringAlignment.Center);
                
        y += tableHeaderLineHeight;

        int tongSA = 0;
        int tongTien = 0;
        int TongVAT = 0;
        decimal VAT = 0;
        // Table Data với phân trang
        while (rowIndex < mMealOrder_dt.Rows.Count)
        {
            if (y + lineHeight > pageHeight)
            {
                e.HasMorePages = true;
                return; // sang trang tiếp theo
            }


            DataRow row = mMealOrder_dt.Rows[rowIndex];
            DateTime orderDate = Convert.ToDateTime(row["OrderDate"]);
            VAT = Convert.ToDecimal(row["VAT"]);
            string Note = Convert.ToString(row["Note"]);
            int quantity = Convert.ToInt32(row["Quantity"]);
            int price = Convert.ToInt32(row["Price"]);
            int thanhTien = quantity * price;

            tongSA += quantity;
            tongTien += thanhTien;
            TongVAT += Convert.ToInt32(thanhTien * (VAT / 100.0m));

            g.DrawRectangle(Pens.Gray, col_STT, y, col_TT + colWidth_TT - col_STT, lineHeight);
            g.DrawLine(Pens.Gray, col_Ngay, y, col_Ngay, y + lineHeight);
            g.DrawLine(Pens.Gray, col_Note, y, col_Note, y + lineHeight);
            g.DrawLine(Pens.Gray, col_SA, y, col_SA, y + lineHeight);
            g.DrawLine(Pens.Gray, col_DG, y, col_DG, y + lineHeight);
            g.DrawLine(Pens.Gray, col_TT, y, col_TT, y + lineHeight);

            Utils.DrawCellText(g, mCountSTT.ToString(), normalFont, new Rectangle(col_STT, y, colWidth_STT, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, orderDate.ToString("dd/MM/yyyy"), normalFont, new Rectangle(col_Ngay, y, colWidth_Ngay, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, Note, normalFont, new Rectangle(col_Note + 5, y, colWidth_Note, lineHeight), StringAlignment.Near);
            Utils.DrawCellText(g, quantity.ToString("N0"), normalFont, new Rectangle(col_SA, y, colWidth_SA, lineHeight), StringAlignment.Center);
            Utils.DrawCellText(g, price.ToString("N0"), normalFont, new Rectangle(col_DG, y, colWidth_DG - 5, lineHeight), StringAlignment.Far);
            Utils.DrawCellText(g, thanhTien.ToString("N0"), normalFont, new Rectangle(col_TT, y, colWidth_TT - 5, lineHeight), StringAlignment.Far);

            y += lineHeight;
            rowIndex++;
            mCountSTT++;
        }

        int tongLineHeight = Convert.ToInt32(lineHeight);

        g.DrawRectangle(Pens.Gray, col_STT, y, col_TT + colWidth_TT - col_STT, tongLineHeight * 3);
        g.DrawLine(Pens.Gray, col_SA, y, col_SA, y + tongLineHeight * 3);
        g.DrawLine(Pens.Gray, col_DG, y, col_DG, y + tongLineHeight * 3);
        g.DrawLine(Pens.Gray, col_TT, y, col_TT, y + tongLineHeight * 3);
        g.DrawLine(Pens.Gray, col_STT, y + tongLineHeight, col_TT + colWidth_TT, y + tongLineHeight);
        g.DrawLine(Pens.Gray, col_STT, y + tongLineHeight * 2, col_TT + colWidth_TT, y + tongLineHeight * 2);

        Utils.DrawCellText(g, $"CỘNG:", tableHeaderFont, new Rectangle(col_STT, y, col_SA - col_STT, tongLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, tongSA.ToString("N0"), tableHeaderFont, new Rectangle(col_SA, y, colWidth_SA, tongLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, tongTien.ToString("N0"), tableHeaderFont, new Rectangle(col_TT, y, colWidth_TT, tongLineHeight), StringAlignment.Far);

        y += tongLineHeight;
        Utils.DrawCellText(g, $"VAT:", tableHeaderFont, new Rectangle(col_STT, y, col_SA - col_STT, tongLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, TongVAT.ToString("N0"), tableHeaderFont, new Rectangle(col_TT, y, colWidth_TT, tongLineHeight), StringAlignment.Far);

        y += tongLineHeight;
        Utils.DrawCellText(g, $"TỔNG CỘNG:", tableHeaderFont, new Rectangle(col_STT, y, col_SA - col_STT, tongLineHeight), StringAlignment.Center);
        Utils.DrawCellText(g, (tongTien + TongVAT).ToString("N0"), tableHeaderFont, new Rectangle(col_TT, y, colWidth_TT, tongLineHeight), StringAlignment.Far);

        y += tongLineHeight * 2;

        DateTime now = DateTime.Now;
        Utils.DrawCellText(g, $"Ngày {now.Day.ToString("D2")} tháng {now.Month.ToString("D2")} năm {now.Year}", new Font("Times New Roman", 11, FontStyle.Italic), new Rectangle(col_DG, y, col_TT + colWidth_TT - col_DG, tongLineHeight), StringAlignment.Center);
        y += tongLineHeight -5;
        Utils.DrawCellText(g, $"Người Lập", new Font("Times New Roman", 11), new Rectangle(col_DG, y, col_TT + colWidth_TT - col_DG, tongLineHeight), StringAlignment.Center);

        y += tongLineHeight * 4;
        Utils.DrawCellText(g, $"{UserManager.Instance.fullName}", new Font("Times New Roman", 11), new Rectangle(col_DG, y, col_TT + colWidth_TT - col_DG, tongLineHeight), StringAlignment.Center);
        e.HasMorePages = false;

    }
}
