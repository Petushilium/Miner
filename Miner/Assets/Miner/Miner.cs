using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.Reactive;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Miner
{
    internal sealed class Miner : BaseDisposableMB
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

                //loading here...

                await _loadingScreen.Hide();
            }
        }

        [Serializable]
        private struct Data
        {
            [SerializeField] private LoadingScreen.LoadingScreen.Data _loadingScreenData;

            public readonly LoadingScreen.LoadingScreen.Data LoadingScreenData => _loadingScreenData;
        }

        [SerializeField] private Data _data;
        [SerializeField] private TMP_Text _consoleTextArea;

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

            Application.logMessageReceived += DrawLog;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Application.logMessageReceived -= DrawLog;
        }

        private void DrawLog(string condition, string stackTrace, LogType type) 
        {
            _consoleTextArea.text += $"{condition}\n";
        }

        private void Update()
        {
            _onUpdate.Execute(Time.deltaTime);
        }
    }
}

