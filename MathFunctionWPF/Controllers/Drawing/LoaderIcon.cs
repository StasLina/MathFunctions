using System;
using System.IO;
using System.Windows.Media.Imaging;
using SkiaSharp;
using SkiaSharp.Extended.Svg;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace MathFunctionWPF.Controllers.Drawing
{
    internal class LoaderIcon
    {
        public static SKBitmap LoadSvgIcon(System.IO.Stream stream, int width, int height)
        {
            // Чтение файла SVG
            var svg = new SKSvg();
            svg.Load(stream);

            // Создание битмапа с заданными размерами и поддержкой альфа-канала
            var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
            var bitmap = new SKBitmap(info);

            using (var canvas = new SKCanvas(bitmap))
            {
                // Белый фон, если нужно, можно заменить на прозрачность
                canvas.Clear(SKColors.Transparent);

                // Масштабируем и отрисовываем SVG
                var scaleX = (float)width / svg.Picture.CullRect.Width;
                var scaleY = (float)height / svg.Picture.CullRect.Height;
                var matrix = SKMatrix.MakeScale(scaleX, scaleY);

                canvas.DrawPicture(svg.Picture, ref matrix);
            }

            return bitmap;
        }

        public static BitmapImage ConvertToBitmapImage(SKBitmap skBitmap)
        {
            // Создаем MemoryStream для сохранения изображения
            using (var memoryStream = new MemoryStream())
            {
                // Сохранение SKBitmap в поток в формате PNG (с альфа-каналом)
                using (var skImage = SKImage.FromBitmap(skBitmap))
                using (var skData = skImage.Encode(SKEncodedImageFormat.Png, 100)) // 100 - качество PNG
                {
                    skData.SaveTo(memoryStream);
                }

                // Сбрасываем позицию потока в начало, чтобы загрузить в BitmapImage
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Создаем BitmapImage и загружаем данные из потока
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // Загружаем изображение сразу
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();

                // Возвращаем готовый BitmapImage
                return bitmapImage;
            }
        }

        public static BitmapImage LoadPngImage(byte[] imageBytes)
        {
            // Создаем новый BitmapImage
            BitmapImage bitmap = new BitmapImage();

            try
            {
                // Создаем MemoryStream из массива байтов
                using (MemoryStream stream = new MemoryStream(imageBytes))
                {
                    // Настроим BitmapImage
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;  // Кешируем изображение сразу
                    bitmap.StreamSource = stream;                   // Устанавливаем источник как поток
                    bitmap.EndInit();                               // Завершаем инициализацию
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки изображения: {ex.Message}");
            }



            return bitmap;
        }

    }
}
