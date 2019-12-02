using Riff.Avi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Riff
{
    public class RiffVideo
    {
        public RiffVideo(string videoPath)
        {
            this.VideoPath = videoPath;
            this.Root = new RiffList(0, null);
            var bytes = File.ReadAllBytes(this.VideoPath);
            this.Root.Parse(bytes);
        }
        private RiffList Root { get; }
        private string VideoPath { get; }

        public string GetVideoInformation()
        {
            return string.Join('\n', this.GetChunk<AviMainHeader>(this.Root).Select(x => x.ToString()));
        }

        public string GetStreamInformation()
        {
            return string.Join('\n', this.GetChunk<AviStreamHeader>(this.Root).Select(x => x.ToString()));
        }

        public RiffChunk<byte> GetChunk(string type)
        {
            return this.GetChunk<byte>(this.Root).Where(x => x.Id.Contains(type)).FirstOrDefault() as RiffChunk<byte>;
        }

        private IEnumerable<RiffElement> GetChunk<T>(RiffList list) where T: struct
        {
            foreach (var child in list.Children)
            {
                if (child is RiffChunk<T>)
                {
                    yield return child;
                }
            }
            foreach (var newList in list.Children.Where(x=>x is RiffList))
            {
                var result = this.GetChunk<T>(newList as RiffList);
                if (!result.Any()) break;
                foreach (var element in result.ToList())
                {
                    yield return element;
                }
            }
        }
    }
}
