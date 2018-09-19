using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extrudes multiple adjacent meshes in a rectangular shape
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ScriptMultiBuilding : ScriptBuildingBuilder {

	private const int dim_x = 25;
	private const int dim_z = 25;

	private BuildingOccupationRecord[,] occs = new BuildingOccupationRecord[dim_x, dim_z];

	// Use this for initialization
	[ContextMenu("Rebuild")]
	void Start () {
		Reserve ();

		BuildTrapsFromOccupationRecords ();
	}

	void BuildTrapsFromOccupationRecords() {
		SetRefs ();

		HashSet<BuildingOccupationRecord> hits = new HashSet<BuildingOccupationRecord> ();

		for (int i = 0; i < dim_x; i++) {
			for (int j = 0; j < dim_z; j++) {
				BuildingOccupationRecord bor = occs [i, j];
				if (hits.Contains (bor) || bor == null)
					continue;
				hits.Add (bor);
				// change to pass in bor
				Fill3DSpace (bor);
			}
		}

		Export ();
	}

	private void Reserve() {
		int _x = Random.Range (1, dim_x / 2);
		int _xend = Random.Range (dim_x / 2, dim_x);
		int _z = Random.Range (1, dim_z / 2);
		int _zend = Random.Range (dim_z / 2, dim_z);
		BuildingOccupationRecord bor = new BuildingOccupationRecord (){ x = _x, xend = _xend, z = _z, zend = _zend, h1 = 0, h2 = Mathf.Sqrt(_xend - _x + _zend - _z)};

		AttemptAssignRecord (bor);
	}

	/// <summary>
	/// Checks validity of a building occupation record, and also tries to recurse new buildings if possible
	/// </summary>
	/// <returns><c>true</c>, if a space could be allocated for this building, <c>false</c> otherwise.</returns>
	/// <param name="bor">Bor. A building occupation record which reserves space for itself in an area that a mesh will build in</param>
	bool AttemptAssignRecord(BuildingOccupationRecord bor) {
		// is it in bounds?
		if (bor.x < 0 || bor.xend > dim_x || bor.z < 0 || bor.zend > dim_z) {
			return false;
		}

		// is it occupied?
		for (int x = bor.x; x < bor.xend; x++) {
			for (int z = bor.z; z < bor.zend; z++) {
				if (occs [x, z] != null) {
					return false;
				}
			}
		}

		// is it a small greeble? reject
		if (Mathf.Abs(bor.xend - bor.x) < 2 || Mathf.Abs(bor.zend - bor.z) < 2)
			return false;

		// assign it in
		for (int x = bor.x; x < bor.xend; x++) {
			for (int z = bor.z; z < bor.zend; z++) {
				occs [x, z] = bor;
			}
		}

		// recurse
		for (int i = 0; i < 10; i++) {
			Propagate (bor);
		}
		return(true);
	}

	/// <summary>
	/// Creates a building record adjacent to current building
	/// </summary>
	/// <param name="prev_occ">Previous building record to be built off of</param>
	private void Propagate(BuildingOccupationRecord prev_occ) {
		int d = Random.Range (0, 4);

		BuildingOccupationRecord bor = new BuildingOccupationRecord() {h1 = 0f, h2 = prev_occ.h2 / Random.Range(1.5f, 3f)};

		if (bor.h2 < 1)
			return;

		switch (d) {
		case 0:
			// left: from x - r to x
			bor.x = prev_occ.x - Random.Range (3, 10);
			bor.xend = prev_occ.x;

			// lateral: y and yend span from y + r: yend - r
			if (Random.value < 0.5f) {
				bor.z = prev_occ.z;
				bor.zend = (int) (prev_occ.zend - Random.Range (2f, bor.zend - bor.z));
			} else {
				bor.z = (int) (prev_occ.z + Random.Range(2f, bor.zend - bor.z));
				bor.zend = prev_occ.zend;
			}
			break;
		case 1:
			// right
			// from xend to xend + r
			bor.x = prev_occ.xend;
			bor.xend = prev_occ.xend + Random.Range(3,10);

			// lateral: y and yend span from y + r: yend - r
			if (Random.value < 0.5f) {
				bor.z = prev_occ.z;
				bor.zend = (int) (prev_occ.zend - Random.Range (2f, bor.zend - bor.z));
			} else {
				bor.z = (int) (prev_occ.z + Random.Range(2f, bor.zend - bor.z));
				bor.zend = prev_occ.zend;
			}
			break;
		case 2:
			// up
			// from z - r to z
			bor.z = prev_occ.z - Random.Range (3, 10);
			bor.zend = prev_occ.z;

			// vertical: x and xend span from x + r: xend - r
			if (Random.value < 0.5f) {
				bor.x = prev_occ.x;
				bor.xend = (int) (prev_occ.xend - Random.Range (2f, bor.xend - bor.x));
			} else {
				bor.x = (int) (prev_occ.x + Random.Range(2f, bor.xend - bor.x));
				bor.xend = prev_occ.xend;
			}

			break;
		case 3:
			// down
			// from zend to zend + r
			bor.z = prev_occ.zend;
			bor.zend = prev_occ.zend + Random.Range (3, 10);

			// vertical: x and xend span from x + r: xend - r
			if (Random.value < 0.5f) {
				bor.x = prev_occ.x;
				bor.xend = (int) (prev_occ.xend - Random.Range (2f, bor.xend - bor.x));
			} else {
				bor.x = (int) (prev_occ.x + Random.Range(2f, bor.xend - bor.x));
				bor.xend = prev_occ.xend;
			}

			break;
		}

		AttemptAssignRecord (bor).ToString ();
		
		return;
	}

	/// <summary>
	/// Fills 3d space occupied by a BuildingOccupationRecord
	/// </summary>
	/// <param name="bor">Building Occupation Record describing x, height, and z span of a building</param>
	public void Fill3DSpace(BuildingOccupationRecord bor) {
		float x = Mathf.Min(bor.x, bor.xend);
		float xend = Mathf.Max(bor.x, bor.xend);
		float z = Mathf.Min(bor.z, bor.zend);
		float zend = Mathf.Max(bor.z, bor.zend);
		float h1 = bor.h1;
		float h2 = bor.h2;

		if (bor.x == bor.xend || bor.z == bor.zend)
			return;
		// greebles skip check in AttemptAssignRecord so include a check here for dimensionality

		v.AddRange (new Vector3[] {

			// bottom rect
			// BA, BB, BC, BD: -8, -7, -6, -5
			new Vector3(xend, h1, zend), new Vector3(x, h1, zend), 
			new Vector3(x, h1, z), new Vector3(xend, h1, z),

			// top rect
			// TA, TB, TC, TD: -4, -3, -2, -1
			new Vector3(xend, h2, zend), new Vector3(x, h2, zend), 
			new Vector3(x, h2, z), new Vector3(xend, h2, z)
		});

		t.AddRange (new int[] {
			v.Count - 8, v.Count - 3, v.Count - 7, 
			v.Count - 8, v.Count - 4, v.Count - 3, 

			v.Count - 7, v.Count - 2, v.Count - 6,
			v.Count - 7, v.Count - 3, v.Count - 2,

			v.Count - 6, v.Count - 1, v.Count - 5,
			v.Count - 6, v.Count - 2, v.Count - 1,

			v.Count - 5, v.Count - 1, v.Count - 8,
			v.Count - 8, v.Count - 1, v.Count - 4,

			// square roof
			v.Count - 4, v.Count - 2, v.Count - 3,
			v.Count - 4, v.Count - 1, v.Count - 2
		});


		if (bor.h1 == 0f) {
			// if a ground-level building then we can also build a greeble onto the top of this building
			int[] _x = new int[] {Random.Range (bor.x, bor.xend), Random.Range (bor.x, bor.xend)};
			int[] _z = new int[] { Random.Range (bor.z, bor.zend), Random.Range (bor.z, bor.zend) };
			BuildingOccupationRecord b2 = new BuildingOccupationRecord() {x = _x[0], xend = _x[1], z = _z[0], zend = _z[1], h1 = bor.h2, h2 = bor.h2 + Random.Range(1f, 2f)};
			Fill3DSpace (b2);
		}
	}

	public override void Export() {
		m.vertices = v.ToArray ();
		m.triangles = t.ToArray ();

		m.RecalculateNormals ();

		mf.mesh = m;

	}
}

/// <summary>
/// Building occupation record. Describes the x, height, and z space of a rectangular building
/// </summary>
public class BuildingOccupationRecord {
	public int x {get; set;}
	public int z {get; set;}
	public int xend {get; set;}
	public int zend {get; set;}
	public float h1 {get; set;}
	public float h2 {get; set;}
	}