
namespace RauViet.ui
{
    partial class MealOrder
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
            this.label5 = new System.Windows.Forms.Label();
            this.log_GV = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.xem_btn = new System.Windows.Forms.Button();
            this.in_btn = new System.Windows.Forms.Button();
            this.in_EndDay_dtp = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.in_StartDay_dtp = new System.Windows.Forms.DateTimePicker();
            this.readOnly_btn = new System.Windows.Forms.Button();
            this.edit_btn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.VAT_tb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.price_tb = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.orderDate_start_dtp = new System.Windows.Forms.DateTimePicker();
            this.isDone_CB = new System.Windows.Forms.CheckBox();
            this.note_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.quantity_tb = new System.Windows.Forms.TextBox();
            this.status_lb = new System.Windows.Forms.Label();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.ID_tb = new System.Windows.Forms.TextBox();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.orderDate_end_dtp = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.linkStartEnd_cb = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.log_GV);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.readOnly_btn);
            this.panel1.Controls.Add(this.edit_btn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.ID_tb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(901, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(589, 681);
            this.panel1.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.PeachPuff;
            this.label5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label5.Location = new System.Drawing.Point(0, 477);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(589, 23);
            this.label5.TabIndex = 72;
            this.label5.Text = "Lịch sử thay đổi";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // log_GV
            // 
            this.log_GV.AllowUserToAddRows = false;
            this.log_GV.AllowUserToDeleteRows = false;
            this.log_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.log_GV.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.log_GV.Location = new System.Drawing.Point(0, 500);
            this.log_GV.Name = "log_GV";
            this.log_GV.ReadOnly = true;
            this.log_GV.Size = new System.Drawing.Size(589, 181);
            this.log_GV.TabIndex = 71;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Info;
            this.groupBox1.Controls.Add(this.xem_btn);
            this.groupBox1.Controls.Add(this.in_btn);
            this.groupBox1.Controls.Add(this.in_EndDay_dtp);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.in_StartDay_dtp);
            this.groupBox1.Location = new System.Drawing.Point(29, 348);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 104);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            // 
            // xem_btn
            // 
            this.xem_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.xem_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xem_btn.Location = new System.Drawing.Point(154, 53);
            this.xem_btn.Name = "xem_btn";
            this.xem_btn.Size = new System.Drawing.Size(79, 37);
            this.xem_btn.TabIndex = 36;
            this.xem_btn.Text = "Xem";
            this.xem_btn.UseVisualStyleBackColor = false;
            // 
            // in_btn
            // 
            this.in_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.in_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.in_btn.Location = new System.Drawing.Point(67, 53);
            this.in_btn.Name = "in_btn";
            this.in_btn.Size = new System.Drawing.Size(79, 37);
            this.in_btn.TabIndex = 35;
            this.in_btn.Text = "In";
            this.in_btn.UseVisualStyleBackColor = false;
            // 
            // in_EndDay_dtp
            // 
            this.in_EndDay_dtp.Location = new System.Drawing.Point(168, 24);
            this.in_EndDay_dtp.Name = "in_EndDay_dtp";
            this.in_EndDay_dtp.Size = new System.Drawing.Size(98, 20);
            this.in_EndDay_dtp.TabIndex = 34;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(133, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 16);
            this.label7.TabIndex = 33;
            this.label7.Text = "==>";
            // 
            // in_StartDay_dtp
            // 
            this.in_StartDay_dtp.Location = new System.Drawing.Point(32, 24);
            this.in_StartDay_dtp.Name = "in_StartDay_dtp";
            this.in_StartDay_dtp.Size = new System.Drawing.Size(98, 20);
            this.in_StartDay_dtp.TabIndex = 32;
            // 
            // readOnly_btn
            // 
            this.readOnly_btn.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.readOnly_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readOnly_btn.Location = new System.Drawing.Point(441, 57);
            this.readOnly_btn.Name = "readOnly_btn";
            this.readOnly_btn.Size = new System.Drawing.Size(42, 32);
            this.readOnly_btn.TabIndex = 32;
            this.readOnly_btn.Text = "X";
            this.readOnly_btn.UseVisualStyleBackColor = false;
            // 
            // edit_btn
            // 
            this.edit_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.edit_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_btn.Location = new System.Drawing.Point(303, 56);
            this.edit_btn.Name = "edit_btn";
            this.edit_btn.Size = new System.Drawing.Size(94, 32);
            this.edit_btn.TabIndex = 31;
            this.edit_btn.Text = "Chỉnh sửa";
            this.edit_btn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.linkStartEnd_cb);
            this.info_gb.Controls.Add(this.label6);
            this.info_gb.Controls.Add(this.orderDate_end_dtp);
            this.info_gb.Controls.Add(this.label4);
            this.info_gb.Controls.Add(this.VAT_tb);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Controls.Add(this.price_tb);
            this.info_gb.Controls.Add(this.label8);
            this.info_gb.Controls.Add(this.orderDate_start_dtp);
            this.info_gb.Controls.Add(this.isDone_CB);
            this.info_gb.Controls.Add(this.note_tb);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Controls.Add(this.label3);
            this.info_gb.Controls.Add(this.quantity_tb);
            this.info_gb.Location = new System.Drawing.Point(61, 89);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(422, 190);
            this.info_gb.TabIndex = 28;
            this.info_gb.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(49, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 16);
            this.label4.TabIndex = 36;
            this.label4.Text = "VAT:";
            // 
            // VAT_tb
            // 
            this.VAT_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VAT_tb.Location = new System.Drawing.Point(94, 132);
            this.VAT_tb.Name = "VAT_tb";
            this.VAT_tb.Size = new System.Drawing.Size(137, 23);
            this.VAT_tb.TabIndex = 37;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(28, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 16);
            this.label1.TabIndex = 34;
            this.label1.Text = "Đơn Giá:";
            // 
            // price_tb
            // 
            this.price_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.price_tb.Location = new System.Drawing.Point(94, 103);
            this.price_tb.Name = "price_tb";
            this.price_tb.Size = new System.Drawing.Size(137, 23);
            this.price_tb.TabIndex = 35;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(45, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 16);
            this.label8.TabIndex = 33;
            this.label8.Text = "Ngày:";
            // 
            // orderDate_start_dtp
            // 
            this.orderDate_start_dtp.Location = new System.Drawing.Point(94, 19);
            this.orderDate_start_dtp.Name = "orderDate_start_dtp";
            this.orderDate_start_dtp.Size = new System.Drawing.Size(98, 20);
            this.orderDate_start_dtp.TabIndex = 32;
            // 
            // isDone_CB
            // 
            this.isDone_CB.AutoSize = true;
            this.isDone_CB.Location = new System.Drawing.Point(94, 165);
            this.isDone_CB.Name = "isDone_CB";
            this.isDone_CB.Size = new System.Drawing.Size(102, 17);
            this.isDone_CB.TabIndex = 21;
            this.isDone_CB.Text = "Đã Thanh Toán";
            this.isDone_CB.UseVisualStyleBackColor = true;
            // 
            // note_tb
            // 
            this.note_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.note_tb.Location = new System.Drawing.Point(94, 45);
            this.note_tb.Name = "note_tb";
            this.note_tb.Size = new System.Drawing.Size(311, 23);
            this.note_tb.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(23, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Diễn Giải:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Số Suất Ăn:";
            // 
            // quantity_tb
            // 
            this.quantity_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.quantity_tb.Location = new System.Drawing.Point(94, 74);
            this.quantity_tb.Name = "quantity_tb";
            this.quantity_tb.Size = new System.Drawing.Size(114, 23);
            this.quantity_tb.TabIndex = 18;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(6, 306);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // newCustomerBtn
            // 
            this.newCustomerBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newCustomerBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCustomerBtn.Location = new System.Drawing.Point(398, 56);
            this.newCustomerBtn.Name = "newCustomerBtn";
            this.newCustomerBtn.Size = new System.Drawing.Size(85, 32);
            this.newCustomerBtn.TabIndex = 25;
            this.newCustomerBtn.Text = "Tạo mới";
            this.newCustomerBtn.UseVisualStyleBackColor = false;
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(179, 295);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(113, 47);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // ID_tb
            // 
            this.ID_tb.Location = new System.Drawing.Point(29, 12);
            this.ID_tb.Name = "ID_tb";
            this.ID_tb.ReadOnly = true;
            this.ID_tb.Size = new System.Drawing.Size(32, 20);
            this.ID_tb.TabIndex = 16;
            this.ID_tb.Visible = false;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataGV.Location = new System.Drawing.Point(0, 0);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(901, 681);
            this.dataGV.TabIndex = 1;
            // 
            // orderDate_end_dtp
            // 
            this.orderDate_end_dtp.Location = new System.Drawing.Point(242, 21);
            this.orderDate_end_dtp.Name = "orderDate_end_dtp";
            this.orderDate_end_dtp.Size = new System.Drawing.Size(98, 20);
            this.orderDate_end_dtp.TabIndex = 38;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(203, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 16);
            this.label6.TabIndex = 39;
            this.label6.Text = "=>";
            // 
            // linkStartEnd_cb
            // 
            this.linkStartEnd_cb.AutoSize = true;
            this.linkStartEnd_cb.Checked = true;
            this.linkStartEnd_cb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.linkStartEnd_cb.Location = new System.Drawing.Point(193, 23);
            this.linkStartEnd_cb.Name = "linkStartEnd_cb";
            this.linkStartEnd_cb.Size = new System.Drawing.Size(15, 14);
            this.linkStartEnd_cb.TabIndex = 73;
            this.linkStartEnd_cb.UseVisualStyleBackColor = true;
            // 
            // MealOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1490, 681);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGV);
            this.Name = "MealOrder";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox quantity_tb;
        private System.Windows.Forms.TextBox note_tb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Button newCustomerBtn;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.CheckBox isDone_CB;
        private System.Windows.Forms.TextBox ID_tb;
        private System.Windows.Forms.Button readOnly_btn;
        private System.Windows.Forms.Button edit_btn;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker orderDate_start_dtp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox price_tb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox VAT_tb;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker in_StartDay_dtp;
        private System.Windows.Forms.Button xem_btn;
        private System.Windows.Forms.Button in_btn;
        private System.Windows.Forms.DateTimePicker in_EndDay_dtp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView log_GV;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker orderDate_end_dtp;
        private System.Windows.Forms.CheckBox linkStartEnd_cb;
    }
}