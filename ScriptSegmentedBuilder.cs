using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script which builds a mesh according to a specification ("spec") determined in object inspector.
/// Extrudes a mesh originating at object's 0, 0, 0 and extending upwards.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ScriptSegmentedBuilder : ScriptBuildingBuilder {
	public List<BuildRectSpec> specs = new List<BuildRectSpec> ();

	/// <summary>
	/// Command can be issued from editor: right-click script in inspector, then select "Rebuild"
	/// </summary>
	[ContextMenu("Rebuild")]
	public void Rebuild () {
		SetRefs ();

		float h = 0;
		Vector3 inst_dim = Vector3.zero;
		Fill3DTrap (0f, 0f, Vector3.zero, Vector3.zero);

		foreach (BuildRectSpec s in specs) {
			//Fill3DTrap (h, h + s.h, inst_dim, s.dim);
			BuildOff3DTrap(h + s.h, s.dim);
			h = h + s.h;
			inst_dim = s.dim;
		}
		if (inst_dim != Vector3.zero)
			Fill3DTrap (h, h, inst_dim, Vector3.zero);
		
		// Fill3DTrap (0f, 1f, Vector3.zero, new Vector3(Random.value, 0f, Random.value));
		Export ();
	}

	/// <summary>
	/// Script automatically rebuilds mesh whenver a parameter of the spec is changed
	/// </summary>
	void OnValidate() {
		Rebuild ();
	}
}

/// <summary>
/// Defines a vertical extrusion upwards of height h, which terminates in a rectangle of dimensions dim
/// </summary>
[System.Serializable]
public class BuildRectSpec {
	public float h;
	public Vector3 dim;
}