using FileEncryptor.WPF.ViewModels.Base;

namespace FileEncryptor.WPF.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {

        #region Title

        private string? _title = "Файловый шифровщик";

        public string Title { get => _title; set => Set(ref _title, value); }

        #endregion // Title




    }
}
