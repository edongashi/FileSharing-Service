using System;
using System.Windows.Input;

namespace FileSharing.Klienti.UI.ViewModels
{
    /// <summary>
    ///   Implementon ICommand duke marrur metodat e nevojshme përmes delegatëve.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> canExecute;
        private readonly Action<object> execute;

        /// <summary>
        ///   Inicializon një instancë të re të klasës <see cref="DelegateCommand"/> që gjithmonë mund të ekzekutohet.
        /// </summary>
        /// <param name="execute">Delegati që do të thirret kur ekzekutohet komanda.</param>
        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        ///   Inicializon një instancë të re të klasës <see cref="DelegateCommand"/>.
        /// </summary>
        /// <param name="execute">Delegati që do të thirret kur ekzekutohet komanda.</param>
        /// <param name="canExecute">Predikati që implementon kushtet e ekzekutimit të komandës.</param>
        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        ///   Ndodh kur ndryshon leja e ekzekutimit të komandës.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///   Shqyrton nëse lejohet ekzekutimi i komandës.
        /// </summary>
        /// <param name="parameter">Parametri i komandës.</param>
        /// <returns>Kthen true nëse komanda mund të ekzekutohet, përndryshe kthen false.</returns>
        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                return true;
            }

            return this.canExecute(parameter);
        }

        /// <summary>
        ///   Ekzekuton komandën.
        /// </summary>
        /// <param name="parameter">Parametri i komandës.</param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        /// <summary>
        ///   Shkrep ngjarjen CanExecuteChanged.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
