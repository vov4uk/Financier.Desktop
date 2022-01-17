using System;
using System.IO;
using System.IO.Compression;

namespace Financier.Reports.DataLoad
{
    public static class FilePrepare
    {
        public static string Prepare(string backupDir)
        {
            FileInfo[] files = new DirectoryInfo(backupDir).GetFiles("*.backup");
            FileInfo fileToDecompress = files.Length > 0 ? files[0] : throw new Exception("В указаной директории отсутствуют файлы бекапа!");
            foreach (FileInfo fileInfo in files)
            {
                if (fileToDecompress.CreationTime < fileInfo.CreationTime)
                    fileToDecompress = fileInfo;
            }
            return Decompress(fileToDecompress);
        }

        public static string Decompress(FileInfo fileToDecompress)
        {
            string tempFileName = Path.GetTempFileName();
            using (FileStream fileStream = fileToDecompress.OpenRead())
            {
                using (FileStream destination = File.Create(tempFileName))
                {
                    using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                        gzipStream.CopyTo(destination);
                }
            }
            return tempFileName;
        }
    }
}