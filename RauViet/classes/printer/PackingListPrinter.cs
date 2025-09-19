using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

public class PackingListPrinter
{
    private List<List<DataGridViewRow>> _groupedOrders; // Gom nhóm theo CustomerCarton
    private int _currentGroupIndex; // Nhóm hiện tại
    private int _currentRowIndex;   // Dòng hiện tại trong nhóm
    private int _yPosition;
    private bool _firstPage;

    private DataGridView _dataGridView;
    private string _customerName;
    private string _customerAddress;
    private PrintDocument _printDoc;

    private List<string> _selectedCartons; // Danh sách CustomerCarton cần in

    public PackingListPrinter(DataGridView dgv, string customerName, string customerAddress, List<string> selectedCartons)
    {
        _dataGridView = dgv;
        _customerName = customerName;
        _customerAddress = customerAddress;
        _selectedCartons = selectedCartons ?? new List<string>();

        _printDoc = new PrintDocument();
        _printDoc.PrintPage += PrintDoc_PrintPage;
    }

    public void PrintPreview()
    {
        PreparePrintRows();
        ResetPrintState();

        using (PrintPreviewDialog preview = new PrintPreviewDialog())
        {
            preview.Document = _printDoc;
            preview.Width = 1123;
            preview.Height = 794;
            preview.ShowDialog();
        }
    }

    public void Print(int number)
    {
        DialogResult dialogResult = MessageBox.Show($"Tổng cộng: {number} đơn\nBạn có chắc chắn muốn in không?","Xác nhận in ấn",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

        if (dialogResult == DialogResult.Yes)
        {
            PreparePrintRows();
            ResetPrintState();
            _printDoc.Print();
        }
    }

    /// <summary>
    /// Chuẩn bị dữ liệu in: gom nhóm theo CustomerCarton nhưng chỉ lấy những carton nằm trong danh sách chọn.
    /// </summary>
    private void PreparePrintRows()
    {
        var rows = _dataGridView.Rows.Cast<DataGridViewRow>()
            .Where(r => !r.IsNewRow)
            .ToList();

        // Lọc theo danh sách carton được chọn
        if (_selectedCartons != null && _selectedCartons.Count > 0)
        {
            rows = rows
                .Where(r =>
                {
                    string carton = r.Cells["CustomerCarton"].Value?.ToString();
                    return !string.IsNullOrEmpty(carton) && _selectedCartons.Contains(carton);
                })
                .ToList();
        }

        // Gom nhóm theo CustomerCarton
        _groupedOrders = rows
            .Where(r => r.Cells["CustomerCarton"].Value != null && !string.IsNullOrEmpty(r.Cells["CustomerCarton"].Value.ToString()))
            .GroupBy(r => r.Cells["CustomerCarton"].Value.ToString())
            .Select(g => g.ToList())
            .ToList();
    }

    private void ResetPrintState()
    {
        _currentGroupIndex = 0;
        _currentRowIndex = 0;
        _yPosition = 50;
        _firstPage = true;
    }

    private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
    {
        if (_groupedOrders == null || _groupedOrders.Count == 0 || _currentGroupIndex >= _groupedOrders.Count)
        {
            e.HasMorePages = false;
            return;
        }

        var currentGroup = _groupedOrders[_currentGroupIndex];
        Graphics g = e.Graphics;

        int startX = 20;
        int lineHeight = 25;
        int pageWidth = e.PageBounds.Width - 60;
        int pageHeight = e.MarginBounds.Bottom;

        Font fontHeader = new Font("Arial", 12, FontStyle.Bold);
        Font fontRegular = new Font("Arial", 10);
        Font fontLarge = new Font("Arial", 32, FontStyle.Bold);
        Brush brush = Brushes.Black;

        int y = _yPosition;

        // ================= HEADER =================
        if (_firstPage)
        {
            int leftWidth = pageWidth / 3;
            Rectangle leftRect = new Rectangle(startX, y, leftWidth, lineHeight * 7);
            g.DrawRectangle(Pens.Black, leftRect);

            g.DrawString("Consignee:", fontHeader, brush,
                new RectangleF(leftRect.X + 5, leftRect.Y + 2, leftRect.Width - 10, lineHeight));
            g.DrawString(_customerName, fontRegular, brush,
                new RectangleF(leftRect.X + 5, leftRect.Y + lineHeight, leftRect.Width - 10, lineHeight));
            g.DrawString(_customerAddress, fontRegular, brush,
                new RectangleF(leftRect.X + 5, leftRect.Y + lineHeight * 2, leftRect.Width - 10, leftRect.Height - lineHeight * 2));

            int rightX = startX + leftWidth;
            int rightWidth = pageWidth - leftWidth;

            int cartHeight = lineHeight * 4;
            string customerCarton = currentGroup[0].Cells["CustomerCarton"].Value?.ToString() ?? "";

            Rectangle rectCarton = new Rectangle(rightX, y, rightWidth, cartHeight);
            g.DrawRectangle(Pens.Black, rectCarton);
            g.DrawString(customerCarton, fontLarge, brush,
                new RectangleF(rectCarton.X, rectCarton.Y + 5, rectCarton.Width, rectCarton.Height),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            int halfWidth = rightWidth / 3;

            Rectangle rectCartonNo = new Rectangle(rightX, y + cartHeight, rightWidth, lineHeight);
            g.DrawRectangle(Pens.Black, rectCartonNo);
            Rectangle rectLabelCartonNo = new Rectangle(rectCartonNo.X, rectCartonNo.Y, halfWidth, rectCartonNo.Height);
            Rectangle rectValueCartonNo = new Rectangle(rectCartonNo.X + halfWidth, rectCartonNo.Y, halfWidth * 2, rectCartonNo.Height);

            string cartonNo = currentGroup[0].Cells["CartonNo"].Value?.ToString() ?? "";
            g.DrawRectangle(Pens.Black, rectLabelCartonNo);
            g.DrawRectangle(Pens.Black, rectValueCartonNo);
            g.DrawString("Carton No:", fontRegular, brush, rectLabelCartonNo);
            g.DrawString(cartonNo, fontRegular, brush, rectValueCartonNo);

            Rectangle rectExportDate = new Rectangle(rightX, y + cartHeight + lineHeight, rightWidth, lineHeight);
            g.DrawRectangle(Pens.Black, rectExportDate);
            Rectangle rectLabelExportDate = new Rectangle(rectExportDate.X, rectExportDate.Y, halfWidth, rectExportDate.Height);
            Rectangle rectValueExportDate = new Rectangle(rectExportDate.X + halfWidth, rectExportDate.Y, halfWidth * 2, rectExportDate.Height);

            string exportDate = currentGroup[0].Cells["ExportDate"].Value?.ToString() ?? "";
            g.DrawRectangle(Pens.Black, rectLabelExportDate);
            g.DrawRectangle(Pens.Black, rectValueExportDate);
            g.DrawString("Export Date:", fontRegular, brush, rectLabelExportDate);
            g.DrawString(exportDate, fontRegular, brush, rectValueExportDate);

            Rectangle rectCartonSize = new Rectangle(rightX, y + cartHeight + lineHeight * 2, rightWidth, lineHeight);
            g.DrawRectangle(Pens.Black, rectCartonSize);
            Rectangle rectLabelCartonSize = new Rectangle(rectCartonSize.X, rectCartonSize.Y, halfWidth, rectCartonSize.Height);
            Rectangle rectValueCartonSize = new Rectangle(rectCartonSize.X + halfWidth, rectCartonSize.Y, halfWidth * 2, rectCartonSize.Height);

            string cartonSize = currentGroup[0].Cells["CartonSize"].Value?.ToString() ?? "";
            g.DrawRectangle(Pens.Black, rectLabelCartonSize);
            g.DrawRectangle(Pens.Black, rectValueCartonSize);
            g.DrawString("Carton Size:", fontRegular, brush, rectLabelCartonSize);
            g.DrawString(cartonSize, fontRegular, brush, rectValueCartonSize);

            y += cartHeight + lineHeight * 3;
            _firstPage = false;
        }

        // ================= Table Header =================
        float colNoWidth = 40;
        float colQuantityWidth = 100;
        float colPackingWidth = 75;
        float colPCSWidth = 50;
        float colNameWidth = pageWidth - colNoWidth - colQuantityWidth - colPackingWidth - colPCSWidth;

        float x = startX;

        g.DrawRectangle(Pens.Black, x, y, colNoWidth, lineHeight * 2);
        g.DrawString("No", fontHeader, brush, new RectangleF(x, y, colNoWidth, lineHeight * 2),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colNoWidth;

        g.DrawRectangle(Pens.Black, x, y, colNameWidth, lineHeight);
        g.DrawString("Name of Goods", fontHeader, brush, new RectangleF(x, y, colNameWidth, lineHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

        g.DrawRectangle(Pens.Black, x, y + lineHeight, colNameWidth / 2 - 50, lineHeight);
        g.DrawString("English Name", fontHeader, brush, new RectangleF(x, y + lineHeight, colNameWidth / 2 - 50, lineHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colNameWidth / 2 - 50;

        g.DrawRectangle(Pens.Black, x, y + lineHeight, colNameWidth / 2 + 50, lineHeight);
        g.DrawString("Vietnamese Name", fontHeader, brush, new RectangleF(x, y + lineHeight, colNameWidth / 2 + 50, lineHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colNameWidth / 2 + 50;

        g.DrawRectangle(Pens.Black, x, y, colQuantityWidth, lineHeight);
        g.DrawString("Quantity", fontHeader, brush, new RectangleF(x, y, colQuantityWidth, lineHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

        g.DrawRectangle(Pens.Black, x, y + lineHeight, colQuantityWidth / 2, lineHeight);
        g.DrawString("Unit", fontHeader, brush, new RectangleF(x, y + lineHeight, colQuantityWidth / 2, lineHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colQuantityWidth / 2;

        g.DrawRectangle(Pens.Black, x, y + lineHeight, colQuantityWidth / 2, lineHeight);
        g.DrawString("N.W", fontHeader, brush, new RectangleF(x, y + lineHeight, colQuantityWidth / 2, lineHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colQuantityWidth / 2;

        g.DrawRectangle(Pens.Black, x, y, colPackingWidth, lineHeight * 2);
        g.DrawString("Packing", fontHeader, brush, new RectangleF(x, y, colPackingWidth, lineHeight * 2),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colPackingWidth;

        g.DrawRectangle(Pens.Black, x, y, colPCSWidth, lineHeight * 2);
        g.DrawString("PCS", fontHeader, brush, new RectangleF(x, y, colPCSWidth, lineHeight * 2),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

        y += lineHeight * 2;

        // ================= Table Data =================
        int count = 1;
        for (; _currentRowIndex < currentGroup.Count; _currentRowIndex++)
        {
            var row = currentGroup[_currentRowIndex];
            x = startX;

            string productEN = row.Cells["ProductNameEN"].Value?.ToString() ?? "";
            string productVN = row.Cells["ProductPackingName"].Value?.ToString() ?? "";
            string unit = row.Cells["Package"].Value?.ToString() ?? "";
            string nw = row.Cells["NWReal"]?.Value?.ToString() ?? "";
            string packingCalc = (row.Cells["Amount"].Value?.ToString() ?? "") + " " + (row.Cells["Packing"].Value?.ToString() ?? "");
            string pcs = row.Cells["PCSReal"]?.Value?.ToString() ?? "";

            if (y + lineHeight > pageHeight)
            {
                _yPosition = 50;
                e.HasMorePages = true;
                return;
            }

            g.DrawRectangle(Pens.Black, x, y, colNoWidth, lineHeight);
            g.DrawString(count.ToString(), fontRegular, brush, new RectangleF(x, y, colNoWidth, lineHeight),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += colNoWidth;

            g.DrawRectangle(Pens.Black, x, y, colNameWidth / 2 - 50, lineHeight);
            g.DrawString(productEN, fontRegular, brush, new RectangleF(x, y, colNameWidth / 2 - 50, lineHeight),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += colNameWidth / 2 - 50;

            g.DrawRectangle(Pens.Black, x, y, colNameWidth / 2 + 50, lineHeight);
            g.DrawString(productVN, fontRegular, brush, new RectangleF(x, y, colNameWidth / 2 + 50, lineHeight),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += colNameWidth / 2 + 50;

            g.DrawRectangle(Pens.Black, x, y, colQuantityWidth / 2, lineHeight);
            g.DrawString(unit, fontRegular, brush, new RectangleF(x, y, colQuantityWidth / 2, lineHeight),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += colQuantityWidth / 2;

            g.DrawRectangle(Pens.Black, x, y, colQuantityWidth / 2, lineHeight);
            g.DrawString(nw, fontRegular, brush, new RectangleF(x, y, colQuantityWidth / 2, lineHeight),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += colQuantityWidth / 2;

            g.DrawRectangle(Pens.Black, x, y, colPackingWidth, lineHeight);
            g.DrawString(packingCalc, fontRegular, brush, new RectangleF(x, y, colPackingWidth, lineHeight),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += colPackingWidth;

            g.DrawRectangle(Pens.Black, x, y, colPCSWidth, lineHeight);
            g.DrawString(pcs, fontRegular, brush, new RectangleF(x, y, colPCSWidth, lineHeight),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            y += lineHeight;
            count++;
        }

        // Xong 1 group
        _currentGroupIndex++;
        _currentRowIndex = 0;
        _yPosition = 50;
        _firstPage = true;

        e.HasMorePages = _currentGroupIndex < _groupedOrders.Count;
    }
}
