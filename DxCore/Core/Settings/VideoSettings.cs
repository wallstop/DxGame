﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DxCore.Core.Settings
{
    [Serializable]
    [DataContract]
    public class VideoSettings
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
                        break;
                    }
                    case WindowMode.Fullscreen:
                    {
                        DxGame.Instance.Window.IsBorderless = false;
                        DxGame.Instance.Graphics.IsFullScreen = true;
                        break;
                    }
                    case WindowMode.Windowed:
                    {
                        DxGame.Instance.Window.IsBorderless = false;
                        DxGame.Instance.Graphics.IsFullScreen = false;
                        break;
                    }
                    default:
                    {
                        throw new InvalidEnumArgumentException($"Unknown {typeof(WindowMode)}: {value}");
                    }
                }
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

        private static void LogPropertyNoOp<T>(string propertyName, T value)
        {
            Logger.Debug("Ignoring no-op setting of {0} ({1})", propertyName, value);
        }

        private static void LogPropertyChange<T>(string propertyName, T newValue, T previous)
        {
            Logger.Info("Changing {0} to {1} from {2}", propertyName, newValue, previous);
        }

        public void HandlePreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs eventArgs)
        {
            /*
                If Monogame ever implements proper listeners on this, modify the incoming event args here.
                Otherwise, we have to rely on the properties above.
            */
            displayModes_ = eventArgs.GraphicsDeviceInformation.Adapter.SupportedDisplayModes.ToList();
            Logger.Info("Found supported display modes: {0}", string.Join(",", displayModes_.Select(_ => _.ToString())));
        }
    }
}
