using System;
using UnityEngine;

public class ReadMe : ScriptableObject
{
	public Texture2D Icon;
	public float IconMaxWidth = 255f;
	public string Title;
	public Section[] Sections;

	[Serializable]
	public class Section
	{
		public string Heading, Text, LinkText, URL;
	}
}
