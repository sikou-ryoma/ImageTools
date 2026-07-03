using ImageResizer.Config;
using ImageResizer.Services;
using ImageConverter = ImageResizer.Services.ImageConverter;

namespace ImageResizer;

internal class Program
{
    static void Main()
    {
        try
        {
            AppConfig.Initialize();

            ImageConverter Converter = new(AppConfig.Settings);

            Converter.Execute();

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