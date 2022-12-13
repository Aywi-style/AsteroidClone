using UnityEngine;

[CreateAssetMenu(fileName = "AllPools", menuName = "Pools/AllPools")]
public class AllPools : ScriptableObject
{
    public Pool Bullets;
    public Pool LaserEffects;
    public Pool[] Asteroids;
    public Pool UFOs;
}
