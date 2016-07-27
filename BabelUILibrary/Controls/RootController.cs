using System;
using Babel.Services;
using DxCore;
using DxCore.Core.Utils.Validate;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;
using NLog;

namespace BabelUILibrary.Controls
{
    public enum MenuType
    {
        None,
        MainMenu,
        Settings
    }

    public class RootController : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /* TODO: Switch to enum of all types of menus / screens */
        private MenuType CurrentMenu { get; set; }

        public ICommand PlayCommand { get; }
        public ICommand QuitCommand { get; }

        public bool RootVisible
        {
            get { return CurrentMenu != MenuType.None; }
            set
            {
                if(value)
                {
                    CurrentMenu = MenuType.MainMenu;
                }
                else
                {
                    CurrentMenu = MenuType.None;
                }
                NotifyMenuTypeChanged();
            }
        }

        public bool MainMenuVisible
        {
            get { return CurrentMenu == MenuType.MainMenu; }
            set
            {
                Validate.Hard.IsTrue(value);
                CurrentMenu = MenuType.MainMenu;
                NotifyMenuTypeChanged();
            }
        }

        public bool SettingsMenuVisible
        {
            get { return CurrentMenu == MenuType.Settings; }
            set
            {
                Validate.Hard.IsTrue(value);
                CurrentMenu = MenuType.Settings;
                NotifyMenuTypeChanged();
            }
        }

        private void NotifyMenuTypeChanged()
        {
            bool doesntMatter = !RootVisible;
            SetProperty(ref doesntMatter, RootVisible, nameof(RootVisible));
            doesntMatter = !MainMenuVisible;
            SetProperty(ref doesntMatter, MainMenuVisible, nameof(MainMenuVisible));
            doesntMatter = !SettingsMenuVisible;
            SetProperty(ref doesntMatter, SettingsMenuVisible, nameof(SettingsMenuVisible));
        }

        private Predicate<object> MainMenuCanExecute { get; }
        private Predicate<object> SettingsCanExecute { get; }

        public ICommand SettingsCommand { get; }

        public RootController()
        {
            CurrentMenu = MenuType.MainMenu;
            MainMenuCanExecute = _ => CurrentMenu == MenuType.MainMenu;
            SettingsCanExecute = _ => CurrentMenu == MenuType.Settings;
            PlayCommand = new RelayCommand(OnPlay, MainMenuCanExecute);
            SettingsCommand = new RelayCommand(OnSettings, MainMenuCanExecute);
            QuitCommand = new RelayCommand(OnQuit, MainMenuCanExecute);
            RootVisible = true;
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

            messageBoxService.Show("Settings currently not implemented :'(", null, false);
        }
    }
}