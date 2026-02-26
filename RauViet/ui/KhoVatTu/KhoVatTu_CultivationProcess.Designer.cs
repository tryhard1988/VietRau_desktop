
namespace RauViet.ui
{
    partial class KhoVatTu_CultivationProcess
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KhoVatTu_CultivationProcess));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.log_GV = new System.Windows.Forms.DataGridView();
            this.readOnly_btn = new System.Windows.Forms.Button();
            this.edit_btn = new System.Windows.Forms.Button();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.waterAmount_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.daysAfter_tb = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.sku_CBB = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.baseDateType_CBB = new System.Windows.Forms.ComboBox();
            this.congViec_CBB = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.materialQuantity_tb = new System.Windows.Forms.TextBox();
            this.vatTu_CB = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.id_tb = new System.Windows.Forms.TextBox();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.plant_lb = new System.Windows.Forms.Label();
            this.LoadDefaultData_btn = new System.Windows.Forms.Button();
            this.processDate_dtp = new System.Windows.Forms.DateTimePicker();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).BeginInit();
            this.info_gb.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LoadDefaultData_btn);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.log_GV);
            this.panel1.Controls.Add(this.readOnly_btn);
            this.panel1.Controls.Add(this.edit_btn);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.id_tb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(816, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(533, 758);
            this.panel1.TabIndex = 69;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.PeachPuff;
            this.label4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label4.Location = new System.Drawing.Point(0, 554);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(533, 23);
            this.label4.TabIndex = 70;
            this.label4.Text = "Lịch sử thay đổi";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // log_GV
            // 
            this.log_GV.AllowUserToAddRows = false;
            this.log_GV.AllowUserToDeleteRows = false;
            this.log_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.log_GV.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.log_GV.Location = new System.Drawing.Point(0, 577);
            this.log_GV.Name = "log_GV";
            this.log_GV.ReadOnly = true;
            this.log_GV.Size = new System.Drawing.Size(533, 181);
            this.log_GV.TabIndex = 69;
            // 
            // readOnly_btn
            // 
            this.readOnly_btn.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.readOnly_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readOnly_btn.Location = new System.Drawing.Point(490, 46);
            this.readOnly_btn.Name = "readOnly_btn";
            this.readOnly_btn.Size = new System.Drawing.Size(42, 32);
            this.readOnly_btn.TabIndex = 48;
            this.readOnly_btn.Text = "X";
            this.readOnly_btn.UseVisualStyleBackColor = false;
            // 
            // edit_btn
            // 
            this.edit_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.edit_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_btn.Location = new System.Drawing.Point(363, 46);
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
            this.newCustomerBtn.Location = new System.Drawing.Point(459, 46);
            this.newCustomerBtn.Name = "newCustomerBtn";
            this.newCustomerBtn.Size = new System.Drawing.Size(74, 32);
            this.newCustomerBtn.TabIndex = 46;
            this.newCustomerBtn.Text = "Tạo mới";
            this.newCustomerBtn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.textBox4);
            this.info_gb.Controls.Add(this.label13);
            this.info_gb.Controls.Add(this.textBox3);
            this.info_gb.Controls.Add(this.label12);
            this.info_gb.Controls.Add(this.textBox2);
            this.info_gb.Controls.Add(this.label11);
            this.info_gb.Controls.Add(this.textBox1);
            this.info_gb.Controls.Add(this.label6);
            this.info_gb.Controls.Add(this.processDate_dtp);
            this.info_gb.Controls.Add(this.waterAmount_tb);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Controls.Add(this.daysAfter_tb);
            this.info_gb.Controls.Add(this.label10);
            this.info_gb.Controls.Add(this.label9);
            this.info_gb.Controls.Add(this.sku_CBB);
            this.info_gb.Controls.Add(this.label8);
            this.info_gb.Controls.Add(this.baseDateType_CBB);
            this.info_gb.Controls.Add(this.congViec_CBB);
            this.info_gb.Controls.Add(this.label3);
            this.info_gb.Controls.Add(this.label5);
            this.info_gb.Controls.Add(this.materialQuantity_tb);
            this.info_gb.Controls.Add(this.vatTu_CB);
            this.info_gb.Controls.Add(this.label7);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info_gb.Location = new System.Drawing.Point(17, 74);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(513, 402);
            this.info_gb.TabIndex = 43;
            this.info_gb.TabStop = false;
            // 
            // waterAmount_tb
            // 
            this.waterAmount_tb.Location = new System.Drawing.Point(163, 367);
            this.waterAmount_tb.Name = "waterAmount_tb";
            this.waterAmount_tb.Size = new System.Drawing.Size(98, 23);
            this.waterAmount_tb.TabIndex = 89;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(76, 366);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 88;
            this.label2.Text = "Lượng Nước:";
            // 
            // daysAfter_tb
            // 
            this.daysAfter_tb.Location = new System.Drawing.Point(163, 336);
            this.daysAfter_tb.Name = "daysAfter_tb";
            this.daysAfter_tb.Size = new System.Drawing.Size(157, 23);
            this.daysAfter_tb.TabIndex = 87;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(75, 335);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(81, 16);
            this.label10.TabIndex = 86;
            this.label10.Text = "Vị Trí Trồng:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(80, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 16);
            this.label9.TabIndex = 84;
            this.label9.Text = "Ngày Xử Lý:";
            // 
            // sku_CBB
            // 
            this.sku_CBB.FormattingEnabled = true;
            this.sku_CBB.IntegralHeight = false;
            this.sku_CBB.Location = new System.Drawing.Point(163, 241);
            this.sku_CBB.Name = "sku_CBB";
            this.sku_CBB.Size = new System.Drawing.Size(251, 24);
            this.sku_CBB.TabIndex = 83;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(49, 242);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 16);
            this.label8.TabIndex = 82;
            this.label8.Text = "Người Thực Hiện:";
            // 
            // baseDateType_CBB
            // 
            this.baseDateType_CBB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.baseDateType_CBB.FormattingEnabled = true;
            this.baseDateType_CBB.IntegralHeight = false;
            this.baseDateType_CBB.Location = new System.Drawing.Point(163, 304);
            this.baseDateType_CBB.Name = "baseDateType_CBB";
            this.baseDateType_CBB.Size = new System.Drawing.Size(188, 24);
            this.baseDateType_CBB.TabIndex = 81;
            // 
            // congViec_CBB
            // 
            this.congViec_CBB.FormattingEnabled = true;
            this.congViec_CBB.IntegralHeight = false;
            this.congViec_CBB.Location = new System.Drawing.Point(163, 53);
            this.congViec_CBB.Name = "congViec_CBB";
            this.congViec_CBB.Size = new System.Drawing.Size(157, 24);
            this.congViec_CBB.TabIndex = 76;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(87, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 16);
            this.label3.TabIndex = 75;
            this.label3.Text = "Công Việc:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(67, 304);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 16);
            this.label5.TabIndex = 73;
            this.label5.Text = "Tổ Phụ Trách:";
            // 
            // materialQuantity_tb
            // 
            this.materialQuantity_tb.Location = new System.Drawing.Point(163, 117);
            this.materialQuantity_tb.Name = "materialQuantity_tb";
            this.materialQuantity_tb.Size = new System.Drawing.Size(98, 23);
            this.materialQuantity_tb.TabIndex = 68;
            // 
            // vatTu_CB
            // 
            this.vatTu_CB.FormattingEnabled = true;
            this.vatTu_CB.IntegralHeight = false;
            this.vatTu_CB.Location = new System.Drawing.Point(163, 85);
            this.vatTu_CB.Name = "vatTu_CB";
            this.vatTu_CB.Size = new System.Drawing.Size(283, 24);
            this.vatTu_CB.TabIndex = 67;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(105, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 16);
            this.label7.TabIndex = 53;
            this.label7.Text = "Vật Tư:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 16);
            this.label1.TabIndex = 47;
            this.label1.Text = "Số Lượng Vật Tư:";
            // 
            // id_tb
            // 
            this.id_tb.Location = new System.Drawing.Point(372, 0);
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
            this.status_lb.Location = new System.Drawing.Point(4, 491);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(59, 23);
            this.status_lb.TabIndex = 40;
            this.status_lb.Text = "status";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(254, 482);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(104, 43);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dataGV);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(816, 758);
            this.panel3.TabIndex = 71;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 43);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(816, 715);
            this.dataGV.TabIndex = 72;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.plant_lb);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(816, 43);
            this.panel2.TabIndex = 0;
            // 
            // plant_lb
            // 
            this.plant_lb.BackColor = System.Drawing.Color.AntiqueWhite;
            this.plant_lb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plant_lb.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plant_lb.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.plant_lb.Location = new System.Drawing.Point(0, 0);
            this.plant_lb.Name = "plant_lb";
            this.plant_lb.Size = new System.Drawing.Size(816, 43);
            this.plant_lb.TabIndex = 83;
            this.plant_lb.Text = "Cải Bẹ Xanh - Thổ Canh";
            this.plant_lb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoadDefaultData_btn
            // 
            this.LoadDefaultData_btn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LoadDefaultData_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadDefaultData_btn.Location = new System.Drawing.Point(17, 46);
            this.LoadDefaultData_btn.Name = "LoadDefaultData_btn";
            this.LoadDefaultData_btn.Size = new System.Drawing.Size(183, 32);
            this.LoadDefaultData_btn.TabIndex = 71;
            this.LoadDefaultData_btn.Text = "Tạo Data Mặc Định";
            this.LoadDefaultData_btn.UseVisualStyleBackColor = false;
            // 
            // processDate_dtp
            // 
            this.processDate_dtp.Location = new System.Drawing.Point(163, 22);
            this.processDate_dtp.Name = "processDate_dtp";
            this.processDate_dtp.Size = new System.Drawing.Size(107, 23);
            this.processDate_dtp.TabIndex = 90;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(163, 148);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(157, 23);
            this.textBox1.TabIndex = 92;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(82, 149);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 16);
            this.label6.TabIndex = 91;
            this.label6.Text = "Liều Lượng:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(163, 179);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(318, 23);
            this.textBox2.TabIndex = 94;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(56, 180);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 16);
            this.label11.TabIndex = 93;
            this.label11.Text = "Tình Trạng Cây:";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(163, 210);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(318, 23);
            this.textBox3.TabIndex = 96;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 211);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(149, 16);
            this.label12.TabIndex = 95;
            this.label12.Text = "Thành Phần / Hoạt Chất:";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(163, 273);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(61, 23);
            this.textBox4.TabIndex = 98;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(49, 273);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(107, 16);
            this.label13.TabIndex = 97;
            this.label13.Text = "Số Ngày Cách Ly:";
            // 
            // KhoVatTu_CultivationProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1349, 758);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "KhoVatTu_CultivationProcess";
            this.Text = "Giám Sát Nhật Ký Sản Xuất";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).EndInit();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button readOnly_btn;
        private System.Windows.Forms.Button edit_btn;
        private System.Windows.Forms.Button newCustomerBtn;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox id_tb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.ComboBox vatTu_CB;
        private System.Windows.Forms.TextBox materialQuantity_tb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView log_GV;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox congViec_CBB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox baseDateType_CBB;
        private System.Windows.Forms.TextBox daysAfter_tb;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox sku_CBB;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox waterAmount_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label plant_lb;
        private System.Windows.Forms.Button LoadDefaultData_btn;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker processDate_dtp;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label13;
    }
}