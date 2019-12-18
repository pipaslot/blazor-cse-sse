using System;
using System.Threading.Tasks;

namespace Components.States
{
    public interface IState
    {
        Task LoadInitialState();

        event EventHandler StateChanged;
    }
}