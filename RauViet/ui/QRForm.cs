using PdfSharp.Drawing;
using PdfSharp.Pdf;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class QRForm : Form
    {
        public QRForm()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string text = txtContent.Text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("Vui lòng nhập nội dung trước!");
                return;
            }

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap icon = null;
            if (picLogo.Image != null)
            {
                icon = new Bitmap(picLogo.Image, new Size(80, 80)); // kích thước logo
            }

            Bitmap qrImage = qrCode.GetGraphic(
                20,                 // độ phân giải ô vuông
                Color.Black,        // màu QR
                Color.White,        // màu nền
                icon,               // logo
                icon == null ? 0 : 15,   // kích thước logo (%) 
                1,                  // bo góc logo
                false               // vẽ đường border quanh logo
            );

            picQR.Image = qrImage;
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (picQR.Image == null)
            {
                MessageBox.Show("Chưa có mã QR để lưu!");
                return;
            }

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "PNG Image|*.png";
            save.Title = "Save QR Code";
            save.FileName = "QRCode.png";

            if (save.ShowDialog() == DialogResult.OK)
            {
                string pngPath = save.FileName;
                string pdfPath = Path.ChangeExtension(save.FileName, ".pdf");

                // Lưu ảnh PNG
                picQR.Image.Save(pngPath, ImageFormat.Png);

                // Lưu PDF
                SaveQRToPdf(picQR.Image, pdfPath);

                MessageBox.Show("Đã lưu PNG và PDF thành công!");
            }
        }

        private void SaveQRToPdf(Image qrImage, string pdfPath)
        {
            PdfDocument doc = new PdfDocument();
            doc.Info.Title = "QR Code PDF";

            PdfPage page = doc.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // chuyển Image WinForms sang XImage
            using (MemoryStream ms = new MemoryStream())
            {
                qrImage.Save(ms, ImageFormat.Png);
                XImage img = XImage.FromStream(ms);

                double x = (page.Width.Point - img.PixelWidth) / 2.0;
                double y = (page.Height.Point - img.PixelHeight) / 2.0;

                gfx.DrawImage(img, x, y);
            }

            doc.Save(pdfPath);
            doc.Close();
        }


    private void logoBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.png;*.jpg;*.jpeg";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                picLogo.Image = Image.FromFile(dlg.FileName);

                btnGenerate_Click(null, null);
            }
        }
    }
}
