using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extrudes a mesh originating at object's 0, 0, 0 and extending upwards.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ScriptBuildingBuilder : MonoBehaviour {
	public Material mat;

	protected MeshRenderer mr;
	protected MeshFilter mf;

	protected Mesh m;
	protected List<Vector3> v = new List<Vector3>();
	protected List<int> t = new List<int>();

	// Use this for initialization
	/// <summary>
	/// Maintains references to vars necessary to extrude a mesh
	/// </summary>
	void Start () {
		
		SetRefs ();

		float h_low = 0f;
		float h_high = Random.Range (0.75f, 1.5f);

		Vector3 footprint = new Vector3 (Random.Range (1f, 3f), 0f, Random.Range (1f, 3f));
		Vector3 init_footprint = footprint;
		bool one_extrusion = true;

		Fill3DTrap (0f, 0f, Vector3.zero, footprint);

		for (int i = 0; i < Random.Range(5, 10); i++) {
			Vector3 footprint_next = (one_extrusion? footprint: new Vector3 (init_footprint.x * Random.Range (0.5f, 1f), 0f, init_footprint.z * Random.Range(0.5f, 1f)));

			BuildOff3DTrap(h_high, footprint_next);

			h_low = h_high;
			h_high = h_high + Random.Range (0.5f, 1.5f);
			footprint = footprint_next;
			one_extrusion = !one_extrusion;
		}

		Fill3DTrap (h_low, h_low + Random.value * Random.value * Random.Range (0.5f, 1.5f), footprint, Vector3.zero);

		Export ();
	}
		
	/// <summary>
	/// Sets critical variable references.
	/// </summary>
	public void SetRefs() {
		mr = GetComponent<MeshRenderer> ();
		mf = GetComponent<MeshFilter> ();
		m = new Mesh ();
		m.subMeshCount = 3;
	}

	// 
	/// <summary>
	/// Extrudes a trapezoid upward from an initial footprint dim1 to a terminal footprint dim2
	/// </summary>
	/// <param name="h1">Initial height wrt height 0 of gameObject</param>
	/// <param name="h2">Terminal height wrt height 0 of gameObject</param></param>
	/// <param name="dim1">Initial rectangle to extrude from</param>
	/// <param name="dim2">Terminal rectangle to extrude to</param>
	public void Fill3DTrap(float h1, float h2, Vector3 dim1, Vector3 dim2) {
		v.AddRange (new Vector3[] {
			
			// bottom rect
			// BA, BB, BC, BD: -8, -7, -6, -5
			new Vector3(dim1.x * 0.5f, h1, dim1.z * 0.5f), new Vector3(dim1.x * -0.5f, h1, dim1.z * 0.5f), 
			new Vector3(dim1.x * -0.5f, h1, dim1.z * -0.5f), new Vector3(dim1.x * 0.5f, h1, dim1.z * -0.5f),

			// top rect
			// TA, TB, TC, TD: -4, -3, -2, -1
			new Vector3(dim2.x * 0.5f, h2, dim2.z * 0.5f), new Vector3(dim2.x * -0.5f, h2, dim2.z * 0.5f), 
			new Vector3(dim2.x * -0.5f, h2, dim2.z * -0.5f), new Vector3(dim2.x * 0.5f, h2, dim2.z * -0.5f)
		});

		t.AddRange (new int[] {
			v.Count - 8, v.Count - 3, v.Count - 7, 
			v.Count - 8, v.Count - 4, v.Count - 3, 

			v.Count - 7, v.Count - 2, v.Count - 6,
			v.Count - 7, v.Count - 3, v.Count - 2,

			v.Count - 6, v.Count - 1, v.Count - 5,
			v.Count - 6, v.Count - 2, v.Count - 1,

			v.Count - 5, v.Count - 1, v.Count - 8,
			v.Count - 8, v.Count - 1, v.Count - 4
		});
	}

	/// <summary>
	/// Assumes a previous rectangle of points already exists and builds off it
	/// </summary>
	/// <param name="h2">Height between previous rectangle and new rectangle</param>
	/// <param name="dim">Dimensions of new rectangle</param>
	public void BuildOff3DTrap(float h2, Vector3 dim) {
		v.AddRange (new Vector3[] {
			// top rect
			// TA, TB, TC, TD: -4, -3, -2, -1
			new Vector3(dim.x * 0.5f, h2, dim.z * 0.5f), new Vector3(dim.x * -0.5f, h2, dim.z * 0.5f), 
			new Vector3(dim.x * -0.5f, h2, dim.z * -0.5f), new Vector3(dim.x * 0.5f, h2, dim.z * -0.5f)
		});

		t.AddRange (new int[] {
			v.Count - 8, v.Count - 3, v.Count - 7, 
			v.Count - 8, v.Count - 4, v.Count - 3, 

			v.Count - 7, v.Count - 2, v.Count - 6,
			v.Count - 7, v.Count - 3, v.Count - 2,

			v.Count - 6, v.Count - 1, v.Count - 5,
			v.Count - 6, v.Count - 2, v.Count - 1,

			v.Count - 5, v.Count - 1, v.Count - 8,
			v.Count - 8, v.Count - 1, v.Count - 4
		});
	}

	public virtual void Export() {
		m.vertices = v.ToArray ();
		m.triangles = t.ToArray ();

		mr.materials = new Material[] { Resources.Load("Materials/MAT_BUILD_TRIM_" + Random.Range (0, 6).ToString()) as Material };

		m.RecalculateNormals ();

		mf.mesh = m;

	}
}