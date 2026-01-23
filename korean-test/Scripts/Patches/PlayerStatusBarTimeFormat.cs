using System;
using System.Reflection;
using System.Text;
using HarmonyLib;
using Qud.UI;
using XRL.World;

namespace KorFontTest.Patches
{
	[HarmonyPatch]
	public static class PlayerStatusBar_TimeFormat
	{
		private static int? TimeValue;

		public static MethodBase TargetMethod()
		{
			Type enumType = AccessTools.Inner(typeof(PlayerStatusBar), "StringDataType");
			if (enumType == null)
			{
				return null;
			}
			return AccessTools.Method(typeof(PlayerStatusBar), "UpdateString", new[] { enumType, typeof(StringBuilder), typeof(bool) });
		}

		public static void Prefix(object type, StringBuilder data)
		{
			if (data == null || !IsTime(type))
			{
				return;
			}

			data.Length = 0;
			data.Append(Calendar.GetMonth())
				.Append(", ")
				.Append(Calendar.GetDay())
				.Append(", ")
				.Append(Calendar.GetTime());
		}

		private static bool IsTime(object dataType)
		{
			if (dataType == null)
			{
				return false;
			}

			if (!TimeValue.HasValue)
			{
				Type enumType = dataType.GetType();
				if (!enumType.IsEnum)
				{
					return false;
				}
				TimeValue = Convert.ToInt32(Enum.Parse(enumType, "Time"));
			}

			return Convert.ToInt32(dataType) == TimeValue.Value;
		}
	}
}
