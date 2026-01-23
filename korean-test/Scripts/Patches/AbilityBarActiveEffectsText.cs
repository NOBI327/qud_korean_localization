using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Qud.UI;
using XRL.Core;
using XRL.World;
using ConsoleLib.Console;

namespace KorFontTest.Patches
{
	internal static class AbilityBarActiveEffectsText
	{
		private static readonly Dictionary<string, string> Replacements = new Dictionary<string, string>
		{
			// Add or edit translations here.
			{ "{{Y|<color=#508d75>ACTIVE EFFECTS:</color>}} ", "{{Y|<color=#508d75>활성 효과:</color>}} " },
			{ "{{C|<color=#3e83a5>TARGET:</color> ", "{{C|<color=#3e83a5>대상:</color> " },
			{ "{{K|TARGET: [none]}}", "{{K|대상: [없음]}}" },
			{ " {{K|[{{g|on}}]}}", " {{K|[{{g|켜짐}}]}}" },
			{ " {{K|[{{g|off}}]}}", " {{K|[{{g|꺼짐}}]}}" },
			{ "{{K|You have no missile weapons equipped.}}", "{{K|장착한 원거리 무기가 없습니다.}}" },
		};

		public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
		{
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Ldstr &&
					instruction.operand is string s &&
					Replacements.TryGetValue(s, out string replacement))
				{
					instruction.operand = replacement;
				}
				yield return instruction;
			}
		}
	}

	[HarmonyPatch(typeof(AbilityBar), "InternalUpdateActiveEffects", new[] { typeof(GameObject) })]
	public static class AbilityBar_InternalUpdateActiveEffects_Text
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return AbilityBarActiveEffectsText.Transpile(instructions);
		}
	}

	[HarmonyPatch(typeof(AbilityBar), "AfterRender", new[] { typeof(XRLCore), typeof(ScreenBuffer) })]
	public static class AbilityBar_AfterRender_Text
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return AbilityBarActiveEffectsText.Transpile(instructions);
		}
	}

	[HarmonyPatch(typeof(MissileWeaponArea), "AfterRender", new[] { typeof(XRLCore), typeof(ScreenBuffer) })]
	public static class AbilityBar_NoMissileWeapon_Text
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return AbilityBarActiveEffectsText.Transpile(instructions);
		}
	}
}
