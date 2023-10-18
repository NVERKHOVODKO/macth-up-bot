//6610584532:AAHyYTG_Rz96QfQEc7H-Dk-7iHHb2PeQN0E

using ConsoleApplication1.Menues;
using Data;
using Entities;
using EntityFrameworkLesson.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

internal class Program
{
    private static ITelegramBotClient _botClient;
    private static ReceiverOptions _receiverOptions;
    private static int Stage = -1;
    private static readonly UserEntity curUser = new();
    private static readonly UserRepository UserRepository = new();

    private static ILogger<Program> _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Program>();

    private readonly BlankMenu BlankMenu;

    public Program(ILogger<Program> logger)
    {
        _logger = logger;
    }

    private static async Task Main()
    {
        _botClient = new TelegramBotClient("6610584532:AAHyYTG_Rz96QfQEc7H-Dk-7iHHb2PeQN0E");
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery },
            ThrowPendingUpdates = true
        };
        using var cts = new CancellationTokenSource();
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);
        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"{me.FirstName} running!");
        await Task.Delay(-1);
    }


    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.CallbackQuery:
                {
                    await CallbackDataRepository.HandleCallBackQuery(botClient, update);
                    break;
                }
                case UpdateType.Message:
                {
                    var message = update.Message;
                    var chat = message.Chat;

                    _logger.LogInformation($"message.From.Id: {message.From.Id}");
                    if (!UserRepository.IsUserExists(message.From.Id))
                    {
                        _logger.LogInformation("message.From.Id: user created");
                        UserRepository.CreateUser(message.From.Id);
                        UserRepository.SetUserTgUsername(message.From.Id, message.From.Username);
                    }
                    else if(UserRepository.GetUserStage(message.From.Id) == 6)//твоя анкета выглядит так:
                    {
                        /*var user = UserRepository.GetUser(message.From.Id);
                        
                        Console.WriteLine("Фотография найдена.");
                        var photo = message.Photo.LastOrDefault();
                        var file = await botClient.GetFileAsync(photo.FileId);

                        var filePath = $"../../../photos/{user.TgId}.jpg";

                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await botClient.DownloadFileAsync(file.FilePath, fileStream);
                            fileStream.Close();
                        }
                        await using Stream stream = File.OpenRead(filePath);
                        var caption = $"Твоя анкета выглядит так:\n" +
                                      $"{user.Name}, {user.Age} лет, {user.City}\n" +
                                      $"{user.About}";

                        await botClient.SendPhotoAsync(
                            message.Chat.Id,
                            InputFile.FromStream(stream, "photo.jpg"),
                            caption: caption,
                            parseMode: ParseMode.Markdown
                        );*//**/
                    }
                    

                    switch (message.Type)
                    {
                        case MessageType.Text:
                        {
                            // Stage = -1 почему?
                            await BlankMenu.HandleMessageTypeText(message, botClient, chat, cancellationToken, curUser);
                            return;
                        }
                        case MessageType.Photo:
                        {
                            await BlankMenu.HandleMessageTypePhoto(message, botClient, chat);
                            return;
                        }
                    }
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }


    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}