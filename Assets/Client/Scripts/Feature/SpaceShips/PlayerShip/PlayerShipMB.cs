using UnityEngine;

public class PlayerShipMB : EcsInfoMB
{
    [field: SerializeField]
    public TrailRenderer[] EnginesTrailRenderer { get; private set; }
}
