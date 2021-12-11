using FileEncryptor.WPF.Infrastructure.Commands.Base;
using System;

#nullable disable

namespace FileEncryptor.WPF.Infrastructure.Commands
{
    internal class LambdaCommand : Command
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public LambdaCommand(Action Execute, Func<bool> CanExecute = null)
            : this(
                  p => Execute(), 
                  CanExecute is null ? null : p => CanExecute()) { }

        public LambdaCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        protected override void Execute(object parameter) => _execute?.Invoke(parameter);

        protected override bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
    }
}
