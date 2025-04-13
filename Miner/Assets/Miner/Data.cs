using System;
using UnityEngine;

namespace Miner 
{
    [Serializable]
    internal struct Data
    {
        [SerializeField] private Loading.Data _loadingScreenData;

        public readonly Loading.Data LoadingScreenData => _loadingScreenData;
    }
}
