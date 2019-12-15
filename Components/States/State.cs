using System;
using System.Threading.Tasks;

namespace Components.States
{
    public abstract class State
    {
        private bool _initialized;
        public async Task LoadInitialState()
        {
            if (!_initialized)
            {
                _initialized = true;
                await OnLoad();
                StateHasChanged();
            }
        }
        protected abstract Task OnLoad();
        public event EventHandler StateChanged;    
        protected void StateHasChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}