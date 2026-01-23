// using HarmonyLib;
// using XRL;
// using XRL.World.Skills;

// namespace KorFontTest.Patches
// {
// 	[HarmonyPatch(typeof(PowerEntry), nameof(PowerEntry.HandleXMLNode))]
// 	public static class PowerEntry_DisplayNameCapture
// 	{
// 		public static void Prefix(XmlDataHelper Reader, out string __state)
// 		{
// 			__state = Reader?.ParseAttribute<string>("DisplayName", null);
// 		}

// 		public static void Postfix(PowerEntry __instance, string __state)
// 		{
// 			if (!string.IsNullOrEmpty(__state))
// 			{
// 				SkillDisplayNameRegistry.SetDisplayName(__instance.Class, __state);
// 			}
// 		}
// 	}
// }
