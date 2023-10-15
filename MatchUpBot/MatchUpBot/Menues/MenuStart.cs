/*using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApplication1.Menues
{
    public class MenuStart
    {
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Start"),
                            new KeyboardButton("Что может бот?"),
                        },
                    });
                    await botClient.SendTextMessageAsync(message.Chat, "Привет! Выберите действие:",
                        replyMarkup: keyboard);
                }
                else if (message.Text.ToLower() == "start")
                {
                    long userId = message.From.Id;
                    await botClient.SendTextMessageAsync(message.Chat, "У вас нет анкеты. Введите данные для ее создания");
                    
                    //добавь сюда ввод данных для анкеты. типо имя возраст
                    

                    await botClient.SendTextMessageAsync(message.Chat, "Вы нажали на Кнопку 1");
                }
                else if (message.Text.ToLower() == "что может бот?")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Здравствуйте! Этот бот предназначен " +
                                                                       "для знакомств и общения с интересными людьми. " +
                                                                       "Вы можете лайкать анкеты пользователей, отправлять " +
                                                                       "сообщения, искать новых друзей и даже находить " +
                                                                       "свою половинку. Надеемся, вам здесь понравится!");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Привет!");
                }
            }
        }

        private async Task<User> CreateProfileAsync(ITelegramBotClient botClient, long chatId)
        {
            await botClient.SendTextMessageAsync(chatId, "Давайте создадим вашу анкету. Введите ваше имя:");
            var nameResponse = await botClient.ReceiveTextMessageAsync(chatId, cancellationToken);

            await botClient.SendTextMessageAsync(chatId, "Введите ваш возраст:");
            var ageResponse = await botClient.ReceiveTextMessageAsync(chatId, cancellationToken);

            // Здесь можно добавить логику для получения других данных анкеты, если необходимо.

            var userProfile = new User
            {
                
            };

            return userProfile;
        }


        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
    }
}*/

