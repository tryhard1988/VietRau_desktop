
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cus_lable = new System.Windows.Forms.Label();
            this.cus_name_label = new System.Windows.Forms.Label();
            this.total_label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.search_tb = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.exportCode_search_cbb = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dddh_gb = new System.Windows.Forms.GroupBox();
            this.dsdd_in2mat_cb = new System.Windows.Forms.CheckBox();
            this.prewiew_print_DSDH_btn = new System.Windows.Forms.Button();
            this.print_order_list_btn = new System.Windows.Forms.Button();
            this.tongdon_gb = new System.Windows.Forms.GroupBox();
            this.tongdon_in2mat_cb = new System.Windows.Forms.CheckBox();
            this.printPendingOrderSummary_btn = new System.Windows.Forms.Button();
            this.preview_print_TD_btn = new System.Windows.Forms.Button();
            this.exportExcel_TD_btn = new System.Windows.Forms.Button();
            this.status_lb = new System.Windows.Forms.Label();
            this.Refesh_btn = new System.Windows.Forms.Button();
            this.readOnly_btn = new System.Windows.Forms.Button();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.edit_btn = new System.Windows.Forms.Button();
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
            this.delete_btn = new System.Windows.Forms.Button();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.priceCNF_tb = new System.Windows.Forms.TextBox();
            this.orderId_tb = new System.Windows.Forms.TextBox();
            this.cusProduct_GV = new System.Windows.Forms.DataGridView();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.dddh_gb.SuspendLayout();
            this.tongdon_gb.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cusProduct_GV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.dataGV);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(690, 681);
            this.panel2.TabIndex = 13;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 24);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(690, 637);
            this.dataGV.TabIndex = 76;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.cus_lable);
            this.panel4.Controls.Add(this.cus_name_label);
            this.panel4.Controls.Add(this.total_label);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 661);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(690, 20);
            this.panel4.TabIndex = 75;
            // 
            // cus_lable
            // 
            this.cus_lable.AutoSize = true;
            this.cus_lable.Dock = System.Windows.Forms.DockStyle.Left;
            this.cus_lable.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cus_lable.ForeColor = System.Drawing.Color.ForestGreen;
            this.cus_lable.Location = new System.Drawing.Point(115, 0);
            this.cus_lable.Name = "cus_lable";
            this.cus_lable.Size = new System.Drawing.Size(35, 16);
            this.cus_lable.TabIndex = 78;
            this.cus_lable.Text = "PCS:";
            // 
            // cus_name_label
            // 
            this.cus_name_label.AutoSize = true;
            this.cus_name_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.cus_name_label.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cus_name_label.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cus_name_label.Location = new System.Drawing.Point(79, 0);
            this.cus_name_label.Name = "cus_name_label";
            this.cus_name_label.Size = new System.Drawing.Size(36, 16);
            this.cus_name_label.TabIndex = 77;
            this.cus_name_label.Text = "PCS:";
            // 
            // total_label
            // 
            this.total_label.AutoSize = true;
            this.total_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.total_label.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total_label.ForeColor = System.Drawing.SystemColors.Highlight;
            this.total_label.Location = new System.Drawing.Point(44, 0);
            this.total_label.Name = "total_label";
            this.total_label.Size = new System.Drawing.Size(35, 16);
            this.total_label.TabIndex = 76;
            this.total_label.Text = "PCS:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 16);
            this.label1.TabIndex = 73;
            this.label1.Text = "Total:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.search_tb);
            this.panel3.Controls.Add(this.label15);
            this.panel3.Controls.Add(this.exportCode_search_cbb);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(690, 24);
            this.panel3.TabIndex = 73;
            // 
            // search_tb
            // 
            this.search_tb.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.search_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.search_tb.Location = new System.Drawing.Point(82, 1);
            this.search_tb.Name = "search_tb";
            this.search_tb.Size = new System.Drawing.Size(252, 23);
            this.search_tb.TabIndex = 65;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(10, 5);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(66, 16);
            this.label15.TabIndex = 66;
            this.label15.Text = "Tìm Kiếm:";
            // 
            // exportCode_search_cbb
            // 
            this.exportCode_search_cbb.BackColor = System.Drawing.Color.White;
            this.exportCode_search_cbb.Dock = System.Windows.Forms.DockStyle.Right;
            this.exportCode_search_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exportCode_search_cbb.FormattingEnabled = true;
            this.exportCode_search_cbb.Location = new System.Drawing.Point(522, 0);
            this.exportCode_search_cbb.Name = "exportCode_search_cbb";
            this.exportCode_search_cbb.Size = new System.Drawing.Size(168, 21);
            this.exportCode_search_cbb.TabIndex = 37;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dddh_gb);
            this.panel1.Controls.Add(this.tongdon_gb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.Refesh_btn);
            this.panel1.Controls.Add(this.readOnly_btn);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.edit_btn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.delete_btn);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.priceCNF_tb);
            this.panel1.Controls.Add(this.orderId_tb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(690, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(416, 681);
            this.panel1.TabIndex = 14;
            // 
            // dddh_gb
            // 
            this.dddh_gb.Controls.Add(this.dsdd_in2mat_cb);
            this.dddh_gb.Controls.Add(this.prewiew_print_DSDH_btn);
            this.dddh_gb.Controls.Add(this.print_order_list_btn);
            this.dddh_gb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dddh_gb.Location = new System.Drawing.Point(68, 348);
            this.dddh_gb.Name = "dddh_gb";
            this.dddh_gb.Size = new System.Drawing.Size(145, 110);
            this.dddh_gb.TabIndex = 70;
            this.dddh_gb.TabStop = false;
            this.dddh_gb.Text = "Danh Sách Đơn Hàng";
            // 
            // dsdd_in2mat_cb
            // 
            this.dsdd_in2mat_cb.AutoSize = true;
            this.dsdd_in2mat_cb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdd_in2mat_cb.Location = new System.Drawing.Point(74, 33);
            this.dsdd_in2mat_cb.Name = "dsdd_in2mat_cb";
            this.dsdd_in2mat_cb.Size = new System.Drawing.Size(65, 17);
            this.dsdd_in2mat_cb.TabIndex = 70;
            this.dsdd_in2mat_cb.Text = "In 2 Mặt";
            this.dsdd_in2mat_cb.UseVisualStyleBackColor = true;
            // 
            // prewiew_print_DSDH_btn
            // 
            this.prewiew_print_DSDH_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.prewiew_print_DSDH_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prewiew_print_DSDH_btn.Location = new System.Drawing.Point(6, 63);
            this.prewiew_print_DSDH_btn.Name = "prewiew_print_DSDH_btn";
            this.prewiew_print_DSDH_btn.Size = new System.Drawing.Size(133, 39);
            this.prewiew_print_DSDH_btn.TabIndex = 41;
            this.prewiew_print_DSDH_btn.Text = "Xem";
            this.prewiew_print_DSDH_btn.UseVisualStyleBackColor = false;
            // 
            // print_order_list_btn
            // 
            this.print_order_list_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.print_order_list_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.print_order_list_btn.Location = new System.Drawing.Point(6, 21);
            this.print_order_list_btn.Name = "print_order_list_btn";
            this.print_order_list_btn.Size = new System.Drawing.Size(62, 39);
            this.print_order_list_btn.TabIndex = 38;
            this.print_order_list_btn.Text = "In";
            this.print_order_list_btn.UseVisualStyleBackColor = false;
            // 
            // tongdon_gb
            // 
            this.tongdon_gb.Controls.Add(this.tongdon_in2mat_cb);
            this.tongdon_gb.Controls.Add(this.printPendingOrderSummary_btn);
            this.tongdon_gb.Controls.Add(this.preview_print_TD_btn);
            this.tongdon_gb.Controls.Add(this.exportExcel_TD_btn);
            this.tongdon_gb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tongdon_gb.Location = new System.Drawing.Point(229, 348);
            this.tongdon_gb.Name = "tongdon_gb";
            this.tongdon_gb.Size = new System.Drawing.Size(159, 152);
            this.tongdon_gb.TabIndex = 69;
            this.tongdon_gb.TabStop = false;
            this.tongdon_gb.Text = "Tổng Đơn";
            // 
            // tongdon_in2mat_cb
            // 
            this.tongdon_in2mat_cb.AutoSize = true;
            this.tongdon_in2mat_cb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tongdon_in2mat_cb.Location = new System.Drawing.Point(79, 31);
            this.tongdon_in2mat_cb.Name = "tongdon_in2mat_cb";
            this.tongdon_in2mat_cb.Size = new System.Drawing.Size(65, 17);
            this.tongdon_in2mat_cb.TabIndex = 69;
            this.tongdon_in2mat_cb.Text = "In 2 Mặt";
            this.tongdon_in2mat_cb.UseVisualStyleBackColor = true;
            // 
            // printPendingOrderSummary_btn
            // 
            this.printPendingOrderSummary_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.printPendingOrderSummary_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printPendingOrderSummary_btn.Location = new System.Drawing.Point(9, 19);
            this.printPendingOrderSummary_btn.Name = "printPendingOrderSummary_btn";
            this.printPendingOrderSummary_btn.Size = new System.Drawing.Size(64, 39);
            this.printPendingOrderSummary_btn.TabIndex = 39;
            this.printPendingOrderSummary_btn.Text = "In";
            this.printPendingOrderSummary_btn.UseVisualStyleBackColor = false;
            // 
            // preview_print_TD_btn
            // 
            this.preview_print_TD_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.preview_print_TD_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.preview_print_TD_btn.Location = new System.Drawing.Point(9, 61);
            this.preview_print_TD_btn.Name = "preview_print_TD_btn";
            this.preview_print_TD_btn.Size = new System.Drawing.Size(136, 39);
            this.preview_print_TD_btn.TabIndex = 42;
            this.preview_print_TD_btn.Text = "Xem";
            this.preview_print_TD_btn.UseVisualStyleBackColor = false;
            // 
            // exportExcel_TD_btn
            // 
            this.exportExcel_TD_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.exportExcel_TD_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportExcel_TD_btn.Location = new System.Drawing.Point(10, 103);
            this.exportExcel_TD_btn.Name = "exportExcel_TD_btn";
            this.exportExcel_TD_btn.Size = new System.Drawing.Size(136, 39);
            this.exportExcel_TD_btn.TabIndex = 67;
            this.exportExcel_TD_btn.Text = "Xuất Excel";
            this.exportExcel_TD_btn.UseVisualStyleBackColor = false;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(177, 24);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(93, 20);
            this.status_lb.TabIndex = 48;
            this.status_lb.Text = "Thành công";
            // 
            // Refesh_btn
            // 
            this.Refesh_btn.BackColor = System.Drawing.Color.Gold;
            this.Refesh_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Refesh_btn.Location = new System.Drawing.Point(17, 89);
            this.Refesh_btn.Name = "Refesh_btn";
            this.Refesh_btn.Size = new System.Drawing.Size(94, 32);
            this.Refesh_btn.TabIndex = 68;
            this.Refesh_btn.Text = "Refesh";
            this.Refesh_btn.UseVisualStyleBackColor = false;
            // 
            // readOnly_btn
            // 
            this.readOnly_btn.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.readOnly_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readOnly_btn.Location = new System.Drawing.Point(352, 89);
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
            this.newCustomerBtn.Location = new System.Drawing.Point(319, 89);
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
            this.edit_btn.Location = new System.Drawing.Point(219, 89);
            this.edit_btn.Name = "edit_btn";
            this.edit_btn.Size = new System.Drawing.Size(94, 32);
            this.edit_btn.TabIndex = 44;
            this.edit_btn.Text = "Chỉnh sửa";
            this.edit_btn.UseVisualStyleBackColor = false;
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
            this.info_gb.Location = new System.Drawing.Point(17, 121);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(387, 221);
            this.info_gb.TabIndex = 40;
            this.info_gb.TabStop = false;
            // 
            // customer_ccb
            // 
            this.customer_ccb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customer_ccb.FormattingEnabled = true;
            this.customer_ccb.Location = new System.Drawing.Point(112, 22);
            this.customer_ccb.Name = "customer_ccb";
            this.customer_ccb.Size = new System.Drawing.Size(203, 24);
            this.customer_ccb.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Tên Khách hàng:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Tên Sản Phẩm:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(7, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "PCS Other:";
            // 
            // PCSOther_tb
            // 
            this.PCSOther_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PCSOther_tb.Location = new System.Drawing.Point(112, 133);
            this.PCSOther_tb.Name = "PCSOther_tb";
            this.PCSOther_tb.Size = new System.Drawing.Size(93, 23);
            this.PCSOther_tb.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(7, 168);
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
            this.packing_ccb.Location = new System.Drawing.Point(112, 95);
            this.packing_ccb.Name = "packing_ccb";
            this.packing_ccb.Size = new System.Drawing.Size(93, 24);
            this.packing_ccb.TabIndex = 33;
            // 
            // netWeight_tb
            // 
            this.netWeight_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.netWeight_tb.Location = new System.Drawing.Point(112, 169);
            this.netWeight_tb.Name = "netWeight_tb";
            this.netWeight_tb.Size = new System.Drawing.Size(93, 23);
            this.netWeight_tb.TabIndex = 29;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(7, 95);
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
            this.product_ccb.Location = new System.Drawing.Point(112, 59);
            this.product_ccb.Name = "product_ccb";
            this.product_ccb.Size = new System.Drawing.Size(259, 24);
            this.product_ccb.TabIndex = 31;
            // 
            // delete_btn
            // 
            this.delete_btn.BackColor = System.Drawing.Color.Red;
            this.delete_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delete_btn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.delete_btn.Location = new System.Drawing.Point(248, 348);
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
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(143, 348);
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
            // cusProduct_GV
            // 
            this.cusProduct_GV.AllowUserToAddRows = false;
            this.cusProduct_GV.AllowUserToDeleteRows = false;
            this.cusProduct_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cusProduct_GV.Dock = System.Windows.Forms.DockStyle.Left;
            this.cusProduct_GV.Location = new System.Drawing.Point(1106, 0);
            this.cusProduct_GV.Name = "cusProduct_GV";
            this.cusProduct_GV.ReadOnly = true;
            this.cusProduct_GV.Size = new System.Drawing.Size(486, 681);
            this.cusProduct_GV.TabIndex = 79;
            // 
            // OrdersList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1542, 681);
            this.Controls.Add(this.cusProduct_GV);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "OrdersList";
            this.Text = "FormTableData";
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.dddh_gb.ResumeLayout(false);
            this.dddh_gb.PerformLayout();
            this.tongdon_gb.ResumeLayout(false);
            this.tongdon_gb.PerformLayout();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cusProduct_GV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Refesh_btn;
        private System.Windows.Forms.Button exportExcel_TD_btn;
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
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox search_tb;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label cus_lable;
        private System.Windows.Forms.Label cus_name_label;
        private System.Windows.Forms.Label total_label;
        private System.Windows.Forms.DataGridView cusProduct_GV;
        private System.Windows.Forms.GroupBox tongdon_gb;
        private System.Windows.Forms.CheckBox tongdon_in2mat_cb;
        private System.Windows.Forms.GroupBox dddh_gb;
        private System.Windows.Forms.CheckBox dsdd_in2mat_cb;
    }
}