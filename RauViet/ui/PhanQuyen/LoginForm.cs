using RauViet.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui.PhanQuyen
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            userName_tb.TabIndex = 1;
            password_tb.TabIndex = 2;
            login_btn.TabIndex = 3;

            change_user_tb.TabIndex = 1;
            change_pass_tb.TabIndex = 2;
            change_new_pass_tb.TabIndex = 3;
            change_comfirm_tb.TabIndex = 4;
            updatePass_btn.TabIndex = 5;

            login_panel.Left = (this.ClientSize.Width - login_panel.Width) / 2;
            login_panel.Top = (this.ClientSize.Height - login_panel.Height) / 2;

            changePass_panel.Left = (this.ClientSize.Width - changePass_panel.Width) / 2;
            changePass_panel.Top = (this.ClientSize.Height - changePass_panel.Height) / 2;

            changePass_panel.Visible = false;
            login_panel.Visible = true;
            login_loading_tb.Visible = false;

            this.FormBorderStyle = FormBorderStyle.FixedDialog; // hoặc FixedSingle
            this.MaximizeBox = false; // tắt nút maximize
            this.MinimizeBox = false; // tắt nút minimize nếu muốn

            // 2️⃣ Luôn hiển thị trên cùng
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            login_btn.Click += Login_btn_Click;
            changePass_btn.Click += ChangePass_btn_Click;
            back_btn.Click += Back_btn_Click;
            updatePass_btn.Click += UpdatePass_btn_Click;
            password_tb.KeyDown += Password_tb_KeyDown;
            this.Load += LoginForm_Load;
        }

        private async void UpdatePass_btn_Click(object sender, EventArgs e)
        {
            string nameStr = change_user_tb.Text;
            string passStr = change_pass_tb.Text;
            string newPassStr = change_new_pass_tb.Text;
            string confirmStr = change_comfirm_tb.Text;

            if(nameStr.CompareTo("") == 0  || passStr.CompareTo("") == 0 || newPassStr.CompareTo("") == 0 || confirmStr.CompareTo("") == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (newPassStr.CompareTo(confirmStr) != 0)
            {
                MessageBox.Show("Mật khẩu mới không khớp.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            bool isSuccess = await SQLManager.Instance.ChangePasswordAsync(nameStr, passStr, newPassStr);

            if (isSuccess)
            {
                changePass_panel.Visible = false;
                login_panel.Visible = true;
                userName_tb.Focus();
            }
            else
            {
                MessageBox.Show("Thất bại, kiểm tra lại", "Thông báo",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void Back_btn_Click(object sender, EventArgs e)
        {
            userName_tb.Focus();
            changePass_panel.Visible = false;
            login_panel.Visible = true;
            return;
            throw new NotImplementedException();
        }

        private void ChangePass_btn_Click(object sender, EventArgs e)
        {
            change_user_tb.Focus();
            userName_tb.Text = "";
            password_tb.Text = "";
            change_comfirm_tb.Text = "";
            change_new_pass_tb.Text = "";
            change_user_tb.Text = "";
            change_pass_tb.Text = "";
            changePass_panel.Visible = true;
            login_panel.Visible = false;

            return;
            throw new NotImplementedException();
        }

        private async void Login_btn_Click(object sender, EventArgs e)
        {
            string userName = userName_tb.Text?.Trim();
            string password = password_tb.Text?.Trim();

            // ⚠️ Kiểm tra trống
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                login_loading_tb.Visible = true;

                var loginTask = SQLManager.Instance.GetPasswordHashFromDatabase(userName);
                var userRoleTask = SQLManager.Instance.GetUserRoleAsync(userName);

                await Task.WhenAll(loginTask, userRoleTask);
                string passwordDB = loginTask.Result;
                DataRow data = userRoleTask.Result;

                bool isPass = true;

                // 🔍 Kiểm tra dữ liệu
                if (string.IsNullOrEmpty(passwordDB) || data == null)
                    isPass = false;
                else if (Utils.VerifyPassword(password, passwordDB))
                {
                    string employeeCode = data["EmployeeCode"].ToString();
                    var employeeInfoTask = SQLManager.Instance.GetUserInfoAsync(employeeCode);
                    var machineInfoTask = SQLManager.Instance.updateMachineInfoWhenLoginAsync(userName);
                    await Task.WhenAll(employeeInfoTask, machineInfoTask);
                    var employeeInfo = employeeInfoTask.Result;
                    if (employeeInfo.Rows.Count <= 0)
                        isPass = false;
                    else
                    {
                        employeeInfo.Columns.Add("RoleCodes", typeof(string));
                        employeeInfo.Rows[0]["RoleCodes"] = data["RoleCodes"];

                        data = employeeInfo.Rows[0];
                    }
                }
                else
                {
                    isPass = false;
                }

                if (!isPass)
                {
                    login_loading_tb.Visible = false;

                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu.", "Thông Báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (userName != Properties.Settings.Default.login_acc)
                {
                    Properties.Settings.Default.login_acc = userName;
                    Properties.Settings.Default.current_form = "";
                    Properties.Settings.Default.Save();
                }
                login_loading_tb.Visible = false;
                UserManager.Instance.init(userName, password, data);

                this.Hide();
                using (var loading = new Loading())
                {
                    loading.ShowDialog();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                login_loading_tb.Visible = false;
                MessageBox.Show($"Lỗi khi đăng nhập:\n{ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Password_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // tránh kêu "ding" khi nhấn Enter
                login_btn.PerformClick();   // gọi sự kiện click của nút login
            }
        }

        private async void LoginForm_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.login_acc))
            {
                userName_tb.Text = Properties.Settings.Default.login_acc;
                password_tb.Text = "";
                await Task.Delay(100);
                password_tb.Focus();
            }
            else
            {
                userName_tb.Text = "";
                password_tb.Text = "";
                userName_tb.Focus();
            }
        }
    }
}
