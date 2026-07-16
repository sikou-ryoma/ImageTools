using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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

        // 入力/出力の衝突チェック（正規化して比較）
        string inputFull = Path.GetFullPath(_fileHelper.InputDirectory)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string outputFull = Path.GetFullPath(_fileHelper.OutputDirectory)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        if (string.Equals(inputFull, outputFull, StringComparison.OrdinalIgnoreCase)
            || outputFull.StartsWith(inputFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("入力フォルダと出力フォルダが同じ、または出力が入力のサブフォルダになっています。設定を確認してください。");
            return;
        }

        // 出力ルート作成
        _fileHelper.CreateOutputDirectory();

        // 入力フォルダ内のディレクトリ構造を再現（空フォルダ含む）
        try
        {
            string topFolder = Path.GetFileName(_fileHelper.InputDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            foreach (string dir in _fileHelper.GetAllDirectories())
            {
                string relativeDir = Path.GetRelativePath(_fileHelper.InputDirectory, dir);
                string outDir = Path.Combine(_fileHelper.OutputDirectory, topFolder, relativeDir);
                Directory.CreateDirectory(outDir);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("ディレクトリ再現に失敗しました:");
            Console.WriteLine(ex.Message);
            return;
        }

        // 画像ファイルのみを対象に列挙
        List<string> files =
            _fileHelper.GetAllFiles()
                       .Where(p => _fileHelper.IsImageFile(p))
                       .ToList();

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

                Console.Write(
                    $"[{index}/{files.Count}] {Path.GetFileName(inputPath)}");

                string outputPath = _fileHelper.GetOutputPathForConversion(inputPath);

                _imageResizer.Resize(inputPath, outputPath);

                Console.WriteLine(" → 変換完了");

                success++;
            }

            catch (Exception ex)
            {
                error++;

                Console.WriteLine("エラー");

                Console.WriteLine(ex.ToString());

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