using Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EntityFrameworkLesson.Repositories;

public class PhotoRepository
{
    public static async Task HandlePhotoMessage(Message message, ITelegramBotClient botClient, UserEntity user)
    {
        try
        {
            if (message.Photo != null)
            {
                Console.WriteLine("Фотография найдена.");
                var photo = message.Photo.LastOrDefault();
                var file = await botClient.GetFileAsync(photo.FileId);
                
                string filePath = $"../../../photos/{user.TgId}.jpg";
                
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await botClient.DownloadFileAsync(file.FilePath, fileStream);
                    fileStream.Close();
                } 
                
                await using Stream stream = System.IO.File.OpenRead(filePath);
                
                string caption = $"Твоя анкета выглядит так:\n" +
                                 $"{user.Name}, {user.Age} лет, {user.City}\n" +
                                 $"{user.About}";
                    
                await botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    InputFile.FromStream(stream,"photo.jpg"),
                    caption: caption,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
                    
                    
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
}