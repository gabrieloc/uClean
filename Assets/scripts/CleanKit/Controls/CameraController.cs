using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public enum ShotSize
	{
		ExtremeCloseUp,
		CloseUp,
		MidShot,
		LongShot,
		VeryLongShot
	}

	public class CameraController : MonoBehaviour
	{
		public float moveSensitivityX = 1.0f;
		public float moveSensitivityY = 1.0f;
		public bool updateZoomSensitivity = true;
		public float zoomSpeed = 1.0f;
		public float minZoom = 0.0f;
		public float maxZoom = 20.0f;

		public float panDecelerationCurve = 0.75f;
		public float zoomDecelerationCurve = 0.8f;

		Vector3 panPosition;
		float zoomValue;

		Camera _camera;

		Vector3? objectPosition;
		float objectDistance;

		public float FocusSpeed = 16.0f;

		void Start ()
		{
			_camera = GetComponent<Camera> ();
		}

		void Update ()
		{
			if (objectPosition.HasValue) {
				
				Vector3 p = objectPosition.Value;
				float d = objectDistance;
				Vector3 r = transform.eulerAngles * Mathf.Deg2Rad;
				Vector3 screenPosition = _camera.WorldToScreenPoint (objectPosition.Value);
				float w = Screen.width * 0.5f;
				float h = Screen.height * 0.5f;

				Vector3 offset = new Vector3 (
					                 d * Mathf.Cos (r.x) * (screenPosition.x > w ? 1 : -1),
					                 d * Mathf.Cos (r.y),
					                 d * Mathf.Cos (r.x) * (screenPosition.y > h ? 1 : -1)
				                 );

				Vector3 position = objectPosition.Value + offset;
				Vector2 screenMultiple = new Vector2 (
					                         Mathf.Abs ((screenPosition.x - w) / w),
					                         Mathf.Abs ((screenPosition.y - h) / h)
				                         );
				screenMultiple += new Vector2 (0.5f, 0.5f);

				float currentDistance = Vector3.Distance (transform.position, p);
				currentDistance = currentDistance > d ? currentDistance / d : d / currentDistance;
				float zoomMultiple = Mathf.Pow (currentDistance, 2.0f);

				float distanceDelta = Time.deltaTime * FocusSpeed * zoomMultiple * screenMultiple.sqrMagnitude;
				Vector3 movePosition = Vector3.MoveTowards (_camera.transform.position, position, distanceDelta);

				_camera.transform.position = movePosition;
			}

			updateInput ();
		}

		public void FocusOnSubject (GameObject subject, ShotSize shotSize = ShotSize.MidShot)
		{
			objectPosition = subject.transform.position;

			float fDistance = objectDistance * 0.5f / Mathf.Tan (_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
//			print (fDistance);
			objectDistance = 10.0f * frameFitMultipleForShotSize (shotSize); // TODO
		}

		float frameFitMultipleForShotSize (ShotSize shotSize)
		{
			switch (shotSize) {
			case ShotSize.ExtremeCloseUp:
				return 0.5f;
			case ShotSize.CloseUp:
				return 0.8f;
			case ShotSize.MidShot:
				return 1.5f;
			case ShotSize.LongShot:
				return 4.0f;
			case ShotSize.VeryLongShot:
				return 8.0f;
			default:
				return 1.0f;
			}
		}

		#if UNITY_EDITOR

		public float mouseSensitivityCurve = 0.25f;

		Vector3 panDelta = new Vector3 ();

		void updateInput ()
		{
			if (Controls.InteractingWithScene ()) {
				panDelta = Vector3.zero;
				return;
			}

			Rect screenRect = new Rect (0, 0, Screen.width, Screen.height);
			if (!screenRect.Contains (Input.mousePosition)) {
				return;
			}

			if (Input.GetMouseButtonDown (0) || Input.GetMouseButton (0)) {
				objectPosition = null;
			}

			if (Input.GetMouseButtonDown (0)) {
				panPosition = Input.mousePosition;
			} else if (Input.GetMouseButton (0)) {
				panDelta = Input.mousePosition - panPosition;
				panPosition = Input.mousePosition;
			} else {
				panDelta *= panDecelerationCurve;
			}

			float sensitivityMultiplier = 0.0f;
			Ray ray = _camera.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.0f));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				float distance = hit.distance;
				float distanceMultiple = distance / maxZoom;
				sensitivityMultiplier = Mathf.Pow (distanceMultiple, 2);
			}

			// TODO install max and min zoom
			float z = Input.mouseScrollDelta.y * sensitivityMultiplier * zoomSpeed;
			Vector3 translate = new Vector3 (
				                    panDelta.x * moveSensitivityX * sensitivityMultiplier * -0.01f,
				                    panDelta.y * moveSensitivityY * sensitivityMultiplier * -0.01f,
				                    z);
			_camera.transform.Translate (translate);
		}
		#else
		void updateInput ()
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