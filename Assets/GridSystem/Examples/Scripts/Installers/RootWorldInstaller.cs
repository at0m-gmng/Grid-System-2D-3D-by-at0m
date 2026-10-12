namespace GridSystem.Installers
{
    using CreatedSystem;
    using CreatedSystem.Data;
    using DragDropSystem.View;
    using Factories;
    using UnityEngine;
    using Zenject;

    public sealed class RootWorldInstaller : MonoInstaller
    {
        [SerializeField]
        private Transform _poolParetn;

        [SerializeField]
        private SpawnSystemArguments _spawnSystemArguments;

        [Header("UI Prefabs")]
        [SerializeField]
        private Transform _itemViewPrefab;

        public override void InstallBindings()
        {
            Container.BindFactory<DraggableData, WorldDraggableObject, WorldDraggableFactory>()
                .FromMonoPoolableMemoryPool(x => x.WithInitialSize(20)
                    .FromComponentInNewPrefab(_itemViewPrefab)
                    .UnderTransform(_poolParetn));

            Container.BindInterfacesTo<WorldSpawnSystem>().AsSingle().WithArguments(_spawnSystemArguments);
        }
    }
}