using System;
using System.IO;
using System.Linq;
using System.Net;
using static System.IO.Directory;

namespace Cake.Json
{
    public static class OsuDlBeatmap
    {
        private static readonly string Path = AppDomain.CurrentDomain.BaseDirectory + @"\osu";

        public static byte[] FindMap(int beatmapId)
        {
            CreateFolder();
            try
            {
                return GetMap(beatmapId);
            }
            catch
            {
               //
            }

            return null;
        }

        private static void CreateFolder()
        {
            if (Exists(Path))
            {
                return;
            }

            CreateDirectory(Path);
        }

        private static byte[] DownloadData(string filename, int beatmapId)
        {
            var data = new WebClient().DownloadData($"https://osu.ppy.sh/osu/{beatmapId}");
            if (!File.Exists(filename))
                File.WriteAllBytes($@"osu\{beatmapId}.txt", data.ToArray());
            return data;
        }

        private static byte[] GetMap(int beatmapId)
        {
            var filename = Path + $@"\{beatmapId}.txt";
            if (File.Exists(filename))
            {
                return File.ReadAllBytes(filename);
            }
            return DownloadData(filename, beatmapId);
        }
    }
}