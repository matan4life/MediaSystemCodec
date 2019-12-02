using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Encoding;
using LinearAlgebraModule;
using MyVideoCodec;
using MyVideoCodec.Interfaces;
using Riff;
using Riff.Avi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CustomVideoApplication
{
    class Program
    {
        static string Path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        static void Main(string[] args)
        {
            MediaToolkit.FFmpegPath = @$"{Directory.GetCurrentDirectory()}\ffmpeg\x64";
            var test = MediaFile.Open($"{Path}/Test1.avi");
            var frame = test.Video.ReadNextFrame();
            Console.WriteLine(frame.Data.Length);
            //IColorTransformable colorTransformer = new ColorTransformer();
            //IDctTransformable dctTransformer = new DctTransformer();
            //IQuantizable quantizer = new Quantizer();
            //ICompressable compressor = new MyVideoCompresser(colorTransformer,
            //    dctTransformer,
            //    quantizer);
            //var test = new byte[768];
            //new Random((int)DateTime.Now.Ticks).NextBytes(test);
            //var result = compressor.Compress(test);
            //Console.WriteLine(result.Count());
        }

        static IDictionary<sbyte, decimal> GetEntropy(IEnumerable<sbyte> bytes)
        {
            return bytes.GroupBy(x => x).Select(x => new KeyValuePair<sbyte, decimal>(x.Key, ((decimal)bytes.Count(y => y == x.Key)) / bytes.Count())).ToDictionary(x => x.Key, x => x.Value);
        }

        static void ConvertFrameToImage(byte[] frame, string fileName, int width, int height)
        {
            var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Marshal.Copy(frame, 0, data.Scan0, frame.Length);
            bitmap.UnlockBits(data);
            var image = bitmap as Image;
            image.Save(fileName);
        }
    }
}
