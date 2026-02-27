
namespace RauViet.ui
{
    partial class KhoVatTu_CultivationProcessTemplate_Insert
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
            this.id_tb = new System.Windows.Forms.TextBox();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.open_btn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.open_btn);
            this.panel1.Controls.Add(this.id_tb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1150, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(199, 758);
            this.panel1.TabIndex = 69;
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
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(40, 198);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(104, 43);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            this.LuuThayDoiBtn.Click += new System.EventHandler(this.LuuThayDoiBtn_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dataGV);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1150, 758);
            this.panel3.TabIndex = 71;
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
            this.dataGV.Size = new System.Drawing.Size(1150, 758);
            this.dataGV.TabIndex = 71;
            // 
            // open_btn
            // 
            this.open_btn.BackColor = System.Drawing.SystemColors.Highlight;
            this.open_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open_btn.Location = new System.Drawing.Point(40, 125);
            this.open_btn.Name = "open_btn";
            this.open_btn.Size = new System.Drawing.Size(104, 43);
            this.open_btn.TabIndex = 43;
            this.open_btn.Text = "open";
            this.open_btn.UseVisualStyleBackColor = false;
            this.open_btn.Click += new System.EventHandler(this.open_btn_Click);
            // 
            // KhoVatTu_CultivationProcessTemplate_Insert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1349, 758);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Name = "KhoVatTu_CultivationProcessTemplate_Insert";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox id_tb;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Button open_btn;
    }
}