namespace TelegramBotExperiments.Models
{
    public class User
    {
        public int TelegramId { get; set; }
        public string ProfileName { get; set; }
        public string Username { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string About { get; set; }
        public string City { get; set; }
        
        
        public override string ToString()
        {
            return base.ToString();
        }

        public string PhotoPath { get; set; }

        public User()
        {
        }

        public User(int telegramId, string profileName, string username, int age, string sex, string about, string city, string photoPath)
        {
            TelegramId = telegramId;
            ProfileName = profileName;
            Username = username;
            Age = age;
            Sex = sex;
            About = about;
            City = city;
            PhotoPath = photoPath;
        }
        
        /*public void PrintToConsole()
        {
            Console.WriteLine("User Profile:");
            Console.WriteLine($"Telegram ID: {TelegramId}");
            Console.WriteLine($"Profile Name: {ProfileName ?? "N/A"}");
            Console.WriteLine($"Username: {Username ?? "N/A"}");
            Console.WriteLine($"Age: {Age.ToString() ?? "N/A"}");
            Console.WriteLine($"Sex: {Sex ?? "N/A"}");
            Console.WriteLine($"About: {About ?? "N/A"}");
            Console.WriteLine($"City: {City ?? "N/A"}");
            Console.WriteLine($"Photo Path: {PhotoPath ?? "N/A"}");
        }*/
    }
}