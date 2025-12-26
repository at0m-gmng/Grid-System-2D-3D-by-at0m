namespace OverlappedSystem.Data
{
    using UnityEngine;

    public struct OverlapEventData
    {
        public OverlapEventData(Vector3 position, int gameObjectID)
        {
            Position = position;
            GameObjectID = gameObjectID;
        }

        public Vector3 Position;
        public int GameObjectID;
    }
}