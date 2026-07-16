namespace ImageResizer.Models;

internal sealed class AppSettings
{
    public string AppName { get; set; } = "";

    public string AppVersion { get; set; } = "";

    public string InputFolder { get; set; } = "";

    public string OutputFolder { get; set; } = "";

    public ResizeSettings Resize { get; set; } = new();

    public OutputSettings Output { get; set; } = new();

    // 修正: 空のリストで初期化する（コンパイルエラー回避）
    public List<string> Extensions { get; set; } = new List<string>();
}

internal sealed class ResizeSettings
{
    public int MaxLongSide { get; set; }

    public bool AllowUpscale { get; set; }
}

internal sealed class OutputSettings
{
    public string Extension { get; set; } = "";

    public int Quality { get; set; }
}