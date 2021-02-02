using AForge.Video.DirectShow;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glitch.Desktop
{
    public class Webcam
    {
        public static string[] ListDevices()
        {
            FilterInfoCollection devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            List<FilterInfo> _devices = new List<FilterInfo>();
            foreach (FilterInfo device in devices) _devices.Add(device);
            return _devices.Select(item => item.Name).ToArray();
        }

        public static async Task<MemoryStream> MakePhoto(int index)
        {
            FilterInfoCollection devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FilterInfo device = devices[index];
            VideoCaptureDevice source = new VideoCaptureDevice(device.MonikerString);

            VideoCapabilities[] allCapabilities = source.VideoCapabilities;
            VideoCapabilities capabilities = allCapabilities
                .OrderByDescending((item) => item.FrameSize.Width * item.FrameSize.Height)
                .FirstOrDefault();

            if (capabilities != null) source.VideoResolution = capabilities;

            Size frameSize = source.VideoResolution != null ? source.VideoResolution.FrameSize : new Size(0, 0);

            bool stop = false;

            MemoryStream _stream = null;

            source.NewFrame += async (frameSender, frameArgs) =>
            {
                if (!stop)
                {
                    stop = true;

                    source.SignalToStop();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (Bitmap bitmap = frameArgs.Frame)
                        {
                            bitmap.Save(stream, ImageFormat.Png);
                        }

                        stream.Seek(0, SeekOrigin.Begin);

                        _stream = new MemoryStream(stream.ToArray()); ;
                    }
                }
            };

            source.Start();

            while (_stream == null)
            {
                await Task.Delay(500);
            }

            return _stream;
        }
    }
}
