using FileEncryptor.WPF.Infrastructure.Commands;
using FileEncryptor.WPF.Services.Interfaces;
using FileEncryptor.WPF.ViewModels.Base;
using System.IO;
using System.Windows.Input;

namespace FileEncryptor.WPF.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Fields and Properties

        private readonly IUserDialog _userDialog;

        #region Title

        private string? _title = "Файловый шифровщик";

        public string Title { get => _title ?? string.Empty; set => Set(ref _title, value); }

        #endregion // Title

        #region Password 

        private string? _password = "123";

        public string? Password { get => _password; set => Set(ref _password, value); }

        #endregion // Password

        #region SelectedFile 

        private FileInfo? _selectedFile;

        public FileInfo? SelectedFile { get => _selectedFile; set => Set(ref _selectedFile, value); }

        #endregion // SelectedFile

        #endregion // Fields and Properties



        #region Commands

        private ICommand? _selectFileCommand;

        public ICommand SelectFileCommand => _selectFileCommand ??= new LambdaCommand(OnSelectFileCommandExecuted);

        private void OnSelectFileCommandExecuted()
        {
            if (!_userDialog.OpenFile("Выбор файла для шифрования", out var filePath)) return;
            var selectedFile = new FileInfo(filePath);
            SelectedFile = selectedFile.Exists ? selectedFile : null;
        }

        #endregion // Commands


        /// <summary>
        /// Конструктор по-умолчанию
        /// </summary>
        public MainWindowViewModel(IUserDialog userDialog)
        {
            _userDialog = userDialog;
        }

    }
}
