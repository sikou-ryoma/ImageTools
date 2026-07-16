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

        // 出力ルート作成
        _fileHelper.CreateOutputDirectory();

        // 入力フォルダ内のディレクトリ構造を再現（空フォルダ含む）
        try
        {
            foreach (string dir in _fileHelper.GetAllDirectories())
            {
                string relativeDir = Path.GetRelativePath(_fileHelper.InputDirectory, dir);
                string outDir = Path.Combine(_fileHelper.OutputDirectory, relativeDir);
                Directory.CreateDirectory(outDir);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("ディレクトリ再現に失敗しました:");
            Console.WriteLine(ex.Message);
            return;
        }

        List<string> files =
            _fileHelper.GetAllFiles().ToList();

        Console.WriteLine($"対象ファイル：{files.Count}件");
        Console.WriteLine();

        int success = 0;
        int error = 0;
        int index = 0;

        foreach (string inputPath in files)
        {
            index++;

            try
            {
                string extension = Path.GetExtension(inputPath).ToLowerInvariant();

                Console.WriteLine(
                    $"[{index}/{files.Count}] {Path.GetFileName(inputPath)}");

                string outputPath = _fileHelper.GetOutputPathForConversion(inputPath);

                _imageResizer.Resize(inputPath, outputPath);

                success++;
            }

            catch (Exception ex)
            {
                error++;

                Console.WriteLine("エラー");

                Console.WriteLine(ex.Message);

                Console.WriteLine();
            }
        }

        Console.WriteLine();

        Console.WriteLine("==========");

        Console.WriteLine($"成功 : {success}");

        Console.WriteLine($"失敗 : {error}");

        Console.WriteLine("==========");

        Console.WriteLine("※画像以外のファイルは無視されました。");

    }
}