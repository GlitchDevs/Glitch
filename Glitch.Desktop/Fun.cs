using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;

namespace Glitch.Desktop
{
    class Fun
    {
        public static Bitmap Capture(int height, int width)
        {
                Bitmap captureBitmap = new Bitmap(height, width, PixelFormat.Format32bppArgb);
                Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
                Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                return captureBitmap;
        }
    }
}
