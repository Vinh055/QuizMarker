using System;
using System.Linq;
using System.Windows;
using QuizMaker.Models;
using QuizMaker.Services;

namespace QuizMaker.Views
{
    public partial class StudentWindow : Window
    {
        public StudentWindow()
        {
            InitializeComponent();
            LblStudentWelcome.Text = $"Xin chào Học viên, {DatabaseService.CurrentUser.Username}!";
            ShowDashboard();
            RefreshStudentDashboard();
        }

        // --- Quản lý hiển thị các Grid nội bộ ---
        private void ShowDashboard()
        {
            GridStudentDashboard.Visibility = Visibility.Visible;
            GridStudentResultDetails.Visibility = Visibility.Collapsed;
        }

        private void ShowResultDetails()
        {
            GridStudentDashboard.Visibility = Visibility.Collapsed;
            GridStudentResultDetails.Visibility = Visibility.Visible;
        }

        private void RefreshStudentDashboard()
        {
            DgStudentQuizzes.ItemsSource = null;
            DgStudentQuizzes.ItemsSource = DatabaseService.Db.Quizzes;
        }

        private void BtnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (DgStudentQuizzes.SelectedItem is Quiz q)
            {
                if (q.HasTimeLimit)
                {
                    if (DateTime.Now < q.OpenTime) { MessageBox.Show("Bài thi chưa mở!"); return; }
                    if (DateTime.Now > q.CloseTime) { MessageBox.Show("Đã hết hạn làm bài!"); return; }
                }

                if (q.Results.Any(r => r.StudentUsername == DatabaseService.CurrentUser.Username)) { MessageBox.Show("Bạn đã làm bài này rồi! Bấm 'Xem kết quả' để xem lại."); return; }
                if (q.Questions.Count == 0) { MessageBox.Show("Bài kiểm tra chưa có câu hỏi!"); return; }

                // TRUYỀN BÀI THI SANG CỬA SỔ LÀM BÀI VÀ ĐÓNG CỬA SỔ HIỆN TẠI
                QuizExecutionWindow executionWin = new QuizExecutionWindow(q);
                executionWin.Show();
                this.Close();
            }
            else { MessageBox.Show("Vui lòng chọn bài kiểm tra!"); }
        }

        private void BtnViewStudentResult_Click(object sender, RoutedEventArgs e)
        {
            if (DgStudentQuizzes.SelectedItem is Quiz q)
            {
                var myRes = q.Results.FirstOrDefault(r => r.StudentUsername == DatabaseService.CurrentUser.Username);
                if (myRes != null)
                {
                    ShowResultDetails();
                    LblStudentScoreDetail.Text = $"Điểm của bạn: {myRes.Score} / {q.Questions.Count}";
                    DgStudentAnswers.ItemsSource = null;
                    DgStudentAnswers.ItemsSource = myRes.Details;
                }
                else { MessageBox.Show("Bạn chưa làm bài này!"); }
            }
            else { MessageBox.Show("Vui lòng chọn bài kiểm tra!"); }
        }

        private void BtnBackToStudentDash_Click(object sender, RoutedEventArgs e)
        {
            ShowDashboard();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            DatabaseService.CurrentUser = null;
            new LoginWindow().Show();
            this.Close();
        }
    }
}