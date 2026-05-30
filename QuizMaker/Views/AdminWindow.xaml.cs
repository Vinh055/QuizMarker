using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuizMaker.Models;
using QuizMaker.Services;

namespace QuizMaker.Views
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            RefreshAdminDashboard();
        }

        private void RefreshAdminDashboard()
        {
            DgAdminUsersList.ItemsSource = null;
            // Chỉ lấy danh sách Học sinh và Giáo viên (Không hiển thị Admin mặc định)
            DgAdminUsersList.ItemsSource = DatabaseService.Db.Users.Where(u => u.Role != "Admin").ToList();
        }

        private void DgAdminUsersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgAdminUsersList.SelectedItem is User selectedUser)
            {
                TxtAdminManageUser.Text = selectedUser.Username;
                TxtAdminManagePass.Text = selectedUser.Password;
                CmbAdminManageRole.SelectedIndex = selectedUser.Role == "Teacher" ? 1 : 0;
            }
        }

        private void BtnAdminSaveUser_Click(object sender, RoutedEventArgs e)
        {
            string user = TxtAdminManageUser.Text.Trim();
            string pass = TxtAdminManagePass.Text.Trim();
            string role = (CmbAdminManageRole.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!");
                return;
            }

            var existingUser = DatabaseService.Db.Users.FirstOrDefault(u => u.Username == user);

            if (existingUser != null)
            {
                if (existingUser.Role == "Admin")
                {
                    MessageBox.Show("Không thể sửa tài khoản Admin mặc định!");
                    return;
                }
                existingUser.Password = pass;
                existingUser.Role = role;
                MessageBox.Show("Đã cập nhật tài khoản!");
            }
            else
            {
                DatabaseService.Db.Users.Add(new User { Username = user, Password = pass, Role = role });
                MessageBox.Show("Đã thêm tài khoản mới!");
            }

            DatabaseService.SaveData();
            RefreshAdminDashboard();
        }

        private void BtnAdminDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (DgAdminUsersList.SelectedItem is User userToDelete)
            {
                if (userToDelete.Role == "Admin")
                {
                    MessageBox.Show("Không thể xóa Admin!");
                    return;
                }

                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa tài khoản '{userToDelete.Username}'?", "Cảnh báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DatabaseService.Db.Users.Remove(userToDelete);

                    // Nếu tài khoản bị xóa là Học sinh -> Xóa luôn toàn bộ kết quả thi để dọn rác DB
                    if (userToDelete.Role == "Student")
                    {
                        foreach (var quiz in DatabaseService.Db.Quizzes)
                        {
                            quiz.Results.RemoveAll(r => r.StudentUsername == userToDelete.Username);
                        }
                    }

                    DatabaseService.SaveData();
                    RefreshAdminDashboard();
                    MessageBox.Show("Đã xóa tài khoản!");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một tài khoản từ danh sách bên phải!");
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Đăng xuất: Xóa phiên làm việc, mở lại màn Login và đóng màn Admin
            DatabaseService.CurrentUser = null;
            new LoginWindow().Show();
            this.Close();
        }
    }
}