namespace GridSystem.GridView
{
    using InventoryPresenters;
    using UnityEngine;

    public abstract class BaseGridView : MonoBehaviour
    {
        public IInventoryPresenter InventoryPresenter => presenter;

        protected IInventoryPresenter presenter;
    }
}