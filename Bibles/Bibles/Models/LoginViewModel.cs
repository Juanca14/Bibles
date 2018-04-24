namespace Bibles.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.Net.Mail;
    using System.Windows.Input;
    using Views;
    using Xamarin.Forms;
    using Helpers;
    using Services;
    using Models;

    public class LoginViewModel : BaseViewModel
    {

        #region Services

        private ApiServices apiServices;
        private DataServices dataServices;

        #endregion

        #region Attributes

        private bool isRunning;
        private bool isEnabled;
        private string password;
        private string email;

        #endregion

        #region Properties

        public string Email
        {
            get { return this.email; }
            set { SetValue(ref this.email, value); }
        }

        public string Password
        {
            get { return this.password; }
            set { SetValue(ref this.password, value); }
        }

        public bool IsRunning
        {
            get { return this.isRunning; }
            set { SetValue(ref this.isRunning, value); }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { SetValue(ref this.isEnabled, value); }
        }

        public bool IsRemembered
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        public LoginViewModel()
        {
            this.apiServices = new ApiServices();
            this.dataServices = new DataServices();
            this.IsRemembered = true;
            this.IsEnabled = true;
            this.Email = "Juank-nac@hotmail.com";
            this.Password = "123456";
        }

        #endregion

        #region Commands

        public ICommand LoginFacebookComand
        {
            get
            {
                return new RelayCommand(LoginFacebook);
            }
        }

        private async void LoginFacebook()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new LoginFacebookPage());
        }

        public ICommand RegisterCommand
        {
            get
            {
                return new RelayCommand(Register);
            }
        }

        private async void Register()
        {
            MainViewModel.GetInstance().Register = new RegisterViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
        }

        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand(Login);
            }
        }


        private async void Login()
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.EmailValidation,
                    Languages.Accept);
                return;
            }
            else
            {
                try
                {
                    MailAddress m = new MailAddress(this.Email);

                }
                catch (FormatException)
                {
                    await Application.Current.MainPage.DisplayAlert(
                     Languages.Error,
                     Languages.EmailType,
                     Languages.Accept);
                    return;
                }
            }


            if (string.IsNullOrEmpty(this.Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordValidation,
                    Languages.Accept);

                await Application.Current.MainPage.Navigation.PopAsync();

                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var connection = await this.apiServices.CheckConnection();

            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;

                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connection.Message,
                    Languages.Accept);

                await Application.Current.MainPage.Navigation.PopAsync();

                return;
            }

            var apiSecurity = Application.Current.Resources["APISecurity"].ToString();

            var token = await this.apiServices.GetToken(
                apiSecurity,
                this.Email,
                this.Password);

            if (token == null)
            {
                this.IsRunning = false;
                this.IsEnabled = true;

                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.SomethingWrong,
                    Languages.Accept);

                await Application.Current.MainPage.Navigation.PopAsync();

                return;
            }

            if (string.IsNullOrEmpty(token.AccessToken))
            {
                this.IsRunning = false;
                this.IsEnabled = true;

                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    token.ErrorDescription,
                    Languages.Accept);

                this.Password = string.Empty;
                return;
            }

            var user = await this.apiServices.GetUserByEmail(
                apiSecurity,
                "/api",
                "/Users/GetUserByEmail",
                token.TokenType,
                token.AccessToken,
                this.Email);

            UserLocal userLocal = null;

            if (user != null)
            {
                userLocal = Converter.ToUserLocal(user);
                this.dataServices.DeleteAllAndInsert(userLocal);
                this.dataServices.DeleteAllAndInsert(token);
            }

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Token = token;
            mainViewModel.User = userLocal;
            mainViewModel.Bibles = new BiblesViewModel();
            Application.Current.MainPage = new MasterPage();

            if (this.IsRemembered)
            {
                Settings.Token = token.AccessToken;
                Settings.TokenType = token.TokenType;
                Settings.IsRemembered = "true";
            }

            this.IsRunning = false;
            this.IsEnabled = true;

            this.Email = String.Empty;
            this.Password = String.Empty;

        }
        #endregion

    }
}
