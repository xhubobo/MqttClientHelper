using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace RecvMessageSample
{
    public partial class DrawForm : Form
    {
        //UI线程的同步上下文
        private readonly SynchronizationContext _syncContext;

        private SKControl _skiaView;
        private SKBitmap _skBitmap;

        private int _recvNumber;
        private string _imageFolder;

        private int _value = -1;
        private static readonly object ValueLockHelper = new object();

        private int Value
        {
            get
            {
                lock (ValueLockHelper)
                {
                    return _value;
                }
            }
            set
            {
                lock (ValueLockHelper)
                {
                    _value = value;
                }
            }
        }

        public DrawForm()
        {
            InitializeComponent();

            _syncContext = SynchronizationContext.Current;
            _imageFolder = Environment.CurrentDirectory + "/Images";
        }

        private void DrawForm_Load(object sender, System.EventArgs e)
        {
            DrawForm_SizeChanged(this, e);

            //skiaView
            _skiaView = new SKControl();
            _skiaView.Dock = DockStyle.Fill;
            _skiaView.Location = new Point(0, 0);
            //_skiaView.Margin = new Padding(48, 24, 48, 24);
            _skiaView.Name = "skiaView";
            //_skiaView.Size = new Size(784, 561);
            _skiaView.TabIndex = 0;
            _skiaView.Text = "skControl1";
            _skiaView.PaintSurface += skiaView_PaintSurface;
            panelDrawArea.Controls.Add(_skiaView);
        }

        private void DrawForm_SizeChanged(object sender, System.EventArgs e)
        {
            panelDrawArea.Left = 0;
            panelDrawArea.Width = ClientSize.Width;
            panelDrawArea.Height = (int)(ClientSize.Width / 16f * 9);
            panelDrawArea.Top = ClientSize.Height - panelDrawArea.Height;
        }

        private void DrawForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        public void SetValue(int value)
        {
            _syncContext.Post(SetValueSafePost, value);
        }

        private void SetValueSafePost(object state)
        {
            Value = (int) state;
            labelDisplay.Text = Value.ToString();
            _skiaView.Refresh();
        }

        private void skiaView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            //载入底图
            _skBitmap = SKBitmap.Decode(
                Value < 0 ? $"{_imageFolder}/test.jpg" : $"_imageFolder/{Value % 5}.jpg");

            // the the canvas and properties
            var canvas = e.Surface.Canvas;

            var watch = Stopwatch.StartNew();
            watch.Start();
            DrawSkia(canvas, e.Info, _skBitmap, labelDisplay.Text);
            watch.Stop();
            Console.WriteLine($"DrawSkia costs {watch.ElapsedMilliseconds} ms.");

            if (Value >= 0)
            {
                labelRecvNumber.Text = (++_recvNumber).ToString();
            }
        }

        private static void DrawSkia(SKCanvas canvas, SKImageInfo info, SKBitmap skBitmap, string text)
        {
            // get the screen density for scaling
            var scale = 1f;
            var scaledSize = new SKSize(info.Width / scale, info.Height / scale);

            // handle the device screen density
            canvas.Scale(scale);

            // make sure the canvas is blank
            canvas.Clear(SKColors.Pink);

            var scaleBg = skBitmap.Width * 1f / skBitmap.Height;
            var scalePanel = 16 * 1f / 9;
            var left = 0f;
            var top = 0f;
            var size = new SKSize(0, 0);
            if (scaleBg > scalePanel)
            {
                size.Height = scaledSize.Height;
                size.Width = size.Height * scaleBg;
                left = (scaledSize.Width - size.Width) / 2;
            }
            else
            {
                //size.Width = scaledSize.Width;
                //size.Height = size.Width / scaleBg;
                //top = (scaledSize.Height - size.Height) / 2;
                size.Height = scaledSize.Height;
                size.Width = size.Height * scaleBg;
                left = (scaledSize.Width - size.Width) / 2;
            }

            canvas.DrawBitmap(skBitmap, new SKRect(left, top,
                left + size.Width, top + size.Height));

            // draw some text
            var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                TextSize = 50
            };
            var widths = paint.GetGlyphWidths(text);
            var total = widths.Sum() / 2;
            //var coord = new SKPoint(scaledSize.Width / 2, (scaledSize.Height + paint.TextSize) / 2);
            var coord = new SKPoint(left + 20 + total, 20 + paint.TextSize);
            canvas.DrawText(text, coord, paint);
        }
    }
}
