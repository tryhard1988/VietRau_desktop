using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

public class OrderListPrinter
{
    private int yPosition = 0;
    private bool firstPage = true;
    private int currentRowIndex = 0; // chỉ số dòng hiện tại
    private List<DataGridViewRow> printRows;

    // ------------------- In trực tiếp -------------------
    public void PrintDirect(DataGridView dgv, string exportCode, DateTime exportDate, string staff)
    {
        if (dgv.Rows.Count == 0) return;

        DialogResult dialogResult = MessageBox.Show($"Chắc chắn Chưa?", "Xác nhận in ấn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (dialogResult == DialogResult.Yes)
        {
            SetupPrint(dgv);

            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += (s, e) =>
            {
                DrawPrintPage(e, exportCode, exportDate, staff);
            };
            printDoc.Print();
        } 
    }

    // ------------------- Xem trước -------------------
    public void PrintPreview(DataGridView dgv, string exportCode, DateTime exportDate, string staff)
    {
        if (dgv.Rows.Count == 0) return;

        SetupPrint(dgv);

        PrintDocument printDoc = new PrintDocument();
        printDoc.PrintPage += (s, e) =>
        {
            DrawPrintPage(e, exportCode, exportDate, staff);
        };

        PrintPreviewDialog preview = new PrintPreviewDialog
        {
            Document = printDoc,
            Width = 1000,
            Height = 800
        };
        preview.ShowDialog();
    }

    // ------------------- Khởi tạo chung -------------------
    private void SetupPrint(DataGridView dgv)
    {
        printRows = dgv.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow).ToList();
        currentRowIndex = 0;
        firstPage = true;
        yPosition = 0;
    }

    // ------------------- Vẽ trang -------------------
    private void DrawPrintPage(PrintPageEventArgs e, string exportCode, DateTime exportDate, string staff)
    {
        Graphics g = e.Graphics;
        int left = e.MarginBounds.Left / 3;
        int rowHeight = 25;
        int pageWidth = e.MarginBounds.Width + left * 4 - 10;
        int pageHeight = e.MarginBounds.Bottom;

        Font fontTitle = new Font("Times New Roman", 16, FontStyle.Bold);
        Font fontHeader = new Font("Times New Roman", 12, FontStyle.Bold);
        Font fontNormal = new Font("Times New Roman", 12);

        StringFormat center = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        StringFormat leftAlign = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

        int y = firstPage ? e.MarginBounds.Top : yPosition;

        // --- HEADER trang đầu ---
        if (firstPage)
        {
            g.DrawString("CÔNG TY CỔ PHẦN VIỆT RAU", fontNormal, Brushes.Black, left, y);
            g.DrawString(DateTime.Now.ToString("dd/MM/yyyy"), fontNormal, Brushes.Black, left + pageWidth - 120, y);
            y += 40;

            string title = "BẢNG CHIA SỔ LƯỢNG THÀNH PHẨM ĐÓNG THÙNG";
            SizeF titleSize = g.MeasureString(title, fontTitle);
            float titleX = left + (pageWidth - titleSize.Width) / 2;
            g.DrawString(title, fontTitle, Brushes.Black, titleX, y);
            y += (int)titleSize.Height + 20;

            g.DrawString($"ExportCode: {exportCode}", fontNormal, Brushes.Black, left, y);
            g.DrawString($"ExportDate: {exportDate:dd/MM/yyyy}", fontNormal, Brushes.Black, left + pageWidth - 200, y);
            y += rowHeight;
            g.DrawString($"Nhân viên phụ trách đóng gói: {staff}", fontNormal, Brushes.Black, left, y);
            y += rowHeight + 10;
        }

        int colSTT = 35, colCustomer = 150, colPCS = 40, colNW = 45, colActualPCS = 40, colActualNW = 45, colPackingPCS = 300,
            colPackingNote = pageWidth - colSTT - colCustomer - colPCS - colNW - colActualPCS - colActualNW - colPackingPCS;

        // Vẽ header bảng
        DrawTableHeader(g, left, ref y, rowHeight, pageWidth, fontHeader);

        string currentProduct = null;
        int stt = 1;

        // --- DỮ LIỆU ---
        for (; currentRowIndex < printRows.Count; currentRowIndex++)
        {
            var row = printRows[currentRowIndex];
            string product = row.Cells["ProductNameVN"].Value?.ToString() ?? "";
            string customer = row.Cells["CustomerName"].Value?.ToString() ?? "";
            string pcs = row.Cells["PCSOther"].Value?.ToString() ?? "";
            string nw = row.Cells["NWOther"].Value?.ToString() ?? "";

            if (product != currentProduct)
            {
                currentProduct = product;
                g.FillRectangle(Brushes.LightGray, left, y, pageWidth, rowHeight);
                g.DrawRectangle(Pens.Black, left, y, pageWidth, rowHeight);
                g.DrawString(currentProduct, fontHeader, Brushes.Black, new RectangleF(left, y, pageWidth, rowHeight), leftAlign);
                y += rowHeight;
                stt = 1;
            }

            int x = left;
            g.DrawRectangle(Pens.Black, x, y, colSTT, rowHeight);
            g.DrawString(stt.ToString(), fontNormal, Brushes.Black, new RectangleF(x, y, colSTT, rowHeight), center);
            x += colSTT;

            g.DrawRectangle(Pens.Black, x, y, colCustomer, rowHeight);
            g.DrawString(customer, fontNormal, Brushes.Black, new RectangleF(x, y, colCustomer, rowHeight), leftAlign);
            x += colCustomer;

            g.DrawRectangle(Pens.Black, x, y, colPCS, rowHeight);
            g.DrawString(pcs, fontNormal, Brushes.Black, new RectangleF(x, y, colPCS, rowHeight), center);
            x += colPCS;

            g.DrawRectangle(Pens.Black, x, y, colNW, rowHeight);
            g.DrawString(nw, fontNormal, Brushes.Black, new RectangleF(x, y, colNW, rowHeight), center);
            x += colNW;

            g.DrawRectangle(Pens.Black, x, y, colActualPCS, rowHeight); x += colActualPCS;
            g.DrawRectangle(Pens.Black, x, y, colActualNW, rowHeight); x += colActualNW;

            int smallBox = colPackingPCS / 10;
            for (int b = 0; b < 10; b++)
            {
                g.DrawRectangle(Pens.Black, x + b * smallBox, y, smallBox, rowHeight);
            }
            x += colPackingPCS;

            g.DrawRectangle(Pens.Black, x, y, colPackingNote, rowHeight);
            x += colPackingNote;

            y += rowHeight;
            stt++;

            if (y + rowHeight > pageHeight)
            {
                yPosition = e.MarginBounds.Top;
                firstPage = false;
                e.HasMorePages = true;
                return;
            }
        }

        e.HasMorePages = false;
        yPosition = 0;
        firstPage = true;
        currentRowIndex = 0;
    }

    // ------------------- Vẽ header bảng -------------------
    private void DrawTableHeader(Graphics g, int left, ref int y, int rowHeight, int pageWidth, Font fontHeader)
    {
        int colSTT = 35, colCustomer = 150, colPCS = 40, colNW = 42, colActualPCS = 40, colActualNW = 42, colPackingPCS = 300,
            colPackingNote = pageWidth - colSTT - colCustomer - colPCS - colNW - colActualPCS - colActualNW - colPackingPCS;
        int x = left;

        g.DrawRectangle(Pens.Black, x, y, colSTT, rowHeight * 2);
        g.DrawString("STT", fontHeader, Brushes.Black, new RectangleF(x, y, colSTT, rowHeight * 2),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colSTT;

        g.DrawRectangle(Pens.Black, x, y, colCustomer, rowHeight * 2);
        g.DrawString("Khách hàng", fontHeader, Brushes.Black, new RectangleF(x, y, colCustomer, rowHeight * 2),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colCustomer;

        g.DrawRectangle(Pens.Black, x, y, colPCS + colNW, rowHeight);
        g.DrawString("Order", fontHeader, Brushes.Black, new RectangleF(x, y, colPCS + colNW, rowHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

        g.DrawRectangle(Pens.Black, x, y + rowHeight, colPCS, rowHeight);
        g.DrawString("PCS", fontHeader, Brushes.Black, new RectangleF(x, y + rowHeight, colPCS, rowHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colPCS;

        g.DrawRectangle(Pens.Black, x, y + rowHeight, colNW, rowHeight);
        g.DrawString("NW", fontHeader, Brushes.Black, new RectangleF(x, y + rowHeight, colNW, rowHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colNW;

        g.DrawRectangle(Pens.Black, x, y, colActualPCS + colActualNW, rowHeight);
        g.DrawString("SL thực tế", fontHeader, Brushes.Black, new RectangleF(x, y, colActualPCS + colActualNW, rowHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

        g.DrawRectangle(Pens.Black, x, y + rowHeight, colActualPCS, rowHeight);
        g.DrawString("PCS", fontHeader, Brushes.Black, new RectangleF(x, y + rowHeight, colActualPCS, rowHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colActualPCS;

        g.DrawRectangle(Pens.Black, x, y + rowHeight, colActualNW, rowHeight);
        g.DrawString("NW", fontHeader, Brushes.Black, new RectangleF(x, y + rowHeight, colActualNW, rowHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colActualNW;

        g.DrawRectangle(Pens.Black, x, y, colPackingPCS + colPackingNote, rowHeight);
        g.DrawString("Đóng Thùng", fontHeader, Brushes.Black, new RectangleF(x, y, colPackingPCS + colPackingNote, rowHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

        g.DrawRectangle(Pens.Black, x, y + rowHeight, colPackingPCS, rowHeight);
        g.DrawString("PCS", fontHeader, Brushes.Black, new RectangleF(x, y + rowHeight, colPackingPCS, rowHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colPackingPCS;

        g.DrawRectangle(Pens.Black, x, y + rowHeight, colPackingNote, rowHeight);
        g.DrawString("Ghi chú", fontHeader, Brushes.Black, new RectangleF(x, y + rowHeight, colPackingNote, rowHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

        y += rowHeight * 2;
    }
}
