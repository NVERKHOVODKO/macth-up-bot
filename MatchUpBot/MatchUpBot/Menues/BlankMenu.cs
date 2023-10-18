using Data;
using Entities;
using Microsoft.Extensions.Logging;
using Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Action = Constants.Action;

namespace ConsoleApplication1.Menues;

public class BlankMenu
{
    private static readonly UserRepository UserRepository = new();
    private static ILogger<BlankMenu> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BlankMenu>();

    public BlankMenu(ILogger<BlankMenu> logger)
    {
        _logger = logger;
    }
  public static async Task HandleMessageTypeText(Message message, ITelegramBotClient botClient, Chat chat,
        CancellationToken cancellationToken, UserEntity curUser)
    {
        var tgId = message.From.Id;
        int Stage = UserRepository.GetUserStage(tgId);
        _logger.LogInformation($"Stage user({tgId}): {Stage}");
        var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new("Start!"),
                    new("Что может бот?")
                }
            })
        {
            ResizeKeyboard = true
        };
        if (Stage == -1)
        {
            await EnterName(message, botClient, chat);
        }

        

        Console.WriteLine(Stage);

        switch (Stage)
        {
            case 0:
                AddNameToDatabase(message);
                await EnterAge(message, botClient, chat);
                break;
            case 1:
                AddAgeToDatabase(message);
                await EnterCity(message, botClient, chat);
                break;
            case 2:
                AddCityToDatabase(message);
                await EnterAbout(message, botClient, chat);
                break;
            case 3:
                AddAboutToDatabase(message);
                await EnterSex(botClient, chat);
                break;
            case 4:
                if(!AddZodiacSignToDatabase(message, curUser,chat,botClient))
                   break;
                await EnterPhoto(message, botClient, chat);
                break;
            case 5:
                await EditProfileChoice(message, botClient, chat);
                break;
            case (int)Action.EditProfile:
                await EditProfile(message, botClient, chat, cancellationToken);
                break;
            /*case (int)Action.EditName: //TODO 
                curUser.Name = message.Text;
                await botClient.SendTextMessageAsync(
                    chat.Id,
                    $"Твое новое имя: {curUser.Name}");
                Stage = (int)Action.EditProfile;
                await UpdateUserInfo(curUser, context);
                break;
            case (int)Action.EditAge: //TODO 
                try
                {
                    curUser.Age = int.Parse(message.Text);
                    await botClient.SendTextMessageAsync(
                        chat.Id,
                        $"Твой новый возраст:{curUser.Age}");

                    Stage = (int)Action.EditProfile;
                    context.UserEntity.Update(curUser);
                    await context.SaveChangesAsync();
                }
                catch (FormatException e)
                {
                    await botClient.SendTextMessageAsync(
                        chat.Id,
                        "Введи корректный возраст");
                }
                break;
            case (int)Action.EditCity: //TODO 

                curUser.City = message.Text;
                await botClient.SendTextMessageAsync(
                    chat.Id,
                    $"Твой новый город: {curUser.City}");

                Stage = (int)Action.EditProfile;
                break;
            case (int)Action.EditDescription: //TODO 
                curUser.About = message.Text;
                await botClient.SendTextMessageAsync(
                    chat.Id,
                    $"Твое новое описание: {curUser.About}");

                Stage = (int)Action.EditProfile;
                curUser.PrintToConsole();
                break;*/
            default:
            {
                await botClient.SendTextMessageAsync(
                    chat.Id,
                    "Используй только текст!");
                return;
            }
        }
    }
    public static async Task HandleMessageTypePhoto(Message message, ITelegramBotClient botClient, Chat chat)
    {
        int Stage = UserRepository.GetUserStage(message.From.Id);
        if (Stage != 7)
        {
            await botClient.SendTextMessageAsync(chat.Id, "Используй текст");
            return;
        }

        await PhotoRepository.HandlePhotoMessage(message, botClient);
        UserRepository.UpdateUserStage(message.From.Id, 8);
        _logger.LogInformation($"user({message.From.Id}): Stage updated: {8}");

        /*ReplyKeyboardMarkup menuKeyboard = new(new[]
        {
            new KeyboardButton[] { "Редактировать профиль" },
            new KeyboardButton[] { "Просмотреть анкеты" },
            new KeyboardButton[] { "Отправить сообщение об ошибке" }
        });
        curUser.PrintToConsole();
        curUser.Stage = 6;
        curUser.Country = "Belarus";
        curUser.Photo = "dsa";
        curUser.TgChatId = "adsa";*/

        /*await context.UserEntity.AddAsync(curUser);
        await context.SaveChangesAsync();
        await botClient.SendTextMessageAsync(chat.Id, "Выбери действие", replyMarkup: menuKeyboard);*/
    }
    
    private static async Task EnterName(Message message, ITelegramBotClient botClient, Chat chat)
    {
        int Stage = 0;
        UserRepository.UpdateUserStage(message.From.Id, 0);
        _logger.LogInformation($"Stage user({message.From.Id}): updated {Stage}");
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Как тебя зовут?");
    }
 
    private static void AddNameToDatabase(Message message)
    {
        UserRepository.SetUserName(message.From.Id, message.Text);
        _logger.LogInformation($"user({message.From.Id}): updated name: {message.Text}");
    }
    private static async Task EnterAge(Message message, ITelegramBotClient botClient, Chat chat)
    {
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Сколько тебе лет?");
        UserRepository.UpdateUserStage(message.From.Id, 1);
        _logger.LogInformation($"user({message.From.Id}): Stage updated: {1}");
    }

    private static void AddAgeToDatabase(Message message)
    {
        UserRepository.SetUserAge(message.From.Id, int.Parse(message.Text));
        _logger.LogInformation($"user({message.From.Id}): updated age: {message.Text}");
    }
    private static async Task EnterCity(Message message, ITelegramBotClient botClient, Chat chat)
    {
        try
        {
            await botClient.SendTextMessageAsync(
                chat.Id,
                "Из какого ты города?");
            UserRepository.UpdateUserStage(message.From.Id, 2);
            _logger.LogInformation($"user({message.From.Id}): Stage updated: {2}");
        }
        catch (FormatException e)
        {
            await botClient.SendTextMessageAsync(chat.Id, "Введи корректный возраст");
            UserRepository.UpdateUserStage(message.From.Id, 1);
            _logger.LogInformation($"user({message.From.Id}): Stage updated: {1}");
        }
    }

    private static void AddCityToDatabase(Message message)
    {
        UserRepository.SetUserCity(message.From.Id, message.Text);
        _logger.LogInformation($"user({message.From.Id}): updated city: {message.Text}");
    }
    
    private static async Task EnterAbout(Message message, ITelegramBotClient botClient, Chat chat)
    {
        var skipKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new("Пропустить")
                }
            })
        {
            ResizeKeyboard = true
        };
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Расскажи о себе?",
            replyMarkup: skipKeyboard);

        UserRepository.UpdateUserStage(message.From.Id, 3);
        _logger.LogInformation($"user({message.From.Id}): Stage updated: {3}");
    }

    private static void AddAboutToDatabase(Message message)
    {
        if (message.Text == "Пропустить")
        {
            UserRepository.SetUserAbout(message.From.Id, string.Empty);
            _logger.LogInformation($"user({message.From.Id}): skipped entering about");
        }
        else
        {
            UserRepository.SetUserAbout(message.From.Id, message.Text);
            _logger.LogInformation($"user({message.From.Id}): updated about: {message.Text}");
        }
        UserRepository.UpdateUserStage(message.From.Id, 4);
        _logger.LogInformation($"user({message.From.Id}): Stage updated: {4}");
    }

    private static int messageId;

    public static int getMessageId()
    {
        return messageId;
    }
    private static async Task EnterSex(ITelegramBotClient botClient, Chat chat)
    {
        InlineKeyboardMarkup sexKeyboard = new(new[]
        {
            InlineKeyboardButton.WithCallbackData("Мужчина", "man"),
            InlineKeyboardButton.WithCallbackData("Женщина", "woman")
        });
        Message sentMessage = await botClient.SendTextMessageAsync(
            chat.Id,
            "Какой у тебя гендер?",
            replyMarkup: sexKeyboard);
        messageId = sentMessage.MessageId;
    }

    private static bool AddZodiacSignToDatabase(Message message, UserEntity curUser, Chat chat, ITelegramBotClient botClient)
    {
        if (!IsZodiacSignValid(message.Text))
        {
            botClient.SendTextMessageAsync(chat.Id, "Введи корректный знак задиака");
            UserRepository.UpdateUserStage(message.From.Id,4);
            _logger.LogInformation($"user({message.From.Id}): Stage updated: {4}");
            return false;
        }
        curUser.ZodiacSign = message.Text;
        UserRepository.SetUserZodiacSign(message.From.Id, message.Text);
        _logger.LogInformation($"user({message.From.Id}): updated ZodiacSign: {message.Text}");
        return true;
    }
    private static async Task EnterPhoto(Message message, ITelegramBotClient botClient, Chat chat)
    {
        var removeKeyboard = new ReplyKeyboardRemove();
        await botClient.SendTextMessageAsync(chat.Id, "Скинь свою фото",
            replyMarkup: removeKeyboard);
        UserRepository.UpdateUserStage(message.From.Id, 7);
        _logger.LogInformation($"user({message.From.Id}): Stage updated: {7}");
    }
    
    
    public static bool IsZodiacSignValid(string zodiacSign)
    {
        List<string> zodiacSigns = new List<string> { "овен", "телец", "близнецы", "рак", "лев", "дева", "весы", "скорпион", "стрелец", "козерог", "водолей", "рыбы" };
        string lowerCaseZodiacSign = zodiacSign.ToLower();
        return zodiacSigns.Contains(lowerCaseZodiacSign);
    }

    
    
    private static async Task EnterInterest(Message message, ITelegramBotClient botClient, Chat chat, UserEntity curUser)
    {
        
    }

    private static async Task EditProfileChoice(Message message, ITelegramBotClient botClient, Chat chat)
    {
        if (message.Text == "Редактировать профиль")
        {
            var removeKeyboard = new ReplyKeyboardRemove();
            removeKeyboard = new ReplyKeyboardRemove();
            await botClient.SendTextMessageAsync(chat.Id, "Давай редактировать",
                replyMarkup: removeKeyboard);
            ReplyKeyboardMarkup menuKeyboard = new(new[]
            {
                new KeyboardButton[] { "Редактировать фото" },
                new KeyboardButton[] { "Редактировать инфу" },
                new KeyboardButton[] { "Назад" }
            });
            await botClient.SendTextMessageAsync(chat.Id, "Выбери что хочешь изменить",
                replyMarkup: menuKeyboard);
            //Stage = (int)Constants.Action.EditProfile;
            /*UserRepository.UpdateUserStage(message.From.Id, 6);
            _logger.LogInformation($"user({message.From.Id}): Stage updated: {6}");*/
        }
    }


    private static async Task EditProfile(Message message, ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken)
    {
        if (message.Text == "Редактировать фото")
        {
            var removeKeyboard = new ReplyKeyboardRemove();
            removeKeyboard = new ReplyKeyboardRemove();
            await botClient.SendTextMessageAsync(chat.Id, "Отправь новое фото",
                replyMarkup: removeKeyboard);
            UserRepository.UpdateUserStage(message.From.Id, 5);
        }

        if (message.Text == "Редактировать инфу")
        {
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                // first row
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Имя",
                        "name"),
                    InlineKeyboardButton.WithCallbackData("Возраст",
                        "age")
                },
                // second row
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Город",
                        "city"),
                    InlineKeyboardButton.WithCallbackData("О себе",
                        "about")
                }
            });

            var sentMessage = await botClient.SendTextMessageAsync(
                chat.Id,
                "Что ты хочешь изменить?",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }

        if (message.Text == "Назад")
        {
            ReplyKeyboardMarkup menuKeyboard = new(new[]
            {
                new KeyboardButton[] { "Редактировать профиль" },
                new KeyboardButton[] { "Просмотреть анкеты" },
                new KeyboardButton[] { "Отправить сообщение об ошибке" }
            });
            await botClient.SendTextMessageAsync(chat.Id, "Выбери действие",
                replyMarkup: menuKeyboard);
            UserRepository.UpdateUserStage(message.From.Id, 6);
            _logger.LogInformation($"user({message.From.Id}): Stage updated: {6}");
        }
    }

    
    /*private static async Task EnterCity(Message message, ITelegramBotClient botClient, Chat chat, UserEntity curUser)
    {
        try
        {
            curUser.Age = int.Parse(message.Text);
            UserRepository.SetUserAge(message.From.Id, int.Parse(message.Text));
            _logger.LogInformation($"user({message.From.Id}): updated age: {message.Text}");
            await botClient.SendTextMessageAsync(
                chat.Id,
                "Из какого ты города?");
            UserRepository.UpdateUserStage(message.From.Id, 2);
            _logger.LogInformation($"user({message.From.Id}): Stage updated: {2}");
        }
        catch (FormatException e)
        {
            await botClient.SendTextMessageAsync(chat.Id, "Введи корректный возраст");
            UserRepository.UpdateUserStage(message.From.Id, 1);
            _logger.LogInformation($"user({message.From.Id}): Stage updated: {1}");
        }
    }*/
    
    public static async Task EnterIsZodiacSignMatter(Message message, ITelegramBotClient botClient, Chat chat)
    {
        try
        {
          
          
        }
        catch (FormatException e)
        {
            //await botClient.SendTextMessageAsync(chat.Id, "Введи корректный возраст");
            UserRepository.UpdateUserStage(message.From.Id, 4);
            _logger.LogInformation($"user({message.From.Id}): Stage updated: {4}");
        }
    }
    
   
    
}