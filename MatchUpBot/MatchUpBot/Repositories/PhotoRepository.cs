using System.Net.NetworkInformation;
using ConsoleApplication1.Menues;
using Data;
using Entities;
using EntityFrameworkLesson.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace Repositories;

public class PhotoRepository
{
    private static ILogger<PhotoRepository> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<PhotoRepository>();
    public static async Task HandlePhotoMessage(Message message, ITelegramBotClient botClient)
    {
        try
        {
            string directoryPath = $"../../../photos/{message.From.Id}"; // Укажите путь к новой папке

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
                            await AddInFolder(message,botClient,"main");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.From.Id,
                                "Ты уже добавил максимальное количество главных фото.Введи сообщение");
                            UpdateStage(message.From.Id,7);
                        }
                        break;
                    case "additional":
                        if (GetFileCountInFolder($"../../../photos/{message.From.Id}/additional/") <10)
                        {
                            await AddInFolder(message,botClient,"additional");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.From.Id,
                                "Ты уже добавил максимальное количество дополнительных фото.Введи сообщение");
                            UpdateStage(message.From.Id,8);
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

                var filePath = $"../../../photos/{message.From.Id}/{folder}/{GetFileCountInFolder($"../../../photos/{message.From.Id}/{folder}")+1}.jpg";

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
                        await SendUserAdditionalProfile(message.From.Id, botClient);
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
    
    public static async Task SendUserAdditionalProfile(long tgId, ITelegramBotClient botClient)
    {
        var filePath = $"../../../photos/{tgId}/additional/";
        
        var user = BlankMenu.UserRepository.GetUser(tgId);

        List<Stream> streams = new List<Stream>();
        int numberOfFiles = GetFileCountInFolder(filePath);
        int i = 0;
        while (i < numberOfFiles)
        {
            streams.Add(File.OpenRead($"../../../photos/{tgId}/additional/{i+1}.jpg"));
            i++;
        }
        List<IAlbumInputMedia> inputMedia = new List<IAlbumInputMedia>();
        
        for (int count = 0; count < numberOfFiles; count++)
        {
            var inputMediaPhoto = new InputMediaPhoto(new InputFileStream(streams[count], $"{count + 1}.jpg"));
            inputMedia.Add(inputMediaPhoto);
        }
        
        await botClient.SendMediaGroupAsync(
            chatId:tgId,
            media: inputMedia,
            disableNotification: true
        );
        
    }

    public static async Task SendUserMainProfile(Message message, ITelegramBotClient botClient)
    {
        var filePath = $"../../../photos/{message.From.Id}/main/";
        
        var user = BlankMenu.UserRepository.GetUser(message.From.Id);

        string caption = $"Твоя анкета выглядит так: \n" +
                         $"{user.Name}, {user.Age} лет,{user.City} \n" +
                         $"{user.About}";

        Message[] messages;
        List<Stream> streams = new List<Stream>();
        int numberOfFiles = GetFileCountInFolder(filePath);
        int i = 0;
        while (i < numberOfFiles)
        {
            streams.Add(File.OpenRead($"../../../photos/{message.From.Id}/main/{i+1}.jpg"));
            i++;
        }
        List<IAlbumInputMedia> inputMedia = new List<IAlbumInputMedia>();
        
        for (int count = 0; count < numberOfFiles; count++)
        {
            var inputMediaPhoto = new InputMediaPhoto(new InputFileStream(streams[count], $"{count + 1}.jpg"));
    
            if (count == 0)
            {
                inputMediaPhoto.Caption = caption;
            }
    
            inputMedia.Add(inputMediaPhoto);
        }


        await botClient.SendMediaGroupAsync(
            chatId:message.From.Id,
            media: inputMedia,
            disableNotification: true
        );
        
    }
    public static int GetFileCountInFolder(string folderPath)
    {
        try
        {
            string[] files = Directory.GetFiles(folderPath);
            return files.Length;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
            return -1; 
        }
    }

}