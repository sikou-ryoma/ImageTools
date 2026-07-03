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
    public string InputDirectory =>
        Path.Combine(AppContext.BaseDirectory, _settings.InputFolder);

    /// <summary>
    /// Outputフォルダの絶対パス
    /// </summary>
    public string OutputDirectory =>
        Path.Combine(AppContext.BaseDirectory, _settings.OutputFolder);

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
    /// 対象画像を取得
    /// </summary>
    public IEnumerable<string> GetImageFiles()
    {
        return Directory
            .EnumerateFiles(
                InputDirectory,
                "*.*",
                SearchOption.AllDirectories)
            .Where(IsImageFile);
    }

    /// <summary>
    /// 画像か判定
    /// </summary>
    public bool IsImageFile(string path)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();

        return _settings.Extensions.Contains(extension);
    }

    /// <summary>
    /// 出力先パスを生成
    /// </summary>
    public string GetOutputPath(string inputPath)
    {
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
}