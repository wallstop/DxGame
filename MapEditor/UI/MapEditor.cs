using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using MapEditor.Core;
using MapEditor.Utils;
using MetroFramework.Forms;
using NLog;

namespace MapEditor.UI
{
    internal enum RectangleMode
    {
        None,
        Pan,
        Create,
        Delete
    }

    public class MapEditor : MetroForm, IMessageFilter
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        private static readonly TimeSpan DRAW_INTERVAL = TimeSpan.FromMilliseconds(16);
        private static readonly string PLACEHOLDER_DIRECTORY = "Placeholders";
        private static readonly double MINIMUM_ZOOM = .25;
        private static readonly double ZOOM_SCALE = 0.001;
        private static readonly Size DEFAULT_SIZE = new Size(1280, 720);
        private static readonly int TITLE_HEIGHT = 60;
        private static readonly int MENU_HEIGHT = 24;
        private static readonly int MAIN_PANEL_BORDER = 20;
        private static readonly double DEFAULT_ZOOM = 1.0;

        private static readonly Dictionary<MouseButtons, RectangleMode> MODE_FOR_BUTTON = new Dictionary
            <MouseButtons, RectangleMode>
        {
            {
                MouseButtons.Left, RectangleMode.Create
            },
            {
                MouseButtons.Middle, RectangleMode.Pan
            },
            {
                MouseButtons.Right, RectangleMode.Delete
            }
        };

        private readonly BoundingBoxEditor boundingBoxEditor_;
        private Bitmap baseImage_;
        private DateTime lastDrawn_;
        private Panel mapArea_;
        private RectangleMode mode_ = RectangleMode.None;
        private Point pointBegin_;
        private Point pointEnd_;
        private double zoomFactor_ = DEFAULT_ZOOM;

        private Rectangle ImageView
            =>
                new Rectangle(0, 0, Size.Width - (2 * MAIN_PANEL_BORDER),
                    Size.Height - (TITLE_HEIGHT + MENU_HEIGHT + MAIN_PANEL_BORDER));

        private Size ImageSize => new Size(ImageView.Width, ImageView.Height);
        private Point Offset { get; set; }

        private Point Translation
        {
            get
            {
                var translation = Offset;
                if (mode_ == RectangleMode.Pan)
                {
                    translation = translation.Add(pointBegin_.Subtract(pointEnd_));
                }
                return translation;
            }
        }

        private double ZoomFactor
        {
            get { return zoomFactor_; }
            set { zoomFactor_ = Math.Max(MINIMUM_ZOOM, value); }
        }

        private DxRectangle SelectedArea
        {
            get
            {
                var translatedOriginPoint = new Point(Offset.X + pointBegin_.X, Offset.Y + pointBegin_.Y);
                var translatedEndPoint = new Point(Offset.X + pointEnd_.X, Offset.Y + pointEnd_.Y);
                DxRectangle area = new DxRectangle(translatedOriginPoint.X, translatedOriginPoint.Y,
                    translatedEndPoint.X - translatedOriginPoint.X, translatedEndPoint.Y - translatedOriginPoint.Y);
                if (area.Width < 0)
                {
                    area.X += area.Width;
                    area.Width = -area.Width;
                }
                if (area.Height < 0)
                {
                    area.Y += area.Height;
                    area.Height = -area.Height;
                }
                return area;
            }
        }

        private DxRectangle TranslatedSelectedArea
        {
            get
            {
                var area = new DxRectangle((float)((SelectedArea.X  - 2 * Translation.X) / ZoomFactor), (float)((SelectedArea.Y  - 2 * Translation.Y) / ZoomFactor), (float)(SelectedArea.Width / ZoomFactor),
                    (float)(SelectedArea.Height / ZoomFactor));
                return area;
            }
        }

        public MapEditor()
        {
            Size = DEFAULT_SIZE;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            InitializeComponent();
            Application.AddMessageFilter(this);
            boundingBoxEditor_ = new BoundingBoxEditor();
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x20a)
            {
                // WM_MOUSEWHEEL, find the control at screen position m.LParam
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                IntPtr hWnd = WindowFromPoint(pos);
                if (hWnd != IntPtr.Zero && hWnd != m.HWnd && FromHandle(hWnd) != null)
                {
                    SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }
            return false;
        }

        private void UpdateOffset()
        {
            Offset = new Point(Offset.X + (pointEnd_.X - pointBegin_.X),
                Offset.Y + (pointEnd_.Y - pointBegin_.Y));
        }

        // P/Invoke declarations
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        private void InitializeComponent()
        {
            /* Pause while we set stuff up */
            SuspendLayout();

            SetupMainMenu();
            SetupMapArea();

            ResumeLayout(false);
            PerformLayout();

            DoubleBuffered = true;
        }

        private void SetupMainMenu()
        {
            Text = "MapEditor";

            var mainMenu = new MenuStrip {ShowItemToolTips = true};
            var file = new ToolStripMenuItem();
            var reset = new ToolStripMenuItem();
            var newMap = new ToolStripMenuItem();
            var save = new ToolStripMenuItem();
            var load = new ToolStripMenuItem();

            mainMenu.Items.Add(file);
            mainMenu.Items.Add(reset);
            mainMenu.Location = new Point(MAIN_PANEL_BORDER, TITLE_HEIGHT);
            mainMenu.Size = new Size(260, MENU_HEIGHT);

            var subMenuSize = new Size(152, 22);

            file.DropDownItems.AddRange(new ToolStripItem[] {newMap, save, load});
            file.Size = new Size(37, 20);
            file.Text = "File";

            var hardReset = new ToolStripMenuItem();
            var softReset = new ToolStripMenuItem();
            reset.DropDownItems.AddRange(new ToolStripItem[] {softReset, hardReset});
            reset.Size = new Size(37, 20);
            reset.Text = "Reset";

            softReset.Size = subMenuSize;
            softReset.Text = "Soft Reset";
            softReset.ToolTipText =
                "TODO: Clears preferences to the last-snapped set, or default if no preferences were snapped";
            hardReset.Size = subMenuSize;
            hardReset.Text = "Hard Reset";
            hardReset.ToolTipText =
                "Resets everything to be as if you had freshly started the Map \nEditor with this image loaded. Clears out snapped preferences.";
            hardReset.Click += HandleHardReset;

            save.Text = "Save Map";
            save.ToolTipText = "TODO";
            save.Size = subMenuSize;
            save.Click += HandleSave;

            newMap.Text = "New Map";
            newMap.Size = subMenuSize;
            newMap.Click += HandleNewMap;

            load.Text = "Load Map";
            load.ToolTipText = "TODO";
            load.Size = subMenuSize;
            //load.Click += HandleNewMap;

            Controls.Add(mainMenu);
            MainMenuStrip = mainMenu;

            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
        }

        private void SetupMapArea()
        {
            mapArea_ = new DoubleBufferedPanel
            {
                Location = new Point(MAIN_PANEL_BORDER, TITLE_HEIGHT + MENU_HEIGHT)
            };

            var contextMenu = new ContextMenu();
            var snapToCurrent = new MenuItem("Snap");
            //contextMenu.MenuItems.Add(snapToCurrent);

            //mapArea_.ContextMenu = contextMenu;

            baseImage_ = new Bitmap(PLACEHOLDER_DIRECTORY + "/Map.png");
            mapArea_.Paint += PaintMapArea;
            mapArea_.MouseDown += HandleDragBegin;
            mapArea_.MouseWheel += HandleMouseWheel;
            mapArea_.MouseMove += HandleDragEvent;
            mapArea_.MouseUp += HandleDragEnd;

            Controls.Add(mapArea_);
        }

        private void HandleSave(object sender, EventArgs args)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Image Files(*.png;)|*.png;",
                FilterIndex = 1,
                RestoreDirectory = true,
                CheckPathExists = true
            };

            switch (saveFileDialog.ShowDialog())
            {
                case DialogResult.OK:
                {
                    var outFile = saveFileDialog.FileName;

                    boundingBoxEditor_.Save(outFile, baseImage_);
                }
                    break;
            }
        }

        private void HandleHardReset(object sender, EventArgs args)
        {
            Offset = new Point(0, 0);
            pointBegin_ = new Point();
            pointEnd_ = new Point();
            ZoomFactor = DEFAULT_ZOOM;
            boundingBoxEditor_.Clear();
            Redraw(true);
        }

        private void HandleDragBegin(object sender, MouseEventArgs mouseEvent)
        {
            if (MODE_FOR_BUTTON.ContainsKey(mouseEvent.Button))
            {
                mode_ = MODE_FOR_BUTTON[mouseEvent.Button];
                pointBegin_ = mouseEvent.Location;
            }
        }

        private void HandleMouseWheel(object sender, MouseEventArgs mouseEvent)
        {
            ZoomFactor += ZOOM_SCALE * mouseEvent.Delta;
            // TODO: Rescale all existing bounding boxes to new shit (redraw + resize)
            boundingBoxEditor_.Resize(ZoomFactor);
            Redraw();
        }

        private void HandleDragEvent(object sender, MouseEventArgs mouseEvent)
        {
            if (mode_ != RectangleMode.None)
            {
                pointEnd_ = new Point(mouseEvent.X, mouseEvent.Y);
            }

            Redraw();
        }

        private void HandleDragEnd(object sender, MouseEventArgs mouseEvent)
        {
            pointEnd_ = new Point(mouseEvent.X, mouseEvent.Y);
            switch (mode_)
            {
                case RectangleMode.Create:
                    HandleNewCollisionArea();
                    break;
                case RectangleMode.Delete:
                    HandleDelete();
                    break;
                case RectangleMode.Pan:
                    HandlePanEnd();
                    break;
            }

            pointBegin_ = new Point();
            pointEnd_ = new Point();
            Redraw();
            mode_ = RectangleMode.None;
        }

        private void HandleNewCollisionArea()
        {
            var area = TranslatedSelectedArea;
            boundingBoxEditor_.Add(area);
        }

        private void HandleDelete()
        {
            var area = TranslatedSelectedArea;
            boundingBoxEditor_.RemoveInRange(area);
        }

        private void HandlePanEnd()
        {
            UpdateOffset();
        }

        private void PaintMapArea(object sender, PaintEventArgs eventArgs)
        {
            var graphics = eventArgs.Graphics;
            mapArea_.Size = ImageSize;
            var translation = Translation;
            graphics.DrawImage(baseImage_, ImageView,
                ImageView.Subtract(translation.Multiply(1 / ZoomFactor)).Multiply(1 / ZoomFactor),
                GraphicsUnit.Pixel);
            foreach (var rectangle in boundingBoxEditor_.Collidables)
            {
                graphics.DrawRectangle(Pens.Black,
                    new Rectangle((int) (rectangle.X * ZoomFactor + translation.X),
                        (int) (rectangle.Y * ZoomFactor + translation.Y),
                        (int) (rectangle.Width * ZoomFactor), (int) (rectangle.Height * ZoomFactor)));
            }
            if (mode_ != RectangleMode.Pan && mode_ != RectangleMode.None)
            {
                var selectedArea = SelectedArea;
                graphics.DrawRectangle(Pens.Red, (int) selectedArea.X - translation.X,
                    (int) selectedArea.Y - translation.Y, (int) selectedArea.Width,
                    (int) selectedArea.Height);
            }
        }

        private void Redraw(bool force = false)
        {
            var currentTime = DateTime.UtcNow;
            if (Check.IsNullOrDefault(lastDrawn_) || currentTime - DRAW_INTERVAL >= lastDrawn_ || force)
            {
                lastDrawn_ = currentTime;
                mapArea_.Invalidate();
                mapArea_.Update();
            }
        }

        private void HandleNewMap(object sender, EventArgs args)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp;|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            switch (openFileDialog.ShowDialog())
            {
                case DialogResult.OK:
                {
                    try
                    {
                        var imageName = openFileDialog.FileName;
                        baseImage_?.Dispose();
                        baseImage_ = new Bitmap(imageName);
                        Redraw();
                    }
                    catch (Exception e)
                    {
                        LOG.Error(e, "Failed to load {}", openFileDialog.FileName);
                    }
                }
                    break;
            }
        }
    }
}