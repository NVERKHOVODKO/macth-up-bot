namespace EntityFrameworkLesson.Utils;

public class DeleteFolderHandle
{
    public static void DeleteFolder(string folderPath)
    {
        try
        {
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
                Console.WriteLine($"Папка по пути {folderPath} успешно удалена.");
            }
            else
            {
                Console.WriteLine($"Папка по пути {folderPath} не существует.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении папки: {ex.Message}");
        }
    }
}