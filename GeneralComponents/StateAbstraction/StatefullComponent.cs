using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace GeneralComponents.StateAbstraction
{
    public class StatefullComponent : ComponentBase, IDisposable
    {
        private readonly List<State> _states = new List<State>();

        protected void Observe(params State[] states)
        {
            _states.AddRange(states);
            foreach (var state in states)
            {
                state.StateChanged += OnStateChanged;
            }
        }

        void OnStateChanged(object sender, EventArgs e) => StateHasChanged();

        void IDisposable.Dispose()
        {
            if (_states != null)
            {
                foreach (var state in _states)
                {
                    state.StateChanged -= OnStateChanged;
                }
            }
        }
    }
}
