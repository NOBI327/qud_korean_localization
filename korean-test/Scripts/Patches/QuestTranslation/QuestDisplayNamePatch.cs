using HarmonyLib;
using XRL.World;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(Quest), "get_DisplayName")]
	public static class Quest_DisplayName_Translate
	{
		public static void Postfix(Quest __instance, ref string __result)
		{
			if (__instance == null)
			{
				return;
			}

			if (QuestDisplayRegistry.TryGetQuestName(__instance.ID, out string displayName))
			{
				__result = "{{W|" + displayName + "}}";
			}
		}
	}
}
