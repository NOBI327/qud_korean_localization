// 모든 텍스트의 크기를 작게 조절
// 아이템 창 등에서 글자가 보이지 않는 문제 해결

using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine;
using XRL.UI;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(UITextSkin), nameof(UITextSkin.Apply))]
	public static class UITextSkin_Apply_GlobalScale
	{
		private const float Scale = 0.86f;
		private static readonly Dictionary<int, float> BaseSizes = new Dictionary<int, float>();
		private static int SuppressCount;

		public static void Postfix(UITextSkin __instance)
		{
			if (__instance == null)
			{
				return;
			}

			TextMeshProUGUI tmp = __instance.GetComponent<TextMeshProUGUI>();
			if (tmp == null)
			{
				return;
			}

			if (SuppressCount > 0)
			{
				return;
			}

			int id = tmp.GetInstanceID();
			float baseSize;
			if (!BaseSizes.TryGetValue(id, out baseSize))
			{
				baseSize = tmp.fontSize;
				StoreBaseSize(id, baseSize);
			}

			if (__instance.style != UITextSkin.Size.unset)
			{
				baseSize = tmp.fontSize;
				StoreBaseSize(id, baseSize);
			}
			else
			{
				float scaledCurrent = baseSize * Scale;
				if (!Mathf.Approximately(tmp.fontSize, scaledCurrent) &&
					!Mathf.Approximately(tmp.fontSize, baseSize))
				{
					baseSize = tmp.fontSize;
					StoreBaseSize(id, baseSize);
				}
			}

			float targetSize = baseSize * Scale;
			if (!Mathf.Approximately(tmp.fontSize, targetSize))
			{
				SetFontSize(tmp, targetSize);
			}
		}

		private static void StoreBaseSize(int id, float value)
		{
			BaseSizes[id] = value;
			if (BaseSizes.Count > 4096)
			{
				BaseSizes.Clear();
			}
		}

		private static void SetFontSize(TextMeshProUGUI tmp, float value)
		{
			SuppressCount++;
			try
			{
				tmp.fontSize = value;
			}
			finally
			{
				SuppressCount--;
			}
		}
	}
}
