using System;
using System.Threading.Tasks;

namespace Components.States
{
    public abstract class TemporaryState : IState
    {
        private bool _initialized;
        private Task _initializeTask;
        public async Task LoadInitialState()
        {
            if (!_initialized)
            {
                if (_initializeTask == null)
                {
                    _initializeTask = OnLoad();
                }
                await _initializeTask;
                _initialized = true;
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