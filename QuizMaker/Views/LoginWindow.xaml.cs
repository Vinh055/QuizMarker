using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuizMaker.Models;
using QuizMaker.Services;

namespace QuizMaker.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            // Nạp dữ liệu từ file JSON lên RAM ngay khi mở app
            DatabaseService.LoadData();

            // BƠM DỮ LIỆU VÀO BẢNG TEST TÀI KHOẢN KHI VỪA MỞ LÊN
            RefreshUserList();
        }

        // Hàm hỗ trợ làm mới bảng danh sách tài khoản test
        private void RefreshUserList()
        {
            DgUsersTest.ItemsSource = null;
            DgUsersTest.ItemsSource = DatabaseService.Db.Users;
        }

        private void BtnDeleteTestUser_Click(object sender, RoutedEventArgs e)
{
                if (DgUsersTest.SelectedItem is not User selectedUser)
                {
                    MessageBox.Show("Vui lòng chọn tài khoản cần xóa!");
                    return;
                }

                if (selectedUser.Role == "Admin")
                {
                    MessageBox.Show("Không nên xóa tài khoản Admin mặc định!");
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa tài khoản '{selectedUser.Username}' không?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result != MessageBoxResult.Yes)
                    return;

                DatabaseService.Db.Users.Remove(selectedUser);
                DatabaseService.SaveData();
                RefreshUserList();

                MessageBox.Show("Đã xóa tài khoản!");
            }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string user = TxtUsername.Text.Trim();
            string pass = TxtPassword.Password;
            string role = (CmbRole.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!");
                return;
            }

            if (DatabaseService.Db.Users.Any(u => u.Username == user))
            {
                MessageBox.Show("Tài khoản đã tồn tại!");
                return;
            }

            DatabaseService.Db.Users.Add(new User { Username = user, Password = pass, Role = role });
            DatabaseService.SaveData();

            // LÀM MỚI BẢNG NGAY SAU KHI ĐĂNG KÝ THÀNH CÔNG
            RefreshUserList();

            MessageBox.Show("Đăng ký thành công!");
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = TxtUsername.Text.Trim();
            string pass = TxtPassword.Password;

            DatabaseService.CurrentUser = DatabaseService.Db.Users.FirstOrDefault(u => u.Username == user && u.Password == pass);

            if (DatabaseService.CurrentUser == null)
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
                return;
            }

            // ĐIỀU HƯỚNG: Mở Cửa sổ tương ứng với Role
            if (DatabaseService.CurrentUser.Role == "Admin")
            {
                new AdminWindow().Show();
            }
            else if (DatabaseService.CurrentUser.Role == "Teacher")
            {
                new TeacherWindow().Show();
            }
            else
            {
                new StudentWindow().Show();
            }

            // Đóng cửa sổ Đăng nhập hiện tại đi
            this.Close();
        }
    }
}