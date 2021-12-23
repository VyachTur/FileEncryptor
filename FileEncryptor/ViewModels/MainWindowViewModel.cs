using FileEncryptor.WPF.Infrastructure.Commands;
using FileEncryptor.WPF.Infrastructure.Commands.Base;
using FileEncryptor.WPF.Services.Interfaces;
using FileEncryptor.WPF.ViewModels.Base;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Input;

namespace FileEncryptor.WPF.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Fields and Properties

        private const string c_encryptedFileSuffix = ".encrypted";

        private readonly IUserDialog _userDialog;
        private readonly IEncryptor _encryptor;

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

        #region ProgressValue 

        private double _progressValue;

        public double ProgressValue { get => _progressValue; set => Set(ref _progressValue, value); }

        #endregion // ProgressValue


        #region ProcessCancellation 

        private CancellationTokenSource? _processCancellation;

        public CancellationTokenSource? ProcessCancellation { get => _processCancellation; set => Set(ref _processCancellation, value); }

        #endregion // ProcessCancellation


        #endregion // Fields and Properties



        #region Commands

        #region SelectFileCommand

        private ICommand? _selectFileCommand;

        public ICommand SelectFileCommand => _selectFileCommand ??= new LambdaCommand(OnSelectFileCommandExecuted);

        private void OnSelectFileCommandExecuted()
        {
            if (!_userDialog.OpenFile("Выбор файла для шифрования", out var filePath)) return;
            var selectedFile = new FileInfo(filePath);
            SelectedFile = selectedFile.Exists ? selectedFile : null;
        }

        #endregion // SelectFileCommand


        #region CancelCommand

        private ICommand? _cancelCommand;

        public ICommand CancelCommand => _cancelCommand ??= new LambdaCommand(OnCancelCommandExecuted, CanCancelCommandExecute);

        private bool CanCancelCommandExecute(object p) => _processCancellation != null && !_processCancellation.IsCancellationRequested;

        private void OnCancelCommandExecuted(object p) => _processCancellation?.Cancel();

        #endregion // CancelCommand


        #region EncryptCommand

        private ICommand? _encryptCommand;

        public ICommand EncryptCommand => _encryptCommand ??= new LambdaCommand(OnEncryptCommandExecuted, CanEncryptCommandExecute);

        private bool CanEncryptCommandExecute(object p) 
            => (p is FileInfo file && file.Exists || SelectedFile != null) 
            && !string.IsNullOrWhiteSpace(Password);

        private async void OnEncryptCommandExecuted(object p)
        {
            var file = p as FileInfo ?? SelectedFile;
            if (file is null) return;

            var defaultFileName = file.FullName + c_encryptedFileSuffix;
            if (!_userDialog.SaveFile("Выбор файла для сохранения", out var destinationPath, defaultFileName)) return;

            var timer = Stopwatch.StartNew();

            var progress = new Progress<double>(percent => ProgressValue = percent);

            _processCancellation = new CancellationTokenSource();
            var cancel = _processCancellation.Token;

            ((Command)EncryptCommand).Executable = false;
            ((Command)DecryptCommand).Executable = false;
            // Дополнительный код, выполняемый параллельно процессу дешифрования
            //......

            try
            {
                await _encryptor.EncriptAsync(file.FullName, destinationPath, Password ??= string.Empty, progress: progress, cancel: cancel);
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                _processCancellation.Dispose();
                _processCancellation = null;
            }

            ((Command)EncryptCommand).Executable = true;
            ((Command)DecryptCommand).Executable = true;

            timer?.Stop();

            //_userDialog.Information("Шифрование", $"Шифрование файла выполнено успешно за {timer?.Elapsed.TotalSeconds:0.##} с.");
        }

        #endregion // EncryptCommand

        #region DecryptCommand

        private ICommand? _decryptCommand;

        public ICommand DecryptCommand => _decryptCommand ??= new LambdaCommand(OnDecryptCommandExecuted, CanDecryptCommandExecute);

        private bool CanDecryptCommandExecute(object p)
            => (p is FileInfo file && file.Exists || SelectedFile != null)
            && !string.IsNullOrWhiteSpace(Password);

        private async void OnDecryptCommandExecuted(object p)
        {
            var file = p as FileInfo ?? SelectedFile;
            if (file is null) return;

            var defaultFileName = file.FullName.EndsWith(c_encryptedFileSuffix)
                ? file.FullName.Substring(0, file.FullName.Length - c_encryptedFileSuffix.Length)
                : file.FullName;

            if (!_userDialog.SaveFile("Выбор файла для сохранения", out var destinationPath, defaultFileName)) return;

            var timer = Stopwatch.StartNew();

            var progress = new Progress<double>(percent => ProgressValue = percent);

            _processCancellation = new CancellationTokenSource();
            var cancel = _processCancellation.Token;

            ((Command)EncryptCommand).Executable = false;
            ((Command)DecryptCommand).Executable = false;
            // Дополнительный код, выполняемый параллельно процессу дешифрования
            //......

            var success = false;
            try
            {
                success = await _encryptor.DecriptAsync(file.FullName, destinationPath, Password ??= string.Empty, progress: progress, cancel: cancel);
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                _processCancellation.Dispose();
                _processCancellation = null;
            }

            ((Command)EncryptCommand).Executable = true;
            ((Command)DecryptCommand).Executable = true;

            timer?.Stop();

            if (success)
            {
            //_userDialog.Information("Шифрование", $"Дешифровка файла выполнена успешно за {timer?.Elapsed.TotalSeconds:0.##} с.");
            }
            else
                _userDialog.Warning("Шифрование", "Сбой дешифровки, указан неверный пароль!");
        }

        #endregion // DecryptCommand


        #endregion // Commands


        /// <summary>
        /// Конструктор по-умолчанию
        /// </summary>
        public MainWindowViewModel(IUserDialog userDialog, IEncryptor encryptor)
        {
            _userDialog = userDialog;
            _encryptor = encryptor;
        }

    }
}
