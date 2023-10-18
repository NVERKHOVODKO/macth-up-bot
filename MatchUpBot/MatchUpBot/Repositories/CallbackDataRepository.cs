using ConsoleApplication1.Menues;
using Data;
using Entities;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = TelegramBotExperiments.Models.User;

namespace EntityFrameworkLesson.Repositories;

public class CallbackDataRepository
{
    private static readonly UserRepository UserRepository = new();
    private static ILogger<CallbackDataRepository> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CallbackDataRepository>();

    public static async Task HandleCallBackQuery(ITelegramBotClient botClient, Update update)
    {
        var callbackQuery = update.CallbackQuery;
        var user = callbackQuery.From;
        if (UserRepository.GetUserStage(user.Id) != 4)
        {
            await botClient.SendTextMessageAsync(user.Id, "Куда ты тыкаешь, аболтус");
            return;
        }
    switch (callbackQuery.Data)
      {
          case "man":
          {
              UserRepository.SetUserGender(user.Id, "Мужчина");
              _logger.LogInformation($"user({user.Id}): updated gender: Мужчина");

              InlineKeyboardMarkup boolKeyboard = new(new[]
              {
                  InlineKeyboardButton.WithCallbackData("Да","zodiacMatters"),
                  InlineKeyboardButton.WithCallbackData("Нет","zodiacDoesntMatters")
              });
              await botClient.EditMessageTextAsync(user.Id,BlankMenu.getMessageId(),"Для тебя важен знак зодиака?",replyMarkup: boolKeyboard);
              break;
          }
              
          case "woman":
          {
              UserRepository.SetUserGender(user.Id, "Женщина");
              _logger.LogInformation($"user({user.Id}): updated gender: Женщина");

              InlineKeyboardMarkup boolKeyboard = new(new[]
              {
                  InlineKeyboardButton.WithCallbackData("Да","zodiacMatters"),
                  InlineKeyboardButton.WithCallbackData("Нет","zodiacDoesntMatters")
              });
              await botClient.EditMessageTextAsync(user.Id,BlankMenu.getMessageId(),"Для тебя важен знак зодиака?",replyMarkup: boolKeyboard);
              break;
          }

          case "zodiacMatters":
          {
              await EnterZodiacSign(botClient, callbackQuery.Message.Chat , user.Id);
              UserRepository.SetUserIsZodiacSignMatters(user.Id, true);
              UpdateStage(user.Id, 4);
              break;
          }
          case "zodiacDoesntMatters":
          {
              await EnterZodiacSign(botClient, callbackQuery.Message.Chat,  user.Id);
              UserRepository.SetUserIsZodiacSignMatters(user.Id, false);
              UpdateStage(user.Id, 4);
              break;
          }

      }
      
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