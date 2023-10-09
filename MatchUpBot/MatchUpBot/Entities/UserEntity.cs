using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class UserEntity
    {
        public UserEntity(long tgId, string name, int age, string country, string city, string gender, string photo, string tgUsername, string tgChatId)
        {
            TgId = tgId;
            Name = name;
            Age = age;
            Country = country;
            City = city;
            Gender = gender;
            Photo = photo;
            TgUsername = tgUsername;
            TgChatId = tgChatId;
        }

        [Key]
        public long TgId { get; set; }//Уникальный айди пользователя
        public string Name { get; set; }//Отображаемое имя
        public int Age { get; set; }// Возраст
        public string Country { get; set; }// Страна
        public string City { get; set; }//Город
        public string Gender { get; set; }//Пол
        public string Photo { get; set; }//Ссылка на фото
        public string TgUsername { get; set; }//Телеграмовский ник-нейм, по которому можно будет перейти к пользователю в личную переписку
        public string TgChatId { get; set; }//Айди чата, куда отправлять ответ
    }
}