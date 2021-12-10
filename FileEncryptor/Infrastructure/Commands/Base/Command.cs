﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileEncryptor.WPF.Infrastructure.Commands.Base
{
    internal abstract class Command : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private bool _executable;

        public bool Executable
        {
            get { return _executable; }
            set
            {
                if (_executable != value) return;
                _executable = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        bool ICommand.CanExecute(object? parameter) => _executable && CanExecute(parameter);

        void ICommand.Execute(object? parameter) { if (CanExecute(parameter)) Execute(parameter); }

        protected virtual bool CanExecute(object? parameter) => true;

        protected abstract void Execute(object? parameter);

    }
}
