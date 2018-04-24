namespace Bibles.ViewModels
{
    using Services;
    using System.Collections.Generic;
    using Models;
    using Xamarin.Forms;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Helpers;

    public class BibleViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private Bible bible;
        private bool isRefreshing;
        private ObservableCollection<BookItemViewModel> books;
        #endregion

        #region Properties

        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { SetValue(ref this.isRefreshing, value); }
        }

        public ObservableCollection<BookItemViewModel> Books
        {
            get { return this.books; }
            set { SetValue(ref this.books, value); }
        }
        #endregion

        #region Constructors

        public BibleViewModel(Bible bible)
        {
            this.apiService = new ApiServices();
            this.bible = bible;
            this.LoadBooks();
        }
        #endregion

        #region Methods
        private async void LoadBooks()
        {
            this.IsRefreshing = true;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connection.Message,
                    Languages.Accept);
                return;
            }

            var apiBible = Application.Current.Resources["APIBible"].ToString();

            var response = await this.apiService.Get<BookResponse>(
                apiBible,
                "/api",
                string.Format("/books?language={0}", bible.LangShort));

            if (!response.IsSuccess)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                return;
            }


            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.BookResponse = (BookResponse)response.Result;

            if (bible.LangShort != "en")
            {
                var response2 = await this.apiService.Get<BookResponse>(
                    apiBible,
                    "/api",
                    "/books?language=en");

                if (!response2.IsSuccess)
                {
                    this.IsRefreshing = false;
                    await Application.Current.MainPage.DisplayAlert(
                        Languages.Error,
                        response2.Message,
                        Languages.Accept);
                    return;
                }

                var booksResult2 = (BookResponse)response2.Result;
                mainViewModel.BookResponse = (BookResponse)response.Result;

                for (int i = 0; i < mainViewModel.BookResponse.Books.Count; i++)
                {
                    mainViewModel.BookResponse.Books[i].Shortname = booksResult2.Books[i].Shortname;
                    //this.bookResponse.Books[i].Shortname = booksResult2.Books[i].Shortname;
                }
            }

            this.Books = new ObservableCollection<BookItemViewModel>(
                this.ToBookItemViewModel());
            this.IsRefreshing = false;
        }

        private IEnumerable<BookItemViewModel> ToBookItemViewModel()
        {
            return MainViewModel.GetInstance().BookResponse.Books.Select(b => new BookItemViewModel
            //return this.bookResponse.Books.Select(b => new BookItemViewModel
            {
                Id = b.Id,
                Name = b.Name,
                Shortname = b.Shortname,
            });
        }
        #endregion

    }
}
