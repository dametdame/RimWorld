using System;
using UnityEngine;
namespace DRimEditor.DetailView
{
	public static class Vector3Extensions
    {
		// Token: 0x0600005A RID: 90 RVA: 0x00004E24 File Offset: 0x00003024
		public static float Sum(this Vector3 vec)
		{
			return vec.x + vec.y + vec.z;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004E4C File Offset: 0x0000304C
		public static float Max(this Vector3 vec)
		{
			return Mathf.Max(vec.x, Mathf.Max(vec.y, vec.z));
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004E7C File Offset: 0x0000307C
		public static float Min(this Vector3 vec)
		{
			return Mathf.Min(vec.x, Mathf.Min(vec.y, vec.z));
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004EAC File Offset: 0x000030AC
		public static Vector3 Subtract(this Vector3 vec, float minus)
		{
			return new Vector3(vec.x - minus, vec.y - minus, vec.z - minus);
		}
	}
}
