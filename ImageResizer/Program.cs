using ImageResizer.Config;
using ImageResizer.Services;
using ImageConverter = ImageResizer.Services.ImageConverter;
using System.Windows.Forms;

namespace ImageResizer;

internal class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        try
        {
            AppConfig.Initialize();

            Console.WriteLine($"{AppConfig.Settings.AppName} - v{AppConfig.Settings.AppVersion}");
            Console.WriteLine("処理を開始します。");

            // コマンドライン引数を優先して入力フォルダを取得
            string? inputFolder = args.Length > 0 ? args[0] : null;

            // 引数がない場合はフォルダ選択ダイアログを表示
            if (string.IsNullOrWhiteSpace(inputFolder))
            {
                using var dialog = new FolderBrowserDialog
                {
                    Description = "入力フォルダを選択してください。",
                    UseDescriptionForTitle = true,
                    SelectedPath = AppContext.BaseDirectory
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    inputFolder = dialog.SelectedPath;
                }
            }

            if (string.IsNullOrWhiteSpace(inputFolder))
            {
                Console.WriteLine("入力フォルダが選択されませんでした。処理を中止します。");
                return;
            }

            // 実行時に設定の入力フォルダを上書き
            AppConfig.Settings.InputFolder = inputFolder;

            ImageConverter converter = new(AppConfig.Settings);

            converter.Execute();

            Console.WriteLine();
            Console.WriteLine("処理が完了しました。");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        Console.WriteLine();
        Console.WriteLine("何かキーを押してください。");

        Console.ReadKey();
    }
}