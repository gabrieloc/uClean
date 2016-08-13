using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public class RoomController : MonoBehaviour
	{
		public PropController propController;
		public BotController botController;
		public Scene scene;

		// Use this for initialization
		void Start ()
		{
			switch (scene) {
			case Scene.Playground:
				preparePlayground ();
				break;
			default:
				break;
			}
		}

		void preparePlayground ()
		{
			RoomInfo playground = RoomInfo.CreatePlayground ();
			preparePropsInRoom (playground);
		}

		void preparePropsInRoom (RoomInfo roomInfo)
		{
			foreach (PropInfo propInfo in roomInfo.props) {
				GameObject prop = PropLoader.CreateProp (propInfo);
				Interactable interactable = prop.GetComponent<Interactable> ();
				interactable.interactableDelegate = propController;
				prop.transform.SetParent (propController.transform);
			}
		}
	}
}
