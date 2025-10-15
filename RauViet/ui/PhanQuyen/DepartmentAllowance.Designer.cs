
namespace RauViet.ui
{
    partial class DepartmentAllowance
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
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.note_tb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.allowanceType_cbb = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.amount_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.delete_btn = new System.Windows.Forms.Button();
            this.loading_lb = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.departmentAllowanceID_tb = new System.Windows.Forms.TextBox();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.allowanceGV = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowanceGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.delete_btn);
            this.panel1.Controls.Add(this.loading_lb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.departmentAllowanceID_tb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(811, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(453, 724);
            this.panel1.TabIndex = 9;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.note_tb);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Controls.Add(this.allowanceType_cbb);
            this.info_gb.Controls.Add(this.label5);
            this.info_gb.Controls.Add(this.amount_tb);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Location = new System.Drawing.Point(16, 223);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(422, 210);
            this.info_gb.TabIndex = 28;
            this.info_gb.TabStop = false;
            // 
            // note_tb
            // 
            this.note_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.note_tb.Location = new System.Drawing.Point(146, 93);
            this.note_tb.Multiline = true;
            this.note_tb.Name = "note_tb";
            this.note_tb.Size = new System.Drawing.Size(246, 54);
            this.note_tb.TabIndex = 27;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(73, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 26;
            this.label1.Text = "Ghi Chú:";
            // 
            // allowanceType_cbb
            // 
            this.allowanceType_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.allowanceType_cbb.FormattingEnabled = true;
            this.allowanceType_cbb.Location = new System.Drawing.Point(143, 19);
            this.allowanceType_cbb.Name = "allowanceType_cbb";
            this.allowanceType_cbb.Size = new System.Drawing.Size(256, 21);
            this.allowanceType_cbb.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(43, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 16);
            this.label5.TabIndex = 24;
            this.label5.Text = "Loại Phụ Cấp:";
            // 
            // amount_tb
            // 
            this.amount_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.amount_tb.Location = new System.Drawing.Point(143, 55);
            this.amount_tb.Name = "amount_tb";
            this.amount_tb.Size = new System.Drawing.Size(137, 23);
            this.amount_tb.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(77, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Số Tiền:";
            // 
            // delete_btn
            // 
            this.delete_btn.BackColor = System.Drawing.Color.Red;
            this.delete_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delete_btn.Location = new System.Drawing.Point(285, 439);
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
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(202, 197);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // newCustomerBtn
            // 
            this.newCustomerBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newCustomerBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCustomerBtn.Location = new System.Drawing.Point(159, 439);
            this.newCustomerBtn.Name = "newCustomerBtn";
            this.newCustomerBtn.Size = new System.Drawing.Size(113, 47);
            this.newCustomerBtn.TabIndex = 25;
            this.newCustomerBtn.Text = "Tạo mới";
            this.newCustomerBtn.UseVisualStyleBackColor = false;
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(36, 439);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(113, 47);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // departmentAllowanceID_tb
            // 
            this.departmentAllowanceID_tb.Location = new System.Drawing.Point(144, 635);
            this.departmentAllowanceID_tb.Name = "departmentAllowanceID_tb";
            this.departmentAllowanceID_tb.ReadOnly = true;
            this.departmentAllowanceID_tb.Size = new System.Drawing.Size(32, 20);
            this.departmentAllowanceID_tb.TabIndex = 16;
            this.departmentAllowanceID_tb.Visible = false;
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
            this.dataGV.Size = new System.Drawing.Size(428, 724);
            this.dataGV.TabIndex = 10;
            // 
            // allowanceGV
            // 
            this.allowanceGV.AllowUserToAddRows = false;
            this.allowanceGV.AllowUserToDeleteRows = false;
            this.allowanceGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.allowanceGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.allowanceGV.Location = new System.Drawing.Point(428, 0);
            this.allowanceGV.Name = "allowanceGV";
            this.allowanceGV.ReadOnly = true;
            this.allowanceGV.Size = new System.Drawing.Size(383, 724);
            this.allowanceGV.TabIndex = 11;
            // 
            // DepartmentAllowance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 724);
            this.Controls.Add(this.allowanceGV);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Name = "DepartmentAllowance";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowanceGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox amount_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Button newCustomerBtn;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Label loading_lb;
        private System.Windows.Forms.Button delete_btn;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.TextBox departmentAllowanceID_tb;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.ComboBox allowanceType_cbb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView allowanceGV;
        private System.Windows.Forms.TextBox note_tb;
        private System.Windows.Forms.Label label1;
    }
}