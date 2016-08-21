using System;
using UnityEngine;
using System.Collections;

namespace CleanKit
{
	public class BotNamer
	{
		public static string New ()
		{
			TextAsset textFile = Resources.Load ("Misc/names", typeof(TextAsset)) as TextAsset;
			string[] names = textFile.text.Split ('\n');
			int index = UnityEngine.Random.Range (0, names.Length);
			return names [index];
		}
	}
}
