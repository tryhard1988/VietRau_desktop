namespace RauViet.ui.PhanQuyen
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.label1 = new System.Windows.Forms.Label();
            this.userName_tb = new System.Windows.Forms.TextBox();
            this.login_btn = new System.Windows.Forms.Button();
            this.password_tb = new System.Windows.Forms.TextBox();
            this.login_loading_tb = new System.Windows.Forms.Label();
            this.changePass_btn = new System.Windows.Forms.Button();
            this.login_panel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.changePass_panel = new System.Windows.Forms.Panel();
            this.back_btn = new System.Windows.Forms.Button();
            this.change_comfirm_tb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.change_new_pass_tb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.updatePass_btn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.change_pass_tb = new System.Windows.Forms.TextBox();
            this.change_user_tb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.login_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.changePass_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tên Đăng Nhập:";
            // 
            // userName_tb
            // 
            this.userName_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userName_tb.Location = new System.Drawing.Point(123, 83);
            this.userName_tb.Name = "userName_tb";
            this.userName_tb.Size = new System.Drawing.Size(162, 23);
            this.userName_tb.TabIndex = 1;
            // 
            // login_btn
            // 
            this.login_btn.BackColor = System.Drawing.SystemColors.HotTrack;
            this.login_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.login_btn.Location = new System.Drawing.Point(6, 164);
            this.login_btn.Name = "login_btn";
            this.login_btn.Size = new System.Drawing.Size(130, 50);
            this.login_btn.TabIndex = 2;
            this.login_btn.Text = "Đăng Nhập";
            this.login_btn.UseVisualStyleBackColor = false;
            // 
            // password_tb
            // 
            this.password_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.password_tb.Location = new System.Drawing.Point(123, 118);
            this.password_tb.Name = "password_tb";
            this.password_tb.PasswordChar = '*';
            this.password_tb.Size = new System.Drawing.Size(162, 23);
            this.password_tb.TabIndex = 4;
            // 
            // login_loading_tb
            // 
            this.login_loading_tb.AutoSize = true;
            this.login_loading_tb.BackColor = System.Drawing.Color.Linen;
            this.login_loading_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.login_loading_tb.ForeColor = System.Drawing.Color.Black;
            this.login_loading_tb.Location = new System.Drawing.Point(82, 218);
            this.login_loading_tb.Name = "login_loading_tb";
            this.login_loading_tb.Size = new System.Drawing.Size(104, 16);
            this.login_loading_tb.TabIndex = 3;
            this.login_loading_tb.Text = "Đang kiểm tra ...";
            // 
            // changePass_btn
            // 
            this.changePass_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.changePass_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changePass_btn.Location = new System.Drawing.Point(144, 164);
            this.changePass_btn.Name = "changePass_btn";
            this.changePass_btn.Size = new System.Drawing.Size(130, 50);
            this.changePass_btn.TabIndex = 6;
            this.changePass_btn.Text = "Đổi Mật Khẩu";
            this.changePass_btn.UseVisualStyleBackColor = false;
            // 
            // login_panel
            // 
            this.login_panel.BackColor = System.Drawing.Color.Transparent;
            this.login_panel.Controls.Add(this.label2);
            this.login_panel.Controls.Add(this.pictureBox1);
            this.login_panel.Controls.Add(this.changePass_btn);
            this.login_panel.Controls.Add(this.login_btn);
            this.login_panel.Controls.Add(this.label1);
            this.login_panel.Controls.Add(this.password_tb);
            this.login_panel.Controls.Add(this.userName_tb);
            this.login_panel.Controls.Add(this.login_loading_tb);
            this.login_panel.Location = new System.Drawing.Point(37, 12);
            this.login_panel.Name = "login_panel";
            this.login_panel.Size = new System.Drawing.Size(286, 311);
            this.login_panel.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(39, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Mật Khẩu:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = global::RauViet.Properties.Resources.logo_vr;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(60, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(178, 58);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // changePass_panel
            // 
            this.changePass_panel.BackColor = System.Drawing.Color.Transparent;
            this.changePass_panel.Controls.Add(this.back_btn);
            this.changePass_panel.Controls.Add(this.change_comfirm_tb);
            this.changePass_panel.Controls.Add(this.label6);
            this.changePass_panel.Controls.Add(this.change_new_pass_tb);
            this.changePass_panel.Controls.Add(this.label5);
            this.changePass_panel.Controls.Add(this.pictureBox2);
            this.changePass_panel.Controls.Add(this.updatePass_btn);
            this.changePass_panel.Controls.Add(this.label3);
            this.changePass_panel.Controls.Add(this.change_pass_tb);
            this.changePass_panel.Controls.Add(this.change_user_tb);
            this.changePass_panel.Controls.Add(this.label4);
            this.changePass_panel.Location = new System.Drawing.Point(384, 36);
            this.changePass_panel.Name = "changePass_panel";
            this.changePass_panel.Size = new System.Drawing.Size(286, 311);
            this.changePass_panel.TabIndex = 8;
            // 
            // back_btn
            // 
            this.back_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.back_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.back_btn.Location = new System.Drawing.Point(145, 237);
            this.back_btn.Name = "back_btn";
            this.back_btn.Size = new System.Drawing.Size(135, 50);
            this.back_btn.TabIndex = 7;
            this.back_btn.Text = "Quay Lại";
            this.back_btn.UseVisualStyleBackColor = false;
            // 
            // change_comfirm_tb
            // 
            this.change_comfirm_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.change_comfirm_tb.Location = new System.Drawing.Point(122, 191);
            this.change_comfirm_tb.Name = "change_comfirm_tb";
            this.change_comfirm_tb.PasswordChar = '*';
            this.change_comfirm_tb.Size = new System.Drawing.Size(162, 23);
            this.change_comfirm_tb.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(22, 194);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 16);
            this.label6.TabIndex = 8;
            this.label6.Text = "Xác Nhận Lại:";
            // 
            // change_new_pass_tb
            // 
            this.change_new_pass_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.change_new_pass_tb.Location = new System.Drawing.Point(123, 154);
            this.change_new_pass_tb.Name = "change_new_pass_tb";
            this.change_new_pass_tb.PasswordChar = '*';
            this.change_new_pass_tb.Size = new System.Drawing.Size(162, 23);
            this.change_new_pass_tb.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(17, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 16);
            this.label5.TabIndex = 6;
            this.label5.Text = "Mật Khẩu Mới:";
            // 
            // pictureBox2
            // 
            this.pictureBox2.ErrorImage = null;
            this.pictureBox2.Image = global::RauViet.Properties.Resources.logo_vr;
            this.pictureBox2.InitialImage = null;
            this.pictureBox2.Location = new System.Drawing.Point(60, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(178, 58);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // updatePass_btn
            // 
            this.updatePass_btn.BackColor = System.Drawing.SystemColors.HotTrack;
            this.updatePass_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updatePass_btn.Location = new System.Drawing.Point(8, 237);
            this.updatePass_btn.Name = "updatePass_btn";
            this.updatePass_btn.Size = new System.Drawing.Size(135, 50);
            this.updatePass_btn.TabIndex = 2;
            this.updatePass_btn.Text = "Xác Nhận Đổi";
            this.updatePass_btn.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Tên Đăng Nhập:";
            // 
            // change_pass_tb
            // 
            this.change_pass_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.change_pass_tb.Location = new System.Drawing.Point(123, 118);
            this.change_pass_tb.Name = "change_pass_tb";
            this.change_pass_tb.PasswordChar = '*';
            this.change_pass_tb.Size = new System.Drawing.Size(162, 23);
            this.change_pass_tb.TabIndex = 4;
            // 
            // change_user_tb
            // 
            this.change_user_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.change_user_tb.Location = new System.Drawing.Point(123, 83);
            this.change_user_tb.Name = "change_user_tb";
            this.change_user_tb.Size = new System.Drawing.Size(162, 23);
            this.change_user_tb.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(43, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Mật Khẩu:";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.BackgroundImage = global::RauViet.Properties.Resources.bg1;
            this.ClientSize = new System.Drawing.Size(704, 411);
            this.Controls.Add(this.changePass_panel);
            this.Controls.Add(this.login_panel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginForm";
            this.Text = "Login";
            this.login_panel.ResumeLayout(false);
            this.login_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.changePass_panel.ResumeLayout(false);
            this.changePass_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox userName_tb;
        private System.Windows.Forms.Button login_btn;
        private System.Windows.Forms.TextBox password_tb;
        private System.Windows.Forms.Label login_loading_tb;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button changePass_btn;
        private System.Windows.Forms.Panel login_panel;
        private System.Windows.Forms.Panel changePass_panel;
        private System.Windows.Forms.TextBox change_comfirm_tb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox change_new_pass_tb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button updatePass_btn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox change_pass_tb;
        private System.Windows.Forms.TextBox change_user_tb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button back_btn;
        private System.Windows.Forms.Label label2;
    }
}