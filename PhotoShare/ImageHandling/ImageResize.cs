using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace PhotoShare.ImageHandling
{
    public class ImageResize
    {

        public Stream ToJpg(Stream fileStream, double width, double height)
        {
            //The file will be corrupted if not read from the beginning
            fileStream.Position = 0;
            using (var image = Image.FromStream(fileStream))
            {
                Stream outputStream = new MemoryStream();

                if (image.Width > image.Height)
                {
                    var ratio = image.Width / width;
                    height = image.Height / ratio;
                }
                else
                {
                    var ratio = image.Height / height;
                    width = image.Width / ratio;
                }

                var newImage = new Bitmap(Convert.ToInt32(width), Convert.ToInt32(height));
                var rectangle = new Rectangle(0, 0, Convert.ToInt32(width), Convert.ToInt32(height));

                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    graphics.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                }
                ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

                foreach (var codec in encoders)
                {
                    if (codec.MimeType != "image/jpeg") continue;

                    var encoderParameters = new EncoderParameters
                    {
                        Param = { [0] = new EncoderParameter(Encoder.Quality, 90L) }
                    };

                    newImage.Save(outputStream, codec, encoderParameters);
                }

                return outputStream;
            }
        }

    }
}