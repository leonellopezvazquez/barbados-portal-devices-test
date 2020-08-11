using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace barbados_portal_test
{
    class ImageCropper
    {

        public static Image cropper(Image imagen, int lane)
        {
            if (lane == 1)
                return cropperleft(imagen);
            else
                return cropperRight(imagen);
        }

        public static Image cropperleft(Image imagen)
        {

            int width = imagen.Width;
            int height = imagen.Height;

            Rectangle destinationleft = new Rectangle(
                0,
                0,
                (width / 2) - 1,
                height - 1);

            Bitmap bmpImage = new Bitmap(imagen);
            Bitmap bmpCrop = bmpImage.Clone(destinationleft,
            bmpImage.PixelFormat);

            using (MemoryStream m = new MemoryStream())
            {
                bmpCrop.Save(m, imagen.RawFormat);
                byte[] imageBytes = m.ToArray();

                //Convert byte[] to Base64 String
                //string base64Stringview = Convert.ToBase64String(imageBytes);

                //Image nuevaimagen = Base64ToImage(base64Stringview);

            }

            return (Image)(bmpCrop);
        }

        public static Image cropperRight(Image imagen)
        {
            int width = imagen.Width;
            int height = imagen.Height;

            Rectangle destinationright = new Rectangle(
            width / 2,
            0,
             (width / 2) - 1,
             height - 1);

            Bitmap bmpImage = new Bitmap(imagen);
            Bitmap bmpCrop = bmpImage.Clone(destinationright,
            bmpImage.PixelFormat);


            using (MemoryStream m = new MemoryStream())
            {
                bmpCrop.Save(m, imagen.RawFormat);
                byte[] imageBytes = m.ToArray();

                //Convert byte[] to Base64 String
                //string base64Stringview = Convert.ToBase64String(imageBytes);

                //Image nuevaimagen = Base64ToImage(base64Stringview);

            }

            return (Image)(bmpCrop);
        }


        public static bool replaceOldImage(Image nuevaimagen, string filename)
        {

            return true;
        }


        public static Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }


        private static Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

    }
}
