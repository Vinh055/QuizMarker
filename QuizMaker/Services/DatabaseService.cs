using System.IO;
using System.Linq;
using System.Text.Json;
using QuizMaker.Models;

namespace QuizMaker.Services
{
    public static class DatabaseService
    {
        public static AppData Db { get; set; } = new AppData();
        public static User CurrentUser { get; set; }
        private const string DbPath = "database.json";

        public static void LoadData()
        {
            if (File.Exists(DbPath))
            {
                try { Db = JsonSerializer.Deserialize<AppData>(File.ReadAllText(DbPath)) ?? new AppData(); }
                catch { Db = new AppData(); }
            }
            if (!Db.Users.Any(u => u.Role == "Admin"))
            {
                Db.Users.Add(new User { Username = "admin", Password = "admin123", Role = "Admin" });
                SaveData();
            }
        }

        public static void SaveData()
        {
            string json = JsonSerializer.Serialize(Db, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DbPath, json);
        }
    }
}