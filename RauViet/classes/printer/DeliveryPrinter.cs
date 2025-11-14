using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using static ClosedXML.Excel.XLPredefinedFormat;

public class DeliveryPrinter
{
    private DataTable dt;
    private string exportCode;
    private System.DateTime exportDate;
    private int totalCount = 0;

    private int rowIndex = 0; // để phân trang
    private int startX = 50;
    private int lineHeight = 40;
    private int lineHeight1 = 25;

    public DeliveryPrinter(DataTable data, string exportCode, System.DateTime exportDate)
    {
        dt = data;
        this.exportCode = exportCode;
        this.exportDate = exportDate;

        foreach (DataRow row in dt.Rows)
            totalCount += Convert.ToInt32(row["count"]);
    }

    private void DrawCellText(Graphics g, string text, Font font, Rectangle rect, StringAlignment alignment = StringAlignment.Center)
    {
        StringFormat format = new StringFormat()
        {
            Alignment = alignment,
            LineAlignment = StringAlignment.Center
        };
        g.DrawString(text, font, Brushes.Black, rect, format);
    }

    public void PrintPreview(Form owner)
    {
        rowIndex = 0; // reset trước khi in
        PrintDocument pd = new PrintDocument();

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
        DialogResult dialogResult = MessageBox.Show($"Chắc chắn Chưa?", "Xác nhận in ấn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (dialogResult == DialogResult.Yes)
        {
            rowIndex = 0; // reset trước khi in
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Pd_PrintPage;
            pd.Print();
        }
        
    }

    private void Pd_PrintPage(object sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics;
        int pageWidth = e.PageBounds.Width;
        int pageHeight = e.PageBounds.Height - 50; // margin dưới

        // Cột
        int col1Width = 50, col2Width = 200, col3Width = 80, col4Width = 80, col5Width = 80;
        int col1 = startX;
        int col2 = col1 + col1Width;
        int col3 = col2 + col2Width;
        int col4 = col3 + col3Width;
        int col5 = col4 + col4Width;
        int col6 = col5 + col5Width;
        int col6Width = pageWidth - startX - col6;

        Font headerFont = new Font("Times New Roman", 14, FontStyle.Bold);
        Font normalFont = new Font("Times New Roman", 10);
        Font tableHeaderFont = new Font("Times New Roman", 10, FontStyle.Bold);

        int y = startX;

        // Header chỉ in 1 lần ở đầu trang
        if (rowIndex == 0)
        {
            string headerText = "PHIẾU GIAO HÀNG";
            SizeF headerSize = g.MeasureString(headerText, headerFont);
            g.DrawString(headerText, headerFont, Brushes.Black, (pageWidth - headerSize.Width) / 2, y);
            y += lineHeight1 * 2;

            string etdEta = $"ETD: {exportDate:dd/MM/yyyy}                                   ETA: {exportDate.AddDays(1):dd/MM/yyyy}";
            SizeF etdSize = g.MeasureString(etdEta, normalFont);
            g.DrawString(etdEta, normalFont, Brushes.Black, (pageWidth - etdSize.Width) / 2, y);
            y += lineHeight1;

            SizeF codeSize = g.MeasureString(exportCode, headerFont);
            g.DrawString(exportCode, headerFont, Brushes.Black, (pageWidth - codeSize.Width) / 2, y);
            y += lineHeight1;

            g.DrawString("Chi tiết kích thước thùng hàng", normalFont, Brushes.Black, startX, y);
            y += lineHeight1;
        }

        // Table Header
        g.DrawRectangle(Pens.Black, col1, y, col6 + col6Width - col1, lineHeight);
        g.DrawLine(Pens.Black, col2, y, col2, y + lineHeight);
        g.DrawLine(Pens.Black, col3, y, col3, y + lineHeight);
        g.DrawLine(Pens.Black, col4, y, col4, y + lineHeight);
        g.DrawLine(Pens.Black, col5, y, col5, y + lineHeight);
        g.DrawLine(Pens.Black, col6, y, col6, y + lineHeight);

        DrawCellText(g, "STT", tableHeaderFont, new Rectangle(col1, y, col2 - col1, lineHeight));
        DrawCellText(g, "Kích thước thùng xốp", tableHeaderFont, new Rectangle(col2, y, col3 - col2, lineHeight));
        DrawCellText(g, "Số lượng giao", tableHeaderFont, new Rectangle(col3, y, col4 - col3, lineHeight));
        DrawCellText(g, "Số lượng nhận", tableHeaderFont, new Rectangle(col4, y, col5 - col4, lineHeight));
        DrawCellText(g, "Ký nhận", tableHeaderFont, new Rectangle(col5, y, col6 - col5, lineHeight));
        DrawCellText(g, "Ghi chú", tableHeaderFont, new Rectangle(col6, y, col6Width, lineHeight));

        y += lineHeight;

        // Table Data với phân trang
        while (rowIndex < dt.Rows.Count)
        {
            if (y + lineHeight > pageHeight)
            {
                e.HasMorePages = true;
                return; // sang trang tiếp theo
            }

            DataRow row = dt.Rows[rowIndex];

            g.DrawRectangle(Pens.Black, col1, y, col6 + col6Width - col1, lineHeight);
            g.DrawLine(Pens.Black, col2, y, col2, y + lineHeight);
            g.DrawLine(Pens.Black, col3, y, col3, y + lineHeight);
            g.DrawLine(Pens.Black, col4, y, col4, y + lineHeight);
            g.DrawLine(Pens.Black, col5, y, col5, y + lineHeight);
            g.DrawLine(Pens.Black, col6, y, col6, y + lineHeight);

            DrawCellText(g, (rowIndex + 1).ToString(), normalFont, new Rectangle(col1, y, col2 - col1, lineHeight));
            DrawCellText(g, row["CartonSize"].ToString(), normalFont, new Rectangle(col2, y, col3 - col2, lineHeight));
            DrawCellText(g, row["count"].ToString(), normalFont, new Rectangle(col3, y, col4 - col3, lineHeight));
            DrawCellText(g, "", normalFont, new Rectangle(col4, y, col5 - col4, lineHeight));
            DrawCellText(g, "", normalFont, new Rectangle(col5, y, col6 - col5, lineHeight));
            DrawCellText(g, "", normalFont, new Rectangle(col6, y, col6Width, lineHeight));

            y += lineHeight;
            rowIndex++;
        }

        e.HasMorePages = false;

        // Tổng cộng và chữ ký (chỉ in ở trang cuối)
        y += 5;
        string totalLabel = "Tổng cộng (Count):";
        string totalValue = $"{totalCount} thùng";
        g.DrawString(totalLabel, headerFont, Brushes.Black, col1, y);
        Rectangle countCell = new Rectangle(col3, y, col4 - col3, lineHeight);
        SizeF textSize = g.MeasureString(totalValue, headerFont);
        float x = countCell.X + (countCell.Width - textSize.Width) / 2;
        float yy = countCell.Y + (countCell.Height - textSize.Height) / 2;
        g.DrawString(totalValue, headerFont, Brushes.Black, x, yy);

        y += lineHeight1 * 2;

        g.DrawString("Người giao hàng", normalFont, Brushes.Black, col2, y);
        g.DrawString("Người nhận hàng", normalFont, Brushes.Black, col5, y);
        g.DrawString("(Ký, ghi rõ họ tên)", normalFont, Brushes.Black, col2, y + lineHeight1);
        g.DrawString("(Ký, ghi rõ họ tên)", normalFont, Brushes.Black, col5, y + lineHeight1);
    }
}
