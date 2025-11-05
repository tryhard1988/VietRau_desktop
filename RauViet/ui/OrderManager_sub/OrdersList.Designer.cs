
namespace RauViet.ui
{
    partial class OrdersList
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
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.exportExcel_TD_btn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sortByPri_Pro_btn = new System.Windows.Forms.Button();
            this.sortBypro_Cus_btn = new System.Windows.Forms.Button();
            this.sortByCus_pro_btn = new System.Windows.Forms.Button();
            this.sortByCus_pri_btn = new System.Windows.Forms.Button();
            this.status_lb = new System.Windows.Forms.Label();
            this.readOnly_btn = new System.Windows.Forms.Button();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.edit_btn = new System.Windows.Forms.Button();
            this.preview_print_TD_btn = new System.Windows.Forms.Button();
            this.prewiew_print_DSDH_btn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.customer_ccb = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.PCSOther_tb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.packing_ccb = new System.Windows.Forms.ComboBox();
            this.netWeight_tb = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.product_ccb = new System.Windows.Forms.ComboBox();
            this.printPendingOrderSummary_btn = new System.Windows.Forms.Button();
            this.print_order_list_btn = new System.Windows.Forms.Button();
            this.exportCode_search_cbb = new System.Windows.Forms.ComboBox();
            this.delete_btn = new System.Windows.Forms.Button();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.priceCNF_tb = new System.Windows.Forms.TextBox();
            this.orderId_tb = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.info_gb.SuspendLayout();
            this.SuspendLayout();
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
            this.dataGV.Size = new System.Drawing.Size(596, 681);
            this.dataGV.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.exportExcel_TD_btn);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.readOnly_btn);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.edit_btn);
            this.panel1.Controls.Add(this.preview_print_TD_btn);
            this.panel1.Controls.Add(this.prewiew_print_DSDH_btn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.printPendingOrderSummary_btn);
            this.panel1.Controls.Add(this.print_order_list_btn);
            this.panel1.Controls.Add(this.exportCode_search_cbb);
            this.panel1.Controls.Add(this.delete_btn);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.priceCNF_tb);
            this.panel1.Controls.Add(this.orderId_tb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(596, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(668, 681);
            this.panel1.TabIndex = 12;
            // 
            // exportExcel_TD_btn
            // 
            this.exportExcel_TD_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.exportExcel_TD_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportExcel_TD_btn.Location = new System.Drawing.Point(292, 423);
            this.exportExcel_TD_btn.Name = "exportExcel_TD_btn";
            this.exportExcel_TD_btn.Size = new System.Drawing.Size(145, 39);
            this.exportExcel_TD_btn.TabIndex = 67;
            this.exportExcel_TD_btn.Text = "Xuất Excel";
            this.exportExcel_TD_btn.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sortByPri_Pro_btn);
            this.groupBox1.Controls.Add(this.sortBypro_Cus_btn);
            this.groupBox1.Controls.Add(this.sortByCus_pro_btn);
            this.groupBox1.Controls.Add(this.sortByCus_pri_btn);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.groupBox1.Location = new System.Drawing.Point(10, 461);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox1.Size = new System.Drawing.Size(426, 164);
            this.groupBox1.TabIndex = 66;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sắp Xếp";
            // 
            // sortByPri_Pro_btn
            // 
            this.sortByPri_Pro_btn.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.sortByPri_Pro_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sortByPri_Pro_btn.ForeColor = System.Drawing.SystemColors.Highlight;
            this.sortByPri_Pro_btn.Location = new System.Drawing.Point(225, 24);
            this.sortByPri_Pro_btn.Name = "sortByPri_Pro_btn";
            this.sortByPri_Pro_btn.Size = new System.Drawing.Size(195, 41);
            this.sortByPri_Pro_btn.TabIndex = 68;
            this.sortByPri_Pro_btn.Text = "*[Ưu Tiên, Tên SP]*\r\n";
            this.sortByPri_Pro_btn.UseVisualStyleBackColor = false;
            // 
            // sortBypro_Cus_btn
            // 
            this.sortBypro_Cus_btn.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.sortBypro_Cus_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sortBypro_Cus_btn.Location = new System.Drawing.Point(28, 118);
            this.sortBypro_Cus_btn.Name = "sortBypro_Cus_btn";
            this.sortBypro_Cus_btn.Size = new System.Drawing.Size(195, 41);
            this.sortBypro_Cus_btn.TabIndex = 67;
            this.sortBypro_Cus_btn.Text = "[Tên SP, Khách Hàng]";
            this.sortBypro_Cus_btn.UseVisualStyleBackColor = false;
            // 
            // sortByCus_pro_btn
            // 
            this.sortByCus_pro_btn.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.sortByCus_pro_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sortByCus_pro_btn.Location = new System.Drawing.Point(28, 71);
            this.sortByCus_pro_btn.Name = "sortByCus_pro_btn";
            this.sortByCus_pro_btn.Size = new System.Drawing.Size(195, 41);
            this.sortByCus_pro_btn.TabIndex = 66;
            this.sortByCus_pro_btn.Text = "[Khách Hàng, Tên SP]";
            this.sortByCus_pro_btn.UseVisualStyleBackColor = false;
            // 
            // sortByCus_pri_btn
            // 
            this.sortByCus_pri_btn.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.sortByCus_pri_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sortByCus_pri_btn.Location = new System.Drawing.Point(28, 24);
            this.sortByCus_pri_btn.Name = "sortByCus_pri_btn";
            this.sortByCus_pri_btn.Size = new System.Drawing.Size(195, 41);
            this.sortByCus_pri_btn.TabIndex = 65;
            this.sortByCus_pri_btn.Text = "[Khách Hàng, Ưu Tiên]";
            this.sortByCus_pri_btn.UseVisualStyleBackColor = false;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(6, 339);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(57, 24);
            this.status_lb.TabIndex = 48;
            this.status_lb.Text = "Email";
            // 
            // readOnly_btn
            // 
            this.readOnly_btn.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.readOnly_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readOnly_btn.Location = new System.Drawing.Point(394, 78);
            this.readOnly_btn.Name = "readOnly_btn";
            this.readOnly_btn.Size = new System.Drawing.Size(42, 32);
            this.readOnly_btn.TabIndex = 47;
            this.readOnly_btn.Text = "X";
            this.readOnly_btn.UseVisualStyleBackColor = false;
            // 
            // newCustomerBtn
            // 
            this.newCustomerBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newCustomerBtn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCustomerBtn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.newCustomerBtn.Location = new System.Drawing.Point(361, 78);
            this.newCustomerBtn.Name = "newCustomerBtn";
            this.newCustomerBtn.Size = new System.Drawing.Size(75, 32);
            this.newCustomerBtn.TabIndex = 46;
            this.newCustomerBtn.Text = "Tạo mới";
            this.newCustomerBtn.UseVisualStyleBackColor = false;
            // 
            // edit_btn
            // 
            this.edit_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.edit_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_btn.Location = new System.Drawing.Point(261, 78);
            this.edit_btn.Name = "edit_btn";
            this.edit_btn.Size = new System.Drawing.Size(94, 32);
            this.edit_btn.TabIndex = 44;
            this.edit_btn.Text = "Chỉnh sửa";
            this.edit_btn.UseVisualStyleBackColor = false;
            // 
            // preview_print_TD_btn
            // 
            this.preview_print_TD_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.preview_print_TD_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.preview_print_TD_btn.Location = new System.Drawing.Point(291, 381);
            this.preview_print_TD_btn.Name = "preview_print_TD_btn";
            this.preview_print_TD_btn.Size = new System.Drawing.Size(145, 39);
            this.preview_print_TD_btn.TabIndex = 42;
            this.preview_print_TD_btn.Text = "Xem Trước Bản In";
            this.preview_print_TD_btn.UseVisualStyleBackColor = false;
            // 
            // prewiew_print_DSDH_btn
            // 
            this.prewiew_print_DSDH_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.prewiew_print_DSDH_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prewiew_print_DSDH_btn.Location = new System.Drawing.Point(135, 381);
            this.prewiew_print_DSDH_btn.Name = "prewiew_print_DSDH_btn";
            this.prewiew_print_DSDH_btn.Size = new System.Drawing.Size(145, 39);
            this.prewiew_print_DSDH_btn.TabIndex = 41;
            this.prewiew_print_DSDH_btn.Text = "Xem Trước Bản In";
            this.prewiew_print_DSDH_btn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.customer_ccb);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Controls.Add(this.label3);
            this.info_gb.Controls.Add(this.label5);
            this.info_gb.Controls.Add(this.PCSOther_tb);
            this.info_gb.Controls.Add(this.label6);
            this.info_gb.Controls.Add(this.packing_ccb);
            this.info_gb.Controls.Add(this.netWeight_tb);
            this.info_gb.Controls.Add(this.label7);
            this.info_gb.Controls.Add(this.product_ccb);
            this.info_gb.Location = new System.Drawing.Point(17, 105);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(419, 221);
            this.info_gb.TabIndex = 40;
            this.info_gb.TabStop = false;
            // 
            // customer_ccb
            // 
            this.customer_ccb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customer_ccb.FormattingEnabled = true;
            this.customer_ccb.Location = new System.Drawing.Point(112, 26);
            this.customer_ccb.Name = "customer_ccb";
            this.customer_ccb.Size = new System.Drawing.Size(203, 24);
            this.customer_ccb.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Tên Khách hàng:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Tên Sản Phẩm:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(7, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "PCS Other:";
            // 
            // PCSOther_tb
            // 
            this.PCSOther_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PCSOther_tb.Location = new System.Drawing.Point(112, 137);
            this.PCSOther_tb.Name = "PCSOther_tb";
            this.PCSOther_tb.Size = new System.Drawing.Size(93, 23);
            this.PCSOther_tb.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(7, 172);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(103, 16);
            this.label6.TabIndex = 28;
            this.label6.Text = "Net Weight (Kg):";
            // 
            // packing_ccb
            // 
            this.packing_ccb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.packing_ccb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.packing_ccb.FormattingEnabled = true;
            this.packing_ccb.Location = new System.Drawing.Point(112, 99);
            this.packing_ccb.Name = "packing_ccb";
            this.packing_ccb.Size = new System.Drawing.Size(93, 24);
            this.packing_ccb.TabIndex = 33;
            // 
            // netWeight_tb
            // 
            this.netWeight_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.netWeight_tb.Location = new System.Drawing.Point(112, 173);
            this.netWeight_tb.Name = "netWeight_tb";
            this.netWeight_tb.Size = new System.Drawing.Size(93, 23);
            this.netWeight_tb.TabIndex = 29;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(7, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 16);
            this.label7.TabIndex = 32;
            this.label7.Text = "Quy Cách:";
            // 
            // product_ccb
            // 
            this.product_ccb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.product_ccb.FormattingEnabled = true;
            this.product_ccb.IntegralHeight = false;
            this.product_ccb.Location = new System.Drawing.Point(112, 63);
            this.product_ccb.Name = "product_ccb";
            this.product_ccb.Size = new System.Drawing.Size(301, 24);
            this.product_ccb.TabIndex = 31;
            // 
            // printPendingOrderSummary_btn
            // 
            this.printPendingOrderSummary_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.printPendingOrderSummary_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printPendingOrderSummary_btn.Location = new System.Drawing.Point(291, 339);
            this.printPendingOrderSummary_btn.Name = "printPendingOrderSummary_btn";
            this.printPendingOrderSummary_btn.Size = new System.Drawing.Size(145, 39);
            this.printPendingOrderSummary_btn.TabIndex = 39;
            this.printPendingOrderSummary_btn.Text = "In Tổng Đơn";
            this.printPendingOrderSummary_btn.UseVisualStyleBackColor = false;
            // 
            // print_order_list_btn
            // 
            this.print_order_list_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.print_order_list_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.print_order_list_btn.Location = new System.Drawing.Point(135, 339);
            this.print_order_list_btn.Name = "print_order_list_btn";
            this.print_order_list_btn.Size = new System.Drawing.Size(145, 39);
            this.print_order_list_btn.TabIndex = 38;
            this.print_order_list_btn.Text = "In DS Đơn hàng";
            this.print_order_list_btn.UseVisualStyleBackColor = false;
            // 
            // exportCode_search_cbb
            // 
            this.exportCode_search_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exportCode_search_cbb.FormattingEnabled = true;
            this.exportCode_search_cbb.Location = new System.Drawing.Point(0, 3);
            this.exportCode_search_cbb.Name = "exportCode_search_cbb";
            this.exportCode_search_cbb.Size = new System.Drawing.Size(153, 21);
            this.exportCode_search_cbb.TabIndex = 37;
            // 
            // delete_btn
            // 
            this.delete_btn.BackColor = System.Drawing.Color.Red;
            this.delete_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delete_btn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.delete_btn.Location = new System.Drawing.Point(248, 332);
            this.delete_btn.Name = "delete_btn";
            this.delete_btn.Size = new System.Drawing.Size(99, 38);
            this.delete_btn.TabIndex = 27;
            this.delete_btn.Text = "Xóa";
            this.delete_btn.UseVisualStyleBackColor = false;
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(143, 332);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(99, 38);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // priceCNF_tb
            // 
            this.priceCNF_tb.Location = new System.Drawing.Point(17, 649);
            this.priceCNF_tb.Name = "priceCNF_tb";
            this.priceCNF_tb.ReadOnly = true;
            this.priceCNF_tb.Size = new System.Drawing.Size(56, 20);
            this.priceCNF_tb.TabIndex = 19;
            this.priceCNF_tb.Visible = false;
            // 
            // orderId_tb
            // 
            this.orderId_tb.Location = new System.Drawing.Point(88, 649);
            this.orderId_tb.Name = "orderId_tb";
            this.orderId_tb.ReadOnly = true;
            this.orderId_tb.Size = new System.Drawing.Size(32, 20);
            this.orderId_tb.TabIndex = 16;
            this.orderId_tb.Visible = false;
            // 
            // OrdersList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGV);
            this.Name = "OrdersList";
            this.Text = "FormTableData";
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button exportExcel_TD_btn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button sortByPri_Pro_btn;
        private System.Windows.Forms.Button sortBypro_Cus_btn;
        private System.Windows.Forms.Button sortByCus_pro_btn;
        private System.Windows.Forms.Button sortByCus_pri_btn;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Button readOnly_btn;
        private System.Windows.Forms.Button newCustomerBtn;
        private System.Windows.Forms.Button edit_btn;
        private System.Windows.Forms.Button preview_print_TD_btn;
        private System.Windows.Forms.Button prewiew_print_DSDH_btn;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.ComboBox customer_ccb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox PCSOther_tb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox packing_ccb;
        private System.Windows.Forms.TextBox netWeight_tb;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox product_ccb;
        private System.Windows.Forms.Button printPendingOrderSummary_btn;
        private System.Windows.Forms.Button print_order_list_btn;
        private System.Windows.Forms.ComboBox exportCode_search_cbb;
        private System.Windows.Forms.Button delete_btn;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.TextBox priceCNF_tb;
        private System.Windows.Forms.TextBox orderId_tb;
    }
}