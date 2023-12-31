﻿using ConsoleApplication1.Menues;
using Data;
using Entities;
using EntityFrameworkLesson.Utils;
using MatchUpBot.Repositories;
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
            if (BlankMenu.UserRepository.GetUserStage(userId) != (int)Action.SetSex)
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
        if (BlankMenu.UserRepository.GetUserStage(tgId) != (int)Action.SetSex)
        {
            await botClient.SendTextMessageAsync(tgId, "Куда ты тыкаешь, аболтус");
            return;
        }

        await botClient.EditMessageTextAsync(tgId, callbackQuery.Message.MessageId, "Отлично!");
        await EnterZodiacSign(botClient, callbackQuery.Message.Chat, tgId);
        BlankMenu.UserRepository.SetUserIsZodiacSignMatters(tgId, matters);
        UpdateStage(tgId, (int)Action.SetIsZodiacSignMatter);
        _isZodiacMattersEntered = true;
    }

    public static async Task HandleCallBackQuery(ITelegramBotClient botClient, Update update)
    {
        var callbackQuery = update.CallbackQuery;
        var user = callbackQuery.From;
        List<CardEntity> cardEntities = BlankMenu.UserRepository.GetCardsForUser(callbackQuery.From.Id);

        switch (callbackQuery.Data)
        {
            case "man":
            {
                await SetGenger("М", botClient, user.Id, callbackQuery);
                break;
            }
            case "woman":
            {
                await SetGenger("Ж", botClient, user.Id, callbackQuery);
                break;
            }
            case "boys":
            {
                var ur = new UserRepository();
                ur.SetUserInterestedGender(callbackQuery.From.Id, "М");
                UpdateStage(user.Id, (int)Action.EnterAction);
                await BlankMenu.EnterAction(botClient, callbackQuery.From.Id);
                break;
            }
            case "girls":
            {
                var ur = new UserRepository();
                ur.SetUserInterestedGender(callbackQuery.From.Id, "Ж");
                UpdateStage(user.Id, (int)Action.EnterAction);
                await BlankMenu.EnterAction(botClient, callbackQuery.From.Id);
                break;
            }
            case "any":
            {
                var ur = new UserRepository();
                ur.SetUserInterestedGender(callbackQuery.From.Id, "Неважно");
                UpdateStage(user.Id, (int)Action.EnterAction);
                await BlankMenu.EnterAction(botClient, callbackQuery.From.Id);
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
                    await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
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
                    await BlankMenu.EnterInterestedGender(callbackQuery, botClient);
                    UpdateStage(user.Id, (int)Action.SetInterestedSex);
                    break;
                }

                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Скинь еще дополнительную фото");

                await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Отправь дополнительные фото");
            }
                break;
            case "additional_photo_no":
                await BlankMenu.EnterInterestedGender(callbackQuery, botClient);
                UpdateStage(user.Id, (int)Action.SetInterestedSex);
                break;
            case "view_add_photo":
                if (PhotoRepository.GetFileCountInFolder($"../../../photos/{callbackQuery.From.Id}/additional") == 0)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Ты не добавил дополнительных фото");
                    break;
                }

                await PhotoRepository.SendUserAdditionalProfile(callbackQuery.From.Id, callbackQuery.From.Id,
                    botClient);
                await BlankMenu.EnterAction(botClient, callbackQuery.From.Id);
                break;
            case "view_profiles":
                UpdateStage(user.Id, (int)Action.GetFirstBlank);
                await BlankMenu.AddReactionKeyboard(botClient, callbackQuery.From.Id);
                UserRepository.UpdateUserStage(callbackQuery.From.Id, (int)Action.GetBlank);
                ViewingProfilesMenu.ShowBlank(callbackQuery.From.Id, botClient);
                //BlankMenu.GetBlankReaction(message, botClient, callbackQuery.From.Id);
                break;
            case "edit_profile":
                EditProfileRepository.SendEditKeyboard(botClient, callbackQuery.From.Id, callbackQuery);
                break;
            case "change_name":
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Введи новое имя. \nДля отмены введи «Отмена»");
                ViewProfilesMenuRepository.ClearViewingHistory(callbackQuery.From.Id);
                UpdateStage(callbackQuery.From.Id, (int)Action.EditName);
                break;
            case "change_age":
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Введи новый возраст. \nДля отмены введи «Отмена»");
                ViewProfilesMenuRepository.ClearViewingHistory(callbackQuery.From.Id);
                UpdateStage(callbackQuery.From.Id, (int)Action.EditAge);
                break;
            case "change_city":
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Введи новый город. \nДля отмены введи «Отмена»");
                ViewProfilesMenuRepository.ClearViewingHistory(callbackQuery.From.Id);
                UpdateStage(callbackQuery.From.Id, (int)Action.EditCity);
                break;
            case "change_about":
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Введи новое описание. \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id, (int)Action.EditDescription);
                break;
            case "change_photo":
                await EditProfileRepository.EditKeyboardToPhotoChoice(botClient, callbackQuery.From.Id, callbackQuery);
                break;
            case "add_additional_photos":
                await botClient.SendTextMessageAsync(callbackQuery.From.Id,
                    "Отправь новое дополнительное фото \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id, (int)Action.AddAdditionalPhoto);
                break;
            case "delete_main_photos":
                if (PhotoRepository.GetFileCountInFolder($"../../../photos/{callbackQuery.From.Id}/main") == 1)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Нельзя удалить последнее фото");
                    break;
                }

                await PhotoRepository.SendUserMainProfile(callbackQuery.From.Id, botClient);
                await botClient.SendTextMessageAsync(callbackQuery.From.Id,
                    "Отправь номер фото,которое хочешь удалить \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id, (int)Action.DeleteMainPhoto);
                break;
            case "delete_additional_photos":
                if (PhotoRepository.GetFileCountInFolder($"../../../photos/{callbackQuery.From.Id}/additional") == 0)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Ты не добавил доп фото");
                    break;
                }

                await PhotoRepository.SendUserAdditionalProfile(callbackQuery.From.Id, callbackQuery.From.Id,
                    botClient);
                await botClient.SendTextMessageAsync(callbackQuery.From.Id,
                    "Отправь номер фото,которое хочешь удалить \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id, (int)Action.DeleteAdditionalPhoto);
                break;
            case "back_to_edit":
            {
                EditProfileRepository.SendEditKeyboard(botClient, callbackQuery.From.Id, callbackQuery);
                break;
            }
            case "back_to_action":
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
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
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
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId,
                    "Что ты хочешь сделать с дополнительными фото?", replyMarkup: editAdditional);
                break;
            case "add_main_photos":
                await botClient.SendTextMessageAsync(callbackQuery.From.Id,
                    "Отправь новое основное фото \nДля отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id, (int)Action.AddMainPhoto);
                break;
            case "add_interests":
                var interests = Interests.GetInterests();

                var messageText = "Выберите ваши интересы:\n";
                for (var i = 0; i < interests.Length; i++) messageText += $"{i + 1}. {interests[i]}\n";

                await botClient.SendTextMessageAsync(callbackQuery.From.Id, messageText);
                UpdateStage(callbackQuery.From.Id, (int)Action.AddInterest);
                break;
            case "view_myself":
                await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Твоя анкета выглядит так:");
                await PhotoRepository.SendBlank(callbackQuery.From.Id, botClient, callbackQuery.From.Id);
                await BlankMenu.EnterAction(botClient, callbackQuery.From.Id);
                break;
            case "delete_profile":
                var confirmKeyboard = new InlineKeyboardMarkup( new List<InlineKeyboardButton[]>
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Да", "deleted_confirmed"),
                        InlineKeyboardButton.WithCallbackData("Нет", "back_to_edit")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Назад", "back_to_edit")
                    },
                });
                await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, 
                    "Ты точно хочешь удалить свою анкету?", replyMarkup:confirmKeyboard);
                break;
            case "deleted_confirmed":
                await EditProfileRepository.DeleteProfile(callbackQuery.From.Id, botClient,
                    callbackQuery.From.Username);
                break;
            case "get_vip":
                /*await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, 
                    "Функция временно заблокированна администратором");
                await BlankMenu.EnterAction(botClient, callbackQuery.From.Id);
                return;*/
                HandleCards(callbackQuery, botClient,cardEntities);
                break;
            case "add_card":
                await botClient.EditMessageTextAsync(callbackQuery.From.Id,callbackQuery.Message.MessageId, 
                    "Введи номер кредитной карты \n " +
                    "в формате XXXXXXXXXXXXXXXX \n"
                    + "Для отмены введи «Отмена»");
                UpdateStage(callbackQuery.From.Id, (int)Action.SetCardNumber);
                break;
            case "next_card":
                if (numberOfCard + 1 >= BlankMenu.UserRepository.GetCardCountForUser(callbackQuery.From.Id))
                {
                    return;
                }
                numberOfCard++;
                
                ShowCards(callbackQuery, botClient, cardEntities);
                break;
            case "previous_card":
                if(numberOfCard < 1)
                {
                    return;
                }
                numberOfCard--;
                ShowCards(callbackQuery, botClient, cardEntities);
                break;
            case "pay_from_card":
                if (BlankMenu.UserRepository.GetUserVip(callbackQuery.From.Id))
                {
                    await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Ты уже VIP, родной");
                    return;
                }
                BlankMenu.UserRepository.SetVipStatus(callbackQuery.From.Id, true);
                await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Теперь ты VIP");
                await BlankMenu.EnterAction(botClient, callbackQuery.From.Id);
                UpdateStage(callbackQuery.From.Id, (int)Action.EnterAction);
                break;
            case "delete_card":
                if (numberOfCard  >= BlankMenu.UserRepository.GetCardCountForUser(callbackQuery.From.Id) || numberOfCard < 0)
                {
                    return;
                }
                BlankMenu.UserRepository.DeleteCard(cardEntities[numberOfCard].Id);
                HandleCards(callbackQuery, botClient,cardEntities);
                await botClient.SendTextMessageAsync(callbackQuery.From.Id, "Карта удалена");
                break;
            case "change_gender":
                await ChangeGender(callbackQuery, botClient);
                break;
            case "edit_gender_to_man":
                await ChangeGender(callbackQuery, botClient, "М");
                ViewProfilesMenuRepository.ClearViewingHistory(callbackQuery.From.Id);
                break;
            case "edit_gender_to_woman":
                await ChangeGender(callbackQuery, botClient, "Ж");
                ViewProfilesMenuRepository.ClearViewingHistory(callbackQuery.From.Id);
                break;
            case "change_interested_gender":
                await ChangeInterestedGender(callbackQuery, botClient);
                ViewProfilesMenuRepository.ClearViewingHistory(callbackQuery.From.Id);
                break;
            case "edit_interested_gender_to_man":
                await ChangeInterestedGender(callbackQuery, botClient,"М");
                ViewProfilesMenuRepository.ClearViewingHistory(callbackQuery.From.Id);
                break;
            case "edit_interested_gender_to_woman":
                await ChangeInterestedGender(callbackQuery, botClient,"Ж");
                ViewProfilesMenuRepository.ClearViewingHistory(callbackQuery.From.Id);
                break;
            case "edit_interested_gender_to_any":
                await ChangeInterestedGender(callbackQuery, botClient,"Неважно");
                ViewProfilesMenuRepository.ClearViewingHistory(callbackQuery.From.Id);
                break;
        }
    }

    private static async Task ChangeGender(CallbackQuery callbackQuery, ITelegramBotClient botClient, string gender = "")
    {
        long tgId = callbackQuery.From.Id;
        if (gender.Equals(BlankMenu.UserRepository.GetUserGender(tgId)))
        {
            return;
        }
        if (gender != "")
        {
            BlankMenu.UserRepository.SetUserGender(callbackQuery.From.Id, gender);
        }
        gender = BlankMenu.UserRepository.GetUserGender(tgId);
        switch (gender)
        {
            case "М":
                gender = "Мужчина";
                break;
            case "Ж":
                gender = "Женщина";
                break;
        }
        var genderKeyboard = new InlineKeyboardMarkup( new List<InlineKeyboardButton[]>
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Мужчина", "edit_gender_to_man"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Женщина", "edit_gender_to_woman")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Назад", "back_to_edit")
            },
        });
        await botClient.EditMessageTextAsync(tgId, callbackQuery.Message.MessageId, $"Твой пол: {gender} \nИзменить на:", replyMarkup:genderKeyboard);
    }

    private static async Task ChangeInterestedGender(CallbackQuery callbackQuery, ITelegramBotClient botClient,
        string interestedGender = "")
    {
        long tgId = callbackQuery.From.Id;
        var user = UserRepository.GetUser(tgId);
        
        if (interestedGender == user.GenderOfInterest)
        {
            return;
        }
        if (interestedGender != "")
        {
            BlankMenu.UserRepository.SetUserInterestedGender(callbackQuery.From.Id, interestedGender);
        }
        interestedGender = UserRepository.GetUser(tgId).GenderOfInterest;
        switch (interestedGender)
        {
            case "М":
                interestedGender = "Мужчины";
                break;
            case "Ж":
                interestedGender = "Женщины";
                break;
            case "Неважно":
                interestedGender = "Неважно";
                break;
        }
        var interestedGenderKeyboard = new InlineKeyboardMarkup( new List<InlineKeyboardButton[]>
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Мужчины", "edit_interested_gender_to_man"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Женщины", "edit_interested_gender_to_woman")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Неважно", "edit_interested_gender_to_any")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Назад", "back_to_edit")
            },
        });
        await botClient.EditMessageTextAsync(tgId, callbackQuery.Message.MessageId, $"Тебе нравятся: {interestedGender} \nИзменить на:", replyMarkup:interestedGenderKeyboard);
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
            UpdateStage(Id, (int)Action.SetIsZodiacSignMatter);
            // В случае ошибки, возможно, вам также стоит сообщить пользователю об этом
            await botClient.SendTextMessageAsync(chat.Id, "Произошла ошибка, пожалуйста, попробуйте снова.");
        }
    }

    private static void UpdateStage(long tgId, int stage)
    {
        UserRepository.UpdateUserStage(tgId, stage);
        _logger.LogInformation($"user({tgId}): Stage updated: {stage}");
    }
    private static async void HandleCards(CallbackQuery callbackQuery, ITelegramBotClient botClient, List<CardEntity> cardEntities)
    {
        numberOfCard = 0;
        if (BlankMenu.UserRepository.GetCardCountForUser(callbackQuery.From.Id) == 0)
        {
            var noCardKeyboard = new InlineKeyboardMarkup( new List<InlineKeyboardButton[]>
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Добавить карту", "add_card"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "back_to_action")
                },
            });
             await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "Ты еще не добавил кредитных карт", replyMarkup:noCardKeyboard);
            return;
        }
        ShowCards(callbackQuery, botClient, cardEntities);
    }

    public static int numberOfCard;

    private static async void ShowCards(CallbackQuery callbackQuery, ITelegramBotClient botClient,
        List<CardEntity> cardEntities)
    {
        var cardKeyboard = new InlineKeyboardMarkup( new List<InlineKeyboardButton[]>
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("<<", "previous_card"),
                InlineKeyboardButton.WithCallbackData(">>", "next_card")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Оплатить", "pay_from_card")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Добавить карту", "add_card"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Удалить карту", "delete_card")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Назад", "back_to_action")
            },
        });
        await botClient.EditMessageTextAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId, "Список твоих кредитных карт: \n" +
                                                              "Номер: " + cardEntities[numberOfCard].CardNumber + "\n" +
                                                              "Годен: " + cardEntities[numberOfCard].ExpirationTime +
                                                              "\n" + "Имя: " + cardEntities[numberOfCard].HolderName +
                                                              "          cvv: " + cardEntities[numberOfCard].CVV,
            replyMarkup: cardKeyboard);
    }
}