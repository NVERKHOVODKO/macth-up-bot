﻿using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace MatchUpBot.Repositories;

public class EditProfileRepository
{
    public static void SendEditKeyboard(ITelegramBotClient botClient, long tgId, CallbackQuery callbackQuery)
    {
        var changeMenu = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Имя", "change_name"),
                    InlineKeyboardButton.WithCallbackData("Возраст", "change_age")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Город", "change_city"),
                    InlineKeyboardButton.WithCallbackData("Описание", "change_about")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Редактировать фото", "change_photo")
                },
                new[]
                {
                InlineKeyboardButton.WithCallbackData("Назад", "back_to_action")
            }
            });
        botClient.EditMessageTextAsync(tgId, callbackQuery.Message.MessageId, "Что ты хочешь изменить?", replyMarkup: changeMenu);

    }

    public static async Task EditKeyboardToAction(ITelegramBotClient botClient, long tgId, CallbackQuery callbackQuery)
    {
        var menuKeyboard = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>
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
                    InlineKeyboardButton.WithCallbackData("Просмотреть доп фото", "view_add_photo")
                }
            });
        await botClient.EditMessageTextAsync(tgId,callbackQuery.Message.MessageId, "Выбери действие", replyMarkup: menuKeyboard);
    }

    public static async Task EditKeyboardToPhotoChoice(ITelegramBotClient botClient, long tgId,
        CallbackQuery callbackQuery)
    {
        var editPhoto = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Основные", "edit_main_photos")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Дополнительные", "edit_additional_photos")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "back_to_edit")
                }
            });
        await botClient.EditMessageTextAsync(callbackQuery.From.Id,callbackQuery.Message.MessageId,
            "Какие фото ты хочешь изменить?", replyMarkup: editPhoto);
    }
}