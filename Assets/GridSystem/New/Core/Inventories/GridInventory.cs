namespace GridSystem.Core.Inventories
{
    using System;
    using System.Collections.Generic;
    using Items;
    using Model;
    using R3;
    using Shapes;
    using UnityEngine;

    /// <summary>
    /// Grid container of item instances. The only owner of item positions.
    /// </summary>
    public sealed class GridInventory : IDisposable
    {
        public GridInventory(int id, string key, int width, int height)
        {
            Id = id;
            Key = key;
            Model = new GridModel(width, height);
        }

        private readonly Dictionary<int, GridItem> _items = new();
        private readonly List<GridItem> _buffer = new();
        private readonly Subject<GridItem> _added = new();
        private readonly Subject<GridItem> _removed = new();
        private readonly Subject<ItemMovement> _moved = new();

        /// <summary>
        /// Runtime unique identifier.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Persistent key used for saving.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Occupancy matrix.
        /// </summary>
        public GridModel Model { get; }

        /// <summary>
        /// Raised after an item is placed.
        /// </summary>
        public Observable<GridItem> Added => _added;

        /// <summary>
        /// Raised after an item is removed.
        /// </summary>
        public Observable<GridItem> Removed => _removed;

        /// <summary>
        /// Raised after an item is moved or rotated.
        /// </summary>
        public Observable<ItemMovement> Moved => _moved;

        /// <summary>
        /// Placed items. Struct enumerator, no allocations.
        /// </summary>
        public Dictionary<int, GridItem>.ValueCollection Items => _items.Values;

        /// <summary>
        /// Placed items count.
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Returns true when the item is placed in this inventory.
        /// </summary>
        public bool Contains(GridItem item) => _items.ContainsKey(item.Id);

        /// <summary>
        /// Finds placed item by id.
        /// </summary>
        public bool TryGetItem(int id, out GridItem item) => _items.TryGetValue(id, out item);

        /// <summary>
        /// Finds item occupying the cell. The cell may be outside of the grid.
        /// </summary>
        public bool TryGetItemAt(Vector2Int cell, out GridItem item)
        {
            item = null;
            bool result = Model.Contains(cell) && _items.TryGetValue(Model.GetOwner(cell), out item);
            return result;
        }

        /// <summary>
        /// Checks placement of the definition, cells of ignored ids are treated as free.
        /// </summary>
        public bool CanPlace(IGridItemDefinition definition, Vector2Int origin, int rotation, int ignoredA, int ignoredB)
            => Model.CanPlace(definition.Shape.GetCells(ResolveRotation(definition, rotation)), origin, ignoredA, ignoredB);

        /// <summary>
        /// Checks placement of the item, its own cells are treated as free.
        /// </summary>
        public bool CanPlace(GridItem item, Vector2Int origin, int rotation) => CanPlace(item.Definition, origin, rotation, item.Id, GridModel.EMPTY);

        /// <summary>
        /// Finds the first free placement scanning rotations, rows and columns.
        /// </summary>
        public bool TryFindPlacement(IGridItemDefinition definition, out Vector2Int origin, out int rotation)
        {
            bool isFound = false;
            origin = default;
            rotation = 0;
            int rotations = definition.IsRotatable ? GridShape.ROTATION_COUNT : 1;
            for (int r = 0; !isFound && r < rotations; r++)
            {
                ReadOnlySpan<Vector2Int> cells = definition.Shape.GetCells(r);
                for (int y = 0; !isFound && y < Model.Height; y++)
                {
                    for (int x = 0; !isFound && x < Model.Width; x++)
                    {
                        Vector2Int candidate = new Vector2Int(x, y);
                        if (Model.CanPlace(cells, candidate, GridModel.EMPTY, GridModel.EMPTY))
                        {
                            isFound = true;
                            origin = candidate;
                            rotation = r;
                        }
                    }
                }
            }

            return isFound;
        }

        /// <summary>
        /// Places a new item at the given position.
        /// </summary>
        public bool TryAdd(GridItem item, Vector2Int origin, int rotation)
        {
            int resolvedRotation = ResolveRotation(item.Definition, rotation);
            bool result = !_items.ContainsKey(item.Id)
                && Model.CanPlace(item.Definition.Shape.GetCells(resolvedRotation), origin, GridModel.EMPTY, GridModel.EMPTY);
            if (result)
            {
                item.Origin = origin;
                item.Rotation = resolvedRotation;
                Model.Fill(item.Cells, origin, item.Id);
                _items.Add(item.Id, item);
                _added.OnNext(item);
            }

            return result;
        }

        /// <summary>
        /// Places a new item at the first free position.
        /// </summary>
        public bool TryAdd(GridItem item)
        {
            bool result = TryFindPlacement(item.Definition, out Vector2Int origin, out int rotation) && TryAdd(item, origin, rotation);
            return result;
        }

        /// <summary>
        /// Moves or rotates a placed item.
        /// </summary>
        public bool TryMove(GridItem item, Vector2Int origin, int rotation)
        {
            int resolvedRotation = ResolveRotation(item.Definition, rotation);
            bool result = _items.ContainsKey(item.Id)
                && Model.CanPlace(item.Definition.Shape.GetCells(resolvedRotation), origin, item.Id, GridModel.EMPTY);
            if (result)
            {
                Vector2Int previousOrigin = item.Origin;
                int previousRotation = item.Rotation;
                Model.Fill(item.Cells, previousOrigin, GridModel.EMPTY);
                item.Origin = origin;
                item.Rotation = resolvedRotation;
                Model.Fill(item.Cells, origin, item.Id);
                _moved.OnNext(new ItemMovement(item, previousOrigin, previousRotation));
            }

            return result;
        }

        /// <summary>
        /// Removes a placed item.
        /// </summary>
        public bool TryRemove(GridItem item)
        {
            bool result = _items.Remove(item.Id);
            if (result)
            {
                Model.Fill(item.Cells, item.Origin, GridModel.EMPTY);
                _removed.OnNext(item);
            }

            return result;
        }

        /// <summary>
        /// Removes every item. Events are raised after the inventory is cleared.
        /// </summary>
        public void Clear()
        {
            _buffer.Clear();
            _buffer.AddRange(_items.Values);
            _items.Clear();
            Model.Clear();
            for (int i = 0; i < _buffer.Count; i++)
            {
                _removed.OnNext(_buffer[i]);
            }

            _buffer.Clear();
        }

        /// <summary>
        /// Completes change streams.
        /// </summary>
        public void Dispose()
        {
            _added.Dispose();
            _removed.Dispose();
            _moved.Dispose();
        }

        private static int ResolveRotation(IGridItemDefinition definition, int rotation) => definition.IsRotatable ? GridShape.NormalizeRotation(rotation) : 0;
    }
}
