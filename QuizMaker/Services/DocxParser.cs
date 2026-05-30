using System.Collections.Generic;
using DocumentFormat.OpenXml.Packaging;
using QuizMaker.Models;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

namespace QuizMaker.Services
{
    public static class DocxParser
    {
        public static List<Question> ParseQuestions(string filePath)
        {
            var list = new List<Question>();
            using (var doc = WordprocessingDocument.Open(filePath, false))
            {
                var body = doc.MainDocumentPart.Document.Body;
                foreach (var para in body.Elements<WordParagraph>())
                {
                    string[] parts = para.InnerText.Trim().Split('|');
                    if (parts.Length == 7)
                    {
                        list.Add(new Question
                        {
                            Title = parts[0].Trim(),
                            Text = parts[1].Trim(),
                            Options = new List<string> { parts[2].Trim(), parts[3].Trim(), parts[4].Trim(), parts[5].Trim() },
                            CorrectAnswer = parts[6].Trim().ToUpper()
                        });
                    }
                }
            }
            return list;
        }
    }
}