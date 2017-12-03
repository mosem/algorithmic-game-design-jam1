using System.Collections;
using UnityEngine;

namespace Flight {
	class SphericalMapping : MonoBehaviour {

		void LatLonToXYZ(float lat, float lon, ref float x, ref float y, ref float z)
		{
			float r = Mathf.Cos (Mathf.Deg2Rad * lon);
			x = r * Mathf.Cos (Mathf.Deg2Rad * lat);
			y = Mathf.Sin (Mathf.Deg2Rad * lon);
			z = r * Mathf.Sin (Mathf.Deg2Rad * lat);
		}
	}
}