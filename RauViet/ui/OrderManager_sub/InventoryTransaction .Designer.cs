
namespace RauViet.ui
{
    partial class InventoryTransaction
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.edit_btn = new System.Windows.Forms.Button();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.readOnly_btn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.transactionType_CB = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.note_tb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.transactionDate_dtp = new System.Windows.Forms.DateTimePicker();
            this.sku_cbb = new System.Windows.Forms.ComboBox();
            this.quantity_tb = new System.Windows.Forms.TextBox();
            this.amount_lb = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.id_tb = new System.Windows.Forms.TextBox();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.edit_btn);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.readOnly_btn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.id_tb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(861, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(403, 681);
            this.panel1.TabIndex = 0;
            // 
            // edit_btn
            // 
            this.edit_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.edit_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_btn.Location = new System.Drawing.Point(199, 77);
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
            this.newCustomerBtn.Location = new System.Drawing.Point(295, 77);
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
            this.readOnly_btn.Location = new System.Drawing.Point(327, 77);
            this.readOnly_btn.Name = "readOnly_btn";
            this.readOnly_btn.Size = new System.Drawing.Size(42, 32);
            this.readOnly_btn.TabIndex = 45;
            this.readOnly_btn.Text = "X";
            this.readOnly_btn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.transactionType_CB);
            this.info_gb.Controls.Add(this.label4);
            this.info_gb.Controls.Add(this.note_tb);
            this.info_gb.Controls.Add(this.label3);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Controls.Add(this.transactionDate_dtp);
            this.info_gb.Controls.Add(this.sku_cbb);
            this.info_gb.Controls.Add(this.quantity_tb);
            this.info_gb.Controls.Add(this.amount_lb);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info_gb.Location = new System.Drawing.Point(17, 102);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(352, 260);
            this.info_gb.TabIndex = 43;
            this.info_gb.TabStop = false;
            // 
            // transactionType_CB
            // 
            this.transactionType_CB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.transactionType_CB.FormattingEnabled = true;
            this.transactionType_CB.IntegralHeight = false;
            this.transactionType_CB.Location = new System.Drawing.Point(131, 69);
            this.transactionType_CB.Name = "transactionType_CB";
            this.transactionType_CB.Size = new System.Drawing.Size(96, 24);
            this.transactionType_CB.TabIndex = 68;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 16);
            this.label4.TabIndex = 67;
            this.label4.Text = "Loại:";
            // 
            // note_tb
            // 
            this.note_tb.Location = new System.Drawing.Point(131, 169);
            this.note_tb.Multiline = true;
            this.note_tb.Name = "note_tb";
            this.note_tb.Size = new System.Drawing.Size(193, 56);
            this.note_tb.TabIndex = 66;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 175);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.TabIndex = 65;
            this.label3.Text = "Ghi Chú:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 16);
            this.label2.TabIndex = 64;
            this.label2.Text = "Ngày Phát Sinh:";
            // 
            // transactionDate_dtp
            // 
            this.transactionDate_dtp.Location = new System.Drawing.Point(131, 103);
            this.transactionDate_dtp.Name = "transactionDate_dtp";
            this.transactionDate_dtp.Size = new System.Drawing.Size(106, 23);
            this.transactionDate_dtp.TabIndex = 63;
            // 
            // sku_cbb
            // 
            this.sku_cbb.FormattingEnabled = true;
            this.sku_cbb.IntegralHeight = false;
            this.sku_cbb.Location = new System.Drawing.Point(131, 35);
            this.sku_cbb.Name = "sku_cbb";
            this.sku_cbb.Size = new System.Drawing.Size(193, 24);
            this.sku_cbb.TabIndex = 62;
            // 
            // quantity_tb
            // 
            this.quantity_tb.Location = new System.Drawing.Point(131, 136);
            this.quantity_tb.Name = "quantity_tb";
            this.quantity_tb.Size = new System.Drawing.Size(96, 23);
            this.quantity_tb.TabIndex = 60;
            // 
            // amount_lb
            // 
            this.amount_lb.AutoSize = true;
            this.amount_lb.Location = new System.Drawing.Point(16, 141);
            this.amount_lb.Name = "amount_lb";
            this.amount_lb.Size = new System.Drawing.Size(66, 16);
            this.amount_lb.TabIndex = 59;
            this.amount_lb.Text = "Số Lượng:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 16);
            this.label1.TabIndex = 47;
            this.label1.Text = "Sản Phẩm:";
            // 
            // id_tb
            // 
            this.id_tb.Location = new System.Drawing.Point(280, 541);
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
            this.status_lb.Location = new System.Drawing.Point(44, 433);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(59, 23);
            this.status_lb.TabIndex = 40;
            this.status_lb.Text = "status";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(140, 380);
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
            this.panel2.Size = new System.Drawing.Size(861, 36);
            this.panel2.TabIndex = 63;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 36);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(861, 645);
            this.dataGV.TabIndex = 64;
            // 
            // InventoryTransaction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "InventoryTransaction";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker transactionDate_dtp;
        private System.Windows.Forms.TextBox quantity_tb;
        private System.Windows.Forms.TextBox note_tb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox transactionType_CB;
        private System.Windows.Forms.Label label4;
    }
}