using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Gma.System.MouseKeyHook;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        long time1 = 0;
        int lastx1 = 0;
        int lasty1 = 0;
        long lastTime1 = 0;
        double vx = 0;
        double vy = 0;
        double v = 0;
        int vMag = 0;
        int timeDiff = 0;
        int xDiff = 0;
        int yDiff = 0;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Drawing.Point lastPoint = System.Drawing.Point.Empty;//Point.Empty represents null for a Point object
        int check = 0;
        int clickCheck = 0;
        string[] list1 = new string[4];
        long startTime = new StartTime().getStartTime();
        AutomationElement publicElement;
        ScreenElement structureInCondition;
        public Form1()
        {
            InitializeComponent();
            //var timer = new Timer();
            timer.Interval = 16;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            Thread th = new Thread(firstClick);
            th.Start();
            //firstClick();
            ScreenElement structure = ScreenElement.fromAutomationElement(ElementFromCursor());
            structureInCondition = structure;
            long currTime = new StartTime().getStartTime();
            long timeSinceStartSession = currTime - startTime;
            System.IO.File.AppendAllText(@"C:\Saif\Office\C#\Projects\MouseClick\file\" + startTime.ToString() + ".json", "\n{\"eventType\": \"mm\"" + ", \"mouseX\": " + Cursor.Position.X + ", \"mouseY\": " + Cursor.Position.Y+ ", \"time\": " + timeSinceStartSession + ", \"relative\": true" + "}");
        }
        private double clip(double a, double min = 0, double max = 1)
        {
            return Math.Max(min, Math.Min(max, a));
        }
        private Color interpolate(Color a, Color b, double ratio)
        {
            ratio = clip(ratio);
            return Color.FromArgb(128, (int)((b.R - a.R) * ratio + a.R), (int)((b.G - a.G) * ratio + a.G), (int)((b.B - a.B) * ratio + a.B));
        }
        private Color interpolate(List<Color> colors, double ratio)
        {
            ratio *= colors.Count;
            var lower = (int)clip(ratio, 0, colors.Count - 2);
            var upper = lower + 1;
            return interpolate(colors[lower], colors[upper], ratio - lower);
        }
        private void firstClick()
        {
            if (check == 0)
            {
                lastTime1 = new StartTime().getStartTime();
                lastPoint = Cursor.Position;
                //isMouseDown = true;
                check = 1;
            }
            else//if our last point is not null, which in this case we have assigned above

            {

                if (pictureBox1.Image == null)//if no available bitmap exists on the picturebox to draw on

                {
                    //create a new bitmap
                    Bitmap bmp = new Bitmap(Convert.ToInt32(SystemParameters.VirtualScreenWidth), Convert.ToInt32(SystemParameters.VirtualScreenHeight));

                    pictureBox1.Image = bmp; //assign the picturebox.Image property to the bitmap created

                }

                using (Graphics g = Graphics.FromImage(pictureBox1.Image))

                {//we need to create a Graphics object to draw on the picture box, its our main tool

                    //when making a Pen object, you can just give it color only or give it color and pen size
                    time1 = new StartTime().getStartTime();
                    xDiff = Math.Abs(Cursor.Position.X - lastPoint.X);
                    timeDiff = Convert.ToInt32(time1 - lastTime1);
                    yDiff = Math.Abs(Cursor.Position.Y - lastPoint.Y);
                    vx = (xDiff * 1.0) / timeDiff;
                    vy = (yDiff * 1.0) / timeDiff;
                    v = Math.Sqrt((vx * vx) + (vy * vy));
                    vMag = Convert.ToInt32(v);
                    double r = v / 11;
                    if (v == 0)
                    {
                        g.DrawRectangle(new Pen(Color.LightBlue), Cursor.Position.X, Cursor.Position.Y, 2, 2);
                    }
                    else
                    {
                        //Color color = interpolate(new List<Color>() { Color.Black, Color.Black, Color.FromArgb(0x330000), Color.FromArgb(0x660000), Color.FromArgb(0x660000), Color.FromArgb(0x800000), Color.FromArgb(0x8B0000), Color.FromArgb(0xCD0000), Color.FromArgb(0xEE0000), Color.FromArgb(0xFF0000), Color.FromArgb(0xFE0000), Color.FromArgb(0xFF2B2B),
                        //Color.FromArgb(0xFF3030), Color.FromArgb(0xFF3333), Color.FromArgb(0xFF4040), Color.FromArgb(0xFF6666), Color.FromArgb(0xFF6A6A), Color.FromArgb(0xFF7777), Color.FromArgb(0xFF9090), Color.FromArgb(0xFF9393), Color.FromArgb(0xFF9999), Color.FromArgb(0xFFA9A9), Color.FromArgb(0xFFAAAA), Color.FromArgb(0xFFAEAE), Color.FromArgb(0xFFBBBB), Color.FromArgb(0xFFC1C1), Color.FromArgb(0xFFCCCC), Color.FromArgb(0xFFD5D5), Color.FromArgb(0xFFDDDD)}, r);
                        //Color color = interpolate(new List<Color>() { Color.FromArgb(0x0500ff), Color.FromArgb(0x0400ff), Color.FromArgb(0x0300ff), Color.FromArgb(0x0200ff), Color.FromArgb(0x0100ff), Color.FromArgb(0x0000ff), Color.FromArgb(0x0002ff), Color.FromArgb(0x0012ff), Color.FromArgb(0x0022ff), Color.FromArgb(0x0032ff), Color.FromArgb(0x0044ff), Color.FromArgb(0x0054ff), Color.FromArgb(0x0064ff), Color.FromArgb(0x0074ff), Color.FromArgb(0x0084ff), Color.FromArgb(0x0094ff), Color.FromArgb(0x00a4ff), Color.FromArgb(0x00b4ff), Color.FromArgb(0x00c4ff), Color.FromArgb(0x00d4ff), Color.FromArgb(0x00e4ff), Color.FromArgb(0x00fff4), Color.FromArgb(0x00ffd0), Color.FromArgb(0x00ffa8), Color.FromArgb(0x00ff83), Color.FromArgb(0x00ff5c), Color.FromArgb(0x00ff36), Color.FromArgb(0x00ff10), Color.FromArgb(0x17ff00), Color.FromArgb(0x3eff00), Color.FromArgb(0x65ff00), Color.FromArgb(0x8aff00), Color.FromArgb(0xb0ff00), Color.FromArgb(0xd7ff00), Color.FromArgb(0xfdff00), Color.FromArgb(0xFFfa00), Color.FromArgb(0xFFf000), Color.FromArgb(0xFFe600), Color.FromArgb(0xFFdc00), Color.FromArgb(0xFFd200), Color.FromArgb(0xFFc800), Color.FromArgb(0xFFbe00), Color.FromArgb(0xFFb400), Color.FromArgb(0xFFaa00), Color.FromArgb(0xFFa000), Color.FromArgb(0xFF9600), Color.FromArgb(0xFF8c00), Color.FromArgb(0xFF8200), Color.FromArgb(0xFF7800), Color.FromArgb(0xFF6e00), Color.FromArgb(0xFF6400), Color.FromArgb(0xFF5a00), Color.FromArgb(0xFF5000), Color.FromArgb(0xFF4600), Color.FromArgb(0xFF3c00), Color.FromArgb(0xFF3200), Color.FromArgb(0xFF2800), Color.FromArgb(0xFF1e00), Color.FromArgb(0xFF1400), Color.FromArgb(0xFF0a00), Color.FromArgb(0xFF0000) }, r);
                        Color color = interpolate(new List<Color>() { Color.FromArgb(0x0500ff), Color.FromArgb(0x0000ff), Color.FromArgb(0x00fff4), Color.FromArgb(0xfdff00), Color.FromArgb(0xFF5000), Color.FromArgb(0xFF4600), Color.FromArgb(0xFF0000) }, r);
                        //Color color = Color.Black;
                        g.DrawLine(new Pen(color, 2), lastPoint.X, lastPoint.Y, Cursor.Position.X, Cursor.Position.Y);
                    }
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    //this is to give the drawing a more smoother, less sharper look

                }

                pictureBox1.Invalidate();//refreshes the picturebox
                lastTime1 = time1;
                lastPoint = Cursor.Position;//keep assigning the lastPoint to the current mouse position

            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.DarkGray;
            Hook.GlobalEvents().MouseClick += MouseClickAll;
            long currTime = new StartTime().getStartTime();
            long timeSinceStartSession = currTime - startTime;
            ScreenElement structure = ScreenElement.fromAutomationElement(ElementFromCursor());
            System.IO.File.AppendAllText(@"C:\Saif\Office\C#\Projects\MouseClick\file\" + startTime.ToString() + ".json", "{\"eventType\": \"mm\"" + ", \"mouseX\": " + Cursor.Position.X + ", \"mouseY\": " + Cursor.Position.Y + ", \"time\": " + timeSinceStartSession + ", \"relative\": true" + "}");
        }

        private void MouseClickAll(object sender, MouseEventArgs e)
        {
            POINT p;
            if (GetCursorPos(out p) && clickCheck == 0)           {
                ScreenElement structure = ScreenElement.fromAutomationElement(ElementFromCursor());
                long currTime = new StartTime().getStartTime();
                long timeSinceStartSession = currTime - startTime;
                if (double.IsInfinity(structure.x) || double.IsInfinity(structure.y))
                {
                    using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                    {
                        g.DrawEllipse(new Pen(Color.Green), p.X, p.Y, 4, 4);
                    }
                    System.IO.File.AppendAllText(@"C:\Saif\Office\C#\Projects\MouseClick\file\" + startTime.ToString() + ".json", "\n{\"eventType\": \"md\"" + ", \"mouseX\": " + p.X + ", \"mouseY\": " + p.Y + ", \"elementX\": " + structureInCondition.x + ", \"elementY\": " + structureInCondition.y + ", \"elementW\": " + structureInCondition.width + ", \"elementH\": " + structureInCondition.height + ", \"time\": " + timeSinceStartSession + ", \"relative\": true" + "}");
                }
                else
                {
                    using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                    {
                        g.DrawEllipse(new Pen(Color.Green), p.X, p.Y, 4, 4);
                    }
                    System.IO.File.AppendAllText(@"C:\Saif\Office\C#\Projects\MouseClick\file\" + startTime.ToString() + ".json", "\n{\"eventType\": \"mc\"" + ", \"mouseX\": " + p.X + ", \"mouseY\": " + p.Y + ", \"elementX\": " + structure.x + ", \"elementY\": " + structure.y + ", \"elementW\": " + structure.width + ", \"elementH\": " + structure.height + ", \"time\": " + timeSinceStartSession + ", \"relative\": true" + "}");
                }
            }
        }
        private AutomationElement ElementFromCursor()
        {
            try
            {
                System.Windows.Point point = new System.Windows.Point(Cursor.Position.X, Cursor.Position.Y);
                AutomationElement element = AutomationElement.FromPoint(point);
                publicElement = element;
                return element;
            }
            catch
            {
                return publicElement;
            }
        }

        public class ScreenElement
        {
            public string classname;
            public string type;
            public string id;
            public bool isControl;
            public double x;
            public double y;
            public double width;
            public double height;
            public ScreenElement parent;

            public static ScreenElement fromAutomationElement(AutomationElement element, CacheRequest cacheRequest = null)
            {
                if (element == null) return null;

                try
                {
                    bool cacheRequestMine = cacheRequest == null;
                    if (cacheRequestMine)
                    {
                        cacheRequest = new CacheRequest();
                        cacheRequest.Add(AutomationElement.ClassNameProperty);
                        cacheRequest.Add(AutomationElement.ControlTypeProperty);
                        cacheRequest.Add(AutomationElement.AutomationIdProperty);
                        cacheRequest.Add(AutomationElement.IsControlElementProperty);
                        cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
                        cacheRequest.AutomationElementMode = AutomationElementMode.Full;
                        cacheRequest.TreeFilter = Automation.RawViewCondition;
                        cacheRequest.Push();
                        element = element.GetUpdatedCache(cacheRequest);
                    }

                    var p = element.Cached;
                    var b = p.BoundingRectangle;
                  ScreenElement parent;
                    try
                    {
                        parent = ScreenElement.fromAutomationElement(TreeWalker.RawViewWalker.GetParent(element, cacheRequest), cacheRequest);
                    }
                    catch (Exception)
                    {
                       parent = null;
                    }
                    var r = new ScreenElement()
                    {
                        classname = p.ClassName,
                        type = p.ControlType.ProgrammaticName,
                        id = p.AutomationId,
                        isControl = p.IsControlElement,
                        x = b.X,
                        y = b.Y,
                        width = b.Width,
                        height = b.Height,
                        // parent = parent,
                    };
                    if (cacheRequestMine)
                    {
                        cacheRequest.Pop();
                    }
                    return r;
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

        private int x = 0;
        private int y = 0;

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;

            this.Invalidate();
        }

    }
}