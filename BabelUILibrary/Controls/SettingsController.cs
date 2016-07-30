﻿using System;
using System.Collections.Generic;
using System.Linq;
using BabelUILibrary.Core;
using DxCore;
using DxCore.Core.Settings;
using DxCore.Core.Utils.Validate;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;
using NLog;

namespace BabelUILibrary.Controls
{
    public sealed class SettingsController : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool Enabled
        {
            get { return true; }
            set { ChangeSettingsVisibility(value); }
        }

        public bool VSync
        {
            get { return DxGame.Instance.GameSettings.VideoSettings.VSyncEnabled; }
            set { DxGame.Instance.GameSettings.VideoSettings.VSyncEnabled = value; }
        }

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        public List<string> AvailableResolutions
        {
            get { return UniqueResolutions.Select(_ => _.ToString()).ToList(); }
        }

        private List<Resolution> UniqueResolutions
        {
            get
            {
                // TODO: Move to settings service D:
                return
                    DxGame.Instance.GameSettings.VideoSettings.DisplayModes.Select(
                        displayMode => new Resolution {Height = displayMode.Height, Width = displayMode.Width})
                        .Distinct()
                        .ToList();
            }
        }

        private int selectedResolutionIndex_;

        public int SelectedResolutionIndex
        {
            get { return selectedResolutionIndex_; }
            set
            {
                Logger.Info("Resolution set to index {0} from {1}", value, selectedResolutionIndex_);
                if(selectedResolutionIndex_ != value)
                {
                    Resolution selectedResolution = UniqueResolutions[value];
                    DxGame.Instance.GameSettings.VideoSettings.ScreenHeight = selectedResolution.Height;
                    DxGame.Instance.GameSettings.VideoSettings.ScreenWidth = selectedResolution.Width;
                }
                selectedResolutionIndex_ = value;
            }
        }

        public List<string> AvailableWindowModes
        {
            get { return UniqueWindowModes.Select(_ => _.ToString()).ToList(); }
        }

        private List<WindowMode> UniqueWindowModes => ((WindowMode[]) Enum.GetValues(typeof(WindowMode))).ToList();

        private int selectedWindowModeIndex_;

        public int SelectedWindowModeIndex
        {
            get { return selectedWindowModeIndex_; }
            set
            {
                Logger.Info("WindowMode set to index {0} from {1}", value, selectedWindowModeIndex_);
                if(selectedWindowModeIndex_ != value)
                {
                    WindowMode selectedWindowMode = UniqueWindowModes[value];
                    DxGame.Instance.GameSettings.VideoSettings.WindowMode = selectedWindowMode;
                }
                selectedWindowModeIndex_ = value;
            }
        }

        private Action<bool> ChangeSettingsVisibility { get; }

        public SettingsController(Action<bool> settingsVisibilityTrigger)
        {
            Validate.Hard.IsNotNull(settingsVisibilityTrigger);
            ChangeSettingsVisibility = settingsVisibilityTrigger;
            SaveCommand = new RelayCommand(OnSave);
            BackCommand = new RelayCommand(OnBack);

            FindCurrentResolution();
            FindCurrentWindowMode();
        }

        private void FindCurrentWindowMode()
        {
            for(int i = 0; i < UniqueWindowModes.Count; ++i)
            {
                WindowMode windowMode = UniqueWindowModes[i];
                if(windowMode == DxGame.Instance.GameSettings.VideoSettings.WindowMode)
                {
                    selectedWindowModeIndex_ = i;
                    break;
                }
            }
        }

        private void FindCurrentResolution()
        {
            for(int i = 0; i < UniqueResolutions.Count; ++i)
            {
                Resolution resolution = UniqueResolutions[i];
                if(resolution.Height == DxGame.Instance.GameSettings.VideoSettings.ScreenHeight &&
                   resolution.Width == DxGame.Instance.GameSettings.VideoSettings.ScreenWidth)
                {
                    selectedResolutionIndex_ = i;
                    break;
                }
            }
        }

        private void OnSave(object context)
        {
            DxGame.Instance.GameSettings.Save();
        }

        private void OnBack(object context)
        {
            Enabled = false;
        }
    }
}
