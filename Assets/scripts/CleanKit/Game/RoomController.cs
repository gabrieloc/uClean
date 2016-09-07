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
				preparePropsInRoomNamed ("playground");
				break;
			
			case Scene.Room:
				preparePropsInRoomNamed ("room");
				break;

			default:
				break;
			}
		}

		void preparePropsInRoomNamed (string name)
		{
			RoomInfo roomInfo = RoomInfo.RoomNamed (name);
			propController.PrepareProps (roomInfo.props);
		}
	}
}
