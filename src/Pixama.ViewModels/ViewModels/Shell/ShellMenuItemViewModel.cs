using System;
using ReactiveUI;

namespace Pixama.ViewModels.ViewModels.Shell
{
    public class ShellMenuItemViewModel : BaseViewModel
    {
        #region Fields

        private string _name;

        #endregion

        #region Properties

        public Type ViewModel { get; }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        #endregion

        public ShellMenuItemViewModel(Type viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
