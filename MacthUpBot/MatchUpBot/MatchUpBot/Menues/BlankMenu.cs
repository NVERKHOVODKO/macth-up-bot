using Data;
using EntityFrameworkLesson.Repositories;
using MatchUpBot.Repositories;
using Microsoft.Extensions.Logging;
using Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApplication1.Menues;

public class BlankMenu
{
    public static readonly UserRepository UserRepository = new();
    public static readonly InterestWeightRepository InterestWeightRepository = new();
    
    private static ILogger<BlankMenu> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BlankMenu>();

    public BlankMenu(ILogger<BlankMenu> logger)
    {
        _logger = logger;
    }

    public static async Task HandleMessageTypeText(Message message, ITelegramBotClient botClient, Chat chat)
    {
        if (message.Text == "/delete")
        {
            await EditProfileRepository.DeleteProfile(message.From.Id, botClient, message.From.Username);
            var skipKeyboard = new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>
                {
                    new KeyboardButton[]
                    {
                        new("Регистрация")
                    }
                })
            {
                ResizeKeyboard = true
            };
            await botClient.SendTextMessageAsync(
                chat.Id,
                "Ваш аккаунт успешно удален",
                replyMarkup: skipKeyboard);
            _logger.LogInformation($"user({message.From.Id}) deleted");
            return;
        }

        
        var Stage = UserRepository.GetUserStage(message.From.Id);
        _logger.LogInformation($"Stage user({message.From.Id}): {Stage}");
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
            await EnterName(message.From.Id, botClient);
            return;
        }

        if (message.Text == "Вернуться в меню" &&
            UserRepository.GetUser(message.From.Id).Stage > (int)Action.AddAdditionalPhoto)
        {
            await UpdateStage(message.From.Id, (int)Action.EnterAction);
            await EnterAction(botClient, chat.Id);
            return;
        }

        if (message.Text == "/menu" && UserRepository.GetUser(message.From.Id).Stage > 10)
        {
            await UpdateStage(message.From.Id, (int)Action.EnterAction);
            await EnterAction(botClient, chat.Id);
            return;
        }
        
        Console.WriteLine(Stage);
        if (LikesMenu.GetLikerId(message.From.Id) != -1)
            await UpdateStage(message.From.Id, (int)Action.GetLikedBlank);
        switch (Stage)
        {
            case (int)Action.SetName:
                AddNameToDatabase(message, botClient, chat);
                await EnterAge(message, botClient, chat);
                break;
            case (int)Action.SetAge:
                if (!AddAgeToDatabase(message, botClient, chat)) break;
                await EnterCity(message, botClient, chat);
                break;
            case (int)Action.SetCity:
                AddCityToDatabase(message);
                await EnterAbout(message, botClient, chat);
                break;
            case (int)Action.SetSex:
                AddAboutToDatabase(message, botClient);
                await EnterSex(message, botClient, chat);
                break;
            case (int)Action.SetIsZodiacSignMatter:
                if (!AddZodiacSignToDatabase(message, chat, botClient)) break;
                await EnterPhoto(message, botClient, chat);
                break;
            case (int)Action.SetInterestedSex:
                await botClient.SendTextMessageAsync(message.From.Id, "Ответь на вопрос");
                break;
            case (int)Action.AddInterest:
                try
                {
                    if (UserRepository.GetUserInterestsById(message.From.Id).Count > 3)
                    {
                        await botClient.SendTextMessageAsync(chat.Id,
                            "Ты уже добавил максиальное количество интересов");
                        await UpdateStage(message.From.Id, (int)Action.EnterAction);
                        await EnterAction(botClient, chat.Id);
                        return;
                    }

                    UserRepository.AddInterestToUser(message.From.Id, int.Parse(message.Text), botClient);
                    
                }
                catch
                {
                    await botClient.SendTextMessageAsync(chat.Id, "Используй числа от 1 до 16");
                    _logger.LogInformation("Incorrect input");
                }

                break;
            case (int)Action.EditName:
                await SetNewName(message, botClient);
                await EnterAction(botClient, chat.Id);
                break;
            case (int)Action.ViewMyself:

            case (int)Action.EditAge:
                await SetNewAge(message, botClient);
                break;
            case (int)Action.EditCity:
                await SetNewCity(message, botClient);
                await EnterAction(botClient, chat.Id);
                break;
            case (int)Action.EditDescription:
                await SetNewAbout(message, botClient);
                await EnterAction(botClient, chat.Id);
                break;
            case (int)Action.AddMainPhoto:
                if (message.Text != "Отмена")
                {
                    await botClient.SendTextMessageAsync(message.From.Id,
                        "Отправь новое фото \nДля отмены введи «Отмена»");
                    break;
                }
                await UpdateStage(message.From.Id, (int)Action.EnterAction);
                await EnterAction(botClient, chat.Id);
                break;
            case (int)Action.AddAdditionalPhoto:
            {
                if (message.Text != "Отмена")
                {
                    await botClient.SendTextMessageAsync(message.From.Id,
                        "Отправь новое фото \nДля отмены введи «Отмена»");
                    break;
                }
                await UpdateStage(message.From.Id, (int)Action.EnterAction);
                await EnterAction(botClient, chat.Id);
                break;
            }
            case (int)Action.DeleteMainPhoto:
                if (message.Text != "Отмена")
                {
                    await DeleteUserPhoto(message, botClient,"main");
                    break;
                }

                await UpdateStage(message.From.Id, (int)Action.EnterAction);
                await EnterAction(botClient, chat.Id);
                break;
            case (int)Action.DeleteAdditionalPhoto:
                if (message.Text != "Отмена")
                {
                    await DeleteUserPhoto(message, botClient, "additional");
                    break;
                }

                await UpdateStage(message.From.Id, (int)Action.EnterAction);
                await EnterAction(botClient, chat.Id);
                break;
            case (int)Action.EnterAction:
                if (UserRepository.GetUser(chat.Id).GenderOfInterest == "N/A")
                {
                    await botClient.SendTextMessageAsync(chat.Id, "Сначала выбери,кто тебе нравится");
                    break;
                }

                await EnterAction(botClient, chat.Id);
                break;
            case (int)Action.GetFirstBlank:
                await AddReactionKeyboard(botClient, chat.Id);
                await UpdateStage(message.From.Id, (int)Action.GetBlank);
                ViewingProfilesMenu.ShowBlank(message.From.Id, botClient);//подбираем анкету
                GetBlankReaction(message, botClient, chat.Id);
                break;
            case (int)Action.GetBlank:
                GetBlankReaction(message, botClient, chat.Id);
                break;
            case (int)Action.GetLikedBlank:
                await PhotoRepository.SendLikerBlank(message.From.Id, botClient, LikesMenu.GetLikerId(message.From.Id));
                await UpdateStage(message.From.Id, (int)Action.GetBlank);
                break;
            default:
            {
                if (Stage == (int)Action.SetPhoto || Stage == (int)Action.SetAdditionalPhoto ||
                    Stage == (int)Action.AddMainPhoto)
                {
                    await botClient.SendTextMessageAsync(
                        chat.Id,
                        "Используй фото!");
                    return;
                }

                await botClient.SendTextMessageAsync(
                    chat.Id,
                    "Используй только текст!");
                return;
            }
        }
    }


    public static async Task
        ClearUserChat(ITelegramBotClient botClient, Message message) //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!111
    {
        var messageToDelete = await botClient.SendTextMessageAsync(message.From.Id, "Clearing chat...");

        var lastMessageId = messageToDelete.MessageId - 1;
        //var lastMessageId = messageToDelete.MessageId;

        while (lastMessageId > 0)
        {
            await botClient.DeleteMessageAsync(message.From.Id, lastMessageId);
            _logger.LogInformation($"Deleted message with ID: {lastMessageId}");
            lastMessageId--;
        }

        await botClient.DeleteMessageAsync(message.From.Id, messageToDelete.MessageId);
    }


    public static async Task GetBlankReaction(Message message, ITelegramBotClient botClient, long chatId)
    {
        _logger.LogInformation($"user({message.From.Id}): getted user()");
        switch (message.Text)
        {
            case "❤️":
                ViewingProfilesMenu.ShowBlank(message.From.Id, botClient);
                _logger.LogInformation(
                    $"user({message.From.Id}): liked user({UserRepository.GetUser(message.From.Id).LastShowedBlankTgId})");
                var vpmr = new ViewProfilesMenuRepository();
                vpmr.AddLike(UserRepository.GetUser(message.From.Id).LastShowedBlankTgId, message.From.Id, botClient);
                await new InterestWeightRepository().UpdateUserInterestWeightIncrement(message.From.Id,UserRepository.GetUserInterestsById(
                    UserRepository.GetUser(message.From.Id).LastShowedBlankTgId).Select(interest => interest.Name).ToList());
               
                break;
            case "👎":
                _logger.LogInformation(
                    $"user({message.From.Id}): disliked user({UserRepository.GetUser(message.From.Id).LastShowedBlankTgId})");
                await new InterestWeightRepository().UpdateUserInterestWeightDecrement(message.From.Id,UserRepository.GetUserInterestsById(
                    UserRepository.GetUser(message.From.Id).LastShowedBlankTgId).Select(interest => interest.Name).ToList());
                await ViewingProfilesMenu.ShowBlank(message.From.Id, botClient);
                break;
            case "🚪":
                await UpdateStage(message.From.Id, (int)Action.EnterAction);
                await EnterAction(botClient, chatId);
                return;
            case "📷":
                await PhotoRepository.SendUserAdditionalProfile(message.From.Id,
                    UserRepository.GetUser(message.From.Id).LastShowedBlankTgId, botClient);
                break;
        }
    }


    public static async Task EnterInterestedGender(CallbackQuery callbackQuery, ITelegramBotClient botClient)
    {
        var sexKeyboard = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Парни", "boys")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Девушки", "girls")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Неважно", "any")
                }
            });
        await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,"Кто тебе интересен?", replyMarkup: sexKeyboard);
    }


    private static async Task DeleteUserPhoto(Message message, ITelegramBotClient botClient, string folder)
    {
        int numberOfPhoto = int.Parse(message.Text);
        try
        {
            if (numberOfPhoto >
                PhotoRepository.GetFileCountInFolder($"../../../photos/{message.From.Id}/{folder}") || numberOfPhoto <= 0 )
            {
                await botClient.SendTextMessageAsync(
                    message.From.Id,
                    "Введи корректный номер");
                return;
            }
            
            if (numberOfPhoto < 1) throw new Exception();
            await PhotoRepository.DeletePhoto(folder, int.Parse(message.Text), message.From.Id);
            await botClient.SendTextMessageAsync(
                message.From.Id,
                "Фото удалено");
            await EnterAction(botClient, message.From.Id);
            await UpdateStage(message.From.Id, (int)Action.EnterAction);
        }
        catch (FormatException e)
        {
            botClient.SendTextMessageAsync(
                message.From.Id,
                "Введи корректный номер");
        }
    }

    private static async Task SetNewAbout(Message message, ITelegramBotClient botClient)
    {
        if (message.Text != "Отмена")
        {
            await botClient.SendTextMessageAsync(
                message.From.Id,
                $"Твое новое описание : {message.Text}");
            UserRepository.SetUserAbout(message.From.Id, message.Text);
        }

        await UpdateStage(message.From.Id, (int)Action.EnterAction);
    }

    private static async Task SetNewCity(Message message, ITelegramBotClient botClient)
    {
        if (message.Text != "Отмена")
        {
            await botClient.SendTextMessageAsync(
                message.From.Id,
                $"Твой новый город : {message.Text}");
            UserRepository.SetUserCity(message.From.Id, message.Text);
        }

        await UpdateStage(message.From.Id, (int)Action.EnterAction);
    }

    private static async Task SetNewAge(Message message, ITelegramBotClient botClient)
    {
        try
        {
            if (message.Text != "Отмена")
            {
                if (int.Parse(message.Text) < 1) throw new Exception();

                UserRepository.SetUserAge(message.From.Id, int.Parse(message.Text));
                _logger.LogInformation($"user({message.From.Id}): updated age: {message.Text}");
                await botClient.SendTextMessageAsync(
                    message.From.Id,
                    $"Твой новый возраст: {message.Text}");
                UserRepository.SetUserAge(message.From.Id, int.Parse(message.Text));
            }

            await EnterAction(botClient, message.From.Id);
            await UpdateStage(message.From.Id, (int)Action.EnterAction);
        }
        catch (FormatException e)
        {
            botClient.SendTextMessageAsync(
                message.From.Id,
                "Введи корректный возраст");
        }
        catch (Exception e)
        {
            botClient.SendTextMessageAsync(
                message.From.Id,
                "Возраст не должен быть отрицательным. Попробуй еще раз");
        }
    }

    private static async Task SetNewName(Message message, ITelegramBotClient botClient)
    {
        if (message.Text != "Отмена")
        {
            await botClient.SendTextMessageAsync(
                message.From.Id,
                $"Твое новое имя: {message.Text}");
            UserRepository.SetUserName(message.From.Id, message.Text);
        }

        await UpdateStage(message.From.Id, (int)Action.EnterAction);
    }


    public static async Task EnterMainPhotos(Message message, ITelegramBotClient botClient)
    {
        var number = PhotoRepository.GetFileCountInFolder($"../../../photos/{message.From.Id}/main/");
        if (number == 3)
        {
            var additionalPhoto = new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Да", "additional_photo_yes"),
                        InlineKeyboardButton.WithCallbackData("Нет", "additional_photo_no")
                    }
                });
            await botClient.SendTextMessageAsync(message.From.Id, "Ты хочешь добавить дополнительные фото (до 10)?",
                replyMarkup: additionalPhoto);
            await UpdateStage(message.From.Id, (int)Action.SetAdditionalPhoto);
            return;
        }

        await botClient.SendTextMessageAsync(message.From.Id, $"Ты отправил " +
                                                              $"{number} из 3");
        var menuKeyboard = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Да", "want_to_add_main_photo")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Нет", "dont_want_to_add_main_photo")
                }
            });
        await UpdateStage(message.From.Id, (int)Action.SetMainPhoto);
        await botClient.SendTextMessageAsync(message.From.Id, "Хочешь отправить еще фото?", replyMarkup: menuKeyboard);
    }

    public static async Task EnterAdditionalPhotos(Message message, ITelegramBotClient botClient)
    {
        var number = PhotoRepository.GetFileCountInFolder($"../../../photos/{message.From.Id}/additional/");
        if (number == 10)
        {
            await botClient.SendTextMessageAsync(message.From.Id, "Дополнительные фото отправлены.");
            await UpdateStage(message.From.Id, (int)Action.EnterAction);
            await EnterAction(botClient, message.From.Id);
            return;
        }

        await botClient.SendTextMessageAsync(message.From.Id, $"Ты отправил " +
                                                              $"{number} из 10");
        var additionalPhoto = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Да", "additional_photo_yes"),
                    InlineKeyboardButton.WithCallbackData("Нет", "additional_photo_no")
                }
            });
        await botClient.SendTextMessageAsync(message.From.Id, "Хочешь отправить еще фото?",
            replyMarkup: additionalPhoto);
    }

    public static async Task HandleMessageTypePhoto(Message message, ITelegramBotClient botClient, Chat chat)
    {
        var Stage = UserRepository.GetUserStage(message.From.Id);
        if (Stage != (int)Action.SetPhoto && Stage != (int)Action.SetMainPhoto &&
            Stage != (int)Action.SetAdditionalPhoto && Stage != (int)Action.AddMainPhoto &&
            Stage != (int)Action.AddAdditionalPhoto) //Refactor this
        {
            await botClient.SendTextMessageAsync(chat.Id, "Используй текст");
            return;
        }

        await PhotoRepository.HandlePhotoMessage(message, botClient);
    }

    public static async Task EnterAction(ITelegramBotClient botClient, long tgId)
    {
        var removeKeyboard = new ReplyKeyboardRemove();
        var message = await botClient.SendTextMessageAsync(tgId, "Выбери действие", replyMarkup: removeKeyboard);
        var messageIdToRemove = message.MessageId;
        await botClient.DeleteMessageAsync(tgId, messageIdToRemove);
        var menuKeyboard = new InlineKeyboardMarkup( new List<InlineKeyboardButton[]>
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Редактировать профиль", "edit_profile")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Просмотреть анкеты", "view_profiles")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Посмотреть свою анкету", "view_myself")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Просмотреть доп. фото", "view_add_photo")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Получить VIP", "get_vip")
            }
        });
        await botClient.SendTextMessageAsync(tgId, "Выбери действие", replyMarkup: menuKeyboard);
    }


    public static async Task AddReactionKeyboard(ITelegramBotClient botClient, long chatId)
    {
        var blankReactionKeyboardMarkup = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[] { new("❤️"), new("👎"), new("🚪"), new("📷") }
            })
        {
            ResizeKeyboard = true
        };
        await botClient.SendTextMessageAsync(
            chatId,
            "Инструкция по пользованию: \n \n" +
            "❤️ - Анкета понравилась \n" +
            "👎 - Анкета не понравилась \n" +
            "🚪 - Вернуться в меню \n" +
            "📷 - Посмотреть еще фото",
            replyMarkup: blankReactionKeyboardMarkup);
    }


    public static async Task EnterName(long tgId, ITelegramBotClient botClient)
    {
        var Stage = 0;
        await UpdateStage(tgId, (int)Action.SetName);
        var removeKeyboard = new ReplyKeyboardRemove();

        await botClient.SendTextMessageAsync(
            tgId,
            "Как тебя зовут?",
            replyMarkup: removeKeyboard);
    }

    private static async Task AddNameToDatabase(Message message, ITelegramBotClient botClient, Chat chat)
    {
        if (message.Text[0] == '/')
        {
            await botClient.SendTextMessageAsync(
                chat.Id,
                "Введи корректное имя: ");
        }
        else if (message.Text.Length > 20)
        {
            await botClient.SendTextMessageAsync(
                chat.Id,
                "Имя не должно быть больше 20 символов. Введи корректное имя: ");
        }
        else
        {
            await UpdateStage(message.From.Id,(int)Action.SetAge);
            UserRepository.SetUserName(message.From.Id, message.Text);
            _logger.LogInformation($"user({message.From.Id}): updated name: {message.Text}");
        }
    }

    private static async Task EnterAge(Message message, ITelegramBotClient botClient, Chat chat)
    {
        if (message.Text.Length > 20) return;
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Сколько тебе лет?");
    }

    private static bool AddAgeToDatabase(Message message, ITelegramBotClient botClient, Chat chat)
    {
        try
        {
            if (int.Parse(message.Text) < 1) throw new Exception();

            UserRepository.SetUserAge(message.From.Id, int.Parse(message.Text));
            _logger.LogInformation($"user({message.From.Id}): updated age: {message.Text}");
            return true;
        }
        catch (FormatException e)
        {
            botClient.SendTextMessageAsync(
                chat.Id,
                "Введи корректный возраст");
            return false;
        }
        catch (Exception e)
        {
            botClient.SendTextMessageAsync(
                chat.Id,
                "Возраст не должен быть отрицательным. Попробуй еще раз");
            return false;
        }
    }

    private static async Task EnterCity(Message message, ITelegramBotClient botClient, Chat chat)
    {
        try
        {
            await botClient.SendTextMessageAsync(
                chat.Id,
                "Из какого ты города?");
            await UpdateStage(message.From.Id, (int)Action.SetCity);
        }
        catch (FormatException e)
        {
            await botClient.SendTextMessageAsync(chat.Id, "Введи корректный возраст");
            await UpdateStage(message.From.Id, (int)Action.SetAge);
        }
    }

    private static void AddCityToDatabase(Message message)
    {
        if (message.Text.Length > 25) return;
        UserRepository.SetUserCity(message.From.Id, message.Text);
        _logger.LogInformation($"user({message.From.Id}): updated city: {message.Text}");
    }

    private static async Task EnterAbout(Message message, ITelegramBotClient botClient, Chat chat)
    {
        if (message.Text.Length > 25)
        {
            await botClient.SendTextMessageAsync(
                chat.Id,
                "Название города не может содержать больше 25 символов. Введи корректное название: ");
            return;
        }

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
        await UpdateStage(message.From.Id, (int)Action.SetSex);
    }

    private static async void AddAboutToDatabase(Message message, ITelegramBotClient botClient)
    {
        if (message.Text.Length > 300)
        {
            await botClient.SendTextMessageAsync(
                message.From.Id,
                "Твое описание не должно быть больше 300 символов. Введи корректное описание:");
        }
        else
        {
            if (message.Text == "Пропустить")
            {
                UserRepository.SetUserAbout(message.From.Id, string.Empty);
                _logger.LogInformation($"user({message.From.Id}): skipped entering about");
                await UpdateStage(message.From.Id, (int)Action.SetSex);
            }
            else
            {
                UserRepository.SetUserAbout(message.From.Id, message.Text);
                _logger.LogInformation($"user({message.From.Id}): updated about: {message.Text}");
                await UpdateStage(message.From.Id, (int)Action.SetSex);
            }
        }
    }

    private static async Task EnterSex(Message message, ITelegramBotClient botClient, Chat chat)
    {
        if (message.Text.Length > 300) return;
        var remove = new ReplyKeyboardRemove();
        var about = UserRepository.GetUserAbout(message.From.Id);
        if (about == "")
            await botClient.SendTextMessageAsync(chat.Id, "Ты не добавил описания о себе", replyMarkup: remove);
        else
            await botClient.SendTextMessageAsync(chat.Id, $"Твое описание о себе:{about}", replyMarkup: remove);
        InlineKeyboardMarkup sexKeyboard = new(new[]
        {
            InlineKeyboardButton.WithCallbackData("Мужчина", "man"),
            InlineKeyboardButton.WithCallbackData("Женщина", "woman")
        });
        await botClient.SendTextMessageAsync(chat.Id, "Какой у тебя гендер?", replyMarkup: sexKeyboard);
    }

    private static bool AddZodiacSignToDatabase(Message message, Chat chat,
        ITelegramBotClient botClient)
    {
        if (UserRepository.GetUserGender(message.From.Id) == "N/A" ||
            CallbackDataRepository.GetIsZodiacMattersEntered() == false)
        {
            botClient.SendTextMessageAsync(chat.Id, "Сначала ответь на вопросы");
            return false;
        }

        if (!IsZodiacSignValid(message.Text))
        {
            botClient.SendTextMessageAsync(chat.Id, "Введи корректный знак задиака");
            UpdateStage(message.From.Id, (int)Action.SetIsZodiacSignMatter);
            return false;
        }
        var user = UserRepository.GetUser(message.From.Id);
        user.ZodiacSign = message.Text.ToLower();
        UserRepository.SetUserZodiacSign(message.From.Id, user.ZodiacSign);
        _logger.LogInformation($"user({message.From.Id}): updated ZodiacSign: {message.Text}");
        return true;
    }

    private static async Task EnterPhoto(Message message, ITelegramBotClient botClient, Chat chat)
    {
        var removeKeyboard = new ReplyKeyboardRemove();
        await botClient.SendTextMessageAsync(chat.Id, "Скинь свою фото",
            replyMarkup: removeKeyboard);
        await UpdateStage(message.From.Id, (int)Action.SetPhoto);
    }


    public static bool IsZodiacSignValid(string zodiacSign)
    {
        var zodiacSigns = new List<string>
        {
            "овен", "телец", "близнецы", "рак", "лев", "дева", "весы", "скорпион", "стрелец", "козерог", "водолей",
            "рыбы"
        };
        var lowerCaseZodiacSign = zodiacSign.ToLower();
        return zodiacSigns.Contains(lowerCaseZodiacSign);
    }

    public static async Task UpdateStage(long tgId, int stage)
    {
        UserRepository.UpdateUserStage(tgId, stage);
        _logger.LogInformation("user({TgId}): Stage updated: {Stage}", tgId, stage);
    }
}