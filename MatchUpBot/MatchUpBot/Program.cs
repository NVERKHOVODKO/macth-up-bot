//6610584532:AAHyYTG_Rz96QfQEc7H-Dk-7iHHb2PeQN0E

using System.Net;
using System.Reflection.Metadata;
using EntityFrameworkLesson;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotExperiments.Models;
using File = System.IO.File;

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


    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
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
                                    curUser.ProfileName = message.Text;
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        $"Сколько тебе лет?");

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
                                    curUser.City = message.Text;
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

                                    Stage = 3;
                                    break;
                                
                                case 3:
                                    curUser.About = message.Text;
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

                                    Stage = 4;
                                    break;
                                
                                case 4:
                                    curUser.Sex = message.Text;
                                    var removeKeyboard = new ReplyKeyboardRemove();
                                    await botClient.SendTextMessageAsync(chat.Id, "Скинь свою секс фото", replyMarkup: removeKeyboard);
                                    curUser.TelegramId = Int32.Parse(message.From.Id.ToString());
                                    curUser.Username = message.From.Username;
                                    Stage = 5;
                                    break;
                                case 6:
                                    if (message.Text == "Редактировать профиль")
                                    {
                                        removeKeyboard = new ReplyKeyboardRemove();
                                        await botClient.SendTextMessageAsync(chat.Id, "Давай редактировать", replyMarkup: removeKeyboard);
                                        var menuKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>
                                            {
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("Редактировать фото"),
                                                    new KeyboardButton("Редактировать инфу"),
                                                    new KeyboardButton("Назад")
                                                },
                                            });
                                        await botClient.SendTextMessageAsync(chat.Id, "Выбери что хочешь изменить", replyMarkup: menuKeyboard);
                                        Stage = 7;
                                    }
                                    break;
                                case 7:
                                    if (message.Text == "Редактировать фото")
                                    {
                                        removeKeyboard = new ReplyKeyboardRemove();
                                        await botClient.SendTextMessageAsync(chat.Id, "Отправь новое фото",
                                            replyMarkup: removeKeyboard);
                                        Stage = 5;
                                    }
                                    break;
                                
                                default:
                                {
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Используй только текст!");
                                    return;
                                }
                            }
                            
                        }
                            return;
                        case MessageType.Photo:
                        {
                            if (Stage != 5)
                            {
                                await botClient.SendTextMessageAsync(chat.Id, "Мне пох на твое фото");
                                return;
                            }
                            
                            HandlePhotoMessage(message, botClient);
                            curUser.PrintToConsole();
                            Stage = 6;
                            
                            var menuKeyboard = new ReplyKeyboardMarkup(
                                new List<KeyboardButton[]>
                                {
                                    new KeyboardButton[]
                                    {
                                        new KeyboardButton("Редактировать профиль"),
                                        new KeyboardButton("Просмотреть анкеты"),
                                        new KeyboardButton("Отправить сообщение об ошибке")
                                    },
                                });
                            await botClient.SendTextMessageAsync(chat.Id, "Выбери действие", replyMarkup: menuKeyboard);
                            
                            break;
                        }
                    }

                    break;
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

    private static async Task HandlePhotoMessage(Message message, ITelegramBotClient botClient)
    {
        try
        {
            if (message.Photo != null)
            {
                Console.WriteLine("Фотография найдена.");
                var photo = message.Photo.LastOrDefault();
                var file = await botClient.GetFileAsync(photo.FileId);
                
                string filePath = $"../../../photos/{curUser.TelegramId}.jpg";
                
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await botClient.DownloadFileAsync(file.FilePath, fileStream);
                    fileStream.Close();
                } 
                
                await using Stream stream = System.IO.File.OpenRead(filePath);
                
                    string caption = $"Твоя анкета выглядит так:\n" +
                    $"{curUser.ProfileName}, {curUser.Age} лет, {curUser.City}\n" +
                        $"{curUser.About}";
                    await botClient.SendPhotoAsync(
                        chatId: message.Chat.Id,
                        InputFile.FromStream(stream,"photo.jpg"),
                        caption: caption,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                    );
                    
                Console.WriteLine("Файл успешно скачан.");
            }
            else
            {
                Console.WriteLine("Сообщение не содержит фотографии.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при скачивании файла: {e.Message}");
        }
    }

    /*public static async Task EditMenu(Message message, ITelegramBotClient botClient)
    {
        var wantToChangePhoto = 0;
        if (message.Type == MessageType.Photo)
        {
            if (wantToChangePhoto == 0)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Зачем мне твое фото");
                return;
            }
            HandlePhotoMessage(message, botClient);
        }
        switch (message.Text)
        {
            case "Редактировать профиль":
                var removeKeyboard = new ReplyKeyboardRemove();

                var editMenu = new ReplyKeyboardMarkup(new List<KeyboardButton>
                {
                    new KeyboardButton("Редактировать фото"),
                    new KeyboardButton("Редактировать основную информацию"),
                    new KeyboardButton("Назад")
                }); 
                await botClient.SendTextMessageAsync(message.Chat.Id, "Давай редактировать",replyMarkup: editMenu);
                break;
            
                    case "Редактировать фото":
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Отправь новое фото");
                        wantToChangePhoto = 1;
                        break;
                    }
                    case "Назад":
                    {
                        break;
                    }
                    
            case "Просмотреть анкеты":
                await botClient.SendTextMessageAsync(message.Chat.Id, "Ты крутой");
                break;
            
        }
    }*/
}