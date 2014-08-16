using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed;
	Camera _camera;

	void Start () {
		GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
		_camera = cameraObject.camera;
	}
	
	void Update () {
		Vector3 target2d = new Vector3();
		Vector3 target3d = new Vector3();
		target2d.z = transform.localPosition.z;
		if (Input.touchCount > 0)
		{
			target2d.x = Input.touches[0].position.x;
			target2d.y = Input.touches[1].position.y;
			target3d = _camera.ScreenToWorldPoint(target2d);
		}
		else if (Input.GetMouseButton(0))
		{
			target2d.x = Input.mousePosition.x;
			target2d.y = Input.mousePosition.y;
			target3d = _camera.ScreenToWorldPoint(target2d);
			Debug.Log(string.Format("2dx: {0}, 2dy: {1}, 2dz: {2}", target2d.x, target2d.y, target2d.z));
			Debug.Log(string.Format("3dx: {0}, 3dy: {1}, 3dz: {2}", target3d.x, target3d.y, target3d.z));
		}
		else
		{
			target3d = transform.position;
		}
		target3d.z = transform.position.z;

		transform.position += (target3d - transform.position) * Time.deltaTime * speed;
	}
}
