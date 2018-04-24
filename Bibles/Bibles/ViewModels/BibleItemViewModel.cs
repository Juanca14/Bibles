namespace Bibles.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System.Windows.Input;
    using Models;
    using Views;

    public class BibleItemViewModel : Bible
    {
        #region Icommand

        public ICommand SelectLandCommand
        {
            get
            {
                return new RelayCommand(SelectBible);
            }
        }

        #endregion

        #region Methods

        private async void SelectBible()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Bible = new BibleViewModel(this);
            mainViewModel.SelectedModule = Module;
            await App.Navigator.PushAsync(new BiblePage());
        }

        #endregion
    }
}
