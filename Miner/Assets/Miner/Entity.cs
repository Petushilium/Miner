using Cysharp.Threading.Tasks;
using Shared.Disposable;

namespace Miner 
{
    internal sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
        }

        private readonly Ctx _ctx;

        private readonly Loading.Entity _loadingScreen;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;

            _loadingScreen = new Loading.Entity(new Loading.Entity.Ctx
            {
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
