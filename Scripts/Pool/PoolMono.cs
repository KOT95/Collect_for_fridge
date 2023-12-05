using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class PoolMono<T> where T : MonoBehaviour
{
    public List<T> Prefabs { get; } = new List<T>();
    public bool AutoExpand { get; set; }
    public Transform Container { get; }

    private List<T> _pool = new List<T>();

    public PoolMono(T prefab, int count)
    {
        this.Prefabs.Add(prefab);
        this.Container = null;
        
        this.CreatePool(count);
    }

    public PoolMono(T[] prefabs, int count, Transform container)
    {
        this.Container = container;
        foreach (var t in prefabs)
        {
            this.Prefabs.Add(t);
            this.CreatePool(count);
        }
    }

    private void CreatePool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            this.CreateObject();
        }
    }

    private T CreateObject(bool isActiveByDefault = false)
    {
        var createdObject = Object.Instantiate(this.Prefabs[Prefabs.Count - 1], this.Container);
        createdObject.gameObject.SetActive(isActiveByDefault);
        this._pool.Add(createdObject);
        return createdObject;
    }

    private T CreateObjectType(bool isActiveByDefault, TypeObject type)
    {
        for (int i = 0; i < Prefabs.Count; i++)
        {
            if (Prefabs[i].GetComponent<ObjectItem>().Type == type)
            {
                var createdObject = Object.Instantiate(this.Prefabs[i], this.Container);
                createdObject.gameObject.SetActive(isActiveByDefault);
                this._pool.Add(createdObject);
                return createdObject;
            }
        }

        return null;
    }

    private bool HasFreeElement(out T element, TypeObject type)
    {
        foreach (var mono in _pool)
        {
            if (!mono.gameObject.activeInHierarchy && mono.GetComponent<ObjectItem>().Type == type)
            {
                element = mono;
                return true;
            }
        }

        element = null;
        return false;
    }

    public T GetFreeElement(TypeObject type)
    {
        if (this.HasFreeElement(out var element, type)) return element;

        if (this.AutoExpand) return this.CreateObjectType(false, type);

        throw new Exception($"There is no free elements in pool of type {typeof(T)}");
    }
}
