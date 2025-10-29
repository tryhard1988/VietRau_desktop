
namespace RauViet.ui
{
    partial class AllowanceType
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
            this.readOnly_btn = new System.Windows.Forms.Button();
            this.edit_btn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.allowanceCode_tb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.isActive_cb = new System.Windows.Forms.CheckBox();
            this.ScopeName_cbb = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.isInsuranceIncluded_cb = new System.Windows.Forms.CheckBox();
            this.allowanceName_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.delete_btn = new System.Windows.Forms.Button();
            this.loading_lb = new System.Windows.Forms.Label();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.allowanceTypeID_tb = new System.Windows.Forms.TextBox();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.readOnly_btn);
            this.panel1.Controls.Add(this.edit_btn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.delete_btn);
            this.panel1.Controls.Add(this.loading_lb);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.allowanceTypeID_tb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(811, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(453, 724);
            this.panel1.TabIndex = 9;
            // 
            // readOnly_btn
            // 
            this.readOnly_btn.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.readOnly_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readOnly_btn.Location = new System.Drawing.Point(396, 192);
            this.readOnly_btn.Name = "readOnly_btn";
            this.readOnly_btn.Size = new System.Drawing.Size(42, 32);
            this.readOnly_btn.TabIndex = 30;
            this.readOnly_btn.Text = "X";
            this.readOnly_btn.UseVisualStyleBackColor = false;
            // 
            // edit_btn
            // 
            this.edit_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.edit_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_btn.Location = new System.Drawing.Point(256, 192);
            this.edit_btn.Name = "edit_btn";
            this.edit_btn.Size = new System.Drawing.Size(94, 32);
            this.edit_btn.TabIndex = 29;
            this.edit_btn.Text = "Chỉnh sửa";
            this.edit_btn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.allowanceCode_tb);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Controls.Add(this.isActive_cb);
            this.info_gb.Controls.Add(this.ScopeName_cbb);
            this.info_gb.Controls.Add(this.label5);
            this.info_gb.Controls.Add(this.isInsuranceIncluded_cb);
            this.info_gb.Controls.Add(this.allowanceName_tb);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Location = new System.Drawing.Point(16, 223);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(422, 210);
            this.info_gb.TabIndex = 28;
            this.info_gb.TabStop = false;
            // 
            // allowanceCode_tb
            // 
            this.allowanceCode_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.allowanceCode_tb.Location = new System.Drawing.Point(136, 75);
            this.allowanceCode_tb.Name = "allowanceCode_tb";
            this.allowanceCode_tb.Size = new System.Drawing.Size(120, 23);
            this.allowanceCode_tb.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(51, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 44;
            this.label1.Text = "Mã Phụ Cấp:";
            // 
            // isActive_cb
            // 
            this.isActive_cb.AutoSize = true;
            this.isActive_cb.Location = new System.Drawing.Point(136, 145);
            this.isActive_cb.Name = "isActive_cb";
            this.isActive_cb.Size = new System.Drawing.Size(107, 17);
            this.isActive_cb.TabIndex = 43;
            this.isActive_cb.Text = "Đang Hoạt Động";
            this.isActive_cb.UseVisualStyleBackColor = true;
            // 
            // ScopeName_cbb
            // 
            this.ScopeName_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ScopeName_cbb.FormattingEnabled = true;
            this.ScopeName_cbb.Location = new System.Drawing.Point(136, 109);
            this.ScopeName_cbb.Name = "ScopeName_cbb";
            this.ScopeName_cbb.Size = new System.Drawing.Size(172, 21);
            this.ScopeName_cbb.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(36, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 16);
            this.label5.TabIndex = 24;
            this.label5.Text = "Nhóm Áp Dụng:";
            // 
            // isInsuranceIncluded_cb
            // 
            this.isInsuranceIncluded_cb.AutoSize = true;
            this.isInsuranceIncluded_cb.Location = new System.Drawing.Point(136, 177);
            this.isInsuranceIncluded_cb.Name = "isInsuranceIncluded_cb";
            this.isInsuranceIncluded_cb.Size = new System.Drawing.Size(172, 17);
            this.isInsuranceIncluded_cb.TabIndex = 21;
            this.isInsuranceIncluded_cb.Text = "Có đóng Bảo Hiểm Hay Không";
            this.isInsuranceIncluded_cb.UseVisualStyleBackColor = true;
            // 
            // allowanceName_tb
            // 
            this.allowanceName_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.allowanceName_tb.Location = new System.Drawing.Point(136, 38);
            this.allowanceName_tb.Name = "allowanceName_tb";
            this.allowanceName_tb.Size = new System.Drawing.Size(246, 23);
            this.allowanceName_tb.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Tên Loại Phụ Cấp:";
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(12, 450);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // delete_btn
            // 
            this.delete_btn.BackColor = System.Drawing.Color.Red;
            this.delete_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delete_btn.Location = new System.Drawing.Point(263, 439);
            this.delete_btn.Name = "delete_btn";
            this.delete_btn.Size = new System.Drawing.Size(113, 47);
            this.delete_btn.TabIndex = 27;
            this.delete_btn.Text = "Xóa";
            this.delete_btn.UseVisualStyleBackColor = false;
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
            // newCustomerBtn
            // 
            this.newCustomerBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newCustomerBtn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCustomerBtn.Location = new System.Drawing.Point(353, 192);
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
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(144, 439);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(113, 47);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // allowanceTypeID_tb
            // 
            this.allowanceTypeID_tb.Location = new System.Drawing.Point(144, 635);
            this.allowanceTypeID_tb.Name = "allowanceTypeID_tb";
            this.allowanceTypeID_tb.ReadOnly = true;
            this.allowanceTypeID_tb.Size = new System.Drawing.Size(32, 20);
            this.allowanceTypeID_tb.TabIndex = 16;
            this.allowanceTypeID_tb.Visible = false;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 0);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(811, 724);
            this.dataGV.TabIndex = 10;
            // 
            // AllowanceType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 724);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Name = "AllowanceType";
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
        private System.Windows.Forms.TextBox allowanceName_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Button newCustomerBtn;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Label loading_lb;
        private System.Windows.Forms.Button delete_btn;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.CheckBox isInsuranceIncluded_cb;
        private System.Windows.Forms.TextBox allowanceTypeID_tb;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.CheckBox isActive_cb;
        private System.Windows.Forms.ComboBox ScopeName_cbb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox allowanceCode_tb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button readOnly_btn;
        private System.Windows.Forms.Button edit_btn;
    }
}