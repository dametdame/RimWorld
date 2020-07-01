using System;
using UnityEngine;
namespace DRimEditor.DetailView
{
	public static class Vector3Extensions
    {
		public static float Sum(this Vector3 vec)
		{
			return vec.x + vec.y + vec.z;
		}

		public static float Max(this Vector3 vec)
		{
			return Mathf.Max(vec.x, Mathf.Max(vec.y, vec.z));
		}

		public static Vector3 Subtract(this Vector3 vec, float minus)
		{
			return new Vector3(vec.x - minus, vec.y - minus, vec.z - minus);
		}
	}
}
