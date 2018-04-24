namespace Bibles.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Models;
    using System.Windows.Input;
    using Xamarin.Forms;
    using Views;

    public class BookItemViewModel : Book
    {
        #region Commands
        public ICommand SelectBookCommand
        {
            get
            {
                return new RelayCommand(SelectBook);
            }
        }

        private void SelectBook()
        {
            MainViewModel.GetInstance().Book = new BookViewModel(this);
            App.Navigator.PushAsync(new BookPage());
        }
        #endregion

    }
}
