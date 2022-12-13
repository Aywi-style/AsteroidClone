using UnityEngine;

[CreateAssetMenu(fileName = "UFOsConfig", menuName = "Configs/UFOsConfig", order = 0)]
public class UFOsConfig : ScriptableObject
{
    [field: SerializeField]
    public int SpawnCooldown { get; private set; }

    [field: Space]
    [field: SerializeField]
    public int ScoresCount { get; private set; }

    [field: Header("Ship Info")]
    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    [field: SerializeField]
    public float StartSpeed { get; private set; }

    [field: SerializeField]
    public float AccelerationValue { get; private set; }

    [field: SerializeField]
    public float DecelerationPercent { get; private set; }

    [field: SerializeField]
    public float RotateAngle { get; private set; }
}
