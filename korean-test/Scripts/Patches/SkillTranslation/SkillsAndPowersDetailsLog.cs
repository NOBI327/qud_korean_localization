// using HarmonyLib;
// using Qud.UI;
// using XRL.UI;
// using UnityEngine;

// namespace KorFontTest.Patches
// {
// 	[HarmonyPatch(typeof(SkillsAndPowersStatusScreen), nameof(SkillsAndPowersStatusScreen.UpdateDetailsFromNode))]
// 	public static class SkillsAndPowersStatusScreen_UpdateDetailsFromNode_Log
// 	{
// 		public static void Prefix(SPNode node)
// 		{
// 			string description = node != null ? node.Description : "<null>";
// 			Debug.LogError("SkillsAndPowers node.Description: " + description);
// 		}
// 	}
// }
