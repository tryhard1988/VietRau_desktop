using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

public class LabelInfo
{
    public string ProductName_EN { get; set; }
    public string BotanicalName { get; set; }
    public string PackingName { get; set; }
    public string BarCodeEAN13 { get; set; }
    public string ArtNr { get; set; }
    public string LotCode { get; set; }
}


public class LabelPrinter
{
    private readonly LabelInfo info;

    public LabelPrinter(LabelInfo labelInfo)
    {
        info = labelInfo;
    }

    public void Print()
    {
        PrintDocument doc = new PrintDocument();

        // Set paper size (mm to inch -> inch to hundredths)
        int width_mm = 65;
        int height_mm = 100;

        int width_hundredths = (int)(width_mm / 25.4 * 100);
        int height_hundredths = (int)(height_mm / 25.4 * 100);

        doc.DefaultPageSettings.PaperSize = new PaperSize("CustomLabel",
            width_hundredths,
            height_hundredths);

        doc.DefaultPageSettings.Landscape = true; // phiếu ngày đang quay ngang
        doc.PrintPage += Doc_PrintPage;

        doc.Print();
    }

    public void PrintPreview()
    {
        PrintDocument doc = new PrintDocument();

        // Set paper size 65mm x 100mm
        int width_mm = 65;
        int height_mm = 100;

        int w = (int)(width_mm / 25.4 * 100);
        int h = (int)(height_mm / 25.4 * 100);

        doc.DefaultPageSettings.PaperSize = new PaperSize("CustomLabel", w, h);
        doc.DefaultPageSettings.Landscape = true;

        doc.PrintPage += Doc_PrintPage;

        PrintPreviewDialog preview = new PrintPreviewDialog
        {
            Document = doc,
            Width = 1200,
            Height = 800
        };

        preview.ShowDialog();
    }


    private void Doc_PrintPage(object sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

        // --- Xoay toàn bộ nội dung 90 độ ---
        g.TranslateTransform(0, e.PageBounds.Height);
        g.RotateTransform(-90);

        // Fonts
        Font bold = new Font("Arial", 12, FontStyle.Bold);
        Font normal = new Font("Arial", 10);

        float y = 10;
        float lineHeight = 22;

        g.DrawString($"Product: {info.ProductName_EN}", bold, Brushes.Black, 10, y);
        y += lineHeight;

        g.DrawString($"Botanical: {info.BotanicalName}", normal, Brushes.Black, 10, y);
        y += lineHeight;

        g.DrawString($"Packing: {info.PackingName}", normal, Brushes.Black, 10, y);
        y += lineHeight;

        g.DrawString($"Art Nr: {info.ArtNr}", normal, Brushes.Black, 10, y);
        y += lineHeight;

        g.DrawString($"Lot: {info.LotCode}", normal, Brushes.Black, 10, y);
        y += lineHeight + 10;

        // --- TẠO ẢNH BARCODE ---
        if (!string.IsNullOrEmpty(info.BarCodeEAN13))
        {
            Bitmap barcodeImg = Ean13Generator.GenerateEAN13(info.BarCodeEAN13);

            float scale = 0.9f; // tỉ lệ nhỏ lại
            int bw = (int)(barcodeImg.Width * scale);
            int bh = (int)(barcodeImg.Height * scale);
            g.DrawImage(barcodeImg, 10, y, bw, bh);

            Font barcodeFont = new Font("Arial", 8, FontStyle.Regular);

            string code = info.BarCodeEAN13;
            int chars = code.Length;
            float charWidth = bw / (float)chars;
            float startX = 10;

            for (int i = 0; i < chars; i++)
            {
                string c = code[i].ToString();
                SizeF sz = g.MeasureString(c, barcodeFont);
                // Vẽ từng ký tự, căn giữa khoảng charWidth
                float x = startX + i * charWidth + (charWidth - sz.Width) / 2;
                float textY = y + bh + 2;
                g.DrawString(c, barcodeFont, Brushes.Black, x, textY);
            }

            y += bh + 2 + barcodeFont.Height; // tăng y cho nội dung tiếp theo

            barcodeImg.Dispose();

            y += bh + 10;
        }

        g.ResetTransform(); // restore góc vẽ
    }


}
