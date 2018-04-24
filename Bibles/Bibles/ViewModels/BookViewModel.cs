namespace Bibles.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Models;
    using Services;
    using Xamarin.Forms;
    using Helpers;

    public class BookViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private string bookSelected;
        private long chapterSelected;
        private Book book;
        private Nav nav;
        private bool isRefreshing;
        private bool isEnabled;
        private bool isVisiblePrev;
        private bool isVisibleNext;
        private ContentResponse contentResponse;
        private ObservableCollection<Verse> verses;
        #endregion

        #region Properties

        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { SetValue(ref this.isRefreshing, value); }
        }

        public ObservableCollection<Verse> Verses
        {
            get { return this.verses; }
            set { SetValue(ref this.verses, value); }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { SetValue(ref this.isEnabled, value); }
        }

        public bool IsVisiblePrev
        {
            get { return this.isVisiblePrev; }
            set { SetValue(ref this.isVisiblePrev, value); }
        }

        public bool IsVisibleNext
        {
            get { return this.isVisibleNext; }
            set { SetValue(ref this.isVisibleNext, value); }
        }

        public string BookSelected
        {
            get { return this.bookSelected; }
            set { SetValue(ref this.bookSelected, value); }
        }

        public long ChapterSelected
        {
            get { return this.chapterSelected; }
            set { SetValue(ref this.chapterSelected, value); }
        }

        #endregion

        #region Constructors

        public BookViewModel(Book book)
        {
            this.apiService = new ApiServices();
            this.book = book;
            this.BookSelected = book.Name;
            this.ChapterSelected = 1;
            this.IsEnabled = true;
            this.nav = new Nav();
            this.LoadContent(null);
        }

        #endregion

        #region Commands

        public ICommand PrevCommand
        {
            get
            {
                return new RelayCommand(PrevChapter);
            }
        }

        public ICommand NextCommand
        {
            get
            {
                return new RelayCommand(NextChapter);
            }
        }

        #endregion

        #region Methods

        private async void LoadContent(string shorname)
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
                await App.Navigator.PopAsync();
                return;
            }

            var apiBible = Application.Current.Resources["APIBible"].ToString();

            if (shorname != null)
            {

                MainViewModel.GetInstance().ChapterName = shorname;

                var response = await this.apiService.Get<ContentResponse>(
                apiBible,
                "/api",
                string.Format(
                "?bible={0}&reference={1}",
                MainViewModel.GetInstance().SelectedModule,
                shorname));

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

                this.contentResponse = (ContentResponse)response.Result;

            }
            else
            {

                var response = await this.apiService.Get<ContentResponse>(
                apiBible,
                "/api",
                string.Format("?bible={0}&reference={1}",
                              MainViewModel.GetInstance().SelectedModule,
                              this.book.Shortname)); //Aqui el shortname del capitulo a buscar

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

                this.contentResponse = (ContentResponse)response.Result;
            }

            this.IsRefreshing = false;

            var contentResult = contentResponse.Contents[0];
            this.nav = contentResponse.Contents[0].Nav;

            var type = typeof(Bibles);
            var properties = type.GetRuntimeFields();
            Bible bible = null;

            foreach (var property in properties)
            {
                bible = (Bible)property.GetValue(contentResult.Bibles);
                if (bible != null)
                {
                    break;
                }
            }

            if (bible == null)
            {
                return;
            }

            type = typeof(Bible);
            properties = type.GetRuntimeFields();
            Dictionary<string, Verse> chapter = null;

            foreach (var property in properties)
            {
                if (property.Name.StartsWith("<Chapter"))
                {
                    chapter = (Dictionary<string, Verse>)property.GetValue(bible);

                    if (chapter != null)
                    {
                        break;
                    }
                }
            }

            var myVerses = chapter.Select(v => new Verse
            {
                Book = v.Value.Book,
                Chapter = v.Value.Chapter,
                Id = v.Value.Id,
                Italics = v.Value.Italics,
                Text = v.Value.Text,
                VerseNumber = v.Value.VerseNumber,
            });

            this.ChapterSelected = myVerses.
                               Where(v => v.VerseNumber == 1).
                               Select(c => c.Chapter).FirstOrDefault();


            this.Verses = new ObservableCollection<Verse>(myVerses);

            this.SetButtons();
        }

        private void PrevChapter()
        {

            this.LoadContent(string.Format(
                                    "{0} {1}",
                                    Navigation(this.nav.PrevChapterBookId.Value),
                                    this.nav.PrevChapterId.Value));
        }

        private void NextChapter()
        {
            this.LoadContent(string.Format(
                                     "{0} {1}",
                                     Navigation(this.nav.NextChapterBookId.Value),
                                     this.nav.NextChapterId.Value));
        }

        private object Navigation(int chapterBookId)
        {
            var bookName = MainViewModel.GetInstance().BookResponse.Books.Where(b => b.Id.Equals(chapterBookId))
                            .Select(x => x.Shortname).FirstOrDefault();

            this.BookSelected = MainViewModel.GetInstance().BookResponse.Books.
                     Where(b => b.Id.Equals(chapterBookId)).
                     Select(x => x.Name).FirstOrDefault();


            return bookName;
        }

        private void SetButtons()
        {
            var prevChapter = this.nav.PrevChapter;
            var nextChapter = this.nav.NextChapter;

            if (string.IsNullOrEmpty(prevChapter))
            {
                this.IsVisiblePrev = false;
            }
            else
            {
                this.IsVisiblePrev = true;
            }

            if (string.IsNullOrEmpty(nextChapter))
            {
                this.IsVisibleNext = false;
            }
            else
            {
                this.IsVisibleNext = true;
            }
        }


        #endregion
    }
}
