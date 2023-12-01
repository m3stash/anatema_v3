using System.Collections.Generic;
using UnityEngine;

public class Pool<T> : MonoBehaviour where T : MonoBehaviour {
    protected Queue<T> availableObjects = new Queue<T>();
    protected List<T> usedObjects = new List<T>();
    protected T prefab;

    private void Awake() {
        availableObjects = new Queue<T>();
        usedObjects = new List<T>();
    }

    public void Setup(T prefab, int size) {
        this.prefab = prefab;

        for (int i = 0; i < size; i++) {
            T obj = Instantiate(prefab, transform);
            availableObjects.Enqueue(obj);
        }
    }

    public void SetupWithParent(T prefab, int size, Transform transform) {
        this.prefab = prefab;

        for (int i = 0; i < size; i++) {
            T obj = Instantiate(prefab, transform);
            availableObjects.Enqueue(obj);
        }
    }

    public virtual T GetOne() {
        T obj;

        if (availableObjects.Count > 0) {
            obj = availableObjects.Dequeue();
        } else {
            obj = Instantiate(prefab, transform);
        }
        usedObjects.Add(obj);

        return obj;
    }

    public void ReleaseOne(T obj) {
        if (usedObjects.Contains(obj)) {
            usedObjects.Remove(obj);
            availableObjects.Enqueue(obj);
            // Reparent the pooled object to us, and disable it.
            var pooledObjectTransform = obj.transform;
            pooledObjectTransform.SetParent(transform);
            pooledObjectTransform.localPosition = Vector3.zero;
            obj.gameObject.SetActive(false);

        }
    }
}