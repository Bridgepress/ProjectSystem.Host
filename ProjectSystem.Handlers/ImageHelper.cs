using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Imaging;

public class ImageHelper
{
    private const int MaxWidth = 320;
    private const int MaxHeight = 240;

    public static bool IsValidImage(IFormFile imageFile)
    {
        if (imageFile == null) return false;

        try
        {
            using (var image = System.Drawing.Image.FromStream(imageFile.OpenReadStream()))
            {
                return image.RawFormat.Equals(ImageFormat.Jpeg) ||
                       image.RawFormat.Equals(ImageFormat.Png) ||
                       image.RawFormat.Equals(ImageFormat.Gif);
            }
        }
        catch
        {
            return false;
        }
    }

    public static byte[] ResizeImage(IFormFile imageFile)
    {
        using (var image = System.Drawing.Image.FromStream(imageFile.OpenReadStream()))
        {
            int newWidth = image.Width;
            int newHeight = image.Height;

            // Пропорциональное изменение размера
            if (image.Width > MaxWidth || image.Height > MaxHeight)
            {
                var ratioX = (double)MaxWidth / image.Width;
                var ratioY = (double)MaxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                newWidth = (int)(image.Width * ratio);
                newHeight = (int)(image.Height * ratio);
            }

            using (var newImage = new Bitmap(newWidth, newHeight))
            {
                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                }

                using (var memoryStream = new MemoryStream())
                {
                    newImage.Save(memoryStream, image.RawFormat);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
