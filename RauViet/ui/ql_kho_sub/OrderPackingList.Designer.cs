
namespace RauViet.ui
{
    partial class OrderPackingList
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.assignCustomerCarton_btn = new System.Windows.Forms.Button();
            this.assignCartonSize_btn = new System.Windows.Forms.Button();
            this.cartonSize_tb = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.assignPCSReal_btn = new System.Windows.Forms.Button();
            this.PCSReal_tb = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.assignCartonNoBtn = new System.Windows.Forms.Button();
            this.cartonNo_tb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.exportCode_cbb = new System.Windows.Forms.ComboBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.carton_GV = new System.Windows.Forms.DataGridView();
            this.fillter_btn = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.print_btn = new System.Windows.Forms.Button();
            this.loading_lb = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.autoEditCartonNo_btn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.carton_GV)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.exportCode_cbb);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.loading_lb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(834, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 681);
            this.panel1.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.groupBox1.Controls.Add(this.autoEditCartonNo_btn);
            this.groupBox1.Controls.Add(this.assignCustomerCarton_btn);
            this.groupBox1.Controls.Add(this.assignCartonSize_btn);
            this.groupBox1.Controls.Add(this.cartonSize_tb);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.assignPCSReal_btn);
            this.groupBox1.Controls.Add(this.PCSReal_tb);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.assignCartonNoBtn);
            this.groupBox1.Controls.Add(this.cartonNo_tb);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(19, 97);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(314, 328);
            this.groupBox1.TabIndex = 53;
            this.groupBox1.TabStop = false;
            // 
            // assignCustomerCarton_btn
            // 
            this.assignCustomerCarton_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.assignCustomerCarton_btn.Location = new System.Drawing.Point(53, 216);
            this.assignCustomerCarton_btn.Name = "assignCustomerCarton_btn";
            this.assignCustomerCarton_btn.Size = new System.Drawing.Size(174, 31);
            this.assignCustomerCarton_btn.TabIndex = 57;
            this.assignCustomerCarton_btn.Text = "Tự Động Tạo Mã Thùng";
            this.assignCustomerCarton_btn.UseVisualStyleBackColor = false;
            // 
            // assignCartonSize_btn
            // 
            this.assignCartonSize_btn.Location = new System.Drawing.Point(163, 109);
            this.assignCartonSize_btn.Name = "assignCartonSize_btn";
            this.assignCartonSize_btn.Size = new System.Drawing.Size(64, 30);
            this.assignCartonSize_btn.TabIndex = 56;
            this.assignCartonSize_btn.Text = "Assign";
            this.assignCartonSize_btn.UseVisualStyleBackColor = true;
            // 
            // cartonSize_tb
            // 
            this.cartonSize_tb.Location = new System.Drawing.Point(53, 113);
            this.cartonSize_tb.Name = "cartonSize_tb";
            this.cartonSize_tb.Size = new System.Drawing.Size(104, 23);
            this.cartonSize_tb.TabIndex = 55;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(27, 97);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 16);
            this.label8.TabIndex = 54;
            this.label8.Text = "Carton Size";
            // 
            // assignPCSReal_btn
            // 
            this.assignPCSReal_btn.Location = new System.Drawing.Point(163, 166);
            this.assignPCSReal_btn.Name = "assignPCSReal_btn";
            this.assignPCSReal_btn.Size = new System.Drawing.Size(64, 30);
            this.assignPCSReal_btn.TabIndex = 53;
            this.assignPCSReal_btn.Text = "Assign";
            this.assignPCSReal_btn.UseVisualStyleBackColor = true;
            // 
            // PCSReal_tb
            // 
            this.PCSReal_tb.Location = new System.Drawing.Point(53, 170);
            this.PCSReal_tb.Name = "PCSReal_tb";
            this.PCSReal_tb.Size = new System.Drawing.Size(104, 23);
            this.PCSReal_tb.TabIndex = 52;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 154);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 16);
            this.label7.TabIndex = 51;
            this.label7.Text = "PCS Thực Tế:";
            // 
            // assignCartonNoBtn
            // 
            this.assignCartonNoBtn.Location = new System.Drawing.Point(163, 49);
            this.assignCartonNoBtn.Name = "assignCartonNoBtn";
            this.assignCartonNoBtn.Size = new System.Drawing.Size(64, 30);
            this.assignCartonNoBtn.TabIndex = 50;
            this.assignCartonNoBtn.Text = "Assign";
            this.assignCartonNoBtn.UseVisualStyleBackColor = true;
            // 
            // cartonNo_tb
            // 
            this.cartonNo_tb.Location = new System.Drawing.Point(53, 53);
            this.cartonNo_tb.Name = "cartonNo_tb";
            this.cartonNo_tb.Size = new System.Drawing.Size(104, 23);
            this.cartonNo_tb.TabIndex = 49;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 16);
            this.label6.TabIndex = 48;
            this.label6.Text = "Carton No:";
            // 
            // exportCode_cbb
            // 
            this.exportCode_cbb.FormattingEnabled = true;
            this.exportCode_cbb.Location = new System.Drawing.Point(-3, 12);
            this.exportCode_cbb.Name = "exportCode_cbb";
            this.exportCode_cbb.Size = new System.Drawing.Size(160, 21);
            this.exportCode_cbb.TabIndex = 52;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.carton_GV);
            this.panel3.Controls.Add(this.fillter_btn);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(350, 13);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(162, 668);
            this.panel3.TabIndex = 51;
            // 
            // carton_GV
            // 
            this.carton_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.carton_GV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.carton_GV.Location = new System.Drawing.Point(0, 43);
            this.carton_GV.Name = "carton_GV";
            this.carton_GV.Size = new System.Drawing.Size(162, 470);
            this.carton_GV.TabIndex = 52;
            // 
            // fillter_btn
            // 
            this.fillter_btn.BackColor = System.Drawing.Color.AntiqueWhite;
            this.fillter_btn.Dock = System.Windows.Forms.DockStyle.Top;
            this.fillter_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fillter_btn.Location = new System.Drawing.Point(0, 0);
            this.fillter_btn.Name = "fillter_btn";
            this.fillter_btn.Size = new System.Drawing.Size(162, 43);
            this.fillter_btn.TabIndex = 46;
            this.fillter_btn.Text = "Lấy Danh Sách Thùng";
            this.fillter_btn.UseVisualStyleBackColor = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.print_btn);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 513);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(162, 155);
            this.panel4.TabIndex = 51;
            // 
            // print_btn
            // 
            this.print_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.print_btn.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.print_btn.Location = new System.Drawing.Point(0, 3);
            this.print_btn.Name = "print_btn";
            this.print_btn.Size = new System.Drawing.Size(162, 58);
            this.print_btn.TabIndex = 45;
            this.print_btn.Text = "Print";
            this.print_btn.UseVisualStyleBackColor = false;
            // 
            // loading_lb
            // 
            this.loading_lb.AutoSize = true;
            this.loading_lb.Dock = System.Windows.Forms.DockStyle.Top;
            this.loading_lb.Location = new System.Drawing.Point(0, 0);
            this.loading_lb.Name = "loading_lb";
            this.loading_lb.Size = new System.Drawing.Size(55, 13);
            this.loading_lb.TabIndex = 10;
            this.loading_lb.Text = "loading_lb";
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(148, 71);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(48, 431);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(208, 59);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu Thay Đổi";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
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
            this.dataGV.Size = new System.Drawing.Size(834, 681);
            this.dataGV.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // autoEditCartonNo_btn
            // 
            this.autoEditCartonNo_btn.BackColor = System.Drawing.Color.Lime;
            this.autoEditCartonNo_btn.Location = new System.Drawing.Point(230, 49);
            this.autoEditCartonNo_btn.Name = "autoEditCartonNo_btn";
            this.autoEditCartonNo_btn.Size = new System.Drawing.Size(69, 30);
            this.autoEditCartonNo_btn.TabIndex = 58;
            this.autoEditCartonNo_btn.Text = "Auto Edit";
            this.autoEditCartonNo_btn.UseVisualStyleBackColor = false;
            // 
            // OrderPackingList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1346, 681);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGV);
            this.Name = "OrderPackingList";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.carton_GV)).EndInit();
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Label loading_lb;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button fillter_btn;
        private System.Windows.Forms.Button print_btn;
        private System.Windows.Forms.DataGridView carton_GV;
        private System.Windows.Forms.ComboBox exportCode_cbb;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button assignCustomerCarton_btn;
        private System.Windows.Forms.Button assignCartonSize_btn;
        private System.Windows.Forms.TextBox cartonSize_tb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button assignPCSReal_btn;
        private System.Windows.Forms.TextBox PCSReal_tb;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button assignCartonNoBtn;
        private System.Windows.Forms.TextBox cartonNo_tb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button autoEditCartonNo_btn;
    }
}