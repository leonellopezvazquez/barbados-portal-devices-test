using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace barbados_portal_test
{
    public class Utilities
    {

        public OverObject ParseImageName(string name)
        {
            using (OverObject imagen = new OverObject())
            {
                try
                {
                    imagen.type = name.Substring(0, 1);
                    name = name.Replace(".jpg", "");
                    string[] cadenas = name.Split(',');

                    string sFecha = cadenas[0].Substring(1, cadenas[0].Length - 1);
                    string sHora = cadenas[1];
                    sFecha = sFecha.Substring(2, 2) + "-" + sFecha.Substring(0, 2) + "-" + sFecha.Substring(4, 4);
                    sHora = sHora.Substring(0, 2) + ":" + sHora.Substring(2, 2) + ":" + sHora.Substring(4, 2);
                    DateTime dt = DateTime.ParseExact(sFecha + " " + sHora, "MM-dd-yyyy HH:mm:ss", null);

                    imagen.datetime = dt.ToString("yyyy/MM/dd HH:mm:ss");
                    imagen.time = sHora;
                    imagen.Hex = cadenas[2];
                    imagen.plate = cadenas[3];
                    imagen.conf = Int16.Parse(cadenas[4]);
                    imagen.lane = Int16.Parse(cadenas[5]);
                    imagen.idCamera = cadenas[6];

                    if (cadenas.Length > 7)
                    {
                        imagen.lattitude = cadenas[7];
                        imagen.longitude = cadenas[8];
                    }

                    cadenas = null;
                    return imagen;
                }
                catch (Exception ex)
                {
                    //Form1.log.Error(ex);
                    return null;
                }
            }
        }

        public Image ImagefromUrl(string url)
        {
            try
            {
                using (var wc = new WebClient())
                {
                    byte[] bytes = wc.DownloadData(url);
                    MemoryStream ms = new MemoryStream(bytes);
                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                    wc.Dispose();
                    ms.Dispose();
                    return img;
                }
            }
            catch (Exception ex)
            {
                //Form1.log.Error(ex);
                return null;
            }
        }

        public Image ImagefromPath(string path)
        {
            try
            {
                Image img = null;
                using (Image ms = Image.FromFile(path))
                {
                    img = ms;
                    return img;
                }
            }
            catch (Exception ex)
            {
                //Form1.log.Error(ex);
                return null;
            }

        }

        public Image Base64ToImage(string base64String)
        {
            try
            {
                // Convert base64 string to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String);
                // Convert byte[] to Image
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    Image image = Image.FromStream(ms, true);
                    return image;
                }
            }
            catch (Exception ex)
            {
                //Form1.log.Error(ex);
                return null;
            }
        }

        public MemoryStream Base64ToStream(string base64String)
        {
            try
            {
                var imageBytes = Convert.FromBase64String(base64String);
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    return ms;
                }
            }
            catch (Exception ex)
            {
                //Form1.log.Error(ex);
                return null;
            }
        }


        public string ImageToBase64(Image image)
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, ImageFormat.Jpeg);
                return Convert.ToBase64String(m.ToArray());
            }
        }

        public bool CreateDirectory(string path)
        {
            try
            {
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }
                return true;
            }
            catch (Exception ex)
            {
                //Form1.log.Error("Error Creating Directory: " + path, ex);
                return false;
            }
        }

        public string ImageFileToBase64(string path)
        {
            try
            {
                using (Image image = Image.FromFile(path))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        string base64String = Convert.ToBase64String(imageBytes);
                        return base64String;
                    }
                }
            }
            catch (Exception ex)
            {
                //Form1.log.Error("ImageFileToBase64: " + path, ex);
                return null;
            }
        }


        public bool DeleteFiles(string nombre, string patch, string alternpatch)
        {
            try
            {
                File.Delete(nombre);
                if (File.Exists(patch))
                    File.Delete(patch);
                else
                    File.Delete(alternpatch);
            }
            catch (Exception ex)
            {
                //Form1.log.Error("Error Deleting file:  " + ex);
            }
            return true;
        }

    }
}
