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

    public static async Task HandleMessageTypePhoto(Message message, ITelegramBotClient botClient, Chat chat, UserEntity curUser)
    {
        int Stage = UserRepository.GetUserStage(message.From.Id);
        if (Stage != 5)
        {
            await botClient.SendTextMessageAsync(chat.Id, "Используй текст");
            return;
        }

        await PhotoRepository.HandlePhotoMessage(message, botClient, curUser);
        curUser.PrintToConsole();
        UserRepository.UpdateUserStage(message.From.Id, 6);
        _logger.LogInformation($"user({message.From.Id}): Stage updated: {6}");

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
    

    private static async Task EnterName(Message message, ITelegramBotClient botClient, Chat chat, UserEntity curUser)
    {
        curUser.Name = message.Text;
        UserRepository.SetUserName(message.From.Id, message.Text);
        _logger.LogInformation($"user({message.From.Id}): updated name: {message.Text}");
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Сколько тебе лет?");
        UserRepository.UpdateUserStage(message.From.Id, 1);
        _logger.LogInformation($"user({message.From.Id}): Stage updated: {1}");
    }

    private static async Task EnterCity(Message message, ITelegramBotClient botClient, Chat chat, UserEntity curUser)
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
    }

    private static async Task EnterAbout(Message message, ITelegramBotClient botClient, Chat chat, UserEntity curUser)
    {
        curUser.City = message.Text;
        UserRepository.SetUserCity(message.From.Id, message.Text);
        _logger.LogInformation($"user({message.From.Id}): updated city: {message.Text}");
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

    private static async Task EnterSex(Message message, ITelegramBotClient botClient, Chat chat, UserEntity curUser)
    {
        curUser.About = message.Text;
        if (message.Text.ToLower() == "пропустить")
        {
            UserRepository.SetUserAbout(message.From.Id, string.Empty);
            _logger.LogInformation($"user({message.From.Id}): skipped entering about");
        }
        else
        {
            UserRepository.SetUserAbout(message.From.Id, message.Text);
            _logger.LogInformation($"user({message.From.Id}): updated about: {message.Text}");
        }
        var sexKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new("Мужчина"),
                    new("Женщина")
                }
            })
        {
            ResizeKeyboard = true
        };
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Какой у тебя гендер?",
            replyMarkup: sexKeyboard);
        UserRepository.UpdateUserStage(message.From.Id, 4);
        _logger.LogInformation($"user({message.From.Id}): Stage updated: {4}");
    }

    private static async Task EnterPhoto(Message message, ITelegramBotClient botClient, Chat chat, UserEntity curUser)
    {
        curUser.Gender = message.Text;
        UserRepository.SetUserGender(message.From.Id, message.Text);
        _logger.LogInformation($"user({message.From.Id}): updated gender: {message.Text}");
        var removeKeyboard = new ReplyKeyboardRemove();
        await botClient.SendTextMessageAsync(chat.Id, "Скинь свою фото",
            replyMarkup: removeKeyboard);
        curUser.TgId = int.Parse(message.From.Id.ToString());
        curUser.TgUsername = message.From.Username;
        UserRepository.UpdateUserStage(message.From.Id, 5);
        _logger.LogInformation($"user({message.From.Id}): Stage updated: {5}");
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
            Stage = 0;
            UserRepository.UpdateUserStage(message.From.Id, 0);
            _logger.LogInformation($"Stage user({tgId}): updated {Stage}");
            await botClient.SendTextMessageAsync(
                chat.Id,
                "Как тебя зовут?");
            return;
        }


        Console.WriteLine(Stage);

        switch (Stage)
        {
            case 0:
                await EnterName(message, botClient, chat, curUser);
                break;
            case 1:
                await EnterCity(message, botClient, chat, curUser);
                break;
            case 2:
                await EnterAbout(message, botClient, chat, curUser);
                break;
            case 3:
                await EnterSex(message, botClient, chat, curUser);
                break;
            case 4:
                await EnterPhoto(message, botClient, chat, curUser);
                break;
            case 6:
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
}