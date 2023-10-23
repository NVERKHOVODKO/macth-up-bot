using ConsoleApplication1.Menues;
using Data;
using MatchUpBot.Repositories;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Repositories;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace EntityFrameworkLesson.Repositories;

public class CallbackDataRepository
{
    private static readonly ILogger<CallbackDataRepository> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CallbackDataRepository>();

    private static string _folder = "main";
    private static bool _isZodiacMattersEntered;

    public static async Task SetGenger(string gender, ITelegramBotClient botClient, long userId,
        CallbackQuery callbackQuery)
    {
        try
        {
            if (BlankMenu.UserRepository.GetUserStage(userId) != 4)
            {
                await botClient.SendTextMessageAsync(userId, "Куда ты тыкаешь, аболтус");
                return;
            }

            BlankMenu.UserRepository.SetUserGender(userId, gender);
            _logger.LogInformation($"user({userId}): updated gender: {gender}");

            InlineKeyboardMarkup boolKeyboard = new(new[]
            {
                InlineKeyboardButton.WithCallbackData("Да", "zodiacMatters"),
                InlineKeyboardButton.WithCallbackData("Нет", "zodiacDoesntMatters")
            });
            await botClient.EditMessageTextAsync(userId, callbackQuery.Message.MessageId,
                "Для тебя важен знак зодиака?", replyMarkup: boolKeyboard);
        }
        catch (ApiRequestException e)
        {
            await botClient.SendTextMessageAsync(userId, "Не тыкай туда, бродяга");
        }
    }

    public static async Task SetZodiacMatters(long tgId, CallbackQuery callbackQuery, ITelegramBotClient botClient,
        bool matters)
    {
        if (BlankMenu.UserRepository.GetUserStage(tgId) != 4)
        {
            await botClient.SendTextMessageAsync(tgId, "Куда ты тыкаешь, аболтус");
            return;
        }

        await EnterZodiacSign(botClient, callbackQuery.Message.Chat, tgId);
        BlankMenu.UserRepository.SetUserIsZodiacSignMatters(tgId, matters);
        UpdateStage(tgId, 4);
        _isZodiacMattersEntered = true;
    }

    public static async Task HandleCallBackQuery(ITelegramBotClient botClient, Update update)
    {
        var callbackQuery = update.CallbackQuery;
        var user = callbackQuery.From;

        switch (callbackQuery.Data)
        {
            case "man":
            {
                await SetGenger("Мужчина", botClient, user.Id, callbackQuery);
                break;
            }
            case "woman":
            {
                await SetGenger("Женщина", botClient, user.Id, callbackQuery);
                break;
            }
            case "boys":
            {
                var ur = new UserRepository();
                ur.SetUserInterestedGender(callbackQuery.From.Id, "Парни");
                await BlankMenu.EnterAction(botClient,callbackQuery.From.Id);
                break;
            }
            case "girls":
            {
                var ur = new UserRepository();
                ur.SetUserInterestedGender(callbackQuery.From.Id, "Девушки");
                await BlankMenu.EnterAction(botClient,callbackQuery.From.Id);
                break;
            }
            case "any":
            {
                var ur = new UserRepository();
                ur.SetUserInterestedGender(callbackQuery.From.Id, "Неважно");
                await BlankMenu.EnterAction(botClient,callbackQuery.From.Id);
                break;
            }
            case "zodiacMatters":
            {
                await SetZodiacMatters(user.Id, callbackQuery, botClient, true);
                break;
            }
            case "zodiacDoesntMatters":
            {
                await SetZodiacMatters(user.Id, callbackQuery, botClient, false);
                break;
            }
            case "want_to_add_main_photo":
            {
                if (PhotoRepository.GetFileCountInFolder($"../../../photos/{callbackQuery.From.Id}/main") == 3)
                {
                    UpdateStage(user.Id, (int)Action.SetMainPhoto);
                    break;
                }

                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Скинь еще главную фото");
                _folder = "main";
                break;
            }
            case "dont_want_to_add_main_photo":
            {
                if (BlankMenu.UserRepository.GetUserStage(callbackQuery.From.Id) == (int)Action.SetMainPhoto)
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
                    await botClient.SendTextMessageAsync(callbackQuery.From.Id,
                        "Ты хочешь добавить дополнительные фото (до 10)?", replyMarkup: additionalPhoto);
                    UpdateStage(user.Id, (int)Action.SetAdditionalPhoto);
                }

                break;
            }
            case "additional_photo_yes":
            {
                _folder = "additional";
                if (PhotoRepository.GetFileCountInFolder($"../../../photos/{callbackQuery.From.Id}/additional") == 10)
                {
                    UpdateStage(user.Id, (int)Action.SetInterestedSex);
                    break;
                }

                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Скинь еще дополнительную фото");

                await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Отправь дополнительные фото");
            }
                break;
            case "additional_photo_no":
                await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Теперь отправь любое сообщение");
                UpdateStage(user.Id, (int)Action.SetInterestedSex);
                break;
            case "view_add_photo":
                if (PhotoRepository.GetFileCountInFolder($"../../../photos/{callbackQuery.From.Id}/additional") == 0)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Ты не добавил дополнительных фото");
                    break;
                }

                await PhotoRepository.SendUserAdditionalProfile(callbackQuery.From.Id,callbackQuery.From.Id, botClient);
                await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Введи текст");
                break;
            case "view_profiles":
                UpdateStage(user.Id, 20);
                await botClient.SendTextMessageAsync(
                    callbackQuery.From.Id,
                    "Напиши что-то");
                break;
            case "edit_profile":
                EditProfileRepository.SendEditKeyboard(botClient, callbackQuery.From.Id, callbackQuery);
                break;
            case "change_name":
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Введи новое имя. \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id,(int)Action.EditName);
                break;
            case "change_age":
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Введи новый возраст. \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id,(int)Action.EditAge);
                break;
            case "change_city":
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Введи новый город. \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id,(int)Action.EditCity);
                break;
            case "change_about":
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Введи новое описание. \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id,(int)Action.EditDescription);
                break;
            case "change_photo":
                await EditProfileRepository.EditKeyboardToPhotoChoice(botClient, callbackQuery.From.Id, callbackQuery);
                break;
            case "add_additional_photos":
                await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Отправь новое дополнительное фото \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id,(int)Action.AddAdditionalPhoto);
                break;
            case "delete_main_photos":
                if (PhotoRepository.GetFileCountInFolder($"../../../photos/{callbackQuery.From.Id}/main") == 1)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Нельзя удалить последнее фото");
                    break;
                }
                await PhotoRepository.SendUserMainProfile(callbackQuery.From.Id, botClient);
                await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Отправь номер фото,которое хочешь удалить \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id,(int)Action.DeleteMainPhoto);
                break;
            case "delete_additional_photos":
                if (PhotoRepository.GetFileCountInFolder($"../../../photos/{callbackQuery.From.Id}/additional") == 0)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Ты не добавил доп фото");
                    break;
                }
                await PhotoRepository.SendUserAdditionalProfile(callbackQuery.From.Id,callbackQuery.From.Id, botClient);
                await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Отправь номер фото,которое хочешь удалить \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id,(int)Action.DeleteAdditionalPhoto);
                break;
            case "back_to_edit":
            {
                EditProfileRepository.SendEditKeyboard(botClient,callbackQuery.From.Id,callbackQuery);
                break;
            }
            case  "back_to_action":
                await EditProfileRepository.EditKeyboardToAction(botClient, callbackQuery.From.Id, callbackQuery);
                break;
            case "back_to_photo":
                await EditProfileRepository.EditKeyboardToPhotoChoice(botClient, callbackQuery.From.Id, callbackQuery);
                break;
            case "edit_main_photos":
                _folder = "main";
                var editMain = new InlineKeyboardMarkup(
                    new List<InlineKeyboardButton[]>
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Добавить", "add_main_photos")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Удалить", "delete_main_photos")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Назад", "back_to_photo")
                        }
                    });
                await botClient.EditMessageTextAsync(callbackQuery.From.Id,callbackQuery.Message.MessageId,
                    "Что ты хочешь сделать с основными фото?", replyMarkup: editMain);
                break;
            case "edit_additional_photos":
                _folder = "additional";
                var editAdditional = new InlineKeyboardMarkup(
                    new List<InlineKeyboardButton[]>
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Добавить", "add_additional_photos")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Удалить", "delete_additional_photos")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Назад", "back_to_photo")
                        }
                    });
                await botClient.EditMessageTextAsync(callbackQuery.From.Id,callbackQuery.Message.MessageId,
                    "Что ты хочешь сделать с дополнительными фото?", replyMarkup: editAdditional);
                break;
            case "add_main_photos":
                await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Отправь новое основное фото \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id,(int)Action.AddMainPhoto);
                break;
        }
    }

    public static string GetFolder()
    {
        return _folder;
    }

    public static bool GetIsZodiacMattersEntered()
    {
        return _isZodiacMattersEntered;
    }

    private static async Task EnterZodiacSign(ITelegramBotClient botClient, Chat chat, long Id)
    {
        try
        {
            var removeKeyboard = new ReplyKeyboardRemove();
            await botClient.SendTextMessageAsync(chat.Id, "Введи свой знак зодиака",
                replyMarkup: removeKeyboard);
        }
        catch (Exception e)
        {
            // Логируем ошибку
            _logger.LogError(e, $"Ошибка при попытке отправить сообщение о знаке зодиака пользователю {Id}");

            // Обновляем стадию пользователя
            UserRepository.UpdateUserStage(Id, 4);

            // В случае ошибки, возможно, вам также стоит сообщить пользователю об этом
            await botClient.SendTextMessageAsync(chat.Id, "Произошла ошибка, пожалуйста, попробуйте снова.");
        }
    }

    private static void UpdateStage(long tgId, int stage)
    {
        UserRepository.UpdateUserStage(tgId, stage);
        _logger.LogInformation($"user({tgId}): Stage updated: {stage}");
    }
}