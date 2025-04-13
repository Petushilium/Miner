using Cysharp.Threading.Tasks;
using Shared.Disposable;
using UniRx;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Miner
{
    internal sealed class EntryPoint : MonoBehaviour
    {
        [SerializeField] private Data _data;

        private ReactiveCommand<float> _onUpdate;
        private Entity _entity;

        private void OnEnable()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopHelper.Initialize(ref playerLoop);

            _onUpdate = new ReactiveCommand<float>().AddTo(this);
            _entity = new Entity(new Entity.Ctx
            {
                OnUpdate = _onUpdate,
                Data = _data,
            }).AddTo(this);
            _entity.AsyncInit();
        }

        private void OnDisable()
        {
            _entity?.Dispose();
            _onUpdate?.Dispose();
        }

        private void Update()
        {
            _onUpdate.Execute(Time.deltaTime);
        }
    }
}

