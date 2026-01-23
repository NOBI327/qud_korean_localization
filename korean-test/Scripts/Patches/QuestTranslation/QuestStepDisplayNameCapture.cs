using HarmonyLib;
using XRL;
using XRL.World;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(QuestLoader.XMLParsing), "HandleStep")]
	public static class QuestLoader_XMLParsing_HandleStep_DisplayCapture
	{
		private struct StepCapture
		{
			public string QuestId;
			public string StepId;
			public string DisplayName;
		}

		static void Prefix(XmlDataHelper xml, QuestLoader.XMLParsing __instance, out StepCapture __state)
		{
			string questId = __instance?.currentQuest?.ID;
			string stepId = xml?.GetAttribute("ID");
			if (string.IsNullOrEmpty(stepId))
			{
				stepId = xml?.GetAttribute("Name");
			}

			__state = new StepCapture
			{
				QuestId = questId,
				StepId = stepId,
				DisplayName = xml?.GetAttribute("DisplayName")
			};
		}

		static void Postfix(StepCapture __state)
		{
			if (!string.IsNullOrEmpty(__state.DisplayName) &&
				!string.IsNullOrEmpty(__state.QuestId) &&
				!string.IsNullOrEmpty(__state.StepId))
			{
				QuestDisplayRegistry.SetStepName(__state.QuestId, __state.StepId, __state.DisplayName);
			}
		}
	}
}
