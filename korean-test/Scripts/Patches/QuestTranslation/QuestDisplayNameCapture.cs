using HarmonyLib;
using XRL;
using XRL.World;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(QuestLoader.XMLParsing), "HandleQuest")]
	public static class QuestLoader_XMLParsing_HandleQuest_DisplayCapture
	{
		private struct DisplayCapture
		{
			public string QuestId;
			public string DisplayName;
			public string DisplayAccomplishment;
			public string DisplayHagiograph;
			public string DisplayGospel;
		}

		static void Prefix(XmlDataHelper xml, QuestLoader.XMLParsing __instance, out DisplayCapture __state)
		{
			string questId = xml?.GetAttribute("ID");
			if (string.IsNullOrEmpty(questId))
			{
				questId = xml?.GetAttribute("Name");
			}

			__state = new DisplayCapture
			{
				QuestId = questId,
				DisplayName = xml?.GetAttribute("DisplayName"),
				DisplayAccomplishment = xml?.GetAttribute("DisplayAccomplishment"),
				DisplayHagiograph = xml?.GetAttribute("DisplayHagiograph"),
				DisplayGospel = xml?.GetAttribute("DisplayGospel")
			};
		}

		static void Postfix(QuestLoader.XMLParsing __instance, DisplayCapture __state)
		{
			if (string.IsNullOrEmpty(__state.QuestId))
			{
				return;
			}

			if (__instance != null && __instance.QuestsByID != null &&
				__instance.QuestsByID.TryGetValue(__state.QuestId, out Quest quest) &&
				quest != null)
			{
				if (!string.IsNullOrEmpty(__state.DisplayAccomplishment))
				{
					quest.Accomplishment = __state.DisplayAccomplishment;
				}
				if (!string.IsNullOrEmpty(__state.DisplayHagiograph))
				{
					quest.Hagiograph = __state.DisplayHagiograph;
				}
				if (!string.IsNullOrEmpty(__state.DisplayGospel))
				{
					quest.Gospel = __state.DisplayGospel;
				}
			}

			if (!string.IsNullOrEmpty(__state.DisplayName))
			{
				QuestDisplayRegistry.SetQuestName(__state.QuestId, __state.DisplayName);
			}
		}
	}
}
