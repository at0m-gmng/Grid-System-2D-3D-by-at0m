namespace GridSystem.Runtime.Views
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Keyed by prefab pool of item views.
    /// </summary>
    public sealed class ItemViewPool
    {
        public ItemViewPool(Transform root)
        {
            _root = root;
        }

        private readonly Transform _root;
        private readonly Dictionary<ItemView, Stack<ItemView>> _free = new();

        /// <summary>
        /// Returns active view instance attached to the parent.
        /// </summary>
        public ItemView Rent(ItemView prefab, Transform parent)
        {
            ItemView view;
            if (GetStack(prefab).TryPop(out view))
            {
                view.transform.SetParent(parent, false);
            }
            else
            {
                view = Object.Instantiate(prefab, parent);
                view.Prefab = prefab;
            }

            view.gameObject.SetActive(true);
            return view;
        }

        /// <summary>
        /// Deactivates the view and stores it for reuse.
        /// </summary>
        public void Return(ItemView view)
        {
            view.Release();
            view.gameObject.SetActive(false);
            view.transform.SetParent(_root, false);
            GetStack(view.Prefab).Push(view);
        }

        /// <summary>
        /// Creates inactive instances in advance.
        /// </summary>
        public void Prewarm(ItemView prefab, int count)
        {
            Stack<ItemView> stack = GetStack(prefab);
            for (int i = 0; i < count; i++)
            {
                ItemView view = Object.Instantiate(prefab, _root);
                view.Prefab = prefab;
                view.gameObject.SetActive(false);
                stack.Push(view);
            }
        }

        private Stack<ItemView> GetStack(ItemView prefab)
        {
            Stack<ItemView> stack;
            if (!_free.TryGetValue(prefab, out stack))
            {
                stack = new Stack<ItemView>();
                _free.Add(prefab, stack);
            }

            return stack;
        }
    }
}
