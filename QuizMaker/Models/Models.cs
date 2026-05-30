using System;
using System.Collections.Generic;

namespace QuizMaker.Models
{
    public class AppData
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class Quiz
    {
        public string Title { get; set; }
        public bool HasTimeLimit { get; set; } = false;
        public DateTime OpenTime { get; set; } = DateTime.Now;
        public DateTime CloseTime { get; set; } = DateTime.Now.AddDays(1);
        public bool IsShuffle { get; set; } = true;
        public List<Question> Questions { get; set; } = new List<Question>();
        public List<QuizResult> Results { get; set; } = new List<QuizResult>();

        public string Status => HasTimeLimit ? ((DateTime.Now >= OpenTime && DateTime.Now <= CloseTime) ? "Đang Mở" : "Đã Đóng / Chưa Mở") : "Luôn Mở";
    }

    public class QuizResult
    {
        public string StudentUsername { get; set; }
        public string QuizTitle { get; set; }
        public int Score { get; set; }
        public DateTime TakenAt { get; set; }
        public List<string> WrongQuestionIds { get; set; } = new List<string>();
        public List<AnswerDetail> Details { get; set; } = new List<AnswerDetail>();
    }

    public class AnswerDetail
    {
        public string QuestionTitle { get; set; }
        public string SelectedOption { get; set; }
        public string CorrectOption { get; set; }
        public bool IsCorrect => SelectedOption == CorrectOption;
    }

    public class Question
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public string CorrectAnswer { get; set; }
        public string ImagePath { get; set; }
    }
}