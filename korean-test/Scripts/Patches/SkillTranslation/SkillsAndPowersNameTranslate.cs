using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Qud.UI;
using XRL.UI;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(SkillsAndPowersStatusScreen), nameof(SkillsAndPowersStatusScreen.UpdateDetailsFromNode))]
	public static class SkillsAndPowersStatusScreen_NameTranslate
	{
		private static readonly Dictionary<string, string> NameMap = new Dictionary<string, string>
		{
			// Add entries here, example:
			{ "Wayfaring", "방랑술" },
			{ "Mind's Compass", "마음의 나침반" },
		};

		private static string TranslateName(string name)
		{
			// UnityEngine.Debug.LogError("SkiilName : " + name);
			if (string.IsNullOrEmpty(name))
			{
				return name;
			}

			if (NameMap.TryGetValue(name, out string translated))
			{
				return translated;
			}

			return name;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo getName = AccessTools.PropertyGetter(typeof(SPNode), nameof(SPNode.Name));
			MethodInfo translate = AccessTools.Method(typeof(SkillsAndPowersStatusScreen_NameTranslate), nameof(TranslateName));

			foreach (CodeInstruction instruction in instructions)
			{
				yield return instruction;
				if (instruction.Calls(getName))
				{
					yield return new CodeInstruction(OpCodes.Call, translate);
				}
			}
		}
	}
}
