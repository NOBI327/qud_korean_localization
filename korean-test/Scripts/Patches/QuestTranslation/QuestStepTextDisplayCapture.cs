using HarmonyLib;
using XRL;
using XRL.World;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(QuestLoader.XMLParsing), "HandleStepText")]
	public static class QuestLoader_XMLParsing_HandleStepText_DisplayCapture
	{
		public static void Prefix(XmlDataHelper xml, QuestLoader.XMLParsing __instance, out string __state)
		{
			__state = xml?.GetAttribute("DisplayText");
		}

		public static void Postfix(QuestLoader.XMLParsing __instance, string __state)
		{
			if (!string.IsNullOrEmpty(__state) && __instance?.currentQuestStep != null)
			{
				__instance.currentQuestStep.Text = __state;
			}
		}
	}
}
