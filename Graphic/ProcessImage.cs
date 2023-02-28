using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml;
using System.Drawing.Drawing2D;

namespace MiMFa.Graphic
{
    public class ProcessImage : ProcessGraphic<Image>
    {
        public override Image FlipHorizental(Image source)
        {
            var bitmap = new Bitmap(source);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return bitmap;
        }
        public override Image FlipVertical(Image source)
        {
            var bitmap = new Bitmap(source);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }
        public override Image RotateRight(Image source)
        {
            var bitmap = new Bitmap(source);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            return bitmap;
        }
        public override Image RotateLeft(Image source)
        {
            var bitmap = new Bitmap(source);
            bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
            return bitmap;
        }
        public override Image Rotate(Image source, float angle)
        {
            angle = angle % 360;
            if (angle == 0) return source;
            //create an empty Bitmap image
            Bitmap bmp = new Bitmap(source.Width, source.Height);

            //turn the Bitmap into a Graphics object
            Graphics gfx = Graphics.FromImage(bmp);

            //now we set the rotation point to the center of our image
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

            //now rotate the image
            gfx.RotateTransform(angle);

            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //now draw our new image onto the graphics object
            gfx.DrawImage(source, new Point(0, 0));

            //dispose of our Graphics object
            gfx.Dispose();

            //return the image
            return bmp;
        }
    
        public override Image Change(Image source, Func<Color, Color> colorProcess)
        {
            Bitmap bmp = new Bitmap(source);
            for (int x = 0; x < bmp.Width; x++)
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color c = bmp.GetPixel(x, y);
                    bmp.SetPixel(x, y, colorProcess(c));
                }
            return bmp;
        }

        public override Image Combine(params Image[] sources)
        {
            List<int> imageHeights = new List<int>();
            List<int> imageWidths = new List<int>();
            foreach (Image img in sources)
            {
                imageHeights.Add(img.Height);
                imageWidths.Add(img.Width);
            }
            Bitmap result = new Bitmap(imageWidths.Max(), imageHeights.Max());
            using (Graphics g = Graphics.FromImage(result))
            {
                foreach (Image img in sources)
                    g.DrawImage(img, Point.Empty);
            }
            return result;
        }
        public virtual Image MergeHorizontal(params Image[] sources)
        {
            List<int> imageHeights = new List<int>();
            List<int> imageWidths = new List<int>();
            foreach (Image img in sources)
            {
                imageHeights.Add(img.Height);
                imageWidths.Add(img.Width);
            }
            int height = imageHeights.Max();
            int width = imageHeights.Sum();
            Bitmap result = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(result);
            g.Clear(SystemColors.AppWorkspace);
            g.DrawImage(sources.First(), new Point(0, 0));
            width = sources.First().Width;
            foreach (Bitmap img in sources.Skip(1))
            {
                g.DrawImage(img, new Point(width, 0));
                width += img.Width;
            }
            g.Dispose();
            return result;
        }
        public virtual Image MergeVertical(params Image[] sources)
        {
            List<int> imageHeights = new List<int>();
            List<int> imageWidths = new List<int>();
            foreach (Image img in sources)
            {
                imageHeights.Add(img.Height);
                imageWidths.Add(img.Width);
            }
            int height = imageHeights.Sum();
            int width = imageHeights.Max();
            Bitmap result = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(result);
            g.Clear(SystemColors.AppWorkspace);
            g.DrawImage(sources.First(), new Point(0, 0));
            height = sources.First().Height;
            foreach (Image img in sources.Skip(1))
            {
                g.DrawImage(img, new Point(0, height));
                height += img.Height;
            }
            g.Dispose();
            return result;
        }


        /// <summary>
        /// Exif 2.2 requires that ASCII property items terminate with a null (0x00).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public virtual PropertyItem CreatePropertyItem(PropertyItem source, int id, byte[] comment)
        {
            source.Id = id;
            source.Type = 2;
            source.Value = comment;
            source.Len = comment.Length;
            return source;
        }
        public virtual PropertyItem CreatePropertyItem(PropertyItem source, int id, string comment)
        {
            return CreatePropertyItem(source,id,Encoding.UTF8.GetBytes(comment));
        }
        public virtual void AddMetadata(string path, string comment, ImageFormat imageformat)
        {
            AddMetadata(new System.IO.FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite), comment, imageformat);
        }
        public virtual void AddMetadata(Stream stream, string comment, ImageFormat imageformat)
        {
            BitmapDecoder decoder;
            if (imageformat == ImageFormat.Jpeg) decoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            else if (imageformat == ImageFormat.Png) decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            else if (imageformat == ImageFormat.Tiff) decoder = new TiffBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            else if (imageformat == ImageFormat.Bmp) decoder = new BmpBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            else if (imageformat == ImageFormat.Gif) decoder = new GifBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            else if (imageformat == ImageFormat.Icon) decoder = new IconBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            else if (imageformat == ImageFormat.Wmf) decoder = new WmpBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            else return;
            BitmapFrame pngFrame = decoder.Frames[0];
            InPlaceBitmapMetadataWriter pngInplace = pngFrame.CreateInPlaceBitmapMetadataWriter();
            if (pngInplace.TrySave() == true)
            { pngInplace.SetQuery("/Text/Description", comment); }
            stream.Close();
        }
    }
}
