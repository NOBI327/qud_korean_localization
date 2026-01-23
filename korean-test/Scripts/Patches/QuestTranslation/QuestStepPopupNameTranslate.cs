using HarmonyLib;
using XRL.World;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(Quest), nameof(Quest.ShowFailStepPopup))]
	public static class Quest_ShowFailStepPopup_NameTranslate
	{
		public static void Prefix(Quest __instance, QuestStep Step, out string __state)
		{
			__state = null;
			if (__instance == null || Step == null)
			{
				return;
			}

			if (QuestDisplayRegistry.TryGetStepName(__instance.ID, Step.ID, out string displayName) &&
				!string.IsNullOrEmpty(displayName) &&
				displayName != Step.Name)
			{
				__state = Step.Name;
				Step.Name = displayName;
			}
		}

		public static void Postfix(QuestStep Step, string __state)
		{
			if (Step != null && __state != null)
			{
				Step.Name = __state;
			}
		}
	}
}
