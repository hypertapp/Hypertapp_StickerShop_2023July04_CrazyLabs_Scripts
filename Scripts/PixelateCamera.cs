using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class PixelateCamera : MonoBehaviour {
	
	public Material mat;
	int cellSize;

	void Awake () {
		
		cellSize = (int)(Mathf.Ceil (Screen.width / mat.GetInt ("_WidthPixel")));
		mat.SetInt ("_Pixelate", cellSize);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		
		if (mat == null)
			return;
		Graphics.Blit(source, destination, mat);
	}
}