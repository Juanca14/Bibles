namespace Bibles.ViewModels
{
    using System.Collections.ObjectModel;
    using Helpers;
    using Models;

    public class MainViewModel : BaseViewModel
    {
        #region Attibrutes
        private UserLocal user;
        private string chapterName;
        #endregion

        #region Properties

        public string SelectedModule
        {
            get;
            set;
        }

        public string ChapterName
        {
            get { return this.chapterName; }
            set { SetValue(ref this.chapterName, value); }
        }

        public TokenResponse Token
        {
            get; set;
        }

        public UserLocal User
        {
            get { return this.user; }
            set { SetValue(ref this.user, value); }
        }

        public BookResponse BookResponse
        {
            get;
            set;
        }

        #endregion

        #region ViewModels

        public LoginViewModel Login
        {
            get;
            set;
        }

        public BiblesViewModel Bibles
        {
            get;
            set;
        }

        public BibleViewModel Bible
        {
            get;
            set;
        }

        public BookViewModel Book
        {
            get;
            set;
        }

        public ObservableCollection<MenuItemViewModel> Menus
        {
            get;
            set;
        }

        public RegisterViewModel Register
        {
            get;
            set;
        }

        #endregion

        #region Construtors

        public MainViewModel()
        {
            instance = this;
            this.Login = new LoginViewModel();
            this.LoadMenu();
        }

        #endregion


        #region Methods

        private void LoadMenu()
        {
            this.Menus = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel
                {
                    Icon = "ic_search",
                    PageName = "SearchByDating",
                    Title = Languages.SearchByDating,
                },

                new MenuItemViewModel
                {
                    Icon = "ic_search",
                    PageName = "SearchByKeyWord",
                    Title = Languages.SearchByKeyWord,
                },

                new MenuItemViewModel
                {
                    Icon = "ic_exit_to_app",
                    PageName = "LoginPage",
                    Title = Languages.LogOut,
                }
            };
        }

        #endregion

        #region Singleton

        private static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }

            return instance;
        }

        #endregion

    }
}
