using UnityEngine;
using System.Collections;

namespace Flight{

public class TileGenerator : MonoBehaviour {
	public int size_x;
	public int size_y;
	public float tileSize;
	public int tileResolution;
	
	public Texture2D texture;
	
	// Use this for initialization
	void Start () {
		BuildMesh(transform);
		BuildTexture(transform);
	}
	
	private void BuildMesh(Transform plane) {
		int numTiles = size_x * size_y;
		int numTris = numTiles * 2;
		
		int vsize_x = size_x + 1;
		int vsize_y = size_y + 1;
		int numVerts = vsize_x * vsize_y;
		
		Vector3[] vertices = new Vector3[ numVerts ];
		Vector3[] normals = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		
		int[] triangles = new int[ numTris * 3 ];
		
		int x, z;
		for(z=0; z < vsize_y; z++) {
			for(x=0; x < vsize_x; x++) {
				vertices[ z * vsize_x + x ] = new Vector3( x*tileSize, 0, -z*tileSize );
				normals[ z * vsize_x + x ] = Vector3.up;
				uv[ z * vsize_x + x ] = new Vector2( (float)x / size_x, 1f - (float)z / size_y );
			}
		}
		
		for(z=0; z < size_y; z++) {
			for(x=0; x < size_x; x++) {
				int squareIndex = z * size_x + x;
				int triOffset = squareIndex * 6;
				triangles[triOffset + 0] = z * vsize_x + x + 		   0;
				triangles[triOffset + 2] = z * vsize_x + x + vsize_x + 0;
				triangles[triOffset + 1] = z * vsize_x + x + vsize_x + 1;
				
				triangles[triOffset + 3] = z * vsize_x + x + 		   0;
				triangles[triOffset + 5] = z * vsize_x + x + vsize_x + 1;
				triangles[triOffset + 4] = z * vsize_x + x + 		   1;
			}
		}
		
		// Create a new Mesh and populate with the data
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
		
		// Assign our mesh to our filter/renderer/collider
		MeshFilter mesh_filter = plane.GetComponent<MeshFilter>();
		MeshCollider mesh_collider = plane.GetComponent<MeshCollider>();
		
		mesh_filter.mesh = mesh;
	}
	
	
	private Color[][] ChopUpTiles() {
		int numTilesPerRow = texture.width / tileResolution;
		int numRows = texture.height / tileResolution;
			Debug.Log(numTilesPerRow);
			Debug.Log(numRows);
		Color[][] tiles = new Color[numTilesPerRow*numRows][];
		
		for(int y=0; y<numRows; y++) {
			for(int x=0; x<numTilesPerRow; x++) {
				tiles[y*numTilesPerRow + x] = texture.GetPixels( x*tileResolution , y*tileResolution, tileResolution, tileResolution );
			}
		}
		
		return tiles;
	}
	
	private void BuildTexture(Transform _plane) {
		int texWidth = size_x * tileResolution;
		int texHeight = size_y * tileResolution;
		Texture2D newTexture = new Texture2D(texWidth, texHeight,texture.format, false);
		
		Color[][] tiles = ChopUpTiles();
		
		for(int y=0; y < size_y; y++) {
			for(int x=0; x < size_x; x++) {
				Color[] p = tiles[Random.Range(0, 6)];
				newTexture.SetPixels(x*tileResolution, y*tileResolution, tileResolution, tileResolution, p);
			}
		}
		newTexture.anisoLevel=1;
		newTexture.mipMapBias=0f;
		newTexture.filterMode = FilterMode.Point;
		newTexture.wrapMode = TextureWrapMode.Clamp;
		newTexture.Apply();
		
		MeshRenderer mesh_renderer = _plane.GetComponent<MeshRenderer>();
		mesh_renderer.sharedMaterials[0].mainTexture = newTexture;
		
	}
}

}
