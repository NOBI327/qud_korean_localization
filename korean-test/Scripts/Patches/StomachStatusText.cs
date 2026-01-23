using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using XRL.World.Parts;

namespace KorFontTest.Patches
{
	internal static class StomachStatusText
	{
		private static readonly Dictionary<string, string> Replacements = new Dictionary<string, string>
		{
			{ "{{g|Sated}}", "{{g|포만}}" },
			{ "{{W|Hungry}}", "{{W|배고픔}}" },
			{ "{{R|Wilted!}}", "{{R|시들었다!}}" },
			{ "{{R|Famished!}}", "{{R|굶주림!}}" },
			{ "{{R|Desiccated!}}", "{{R|바싹말랐다!}}" },
			{ "{{r|Dry}}", "{{r|건조}}" },
			{ "{{c|Moist}}", "{{c|촉촉}}" },
			{ "{{b|Wet}}", "{{b|젖음}}" },
			{ "{{B|Soaked}}", "{{B|흠뻑젖음}}" },
			{ "{{R|Dehydrated!}}", "{{R|탈수!}}" },
			{ "{{r|Parched}}", "{{r|바싹마름}}" },
			{ "{{Y|Thirsty}}", "{{Y|목마름}}" },
			{ "{{g|Quenched}}", "{{g|갈증해소}}" },
			{ "{{G|Tumescent}}", "{{G|부풀어오름}}" }
		};

		public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
		{
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Ldstr &&
					instruction.operand is string s &&
					Replacements.TryGetValue(s, out string replacement))
				{
					instruction.operand = replacement; // Edit translations here if needed.
				}
				yield return instruction;
			}
		}
	}

	[HarmonyPatch(typeof(Stomach), nameof(Stomach.FoodStatus))]
	public static class Stomach_FoodStatus_Text
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return StomachStatusText.Transpile(instructions);
		}
	}

	[HarmonyPatch(typeof(Stomach), nameof(Stomach.WaterStatus))]
	public static class Stomach_WaterStatus_Text
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return StomachStatusText.Transpile(instructions);
		}
	}
}
