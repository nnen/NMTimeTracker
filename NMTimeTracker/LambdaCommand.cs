using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NMTimeTracker
{
    public class LambdaCommand : ICommand
    {
        private readonly Action m_action;


        public LambdaCommand(Action action)
        {
            m_action = action;
        }


#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            m_action.Invoke();
        }
    }
}
