using UnityEngine;

namespace Client
{
    struct LaserEffectComponent
    {
        public GameObject GameObject;
        public Vector3 FinishPoint;

        public int MaxLifeDuration;
        public int CurrentLifeDuration;
    }
}