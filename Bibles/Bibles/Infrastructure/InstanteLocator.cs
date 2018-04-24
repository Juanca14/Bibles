namespace Bibles.Infrastructure
{
    using ViewModels;

    public class InstanteLocator
    {
        #region Properties

        public MainViewModel Main
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        public InstanteLocator()
        {
            this.Main = new MainViewModel();
        }

        #endregion
    }
}
