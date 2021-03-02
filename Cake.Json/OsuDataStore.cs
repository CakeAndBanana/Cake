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

        public static byte[] FindMap(int beatmapId, DateTime mapDate)
        {
            CreateFolder();
            try
            {
                return GetMap(beatmapId, mapDate);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            File.WriteAllBytes($@"osu\{beatmapId}.txt", data.ToArray());
            return data;
        }

        private static byte[] GetMap(int beatmapId, DateTime mapDate)
        {
            var filename = Path + $@"\{beatmapId}.txt";
            var writeTime = File.GetLastWriteTime(filename);
            if (File.Exists(filename) && writeTime > mapDate)
            {
                return File.ReadAllBytes(filename);
            }
            return DownloadData(filename, beatmapId);
        }
    }
}