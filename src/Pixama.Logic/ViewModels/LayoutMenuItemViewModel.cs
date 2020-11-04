using System;

namespace Pixama.Logic.ViewModels
{
    public class LayoutMenuItemViewModel : BaseViewModel
    {
        #region Constructors

        public LayoutMenuItemViewModel(string name, Type viewModel)
        {
            this.ViewModel = viewModel;
            this.Name = name;
        }

        #endregion

        #region Properties

        public Type ViewModel { get; }

        public string Name { get; }

        #endregion
    }
}
