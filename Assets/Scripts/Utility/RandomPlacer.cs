using UnityEngine;
using System.Collections;

public class RandomPlacer : MonoBehaviour
{
    public Vector3 Size;
    public GameObject ObjectToPlace;
    public bool AsChilds;

    public int Count = 10;

    public Vector3 SizeFrom = new Vector3(0.8f, 0.8f, 0.8f);
    public Vector3 SizeTo = new Vector3(1.2f, 1.2f, 1.2f);

    public Vector3 RotationFrom = new Vector3(0, 0, 0);
    public Vector3 RotationTo = new Vector3(0, 180, 0);

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
            var scale = RandomComponentVectorInRange(SizeFrom, SizeTo);
            var rot = RandomComponentVectorInRange(RotationFrom, RotationTo);

            var go = (GameObject)Instantiate(ObjectToPlace, pos, Quaternion.Euler(rot));
            go.transform.localScale = scale;

            if (AsChilds)
            {
                go.transform.SetParent(transform, true);   
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Size);
    }

    Vector3 RandomComponentVectorInRange(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }
}
