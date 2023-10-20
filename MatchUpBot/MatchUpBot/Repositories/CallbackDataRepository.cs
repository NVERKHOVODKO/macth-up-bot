using ConsoleApplication1.Menues;
using Data;
using Entities;
using Microsoft.Extensions.Logging;
using Repositories;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = TelegramBotExperiments.Models.User;

namespace EntityFrameworkLesson.Repositories;

public class CallbackDataRepository
{
    private static ILogger<CallbackDataRepository> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CallbackDataRepository>();

    public static async Task HandleCallBackQuery(ITelegramBotClient botClient, Update update)
    {
        var callbackQuery = update.CallbackQuery;
        var user = callbackQuery.From;

        switch (callbackQuery.Data)
        {
            case "man":
            {
                try
                {
                    if (BlankMenu.UserRepository.GetUserStage(user.Id) != 4)
                    {
                        await botClient.SendTextMessageAsync(user.Id, "Куда ты тыкаешь, аболтус");
                        return;
                    }
                    BlankMenu.UserRepository.SetUserGender(user.Id, "Мужчина");
                    _logger.LogInformation($"user({user.Id}): updated gender: Мужчина");

                    InlineKeyboardMarkup boolKeyboard = new(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Да", "zodiacMatters"),
                        InlineKeyboardButton.WithCallbackData("Нет", "zodiacDoesntMatters")
                    });
                    await botClient.EditMessageTextAsync(user.Id, BlankMenu.getMessageId(),
                        "Для тебя важен знак зодиака?", replyMarkup: boolKeyboard);
                    break;
                }
                catch (Telegram.Bot.Exceptions.ApiRequestException e)
                {
                    await botClient.SendTextMessageAsync(user.Id, "Не тыкай туда, бродяга");
                    break;
                }

            }

            case "woman":
            {
                try
                {
                    if (BlankMenu.UserRepository.GetUserStage(user.Id) != 4)
                    {
                        await botClient.SendTextMessageAsync(user.Id, "Куда ты тыкаешь, аболтус");
                        return;
                    }
                    if (BlankMenu.getMessageId() == 0)
                    {
                        throw new Telegram.Bot.Exceptions.ApiRequestException("warning");
                    }

                    BlankMenu.UserRepository.SetUserGender(user.Id, "Женщина");
                    _logger.LogInformation($"user({user.Id}): updated gender: Женщина");

                    InlineKeyboardMarkup boolKeyboard = new(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Да", "zodiacMatters"),
                        InlineKeyboardButton.WithCallbackData("Нет", "zodiacDoesntMatters")
                    });
                    await botClient.EditMessageTextAsync(user.Id, BlankMenu.getMessageId(),
                        "Для тебя важен знак зодиака?", replyMarkup: boolKeyboard);
                    break;
                }
                catch (Telegram.Bot.Exceptions.ApiRequestException e)
                {
                    await botClient.SendTextMessageAsync(user.Id, "Не тыкай туда, бродяга");
                    break;
                }
            }

            case "zodiacMatters":
            {
                if (BlankMenu.UserRepository.GetUserStage(user.Id) != 4)
                {
                    await botClient.SendTextMessageAsync(user.Id, "Куда ты тыкаешь, аболтус");
                    return;
                }
                await EnterZodiacSign(botClient, callbackQuery.Message.Chat, user.Id);
                BlankMenu.UserRepository.SetUserIsZodiacSignMatters(user.Id, true);
                UpdateStage(user.Id, 4);
                _isZodiacMattersEntered = true;
                break;
            }
            case "zodiacDoesntMatters":
            {
                if (BlankMenu.UserRepository.GetUserStage(user.Id) != 4)
                {
                    await botClient.SendTextMessageAsync(user.Id, "Куда ты тыкаешь, аболтус");
                    return;
                }
                await EnterZodiacSign(botClient, callbackQuery.Message.Chat, user.Id);
                BlankMenu.UserRepository.SetUserIsZodiacSignMatters(user.Id, false);
                UpdateStage(user.Id, 4);
                _isZodiacMattersEntered = true;
                break;
            }
            case "want_to_add_main_photo":
            {
                if (PhotoRepository.GetFileCountInFolder($"../../../photos/{callbackQuery.Message.From.Id}/main") ==3)
                {
                    UpdateStage(user.Id, 7);
                    break;
                }
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Скинь еще главную фото");
                _folder = "main";
                break;
            }
            case "dont_want_to_add_main_photo":
            {
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Фотографии сохранены. Отправь сообщение.");
                UpdateStage(user.Id, 7);
                break;
            }
        }
    }

    private static string _folder = "main";

    public static string GetFolder()
    {
        return _folder;
    }
    private static bool _isZodiacMattersEntered = false;

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
           BlankMenu.UserRepository.UpdateUserStage(Id, 4);

           // В случае ошибки, возможно, вам также стоит сообщить пользователю об этом
           await botClient.SendTextMessageAsync(chat.Id, "Произошла ошибка, пожалуйста, попробуйте снова.");
       }
   }
   private static void UpdateStage(long tgId, int stage)
   {
       BlankMenu.UserRepository.UpdateUserStage(tgId, stage);
       _logger.LogInformation($"user({tgId}): Stage updated: {stage}");
   }
}