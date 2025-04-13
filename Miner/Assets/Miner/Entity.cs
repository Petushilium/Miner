using Shared.Disposable;
using System;

namespace Miner 
{
    internal sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public IObservable<float> OnUpdate;
            public Data Data;
        }

        private readonly Ctx _ctx;

        private readonly LoadingScreen.LoadingScreen.Entity _loadingScreen;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;

            _loadingScreen = new LoadingScreen.LoadingScreen.Entity(new LoadingScreen.LoadingScreen.Entity.Ctx
            {
                OnUpdate = _ctx.OnUpdate,
                Data = _ctx.Data.LoadingScreenData,
            }).AddTo(this);
        }

        public async void AsyncInit()
        {
            _loadingScreen.ShowImmediate();

            //loading here...

            await _loadingScreen.Hide();
        }
    }
}
