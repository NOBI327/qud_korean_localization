// using HarmonyLib;
// using XRL;
// using XRL.World.Skills;

// namespace KorFontTest.Patches
// {
// 	[HarmonyPatch(typeof(SkillEntry), nameof(SkillEntry.HandleXMLNode))]
// 	public static class SkillEntry_DisplayNameCapture
// 	{
// 		public static void Prefix(XmlDataHelper Reader, out string __state)
// 		{
// 			__state = Reader?.ParseAttribute<string>("DisplayName", null);
// 		}

// 		public static void Postfix(SkillEntry __instance, string __state)
// 		{
// 			if (!string.IsNullOrEmpty(__state))
// 			{
// 				SkillDisplayNameRegistry.SetDisplayName(__instance.Class, __state);
// 			}
// 		}
// 	}
// }
