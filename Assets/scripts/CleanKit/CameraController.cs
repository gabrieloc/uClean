using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public class CameraController : MonoBehaviour
	{
		public float moveSensitivityX = 1.0f;
		public float moveSensitivityY = 1.0f;
		public bool updateZoomSensitivity = true;
		public float orthoZoomSpeed = 0.05f;
		public float minZoom = 1.0f;
		public float maxZoom = 20.0f;

		public float panDecelerationCurve = 0.75f;
		public float zoomDecelerationCurve = 0.8f;

		private Vector3 panPosition;
		private float zoomValue;

		private Camera _camera;

		void Start ()
		{
			_camera = Camera.main;
		}

		#if UNITY_EDITOR

		bool isPanning = false;
		Vector3 panOrigin;

		void Update ()
		{
			Rect screenRect = new Rect (0, 0, Screen.width, Screen.height);
			if (!screenRect.Contains (Input.mousePosition)) {
				return;
			}

			float scrollPosition = Input.GetAxis ("Mouse ScrollWheel");
			Vector3 mousePosition = Input.mousePosition;

			if (Input.GetMouseButtonDown (0)) {
				isPanning = true;
				panPosition = _camera.transform.position;
				panOrigin = Camera.main.ScreenToViewportPoint (mousePosition);
			} else if (Input.GetMouseButtonUp (0)) {
				isPanning = false;
			}

			Vector3 position = _camera.ScreenToViewportPoint (Input.mousePosition) - panOrigin;
			if (isPanning) {// && delta.magnitude > 0) {
				_camera.transform.position = panPosition - position;
			} else if (scrollPosition != 0.0f && mousePosition != Vector3.zero) {
				_camera.orthographicSize -= Input.mouseScrollDelta.y;
				_camera.orthographicSize = Mathf.Clamp (_camera.orthographicSize, minZoom, maxZoom);
			}
		}
		#else
		void Update ()
		{
			if (updateZoomSensitivity) {
				moveSensitivityX = _camera.orthographicSize / 5.0f;
				moveSensitivityY = _camera.orthographicSize / 5.0f;
			}

			Touch[] touches = Input.touches;
			switch (touches.Length) {
			case 1:
				TouchPhase phase = touches [0].phase;
				if (phase == TouchPhase.Moved) {
					Vector2 delta = touches [0].deltaPosition;
					float positionX = -delta.x * moveSensitivityX * Time.deltaTime;
					float positionY = -delta.y * moveSensitivityY * Time.deltaTime;
					panPosition = new Vector3 (positionX, positionY, 0);
					_camera.transform.position += panPosition;

					zoomValue = 0.0f;
				} else if (phase == TouchPhase.Stationary) {
					panPosition = Vector3.zero;
				}
				break;
			case 2:
				Touch touchOne = touches [0];
				Touch touchTwo = touches [1];
				Vector2 touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;
				Vector2 touchTwoPreviousPosition = touchTwo.position - touchTwo.deltaPosition;

				float previousTouchDeltaMagnitude = (touchOnePreviousPosition - touchTwoPreviousPosition).magnitude;
				float touchDeltaMagnitude = (touchOne.position - touchTwo.position).magnitude;

				float deltaMagnitudeDiff = previousTouchDeltaMagnitude - touchDeltaMagnitude;
				zoomValue = deltaMagnitudeDiff * orthoZoomSpeed;

				_camera.orthographicSize += zoomValue;
				_camera.orthographicSize = Mathf.Clamp (_camera.orthographicSize, minZoom, maxZoom);

				panPosition = Vector3.zero;

				break;
			default:
				if (panPosition != Vector3.zero) {
					panPosition *= panDecelerationCurve;
					_camera.transform.position += panPosition;
				} else if (zoomValue != 0.0f) {
					zoomValue *= zoomDecelerationCurve;
					_camera.orthographicSize += zoomValue;
					_camera.orthographicSize = Mathf.Clamp (_camera.orthographicSize, minZoom, maxZoom);
				}
				break;
			}
		}
		#endif
	}
}