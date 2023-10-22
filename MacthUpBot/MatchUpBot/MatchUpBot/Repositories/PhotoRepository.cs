﻿using ConsoleApplication1.Menues;
using Data;
using EntityFrameworkLesson.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace Repositories;

public class PhotoRepository
{
    private static readonly ILogger<PhotoRepository> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<PhotoRepository>();

    public static async Task HandlePhotoMessage(Message message, ITelegramBotClient botClient)
    {
        try
        {
            var directoryPath = $"../../../photos/{message.From.Id}"; // Укажите путь к новой папке

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Directory.CreateDirectory(directoryPath + "/main");
                Directory.CreateDirectory(directoryPath + "/additional");
                Console.WriteLine("Папка успешно создана.");
            }
            else
            {
                Console.WriteLine("Папка уже существует.");
            }

            switch (CallbackDataRepository.GetFolder())
            {
                case "main":
                    if (GetFileCountInFolder($"../../../photos/{message.From.Id}/main/") < 3)
                    {
                        await AddInFolder(message, botClient, "main");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.From.Id,
                            "Ты уже добавил максимальное количество главных фото.Введи сообщение");
                        UpdateStage(message.From.Id, 7);
                    }

                    break;
                case "additional":
                    if (GetFileCountInFolder($"../../../photos/{message.From.Id}/additional/") < 10)
                    {
                        await AddInFolder(message, botClient, "additional");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.From.Id,
                            "Ты уже добавил максимальное количество дополнительных фото.Введи сообщение");
                        UpdateStage(message.From.Id, 8);
                    }

                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при скачивании файла: {e.Message}");
        }
    }

    private static void UpdateStage(long tgId, int stage)
    {
        BlankMenu.UserRepository.UpdateUserStage(tgId, stage);
        _logger.LogInformation($"user({tgId}): Stage updated: {stage}");
    }

    public static async Task AddInFolder(Message message, ITelegramBotClient botClient, string folder)
    {
        try
        {
            if (message.Photo != null)
            {
                Console.WriteLine("Фотография найдена.");
                var photo = message.Photo.LastOrDefault();
                var file = await botClient.GetFileAsync(photo.FileId);

                var filePath =
                    $"../../../photos/{message.From.Id}/{folder}/{GetFileCountInFolder($"../../../photos/{message.From.Id}/{folder}") + 1}.jpg";

                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await botClient.DownloadFileAsync(file.FilePath, fileStream);
                    fileStream.Close();
                }

                switch (folder)
                {
                    case "main":
                        await SendUserMainProfile(message, botClient);
                        await BlankMenu.EnterMainPhotos(message, botClient);
                        break;
                    case "additional":
                        await SendUserAdditionalProfile(message.From.Id, message.From.Id, botClient);
                        await BlankMenu.EnterAdditionalPhotos(message, botClient);
                        break;
                }


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

    public static async Task SendUserAdditionalProfile(long tgIdSearcher, long tgIdShowed, ITelegramBotClient botClient)
    {
        _logger.LogInformation($"tgIdSearcher({tgIdSearcher})");
        _logger.LogInformation($"tgIdShowed({tgIdShowed})");

        var filePath = $"../../../photos/{tgIdShowed}/additional/";

        var user = BlankMenu.UserRepository.GetUser(tgIdShowed);

        var streams = new List<Stream>();
        var numberOfFiles = GetFileCountInFolder(filePath);
        var i = 0;
        while (i < numberOfFiles)
        {
            streams.Add(File.OpenRead($"../../../photos/{tgIdShowed}/additional/{i + 1}.jpg"));
            i++;
        }

        var inputMedia = new List<IAlbumInputMedia>();

        for (var count = 0; count < numberOfFiles; count++)
        {
            var inputMediaPhoto = new InputMediaPhoto(new InputFileStream(streams[count], $"{count + 1}.jpg"));
            inputMedia.Add(inputMediaPhoto);
        }

        await botClient.SendMediaGroupAsync(
            tgIdSearcher,
            inputMedia,
            disableNotification: true
        );
    }

    public static async Task SendUserMainProfile(Message message, ITelegramBotClient botClient)
    {
        var filePath = $"../../../photos/{message.From.Id}/main/";

        var user = BlankMenu.UserRepository.GetUser(message.From.Id);

        var caption = $"Твоя анкета выглядит так: \n" +
                      $"{user.Name}, {user.Age} лет, {user.City} \n" +
                      $"{user.About}";

        Message[] messages;
        var streams = new List<Stream>();
        var numberOfFiles = GetFileCountInFolder(filePath);
        var i = 0;
        while (i < numberOfFiles)
        {
            streams.Add(File.OpenRead($"../../../photos/{message.From.Id}/main/{i + 1}.jpg"));
            i++;
        }

        var inputMedia = new List<IAlbumInputMedia>();

        for (var count = 0; count < numberOfFiles; count++)
        {
            var inputMediaPhoto = new InputMediaPhoto(new InputFileStream(streams[count], $"{count + 1}.jpg"));

            if (count == 0) inputMediaPhoto.Caption = caption;

            inputMedia.Add(inputMediaPhoto);
        }


        await botClient.SendMediaGroupAsync(
            message.From.Id,
            inputMedia,
            disableNotification: true
        );
    }

    public static string GetZodiacPicture(string zodiacSign)
    {
        Dictionary<string, string> zodiacSymbols = new Dictionary<string, string>
        {
            { "Овен", "♈" },
            { "Телец", "♉" },
            { "Близнецы", "♊" },
            { "Рак", "♋" },
            { "Лев", "♌" },
            { "Дева", "♍" },
            { "Весы", "♎" },
            { "Скорпион", "♏" },
            { "Стрелец", "♐" },
            { "Козерог", "♑" },
            { "Водолей", "♒" },
            { "Рыбы", "♓" }
        };

        if (zodiacSymbols.ContainsKey(zodiacSign))
        {
            return zodiacSymbols[zodiacSign];
        }
        else
        {
            return string.Empty;
        }
    }
    
    public static async Task SendBlank(Message message, ITelegramBotClient botClient, long userBlankId)
    {
        var filePath = $"../../../photos/{userBlankId}/main/";

        var user = BlankMenu.UserRepository.GetUser(userBlankId);


        string caption;
        if (BlankMenu.UserRepository.GetUser(message.From.Id).IsZodiacSignMatters == true)
        {
            caption = $"{user.Name}, {user.Age} лет, {user.City} \n" +
                          $"{user.About}\n" + $"{user.ZodiacSign} {GetZodiacPicture(user.ZodiacSign)}(85% совместимость)";
        }
        else
        {
            caption = $"{user.Name}, {user.Age} лет, {user.City} \n" +
                          $"{user.About}";
        }
        

        Message[] messages;
        var streams = new List<Stream>();
        var numberOfFiles = GetFileCountInFolder(filePath);
        var i = 0;
        while (i < numberOfFiles)
        {
            streams.Add(File.OpenRead($"../../../photos/{userBlankId}/main/{i + 1}.jpg"));
            i++;
        }

        var inputMedia = new List<IAlbumInputMedia>();

        for (var count = 0; count < numberOfFiles; count++)
        {
            var inputMediaPhoto = new InputMediaPhoto(new InputFileStream(streams[count], $"{count + 1}.jpg"));

            if (count == 0) inputMediaPhoto.Caption = caption;

            inputMedia.Add(inputMediaPhoto);
        }

        await botClient.SendMediaGroupAsync(
            message.From.Id,
            inputMedia,
            disableNotification: true
            );
    }
    
    private static async Task EnterReaction(Message message, ITelegramBotClient botClient, Chat chat)
    {
        var blankReactionKeyboardMarkup = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[] { new ("❤️"), new ("👎"), new ("🚪"), new ("📷") }
            })
        {
            ResizeKeyboard = true
        };
        await botClient.SendTextMessageAsync(
            chat.Id,
            "-",
            replyMarkup: blankReactionKeyboardMarkup);    
    }


    public static int GetFileCountInFolder(string folderPath)
    {
        try
        {
            var files = Directory.GetFiles(folderPath);
            return files.Length;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
            return -1;
        }
    }
}