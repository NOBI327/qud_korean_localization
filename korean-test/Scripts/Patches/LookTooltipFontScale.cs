using System.Collections.Generic;
using HarmonyLib;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(TooltipManager), nameof(TooltipManager.SetTextAndSize))]
	public static class TooltipManager_SetTextAndSize_LookScale
	{
		private const float Scale = 0.85f;
		private static readonly Dictionary<int, float> BaseTextSizes = new Dictionary<int, float>();
		private static readonly Dictionary<int, float> BaseTmpSizes = new Dictionary<int, float>();

		public static void Prefix(TooltipTrigger trigger)
		{
			if (trigger == null)
			{
				return;
			}

			GameManager gameManager = GameManager.Instance;
			if (gameManager == null || (trigger != gameManager.lookerTooltip && trigger != gameManager.tileTooltip))
			{
				return;
			}

			Tooltip tooltip = trigger.Tooltip;
			if (tooltip == null)
			{
				return;
			}

			if (tooltip.TextFields != null)
			{
				for (int i = 0; i < tooltip.TextFields.Count; i++)
				{
					ApplyTextScale(tooltip.TextFields[i]?.Text);
				}
			}

			if (tooltip.TMPFields != null)
			{
				for (int i = 0; i < tooltip.TMPFields.Count; i++)
				{
					ApplyTmpScale(tooltip.TMPFields[i]?.Text);
				}
			}
		}

		private static void ApplyTextScale(Text text)
		{
			if (text == null)
			{
				return;
			}

			int id = text.GetInstanceID();
			float baseSize;
			if (!BaseTextSizes.TryGetValue(id, out baseSize))
			{
				baseSize = text.fontSize;
				StoreBaseSize(BaseTextSizes, id, baseSize);
			}

			float scaled = baseSize * Scale;
			if (!Mathf.Approximately(text.fontSize, scaled) && !Mathf.Approximately(text.fontSize, baseSize))
			{
				baseSize = text.fontSize;
				StoreBaseSize(BaseTextSizes, id, baseSize);
				scaled = baseSize * Scale;
			}

			int targetSize = Mathf.Max(1, Mathf.RoundToInt(scaled));
			if (text.fontSize != targetSize)
			{
				text.fontSize = targetSize;
			}
		}

		private static void ApplyTmpScale(TextMeshProUGUI tmp)
		{
			if (tmp == null)
			{
				return;
			}

			int id = tmp.GetInstanceID();
			float baseSize;
			if (!BaseTmpSizes.TryGetValue(id, out baseSize))
			{
				baseSize = tmp.fontSize;
				StoreBaseSize(BaseTmpSizes, id, baseSize);
			}

			float scaled = baseSize * Scale;
			if (!Mathf.Approximately(tmp.fontSize, scaled) && !Mathf.Approximately(tmp.fontSize, baseSize))
			{
				baseSize = tmp.fontSize;
				StoreBaseSize(BaseTmpSizes, id, baseSize);
				scaled = baseSize * Scale;
			}

			if (!Mathf.Approximately(tmp.fontSize, scaled))
			{
				tmp.fontSize = scaled;
			}
		}

		private static void StoreBaseSize(Dictionary<int, float> table, int id, float value)
		{
			table[id] = value;
			if (table.Count > 2048)
			{
				table.Clear();
			}
		}
	}
}
