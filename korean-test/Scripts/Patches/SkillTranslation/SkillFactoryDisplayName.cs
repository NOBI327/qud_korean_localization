// using HarmonyLib;
// using XRL.World.Skills;

// namespace KorFontTest.Patches
// {
// 	[HarmonyPatch(typeof(SkillFactory), nameof(SkillFactory.GetSkillOrPowerName))]
// 	public static class SkillFactory_GetSkillOrPowerName_DisplayName
// 	{
// 		public static bool Prefix(string ClassName, ref string __result)
// 		{
// 			if (SkillDisplayNameRegistry.TryGetDisplayName(ClassName, out string displayName))
// 			{
// 				__result = displayName;
// 				return false;
// 			}

// 			return true;
// 		}
// 	}
// }
