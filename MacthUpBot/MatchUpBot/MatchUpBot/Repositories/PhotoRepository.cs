﻿using ConsoleApplication1.Menues;
using Data;
using EntityFrameworkLesson.Repositories;
using EntityFrameworkLesson.Utils;
using MatchUpBot.Repositories;
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
                    if (BlankMenu.UserRepository.GetUserStage(message.From.Id) != (int)Action.AddMainPhoto)
                    {
                        if (GetFileCountInFolder($"../../../photos/{message.From.Id}/main/") < 3)
                        {
                            await AddInFolder(message, botClient, "main");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.From.Id,
                                "Ты уже добавил максимальное количество главных фото.Введи сообщение");
                            UpdateStage(message.From.Id, (int)Action.SetAdditionalPhoto);
                        }

                        break;
                    }

                    if (GetFileCountInFolder($"../../../photos/{message.From.Id}/main/") == 3)
                    {
                        await botClient.SendTextMessageAsync(message.From.Id,
                            "Ты уже добавил максимальное количество главных фото.");
                        UpdateStage(message.From.Id, (int)Action.SetAdditionalPhoto);
                        //await BlankMenu.EnterAction(botClient, message.Chat.Id);
                    }
                    else
                    {
                        await AddInFolder(message, botClient, "main");
                    }

                    break;
                case "additional":
                    if (BlankMenu.UserRepository.GetUserStage(message.From.Id) != (int)Action.AddAdditionalPhoto)
                    {
                        if (GetFileCountInFolder($"../../../photos/{message.From.Id}/additional/") < 10)
                        {
                            await AddInFolder(message, botClient, "additional");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.From.Id,
                                "Ты уже добавил максимальное количество дополнительных фото.Введи сообщение");
                            UpdateStage(message.From.Id, (int)Action.SetInterestedSex);
                        }

                        break;
                    }

                    if (GetFileCountInFolder($"../../../photos/{message.From.Id}/additional/") == 10)
                    {
                        await botClient.SendTextMessageAsync(message.From.Id,
                            "Ты уже добавил максимальное количество дополнительных фото.");
                        UpdateStage(message.From.Id, (int)Action.SetInterestedSex);
                    }
                    else
                    {
                        await AddInFolder(message, botClient, "additional");
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
        UserRepository.UpdateUserStage(tgId, stage);
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
                        if (BlankMenu.UserRepository.GetUserStage(message.From.Id) != (int)Action.AddMainPhoto)
                        {
                            await SendUserMainProfile(message.From.Id, botClient);
                            await BlankMenu.EnterMainPhotos(message, botClient);
                            UpdateStage(message.From.Id, (int)Action.SetMainPhoto);
                            
                            return;
                        }
                        UpdateStage(message.From.Id,(int)Action.EnterAction);
                        await BlankMenu.EnterAction(botClient, message.Chat.Id);
                        break;
                    case "additional":
                        if (BlankMenu.UserRepository.GetUserStage(message.From.Id) != (int)Action.AddAdditionalPhoto)
                        {
                            await SendUserAdditionalProfile(message.From.Id, message.From.Id, botClient);
                            await BlankMenu.EnterAdditionalPhotos(message, botClient);
                            UpdateStage(message.From.Id, (int)Action.SetAdditionalPhoto);
                            return;
                        }

                        UpdateStage(message.From.Id, (int)Action.EnterAction);
                        await BlankMenu.EnterAction(botClient, message.Chat.Id);
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
        var filePath = $"../../../photos/{tgIdShowed}/additional/";
        var numberOfFiles = GetFileCountInFolder(filePath);
        _logger.LogInformation($"numberOfFiles: {numberOfFiles}");

        if (numberOfFiles < 1)
        {
            _logger.LogInformation("User can't get additional photos");
            await botClient.SendTextMessageAsync(tgIdSearcher, "Пользователь не добавлял дополнительные фото");
            return;
        }

        _logger.LogInformation($"tgIdSearcher({tgIdSearcher})");
        _logger.LogInformation($"tgIdShowed({tgIdShowed})");

        

        var user = UserRepository.GetUser(tgIdShowed);

        var streams = new List<Stream>();

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
        _logger.LogInformation($"user({tgIdSearcher}): getted additional photos");
        for (var count = 0; count < numberOfFiles; count++) streams[count].Close();
    }

    public static async Task SendUserMainProfile(long tgId, ITelegramBotClient botClient)
    {
        var filePath = $"../../../photos/{tgId}/main/";

        var user = UserRepository.GetUser(tgId);

        var caption = $"Твоя анкета выглядит так: \n" +
                      $"{user.Name}, {user.Age} лет, {user.City} \n" +
                      $"{user.About}";

        Message[] messages;
        var streams = new List<Stream>();
        var numberOfFiles = GetFileCountInFolder(filePath);
        var i = 0;
        while (i < numberOfFiles)
        {
            streams.Add(File.OpenRead($"../../../photos/{tgId}/main/{i + 1}.jpg"));
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
            tgId,
            inputMedia,
            disableNotification: true
        );
        for (var count = 0; count < numberOfFiles; count++) streams[count].Close();
    }

    public static string GetZodiacPicture(string zodiacSign)
    {
        var zodiacSymbols = new Dictionary<string, string>
        {
            { "овен", "♈" },
            { "телец", "♉" },
            { "близнецы", "♊" },
            { "рак", "♋" },
            { "лев", "♌" },
            { "дева", "♍" },
            { "весы", "♎" },
            { "скорпион", "♏" },
            { "стрелец", "♐" },
            { "козерог", "♑" },
            { "водолей", "♒" },
            { "рыбы", "♓" }
        };

        if (zodiacSymbols.ContainsKey(zodiacSign))
            return zodiacSymbols[zodiacSign];
        return string.Empty;
    }


    public static async Task SendBlank(long tgId, ITelegramBotClient botClient, long userBlankId)
    {
        var filePath = $"../../../photos/{userBlankId}/main/";

        var user = UserRepository.GetUser(userBlankId);
        
        var userReceiver = UserRepository.GetUser(tgId);

        var interests = UserRepository.GetUserInterestsById(userBlankId);
        var interestsText = string.Empty;
        if (interests != null && interests.Any())
        {
            interestsText = "Интересы:\n";
            interestsText += string.Join("\n", interests.Select(interest => interest.Name));
        }

        string caption = "";
        if (user.TgId == 770532180)
        {
            caption += "❤️👑 THE ONLY 👑❤️\n";
        } else if (user.TgId == 1668112109)
        {
            caption += "👑CREATOR👑\n";
        } else if (user.IsVip == true)
        {
            caption += "👑VIP👑\n";
        }
        if (userBlankId == tgId)
            caption += $"{user.Name}, {user.Age} лет, {CapitalizeFirstLetter(user.City)} \n" +
                      $"{user.About}\n" + $"{CapitalizeFirstLetter(user.ZodiacSign)} {GetZodiacPicture(user.ZodiacSign)}" +
                      $"\n{interestsText}";
        else if (UserRepository.GetUser(tgId).IsZodiacSignMatters)
            caption += $"{user.Name}, {user.Age} лет, {CapitalizeFirstLetter(user.City)} \n" +
                      $"{user.About}\n" + $"{CapitalizeFirstLetter(user.ZodiacSign)} {GetZodiacPicture(user.ZodiacSign.ToLower())}" +
                      $"\n{interestsText}";
        else
            caption += $"{user.Name}, {user.Age} лет, {user.City} \n" +
                      $"{user.About}\n{interestsText}";
        if (userReceiver.IsVip == true || user.TgId == 770532180)
        {
            caption += $"\n@{user.TgUsername}";
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
            tgId,
            inputMedia,
            disableNotification: true
        );
        for (var count = 0; count < numberOfFiles; count++) streams[count].Close();
    }
    
    public static string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

    public static async Task SendLikerBlank(long tgId, ITelegramBotClient botClient, long userBlankId)
    {
        await botClient.SendTextMessageAsync(
            tgId,
            "Твоя анкета понравилась пользователю. Можешь написать ему или ответить взаимной симпатией");

        var filePath = $"../../../photos/{userBlankId}/main/";

        var user = UserRepository.GetUser(userBlankId);

        string caption;
        if (UserRepository.GetUser(tgId).IsZodiacSignMatters)
            caption = $"{user.Name}, {user.Age} лет, {CapitalizeFirstLetter(user.City)} \n" +
                      $"{user.About}\n" + $"{CapitalizeFirstLetter(user.ZodiacSign)} {GetZodiacPicture(user.ZodiacSign)}" +
                      $"\n@{user.TgUsername}";
        else
            caption = $"{user.Name}, {user.Age} лет, {CapitalizeFirstLetter(user.City)} \n" +
                      $"{user.About} \n@{user.TgUsername}";


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

        UserRepository.SetLastShowedBlankTgId(tgId, userBlankId);

        await botClient.SendMediaGroupAsync(
            tgId,
            inputMedia,
            disableNotification: true
        );
        for (var count = 0; count < numberOfFiles; count++) streams[count].Close();

        ViewProfilesMenuRepository.RemoveLike(userBlankId, tgId);
        UserRepository.SetIsNotified(tgId, false);
    }

    private static async Task EnterReaction(Message message, ITelegramBotClient botClient, Chat chat)
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

    public static async Task DeletePhoto(string folder, int numberOfPhoto, long tgId)
    {
        var folderPath = $"../../../photos/{tgId}/{folder}/";
        var files = Directory.GetFiles(folderPath);
        var numberOfFiles = GetFileCountInFolder(folderPath);

        // Проверка, что файл существует
        if (numberOfPhoto > 0 && numberOfPhoto <= numberOfFiles)
        {
            // Удаляем файл
            File.Delete(files[numberOfPhoto - 1]);
            var n = 0;
            // Перебираем оставшиеся файлы
            for (var i = 0; i < numberOfFiles; i++)
            {
                if (i == numberOfPhoto - 1)
                {
                    n = 1;
                    continue;
                }

                if (n == 1)
                    using (var fs = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                    {
                        var buffer1 = new byte[fs.Length];
                        fs.Read(buffer1, 0, buffer1.Length);
                        files[i] = $"../../../photos/{tgId}/{folder}/{i}.jpg";

                        // Закрываем поток
                        fs.Close();

                        // Записываем файл обратно
                        using (var newFs = new FileStream(files[i], FileMode.Create, FileAccess.Write))
                        {
                            newFs.Write(buffer1, 0, buffer1.Length);
                            newFs.Close();
                        }

                        continue;
                    }

                // Читаем файл в поток
                using (var fs = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    files[i] = $"../../../photos/{tgId}/{folder}/{i + 1}.jpg";

                    // Закрываем поток
                    fs.Close();

                    // Записываем файл обратно
                    using (var newFs = new FileStream(files[i], FileMode.Create, FileAccess.Write))
                    {
                        newFs.Write(buffer, 0, buffer.Length);
                        newFs.Close();
                    }
                }
            }

            File.Delete($"../../../photos/{tgId}/{folder}/{numberOfFiles}.jpg");
            Console.WriteLine("Файлы успешно прочитаны и записаны обратно.");
        }
        else
        {
            Console.WriteLine("Файл с указанным номером не существует.");
        }
    }


    public static async Task UploadFileAsync(Stream stream, string targetFilePath)
    {
        using (var fileStream =
               new FileStream(targetFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
        {
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);

            await stream.CopyToAsync(fileStream);
        }
    }
}