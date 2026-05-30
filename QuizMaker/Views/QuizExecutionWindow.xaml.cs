using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using QuizMaker.Models;
using QuizMaker.Services;

namespace QuizMaker.Views
{
    public partial class QuizExecutionWindow : Window
    {
        private Quiz _currentStudentQuiz;
        private List<Question> _currentTestSession;
        private int _currentQuestionIndex = 0;
        private int _score = 0;
        private QuizResult _currentResult;

        // Constructor nhận dữ liệu bài thi được truyền từ StudentWindow sang
        public QuizExecutionWindow(Quiz targetQuiz)
        {
            InitializeComponent();
            _currentStudentQuiz = targetQuiz;
            StartQuiz();
        }

        private void StartQuiz()
        {
            _currentQuestionIndex = 0;
            _score = 0;
            _currentResult = new QuizResult
            {
                StudentUsername = DatabaseService.CurrentUser.Username,
                QuizTitle = _currentStudentQuiz.Title
            };

            // Thực hiện tính năng trộn đề
            _currentTestSession = _currentStudentQuiz.IsShuffle
                ? _currentStudentQuiz.Questions.OrderBy(x => Guid.NewGuid()).ToList()
                : _currentStudentQuiz.Questions.ToList();

            LblQuizTitleDoing.Text = $"Đang làm: {_currentStudentQuiz.Title}";
            GridQuiz.Visibility = Visibility.Visible;
            LoadQuestionUI();
        }

        private void LoadQuestionUI()
        {
            OptA.IsChecked = false; OptB.IsChecked = false; OptC.IsChecked = false; OptD.IsChecked = false;
            var currentQ = _currentTestSession[_currentQuestionIndex];

            LblQuestionTitle.Text = $"{currentQ.Title} ({_currentQuestionIndex + 1}/{_currentTestSession.Count})";
            LblQuestionText.Text = currentQ.Text;
            OptA.Content = $"A. {currentQ.Options[0]}";
            OptB.Content = $"B. {currentQ.Options[1]}";
            OptC.Content = $"C. {currentQ.Options[2]}";
            OptD.Content = $"D. {currentQ.Options[3]}";

            if (!string.IsNullOrEmpty(currentQ.ImagePath) && File.Exists(currentQ.ImagePath))
            {
                ImgQuestion.Source = new BitmapImage(new Uri(currentQ.ImagePath, UriKind.Absolute));
                ImgQuestion.Visibility = Visibility.Visible;
            }
            else ImgQuestion.Visibility = Visibility.Collapsed;
        }

        private void BtnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            string ans = OptA.IsChecked == true ? "A" : OptB.IsChecked == true ? "B" : OptC.IsChecked == true ? "C" : OptD.IsChecked == true ? "D" : "";
            if (ans == "") { MessageBox.Show("Hãy chọn đáp án!"); return; }

            var currentQ = _currentTestSession[_currentQuestionIndex];

            _currentResult.Details.Add(new AnswerDetail
            {
                QuestionTitle = currentQ.Title,
                SelectedOption = ans,
                CorrectOption = currentQ.CorrectAnswer
            });

            if (ans == currentQ.CorrectAnswer) _score++;
            else _currentResult.WrongQuestionIds.Add(currentQ.Id);

            _currentQuestionIndex++;

            if (_currentQuestionIndex < _currentTestSession.Count)
            {
                LoadQuestionUI();
            }
            else
            {
                _currentResult.Score = _score;
                _currentResult.TakenAt = DateTime.Now;

                // Ghi nhận kết quả và lưu Database
                _currentStudentQuiz.Results.Add(_currentResult);
                DatabaseService.SaveData();

                MessageBox.Show($"Nộp bài thành công! Bạn đúng {_score}/{_currentTestSession.Count} câu.");

                // Trở về Hub học viên
                new StudentWindow().Show();
                this.Close();
            }
        }
    }
}