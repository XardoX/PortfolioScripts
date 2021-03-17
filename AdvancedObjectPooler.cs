using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ObjectPooler : MonoBehaviour 
{
	public static ObjectPooler Instance;
	public ObjectsToPool[] objectsToPool;
	

	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
		foreach(ObjectsToPool o in objectsToPool)
		{
			for(int j = 0; j < o.listOfObjects.Length; j++)
			{
			 	o.listOfObjects[j].pooledObjects = new List<GameObject> ();
				for (int i = 0; i < o.listOfObjects[j].amountToPool; i++) 
				{
					GameObject obj = (GameObject)Instantiate (o.listOfObjects[j].prefab);
					obj.SetActive (false);
					o.listOfObjects[j].pooledObjects.Add (obj);
					obj.transform.parent = o.objectsParent.transform;
					obj.transform.position = o.objectsParent.transform.position;
			}
			}
		}
        
	}
	public GameObject GetPooledObject(int objectID)
	{
		return GetPooledObject(objectID, 0);
	}

	public GameObject GetPooledObject(int objectID, int localID)
	{
		for (int i = 0; i < objectsToPool[objectID].listOfObjects[localID].pooledObjects.Count; i++) 
		{
			if (!objectsToPool[objectID].listOfObjects[localID].pooledObjects[i].activeInHierarchy) 
			{
				return objectsToPool[objectID].listOfObjects[localID].pooledObjects[i];
			}
		}
		Debug.Log("Object pooler returned null");
		return null;
	}

	public void ResetObjects(int objectID)
	{
		foreach(ListOfObjects list in objectsToPool[objectID].listOfObjects)
		{
			foreach(GameObject pooledObject in list.pooledObjects)
			{
				pooledObject.transform.parent = objectsToPool[objectID].objectsParent.transform;
				pooledObject.transform.localPosition = Vector3.zero;
				pooledObject.SetActive(false);
			}
		}
	}
}
[System.Serializable]
public class ObjectsToPool
{
	public ListOfObjects[] listOfObjects;
	[Required][AllowNesting]
	public GameObject objectsParent;
}
[System.Serializable]
public class ListOfObjects
{
	public string name;
	public GameObject prefab;
	[MinValue(0)] [AllowNesting]
	public int amountToPool;
	[MinValue(0)] [AllowNesting]
	[ReadOnly]
	public List<GameObject> pooledObjects;
}