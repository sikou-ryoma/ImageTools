using ImageMagick;
using ImageResizer.Models;

namespace ImageResizer.Services;

internal sealed class ImageResizer
{
    private readonly AppSettings _settings;

    public ImageResizer(AppSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// 画像を読み込み、リサイズしてJPEG保存する
    /// </summary>
    /// <param name="inputPath">入力画像</param>
    /// <param name="outputPath">出力画像</param>
    public void Resize(string inputPath, string outputPath)
    {
        using MagickImage image = new(inputPath);

        ResizeImage(image);

        SaveAsJpeg(image, outputPath);
    }

    /// <summary>
    /// 長辺を指定サイズに縮小
    /// </summary>
    private void ResizeImage(MagickImage image)
    {
        int width = (int)image.Width;
        int height = (int)image.Height;

        int longSide = Math.Max(width, height);

        if (!_settings.Resize.AllowUpscale &&
            longSide <= _settings.Resize.MaxLongSide)
        {
            return;
        }

        double scale =
            (double)_settings.Resize.MaxLongSide / longSide;

        int newWidth =
            (int)Math.Round(width * scale);

        int newHeight =
            (int)Math.Round(height * scale);

        image.Resize((uint)newWidth, (uint)newHeight);
    }

    /// <summary>
    /// JPEG保存
    /// </summary>
    private void SaveAsJpeg(
        MagickImage image,
        string outputPath)
    {
        image.Format = MagickFormat.Jpeg;

        image.Quality = (uint)_settings.Output.Quality;

        image.Write(outputPath);
    }
}