using UnityEngine;

namespace Client
{
    struct BulletSpawnEvent
    {
        public float MaxLifeDuration;
        public Vector3 SpawnPoint;
        public Vector3 Direction;
        public float Speed;

        /// <summary>
        /// Create bullet on Spawn point with current direction, speed and life duration
        /// </summary>
        /// <param name="maxLifeDuration">How long bullet will be live</param>
        /// <param name="spawnPoint">Where create bullet</param>
        /// <param name="direction">In which direction will the bullet fly</param>
        /// <param name="speed">How fast will the bullet fly</param>
        public void Invoke(float maxLifeDuration, Vector3 spawnPoint, Vector3 direction, float speed)
        {
            MaxLifeDuration = maxLifeDuration;
            SpawnPoint = spawnPoint;
            Direction = direction;
            Speed = speed;
        }
    }
}