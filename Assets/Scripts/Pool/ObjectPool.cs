using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly Queue<T> _pool;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        _pool = new Queue<T>();

        for (int i = 0; i < initialSize; i++)
        {
            T instance = GameObject.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            _pool.Enqueue(instance);
        }
    }

    public T GetFromPool()
    {
        if (_pool.Count > 0)
        {
            T instance = _pool.Dequeue();
            instance.gameObject.SetActive(true);
            return instance;
        }
        else
        {
            T instance = GameObject.Instantiate(_prefab, _parent);
            return instance;
        }
    }

    public void ReturnToPool(T instance)
    {
        instance.gameObject.SetActive(false);
        _pool.Enqueue(instance);
    }
}
