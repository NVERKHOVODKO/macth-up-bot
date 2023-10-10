//6610584532:AAHyYTG_Rz96QfQEc7H-Dk-7iHHb2PeQN0E

using System.Net;
using System.Reflection.Metadata;
using Entities;
using EntityFrameworkLesson;
using EntityFrameworkLesson.Repositories;
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
        _receiverOptions = new ReceiverOptions{
            AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery },
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
            /*using (var context = new Context())
            {
                await context.UserEntity.AddAsync(new UserEntity(1,"sdf", "2", "string country", "string city", "string gender", "string photo", "string tgUsername", "string tgChatId"));
                await context.SaveChangesAsync();
            }*/
            if (update.Type == UpdateType.CallbackQuery)
            {
                Console.WriteLine("dada");
            }
            switch (update.Type)
            {
                case UpdateType.CallbackQuery:
                {
                    // Переменная, которая будет содержать в себе всю информацию о кнопке, которую нажали
                    var callbackQuery = update.CallbackQuery;
                    
                    // Аналогично и с Message мы можем получить информацию о чате, о пользователе и т.д.
                    var user = callbackQuery.From;
                    
                    // Вот тут нужно уже быть немножко внимательным и не путаться!
                    // Мы пишем не callbackQuery.Chat , а callbackQuery.Message.Chat , так как
                    // кнопка привязана к сообщению, то мы берем информацию от сообщения.
                    var chat = callbackQuery.Message.Chat; 
                    
                    // Добавляем блок switch для проверки кнопок
                    switch (callbackQuery.Data)
                    {
                        // Data - это придуманный нами id кнопки, мы его указывали в параметре
                        // callbackData при создании кнопок. У меня это button1, button2 и button3

                        case "name":
                        {
                            // В этом типе клавиатуры обязательно нужно использовать следующий метод
                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                            // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку
                            
                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Введите новое имя");
                            return;
                        }
                        
                        case "age":
                        {
                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Введите новый возраст");
                            return;
                        }
                        
                        case "city":
                        {
                           
                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                            
                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Введите новый город");
                            return;
                        }
                        case "about":
                        {
                           
                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                            
                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Расскажите о себе");
                            return;
                        }
                    }
                    
                    return;
                }
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
                                    await botClient.SendTextMessageAsync(chat.Id, "Скинь свою фото", replyMarkup: removeKeyboard);
                                    curUser.TelegramId = Int32.Parse(message.From.Id.ToString());
                                    curUser.Username = message.From.Username;
                                    Stage = 5;
                                    break;
                                case 6:
                                    if (message.Text == "Редактировать профиль")
                                    {
                                        removeKeyboard = new ReplyKeyboardRemove();
                                        await botClient.SendTextMessageAsync(chat.Id, "Давай редактировать", replyMarkup: removeKeyboard);
                                        ReplyKeyboardMarkup menuKeyboard = new(new[]
                                        {
                                               
                                            new KeyboardButton[] {"Редактировать фото"},
                                            new KeyboardButton[]{"Редактировать инфу"},
                                            new KeyboardButton[]{"Назад"}
                                               
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
                                    if (message.Text == "Редактировать инфу")
                                    {
                                        
                                        InlineKeyboardMarkup inlineKeyboard = new(new[]
                                        {
                                            // first row
                                            new []
                                            {
                                                InlineKeyboardButton.WithCallbackData(text: "Имя", callbackData: "name"),
                                                InlineKeyboardButton.WithCallbackData(text: "Возраст", callbackData: "age"),
                                            },
                                            // second row
                                            new []
                                            {
                                                InlineKeyboardButton.WithCallbackData(text: "Город", callbackData: "city"),
                                                InlineKeyboardButton.WithCallbackData(text: "О себе", callbackData: "about"),
                                            },
                                        });
                                        
                                        Message sentMessage = await botClient.SendTextMessageAsync(
                                            chatId: chat.Id,
                                            text: "Что ты хочешь изменить?",
                                            replyMarkup: inlineKeyboard,
                                            cancellationToken: cancellationToken);

                                    }
                                    if (message.Text == "Назад")
                                    {
                                        ReplyKeyboardMarkup menuKeyboard = new(new[]
                                            {
                                               
                                                    new KeyboardButton[] {"Редактировать профиль"},
                                                    new KeyboardButton[]{"Просмотреть анкеты"},
                                                    new KeyboardButton[]{"Отправить сообщение об ошибке"}
                                               
                                            });
                                        await botClient.SendTextMessageAsync(chat.Id, "Выбери действие", replyMarkup: menuKeyboard);
                                        Stage = 6;
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
                            await PhotoRepository.HandlePhotoMessage(message, botClient, curUser);
                            curUser.PrintToConsole();
                            Stage = 6;
                            
                            ReplyKeyboardMarkup menuKeyboard = new(new[]
                            {
                                               
                                new KeyboardButton[] {"Редактировать профиль"},
                                new KeyboardButton[]{"Просмотреть анкеты"},
                                new KeyboardButton[]{"Отправить сообщение об ошибке"}
                                               
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


    /*
    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] Имба можно где-нибудь применять
    {
        KeyboardButton.WithRequestLocation("Share Location"), - При нажатии кидает местонахождение боту
        KeyboardButton.WithRequestContact("Share Contact"), - При нажатии кидает номер телефона боту
    });

    Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chat.Id,
        text: "Who or Where are you?",
        replyMarkup: replyKeyboardMarkup,
        cancellationToken: cancellationToken);
        */

}