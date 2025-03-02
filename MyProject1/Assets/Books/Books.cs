using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Disposable;
using Shared.Reactive;
using System;
using System.Collections.Generic;
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

            [Serializable]
            public struct BookCard
            {
                public string Title;
                public List<string> Genres;
                public string Description;
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

                var rawBooks = await GetText("books.json");
                var bookPaths = JsonConvert.DeserializeObject<List<string>>(rawBooks);

                foreach (var bookPath in bookPaths) 
                {
                    var image = await GetTexture($"{bookPath}/Image.jpg");
                    var bookCardRaw = await GetText($"{bookPath}/Card.json");
                    var bookCard = JsonConvert.DeserializeObject<BookCard>(bookCardRaw);

                    _ctx.Data.BooksScreen.AddBook(image, bookCard.Title, bookCard.Genres, bookCard.Description);
                }

                await _loadingScreen.Hide();
            }

            private async UniTask<string> GetText(string localPath)
            {
                using var request = UnityWebRequest.Get(GetPath(localPath));

                SetHeaders(request);

                await request.SendWebRequest();

                return request.downloadHandler.text;
            }

            private async UniTask<Texture2D> GetTexture(string localPath)
            {
                using var request = UnityWebRequestTexture.GetTexture(GetPath(localPath));

                SetHeaders(request);

                await request.SendWebRequest();

                return DownloadHandlerTexture.GetContent(request);
            }

            private string GetPath(string localPath)
            {
                return $"{Application.streamingAssetsPath}/Books/{localPath}";
            }

            private void SetHeaders(UnityWebRequest request)
            {
                request.SetRequestHeader("Access-Control-Allow-Credentials", "true");
                request.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
                request.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            }
        }

        [Serializable]
        private struct Data
        {
            [SerializeField] private LoadingScreen.LoadingScreen.Data _loadingScreenData;
            [SerializeField] private UI.BooksScreen _booksScreen;

            public readonly LoadingScreen.LoadingScreen.Data LoadingScreenData => _loadingScreenData;
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

