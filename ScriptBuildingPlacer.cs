using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiates a grid of objects in the game world
/// </summary>
public class ScriptBuildingPlacer : MonoBehaviour {
	public GameObject prefab_building_builder;

	// Use this for initialization
	void Start () {
		for (int x = 0; x < 5; x++) {
			for (int y = 0; y < 5; y++) {
			PlaceBuildingSingle(x * 10, y * 10);
			}
		}
	}

	/// <summary>
	/// Places a single self-building object
	/// </summary>
	/// <param name="x">local x position</param>
	/// <param name="y">local y position</param>
	void PlaceBuildingSingle(int x, int y) {
		Vector3 center = transform.position + new Vector3 (x, 0f, y);

		GameObject go = Instantiate (prefab_building_builder, center, Quaternion.identity, transform) as GameObject;
	}

}
