//6610584532:AAHyYTG_Rz96QfQEc7H-Dk-7iHHb2PeQN0E

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotExperiments.Models;

class Program
{
    private static ITelegramBotClient _botClient;
    private static ReceiverOptions _receiverOptions;
    private static int Stage = -1;

    private static TelegramBotExperiments.Models.User curUser = new TelegramBotExperiments.Models.User();

    static async Task Main()
    {
        _botClient = new TelegramBotClient("6610584532:AAHyYTG_Rz96QfQEc7H-Dk-7iHHb2PeQN0E");
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message },
            ThrowPendingUpdates = true,
        };

        using var cts = new CancellationTokenSource();

        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"{me.FirstName} запущен!");

        await Task.Delay(-1);
    }

    
    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    var message = update.Message;
                    var chat = message.Chat;
                    
                    switch (message.Type)
                    {
                        case MessageType.Text:
                        {
                            var replyKeyboard = new ReplyKeyboardMarkup(
                                new List<KeyboardButton[]>
                                {
                                    new KeyboardButton[]
                                    {
                                        new KeyboardButton("Start!"),
                                        new KeyboardButton("Что может бот?"),
                                    },
                                })
                            {
                                ResizeKeyboard = true,
                            };
                            if (Stage == -1)
                            {
                                Stage = 0;
                                await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    $"Как тебя зовут?");
                                return;
                            }
                            Console.WriteLine(Stage);
                            switch (Stage)
                            {
                                case 0:
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        $"Сколько тебе лет?");
                                    curUser.ProfileName = message.Text;
                                    Stage = 1;
                                    break;
                                case 1:
                                    try
                                    {
                                        curUser.Age = Int32.Parse(message.Text.ToString());
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            $"Из какого ты города?");
                                        Stage = 2;
                                        break;
                                    }
                                    catch (FormatException e)
                                    {
                                        await botClient.SendTextMessageAsync(chat.Id, "Введи корректный возраст");
                                        Stage = 1;
                                        break;
                                    }
                                  
                                case 2:
                                    var skipKeyboard = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>
                                        {
                                            new KeyboardButton[]
                                            {
                                                new KeyboardButton("Пропустить"),
                                            },
                                        })
                                    {
                                        ResizeKeyboard = true,
                                    };
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Расскажи о себе?",
                                        replyMarkup: skipKeyboard);
                                    curUser.City = message.Text;
                                    Stage = 3;
                                    break;
                                case 3:
                                    var sexKeyboard = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>
                                        {
                                            new KeyboardButton[]
                                            {
                                                new KeyboardButton("Мужчина"),
                                                new KeyboardButton("Женщина"),
                                            },
                                        })
                                    {
                                        ResizeKeyboard = true,
                                    };
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Какой у тебя гендер?",
                                        replyMarkup: sexKeyboard);
                                    curUser.About = message.Text;
                                    Stage = 4;
                                    break;
                                case 4:
                                    await botClient.SendTextMessageAsync(chat.Id, "Скинь свою секс фото");
                                    //Сохранение фото в директорию Photos
                                    Stage = 5;
                                    break;
                                case 5:
                                    var removeKeyboard = new ReplyKeyboardRemove();
                                    curUser.Sex = message.Text;
                                    Stage = 6;
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        $"Твоя анкета выглядит так:\n" +
                                        $"{curUser.ProfileName}, {curUser.Age} лет, {curUser.City}\n" +
                                        $"{curUser.About}",
                                        replyMarkup: removeKeyboard);

                                    curUser.PrintToConsole();
                                    break;
                            }
                            curUser.TelegramId = Int32.Parse(message.From.Id.ToString());
                            curUser.Username = message.From.Username;

                            return;
                        }
                        
                        default:
                        {
                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Используй только текст!");
                            return;
                        }
                    }

                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}