using UnityEngine;

namespace Client
{
    struct LaserSpawnEvent
    {
        public int OwnerEntity;
        public Vector3 SpawnPoint;
        public Vector3 Direction;

        /// <summary>
        /// Create laser on Spawn point with current direction
        /// </summary>
        /// <param name="ownerEntity">Whose laser weapon</param>
        /// <param name="spawnPoint">Where create bullet</param>
        /// <param name="direction">In which direction will the bullet fly</param>
        public void Invoke(int ownerEntity, Vector3 spawnPoint, Vector3 direction)
        {
            OwnerEntity = ownerEntity;
            SpawnPoint = spawnPoint;
            Direction = direction;
        }
    }
}