namespace Bibles.ViewModels
{
    using Models;
    using GalaSoft.MvvmLight.Command;
    using Services;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using Xamarin.Forms;
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;

    public class BiblesViewModel : BaseViewModel
    {

        #region Services

        private ApiServices apiservices;

        #endregion

        #region Attributes

        private BibleResponse bibleResponse;
        private ObservableCollection<BibleItemViewModel> bibles;
        private bool isRefreshing;
        private string filter;

        #endregion

        #region Properties

        public ObservableCollection<BibleItemViewModel> Bibles
        {
            get { return this.bibles; }
            set { SetValue(ref this.bibles, value); }
        }

        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { SetValue(ref this.isRefreshing, value); }
        }

        public string Filter
        {
            get { return this.filter; }

            set
            {
                SetValue(ref this.filter, value);
                Search();
            }
        }

        #endregion

        #region Constructors

        public BiblesViewModel()
        {
            this.apiservices = new ApiServices();
            this.LoadBibles();
        }

        #endregion

        #region ICommand

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadBibles);
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(Search);
            }
        }


        #endregion

        #region Methods

        private void Search()
        {

            this.IsRefreshing = true;

            /*

            if (string.IsNullOrEmpty(this.Filter))
            {
                this.Lands = new ObservableCollection<Bibles>
                    (this.ToLandItemViewModel());
            }
            else
            {
                this.Lands = new ObservableCollection<Bibles>(
                    this.ToLandItemViewModel().
                    Where(l => l.Name.ToLower().Contains(this.Filter.ToLower()) ||
                    l.Capital.ToLower().Contains(this.Filter.ToLower())));
            }

            */

            this.IsRefreshing = false;

        }

        private async void LoadBibles()
        {
            this.IsRefreshing = true;
            var connection = await this.apiservices.CheckConnection();

            if (!connection.IsSuccess)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connection.Message,
                    Languages.Accept);

                await App.Navigator.PopAsync();

                return;
            }

            var apiBible = Application.Current.Resources["APIBible"].ToString();

            var response = await this.apiservices.Get<BibleResponse>
                (
                apiBible,
                "/api",
                "/bibles"
                );

            if (!response.IsSuccess)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                await App.Navigator.PopAsync();

                return;
            }


            this.bibleResponse = (BibleResponse)response.Result;
            this.Bibles = new ObservableCollection<BibleItemViewModel>(
                this.ToBibleItemViewModel());

            this.IsRefreshing = false;

        }

        private IEnumerable<BibleItemViewModel> ToBibleItemViewModel()
        {
            return this.bibleResponse.Bibles.Select(b => new BibleItemViewModel
            {
                Copyright = b.Value.Copyright,
                Italics = b.Value.Italics,
                Lang = b.Value.Lang,
                LangShort = b.Value.LangShort,
                Module = b.Value.Module,
                Name = b.Value.Name,
                Rank = b.Value.Rank,
                Research = b.Value.Research,
                Shortname = b.Value.Shortname,
                Strongs = b.Value.Strongs,
                Year = b.Value.Year,
            });
        }


        #endregion
    }
}
