using Babel.Models;
using DxCore;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;
using NLog;

namespace BabelUILibrary.Controls
{
    public class RootController : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /* TODO: Switch to enum of all types of menus / screens */
        private bool rootVisible_;

        public ICommand PlayCommand { get; }
        public ICommand QuitCommand { get; }

        public bool RootVisible
        {
            get { return rootVisible_; }
            set { SetProperty(ref rootVisible_, value); }
        }

        public ICommand SettingsCommand { get; }

        public RootController()
        {
            PlayCommand = new RelayCommand(OnPlay);
            SettingsCommand = new RelayCommand(OnSettings);
            QuitCommand = new RelayCommand(OnQuit);
            rootVisible_ = true;
        }

        private void OnPlay(object context)
        {
            RootVisible = false;
            new GameModel().Create();
        }

        private void OnQuit(object context)
        {
            RootVisible = false;
            Logger.Info("Exiting, seeya");
            DxGame.Instance.Exit();
        }

        private void OnSettings(object context)
        {
            IMessageBoxService messageBoxService = GetService<IMessageBoxService>();
            if(ReferenceEquals(messageBoxService, null))
            {
                Logger.Warn("Could not find an {0}", typeof(IMessageBoxService));
                return;
            }

            messageBoxService.Show("Settings currently not implemented", null, false);
        }
    }
}