using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using QuizMaker.Models;
using QuizMaker.Services;

namespace QuizMaker.Views
{
    public partial class TeacherWindow : Window
    {
        public TeacherWindow()
        {
            InitializeComponent();
            ShowDashboard();
            RefreshTeacherDashboard();
        }

        // --- Quản lý hiển thị các Grid nội bộ ---
        private void ShowDashboard()
        {
            GridTeacherDashboard.Visibility = Visibility.Visible;
            GridTeacherStats.Visibility = Visibility.Collapsed;
        }

        private void ShowStats()
        {
            GridTeacherDashboard.Visibility = Visibility.Collapsed;
            GridTeacherStats.Visibility = Visibility.Visible;
        }

        private void RefreshTeacherDashboard()
        {
            LstTeacherQuizzes.ItemsSource = null;
            LstTeacherQuizzes.ItemsSource = DatabaseService.Db.Quizzes;
        }

        private void LstTeacherQuizzes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstTeacherQuizzes.SelectedItem is Quiz q)
            {
                TxtNewQuizName.Text = q.Title;
                ChkHasTimeLimit.IsChecked = q.HasTimeLimit;
                TxtOpenTime.Text = q.OpenTime.ToString("yyyy-MM-dd HH:mm");
                TxtCloseTime.Text = q.CloseTime.ToString("yyyy-MM-dd HH:mm");
                ChkShuffle.IsChecked = q.IsShuffle;

                LblSelectedQuizQuestions.Text = $"Câu hỏi trong: {q.Title}";
                DgQuestions.ItemsSource = null;
                DgQuestions.ItemsSource = q.Questions;
            }
        }

        private void ChkHasTimeLimit_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (PanelTimeLimits != null)
                PanelTimeLimits.IsEnabled = ChkHasTimeLimit.IsChecked == true;
        }

        private void BtnAddOrUpdateQuiz_Click(object sender, RoutedEventArgs e)
        {
            DateTime open = DateTime.Now;
            DateTime close = DateTime.Now.AddDays(1);
            bool hasTime = ChkHasTimeLimit.IsChecked == true;

            if (hasTime)
            {
                if (!DateTime.TryParse(TxtOpenTime.Text, out open) || !DateTime.TryParse(TxtCloseTime.Text, out close))
                {
                    MessageBox.Show("Sai định dạng ngày giờ! Dùng: yyyy-MM-dd HH:mm"); return;
                }
            }

            if (LstTeacherQuizzes.SelectedItem is Quiz existing)
            {
                existing.Title = TxtNewQuizName.Text;
                existing.HasTimeLimit = hasTime;
                existing.OpenTime = open;
                existing.CloseTime = close;
                existing.IsShuffle = ChkShuffle.IsChecked == true;
            }
            else
            {
                DatabaseService.Db.Quizzes.Add(new Quiz
                {
                    Title = TxtNewQuizName.Text,
                    HasTimeLimit = hasTime,
                    OpenTime = open,
                    CloseTime = close,
                    IsShuffle = ChkShuffle.IsChecked == true
                });
            }
            DatabaseService.SaveData();
            RefreshTeacherDashboard();
            MessageBox.Show("Đã lưu bài!");
        }

        private void BtnDeleteQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (LstTeacherQuizzes.SelectedItem is Quiz q)
            {
                if (MessageBox.Show($"Bạn có chắc muốn xóa toàn bộ bài '{q.Title}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DatabaseService.Db.Quizzes.Remove(q);
                    DatabaseService.SaveData();
                    RefreshTeacherDashboard();
                    DgQuestions.ItemsSource = null;
                    LblSelectedQuizQuestions.Text = "Câu hỏi trong bài: (Chưa chọn bài)";
                }
            }
            else { MessageBox.Show("Vui lòng chọn một bài kiểm tra để xóa!"); }
        }

        private void BtnUploadDoc_Click(object sender, RoutedEventArgs e)
        {
            if (LstTeacherQuizzes.SelectedItem is Quiz selectedQuiz)
            {
                OpenFileDialog ofd = new OpenFileDialog { Filter = "Word Docs|*.docx" };
                if (ofd.ShowDialog() == true)
                {
                    // Truy cập phương thức tĩnh (static) hoàn toàn qua Tên Class (DocxParser)
                    selectedQuiz.Questions.AddRange(DocxParser.ParseQuestions(ofd.FileName));
                    DatabaseService.SaveData();
                    DgQuestions.ItemsSource = null;
                    DgQuestions.ItemsSource = selectedQuiz.Questions;
                    MessageBox.Show("Đã thêm câu hỏi!");
                }
            }
            else { MessageBox.Show("Chọn một bài bên trái trước!"); }
        }

        private void BtnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            if (LstTeacherQuizzes.SelectedItem is Quiz selectedQuiz && DgQuestions.SelectedItem is Question selectedQ)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Image Files (*.jpg;*.png)|*.jpg;*.png" };
                if (openFileDialog.ShowDialog() == true)
                {
                    selectedQ.ImagePath = openFileDialog.FileName;
                    DatabaseService.SaveData();
                    DgQuestions.ItemsSource = null;
                    DgQuestions.ItemsSource = selectedQuiz.Questions;
                    MessageBox.Show("Đã đính kèm ảnh thành công!");
                }
            }
            else { MessageBox.Show("Vui lòng chọn một câu hỏi trong bảng lưới."); }
        }

        private void BtnViewStats_Click(object sender, RoutedEventArgs e)
        {
            if (LstTeacherQuizzes.SelectedItem is Quiz q)
            {
                ShowStats();

                int totalTakes = q.Results.Count;
                double avg = totalTakes > 0 ? q.Results.Average(r => r.Score) : 0;

                var wrongCounts = q.Results.SelectMany(r => r.WrongQuestionIds)
                                           .GroupBy(id => id)
                                           .OrderByDescending(g => g.Count())
                                           .FirstOrDefault();

                string worstQ = wrongCounts != null ? q.Questions.FirstOrDefault(x => x.Id == wrongCounts.Key)?.Title : "Không có";

                LblStatSummary.Text = $"Lượt thi: {totalTakes} | Điểm trung bình: {avg:F2} / {q.Questions.Count} | Câu sai nhiều nhất: {worstQ}";
                DgStats.ItemsSource = q.Results;
            }
            else { MessageBox.Show("Vui lòng chọn bài kiểm tra muốn xem thống kê!"); }
        }

        private void BtnBackToTeacherDash_Click(object sender, RoutedEventArgs e)
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