using System;
using UnityEngine;

namespace Miner.Loading 
{
    [Serializable]
    public struct Data
    {
        [SerializeField] private View.Screen _screen;

        public readonly View.IScreen Screen => _screen; 
    }
}
