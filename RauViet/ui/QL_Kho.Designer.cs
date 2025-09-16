namespace RauViet.ui
{
    partial class QL_kho
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.khachhang_btn = new System.Windows.Forms.Button();
            this.products_SKU_btn = new System.Windows.Forms.Button();
            this.productpacking_btn = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.content_panel = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // khachhang_btn
            // 
            this.khachhang_btn.Dock = System.Windows.Forms.DockStyle.Left;
            this.khachhang_btn.Location = new System.Drawing.Point(0, 0);
            this.khachhang_btn.Name = "khachhang_btn";
            this.khachhang_btn.Size = new System.Drawing.Size(75, 62);
            this.khachhang_btn.TabIndex = 0;
            this.khachhang_btn.Text = "Khách Hàng";
            this.khachhang_btn.UseVisualStyleBackColor = true;
            // 
            // products_SKU_btn
            // 
            this.products_SKU_btn.Dock = System.Windows.Forms.DockStyle.Left;
            this.products_SKU_btn.Location = new System.Drawing.Point(75, 0);
            this.products_SKU_btn.Name = "products_SKU_btn";
            this.products_SKU_btn.Size = new System.Drawing.Size(75, 62);
            this.products_SKU_btn.TabIndex = 1;
            this.products_SKU_btn.Text = "DS Sản Phẩm Theo SKU";
            this.products_SKU_btn.UseVisualStyleBackColor = true;
            // 
            // productpacking_btn
            // 
            this.productpacking_btn.Dock = System.Windows.Forms.DockStyle.Left;
            this.productpacking_btn.Location = new System.Drawing.Point(150, 0);
            this.productpacking_btn.Name = "productpacking_btn";
            this.productpacking_btn.Size = new System.Drawing.Size(75, 62);
            this.productpacking_btn.TabIndex = 2;
            this.productpacking_btn.Text = "DS Sản Phẩn Theo Qui Cách";
            this.productpacking_btn.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Dock = System.Windows.Forms.DockStyle.Left;
            this.button4.Location = new System.Drawing.Point(225, 0);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 62);
            this.button4.TabIndex = 3;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Dock = System.Windows.Forms.DockStyle.Left;
            this.button5.Location = new System.Drawing.Point(300, 0);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 62);
            this.button5.TabIndex = 4;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Dock = System.Windows.Forms.DockStyle.Left;
            this.button6.Location = new System.Drawing.Point(375, 0);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 62);
            this.button6.TabIndex = 5;
            this.button6.Text = "button6";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.productpacking_btn);
            this.panel1.Controls.Add(this.products_SKU_btn);
            this.panel1.Controls.Add(this.khachhang_btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1046, 62);
            this.panel1.TabIndex = 0;
            // 
            // content_panel
            // 
            this.content_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content_panel.Location = new System.Drawing.Point(0, 62);
            this.content_panel.Name = "content_panel";
            this.content_panel.Size = new System.Drawing.Size(1046, 485);
            this.content_panel.TabIndex = 1;
            // 
            // QL_kho
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1046, 547);
            this.Controls.Add(this.content_panel);
            this.Controls.Add(this.panel1);
            this.Name = "QL_kho";
            this.Text = "Quản Lý Kho";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button khachhang_btn;
        private System.Windows.Forms.Button products_SKU_btn;
        private System.Windows.Forms.Button productpacking_btn;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel content_panel;
    }
}