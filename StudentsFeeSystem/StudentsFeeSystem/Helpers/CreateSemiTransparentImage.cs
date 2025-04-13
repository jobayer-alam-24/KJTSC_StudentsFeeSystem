using System.Drawing;
using System.Drawing.Imaging;

public static class ImageHelper
{
    public static void CreateSemiTransparentImage(string inputImagePath, string outputImagePath, float opacity)
    {
        using (var originalImage = Image.FromFile(inputImagePath))
        {
            var width = originalImage.Width;
            var height = originalImage.Height;
            var transparentImage = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(transparentImage))
            {
                var colorMatrix = new ColorMatrix
                {
                    Matrix33 = opacity // Set the opacity value (0.0f to 1.0f)
                };

                var imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                graphics.DrawImage(originalImage, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel, imageAttributes);
            }

            transparentImage.Save(outputImagePath, ImageFormat.Png);
        }
    }
}
