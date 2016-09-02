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

		public float panDecelerationCurve = 0.75f;

		Camera _camera;

		Vector3? objectPosition;
		Vector3? panPosition;
		float objectDistance;

		Vector3 panDelta = new Vector3 ();
		float sensitivityMultiplier;

		public float FocusSpeed = 64.0f;

		void Start ()
		{
			_camera = GetComponent<Camera> ();
		}

		void Update ()
		{
			updateSensitivityMultipler ();

			if (objectPosition.HasValue) {
				followPoint (objectPosition.Value);
			} else if (panPosition.HasValue) {
				Vector3 translate = new Vector3 (
					                    panDelta.x * moveSensitivityX * sensitivityMultiplier * -Time.deltaTime,
					                    panDelta.y * moveSensitivityY * sensitivityMultiplier * -Time.deltaTime,
					                    0.0f);
				_camera.transform.Translate (translate);
				panDelta *= panDecelerationCurve;
			}
		}

		public void FocusOnSubject (GameObject subject, ShotSize shotSize = ShotSize.MidShot)
		{
			objectPosition = subject.transform.position;
			Renderer renderer = subject.GetComponent<Renderer> ();
			float magnitude = renderer.bounds.extents.magnitude;
			float fDistance = magnitude / Mathf.Tan (_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
			objectDistance = fDistance * frameFitMultipleForShotSize (shotSize);
		}

		void updateSensitivityMultipler ()
		{
			float depth = centerDepth ();
			if (depth < _camera.farClipPlane * 0.5f) {
				sensitivityMultiplier = depth / _camera.farClipPlane * 2.0f;
			} else {
				sensitivityMultiplier = 1 / (depth / _camera.farClipPlane) - 1.0f;
			}
		}

		void followPoint (Vector3 point)
		{
			float d = objectDistance;
			Vector3 r = transform.eulerAngles * Mathf.Deg2Rad;

			float ox = d * Mathf.Cos (r.x) * -1;
			float oz = d * Mathf.Cos (r.y) * -1;
			float oy = Mathf.Sqrt (Mathf.Pow (ox, 2) + Mathf.Pow (oz, 2));
			Vector3 offset = new Vector3 (ox, oy, oz);
			Vector3 position = objectPosition.Value + offset;

			float w = Screen.width * 0.5f;
			float h = Screen.height * 0.5f;

			Vector3 screenPosition = _camera.WorldToScreenPoint (point);
			Vector2 screenMultiple = new Vector2 (
				                         Mathf.Abs ((screenPosition.x - w) / w),
				                         Mathf.Abs ((screenPosition.y - h) / h)
			                         );

			float currentDistance = Vector3.Distance (transform.position, point);
			currentDistance = currentDistance > d ? currentDistance / d : d / currentDistance;
			float zoomMultiple = Mathf.Pow (currentDistance, 2.0f);

			float distanceDelta = Time.deltaTime * FocusSpeed * zoomMultiple * screenMultiple.sqrMagnitude;
			Vector3 movePosition = Vector3.MoveTowards (_camera.transform.position, position, distanceDelta);

			_camera.transform.position = movePosition;
		}

		float frameFitMultipleForShotSize (ShotSize shotSize)
		{
			switch (shotSize) {
			case ShotSize.ExtremeCloseUp:
				return 1.0f;
			case ShotSize.CloseUp:
				return 2.0f;
			case ShotSize.MidShot:
				return 3.0f;
			case ShotSize.LongShot:
				return 5.0f;
			case ShotSize.VeryLongShot:
				return 8.0f;
			default:
				return 1.0f;
			}
		}

		public void BeginPanning (Vector3 initialPosition)
		{
			panPosition = initialPosition;
			objectPosition = null;
		}

		public void UpdatePanPosition (Vector3 newPosition)
		{
			panDelta = newPosition - panPosition.Value;
//			print (newPosition);
			panPosition = newPosition;
			objectPosition = null;
		}

		public void UpdateZoomValue (float zoomValue)
		{
//			print (zoomValue);
			Vector3 translate = Vector3.zero;
			float depth = centerDepth ();

			if (centerDepth () > _camera.nearClipPlane && depth < _camera.farClipPlane) {
				translate.z += zoomValue;
			} else {
				float zoomRecoverySpeed = 10.0f;
				translate.z += zoomRecoverySpeed * (centerDepth () > _camera.nearClipPlane ? 1 : -1);
			}
			_camera.transform.Translate (translate);
		}

		float centerDepth ()
		{
			Ray ray = _camera.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.0f));
			RaycastHit hit;
			float depth = 0.0f;
			if (Physics.Raycast (ray, out hit)) {
				depth = hit.distance;
			}
			return depth;
		}
	}
}