using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Components.States;
using Microsoft.AspNetCore.Components;


namespace Components.Shared
{
    public abstract class StatefulComponent : ComponentBase, IDisposable
    {
        private readonly List<IState> _states = new List<IState>();

        protected void Observe(params IState[] states)
        {
            _states.AddRange(states);
            foreach (var state in states)
            {
                state.StateChanged += OnStateChanged;
            }
        }

        void OnStateChanged(object sender, EventArgs e) => StateHasChanged();

        public virtual void Dispose()
        {
            if (_states != null)
            {
                foreach (var state in _states)
                {
                    state.StateChanged -= OnStateChanged;
                }
            }
        }

#if ClientSideExecution
        protected override async Task OnInitializedAsync()
        {
            await LoadStateAsync();
            await base.OnInitializedAsync();
        }
#else
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender){
                await LoadStateAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }
#endif

        private async Task LoadStateAsync()
        {
            var tasks = new List<Task>();
            foreach (var state in _states)
            {
                tasks.Add(state.LoadInitialState());
            }
            await Task.WhenAll(tasks);
        }
    }
}
