﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DXGame.Core.Utils;
using MapEditor.Utils;
using MetroFramework.Forms;
using NLog;

namespace MapEditor.UI
{
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
        private Bitmap baseImage_;
        private Point currentOffset_ = new Point(0, 0);
        private Point dragBegin_;
        private Point dragEnd_;
        private DateTime lastDrawn_;
        private Panel mapArea_;
        private double zoomFactor_ = DEFAULT_ZOOM;

        private Rectangle ImageView
            =>
                new Rectangle(0, 0, Size.Width - (2 * MAIN_PANEL_BORDER),
                    Size.Height - (TITLE_HEIGHT + MENU_HEIGHT + MAIN_PANEL_BORDER));

        private Size ImageSize => new Size(ImageView.Width, ImageView.Height);

        private Point Offset
        {
            get
            {
                if (Check.IsNotNullOrDefault(dragBegin_) && Check.IsNotNullOrDefault(dragEnd_))
                {
                    return new Point(currentOffset_.X + (dragEnd_.X - dragBegin_.X),
                        currentOffset_.Y + (dragEnd_.Y - dragBegin_.Y));
                }
                return currentOffset_;
            }
            set { currentOffset_ = value; }
        }

        private double ZoomFactor
        {
            get { return zoomFactor_; }
            set { zoomFactor_ = Math.Max(MINIMUM_ZOOM, value); }
        }

        public MapEditor()
        {
            Size = DEFAULT_SIZE;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            InitializeComponent();
            Application.AddMessageFilter(this);
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
            contextMenu.MenuItems.Add(snapToCurrent);

            mapArea_.ContextMenu = contextMenu;

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
        }

        private void HandleHardReset(object sender, EventArgs args)
        {
            currentOffset_ = new Point(0, 0);
            dragBegin_ = new Point();
            dragEnd_ = new Point();
            ZoomFactor = DEFAULT_ZOOM;
            Redraw(true);
        }

        private void HandleDragBegin(object sender, MouseEventArgs mouseEvent)
        {
            /* Only pan on middle mouse */
            if (mouseEvent.Button == MouseButtons.Middle)
            {
                dragBegin_ = mouseEvent.Location;
            }
        }

        private void HandleMouseWheel(object sender, MouseEventArgs mouseEvent)
        {
            ZoomFactor -= ZOOM_SCALE * mouseEvent.Delta;
            Redraw();
        }

        private void HandleDragEvent(object sender, MouseEventArgs mouseEvent)
        {
            if (Check.IsNotNullOrDefault(dragBegin_))
            {
                dragEnd_ = new Point(mouseEvent.X, mouseEvent.Y);
                Redraw();
            }
        }

        private void HandleDragEnd(object sender, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.Button == MouseButtons.Middle)
            {
                Offset = Offset;
                dragBegin_ = new Point();
                dragEnd_ = new Point();
                Redraw();
            }
        }

        private void PaintMapArea(object sender, PaintEventArgs eventArgs)
        {
            var graphics = eventArgs.Graphics;
            mapArea_.Size = ImageSize;
            graphics.DrawImage(baseImage_, ImageView.Add(Offset), ImageView.Multiply(ZoomFactor), GraphicsUnit.Pixel);
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