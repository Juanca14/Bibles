namespace Bibles.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System.Windows.Input;
    using Views;
    using Xamarin.Forms;
    using Helpers;

    public class MenuItemViewModel
    {

        #region Properties
        public string Icon { get; set; }

        public string Title { get; set; }

        public string PageName { get; set; }
        #endregion

        #region Commands

        public ICommand NavigateCommand
        {
            get
            {
                return new RelayCommand(Navigate);
            }
        }

        private void Navigate()
        {
            var mainViewModel = MainViewModel.GetInstance();
            App.Master.IsPresented = false;

            if (this.PageName == "LoginPage")
            {
                Settings.Token = string.Empty;
                Settings.TokenType = string.Empty;
                Settings.IsRemembered = string.Empty;
                mainViewModel.Token = null;
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }

        }

        #endregion
    }
}
