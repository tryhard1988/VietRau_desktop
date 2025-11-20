using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

public class LabelPrinter
{
    private string mProductName_EN;
    private string mBotanicalName;
    private string mPackingName;
    private string mBarCodeEAN13;
    private string mArtNr;
    private string mLotCode;
    private bool isPreview = false;
    public LabelPrinter(string mProductName_EN, string mBotanicalName, string mPackingName, string mBarCodeEAN13, string mArtNr, string mLotCode)
    {
        this.mProductName_EN = mProductName_EN;
        this.mBotanicalName = mBotanicalName;
        this.mPackingName = mPackingName;
        this.mBarCodeEAN13 = mBarCodeEAN13;
        this.mArtNr = mArtNr;
        this.mLotCode = mLotCode;
    }

    public void Print()
    {
        PrintDocument doc = new PrintDocument();

        isPreview = false;
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

        using (PrintDialog pd = new PrintDialog())
        {
            pd.Document = doc;

            if (pd.ShowDialog() == DialogResult.OK)
            {
                doc.Print();
            }
        }
    }

    public void PrintPreview()
    {
        PrintDocument doc = new PrintDocument();
        isPreview = true;
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

        if (isPreview)
        {
            Image bg = RauViet.Properties.Resources.vr_label;
            g.DrawImage(bg, 0, 0, e.PageBounds.Height, e.PageBounds.Width); // chú ý Width & Height do xoay
        }

        {
            SizeF size = g.MeasureString(mProductName_EN, bold);
            float _x = (e.PageBounds.Height - size.Width) / 2f;
            float _y = (e.PageBounds.Width - size.Height) / 2f - 45;
            g.DrawString(mProductName_EN, bold, Brushes.Black, _x, _y);
        }
        {
            SizeF size = g.MeasureString(mBotanicalName, normal);
            float _x = (e.PageBounds.Height - size.Width) / 2f;
            float _y = (e.PageBounds.Width - size.Height) / 2f - 30;
            g.DrawString(mBotanicalName, normal, Brushes.Black, _x, _y);
        }
        {
            SizeF size = g.MeasureString($"Lot: {mLotCode}", bold);
            float _x = (e.PageBounds.Height - size.Width + 10);
            float _y = (e.PageBounds.Width - size.Height) / 2f;
            g.DrawString($"Art-Nr: {mArtNr}", normal, Brushes.Black, 20, _y);
            g.DrawString($"Lot: {mLotCode}", normal, Brushes.Black, _x, _y);
        }
        {
            SizeF size = g.MeasureString(mPackingName, bold);
            float _x = (e.PageBounds.Height - size.Width) / 2f;
            float _y = e.PageBounds.Width * 2/3 + 13;
            g.DrawString(mPackingName, normal, Brushes.Black, _x, _y);
        }

        // --- TẠO ẢNH BARCODE ---
        if (!string.IsNullOrEmpty(mBarCodeEAN13))
        {
            Bitmap barcodeImg = Ean13Generator.GenerateEAN13(mBarCodeEAN13);

            int bw = 200;
            int bh = 50;
            float barImage_X = (e.PageBounds.Height - bw) / 2f;
            float barImage_Y = (e.PageBounds.Width * 2/3 - bh) - 5;
            g.DrawImage(barcodeImg, barImage_X, barImage_Y, bw, bh);

            Font barcodeFont = new Font("Arial", 8, FontStyle.Regular);

            string code = mBarCodeEAN13;
            int chars = code.Length;
            float charWidth = bw / (float)chars;
            

            for (int i = 0; i < chars; i++)
            {
                string c = code[i].ToString();
                SizeF sz = g.MeasureString(c, barcodeFont);
                // Vẽ từng ký tự, căn giữa khoảng charWidth
                float x = barImage_X + i * charWidth + (charWidth - sz.Width) / 2;
                float textY = barImage_Y + bh + 2;
                g.DrawString(c, barcodeFont, Brushes.Black, x, textY);
            }

            barcodeImg.Dispose();
        }

        g.ResetTransform(); // restore góc vẽ
    }


}
