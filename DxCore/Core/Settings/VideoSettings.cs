using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DxCore.Core.Settings
{
    [Serializable]
    [DataContract]
    public sealed class VideoSettings
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static VideoSettings DefaultVideoSettings
            =>
                new VideoSettings
                {
                    ScreenHeight = 720,
                    ScreenWidth = 1280,
                    WindowMode = WindowMode.Windowed,
                    ImmediateMode = true
                };

        // TODO: We need a way of pub/sub listeners for properties

        [IgnoreDataMember]
        private List<WeakReference<Action>> SettingsUpdatedListeners { get; set; }

        [IgnoreDataMember]
        public DxVector2 ScreenDimensions => new DxVector2(ScreenWidth, ScreenHeight);

        [IgnoreDataMember] private int screenHeight_;

        [DataMember]
        public int ScreenHeight
        {
            get { return screenHeight_; }
            set
            {
                if(screenHeight_ == value)
                {
                    LogPropertyNoOp(nameof(ScreenHeight), value);
                    return;
                }
                LogPropertyChange(nameof(ScreenHeight), value, screenHeight_);
                screenHeight_ = value;

                DxGame.Instance.Graphics.PreferredBackBufferHeight = screenHeight_;
                DxGame.Instance.Graphics.ApplyChanges();
            }
        }

        [IgnoreDataMember] private int screenWidth_;

        [DataMember]
        public int ScreenWidth
        {
            get { return screenWidth_; }
            set
            {
                if(screenWidth_ == value)
                {
                    LogPropertyNoOp(nameof(ScreenWidth), value);
                    return;
                }
                LogPropertyChange(nameof(ScreenWidth), value, screenWidth_);
                screenWidth_ = value;
                DxGame.Instance.Graphics.PreferredBackBufferWidth = screenWidth_;
                DxGame.Instance.Graphics.ApplyChanges();
            }
        }

        [IgnoreDataMember] private WindowMode windowMode_;

        [DataMember]
        public WindowMode WindowMode
        {
            get { return windowMode_; }
            set
            {
                if(windowMode_ == value)
                {
                    LogPropertyNoOp(nameof(WindowMode), value);
                    return;
                }
                LogPropertyChange(nameof(WindowMode), value, windowMode_);
                windowMode_ = value;
                switch(windowMode_)
                {
                    case WindowMode.Borderless:
                    {
                        DxGame.Instance.Window.IsBorderless = true;
                        DxGame.Instance.Graphics.IsFullScreen = false;
                        if (DxGame.Instance.GraphicsDevice != null)
                        {
                            DxGame.Instance.Graphics.PreferredBackBufferWidth = DxGame.Instance.GraphicsDevice.DisplayMode.Width;
                            DxGame.Instance.Graphics.PreferredBackBufferHeight = DxGame.Instance.GraphicsDevice.DisplayMode.Height;
                        }
                        DxGame.Instance.Graphics.ApplyChanges();
                        DxGame.Instance.Window.Position = Point.Zero;
                        break;
                    }
                    case WindowMode.Fullscreen:
                    {
                        DxGame.Instance.Window.IsBorderless = false;
                        DxGame.Instance.Graphics.IsFullScreen = true;
                        DxGame.Instance.Graphics.ApplyChanges();
                        break;
                    }
                    case WindowMode.Windowed:
                    {
                        DxGame.Instance.Window.IsBorderless = false;
                        DxGame.Instance.Graphics.IsFullScreen = false;
                        Point position = DxGame.Instance.Window.Position;
                        if (DxGame.Instance.GraphicsDevice != null)
                        {
                            //Intended to "center" the window after reverting from fullscreen or borderless, but unfortunately doesn't take the Windows
                            //menu bar into account so the height will be x pixels too low where x is the menu bar height.
                            position.X = (DxGame.Instance.GraphicsDevice.DisplayMode.Width - ScreenWidth) / 2;
                            position.Y = (DxGame.Instance.GraphicsDevice.DisplayMode.Height - ScreenHeight) / 2;
                            DxGame.Instance.Graphics.PreferredBackBufferWidth = ScreenWidth;
                            DxGame.Instance.Graphics.PreferredBackBufferHeight = ScreenHeight;
                        }
                        DxGame.Instance.Graphics.ApplyChanges();
                        DxGame.Instance.Window.Position = position;
                        break;
                    }
                    default:
                    {
                        throw new InvalidEnumArgumentException($"Unknown {typeof(WindowMode)}: {value}");
                    }
                }
                //DxGame.Instance.Graphics.ApplyChanges();
                LogPropertyNoOp(nameof(DxGame.Instance.Window.Position), DxGame.Instance.Window.Position);
            }
        }

        // TODO: Care
        [DataMember]
        public Amount Particles { get; set; }

        // TODO: Care
        [DataMember]
        public Amount Shadows { get; set; }

        // TODO: Care
        [DataMember]
        public Amount Blood { get; set; }

        [IgnoreDataMember] private bool immediateMode_;

        [DataMember]
        public bool ImmediateMode
        {
            get { return immediateMode_; }
            set
            {
                if(immediateMode_ == value)
                {
                    LogPropertyNoOp(nameof(ImmediateMode), value);
                    return;
                }
                LogPropertyChange(nameof(ImmediateMode), value, immediateMode_);
                immediateMode_ = value;
                if(immediateMode_)
                {
                    DxGame.Instance.Graphics.SynchronizeWithVerticalRetrace = false;
                }
                else
                {
                    DxGame.Instance.Graphics.SynchronizeWithVerticalRetrace = true;
                }
                DxGame.Instance.Graphics.ApplyChanges();
            }
        }

        [IgnoreDataMember]
        public bool VSyncEnabled
        {
            get { return !ImmediateMode; }
            set { ImmediateMode = !value; }
        }

        [IgnoreDataMember] private List<DisplayMode> displayModes_ = new List<DisplayMode>();

        [IgnoreDataMember]
        public IEnumerable<DisplayMode> DisplayModes => displayModes_.ToList();

        public VideoSettings()
        {
            Initialize();
        }

        public void RegisterPropertyChangeListener(Action listener)
        {
            Validate.Hard.IsNotNull(listener);
            SettingsUpdatedListeners.Add(new WeakReference<Action>(listener, true));
        }

        private void LogPropertyNoOp<T>(string propertyName, T value)
        {
            Logger.Debug("Ignoring no-op setting of {0} ({1})", propertyName, value);
        }

        private void LogPropertyChange<T>(string propertyName, T newValue, T previous)
        {
            Logger.Info("Changing {0} to {1} from {2}", propertyName, newValue, previous);
            NotifyListenersOfPropertyChange();
        }

        public void HandleDeviceCreated(object sender, EventArgs eventArgs)
        {
            if (windowMode_ == WindowMode.Borderless)
            {
                DxGame.Instance.Graphics.PreferredBackBufferWidth = DxGame.Instance.GraphicsDevice.DisplayMode.Width;
                DxGame.Instance.Graphics.PreferredBackBufferHeight = DxGame.Instance.GraphicsDevice.DisplayMode.Height;
                DxGame.Instance.Graphics.ApplyChanges();
                DxGame.Instance.Window.Position = Point.Zero;
            }
        }

        public void HandlePreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs eventArgs)
        {
            /*
                If Monogame ever implements proper listeners on this, modify the incoming event args here.
                Otherwise, we have to rely on the properties above.
            */
            displayModes_ = eventArgs.GraphicsDeviceInformation.Adapter.SupportedDisplayModes.ToList();
            /* TODO: Inject initialization of properties here */
            Logger.Info("Found supported display modes: {0}", string.Join(",", displayModes_.Select(_ => _.ToString())));
            NotifyListenersOfPropertyChange();
        }

        private void NotifyListenersOfPropertyChange()
        {
            foreach(WeakReference<Action> maybeListener in SettingsUpdatedListeners.ToArray())
            {
                Action listener;
                if(maybeListener.TryGetTarget(out listener))
                {
                    listener();
                }
                else
                {
                    SettingsUpdatedListeners.Remove(maybeListener);
                }
            }
        }

        [OnDeserializing]
        private void OnDeserialize(StreamingContext context)
        {
            Initialize();
        }

        private void Initialize()
        {
            SettingsUpdatedListeners = new List<WeakReference<Action>>();
        }
    }
}
