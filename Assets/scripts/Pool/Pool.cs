using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> : MonoBehaviour where T : MonoBehaviour {
    [SerializeField] protected List<T> availableObjects;
    [SerializeField] protected List<T> usedObjects;

    protected T prefab;

    private void Awake() {
        this.availableObjects = new List<T>();
        this.usedObjects = new List<T>();
    }

    public void Setup(T prefab, int size) {
        this.prefab = prefab;
        for (int i = 1; i < size; i++) {
            T obj = Instantiate(prefab, transform);
            // obj.gameObject.SetActive(false);
            availableObjects.Add(obj);
        }
    }

    /*
     * Get one pool object from availableObjects or create new instance if pull is full
     */
    public virtual T GetOne() {
        T obj = null;

        if (availableObjects.Count > 0) {
            obj = availableObjects[availableObjects.Count - 1];
            availableObjects.RemoveAt(availableObjects.Count - 1);
        } else {
            obj = Instantiate(prefab, transform);
        }

        usedObjects.Add(obj);

        return obj;
    }

    /*
     *  Remove object from usedObjects and add it to Available
     */
    public void DeleteObjectFromActivePool(T pooledObject) {
        usedObjects.Remove(pooledObject);
        availableObjects.Add(pooledObject);
        var pooledObjectTransform = pooledObject.transform;
        pooledObjectTransform.parent = transform;
        pooledObjectTransform.localPosition = Vector3.zero;
        pooledObject.gameObject.SetActive(false);
    }

}
