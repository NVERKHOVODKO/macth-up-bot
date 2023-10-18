using Data;
using Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace Repositories;

public class PhotoRepository
{
    public static async Task HandlePhotoMessage(Message message, ITelegramBotClient botClient)
    {
        try
        {
            if (message.Photo != null)
            {
                Console.WriteLine("Фотография найдена.");
                var photo = message.Photo.LastOrDefault();
                var file = await botClient.GetFileAsync(photo.FileId);

                var filePath = $"../../../photos/{message.From.Id}.jpg";

                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await botClient.DownloadFileAsync(file.FilePath, fileStream);
                    fileStream.Close();
                }

                await using Stream stream = File.OpenRead(filePath);
                UserRepository userRepository = new UserRepository();
                UserEntity user = userRepository.GetUser(message.From.Id);
                var caption = $"Твоя анкета выглядит так:\n" +
                              $"{user.Name}, {user.Age} лет, {user.City}\n" +
                              $"{user.About}";

                await botClient.SendPhotoAsync(
                    message.Chat.Id,
                    InputFile.FromStream(stream, "photo.jpg"),
                    caption: caption,
                    parseMode: ParseMode.Markdown
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