using ImageResizer.Helpers;
using ImageResizer.Models;

namespace ImageResizer.Services;

internal sealed class ImageConverter
{
    private readonly AppSettings _settings;

    private readonly FileHelper _fileHelper;

    private readonly ImageResizer _imageResizer;

    public ImageConverter(AppSettings settings)
    {
        _settings = settings;

        _fileHelper = new(settings);

        _imageResizer = new(settings);
    }

    public void Execute()
    {
        if (!_fileHelper.ExistsInputDirectory())
        {
            Console.WriteLine("Inputフォルダが見つかりません。");
            Console.WriteLine(_fileHelper.InputDirectory);

            return;
        }

        _fileHelper.CreateOutputDirectory();

        List<string> files =
            _fileHelper.GetImageFiles().ToList();

        Console.WriteLine($"対象画像：{files.Count}件");
        Console.WriteLine();

        int success = 0;
        int error = 0;

        foreach (string inputPath in files)
        {
            try
            {
                string outputPath =
                    _fileHelper.GetOutputPath(inputPath);

                Console.WriteLine(
                    $"[{success + error + 1}/{files.Count}] " +
                    Path.GetFileName(inputPath));

                _imageResizer.Resize(
                    inputPath,
                    outputPath);

                success++;
            }
            catch (Exception ex)
            {
                error++;

                Console.WriteLine($"エラー");

                Console.WriteLine(ex.Message);

                Console.WriteLine();
            }
        }

        Console.WriteLine();

        Console.WriteLine("==========");

        Console.WriteLine($"成功 : {success}");

        Console.WriteLine($"失敗 : {error}");

        Console.WriteLine("==========");
    }
}