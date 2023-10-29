using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;

public class GoogleDriveManager
{
    private const string AppDataFolderName = "appDataFolder";
    private const string DefaultDataStoreFolderName = ".authdata";
    private const string ClientSecretsFilePath = "C:\\Users\\rosny\\source\\repos\\Lab1\\Lab1.GoogleDrive\\client_secret_1090855896705-kmplfaso155s1sa8ohebfjsn2qhd8lvb.apps.googleusercontent.com.json";

    private static DriveService? driveService;

    public static async Task InitClientAsync()
    {
        if (driveService is not null)
        {
            return;
        }

        var clientSecrets = (await GoogleClientSecrets.FromFileAsync(ClientSecretsFilePath)).Secrets;
        var dataStore = new FileDataStore(DefaultDataStoreFolderName);
        var scopes = new[]
        {
            DriveService.Scope.DriveAppdata
        };

        var credential =
            await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets
                , scopes
                , "user"
                , CancellationToken.None
                , dataStore);

        driveService = new DriveService(new DriveService.Initializer
        {
            HttpClientInitializer = credential
        });
    }

    public static async Task ListFilesAsync()
    {
        if (driveService is null)
        {
            throw new InvalidOperationException("Drive service has not been initialized");
        }

        var request = driveService.Files.List();
        request.Spaces = AppDataFolderName;
        request.Fields = "files(id, name)";

        var result = await request.ExecuteAsync();

        foreach (var file in result.Files)
        {
            await Console.Out.WriteLineAsync($"Found file: {file.Name} ({file.Id})");
        }
    }

    public static async Task<string> WriteFileAsync(string fileName, string content)
    {
        if (driveService is null)
        {
            return "Помилка: сервіс Google Drive не було ініціалізовано.";
        }

        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(content))
        {
            return "Помилка: невірна назва файлу або вміст файлу.";
        }

        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = $"{fileName}.txt",
            Parents = new List<string>() { AppDataFolderName }
        };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var request = driveService.Files.Create(fileMetadata, stream, "text/plain");
        request.Fields = "id";

        var uploadResult = await request.UploadAsync();

        if (uploadResult.Exception is not null)
        {
            return $"Помилка при завантаженні файлу: {uploadResult.Exception.Message}";
        }

        return $"Файл успішно створено з ID: {request.ResponseBody.Id}";
    }

    public static async Task<string> ReadFileContentAsync(string fileId)
    {
        if (driveService is null)
        {
            throw new InvalidOperationException("Drive service has not been initialized");
        }

        var request = driveService.Files.Get(fileId);

        using var stream = new MemoryStream();

        var downloadResult = await request.DownloadAsync(stream);

        if (downloadResult.Exception is not null)
        {
            throw new Exception($"Failed to download the file: {downloadResult.Exception.Message}");
        }
        else
        {
            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
    }

}