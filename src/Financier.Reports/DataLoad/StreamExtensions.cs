using System.IO;

namespace Financier.Reports.DataLoad
{
    public static class StreamExtensions
    {
        public static void CopyTo(this Stream input, Stream output)
        {
            byte[] buffer = new byte[16384];
            int count;
            while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, count);
        }
    }
}