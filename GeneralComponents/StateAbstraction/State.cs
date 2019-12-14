using System;
using System.Threading.Tasks;

namespace GeneralComponents.StateAbstraction
{
    public abstract class State
    {
        //private bool _initialized;
        //public async Task LoadInitialState()
        //{
        //    if (!_initialized)
        //    {
        //        await OnLoad();
        //        StateHasChanged();
        //        _initialized = true;
        //    }
        //}
        //protected abstract Task OnLoad();
        public event EventHandler StateChanged;    
        protected void StateHasChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}