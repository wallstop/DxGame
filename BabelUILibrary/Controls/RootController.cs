using System;
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

        private Predicate<object> CanExecute { get; }

        public ICommand SettingsCommand { get; }

        public RootController()
        {
            CanExecute = _ => RootVisible;
            PlayCommand = new RelayCommand(OnPlay, CanExecute);
            SettingsCommand = new RelayCommand(OnSettings, CanExecute);
            QuitCommand = new RelayCommand(OnQuit, CanExecute);
            rootVisible_ = true;
        }

        private void OnPlay(object context)
        {
            RootVisible = false;
            new GameService().Create();
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