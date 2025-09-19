using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

public class OrderSummaryPrinter
{
    private int currentRowIndex = 0; // chỉ số dòng hiện tại
    private int yPosition = 0;
    private bool firstPage = true;
    private DataTable data;

    public void Print(DataTable dataTable, string exportCode, DateTime exportDate)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return;

        data = dataTable;
        currentRowIndex = 0;
        firstPage = true;
        yPosition = 0;

        PrintDocument printDoc = new PrintDocument();
        printDoc.PrintPage += PrintDoc_PrintPage;

        PrintPreviewDialog preview = new PrintPreviewDialog
        {
            Document = printDoc,
            Width = 1000,
            Height = 800
        };
        preview.ShowDialog();
    }

    private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
    {
        int startX = 50;
        int startY = firstPage ? 50 : yPosition;
        int rowHeight = 30;

        int colWidth_STT = 50;
        int colWidth_Product = 450;
        int colWidth_pcs = 60;
        int colWidth_nw = 170;

        int y = startY;

        Font fontTitle = new Font("Times New Roman", 20, FontStyle.Bold);
        Font fontHeader = new Font("Times New Roman", 15, FontStyle.Bold);
        Font fontContent = new Font("Times New Roman", 13);
        Brush brush = Brushes.Black;

        StringFormat sfHeader = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        StringFormat sfDataLeft = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        StringFormat sfDataCenter = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        // Vẽ tiêu đề chỉ trên trang đầu
        if (firstPage)
        {
            string title = "TỔNG HỢP ĐƠN HÀNG THEO ĐÓNG GÓI";
            SizeF titleSize = e.Graphics.MeasureString(title, fontTitle);
            float titleX = e.MarginBounds.Left + (e.MarginBounds.Width - titleSize.Width) / 2;
            e.Graphics.DrawString(title, fontTitle, brush, titleX, y);
            y += (int)titleSize.Height + 10 + rowHeight;

            e.Graphics.DrawString($"Mã đơn: {data.Rows[0]["ExportCode"]}", fontHeader, brush, startX, y);
            y += rowHeight;

            DateTime exportDate = Convert.ToDateTime(data.Rows[0]["ExportDate"]);
            e.Graphics.DrawString($"Ngày Bay (ETD): {exportDate:dd/MM/yyyy}         -        Ngày Nhận Hàng (ETA): {exportDate.AddDays(1):dd/MM/yyyy}", fontContent, brush, startX, y);
            y += rowHeight - 5;

            e.Graphics.DrawString($"Ngày Bắt Đầu SC: {exportDate.AddDays(-2):dd/MM/yyyy}        -       Ngày Hoàn Thành SC: {exportDate.AddDays(-1):dd/MM/yyyy}", fontContent, brush, startX, y);
            y += rowHeight * 2;
        }

        // Header bảng
        int x = startX;
        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_STT, rowHeight);
        e.Graphics.DrawString("STT", fontHeader, brush, new RectangleF(x, y, colWidth_STT, rowHeight), sfHeader);
        x += colWidth_STT;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Product, rowHeight);
        e.Graphics.DrawString("Tên Sản Phẩm", fontHeader, brush, new RectangleF(x, y, colWidth_Product, rowHeight), sfHeader);
        x += colWidth_Product;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_pcs, rowHeight);
        e.Graphics.DrawString("PCS", fontHeader, brush, new RectangleF(x, y, colWidth_pcs, rowHeight), sfHeader);
        x += colWidth_pcs;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_nw, rowHeight);
        e.Graphics.DrawString("Khối Lượng(Kg)", fontHeader, brush, new RectangleF(x, y, colWidth_nw, rowHeight), sfHeader);
        y += rowHeight;

        // ------------------ Dữ liệu ------------------
        int totalPCS = 0;
        decimal totalNetWeight = 0;

        for (; currentRowIndex < data.Rows.Count; currentRowIndex++)
        {
            var row = data.Rows[currentRowIndex];

            x = startX;

            // STT
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_STT, rowHeight);
            e.Graphics.DrawString((currentRowIndex + 1).ToString(), fontContent, brush, new RectangleF(x, y, colWidth_STT, rowHeight), sfHeader);
            x += colWidth_STT;

            // ProductPackingName
            string name = row["ProductPackingName"]?.ToString() ?? "";
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Product, rowHeight);
            e.Graphics.DrawString(name, fontContent, brush, new RectangleF(x, y, colWidth_Product, rowHeight), sfDataLeft);
            x += colWidth_Product;

            // TotalPCSOther
            int pcs = Convert.ToInt32(row["TotalPCSOther"] ?? 0);
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_pcs, rowHeight);
            e.Graphics.DrawString(pcs.ToString(), fontContent, brush, new RectangleF(x, y, colWidth_pcs, rowHeight), sfDataCenter);
            totalPCS += pcs;
            x += colWidth_pcs;

            // TotalNetWeight
            decimal net = Convert.ToDecimal(row["TotalNWOther"] ?? 0);
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_nw, rowHeight);
            e.Graphics.DrawString(net.ToString("N2"), fontContent, brush, new RectangleF(x, y, colWidth_nw, rowHeight), sfDataCenter);
            totalNetWeight += net;
            x += colWidth_nw;

            y += rowHeight;

            // Kiểm tra phân trang
            if (y + rowHeight > e.MarginBounds.Bottom)
            {
                yPosition = e.MarginBounds.Top;
                firstPage = false;
                e.HasMorePages = true;
                return;
            }
        }

        // Tổng cuối bảng
        x = startX;
        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_STT, rowHeight); // STT trống
        x += colWidth_STT;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Product, rowHeight);
        e.Graphics.DrawString("Tổng", fontHeader, brush, new RectangleF(x, y, colWidth_Product, rowHeight), sfDataLeft);
        x += colWidth_Product;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_pcs, rowHeight);
        e.Graphics.DrawString(totalPCS.ToString(), fontHeader, brush, new RectangleF(x, y, colWidth_pcs, rowHeight), sfDataCenter);
        x += colWidth_pcs;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_nw, rowHeight);
        e.Graphics.DrawString(totalNetWeight.ToString("N2"), fontHeader, brush, new RectangleF(x, y, colWidth_nw, rowHeight), sfDataCenter);

        e.HasMorePages = false;
        currentRowIndex = 0;
        firstPage = true;
        yPosition = 0;
    }
}
