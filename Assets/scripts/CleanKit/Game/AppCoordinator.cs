using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace CleanKit
{
	public class AppCoordinator : MonoBehaviour
	{
		enum Scene
		{
			Launch = 1,
			Setup,
			Room
		}

		public void RouteToSetup ()
		{
			routeToScene (Scene.Setup);
		}

		public void RouteToRoom ()
		{
			routeToScene (Scene.Room);
		}

		void routeToScene (Scene scene)
		{
			SceneManager.LoadScene (identifierForScene (scene), LoadSceneMode.Additive);
		}

		string identifierForScene (Scene scene)
		{
			switch (scene) {
			case Scene.Launch:
				return "launch";
			case Scene.Setup:
				return "setup";
			case Scene.Room:
				return "room";
			default:
				return null;
			}
		}
	}
}
