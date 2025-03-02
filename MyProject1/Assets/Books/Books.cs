using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.Reactive;
using System;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.Networking;

namespace Books
{
    internal sealed class Books : BaseDisposableMB
    {
        private class Entity : BaseDisposable
        {
            private class Logic : BaseDisposable
            {
                public struct Ctx
                {
                    public IReadOnlyReactiveCommand<float> OnUpdate;
                }

                private readonly Ctx _ctx;

                public Logic(Ctx ctx)
                {
                    _ctx = ctx;
                }
            }

            public struct Ctx
            {
                public IReadOnlyReactiveCommand<float> OnUpdate;
                public Data Data;
            }

            private readonly Ctx _ctx;

            private readonly LoadingScreen.LoadingScreen.Entity _loadingScreen;

            public Entity(Ctx ctx)
            {
                _ctx = ctx;

                _ = new Logic(new Logic.Ctx
                {
                    OnUpdate = _ctx.OnUpdate,
                }).AddTo(this);

                _loadingScreen = new LoadingScreen.LoadingScreen.Entity(new LoadingScreen.LoadingScreen.Entity.Ctx
                {
                    OnUpdate = _ctx.OnUpdate,
                    Data = _ctx.Data.LoadingScreenData,
                }).AddTo(this);

                AsyncInit();
            }

            private async void AsyncInit()
            {
                _loadingScreen.ShowImmediate();

                var url = $"{Application.streamingAssetsPath}/story.json";
                using (var request = UnityWebRequest.Get(url)) 
                {
                    request.SetRequestHeader("Access-Control-Allow-Credentials", "true");
                    request.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
                    request.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                    request.SetRequestHeader("Access-Control-Allow-Origin", "*");

                    await request.SendWebRequest();

                    Debug.Log(request.downloadHandler.text);
                }

                foreach(var book in _ctx.Data.Books.BooksData) 
                {
                    _ctx.Data.BooksScreen.AddBook(book.MainImage, book.Title, book.Genres, book.Description);
                }

                await _loadingScreen.Hide();
            }
        }

        [Serializable]
        private struct Data
        {
            [SerializeField] private LoadingScreen.LoadingScreen.Data _loadingScreenData;
            [SerializeField] private ScriptableObjects.Books _books;
            [SerializeField] private UI.BooksScreen _booksScreen;

            public readonly LoadingScreen.LoadingScreen.Data LoadingScreenData => _loadingScreenData;
            public readonly ScriptableObjects.Books Books => _books;
            public readonly UI.BooksScreen BooksScreen => _booksScreen;
        }

        [SerializeField] private Data _data;

        private IReactiveCommand<float> _onUpdate;

        private void OnEnable()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopHelper.Initialize(ref playerLoop);

            _onUpdate = new ReactiveCommand<float>().AddTo(this);
            _ = new Entity(new Entity.Ctx
            {
                OnUpdate = _onUpdate,
                Data = _data,
            }).AddTo(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        private void Update()
        {
            _onUpdate.Execute(Time.deltaTime);
        }
    }
}

