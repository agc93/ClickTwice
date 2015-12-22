using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickTwice.UpdateManager
{
    public class UpdateInfo
    {
        public Version NewVersion { get; internal set; }
        public bool Mandatory { get; internal set; }
        public Version MinimumVersion { get; internal set; }
        public DownloadSize DownloadSize { get; internal set; }

        public Uri UpdateSource { get; internal set; }

        public string InfoMessage
            =>
                $"New update available! New {(Mandatory ? "required update to" : string.Empty)} version {NewVersion.ToString(4)} ({DownloadSize} download)"
            ;
    }

    public class DownloadSize
    {
        private long ByteCount { get; set; }
        public DownloadSize(int bytes)
        {
            ByteCount = bytes;
        }

        public DownloadSize(long bytes)
        {
            ByteCount = bytes;
        }

        public long Bytes => ByteCount;

        public long Kilobytes => (ByteCount/1024);
        public long Megabytes => ((ByteCount/1024)/1024);
        public long Gigabytes => (((ByteCount/1024)/1024)/1024);
        public override string ToString()
        {
            return GetBytesReadable(ByteCount);
        }

        public string ToString(int places)
        {
            string suffix = "0.";
            for (int i = 0; i < places; i++)
            {
                suffix = suffix + "#";
            }
            var format = suffix + " ";
            return GetBytesReadable(ByteCount, format);
        }

        // Returns the human-readable file size for an arbitrary, 64-bit file size 
        // The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        private string GetBytesReadable(long i, string format = "0.## ")
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString(format) + suffix;
        }
    }
}
