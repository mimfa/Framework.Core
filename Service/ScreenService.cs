using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MiMFa.Service
{
    public enum ScreenScope
    {
        Screen,
        Window
    }
    public class ScreenService
    {

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static Rectangle GetScreenRect()
        {
            return Screen.GetBounds(Point.Empty);
        }
        public static Size GetScreenSize()
        {
            return GetScreenRect().Size;
        }

        public static Bitmap Capture(ScreenScope screenCaptureMode = ScreenScope.Window)
        {
            Rectangle bounds;

            if (screenCaptureMode == ScreenScope.Screen)
            {
                bounds = Screen.GetBounds(Point.Empty);
                _CursorPosition = Cursor.Position;
            }
            else
            {
                var foregroundWindowsHandle = GetForegroundWindow();
                var rect = new Rect();
                GetWindowRect(foregroundWindowsHandle, ref rect);
                bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                _CursorPosition = new Point(Cursor.Position.X - rect.Left, Cursor.Position.Y - rect.Top);
            }
            return Capture(bounds);
        }
        public static Bitmap Capture(Control control)
        {
            var rect = control.RectangleToScreen(control.ClientRectangle);
            _CursorPosition = new Point(Cursor.Position.X - rect.Left, Cursor.Position.Y - rect.Top);
            return Capture(new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top));
        }
        public static Bitmap Capture(Rectangle bounds)
        {
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var g = Graphics.FromImage(result))
            {
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }
            return result;
        }

        public static Color ColorAt(Point location, ScreenScope screenCaptureMode = ScreenScope.Screen)
        {
            var screenPixel = Capture(screenCaptureMode);
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }
        public static Color ColorAtCursor()
        {
            return ColorAt(CursorPosition, ScreenScope.Window);
        }
        protected static Point _CursorPosition
        {
            get;
            set;
        }
        public static Point CursorPosition
        {
            get
            {
                Point p = new Point();
                GetCursorPos(ref p);
                return p;
            }
        }
    }
}
