using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcsInfoMB : MonoBehaviour, IEcsInfo
{
    [SerializeField]
    private int _entity;

    public void Init(int entity)
    {
        _entity = entity;
    }

    public int GetEntity()
    {
        return _entity;
    }
}
