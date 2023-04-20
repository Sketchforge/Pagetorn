using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    [SerializeField] private List<MeshFilter> sourceMeshFilters;
    [SerializeField] private MeshFilter targetMeshFilter;

	private void OnEnable()
	{
        CombineMeshes();
	}

	[Button]
    private void CombineMeshes()
	{
        var combine = new CombineInstance[sourceMeshFilters.Count];

        for (var i = 0; i< sourceMeshFilters.Count; i++)
		{
            combine[i].mesh = sourceMeshFilters[i].sharedMesh;
            combine[i].transform = sourceMeshFilters[i].transform.localToWorldMatrix;
		}

        targetMeshFilter.mesh = new Mesh();
        targetMeshFilter.sharedMesh.CombineMeshes(combine);

        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(1, 0.87f, 1.22f);
        transform.position = Vector3.zero;
	}
}
