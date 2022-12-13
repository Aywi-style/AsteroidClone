using UnityEngine;
using System;
using Client;

[CreateAssetMenu(fileName = "AsteroidsConfig", menuName = "Configs/AsteroidsConfig", order = 0)]
public class AsteroidsConfig : ScriptableObject
{
    [field: SerializeField]
    public int SpawnCooldown { get; private set; }

    [field: SerializeField]
    public Asteroid[] Asteroids { get; private set; }
}

[Serializable]
public class Asteroid
{
    [field: SerializeField]
    public AsteroidType AsteroidType { get; private set; }

    [field: SerializeField]
    public int StartSpeed { get; private set; }

    [field: SerializeField]
    public int ScoresCount { get; private set; }

    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}
