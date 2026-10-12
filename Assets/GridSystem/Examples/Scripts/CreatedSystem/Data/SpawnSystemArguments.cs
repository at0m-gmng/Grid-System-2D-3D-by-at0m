namespace GridSystem.CreatedSystem.Data
{
    using System;
    using UnityEngine;

    [Serializable]
    public sealed class SpawnSystemArguments
    {
        [field: SerializeField] public Transform TemporatyTransform { get; private set; }
    }
}