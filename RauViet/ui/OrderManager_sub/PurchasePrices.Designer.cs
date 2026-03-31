
namespace RauViet.ui
{
    partial class PurchasePrices
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.log_GV = new System.Windows.Forms.DataGridView();
            this.edit_btn = new System.Windows.Forms.Button();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.readOnly_btn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.seller_cbb = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.sku_cbb = new System.Windows.Forms.ComboBox();
            this.price_tb = new System.Windows.Forms.TextBox();
            this.amount_lb = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.id_tb = new System.Windows.Forms.TextBox();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).BeginInit();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.log_GV);
            this.panel1.Controls.Add(this.edit_btn);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.readOnly_btn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.id_tb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(695, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(569, 681);
            this.panel1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.PeachPuff;
            this.label5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label5.Location = new System.Drawing.Point(0, 438);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(569, 23);
            this.label5.TabIndex = 68;
            this.label5.Text = "Lịch sử thay đổi";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // log_GV
            // 
            this.log_GV.AllowUserToAddRows = false;
            this.log_GV.AllowUserToDeleteRows = false;
            this.log_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.log_GV.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.log_GV.Location = new System.Drawing.Point(0, 461);
            this.log_GV.Name = "log_GV";
            this.log_GV.ReadOnly = true;
            this.log_GV.Size = new System.Drawing.Size(569, 220);
            this.log_GV.TabIndex = 67;
            // 
            // edit_btn
            // 
            this.edit_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.edit_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_btn.Location = new System.Drawing.Point(321, 106);
            this.edit_btn.Name = "edit_btn";
            this.edit_btn.Size = new System.Drawing.Size(94, 32);
            this.edit_btn.TabIndex = 47;
            this.edit_btn.Text = "Chỉnh sửa";
            this.edit_btn.UseVisualStyleBackColor = false;
            // 
            // newCustomerBtn
            // 
            this.newCustomerBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newCustomerBtn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCustomerBtn.Location = new System.Drawing.Point(417, 106);
            this.newCustomerBtn.Name = "newCustomerBtn";
            this.newCustomerBtn.Size = new System.Drawing.Size(74, 32);
            this.newCustomerBtn.TabIndex = 46;
            this.newCustomerBtn.Text = "Tạo mới";
            this.newCustomerBtn.UseVisualStyleBackColor = false;
            // 
            // readOnly_btn
            // 
            this.readOnly_btn.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.readOnly_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readOnly_btn.Location = new System.Drawing.Point(449, 106);
            this.readOnly_btn.Name = "readOnly_btn";
            this.readOnly_btn.Size = new System.Drawing.Size(42, 32);
            this.readOnly_btn.TabIndex = 45;
            this.readOnly_btn.Text = "X";
            this.readOnly_btn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.seller_cbb);
            this.info_gb.Controls.Add(this.label7);
            this.info_gb.Controls.Add(this.sku_cbb);
            this.info_gb.Controls.Add(this.price_tb);
            this.info_gb.Controls.Add(this.amount_lb);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info_gb.Location = new System.Drawing.Point(17, 131);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(488, 142);
            this.info_gb.TabIndex = 43;
            this.info_gb.TabStop = false;
            // 
            // seller_cbb
            // 
            this.seller_cbb.FormattingEnabled = true;
            this.seller_cbb.IntegralHeight = false;
            this.seller_cbb.Location = new System.Drawing.Point(105, 35);
            this.seller_cbb.Name = "seller_cbb";
            this.seller_cbb.Size = new System.Drawing.Size(366, 24);
            this.seller_cbb.TabIndex = 76;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 16);
            this.label7.TabIndex = 75;
            this.label7.Text = "Nhà Cung Cấp:";
            // 
            // sku_cbb
            // 
            this.sku_cbb.FormattingEnabled = true;
            this.sku_cbb.IntegralHeight = false;
            this.sku_cbb.Location = new System.Drawing.Point(105, 67);
            this.sku_cbb.Name = "sku_cbb";
            this.sku_cbb.Size = new System.Drawing.Size(193, 24);
            this.sku_cbb.TabIndex = 62;
            // 
            // price_tb
            // 
            this.price_tb.Location = new System.Drawing.Point(105, 99);
            this.price_tb.Name = "price_tb";
            this.price_tb.Size = new System.Drawing.Size(147, 23);
            this.price_tb.TabIndex = 60;
            // 
            // amount_lb
            // 
            this.amount_lb.AutoSize = true;
            this.amount_lb.Location = new System.Drawing.Point(69, 102);
            this.amount_lb.Name = "amount_lb";
            this.amount_lb.Size = new System.Drawing.Size(30, 16);
            this.amount_lb.TabIndex = 59;
            this.amount_lb.Text = "Giá:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 16);
            this.label1.TabIndex = 47;
            this.label1.Text = "Sản Phẩm:";
            // 
            // id_tb
            // 
            this.id_tb.Location = new System.Drawing.Point(338, 16);
            this.id_tb.Name = "id_tb";
            this.id_tb.ReadOnly = true;
            this.id_tb.Size = new System.Drawing.Size(31, 20);
            this.id_tb.TabIndex = 42;
            this.id_tb.Visible = false;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.Location = new System.Drawing.Point(17, 288);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(59, 23);
            this.status_lb.TabIndex = 40;
            this.status_lb.Text = "status";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(222, 279);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(104, 43);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(695, 36);
            this.panel2.TabIndex = 63;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGV.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 36);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(695, 645);
            this.dataGV.TabIndex = 64;
            // 
            // PurchasePrices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "PurchasePrices";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).EndInit();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.TextBox id_tb;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.ComboBox sku_cbb;
        private System.Windows.Forms.Label amount_lb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Button readOnly_btn;
        private System.Windows.Forms.Button newCustomerBtn;
        private System.Windows.Forms.Button edit_btn;
        private System.Windows.Forms.TextBox price_tb;
        private System.Windows.Forms.ComboBox seller_cbb;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView log_GV;
    }
}