using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

public class PackingListPrinter
{
    private List<List<DataRow>> _groupedOrders; // Gom nhóm theo CustomerCarton
    private int _currentGroupIndex; // Nhóm hiện tại
    private int _currentRowIndex;   // Dòng hiện tại trong nhóm
    private int _yPosition;
    private int sttCount;
    private bool _firstPage;

    private DataTable _datatable;
    private string _customerName;
    private string _customerAddress;
    private PrintDocument _printDoc;

    private List<string> _selectedCartons; // Danh sách CustomerCarton cần in

    public PackingListPrinter(DataTable dgv, string customerName, string customerAddress, List<string> selectedCartons)
    {
        _datatable = dgv;
        _customerName = customerName;
        _customerAddress = customerAddress;
        _selectedCartons = selectedCartons ?? new List<string>();

        _printDoc = new PrintDocument();
        _printDoc.PrintPage += PrintDoc_PrintPage;
    }

    public void PrintPreview(Form owner)
    {
        PreparePrintRows();
        ResetPrintState();

        using (PrintPreviewDialog preview = new PrintPreviewDialog())
        {
            preview.Document = _printDoc;
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

    public void Print(string cusCarton)
    {
        DialogResult dialogResult = MessageBox.Show($"Mã Thùng: {cusCarton}\nBạn có chắc chắn muốn in không?", "Xác nhận in ấn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
        var rows = _datatable.AsEnumerable()
                            .Where(r => r.Field<int?>("CartonNo").HasValue) // CartonNo có giá trị
                            .ToList();

        // Lọc theo danh sách carton được chọn
        if (_selectedCartons != null && _selectedCartons.Count > 0)
        {
            rows = rows
                .Where(r =>
                {
                    string carton = r["CustomerCarton"]?.ToString();
                    return !string.IsNullOrEmpty(carton) && _selectedCartons.Contains(carton);
                })
                .ToList();
        }

        // Gom nhóm theo CustomerCarton
        _groupedOrders = rows
            .Where(r => r["CustomerCarton"] != null && !string.IsNullOrEmpty(r["CustomerCarton"].ToString()))
            .GroupBy(r => r["CustomerCarton"].ToString())
            .Select(g => g.ToList())
            .ToList();
    }

    private void ResetPrintState()
    {
        _currentGroupIndex = 0;
        _currentRowIndex = 0;
        sttCount = 1;
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

        Font fontLarge1 = new Font("Times New Roman", 14);
        Font fontLarge1Bold = new Font("Times New Roman", 18, FontStyle.Bold);
        Font fontHeader = new Font("Times New Roman", 12, FontStyle.Bold);
        Font fontRegular = new Font("Times New Roman", 11);
        Font fontLarge = new Font("Times New Roman", 52, FontStyle.Bold);
        Brush brush = Brushes.Black;

        int y = _yPosition;

        // ================= HEADER =================
        if (_firstPage)
        {
            int leftWidth = pageWidth / 3 - 80;
            Rectangle leftRect = new Rectangle(startX, y, leftWidth, (lineHeight + 10) * 7);
            g.DrawRectangle(Pens.Black, leftRect);

            g.DrawString("Consignee:", fontHeader, brush,
                new RectangleF(leftRect.X + 5, leftRect.Y + 2, leftRect.Width - 10, lineHeight));
            g.DrawString(_customerName, fontRegular, brush,
                new RectangleF(leftRect.X + 5, leftRect.Y + lineHeight, leftRect.Width - 10, lineHeight));
            g.DrawString(_customerAddress, fontRegular, brush,
                new RectangleF(leftRect.X + 5, leftRect.Y + lineHeight * 2, leftRect.Width - 10, leftRect.Height - lineHeight * 2));

            int rightX = startX + leftWidth;
            int rightWidth = pageWidth - leftWidth;

            int cartHeight = (lineHeight + 10) * 4;
            string customerCarton = currentGroup[0]["CustomerCarton"]?.ToString() ?? "";

            Rectangle rectCarton = new Rectangle(rightX, y, rightWidth, cartHeight);
            g.DrawRectangle(Pens.Black, rectCarton);
            g.DrawString(customerCarton, fontLarge, brush,
                new RectangleF(rectCarton.X, rectCarton.Y + 5, rectCarton.Width, rectCarton.Height),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            int halfWidth = rightWidth / 3;

            Rectangle rectCartonNo = new Rectangle(rightX, y + cartHeight, rightWidth, lineHeight + 10);
            g.DrawRectangle(Pens.Black, rectCartonNo);
            Rectangle rectLabelCartonNo = new Rectangle(rectCartonNo.X, rectCartonNo.Y, halfWidth, rectCartonNo.Height);
            Rectangle rectValueCartonNo = new Rectangle(rectCartonNo.X + halfWidth, rectCartonNo.Y, halfWidth * 2, rectCartonNo.Height);

            string cartonNo = currentGroup[0]["CartonNo"]?.ToString() ?? "";
            g.DrawRectangle(Pens.Black, rectLabelCartonNo);
           // g.DrawRectangle(Pens.Black, rectValueCartonNo);
            g.DrawString("Carton No:", fontLarge1, brush, rectLabelCartonNo, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            g.DrawString(cartonNo, fontLarge1Bold, brush, rectValueCartonNo, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            Rectangle rectExportDate = new Rectangle(rightX, y + cartHeight + lineHeight + 10, rightWidth, lineHeight + 10);
            g.DrawRectangle(Pens.Black, rectExportDate);
            Rectangle rectLabelExportDate = new Rectangle(rectExportDate.X, rectExportDate.Y, halfWidth, rectExportDate.Height);
            Rectangle rectValueExportDate = new Rectangle(rectExportDate.X + halfWidth, rectExportDate.Y, halfWidth * 2, rectExportDate.Height);

            string exportDate = ((DateTime)currentGroup[0]["ExportDate"]).ToString("dd/MM/yy");
            g.DrawRectangle(Pens.Black, rectLabelExportDate);
           // g.DrawRectangle(Pens.Black, rectValueExportDate);
            g.DrawString("Export Date:", fontLarge1, brush, rectLabelExportDate, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            g.DrawString(exportDate, fontLarge1Bold, brush, rectValueExportDate, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            Rectangle rectCartonSize = new Rectangle(rightX, y + cartHeight + (lineHeight + 10) * 2, rightWidth, lineHeight + 10);
            g.DrawRectangle(Pens.Black, rectCartonSize);
            Rectangle rectLabelCartonSize = new Rectangle(rectCartonSize.X, rectCartonSize.Y, halfWidth, rectCartonSize.Height);
            Rectangle rectValueCartonSize = new Rectangle(rectCartonSize.X + halfWidth, rectCartonSize.Y, halfWidth * 2, rectCartonSize.Height);

            string cartonSize = currentGroup[0]["CartonSize"]?.ToString() ?? "";
            g.DrawRectangle(Pens.Black, rectLabelCartonSize);
           // g.DrawRectangle(Pens.Black, rectValueCartonSize);
            g.DrawString("Carton Size:", fontLarge1, brush, rectLabelCartonSize, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            g.DrawString(cartonSize, fontLarge1Bold, brush, rectValueCartonSize, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            y += cartHeight + (lineHeight + 10) * 3;
            _firstPage = false;
        }

        // ================= Table Header =================
        float colNoWidth = 40;
        float colQuantityWidth = 120;
        float colPackingWidth = 75;
        float colPCSWidth = 60;
        float colNameWidth = pageWidth - colNoWidth - colQuantityWidth - colPackingWidth - colPCSWidth;

        float x = startX;

        g.DrawRectangle(Pens.Black, x, y, colNoWidth, lineHeight * 2);
        g.DrawString("No", fontHeader, brush, new RectangleF(x, y, colNoWidth, lineHeight * 2),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colNoWidth;

        g.DrawRectangle(Pens.Black, x, y, colNameWidth, lineHeight);
        g.DrawString("Name of Goods", fontHeader, brush, new RectangleF(x, y, colNameWidth, lineHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

        g.DrawRectangle(Pens.Black, x, y + lineHeight, colNameWidth / 2, lineHeight);
        g.DrawString("English Name", fontHeader, brush, new RectangleF(x, y + lineHeight, colNameWidth / 2, lineHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colNameWidth / 2;

        g.DrawRectangle(Pens.Black, x, y + lineHeight, colNameWidth / 2, lineHeight);
        g.DrawString("Vietnamese Name", fontHeader, brush, new RectangleF(x, y + lineHeight, colNameWidth / 2, lineHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        x += colNameWidth / 2;

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

        decimal totalNW = 0;
        float xNW = 0;
        for (; _currentRowIndex < currentGroup.Count; _currentRowIndex++)
        {
            var row = currentGroup[_currentRowIndex];
            x = startX;

            string productEN = row["ProductNameEN"]?.ToString() ?? "";
            string productVN = row["ProductNameVN"]?.ToString() ?? "";
            string unit = row["Package"]?.ToString() ?? "";
            string nw = row["NWReal"]?.ToString() ?? "";
            decimal amount = row["Amount"] == null || row["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Amount"]);
            string packing = row["Packing"]?.ToString() ?? "";
            string packingCalc = "";
            string pcs = row["PCSReal"]?.ToString() ?? "";

            string name = productEN.Length > productVN.Length ? productEN : productVN;

            // RectangleF rect = new RectangleF(x, y, colNameWidth, 9999);
            float colNameWidthTemp = colNameWidth / 2 - 20;
            SizeF textSize = e.Graphics.MeasureString(name, fontRegular, (int)colNameWidthTemp, StringFormat.GenericDefault);
            int dynamicHeight = Math.Max(lineHeight, (int)textSize.Height);

            if (amount > 0 && packing.CompareTo("") != 0)
            {
                packingCalc = amount.ToString("0.##") + " " + packing;
            }
            else
            {
                packingCalc = packing.CompareTo("weight") == 0 ? packing : "";
            }

            if (y + lineHeight > pageHeight)
            {
                _yPosition = 50;
                e.HasMorePages = true;
                return;
            }

            g.DrawRectangle(Pens.Black, x, y, colNoWidth, dynamicHeight);
            g.DrawString(sttCount.ToString(), fontRegular, brush, new RectangleF(x, y, colNoWidth, dynamicHeight),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += colNoWidth;

            g.DrawRectangle(Pens.Black, x, y, colNameWidth / 2, dynamicHeight);
            g.DrawString(productEN, fontRegular, brush, new RectangleF(x + 5, y, colNameWidth / 2 - 5, dynamicHeight),
                new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
            x += colNameWidth / 2;

            g.DrawRectangle(Pens.Black, x, y, colNameWidth / 2, dynamicHeight);
            g.DrawString(productVN, fontRegular, brush, new RectangleF(x + 5, y, colNameWidth / 2 - 5, dynamicHeight),
                new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
            x += colNameWidth / 2;

            g.DrawRectangle(Pens.Black, x, y, colQuantityWidth / 2, dynamicHeight);
            g.DrawString(unit, fontRegular, brush, new RectangleF(x, y, colQuantityWidth / 2, dynamicHeight),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += colQuantityWidth / 2;

            g.DrawRectangle(Pens.Black, x, y, colQuantityWidth / 2, dynamicHeight);
            g.DrawString(nw, fontRegular, brush, new RectangleF(x, y, colQuantityWidth / 2, dynamicHeight),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            xNW = x;
            x += colQuantityWidth / 2;
            totalNW += Convert.ToDecimal(nw);

            g.DrawRectangle(Pens.Black, x, y, colPackingWidth, dynamicHeight);
            g.DrawString(packingCalc, fontRegular, brush, new RectangleF(x, y, colPackingWidth, dynamicHeight),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += colPackingWidth;

            g.DrawRectangle(Pens.Black, x, y, colPCSWidth, dynamicHeight);
            if (Convert.ToInt32(pcs) > 0)
            {
                g.DrawRectangle(Pens.Black, x, y, colPCSWidth, dynamicHeight);
                g.DrawString(pcs, fontRegular, brush, new RectangleF(x, y, colPCSWidth, dynamicHeight),
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            y += dynamicHeight;
            sttCount++;
        }

        g.DrawRectangle(Pens.Black, xNW + colQuantityWidth / 2, y, colPCSWidth + colPackingWidth, lineHeight);
        g.DrawRectangle(Pens.Black, startX, y, xNW - startX, lineHeight);
        g.DrawString("N.W Total", fontHeader, brush, new RectangleF(startX, y, xNW - startX, lineHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

        g.DrawRectangle(Pens.Black, xNW, y, colQuantityWidth / 2, lineHeight);
        g.DrawString(totalNW.ToString("F2"), fontHeader, brush, new RectangleF(xNW, y, colQuantityWidth / 2, lineHeight),
            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        // Xong 1 group
        _currentGroupIndex++;
        _currentRowIndex = 0;
        sttCount = 1;
        _yPosition = 50;
        _firstPage = true;

        e.HasMorePages = _currentGroupIndex < _groupedOrders.Count;
    }
}
