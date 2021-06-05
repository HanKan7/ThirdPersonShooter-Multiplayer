    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMeshCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var allDescendents = GetComponentsInChildren<MeshRenderer>();
        foreach (var t in allDescendents)
        {
            t.gameObject.AddComponent<MeshCollider>();
        }
    }
}
