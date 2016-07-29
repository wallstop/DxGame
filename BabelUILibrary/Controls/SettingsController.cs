using System.Collections.Generic;
using System.Linq;
using DxCore;
using EmptyKeys.UserInterface.Mvvm;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace BabelUILibrary.Controls
{
    public sealed class SettingsController : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public List<string> AvailableResolutions
        {
            get
            {
                // TODO: Move to settings service D:
                return
                    DxGame.Instance.GameSettings.VideoSettings.DisplayModes.Select(
                        displayMode => displayMode.Width + "x" + displayMode.Height).ToList();
            }
        }

        private int selectedResolutionIndex_;

        public int SelectedResolutionIndex
        {
            get { return selectedResolutionIndex_; }
            set
            {
                Logger.Info("Resolution set to index {0}", value);
                if(selectedResolutionIndex_ != value)
                {
                    DisplayMode selectedDisplayMode = DxGame.Instance.GameSettings.VideoSettings.DisplayModes.ToList()[value];
                    DxGame.Instance.GameSettings.VideoSettings.ScreenHeight = selectedDisplayMode.Height;
                    DxGame.Instance.GameSettings.VideoSettings.ScreenWidth = selectedDisplayMode.Width;
                }
                selectedResolutionIndex_ = value;
                Logger.Info(string.Join(",", AvailableResolutions));
            }
        }

        private void AvailableResolutionsChanged()
        {
            List<string> doesntMatter = new List<string> {"plsno"};
            SetProperty(ref doesntMatter, AvailableResolutions, nameof(AvailableResolutions));
        }

        public SettingsController()
        {
            DxGame.Instance.GameSettings.VideoSettings.RegisterPropertyChangeListener(AvailableResolutionsChanged);
        }
    }
}
