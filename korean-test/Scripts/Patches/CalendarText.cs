using System;
using System.Collections.Generic;
using HarmonyLib;
using XRL.World;

namespace KorFontTest.Patches
{
	internal static class CalendarText
	{
		private const string MarginaliaFormatKey = "On the {0} of {1}";
		private static readonly Dictionary<string, string> Replacements = new Dictionary<string, string>
		{
			// Marginalia format.
			{ MarginaliaFormatKey, "{1}의 {0}에" },

			// Months.
			{ "Nivvun Ut", "니븐 우트" },
			{ "Iyur Ut", "이유르 우트" },
			{ "Simmun Ut", "심문 우트" },
			{ "Tuum Ut", "투움 우트" },
			{ "Ubu Ut", "우부 우트" },
			{ "Uulu Ut", "우울루 우트" },
			{ "Ut yara Ux", "우트 야라 우크" },
			{ "Tishru i Ux", "티슈루 i 우크" },
			{ "Tishru ii Ux", "티슈루 ii 우크" },
			{ "Kisu Ux", "키수 우크" },
			{ "Tebet Ux", "테벳 우크" },
			{ "Shwut Ux", "슈웃 우크" },
			{ "Uru Ux", "우루 우크" },

			// Days.
			{ "0th", "0일" },
			{ "1st", "1일" },
			{ "2nd", "2일" },
			{ "3rd", "3일" },
			{ "4th", "4일" },
			{ "5th", "5일" },
			{ "6th", "6일" },
			{ "7th", "7일" },
			{ "8th", "8일" },
			{ "9th", "9일" },
			{ "10th", "10일" },
			{ "11th", "11일" },
			{ "12th", "12일" },
			{ "13th", "13일" },
			{ "14th", "14일" },
			{ "Ides", "중순" },
			{ "16th", "16일" },
			{ "17th", "17일" },
			{ "18th", "18일" },
			{ "19th", "19일" },
			{ "20th", "20일" },
			{ "21st", "21일" },
			{ "22nd", "22일" },
			{ "23rd", "23일" },
			{ "24th", "24일" },
			{ "25th", "25일" },
			{ "26th", "26일" },
			{ "27th", "27일" },
			{ "28th", "28일" },
			{ "29th", "29일" },
			{ "30th", "30일" },

			// Times of day.
			{ "Beetle Moon Zenith", "딱정벌레 달 정점" },
			{ "Waning Beetle Moon", "기우는 딱정벌레 달" },
			{ "The Shallows", "얕은 물" },
			{ "Harvest Dawn", "수확의 여명" },
			{ "Waxing Salt Sun", "차오르는 소금 해" },
			{ "High Salt Sun", "높은 소금 해" },
			{ "Waning Salt Sun", "기우는 소금 해" },
			{ "Hindsun", "힌드선" },
			{ "Jeweled Dusk", "보석빛 황혼" },
			{ "Waxing Beetle Moon", "차오르는 딱정벌레 달" },
			{ "Zero Hour", "영시" }
		};

		public static string Translate(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			if (Replacements.TryGetValue(value, out string replacement))
			{
				return replacement;
			}
			return value;
		}

		public static bool TryGetMarginaliaFormat(out string format)
		{
			return Replacements.TryGetValue(MarginaliaFormatKey, out format);
		}
	}

	[HarmonyPatch(typeof(Calendar), nameof(Calendar.GetMonth), new[] { typeof(int) })]
	public static class Calendar_GetMonth_Text
	{
		public static void Postfix(ref string __result)
		{
			__result = CalendarText.Translate(__result);
		}
	}

	[HarmonyPatch(typeof(Calendar), nameof(Calendar.GetDay), new[] { typeof(int) })]
	public static class Calendar_GetDay_Text
	{
		public static void Postfix(ref string __result)
		{
			__result = CalendarText.Translate(__result);
		}
	}

	[HarmonyPatch(typeof(Calendar), nameof(Calendar.GetTime), new[] { typeof(int) })]
	public static class Calendar_GetTime_Text
	{
		public static void Postfix(ref string __result)
		{
			__result = CalendarText.Translate(__result);
		}
	}

	[HarmonyPatch(typeof(Calendar), nameof(Calendar.GetMarginaliaTime), new[] { typeof(long) })]
	public static class Calendar_GetMarginaliaTime_Text
	{
		public static void Postfix(long Time, ref string __result)
		{
			if (!CalendarText.TryGetMarginaliaFormat(out string format))
			{
				__result = CalendarText.Translate(__result);
				return;
			}

			string day = Calendar.GetDay(Time);
			string month = Calendar.GetMonth(Time);
			__result = string.Format(format, day, month);
		}
	}
}
