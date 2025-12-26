namespace IdentificationSystem
{
    using System.Collections.Generic;

    public sealed class ItemsIdentificationSystem : IIdentificationSystem
    {
        private readonly HashSet<int> _items = new();
        private int _lastIssued = default; 
    
        public bool Registry(out int id)
        {
            id = 0;
            for (int i = 1; i <= _lastIssued; i++)
            {
                if (!_items.Contains(i))
                {
                    id = i;
                    _items.Add(id);
                    return true;
                }
            }
            _lastIssued++;
            id = _lastIssued;
            _items.Add(id);
            return true;
        }
        public void UnRegistry(int id)
        {
            if (_items.Contains(id))
            {
                _items.Remove(id);
            }
        }
    }

    public interface IIdentificationSystem
    {
        public bool Registry(out int id);
        public void UnRegistry(int id);
    }
}