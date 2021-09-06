 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEditor;
 using NaughtyAttributes;
 #if UNITY_EDITOR
 public class MirrorGameObjects : MonoBehaviour
 {
     [SerializeField]
     private Transform objectsToMirrorParent;
     [SerializeField][Label("Mirrored objects parent")]
     private Transform slaveParent;
     [SerializeField][ReadOnly]
     private List <Transform> objsToMirror = new List<Transform>();

    [SerializeField]
     private bool useGrandChildren;
     private Plane mirrorPlane;
 
     private Transform[] masterObjs;
     private Transform[] slaveObjs;
 
     // Use this for initialization
     [Button]
     public void ClearMirrored()
     {
         if(slaveParent != null) 
         {
            for(int i = slaveParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(slaveParent.GetChild(i).gameObject);
            }
         }
     }
     [Button]
     public void Mirror ()
     {
         if(slaveParent != null) 
         {
            for(int i = slaveParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(slaveParent.GetChild(i).gameObject);
            }
         }
         if(slaveParent == null)
         {
            slaveParent = new GameObject().transform;
            slaveParent.position = objectsToMirrorParent.transform.position;
            slaveParent.parent = objectsToMirrorParent.transform.parent;
            slaveParent.name = objectsToMirrorParent.gameObject.name +" Mirrored";
         }
         objsToMirror.Clear();
         foreach(Transform child in objectsToMirrorParent.transform)
         {
             if(useGrandChildren)
                {
                    foreach(Transform grandChild in child)
                    {
                        if(grandChild.position.z != 0) objsToMirror.Add(grandChild); 
                    }   
                }else 
                {
                    if(child.position.z != 0) objsToMirror.Add(child); 
                }
         }
         mirrorPlane = new Plane(slaveParent.forward, slaveParent.position);
         CreateSlaves();
         MirrorObjects();
     }
     void CreateSlaves()
     {
         if (!slaveParent)
             slaveParent = objectsToMirrorParent.transform;
         if (objsToMirror.Count < 1)
         {
             foreach (Transform child in objectsToMirrorParent.transform)
             {
                if(useGrandChildren)
                {
                    foreach(Transform grandChild in child)
                    {
                        if(grandChild.position.z != 0) objsToMirror.Add(grandChild); 
                    }   
                }else 
                {
                    if(child.position.z != 0) objsToMirror.Add(child); 
                }
            }   
         }
             
         masterObjs = objsToMirror.ToArray();
         slaveObjs = new Transform[masterObjs.Length];
         for (int i = 0; i < objsToMirror.Count; i++)
         {
             if(PrefabUtility.GetPrefabInstanceStatus(masterObjs[i].gameObject) == PrefabInstanceStatus.NotAPrefab)
             {
                slaveObjs[i] = Instantiate(masterObjs[i],slaveParent);
             }else
             {
                GameObject go = PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(masterObjs[i].gameObject) as GameObject,slaveParent) as GameObject;
                slaveObjs[i] = go.transform; 
                slaveObjs[i].name = masterObjs[i].name + " Mirrored";
             }
         }
         
     }

     void MirrorObjects()
     {
        Vector3 closestPoint;
        float distanceToMirror;
        Vector3 mirrorPos;
        for (int i = 0; i < objsToMirror.Count; i++)
        {
            if(masterObjs[i].localPosition.z != 0)
            {
            
                closestPoint = mirrorPlane.ClosestPointOnPlane(masterObjs[i].position);
                distanceToMirror = mirrorPlane.GetDistanceToPoint(masterObjs[i].position);

                mirrorPos = closestPoint - mirrorPlane.normal * distanceToMirror;
                slaveObjs[i].position = mirrorPos;
                slaveObjs[i].rotation = ReflectRotation(masterObjs[i].rotation, mirrorPlane.normal);
            }
        }
     }
    private Quaternion ReflectRotation(Quaternion source, Vector3 normal)
    {
        return Quaternion.LookRotation(Vector3.Reflect(source * Vector3.forward, normal), Vector3.Reflect(source * Vector3.up, normal));
    }
 
 
     public void SwitchMaster()
     {
         Transform[] temp = masterObjs;
         masterObjs = slaveObjs;
         slaveObjs = temp;
     }
 }
 #endif