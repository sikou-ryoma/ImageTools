using ImageResizer.Models;
using Microsoft.Extensions.Configuration;

namespace ImageResizer.Config;

internal static class AppConfig
{
    public static AppSettings Settings { get; private set; } = new();

    public static void Initialize()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        Settings = config.Get<AppSettings>()
            ?? throw new Exception("設定ファイルの読み込みに失敗しました。");
    }
}