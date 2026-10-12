namespace GridSystem.Installers
{
    using CreatedSystem;
    using CreatedSystem.Data;
    using DragDropSystem.View;
    using Factories;
    using UnityEngine;
    using Zenject;

    public sealed class RootWindowInstaller : MonoInstaller
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
            Container.BindFactory<DraggableData, UIDraggableObject, UIDraggableFactory>()
                .FromMonoPoolableMemoryPool(x => x.WithInitialSize(20)
                    .FromComponentInNewPrefab(_itemViewPrefab)
                    .UnderTransform(_poolParetn));

            Container.BindInterfacesTo<UISpawnSystem>().AsSingle().WithArguments(_spawnSystemArguments);
        }
    }
}