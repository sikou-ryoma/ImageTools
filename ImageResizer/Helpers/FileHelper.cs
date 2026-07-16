using System.IO;
using System.Collections.Generic;
using ImageResizer.Models;

namespace ImageResizer.Helpers;

internal sealed class FileHelper
{
    private readonly AppSettings _settings;

    public FileHelper(AppSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Inputフォルダの絶対パス
    /// </summary>
    public string InputDirectory
    {
        get
        {
            if (Path.IsPathRooted(_settings.InputFolder))
            {
                return _settings.InputFolder;
            }

            return Path.Combine(AppContext.BaseDirectory, _settings.InputFolder);
        }
    }

    /// <summary>
    /// 入力フォルダの親ディレクトリ（相対パスの基準）
    /// 親が取れない場合は InputDirectory を返す
    /// </summary>
    private string BaseInputDirectory
    {
        get
        {
            try
            {
                var parent = Directory.GetParent(InputDirectory);
                if (parent != null && !string.IsNullOrWhiteSpace(parent.FullName))
                {
                    return parent.FullName;
                }
            }
            catch
            {
                // Fall through to return InputDirectory
            }

            return InputDirectory;
        }
    }

    /// <summary>
    /// Outputフォルダの絶対パス
    /// </summary>
    public string OutputDirectory
    {
        get
        {
            if (Path.IsPathRooted(_settings.OutputFolder))
            {
                return _settings.OutputFolder;
            }

            return Path.Combine(AppContext.BaseDirectory, _settings.OutputFolder);
        }
    }

    /// <summary>
    /// Inputフォルダが存在するか
    /// </summary>
    public bool ExistsInputDirectory()
    {
        return Directory.Exists(InputDirectory);
    }

    /// <summary>
    /// Outputフォルダを作成
    /// </summary>
    public void CreateOutputDirectory()
    {
        Directory.CreateDirectory(OutputDirectory);
    }

    /// <summary>
    /// 入力ディレクトリ内のすべてのファイルを取得（画像に限定しない）
    /// </summary>
    public IEnumerable<string> GetAllFiles()
    {
        return Directory
            .EnumerateFiles(
                InputDirectory,
                "*.*",
                SearchOption.AllDirectories);
    }

    /// <summary>
    /// 入力ディレクトリ内のすべてのディレクトリを取得（空フォルダ再現用）
    /// 入力フォルダ自身も含める
    /// </summary>
    public IEnumerable<string> GetAllDirectories()
    {
        // まず入力フォルダ自身を返す（出力側に選択フォルダ名を作るため）
        yield return InputDirectory;

        foreach (var dir in Directory.EnumerateDirectories(InputDirectory, "*", SearchOption.AllDirectories))
        {
            yield return dir;
        }
    }

    /// <summary>
    /// 画像か判定（既存メソッドを保持）
    /// </summary>
    public bool IsImageFile(string path)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();

        return _settings.Extensions.Contains(extension);
    }

    /// <summary>
    /// 出力先パスを生成（変換して拡張子を置き換える：HEIC -> 設定の出力拡張子）
    /// 相対パスは InputDirectory を基準にする（選択フォルダを含める）
    /// </summary>
    public string GetOutputPathForConversion(string inputPath)
    {
        // 基準を BaseInputDirectory から InputDirectory に変更
        string relativePath =
            Path.GetRelativePath(InputDirectory, inputPath);

        string outputPath =
            Path.Combine(OutputDirectory, relativePath);

        outputPath =
            Path.ChangeExtension(
                outputPath,
                _settings.Output.Extension);

        string? directory =
            Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return outputPath;
    }

    /// <summary>
    /// 出力先パスを生成（拡張子を保持してそのままコピーする用）
    /// 相対パスは InputDirectory を基準にする（選択フォルダを含める）
    /// </summary>
    public string GetOutputPathForCopy(string inputPath)
    {
        // 基準を BaseInputDirectory から InputDirectory に変更
        string relativePath =
            Path.GetRelativePath(InputDirectory, inputPath);

        string outputPath =
            Path.Combine(OutputDirectory, relativePath);

        string? directory =
            Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return outputPath;
    }
}