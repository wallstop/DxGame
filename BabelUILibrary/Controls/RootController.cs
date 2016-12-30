using System;
using Babel.Services;
using DxCore;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;
using NLog;
using WallNetCore.Validate;

namespace BabelUILibrary.Controls
{
    public enum MenuType
    {
        None,
        MainMenu,
        Settings
    }

    public sealed class RootController : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        public ICommand SettingsCommand { get; }

        public SettingsController SettingsController { get; }

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

        /* TODO: Switch to enum of all types of menus / screens */
        private MenuType CurrentMenu { get; set; }

        private Predicate<object> MainMenuCanExecute { get; }

        public RootController()
        {
            CurrentMenu = MenuType.MainMenu;
            MainMenuCanExecute = _ => CurrentMenu == MenuType.MainMenu;
            PlayCommand = new RelayCommand(OnPlay, MainMenuCanExecute);
            SettingsCommand = new RelayCommand(OnSettings, MainMenuCanExecute);
            QuitCommand = new RelayCommand(OnQuit, MainMenuCanExecute);
            RootVisible = true;

            SettingsController = new SettingsController(settingsEnabled =>
            {
                if(settingsEnabled)
                {
                    SettingsMenuVisible = true;
                }
                else
                {
                    MainMenuVisible = true;
                }
            });
        }

        private void NotifyMenuTypeChanged()
        {
            bool doesntMatter = !RootVisible;
            /* Trust me I know what I'm doing */
            SetProperty(ref doesntMatter, RootVisible, nameof(RootVisible));
            doesntMatter = !MainMenuVisible;
            SetProperty(ref doesntMatter, MainMenuVisible, nameof(MainMenuVisible));
            doesntMatter = !SettingsMenuVisible;
            SetProperty(ref doesntMatter, SettingsMenuVisible, nameof(SettingsMenuVisible));
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
            SettingsMenuVisible = true;
            NotifyMenuTypeChanged();
        }
    }
}