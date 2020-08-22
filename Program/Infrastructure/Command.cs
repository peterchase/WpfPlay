using System;
using System.Windows.Input;

namespace FridayAfternoon.WpfPlay.Program.Infrastructure
{
    public sealed class Command : ICommand
    {
        private readonly Action mExecute;
        private readonly Func<bool> mCanExecute;

        public Command(Action execute, Func<bool> canExecute = null)
        {
            mExecute = execute ?? throw new ArgumentNullException(nameof(execute));
            mCanExecute = canExecute ?? (() => true);
        }

        public bool CanExecute(object parameter) => mCanExecute();

        public void Execute(object parameter) => mExecute();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Refresh() => CommandManager.InvalidateRequerySuggested();
    }
}
