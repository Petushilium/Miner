using System;
using UnityEngine;

namespace Miner 
{
    [Serializable]
    internal struct Data
    {
        [SerializeField] private LoadingScreen.LoadingScreen.Data _loadingScreenData;

        public readonly LoadingScreen.LoadingScreen.Data LoadingScreenData => _loadingScreenData;
    }
}
