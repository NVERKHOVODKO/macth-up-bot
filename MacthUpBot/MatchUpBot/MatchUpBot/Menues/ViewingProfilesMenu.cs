﻿using Data;
using Entities;
using MatchUpBot.Repositories;
using Microsoft.Extensions.Logging;
using Repositories;
using Telegram.Bot;

namespace ConsoleApplication1.Menues;

public class ViewingProfilesMenu
{
    public static readonly UserRepository UserRepository = new();

    private static ILogger<BlankMenu> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BlankMenu>();

    private static readonly ViewProfilesMenuRepository vpmr = new();


    public ViewingProfilesMenu(ILogger<BlankMenu> logger)
    {
        _logger = logger;
    }

    public static UserEntity GetMatchingProfile(long recieverId)
    {
        double priority = 70;//"очки совпадения" 0-100. но у нас 70 тк нет идеальных совпадений
        UserEntity userEntity = null;
        while (userEntity == null)//цикл для поиска
        {
            userEntity = vpmr.GetMatchingProfile(recieverId, priority);
            if (priority < 20)//если плохое совпадение то возвращаем null
                return null;
            priority -= 5;//с каждой итерацией менее придирчиво подбираем анкету
        }
        return userEntity;
    }

    public static async Task ShowBlank(long userId, ITelegramBotClient botClient)
    {
        var userSearched = GetMatchingProfile(userId);//берем подходящий профиль
        _logger.LogInformation($"user({userId}): getting a blank({userSearched.TgId})");//если нет такого то кидаем это
        if (userSearched == null)
        {
            await botClient.SendTextMessageAsync(userId, "Не получилось найти кого-то подходящего для тебя(");
            return;
        }
        else//если такой есть отправляем в нужный чат
        {
            _logger.LogInformation($"user({userSearched.TgId}): getted to user({userId})");
            await PhotoRepository.SendBlank(userId, botClient, userSearched.TgId);
            _logger.LogInformation($"user({userSearched.TgId}): sended to user({userId})");
            UserRepository.SetLastShowedBlankTgId(userId, userSearched.TgId);
            return;
        }
    }
}