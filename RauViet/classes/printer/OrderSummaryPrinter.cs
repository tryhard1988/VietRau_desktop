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
    private string exportCode;
    private DateTime exportDate;
    int totalPCS = 0;
    decimal totalNetWeight = 0;
    // ------------------- Hàm Print Preview -------------------
    public void PrintPreview(DataTable dataTable, string exportCode, DateTime exportDate, Form owner)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return;

        SetupPrint(dataTable, exportCode, exportDate);

        PrintDocument printDoc = new PrintDocument();
        printDoc.PrintPage += PrintDoc_PrintPage;

        PrintPreviewDialog preview = new PrintPreviewDialog
        {
            Document = printDoc,
            Width = 1000,
            Height = 800
        };
        preview.ShowDialog(owner);
    }

    // ------------------- Hàm Print trực tiếp -------------------
    public void PrintDirect(DataTable dataTable, string exportCode, DateTime exportDate)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return;

        DialogResult dialogResult = MessageBox.Show($"Chắc chắn Chưa?", "Xác nhận in ấn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (dialogResult == DialogResult.Yes)
        {
            SetupPrint(dataTable, exportCode, exportDate);

            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += PrintDoc_PrintPage;
            printDoc.Print(); // in trực tiếp ra máy in mặc định
        } 
    }

    // ------------------- Setup dữ liệu chung -------------------
    private void SetupPrint(DataTable dataTable, string exportCode, DateTime exportDate)
    {
        this.data = dataTable;
        this.exportCode = exportCode;
        this.exportDate = exportDate;
        currentRowIndex = 0;
        firstPage = true;
        yPosition = 0;
        totalPCS = 0;
        totalNetWeight = 0;
    }

    // ------------------- Hàm PrintPage -------------------
    private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
    {
        int startX = 50;
        int startY = firstPage ? 50 : yPosition;
        int rowHeight = 30;

        int colWidth_STT = 50;
        int colWidth_Product = 230;
        int colWidth_pcs = 60;
        int colWidth_nw = 170;
        int colWidth_gc = 220;

        int y = startY;

        Font fontTitle = new Font("Times New Roman", 20, FontStyle.Bold);
        Font fontHeader = new Font("Times New Roman", 15, FontStyle.Bold);
        Font fontContent = new Font("Times New Roman", 13);
        Brush brush = Brushes.Black;

        StringFormat sfHeader = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        StringFormat sfDataLeft = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        StringFormat sfDataCenter = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        // Tiêu đề trang đầu
        if (firstPage)
        {
            string title = "TỔNG HỢP ĐƠN HÀNG THEO ĐÓNG GÓI";
            SizeF titleSize = e.Graphics.MeasureString(title, fontTitle);
            float titleX = e.MarginBounds.Left + (e.MarginBounds.Width - titleSize.Width) / 2;
            e.Graphics.DrawString(title, fontTitle, brush, titleX, y);
            y += (int)titleSize.Height + 10 + rowHeight;

            e.Graphics.DrawString($"Mã đơn: {exportCode}", fontHeader, brush, startX, y);
            y += rowHeight;

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
        x += colWidth_nw;

        e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_gc, rowHeight);
        e.Graphics.DrawString("Ghi chú", fontHeader, brush, new RectangleF(x, y, colWidth_gc, rowHeight), sfHeader);
        y += rowHeight;
        
        // ------------------ Dữ liệu ------------------
        

        for (; currentRowIndex < data.Rows.Count; currentRowIndex++)
        {
            var row = data.Rows[currentRowIndex];

            x = startX;

            

            // ProductPackingName
            string name = row["ProductPackingName"]?.ToString() ?? "";
            RectangleF rect = new RectangleF(x, y, colWidth_Product, 9999);
            SizeF textSize = e.Graphics.MeasureString(name,fontContent,(int)colWidth_Product, StringFormat.GenericDefault);
            int dynamicHeight = Math.Max(rowHeight, (int)textSize.Height);

            // STT
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_STT, dynamicHeight);
            e.Graphics.DrawString((currentRowIndex + 1).ToString(), fontContent, brush, new RectangleF(x, y, colWidth_STT, rowHeight), sfHeader);
            x += colWidth_STT;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_Product, dynamicHeight);
            e.Graphics.DrawString(name, fontContent, brush, new RectangleF(x, y, colWidth_Product, dynamicHeight), sfDataLeft);
            x += colWidth_Product;

            // TotalPCSOther
            int pcs = Convert.ToInt32(row["TotalPCSOther"] ?? 0);
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_pcs, dynamicHeight);
            e.Graphics.DrawString(pcs.ToString(), fontContent, brush, new RectangleF(x, y, colWidth_pcs, rowHeight), sfDataCenter);
            totalPCS += pcs;
            x += colWidth_pcs;

            // TotalNetWeight
            decimal net = Convert.ToDecimal(row["TotalNWOther"] ?? 0);
            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_nw, dynamicHeight);
            e.Graphics.DrawString(net.ToString("N2"), fontContent, brush, new RectangleF(x, y, colWidth_nw, rowHeight), sfDataCenter);
            totalNetWeight += net;
            x += colWidth_nw;

            e.Graphics.DrawRectangle(Pens.Black, x, y, colWidth_gc, dynamicHeight);

            y += dynamicHeight;

            // Kiểm tra phân trang
            if (y + rowHeight > e.MarginBounds.Bottom)
            {
                currentRowIndex++;
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
