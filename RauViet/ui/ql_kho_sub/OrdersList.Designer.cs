
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.exportCode_cbb = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.packing_ccb = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.product_ccb = new System.Windows.Forms.ComboBox();
            this.customer_ccb = new System.Windows.Forms.ComboBox();
            this.netWeight_tb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.delete_btn = new System.Windows.Forms.Button();
            this.loading_lb = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.PCSOther_tb = new System.Windows.Forms.TextBox();
            this.priceCNF_tb = new System.Windows.Forms.TextBox();
            this.orderId_tb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.exportCode_search_cbb = new System.Windows.Forms.ComboBox();
            this.print_order_list_btn = new System.Windows.Forms.Button();
            this.printPendingOrderSummary_btn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.printPendingOrderSummary_btn);
            this.panel1.Controls.Add(this.print_order_list_btn);
            this.panel1.Controls.Add(this.exportCode_search_cbb);
            this.panel1.Controls.Add(this.exportCode_cbb);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.packing_ccb);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.product_ccb);
            this.panel1.Controls.Add(this.customer_ccb);
            this.panel1.Controls.Add(this.netWeight_tb);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.delete_btn);
            this.panel1.Controls.Add(this.loading_lb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.PCSOther_tb);
            this.panel1.Controls.Add(this.priceCNF_tb);
            this.panel1.Controls.Add(this.orderId_tb);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(862, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(402, 681);
            this.panel1.TabIndex = 9;
            // 
            // exportCode_cbb
            // 
            this.exportCode_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exportCode_cbb.FormattingEnabled = true;
            this.exportCode_cbb.Location = new System.Drawing.Point(131, 155);
            this.exportCode_cbb.Name = "exportCode_cbb";
            this.exportCode_cbb.Size = new System.Drawing.Size(267, 21);
            this.exportCode_cbb.TabIndex = 35;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(43, 160);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 34;
            this.label8.Text = "Mã Xuất Cảng";
            // 
            // packing_ccb
            // 
            this.packing_ccb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.packing_ccb.FormattingEnabled = true;
            this.packing_ccb.Location = new System.Drawing.Point(131, 254);
            this.packing_ccb.Name = "packing_ccb";
            this.packing_ccb.Size = new System.Drawing.Size(267, 21);
            this.packing_ccb.TabIndex = 33;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(43, 259);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "Quy Cách";
            // 
            // product_ccb
            // 
            this.product_ccb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.product_ccb.FormattingEnabled = true;
            this.product_ccb.Location = new System.Drawing.Point(131, 220);
            this.product_ccb.Name = "product_ccb";
            this.product_ccb.Size = new System.Drawing.Size(267, 21);
            this.product_ccb.TabIndex = 31;
            // 
            // customer_ccb
            // 
            this.customer_ccb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.customer_ccb.FormattingEnabled = true;
            this.customer_ccb.Location = new System.Drawing.Point(131, 194);
            this.customer_ccb.Name = "customer_ccb";
            this.customer_ccb.Size = new System.Drawing.Size(267, 21);
            this.customer_ccb.TabIndex = 30;
            // 
            // netWeight_tb
            // 
            this.netWeight_tb.Location = new System.Drawing.Point(131, 333);
            this.netWeight_tb.Name = "netWeight_tb";
            this.netWeight_tb.ReadOnly = true;
            this.netWeight_tb.Size = new System.Drawing.Size(267, 20);
            this.netWeight_tb.TabIndex = 29;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(43, 337);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Net Weight (Kg)";
            // 
            // delete_btn
            // 
            this.delete_btn.Location = new System.Drawing.Point(274, 436);
            this.delete_btn.Name = "delete_btn";
            this.delete_btn.Size = new System.Drawing.Size(124, 23);
            this.delete_btn.TabIndex = 27;
            this.delete_btn.Text = "Xóa";
            this.delete_btn.UseVisualStyleBackColor = true;
            // 
            // loading_lb
            // 
            this.loading_lb.AutoSize = true;
            this.loading_lb.Location = new System.Drawing.Point(161, 66);
            this.loading_lb.Name = "loading_lb";
            this.loading_lb.Size = new System.Drawing.Size(55, 13);
            this.loading_lb.TabIndex = 10;
            this.loading_lb.Text = "loading_lb";
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(49, 400);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(32, 13);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // newCustomerBtn
            // 
            this.newCustomerBtn.Location = new System.Drawing.Point(274, 92);
            this.newCustomerBtn.Name = "newCustomerBtn";
            this.newCustomerBtn.Size = new System.Drawing.Size(124, 23);
            this.newCustomerBtn.TabIndex = 25;
            this.newCustomerBtn.Text = "Tạo mới khách hàng";
            this.newCustomerBtn.UseVisualStyleBackColor = true;
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(52, 436);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(124, 23);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu Thay Đổi";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = true;
            // 
            // PCSOther_tb
            // 
            this.PCSOther_tb.Location = new System.Drawing.Point(131, 307);
            this.PCSOther_tb.Name = "PCSOther_tb";
            this.PCSOther_tb.Size = new System.Drawing.Size(267, 20);
            this.PCSOther_tb.TabIndex = 20;
            // 
            // priceCNF_tb
            // 
            this.priceCNF_tb.Location = new System.Drawing.Point(131, 281);
            this.priceCNF_tb.Name = "priceCNF_tb";
            this.priceCNF_tb.ReadOnly = true;
            this.priceCNF_tb.Size = new System.Drawing.Size(267, 20);
            this.priceCNF_tb.TabIndex = 19;
            // 
            // orderId_tb
            // 
            this.orderId_tb.Location = new System.Drawing.Point(131, 129);
            this.orderId_tb.Name = "orderId_tb";
            this.orderId_tb.ReadOnly = true;
            this.orderId_tb.Size = new System.Drawing.Size(267, 20);
            this.orderId_tb.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(43, 311);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "PCS Other";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 285);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Price CNF";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 225);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Tên Sản Phẩm";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Tên Khách hàng";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 133);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Mã Đơn Hàng";
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
            this.dataGV.Size = new System.Drawing.Size(862, 681);
            this.dataGV.TabIndex = 11;
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
            // print_order_list_btn
            // 
            this.print_order_list_btn.Location = new System.Drawing.Point(46, 513);
            this.print_order_list_btn.Name = "print_order_list_btn";
            this.print_order_list_btn.Size = new System.Drawing.Size(164, 23);
            this.print_order_list_btn.TabIndex = 38;
            this.print_order_list_btn.Text = "In Danh Sách Đơn hàng";
            this.print_order_list_btn.UseVisualStyleBackColor = true;
            // 
            // printPendingOrderSummary_btn
            // 
            this.printPendingOrderSummary_btn.Location = new System.Drawing.Point(216, 513);
            this.printPendingOrderSummary_btn.Name = "printPendingOrderSummary_btn";
            this.printPendingOrderSummary_btn.Size = new System.Drawing.Size(164, 23);
            this.printPendingOrderSummary_btn.TabIndex = 39;
            this.printPendingOrderSummary_btn.Text = "In Tổng Đơn";
            this.printPendingOrderSummary_btn.UseVisualStyleBackColor = true;
            // 
            // OrdersList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Name = "OrdersList";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox PCSOther_tb;
        private System.Windows.Forms.TextBox priceCNF_tb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Button newCustomerBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox orderId_tb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Label loading_lb;
        private System.Windows.Forms.Button delete_btn;
        private System.Windows.Forms.ComboBox packing_ccb;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox product_ccb;
        private System.Windows.Forms.ComboBox customer_ccb;
        private System.Windows.Forms.TextBox netWeight_tb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.ComboBox exportCode_cbb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox exportCode_search_cbb;
        private System.Windows.Forms.Button print_order_list_btn;
        private System.Windows.Forms.Button printPendingOrderSummary_btn;
    }
}