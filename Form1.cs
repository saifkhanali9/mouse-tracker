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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string[] list1 = new string[4];
        long startTime = new StartTime().getStartTime();
        AutomationElement publicElement;
        ScreenElement structureInCondition;
        public Form1()
        {
            InitializeComponent();
            var timer = new Timer();
            timer.Interval = 16;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            ScreenElement structure = ScreenElement.fromAutomationElement(ElementFromCursor());
            structureInCondition = structure;
            elementWLabel.Text = structure.width.ToString();
            elementHLabel.Text = structure.height.ToString();
            elementXLabel.Text = structure.x.ToString();
            elementYLabel.Text = structure.y.ToString();
            mmXVal.Text = Cursor.Position.X.ToString();
            mmYVal.Text = Cursor.Position.Y.ToString();
            long currTime = new StartTime().getStartTime();
            long timeSinceStartSession = currTime - startTime;
            timeVal.Text = timeSinceStartSession.ToString();
            System.IO.File.AppendAllText(@"C:\Saif\Office\C#\Projects\MouseClick\file\" + startTime.ToString() + ".json", "\n{\"eventType\": \"mm\"" + ", \"mouseX\": " + Cursor.Position.X + ", \"mouseY\": " + Cursor.Position.Y+ ", \"time\": " + timeSinceStartSession + "}");
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
            Hook.GlobalEvents().MouseClick += MouseClickAll;
            long currTime = new StartTime().getStartTime();
            long timeSinceStartSession = currTime - startTime;
            ScreenElement structure = ScreenElement.fromAutomationElement(ElementFromCursor());
            System.IO.File.AppendAllText(@"C:\Saif\Office\C#\Projects\MouseClick\file\" + startTime.ToString() + ".json", "{\"eventType\": \"mm\"" + ", \"mouseX\": " + Cursor.Position.X + ", \"mouseY\": " + Cursor.Position.Y + ", \"time\": " + timeSinceStartSession + "}");
        }

        private void MouseClickAll(object sender, MouseEventArgs e)
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                ScreenElement structure = ScreenElement.fromAutomationElement(ElementFromCursor());
                elementWLabel.Text = structure.width.ToString();
                elementHLabel.Text = structure.height.ToString();
                elementXLabel.Text = structure.x.ToString();
                elementYLabel.Text = structure.y.ToString();
                mcOutXVal.Text = Convert.ToString(p.X);
                mcOutYVal.Text = Convert.ToString(p.Y);
                long currTime = new StartTime().getStartTime();
                long timeSinceStartSession = currTime - startTime;
                if (double.IsInfinity(structure.x) || double.IsInfinity(structure.y))
                {
                    System.IO.File.AppendAllText(@"C:\Saif\Office\C#\Projects\MouseClick\file\" + startTime.ToString() + ".json", "\n{\"eventType\": \"mc\"" + ", \"mouseX\": " + p.X + ", \"mouseY\": " + p.Y + ", \"elementX\": " + structureInCondition.x + ", \"elementY\": " + structureInCondition.y + ", \"elementW\": " + structureInCondition.width + ", \"elementH\": " + structureInCondition.height + ", \"time\": " + timeSinceStartSession + "}");
                }
                else
                {
                    System.IO.File.AppendAllText(@"C:\Saif\Office\C#\Projects\MouseClick\file\" + startTime.ToString() + ".json", "\n{\"eventType\": \"mc\"" + ", \"mouseX\": " + p.X + ", \"mouseY\": " + p.Y + ", \"elementX\": " + structure.x + ", \"elementY\": " + structure.y + ", \"elementW\": " + structure.width + ", \"elementH\": " + structure.height + ", \"time\": " + timeSinceStartSession + "}");
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
        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            double xVal = e.X;
            double yVal = e.Y;
            mmXButtonVal.Text = xVal.ToString();
            mmYButtonVal.Text = yVal.ToString();
            long currTime = new StartTime().getStartTime();
            long timeSinceStartSession = currTime - startTime;
            timeButtonVal.Text = timeSinceStartSession.ToString();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Keep Aruba Happy","No... " +
                "I won't change it!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
