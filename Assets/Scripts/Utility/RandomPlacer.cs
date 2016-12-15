using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomPlacer : MonoBehaviour
{
    public Vector3 Size = new Vector3(10, 10, 0);
    public GameObject ObjectToPlace;
    public bool AsChilds;
    public bool UseRaycast;

    public int Count = 10;

    public Vector3 SizeFrom = new Vector3(0.8f, 0.8f, 0.8f);
    public Vector3 SizeTo = new Vector3(1.2f, 1.2f, 1.2f);

    public Vector3 RotationFrom = new Vector3(0, 0, 0);
    public Vector3 RotationTo = new Vector3(0, 180, 0);

    public Vector3 RaycastDirection = Vector3.down;
    public float RaycastRange = 1f;

#if UNITY_EDITOR
    [ContextMenu("Place object")]
    void Place()
    {
	    if (ObjectToPlace == null || Count == 0)
	    {
	        Debug.LogWarning("Nothing to place");
            return;
	    }

        var a = transform.position - Size / 2;
        var b = transform.position + Size / 2;

        for (var i = 0; i < Count; i++)
        {
            var pos = RandomComponentVectorInRange(a, b);

            if (UseRaycast)
            {
                RaycastHit hit;
                if (Physics.Raycast(pos, transform.TransformDirection(RaycastDirection), out hit, RaycastRange))
                {
                    pos = hit.point;
                }
                else
                {
                    continue;
                }
            }


            var scale = RandomComponentVectorInRange(SizeFrom, SizeTo);
            var rot = RandomComponentVectorInRange(RotationFrom, RotationTo);
            pos += new Vector3(0, scale.y * 0.5f, 0);

            
            var go = (GameObject)PrefabUtility.InstantiatePrefab(ObjectToPlace);
            go.transform.position = pos;
            go.transform.localRotation = Quaternion.Euler(rot);
            go.transform.localScale = scale;

            if (AsChilds)
            {
                go.transform.SetParent(transform, true);   
            }
        }
    }
#endif

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Size);
    }

    Vector3 RandomComponentVectorInRange(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }
}
