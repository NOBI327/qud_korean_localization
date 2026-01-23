using HarmonyLib;
using XRL;
using XRL.World.Skills;

namespace KorFontTest.Patches.SkillTranslation
{
	[HarmonyPatch(typeof(SkillEntry), nameof(SkillEntry.HandleXMLNode))]
	public static class SkillEntry_DisplayCapture
	{
		private struct DisplayState
		{
			public string DisplayName;
			public string DisplayDescription;
			public string DisplaySnippet;
		}

		static void Prefix(XmlDataHelper Reader, out DisplayState __state)
		{
			__state = new DisplayState
			{
				DisplayName = Reader?.GetAttribute("DisplayName"),
				DisplayDescription = Reader?.GetAttribute("DisplayDescription"),
				DisplaySnippet = Reader?.GetAttribute("DisplaySnippet")
			};
		}

		static void Postfix(SkillEntry __instance, DisplayState __state)
		{
			if (__instance == null)
			{
				return;
			}

			if (!string.IsNullOrEmpty(__state.DisplayName))
			{
				__instance.Name = __state.DisplayName;
			}

			if (!string.IsNullOrEmpty(__state.DisplayDescription))
			{
				__instance.Description = __state.DisplayDescription;
				SkillDisplayTextRegistry.SetDescription(__instance.Class, __state.DisplayDescription);
			}

			if (!string.IsNullOrEmpty(__state.DisplaySnippet))
			{
				__instance.Snippet = __state.DisplaySnippet;
			}
		}
	}
}
