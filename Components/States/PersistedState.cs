using System.Threading.Tasks;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;

namespace Components.States
{
    public abstract class PersistedState<TData> : TemporaryState where TData : new()
    {
        protected TData Data = new TData();
        private readonly LocalStorage _localStorage;
        private string StorageUniqueName => GetType().FullName;

        protected PersistedState(LocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task OnLoad()
        {
            var loaded = await _localStorage.GetItemAsync<TData>(StorageUniqueName);
            Data = loaded != null ? loaded : Data;
        }

        protected async Task SaveChangesAsync()
        {
            await _localStorage.SetItemAsync(StorageUniqueName, Data);
            StateHasChanged();
        }

        protected void SaveChanges()
        {
            _localStorage.SetItemAsync(StorageUniqueName, Data);
            StateHasChanged();
        }
    }
}