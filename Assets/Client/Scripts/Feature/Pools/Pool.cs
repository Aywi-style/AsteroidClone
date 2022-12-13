using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPool", menuName = "Pools/NewPool")]
[System.Serializable]
public class Pool : ScriptableObject
{
    public int TotalObjectsCount { get; private set; }
    public int CurrentObjectsCount { get; private set; }
    public string ParentName { get; private set; }
    public GameObject Prefab;
    public Queue<GameObject> ObjectPool { get; private set; }
    public Vector3 CreationPoint { get; set; }

    public GameObject Parent { get; private set; }

    public Pool(GameObject prefab, Vector3 creationPoint, int objectsCount, string parentName = null)
    {
        if (objectsCount < 1)
        {
            Debug.LogError("ObjectsCount is less than zero!");
            return;
        }

        if (prefab is null)
        {
            Debug.LogError("The pool did not have a prefab!");
            return;
        }

        Prefab = prefab;
        ObjectPool = new Queue<GameObject>();
        CreationPoint = creationPoint;
        ParentName = parentName ?? null;

        if (ParentName != null)
        {
            Parent = new GameObject();
            Parent.name = ParentName;
        }

        CreateObjects(objectsCount);
    }

    public virtual void CreateObjects(int objectsCount)
    {
        for (int i = 0; i < objectsCount; i++)
        {
            var poolObject = GameObject.Instantiate(Prefab, CreationPoint, Quaternion.identity);
            poolObject.SetActive(false);
            ObjectPool.Enqueue(poolObject);

            if (Parent != null)
            {
                poolObject.transform.SetParent(Parent.transform);
            }

            TotalObjectsCount += objectsCount;
            CurrentObjectsCount += objectsCount;
        }
    }

    public virtual GameObject GetFromPool()
    {
        GameObject poolObject;

        if (ObjectPool.Count > 0)
        {
            poolObject = ObjectPool.Dequeue();
            poolObject.SetActive(true);
        }
        else
        {
            CreateObjects(1);
            poolObject = ObjectPool.Dequeue();
            poolObject.SetActive(true);
        }

        CurrentObjectsCount--;

        return poolObject;
    }

    public virtual void ReturnToPool(GameObject gameObject)
    {
        ObjectPool.Enqueue(gameObject);

        CurrentObjectsCount++;
    }
}
