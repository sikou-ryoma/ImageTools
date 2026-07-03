namespace ImageResizer.Models;

internal sealed class AppSettings
{
    public string InputFolder { get; set; } = "";

    public string OutputFolder { get; set; } = "";

    public ResizeSettings Resize { get; set; } = new();

    public OutputSettings Output { get; set; } = new();

    public List<string> Extensions { get; set; } = [];
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