using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using XRL.World.Parts.Mutation;
using System.Text;
using ConsoleLib.Console;
using XRL;
using XRL.Language;
using XRL.UI;
using XRL.Rules;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Capabilities;
using XRL.World.Parts;
using MutationNightVision = XRL.World.Parts.Mutation.NightVision;

namespace KorFontTest.Patches.MutationDescriptionTranslation
{
	[HarmonyPatch(typeof(AcidSlimeGlands), nameof(AcidSlimeGlands.GetDescription))]
	public static class AcidSlimeGlands_GetDescription_Translate
	{
		static void Postfix(AcidSlimeGlands __instance, ref string __result)
		{
			// "You spit a puddle of corrosive acid."
			__result = "부식성 산 웅덩이를 뱉습니다.";
		}
	}
	[HarmonyPatch(typeof(AcidSlimeGlands), nameof(AcidSlimeGlands.GetLevelText))]
	public static class AcidSlimeGlands_GetLevelText_Translate
	{
		static void Postfix(AcidSlimeGlands __instance, int __0, ref string __result)
		{
			// "Covers the area in acidic slime.\nArea: 3x3\nRange: 8\nCooldown: 10 rounds\n"
			__result = "영역을 산성 슬라임으로 뒤덮습니다.\n영역: 3x3\n사정거리: 8\n쿨다운: 10 라운드\n";
		}
	}
	
	[HarmonyPatch(typeof(AdrenalControl2), nameof(AdrenalControl2.GetDescription))]
	public static class AdrenalControl2_GetDescription_Translate
	{
		static void Postfix(AdrenalControl2 __instance, ref string __result)
		{
			// "You regulate your body's release of adrenaline."
			__result = "당신은 신체의 아드레날린 분비를 조절합니다.";
		}
	}
	[HarmonyPatch(typeof(AdrenalControl2), nameof(AdrenalControl2.GetLevelText))]
	public static class AdrenalControl2_GetLevelText_Translate
	{
		static void Postfix(AdrenalControl2 __instance, int __0, ref string __result)
		{
			/*
			string text = "";
			text = text + "You can increase your body's adrenaline flow for " + GetQuicknessDuration(Level) + " rounds.\n";
			text = text + "While it's flowing, you gain +{{C|" + GetQuicknessBonus(Level) + "}} quickness and other physical mutations gain +{{C|" + GetMutationBonus(Level) + "}} rank.\n";
			return text + "Cooldown: " + GetCooldown(Level) + " rounds";
			*/
			int quicknessDuration = __instance.GetQuicknessDuration(__0);
			int quicknessBonus = __instance.GetQuicknessBonus(__0);
			int mutationBonus = __instance.GetMutationBonus(__0);
			int cooldown = __instance.GetCooldown(__0);

            string text = "";
			text = text + $"당신은 {quicknessDuration}라운드 동안 신체의 아드레날린 흐름을 증가시킬 수 있습니다.\n";
			text = text + $"아드레날린이 흐르는 동안, +{{{{C|{quicknessBonus}}}}} 신속과 다른 신체적 변이는 +{{{{C|{mutationBonus}}}}}랭크만큼 얻습니다.\n";
			text = text + $"쿨다운: {cooldown} 라운드";
			__result = text;
		}
	}

	[HarmonyPatch(typeof(FreezingRay), nameof(FreezingRay.GetDescription))]
	public static class FreezingRay_GetDescription_Translate
	{
		static void Postfix(FreezingRay __instance, ref string __result)
		{
			/*
			BodyPart registeredSlot = GetRegisteredSlot(BodyPartType, evenIfDismembered: true);
			if (registeredSlot != null)
			{
				return "You emit a ray of frost from your " + registeredSlot.GetOrdinalName() + ".";
			}
			return "You emit a ray of frost.";
			*/
			string original_description = __result;
			string output = "";
			if (original_description.Contains("from your"))
			{
				int startIndex = original_description.IndexOf("from your ") + "from your ".Length;
				int endIndex = original_description.IndexOf(".", startIndex);
				string extracted = original_description.Substring(startIndex, endIndex - startIndex);
				
				output = output + "당신의 " + extracted + "(으)로부터 ";
			}
			output = output + "냉기 광선을 방출합니다.";
			__result = output;
		}
	}
	[HarmonyPatch(typeof(FreezingRay), nameof(FreezingRay.GetLevelText))]
	public static class FreezingRay_GetLevelText_Translate
	{
		static void Postfix(FreezingRay __instance, int __0, ref string __result)
		{
			/*
			int rANGE = RANGE;
			return string.Concat(string.Concat(string.Concat("Emits a " + rANGE + "-square ray of frost in the direction of your choice.\n", "Damage: {{rules|", ComputeDamage(Level), "}}\n"), "Cooldown: 20 rounds\n"), "Melee attacks cool opponents by {{rules|", GetCoolOnHitAmount(Level), "}} degrees");
			*/
			int rANGE = FreezingRay.RANGE;
			string damage = __instance.ComputeDamage(__0);
			string degree = __instance.GetCoolOnHitAmount(__0);

			string text = "";
			text = text + $"{rANGE}크기 사각형 내 범위에 선택한 방향으로 냉기 광선을 방출합니다.\n";
			text = text + $"피해: {{{{rules|{damage}}}}}\n";
			text = text + $"쿨다운: 20 라운드\n";
			text = text + $"근접 공격은 적의 온도를 {{{{rules|{degree}}}}}도 만큼 낮춥니다.\n";

			__result = text;
		}
	}

	[HarmonyPatch(typeof(Albino), nameof(Albino.GetDescription))]
	public static class Albino_GetDescription_Translate
	{
		static void Postfix(Albino __instance, ref string __result)
		{
			/*
			return "Your skin, hair, and eyes are absent of pigment.\n\nYou regenerate hit points at one-fifth the usual rate in the daylight.";
			*/
			__result = "피부, 머리카락, 눈에는 색소가 없습니다.\n\n당신은 낮 동안의 일반적인 속도의 1/5 속도로 체력을 재생성합니다.";
		}
	}

	[HarmonyPatch(typeof(Albino), nameof(Albino.GetLevelText))]
	public static class Albino_GetLevelText_Translate
	{
		static void Postfix(Albino __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Amnesia), nameof(Amnesia.GetDescription))]
	public static class Amnesia_GetDescription_Translate
	{
		static void Postfix(Amnesia __instance, ref string __result)
		{
			/*
			return "You forget things and places.\n\nWhenever you learn a new secret, there's a small chance you forget a secret.\nWhenever you return to a map you previously visited, there's a small chance you forget the layout.";
			*/
			__result = "당신은 사물과 장소를 잊어버립니다.\n\n새로운 비밀을 배울 때마다 비밀을 잊어버릴 가능성이 적습니다.\n이전에 방문했던 지도로 돌아갈 때마다 레이아웃을 잊어버릴 가능성이 적습니다.";
		}
	}

	[HarmonyPatch(typeof(Amnesia), nameof(Amnesia.GetLevelText))]
	public static class Amnesia_GetLevelText_Translate
	{
		static void Postfix(Amnesia __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Amphibious), nameof(Amphibious.GetDescription))]
	public static class Amphibious_GetDescription_Translate
	{
		static void Postfix(Amphibious __instance, ref string __result)
		{
			/*
			return "Your skin must be kept moist with fresh water.\n\nYou pour water on yourself rather than drinking it to quench your thirst.\nYou require about two-thirds more water than usual.\n+100 reputation with {{w|frogs}}";
			*/
			__result = "깨끗한 물로 피부를 촉촉하게 유지해야 합니다.\n\n갈증을 해소하기 위해 물을 마시는 것이 아니라 스스로 물을 붓는 것입니다.\n평소보다 약 2/3 더 많은 물이 필요합니다.\n{{w|개구리}} 평판 +100";
		}
	}

	[HarmonyPatch(typeof(Amphibious), nameof(Amphibious.GetLevelText))]
	public static class Amphibious_GetLevelText_Translate
	{
		static void Postfix(Amphibious __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Analgesia), nameof(Analgesia.GetDescription))]
	public static class Analgesia_GetDescription_Translate
	{
		static void Postfix(Analgesia __instance, ref string __result)
		{
			/*
			return "You lack a developed sense of pain.\n\nYou only know your general state of health and not your precise number of hit points.";
			*/
			__result = "당신은 발달된 통증 감각이 부족합니다.\n\n당신은 당신의 일반적인 건강 상태만 알고 정확한 체력 수는 알 수 없습니다.";
		}
	}

	[HarmonyPatch(typeof(Analgesia), nameof(Analgesia.GetLevelText))]
	public static class Analgesia_GetLevelText_Translate
	{
		static void Postfix(Analgesia __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Astral), nameof(Astral.GetDescription))]
	public static class Astral_GetDescription_Translate
	{
		static void Postfix(Astral __instance, ref string __result)
		{
			/*
			return "You live in an alternate plane of reality.";
			*/
			__result = "당신은 현실의 대체 평면에 살고 있습니다.";
		}
	}

	[HarmonyPatch(typeof(Astral), nameof(Astral.GetLevelText))]
	public static class Astral_GetLevelText_Translate
	{
		static void Postfix(Astral __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Beak), nameof(Beak.GetLevelText))]
	public static class Beak_GetLevelText_Translate
	{
		static void Postfix(Beak __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Beguiling), nameof(Beguiling.GetDescription))]
	public static class Beguiling_GetDescription_Translate
	{
		static void Postfix(Beguiling __instance, ref string __result)
		{
			/*
			return "You beguile a nearby creature into serving you loyally.";
			*/
			__result = "당신은 근처의 생물을 속여 당신에게 충성스럽게 봉사하게 합니다.";
		}
	}

	[HarmonyPatch(typeof(Beguiling), nameof(Beguiling.GetLevelText))]
	public static class Beguiling_GetLevelText_Translate
	{
		static void Postfix(Beguiling __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat("Mental attack versus a creature with a mind\n" + "Success roll: {{rules|mutation rank}} or Ego mod (whichever is higher) + character level + 1d8 VS. Defender MA + character level\n", "Range: 1\n"), "Beguiled creature: +{{rules|", (Level * 5).ToString(), "}} bonus hit points\n"), "Cooldown: 50 rounds");
			*/
			var value0 = (__0 * 5).ToString();
			__result = $"정신 공격 대 정신을 가진 생물\n성공 판정: {{{{rules|돌연변이 순위}}}} 또는 Ego 모드(둘 중 더 높은 것) + 캐릭터 레벨 + 1d8 VS. 디펜더 MA + 캐릭터 레벨\n범위: 1\n미혹된 생물: +{{{{rules|{value0}}}}} 보너스 체력\n쿨다운: 50라운드";
		}
	}

	[HarmonyPatch(typeof(Belcher), nameof(Belcher.GetDescription))]
	public static class Belcher_GetDescription_Translate
	{
		static void Postfix(Belcher __instance, ref string __result)
		{
			/*
			return Description;
			*/
			__result = __instance.Description;
		}
	}

	[HarmonyPatch(typeof(Belcher), nameof(Belcher.GetLevelText))]
	public static class Belcher_GetLevelText_Translate
	{
		static void Postfix(Belcher __instance, int __0, ref string __result)
		{
			/*
			return Description;
			*/
			__result = __instance.Description;
		}
	}

	[HarmonyPatch(typeof(BlinkingTic), nameof(BlinkingTic.GetDescription))]
	public static class BlinkingTic_GetDescription_Translate
	{
		static void Postfix(BlinkingTic __instance, ref string __result)
		{
			/*
			return "You teleport about uncontrollably.\n\nSmall chance each round you're in combat that you randomly teleport to a nearby location.";
			*/
			__result = "당신은 통제할 수 없을 정도로 순간이동합니다.\n\n전투 중 매 라운드마다 근처 위치로 무작위로 순간이동할 가능성이 적습니다.";
		}
	}

	[HarmonyPatch(typeof(BlinkingTic), nameof(BlinkingTic.GetLevelText))]
	public static class BlinkingTic_GetLevelText_Translate
	{
		static void Postfix(BlinkingTic __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(BrittleBones), nameof(BrittleBones.GetDescription))]
	public static class BrittleBones_GetDescription_Translate
	{
		static void Postfix(BrittleBones __instance, ref string __result)
		{
			/*
			return "Your bones are brittle.\n\nYou suffer 50% more damage from bludgeoning attacks, falling, and other sources of concussive damage.";
			*/
			__result = "당신의 뼈는 부서지기 쉽습니다.\n\n곤봉 공격, 낙하 및 기타 뇌진탕 피해로 인해 50% 더 많은 피해를 입습니다.";
		}
	}

	[HarmonyPatch(typeof(BrittleBones), nameof(BrittleBones.GetLevelText))]
	public static class BrittleBones_GetLevelText_Translate
	{
		static void Postfix(BrittleBones __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Burgeoning), nameof(Burgeoning.GetDescription))]
	public static class Burgeoning_GetDescription_Translate
	{
		static void Postfix(Burgeoning __instance, ref string __result)
		{
			/*
			return "You cause plants to spontaneously grow in a nearby area, hindering your enemies.";
			*/
			__result = "주변 지역에 식물이 저절로 자라게 하여 적을 방해합니다.";
		}
	}

	[HarmonyPatch(typeof(Burrowing), nameof(Burrowing.GetDescription))]
	public static class Burrowing_GetDescription_Translate
	{
		static void Postfix(Burrowing __instance, ref string __result)
		{
			/*
			return "You can travel underground by burrowing.";
			*/
			__result = "굴을 파서 지하로 이동할 수 있습니다.";
		}
	}

	[HarmonyPatch(typeof(Burrowing), nameof(Burrowing.GetLevelText))]
	public static class Burrowing_GetLevelText_Translate
	{
		static void Postfix(Burrowing __instance, int __0, ref string __result)
		{
			/*
			return "Cooldown: " + GetCooldown(Level) + " rounds\n";
			*/
			var value0 = __instance.GetCooldown(__0);
			__result = $"쿨다운: {value0} 라운드";
		}
	}

	[HarmonyPatch(typeof(BurrowingClaws), nameof(BurrowingClaws.GetDescription))]
	public static class BurrowingClaws_GetDescription_Translate
	{
		static void Postfix(BurrowingClaws __instance, ref string __result)
		{
			/*
			return Blueprint.GetTag("VariantDescription").Coalesce("You bear spade-like claws that can burrow through the earth.");
			*/
			var value0 = __instance.Blueprint.GetTag("VariantDescription").Coalesce("You bear spade-like claws that can burrow through the earth.");
			__result = $"{value0}";
		}
	}

	[HarmonyPatch(typeof(Carapace), nameof(Carapace.GetDescription))]
	public static class Carapace_GetDescription_Translate
	{
		static void Postfix(Carapace __instance, ref string __result)
		{
			/*
			return Blueprint.GetTag("VariantDescription").Coalesce("You are protected by a durable carapace.");
			*/
			var value0 = __instance.Blueprint.GetTag("VariantDescription").Coalesce("You are protected by a durable carapace.");
			__result = $"{value0}";
		}
	}

	[HarmonyPatch(typeof(Carnivorous), nameof(Carnivorous.GetDescription))]
	public static class Carnivorous_GetDescription_Translate
	{
		static void Postfix(Carnivorous __instance, ref string __result)
		{
			/*
			return "You eat meat exclusively.\n\nYou get no satiation from foods that aren't meat.\nIf you eat raw food that isn't meat, there's a 50% chance you become ill for 2 hours.\nYou can't cook with plant or fungus ingredients.\nYou don't get ill when you eat raw meat.\nYou can eat raw meat without being famished.\n";
			*/
			__result = "고기만 먹습니다.\n\n고기가 아닌 음식에서는 포만감을 느끼지 못합니다.\n고기가 아닌 생식을 먹으면 2시간 동안 병에 걸릴 확률이 50%입니다.\n식물이나 곰팡이 재료로는 요리를 할 수 없습니다.\n생고기를 먹어도 아프지 않습니다.\n배고프지 않게 생고기를 먹을 수 있다.";
		}
	}

	[HarmonyPatch(typeof(Carnivorous), nameof(Carnivorous.GetLevelText))]
	public static class Carnivorous_GetLevelText_Translate
	{
		static void Postfix(Carnivorous __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Chimera), nameof(Chimera.GetDescription))]
	public static class Chimera_GetDescription_Translate
	{
		static void Postfix(Chimera __instance, ref string __result)
		{
			/*
			return "You only manifest physical mutations, and all of your mutation choices when manifesting a new mutation are physical.\n\n" + "Whenever you manifest a new mutation, one of your choices will also cause you to grow a new limb at random.";
			*/
			__result = "물리적인 돌연변이만 나타나며, 새로운 돌연변이가 나타날 때의 모든 돌연변이 선택은 물리적입니다.\n\n새로운 돌연변이가 나타날 때마다 선택 사항 중 하나를 선택하면 무작위로 새로운 팔다리가 자라게 됩니다.";
		}
	}

	[HarmonyPatch(typeof(Chimera), nameof(Chimera.GetLevelText))]
	public static class Chimera_GetLevelText_Translate
	{
		static void Postfix(Chimera __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(ColdAbsorption), nameof(ColdAbsorption.GetDescription))]
	public static class ColdAbsorption_GetDescription_Translate
	{
		static void Postfix(ColdAbsorption __instance, ref string __result)
		{
			/*
			return "You regenerate by absorbing cold.";
			*/
			__result = "추위를 흡수하면 재생됩니다.";
		}
	}

	[HarmonyPatch(typeof(ColdBlooded), nameof(ColdBlooded.GetDescription))]
	public static class ColdBlooded_GetDescription_Translate
	{
		static void Postfix(ColdBlooded __instance, ref string __result)
		{
			/*
			return "Your vitality depends on your temperature; at higher temperatures, you are more lively. At lower temperatures, you are more torpid.\n\nYour base quickness score is reduced by 10.\nYour quickness increases as your temperature increases and decreases as your temperature decreases.\n+100 reputation with {{w|unshelled reptiles}}";
			*/
			__result = "당신의 활력은 온도에 달려 있습니다. 더 높은 온도에서는 더 활기차게 됩니다. 낮은 온도에서는 더 멍청해집니다.\n\n기본 민첩성 점수가 10 감소합니다.\n체온이 올라가면 민첩성이 증가하고 체온이 낮아지면 민첩성이 감소합니다.\n{{w|껍질을 벗기지 않은 파충류}} 평판 +100";
		}
	}

	[HarmonyPatch(typeof(ColdBlooded), nameof(ColdBlooded.GetLevelText))]
	public static class ColdBlooded_GetLevelText_Translate
	{
		static void Postfix(ColdBlooded __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Confusion), nameof(Confusion.GetDescription))]
	public static class Confusion_GetDescription_Translate
	{
		static void Postfix(Confusion __instance, ref string __result)
		{
			/*
			return "You confuse nearby enemies.";
			*/
			__result = "근처의 적을 혼란스럽게 합니다.";
		}
	}

	[HarmonyPatch(typeof(Confusion), nameof(Confusion.GetLevelText))]
	public static class Confusion_GetLevelText_Translate
	{
		static void Postfix(Confusion __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat(string.Concat("Affected creatures act semi-randomly and receive a {{rules|-" + GetMentalPenalty(Level) + "}} penalty to their mental abilities.\n", "Cone angle: {{rules|", GetConeAngle(Level).ToString(), "}} degrees\n"), "Cone length: {{rules|", GetConeLength(Level).ToString(), "}}\n"), "Duration: {{rules|", GetDuration(Level), "}} rounds\n"), "Cooldown: ", GetCooldown(Level).ToString(), " rounds");
			*/
			var value0 = Confusion.GetMentalPenalty(__0);
			var value1 = Confusion.GetConeAngle(__0).ToString();
			var value2 = Confusion.GetConeLength(__0).ToString();
			var value3 = Confusion.GetDuration(__0);
			var value4 = Confusion.GetCooldown(__0).ToString();
			__result = $"영향을 받은 생물은 반 무작위로 행동하고 정신 능력에 {{{{rules|-{value0}}}}} 페널티를 받습니다.\n원뿔 각도: {{{{rules|{value1}}}}}도\n콘 길이: {{{{rules|{value2}}}}}\n지속 시간: {{{{rules|{value3}}}}} 라운드\n쿨다운: {value4} 라운드";
		}
	}

	[HarmonyPatch(typeof(ConfusionBreather), nameof(ConfusionBreather.GetDescription))]
	public static class ConfusionBreather_GetDescription_Translate
	{
		static void Postfix(ConfusionBreather __instance, ref string __result)
		{
			/*
			return "You breathe confusion gas.";
			*/
			__result = "당신은 혼란가스를 들이마십니다.";
		}
	}

	[HarmonyPatch(typeof(ConfusionBreather), nameof(ConfusionBreather.GetLevelText))]
	public static class ConfusionBreather_GetLevelText_Translate
	{
		static void Postfix(ConfusionBreather __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat("Breathes confusion gas in a cone.\n" + "Cone length: " + GetConeLength() + " tiles\n", "Cone angle: ", GetConeAngle().ToString(), " degrees\n"), "Cooldown: 15 rounds\n");
			*/
			var value0 = __instance.GetConeLength();
			var value1 = __instance.GetConeAngle().ToString();
			__result = $"원뿔 형태로 혼란 가스를 흡입합니다.\n원뿔 길이: {value0} 타일\n원뿔 각도: {value1}도\n쿨타임: 15라운드";
		}
	}

	[HarmonyPatch(typeof(CorrosiveBreather), nameof(CorrosiveBreather.GetDescription))]
	public static class CorrosiveBreather_GetDescription_Translate
	{
		static void Postfix(CorrosiveBreather __instance, ref string __result)
		{
			/*
			return "You breathe corrosive gas.";
			*/
			__result = "부식성 가스를 흡입합니다.";
		}
	}

	[HarmonyPatch(typeof(CorrosiveBreather), nameof(CorrosiveBreather.GetLevelText))]
	public static class CorrosiveBreather_GetLevelText_Translate
	{
		static void Postfix(CorrosiveBreather __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat("Breathes corrosive gas in a cone.\n" + "Cone length: " + GetConeLength() + " tiles\n", "Cone angle: ", GetConeAngle().ToString(), " degrees\n"), "Cooldown: 15 rounds\n");
			*/
			var value0 = __instance.GetConeLength();
			var value1 = __instance.GetConeAngle().ToString();
			__result = $"콘에서 부식성 가스를 흡입합니다.\n원뿔 길이: {value0} 타일\n원뿔 각도: {value1}도\n쿨타임: 15라운드";
		}
	}

	[HarmonyPatch(typeof(CrungleGaze), nameof(CrungleGaze.GetDescription))]
	public static class CrungleGaze_GetDescription_Translate
	{
		static void Postfix(CrungleGaze __instance, ref string __result)
		{
			/*
			return "You provoke waking dreams with your gaze.";
			*/
			__result = "당신은 시선으로 깨어있는 꿈을 불러일으킵니다.";
		}
	}

	[HarmonyPatch(typeof(CrungleGaze), nameof(CrungleGaze.GetLevelText))]
	public static class CrungleGaze_GetLevelText_Translate
	{
		static void Postfix(CrungleGaze __instance, int __0, ref string __result)
		{
			/*
			return "You can gaze {{rules|" + GetRange(Level) + "}} squares after a " + Grammar.Cardinal(GetDelay(Level)) + "-turn warmup and send your target to a waking dream.\nCooldown: {{rules|" + GetCooldown(Level) + "}} rounds";
			*/
			var value0 = __instance.GetRange(__0);
			var value1 = Grammar.Cardinal(__instance.GetDelay(__0));
			var value2 = __instance.GetCooldown(__0);
			__result = $"{value1}턴 워밍업 후 {{{{rules|{value0}}}}} 사각형을 응시하고 대상을 깨어 있는 꿈으로 보낼 수 있습니다.\n쿨다운: {{{{rules|{value2}}}}} 라운드";
		}
	}

	[HarmonyPatch(typeof(Cryokinesis), nameof(Cryokinesis.GetDescription))]
	public static class Cryokinesis_GetDescription_Translate
	{
		static void Postfix(Cryokinesis __instance, ref string __result)
		{
			/*
			return "You chill a nearby area with your mind.";
			*/
			__result = "당신은 마음으로 근처 지역을 식힙니다.";
		}
	}

	[HarmonyPatch(typeof(Crystallinity), nameof(Crystallinity.GetDescription))]
	public static class Crystallinity_GetDescription_Translate
	{
		static void Postfix(Crystallinity __instance, ref string __result)
		{
			/*
			return "You are a crystalline being.";
			*/
			__result = "당신은 결정체 존재입니다.";
		}
	}

	[HarmonyPatch(typeof(Crystallinity), nameof(Crystallinity.GetLevelText))]
	public static class Crystallinity_GetLevelText_Translate
	{
		static void Postfix(Crystallinity __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat("" + "+4 AV\n", "-50 Electrical Resistance\n"), "25% chance to refract light-based attacks\n"), "Effects that make non-biological clones of you produce twice as many.");
			*/
			__result = "+4 AV\n-50 전기 저항\n빛 기반 공격을 굴절시킬 확률 25%\n당신의 비생물학적 복제물을 만드는 효과는 두 배나 더 많이 생산합니다.";
		}
	}

	[HarmonyPatch(typeof(DarkVision), nameof(DarkVision.GetDescription))]
	public static class DarkVision_GetDescription_Translate
	{
		static void Postfix(DarkVision __instance, ref string __result)
		{
			/*
			return "You see in the dark.";
			*/
			__result = "어둠 속에서도 보입니다.";
		}
	}

	[HarmonyPatch(typeof(DarkVision), nameof(DarkVision.GetLevelText))]
	public static class DarkVision_GetLevelText_Translate
	{
		static void Postfix(DarkVision __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Decarbonizer), nameof(Decarbonizer.GetDescription))]
	public static class Decarbonizer_GetDescription_Translate
	{
		static void Postfix(Decarbonizer __instance, ref string __result)
		{
			/*
			return "You extract carbon from living material.";
			*/
			__result = "살아있는 물질에서 탄소를 추출합니다.";
		}
	}

	[HarmonyPatch(typeof(Decarbonizer), nameof(Decarbonizer.GetLevelText))]
	public static class Decarbonizer_GetLevelText_Translate
	{
		static void Postfix(Decarbonizer __instance, int __0, ref string __result)
		{
			/*
			return string.Concat("Shoots a beam with a 3-round windup that dismembers the limbs of its targets.\n" + "Beam distance: {{rules|" + GetBeamDistance(Level) + "}} spaces\n", "Cooldown: ", GetCooldown(Level).ToString(), " rounds");
			*/
			var value0 = __instance.GetBeamDistance(__0);
			var value1 = Teleportation.GetCooldown(__0).ToString();
			__result = $"대상의 사지를 절단하는 3라운드 와인드업으로 빔을 발사합니다.\n빔 거리: {{{{rules|{value0}}}}} 공백\n쿨다운: {value1} 라운드";
		}
	}

	[HarmonyPatch(typeof(DefensiveChromatophores), nameof(DefensiveChromatophores.GetDescription))]
	public static class DefensiveChromatophores_GetDescription_Translate
	{
		static void Postfix(DefensiveChromatophores __instance, ref string __result)
		{
			/*
			return "In stressful situations, you scintillate.";
			*/
			__result = "스트레스가 많은 상황에서는 반짝반짝 빛납니다.";
		}
	}

	[HarmonyPatch(typeof(DefensiveChromatophores), nameof(DefensiveChromatophores.GetLevelText))]
	public static class DefensiveChromatophores_GetLevelText_Translate
	{
		static void Postfix(DefensiveChromatophores __instance, int __0, ref string __result)
		{
			/*
			return "You can't act while scintillating.\nConfuses nearby hostile creatures per Confusion rank " + Level + ".\nDuration: 5 rounds\nCooldown: 200 rounds";
			*/
			var value0 = __0;
			__result = $"반짝이는 동안에는 행동할 수 없습니다.\n혼란 등급 {value0}에 따라 근처의 적대적인 생물을 혼란시킵니다.\n지속시간: 5라운드\n쿨타임: 200발";
		}
	}

	[HarmonyPatch(typeof(Disintegration), nameof(Disintegration.GetDescription))]
	public static class Disintegration_GetDescription_Translate
	{
		static void Postfix(Disintegration __instance, ref string __result)
		{
			/*
			return "You disintegrate nearby matter.";
			*/
			__result = "근처의 물질을 분해합니다.";
		}
	}

	[HarmonyPatch(typeof(Disintegration), nameof(Disintegration.GetLevelText))]
	public static class Disintegration_GetLevelText_Translate
	{
		static void Postfix(Disintegration __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat(string.Concat("" + "Area: 7x7 around self\n", "Damage to non-structural objects: {{rules|", GetNonStructuralDamage(Level), "}}\n"), "Damage to structural objects: {{rules|", GetStructuralDamage(Level), "}}\n"), "You are exhausted for 3 rounds after using this power.\n"), "Cooldown: ", GetCooldown(Level).ToString(), " rounds");
			*/
			var value0 = Disintegration.GetNonStructuralDamage(__0);
			var value1 = Disintegration.GetStructuralDamage(__0);
			var value2 = __instance.GetCooldown(__0).ToString();
			__result = $"영역: 자신 주변 7x7\n비구조물에 대한 손상: {{{{rules|{value0}}}}}\n구조물 손상: {{{{rules|{value1}}}}}\n이 힘을 사용한 후 3라운드 동안 탈진합니다.\n쿨다운: {value2} 라운드";
		}
	}

	[HarmonyPatch(typeof(Domination), nameof(Domination.GetDescription))]
	public static class Domination_GetDescription_Translate
	{
		static void Postfix(Domination __instance, ref string __result)
		{
			/*
			return "You garrote an adjacent creature's mind and control its actions while your own body lies dormant.";
			*/
			__result = "당신은 자신의 몸이 휴면 상태에 있는 동안 인접한 생물의 정신을 교살하고 그 행동을 제어합니다.";
		}
	}

	[HarmonyPatch(typeof(Domination), nameof(Domination.GetLevelText))]
	public static class Domination_GetLevelText_Translate
	{
		static void Postfix(Domination __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat("Mental attack versus creature with a mind\n" + "Success roll: {{rules|mutation rank}} or Ego mod (whichever is higher) + character level + 1d8 VS. Defender MA + character level\n", "Range: 1\n"), "Duration: {{rules|", GetDuration(Level).ToString(), "}} rounds\n"), "Cooldown: 75 rounds");
			*/
			var value0 = __instance.GetDuration(__0).ToString();
			__result = $"정신 공격 대 정신을 가진 생물\n성공 판정: {{{{rules|돌연변이 순위}}}} 또는 Ego 모드(둘 중 더 높은 것) + 캐릭터 레벨 + 1d8 VS. 디펜더 MA + 캐릭터 레벨\n범위: 1\n지속 시간: {{{{rules|{value0}}}}} 라운드\n쿨타임: 75라운드";
		}
	}

	[HarmonyPatch(typeof(Dystechnia), nameof(Dystechnia.GetLevelText))]
	public static class Dystechnia_GetLevelText_Translate
	{
		static void Postfix(Dystechnia __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(ElectricalGeneration), nameof(ElectricalGeneration.GetDescription))]
	public static class ElectricalGeneration_GetDescription_Translate
	{
		static void Postfix(ElectricalGeneration __instance, ref string __result)
		{
			/*
			return "You accrue electrical charge that you can use and discharge to deal damage.";
			*/
			__result = "피해를 입히기 위해 사용하고 방전할 수 있는 전하가 축적됩니다.";
		}
	}

	[HarmonyPatch(typeof(ElectromagneticImpulse), nameof(ElectromagneticImpulse.GetDescription))]
	public static class ElectromagneticImpulse_GetDescription_Translate
	{
		static void Postfix(ElectromagneticImpulse __instance, ref string __result)
		{
			/*
			return "You involuntarily release electromagnetic pulses, deactivating robots and artifacts around yourself.\n\nSmall chance each round you're in combat that you release an electromagnetic pulse with radius 3, deactivating robots and artifacts (including those you carry) for 11-20 rounds.";
			*/
			__result = "당신은 무의식적으로 전자기 펄스를 방출하여 주변의 로봇과 인공물을 비활성화합니다.\n\n전투 중 매 라운드마다 반경 3의 전자기 펄스를 방출하여 11~20라운드 동안 로봇과 인공물(휴대하는 것 포함)을 비활성화할 가능성이 적습니다.";
		}
	}

	[HarmonyPatch(typeof(ElectromagneticImpulse), nameof(ElectromagneticImpulse.GetLevelText))]
	public static class ElectromagneticImpulse_GetLevelText_Translate
	{
		static void Postfix(ElectromagneticImpulse __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(ElectromagneticPulse), nameof(ElectromagneticPulse.GetDescription))]
	public static class ElectromagneticPulse_GetDescription_Translate
	{
		static void Postfix(ElectromagneticPulse __instance, ref string __result)
		{
			/*
			return "You generate an electromagnetic pulse that disables nearby artifacts and machines.";
			*/
			__result = "근처의 인공물과 기계를 비활성화하는 전자기 펄스를 생성합니다.";
		}
	}

	[HarmonyPatch(typeof(ElectromagneticPulse), nameof(ElectromagneticPulse.GetLevelText))]
	public static class ElectromagneticPulse_GetLevelText_Translate
	{
		static void Postfix(ElectromagneticPulse __instance, int __0, ref string __result)
		{
			/*
			int num = GetRadius(Level) * 2 + 1;
						string text = "Area: {{rules|" + num + "x" + num + "}} centered around yourself\n";
						text = text + "Duration: {{rules|" + (4 + Level * 2) + "-" + (13 + Level * 2) + "}} rounds\n";
						return text + "Cooldown: " + GetCooldown(Level) + " rounds";
			*/
			int num = ElectromagneticPulse.GetRadius(__0) * 2 + 1;
			int cooldown = ElectromagneticPulse.GetCooldown(__0);

			string text = $"영역: 플레이어 중심 {{{{rules|{num}x{num}}}}}\n";
			text = text + $"지속시간: {{{{rules|{4 + __0 * 2}-{13 + __0 * 2}}}}} 라운드\n";
			__result = text + $"쿨다운: {cooldown} 라운드";
		}
	}

	[HarmonyPatch(typeof(ErosTeleportation), nameof(ErosTeleportation.GetDescription))]
	public static class ErosTeleportation_GetDescription_Translate
	{
		static void Postfix(ErosTeleportation __instance, ref string __result)
		{
			/*
			return "You teleport to a nearby location near your leader.";
			*/
			__result = "리더 근처의 가까운 위치로 순간이동합니다.";
		}
	}

	[HarmonyPatch(typeof(ErosTeleportation), nameof(ErosTeleportation.GetLevelText))]
	public static class ErosTeleportation_GetLevelText_Translate
	{
		static void Postfix(ErosTeleportation __instance, int __0, ref string __result)
		{
			/*
			return "Cooldown: " + GetCooldown(Level) + " rounds";
			*/
			var value0 = __instance.GetCooldown(__0);
			__result = $"쿨다운: {value0} 라운드";
		}
	}

	[HarmonyPatch(typeof(Esper), nameof(Esper.GetLevelText))]
	public static class Esper_GetLevelText_Translate
	{
		static void Postfix(Esper __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(EvilTwin), nameof(EvilTwin.GetDescription))]
	public static class EvilTwin_GetDescription_Translate
	{
		static void Postfix(EvilTwin __instance, ref string __result)
		{
			/*
			return "Acting on some inscrutable impulse, a parallel version of yourself travels through space and time to destroy you.\n\nEach time you embark on a new location, there's a small chance your evil twin has tracked you there and attempts to kill you.";
			*/
			__result = "알 수 없는 충동에 따라 자신의 평행 버전이 공간과 시간을 여행하며 당신을 파괴합니다.\n\n새로운 장소로 이동할 때마다 사악한 쌍둥이가 그곳에서 당신을 추적하여 죽이려고 시도할 가능성이 적습니다.";
		}
	}

	[HarmonyPatch(typeof(EvilTwin), nameof(EvilTwin.GetLevelText))]
	public static class EvilTwin_GetLevelText_Translate
	{
		static void Postfix(EvilTwin __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(FearAura), nameof(FearAura.GetDescription))]
	public static class FearAura_GetDescription_Translate
	{
		static void Postfix(FearAura __instance, ref string __result)
		{
			/*
			return "You scare creatures around you.";
			*/
			__result = "당신은 주변의 생물들에게 겁을 줍니다.";
		}
	}

	[HarmonyPatch(typeof(FearAura), nameof(FearAura.GetLevelText))]
	public static class FearAura_GetLevelText_Translate
	{
		static void Postfix(FearAura __instance, int __0, ref string __result)
		{
			/*
			return string.Concat("You cause adjacent creatures to flee in terror.\n" + "Cooldown: " + GetCooldown(Level) + " rounds", "Additionally, you sometimes scare creatures passively.");
			*/
			var value0 = __instance.GetCooldown(__0);
			__result = $"당신은 인접한 생명체들을 공포에 질려 도망치게 만듭니다.\n재사용 대기시간: {value0} 라운드 게다가 때로는 수동적으로 생물에게 겁을 주기도 합니다.";
		}
	}

	[HarmonyPatch(typeof(FireBreather), nameof(FireBreather.GetDescription))]
	public static class FireBreather_GetDescription_Translate
	{
		static void Postfix(FireBreather __instance, ref string __result)
		{
			/*
			return "You breathe fire.";
			*/
			__result = "당신은 불을 뿜습니다.";
		}
	}

	[HarmonyPatch(typeof(FlamingRay), nameof(FlamingRay.GetLevelText))]
	public static class FlamingRay_GetLevelText_Translate
	{
		static void Postfix(FlamingRay __instance, int __0, ref string __result)
		{
			/*
			int rANGE = RANGE;
						return string.Concat(string.Concat(string.Concat("Emits a " + rANGE + "-square ray of flame in the direction of your choice.\n", "Damage: {{rules|", ComputeDamage(level), "}}\n"), "Cooldown: 10 rounds\n"), "Melee attacks heat opponents by {{rules|", GetHeatOnHitAmount(level), "}} degrees");
			*/
			int range = FlamingRay.RANGE;
			string damage = __instance.ComputeDamage(__0);
			string heat = __instance.GetHeatOnHitAmount(__0);
			__result = $"선택한 방향으로 {range} 정사각형의 화염 광선을 방출합니다.\n피해량: {{{{rules|{damage}}}}}\n쿨다운: 10 라운드\n근접 공격은 상대를 {{{{rules|{heat}}}}}도만큼 가열합니다.";
		}
	}

	[HarmonyPatch(typeof(ForceBubble), nameof(ForceBubble.GetDescription))]
	public static class ForceBubble_GetDescription_Translate
	{
		static void Postfix(ForceBubble __instance, ref string __result)
		{
			/*
			return "You generate a forcefield around yourself.";
			*/
			__result = "당신은 자신 주위에 역장을 생성합니다.";
		}
	}

	[HarmonyPatch(typeof(ForceBubble), nameof(ForceBubble.GetLevelText))]
	public static class ForceBubble_GetLevelText_Translate
	{
		static void Postfix(ForceBubble __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat("Creates a 3x3 forcefield centered on yourself\n" + "Duration: {{rules|" + GetDuration(Level) + "}} rounds\n", "Cooldown: 100 rounds\n"), "You may fire missile weapons through the forcefield.");
			*/
			var value0 = __instance.GetDuration(__0);
			__result = $"자신을 중심으로 3x3 역장을 생성합니다.\n지속 시간: {{{{rules|{value0}}}}} 라운드\n쿨다운: 100라운드\n역장을 통해 미사일 무기를 발사할 수 있습니다.";
		}
	}

	[HarmonyPatch(typeof(ForceWall), nameof(ForceWall.GetDescription))]
	public static class ForceWall_GetDescription_Translate
	{
		static void Postfix(ForceWall __instance, ref string __result)
		{
			/*
			return "You generate a wall of force that protects you from your enemies.";
			*/
			__result = "당신은 적으로부터 당신을 보호하는 힘의 벽을 생성합니다.";
		}
	}

	[HarmonyPatch(typeof(ForceWall), nameof(ForceWall.GetLevelText))]
	public static class ForceWall_GetLevelText_Translate
	{
		static void Postfix(ForceWall __instance, int __0, ref string __result)
		{
			/*
			int cooldown = GetCooldown(Level);
						return string.Concat(string.Concat("Creates 9 contiguous squares of immobile forcefield.\n" + "Duration: {{rules|" + GetDuration(Level) + "}} rounds\n", "Cooldown: ", cooldown.ToString(), " rounds\n"), "You may fire missile weapons through the forcefield.");
			*/
			int cooldown = __instance.GetCooldown(__0);
			var value0 = __instance.GetDuration(__0);
			var value1 = cooldown.ToString();
			__result = $"움직이지 않는 역장의 연속 사각형 9개를 생성합니다.\n지속 시간: {{{{rules|{value0}}}}} 라운드\n쿨다운: {value1} 라운드\n역장을 통해 미사일 무기를 발사할 수 있습니다.";
		}
	}

	[HarmonyPatch(typeof(FreezeBreath), nameof(FreezeBreath.GetDescription))]
	public static class FreezeBreath_GetDescription_Translate
	{
		static void Postfix(FreezeBreath __instance, ref string __result)
		{
			/*
			return "You emit jets of frost from your mouth.";
			*/
			__result = "입에서 서리가 내뿜어집니다.";
		}
	}

	[HarmonyPatch(typeof(FreezeBreath), nameof(FreezeBreath.GetLevelText))]
	public static class FreezeBreath_GetLevelText_Translate
	{
		static void Postfix(FreezeBreath __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat("Emits a " + Range + "-square ray of frost in the direction of your choice\n", "Cooldown: 30 rounds\n"), "Damage: ", ComputeDamage(Level), "\n"), "Cannot wear face accessories");
			*/
			var value0 = __instance.Range;
			var value1 = __instance.ComputeDamage(__0);
			__result = $"선택한 방향으로 {value0} 정사각형 서리 광선을 방출합니다.\n쿨타임: 30라운드\n피해량: {value1}\n얼굴 액세서리 착용 불가";
		}
	}

	[HarmonyPatch(typeof(FrostWebs), nameof(FrostWebs.GetDescription))]
	public static class FrostWebs_GetDescription_Translate
	{
		static void Postfix(FrostWebs __instance, ref string __result)
		{
			/*
			return string.Concat(string.Concat("You fill a nearby area with frosty webs.\n\n" + "Range: 12\n", "Area: 3x3\n"), "Cooldown: 30 rounds\n");
			*/
			__result = "근처 지역을 서리가 내린 거미줄로 채웁니다.\n\n범위: 12\n면적: 3x3\n쿨타임: 30라운드";
		}
	}

	[HarmonyPatch(typeof(FrostWebs), nameof(FrostWebs.GetLevelText))]
	public static class FrostWebs_GetLevelText_Translate
	{
		static void Postfix(FrostWebs __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(GasGeneration), nameof(GasGeneration.GetDescription))]
	public static class GasGeneration_GetDescription_Translate
	{
		static void Postfix(GasGeneration __instance, ref string __result)
		{
			/*
			return Description;
			*/
			GameObjectBlueprint blueprint = GameObjectFactory.Factory.GetBlueprint(__instance.GasObject);
			string displayName = blueprint?.GetPartParameter<string>("Render", "DisplayName");
			if (!string.IsNullOrEmpty(displayName))
			{
			    __result = $"당신은 자신 주위에 {displayName}의 폭발을 방출합니다.";
			}
			else
			{
			    __result = "당신은 자신 주위에 기체 폭발을 방출합니다.";
			}
		}
	}

	[HarmonyPatch(typeof(Gigantism), nameof(Gigantism.GetDescription))]
	public static class Gigantism_GetDescription_Translate
	{
		static void Postfix(Gigantism __instance, ref string __result)
		{
			/*
			return "You are unusually large.";
			*/
			__result = "당신은 비정상적으로 큽니다.";
		}
	}

	[HarmonyPatch(typeof(Gigantism), nameof(Gigantism.GetLevelText))]
	public static class Gigantism_GetLevelText_Translate
	{
		static void Postfix(Gigantism __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat("" + "Can only use most equipment if it is gigantic.\n", "Cannot enter small spaces.\n"), "Heavier.\n"), "Can carry more.");
			*/
			__result = "장비가 거대할 경우에만 대부분의 장비를 사용할 수 있습니다.\n작은 공간에는 들어갈 수 없습니다.\n더 무겁다.\n더 많이 가지고 다닐 수 있습니다.";
		}
	}

	[HarmonyPatch(typeof(HeatAbsorption), nameof(HeatAbsorption.GetDescription))]
	public static class HeatAbsorption_GetDescription_Translate
	{
		static void Postfix(HeatAbsorption __instance, ref string __result)
		{
			/*
			return "You regenerate by absorbing heat.";
			*/
			__result = "열을 흡수하여 재생됩니다.";
		}
	}

	[HarmonyPatch(typeof(HeightenedAgility), nameof(HeightenedAgility.GetDescription))]
	public static class HeightenedAgility_GetDescription_Translate
	{
		static void Postfix(HeightenedAgility __instance, ref string __result)
		{
			/*
			return "Your joints stretch much further than usual.";
			*/
			__result = "관절이 평소보다 훨씬 더 많이 늘어납니다.";
		}
	}

	[HarmonyPatch(typeof(HeightenedAgility), nameof(HeightenedAgility.GetLevelText))]
	public static class HeightenedAgility_GetLevelText_Translate
	{
		static void Postfix(HeightenedAgility __instance, int __0, ref string __result)
		{
			/*
			return string.Concat("+{{rules|" + (2 + (Level - 1) / 2) + "}} Agility\n", "{{rules|", GetCooldownCancelChance(Level).ToString(), "%}} chance that Sprint and skills with Agility prerequisites don't go on cooldown after use");
			*/
			var value0 = (2 + (__0 - 1) / 2);
			var value1 = __instance.GetCooldownCancelChance(__0).ToString();
			__result = $"+{{{{rules|{value0}}}}} 민첩성\n{{{{rules|{value1}%}}}} 민첩성 전제 조건이 있는 스프린트 및 스킬 사용 후 재사용 대기시간이 발생하지 않을 가능성";
		}
	}

	[HarmonyPatch(typeof(HeightenedEgo), nameof(HeightenedEgo.GetDescription))]
	public static class HeightenedEgo_GetDescription_Translate
	{
		static void Postfix(HeightenedEgo __instance, ref string __result)
		{
			/*
			return "You possess a towering vision of self that you project onto the minds of nearby creatures.";
			*/
			__result = "당신은 근처 생물의 마음에 투사하는 우뚝 솟은 자아 비전을 가지고 있습니다.";
		}
	}

	[HarmonyPatch(typeof(HeightenedHearing), nameof(HeightenedHearing.GetDescription))]
	public static class HeightenedHearing_GetDescription_Translate
	{
		static void Postfix(HeightenedHearing __instance, ref string __result)
		{
			/*
			return "You are possessed of unnaturally acute hearing.";
			*/
			__result = "당신은 비정상적으로 예민한 청각을 가지고 있습니다.";
		}
	}

	[HarmonyPatch(typeof(HeightenedIntelligence), nameof(HeightenedIntelligence.GetDescription))]
	public static class HeightenedIntelligence_GetDescription_Translate
	{
		static void Postfix(HeightenedIntelligence __instance, ref string __result)
		{
			/*
			return "You possess extraordinary analytical prowess but you find difficulty in relating to others.";
			*/
			__result = "당신은 뛰어난 분석력을 가지고 있지만 다른 사람들과 관계를 맺는 데 어려움을 겪습니다.";
		}
	}

	[HarmonyPatch(typeof(HeightenedSmell), nameof(HeightenedSmell.GetDescription))]
	public static class HeightenedSmell_GetDescription_Translate
	{
		static void Postfix(HeightenedSmell __instance, ref string __result)
		{
			/*
			return "You are possessed of exceptionally acute smell.";
			*/
			__result = "당신은 유난히 심한 냄새를 풍기는 사람입니다.";
		}
	}

	[HarmonyPatch(typeof(HeightenedSpeed), nameof(HeightenedSpeed.GetDescription))]
	public static class HeightenedSpeed_GetDescription_Translate
	{
		static void Postfix(HeightenedSpeed __instance, ref string __result)
		{
			/*
			return "You are gifted with tremendous speed.";
			*/
			__result = "당신은 엄청난 속도를 타고났습니다.";
		}
	}

	[HarmonyPatch(typeof(HeightenedSpeed), nameof(HeightenedSpeed.GetLevelText))]
	public static class HeightenedSpeed_GetLevelText_Translate
	{
		static void Postfix(HeightenedSpeed __instance, int __0, ref string __result)
		{
			/*
			return "+{{rules|" + GetSpeedBonus(Level) + "}} Quickness";
			*/
			var value0 = __instance.GetSpeedBonus(__0);
			__result = $"+{{{{rules|{value0}}}}} 신속성";
		}
	}

	[HarmonyPatch(typeof(HeightenedStrength), nameof(HeightenedStrength.GetDescription))]
	public static class HeightenedStrength_GetDescription_Translate
	{
		static void Postfix(HeightenedStrength __instance, ref string __result)
		{
			/*
			return "You are possessed of hulking strength.";
			*/
			__result = "당신은 엄청난 힘을 가지고 있습니다.";
		}
	}

	[HarmonyPatch(typeof(HeightenedStrength), nameof(HeightenedStrength.GetLevelText))]
	public static class HeightenedStrength_GetLevelText_Translate
	{
		static void Postfix(HeightenedStrength __instance, int __0, ref string __result)
		{
			/*
			return "+{{C|" + GetStrengthBonus(Level) + "}} Strength\n{{C|" + GetDazedChance(Level) + "%}} chance to daze your opponent on a successful melee attack for 2-3 rounds";
			*/
			var value0 = __instance.GetStrengthBonus(__0);
			var value1 = __instance.GetDazedChance(__0);
			__result = $"+{{{{C|{value0}}}}} 힘\n{{{{C|{value1}%}}}} 2~3라운드 동안 성공적인 근접 공격으로 상대를 멍하게 만들 수 있는 기회";
		}
	}

	[HarmonyPatch(typeof(HeightenedWillpower), nameof(HeightenedWillpower.GetDescription))]
	public static class HeightenedWillpower_GetDescription_Translate
	{
		static void Postfix(HeightenedWillpower __instance, ref string __result)
		{
			/*
			return "You are possessed of an indefatigable focus which every so often manifests itself as stubbornness.";
			*/
			__result = "당신은 지치지 않는 집중력을 갖고 있으며, 이는 종종 완고함으로 나타납니다.";
		}
	}

	[HarmonyPatch(typeof(Hemophilia), nameof(Hemophilia.GetDescription))]
	public static class Hemophilia_GetDescription_Translate
	{
		static void Postfix(Hemophilia __instance, ref string __result)
		{
			/*
			return "Your blood does not clot easily.\n\nIt takes much longer than usual for you to stop bleeding.";
			*/
			__result = "혈액이 쉽게 응고되지 않습니다.\n\n출혈이 멈추는 데는 평소보다 훨씬 오랜 시간이 걸립니다.";
		}
	}

	[HarmonyPatch(typeof(Hemophilia), nameof(Hemophilia.GetLevelText))]
	public static class Hemophilia_GetLevelText_Translate
	{
		static void Postfix(Hemophilia __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(HooksForFeet), nameof(HooksForFeet.GetLevelText))]
	public static class HooksForFeet_GetLevelText_Translate
	{
		static void Postfix(HooksForFeet __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(IceBreather), nameof(IceBreather.GetDescription))]
	public static class IceBreather_GetDescription_Translate
	{
		static void Postfix(IceBreather __instance, ref string __result)
		{
			/*
			return "You breathe ice.";
			*/
			__result = "당신은 얼음을 호흡합니다.";
		}
	}

	[HarmonyPatch(typeof(Infiltrate), nameof(Infiltrate.GetDescription))]
	public static class Infiltrate_GetDescription_Translate
	{
		static void Postfix(Infiltrate __instance, ref string __result)
		{
			/*
			return "You teleport and bring creatures along with you.";
			*/
			__result = "당신은 순간이동하여 생물을 데리고 옵니다.";
		}
	}

	[HarmonyPatch(typeof(Infiltrate), nameof(Infiltrate.GetLevelText))]
	public static class Infiltrate_GetLevelText_Translate
	{
		static void Postfix(Infiltrate __instance, int __0, ref string __result)
		{
			/*
			return string.Concat("You teleport to a nearby location and bring everyone within radius " + GetTeleportRadius(Level) + " along with you.\n", "Cooldown: ", GetCooldown(Level).ToString(), " rounds");
			*/
			var value0 = __instance.GetTeleportRadius(__0);
			var value1 = Teleportation.GetCooldown(__0).ToString();
			__result = $"가까운 위치로 순간이동하여 반경 {value0} 내의 모든 사람을 함께 데려옵니다.\n쿨다운: {value1} 라운드";
		}
	}

	[HarmonyPatch(typeof(Invisibility), nameof(Invisibility.GetDescription))]
	public static class Invisibility_GetDescription_Translate
	{
		static void Postfix(Invisibility __instance, ref string __result)
		{
			/*
			return "You cannot be seen.";
			*/
			__result = "당신은 볼 수 없습니다.";
		}
	}

	[HarmonyPatch(typeof(Invisibility), nameof(Invisibility.GetLevelText))]
	public static class Invisibility_GetLevelText_Translate
	{
		static void Postfix(Invisibility __instance, int __0, ref string __result)
		{
			/*
			return GetDescription();
			*/
			var value0 = __instance.GetDescription();
			__result = $"{value0}";
		}
	}

	[HarmonyPatch(typeof(IrisdualBeam), nameof(IrisdualBeam.GetDescription))]
	public static class IrisdualBeam_GetDescription_Translate
	{
		static void Postfix(IrisdualBeam __instance, ref string __result)
		{
			/*
			return "You molt powerful beams across the spectrum of light and matter.";
			*/
			__result = "당신은 빛과 물질의 스펙트럼에 걸쳐 강력한 광선을 털갈이합니다.";
		}
	}

	[HarmonyPatch(typeof(IrisdualBeam), nameof(IrisdualBeam.GetLevelText))]
	public static class IrisdualBeam_GetLevelText_Translate
	{
		static void Postfix(IrisdualBeam __instance, int __0, ref string __result)
		{
			/*
			StringBuilder stringBuilder = Event.NewStringBuilder();
						string text = ((GetBeams(Level) == 1) ? "beam" : "beams");
						stringBuilder.Append("Fires {{rules|").Append(GetBeams(Level)).Append("}} " + text + " at random enemies every round for {{rules|")
							.Append(GetDuration(Level))
							.Append("}} rounds\n");
						stringBuilder.Append("Cooldown: {{rules|").Append(GetCooldown(Level)).Append("}} rounds");
						return stringBuilder.ToString();
			*/
			int beams = __instance.GetBeams(__0);
			int duration = __instance.GetDuration(__0);
			int cooldown = __instance.GetCooldown(__0);
			__result = $"매 라운드 무작위 적에게 {{{{rules|{beams}}}}}개의 광선을 {{{{rules|{duration}}}}} 라운드 동안 발사합니다.\n쿨다운: {{{{rules|{cooldown}}}}} 라운드";
		}
	}

	[HarmonyPatch(typeof(IrritableGenome), nameof(IrritableGenome.GetDescription))]
	public static class IrritableGenome_GetDescription_Translate
	{
		static void Postfix(IrritableGenome __instance, ref string __result)
		{
			/*
			return "Your genome is irritable and unpredictable.\n\nWhenever you spend a mutation point, the next mutation point you gain will be spent randomly.\nWhenever you buy a new mutation, you get a random one instead of a choice of three.";
			*/
			__result = "당신의 게놈은 짜증나고 예측할 수 없습니다.\n\n돌연변이 포인트를 사용할 때마다 다음으로 얻는 돌연변이 포인트는 무작위로 사용됩니다.\n새로운 돌연변이를 구매할 때마다 세 가지 중 하나를 선택하는 대신 무작위로 하나를 얻게 됩니다.";
		}
	}

	[HarmonyPatch(typeof(IrritableGenome), nameof(IrritableGenome.GetLevelText))]
	public static class IrritableGenome_GetLevelText_Translate
	{
		static void Postfix(IrritableGenome __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Kindle), nameof(Kindle.GetDescription))]
	public static class Kindle_GetDescription_Translate
	{
		static void Postfix(Kindle __instance, ref string __result)
		{
			/*
			return string.Concat(string.Concat("" + "You ignite a small fire with your mind.\n\n", "Range: 12\n"), "Cooldown: 50");
			*/
			__result = "당신은 마음으로 작은 불을 붙입니다.\n\n범위: 12\n쿨타임: 50";
		}
	}

	[HarmonyPatch(typeof(Kindle), nameof(Kindle.GetLevelText))]
	public static class Kindle_GetLevelText_Translate
	{
		static void Postfix(Kindle __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(LeyShifting), nameof(LeyShifting.GetDescription))]
	public static class LeyShifting_GetDescription_Translate
	{
		static void Postfix(LeyShifting __instance, ref string __result)
		{
			/*
			return string.Concat("" + "You shift spacetime in the local region.\n\n", "Cooldown: ", 250.ToString());
			*/
			var value0 = 250.ToString();
			__result = $"당신은 지역의 시공간을 이동합니다.\n\n쿨다운: {value0}";
		}
	}

	[HarmonyPatch(typeof(LeyShifting), nameof(LeyShifting.GetLevelText))]
	public static class LeyShifting_GetLevelText_Translate
	{
		static void Postfix(LeyShifting __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(LifeDrain), nameof(LifeDrain.GetDescription))]
	public static class LifeDrain_GetDescription_Translate
	{
		static void Postfix(LifeDrain __instance, ref string __result)
		{
			/*
			return "You bond with a nearby organic creature and leech its life force.";
			*/
			__result = "당신은 근처의 유기체와 결속을 맺고 그 생명력을 흡수합니다.";
		}
	}

	[HarmonyPatch(typeof(LifeDrain), nameof(LifeDrain.GetLevelText))]
	public static class LifeDrain_GetLevelText_Translate
	{
		static void Postfix(LifeDrain __instance, int __0, ref string __result)
		{
			/*
			string text = "Mental attack versus an organic creature\n";
						text = text + "Drains {{rules|" + Level + "}} hit " + ((Level == 1) ? "point" : "points") + " per round\n";
						text += "Target gets a mental save to resist damage each round\n";
						text += "Duration: 20 rounds\n";
						return text + "Cooldown: 200 rounds\n";
			*/
			string text = "유기체 대상 정신 공격\n";
			text += $"매 라운드 {{{{rules|{__0}}}}} 체력을 흡수합니다.\n";
			text += "대상은 매 라운드 피해를 저항하기 위한 정신 내성 굴림을 합니다.\n";
			text += "지속 시간: 20 라운드\n";
			__result = text + "쿨다운: 200 라운드\n";
		}
	}

	[HarmonyPatch(typeof(LightManipulation), nameof(LightManipulation.GetDescription))]
	public static class LightManipulation_GetDescription_Translate
	{
		static void Postfix(LightManipulation __instance, ref string __result)
		{
			/*
			return "You manipulate light to your advantage.";
			*/
			__result = "당신은 빛을 당신에게 유리하게 조작합니다.";
		}
	}

	[HarmonyPatch(typeof(LightManipulation), nameof(LightManipulation.GetLevelText))]
	public static class LightManipulation_GetLevelText_Translate
	{
		static void Postfix(LightManipulation __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("" + "You produce ambient light within a radius of {{rules|" + GetMaxLightRadius(Level) + "}}.\n", "You may focus the light into a laser beam, temporarily reducing the radius of your ambient light by 1.\n"), "Laser damage increment: {{rules|", GetDamage(Level), "}}\n"), "Laser penetration: {{rules|", (GetLasePenetrationBonus(Level) + RuleSettings.VISUAL_PENETRATION_BONUS).ToString(), "}}\n"), "Ambient light recharges at a rate of 1 unit every ", GetRadiusRegrowthTurns().ToString(), " rounds until it reaches its maximum value.\n"), "{{rules|", GetReflectChance(Level).ToString(), "%}} chance to reflect light-based damage");
			*/
			var value0 = __instance.GetMaxLightRadius(__0);
			var value1 = __instance.GetDamage(__0);
			var value2 = (__instance.GetLasePenetrationBonus(__0) + RuleSettings.VISUAL_PENETRATION_BONUS).ToString();
			var value3 = __instance.GetRadiusRegrowthTurns().ToString();
			var value4 = __instance.GetReflectChance(__0).ToString();
			__result = $"{{{{rules|{value0}}}}} 반경 내에서 주변광을 생성합니다.\n빛을 레이저 빔에 집중시켜 일시적으로 주변광의 반경을 1만큼 줄일 수 있습니다.\n레이저 손상 증가: {{{{rules|{value1}}}}}\n레이저 침투: {{{{rules|{value2}}}}}\n주변 조명은 최대값에 도달할 때까지 {value3} 라운드마다 1단위의 비율로 재충전됩니다.\n{{{{rules|{value4}%}}}} 빛 기반 손상을 반사할 확률";
		}
	}

	[HarmonyPatch(typeof(LiquidFont), nameof(LiquidFont.GetLevelText))]
	public static class LiquidFont_GetLevelText_Translate
	{
		static void Postfix(LiquidFont __instance, int __0, ref string __result)
		{
			/*
			return "You ooze fluids with the best of them.\n";
			*/
			__result = "당신은 그들 중 최고로 체액을 흘립니다.";
		}
	}

	[HarmonyPatch(typeof(LiquidSpitter), nameof(LiquidSpitter.GetDescription))]
	public static class LiquidSpitter_GetDescription_Translate
	{
		static void Postfix(LiquidSpitter __instance, ref string __result)
		{
			/*
			return "You spit a puddle of " + ColorUtility.StripBackgroundFormatting(LiquidName) + ".";
			*/
			var value0 = ColorUtility.StripBackgroundFormatting(__instance.LiquidName);
			__result = $"{value0} 웅덩이를 뱉었습니다.";
		}
	}

	[HarmonyPatch(typeof(LiquidSpitter), nameof(LiquidSpitter.GetLevelText))]
	public static class LiquidSpitter_GetLevelText_Translate
	{
		static void Postfix(LiquidSpitter __instance, int __0, ref string __result)
		{
			/*
			return "Range: 8\nArea: 3x3\nCooldown: 10 rounds";
			*/
			__result = "범위: 8\n면적: 3x3\n쿨타임: 10라운드";
		}
	}

	[HarmonyPatch(typeof(MagneticPulse), nameof(MagneticPulse.GetDescription))]
	public static class MagneticPulse_GetDescription_Translate
	{
		static void Postfix(MagneticPulse __instance, ref string __result)
		{
			/*
			return "You emit powerful magnetic pulses.";
			*/
			__result = "당신은 강력한 자기 펄스를 방출합니다.";
		}
	}

	[HarmonyPatch(typeof(MagneticPulse), nameof(MagneticPulse.GetLevelText))]
	public static class MagneticPulse_GetLevelText_Translate
	{
		static void Postfix(MagneticPulse __instance, int __0, ref string __result)
		{
			/*
			return string.Concat("After a one-round warmup, you emit a pulse with radius " + Level + " that attempts to pull metal objects toward you, including metal gear equipped on creatures.\n", "Cooldown: ", Cooldown.Things("round"), "\n");
			*/
			var value0 = __0;
			var value1 = __instance.Cooldown.Things("round");
			__result = $"1라운드 워밍업 후, 생명체에 장착된 금속 장비를 포함하여 금속 물체를 사용자 쪽으로 끌어당기려는 반경 {value0}의 펄스를 방출합니다.\n쿨다운: {value1}";
		}
	}

	[HarmonyPatch(typeof(MassMind), nameof(MassMind.GetDescription))]
	public static class MassMind_GetDescription_Translate
	{
		static void Postfix(MassMind __instance, ref string __result)
		{
			/*
			return "You tap into the aggregate mind and steal power from other espers.";
			*/
			__result = "당신은 집단 정신을 활용하고 다른 에스퍼로부터 힘을 훔칩니다.";
		}
	}

	[HarmonyPatch(typeof(MassMind), nameof(MassMind.GetLevelText))]
	public static class MassMind_GetLevelText_Translate
	{
		static void Postfix(MassMind __instance, int __0, ref string __result)
		{
			/*
			string text = "";
						text += "Refreshes all mental mutations\n";
						text = text + "Cooldown: {{rules|" + GetCooldown(Level) + "}} rounds\n";
						text += "Cooldown is not affected by Willpower.\n";
						text += "Each use attracts slightly more attention from psychic interlopers.\n";
						text = ((Level != base.Level) ? (text + "{{rules|Decreased chance for another esper to steal your powers}}\n") : (text + "{{rules|Small chance each round for another esper to steal your powers}}\n"));
						return text + "-200 reputation with {{w|the Seekers of the Sightless Way}}";
			*/
			string text = "";
			text += "모든 정신 변이를 재충전합니다.\n";
			int cooldown = __instance.GetCooldown(__0);
			text += $"쿨다운: {{{{rules|{cooldown}}}}} 라운드\n";
			text += "쿨다운은 의지력의 영향을 받지 않습니다.\n";
			text += "사용할 때마다 심령 침입자의 관심을 조금 더 끕니다.\n";
			if (__0 != __instance.Level)
			{
			    text += "{{rules|다른 에스퍼가 능력을 훔칠 확률 감소}}\n";
			}
			else
			{
			    text += "{{rules|매 라운드 다른 에스퍼가 능력을 훔칠 작은 확률}}\n";
			}
			__result = text + "-200 {{w|보이지 않는 길을 찾는 자들}} 평판";
		}
	}

	[HarmonyPatch(typeof(MentalMirror), nameof(MentalMirror.GetDescription))]
	public static class MentalMirror_GetDescription_Translate
	{
		static void Postfix(MentalMirror __instance, ref string __result)
		{
			/*
			return "You reflect mental attacks back at your attackers.";
			*/
			__result = "공격자에게 정신적 공격을 반사합니다.";
		}
	}

	[HarmonyPatch(typeof(MentalMirror), nameof(MentalMirror.GetLevelText))]
	public static class MentalMirror_GetLevelText_Translate
	{
		static void Postfix(MentalMirror __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat("When you suffer a mental attack while Mental Mirror is off cooldown, you gain +{{rules|" + GetMABonus(Level) + "}} mental armor (MA).\n", "If the attack then fails to penetrate your MA, it's reflected back at your attacker.\n"), "Cooldown: ", GetCooldown(Level).ToString());
			*/
			var value0 = __instance.GetMABonus(__0);
			var value1 = Teleportation.GetCooldown(__0).ToString();
			__result = $"정신 거울이 재사용 대기시간이 아닌 동안 정신 공격을 받으면 +{{{{rules|{value0}}}}} 정신 갑옷(MA)을 얻습니다.\n공격이 MA를 관통하지 못하면 공격자에게 반사됩니다.\n쿨다운: {value1}";
		}
	}

	[HarmonyPatch(typeof(Metamorphosis), nameof(Metamorphosis.GetDescription))]
	public static class Metamorphosis_GetDescription_Translate
	{
		static void Postfix(Metamorphosis __instance, ref string __result)
		{
			/*
			return "You assume the form of any creature you touch.";
			*/
			__result = "당신은 당신이 만지는 모든 생물의 형태를 취합니다.";
		}
	}

	[HarmonyPatch(typeof(Metamorphosis), nameof(Metamorphosis.GetLevelText))]
	public static class Metamorphosis_GetLevelText_Translate
	{
		static void Postfix(Metamorphosis __instance, int __0, ref string __result)
		{
			/*
			return string.Concat("May only assume the form of creatures level " + GetMaxLevel(Level) + " or lower.\n", "Cooldown: ", GetCooldown(Level).ToString(), " rounds");
			*/
			var value0 = __instance.GetMaxLevel(__0);
			var value1 = __instance.GetCooldown(__0).ToString();
			__result = $"{value0} 레벨 이하의 생물로만 변신할 수 있습니다.\n쿨다운: {value1} 라운드";
		}
	}

	[HarmonyPatch(typeof(MultiHorns), nameof(MultiHorns.GetDescription))]
	public static class MultiHorns_GetDescription_Translate
	{
		static void Postfix(MultiHorns __instance, ref string __result)
		{
			/*
			return "Several horns jut out of your head.";
			*/
			__result = "머리에는 여러 개의 뿔이 튀어나와 있습니다.";
		}
	}

	[HarmonyPatch(typeof(MultipleArms), nameof(MultipleArms.GetDescription))]
	public static class MultipleArms_GetDescription_Translate
	{
		static void Postfix(MultipleArms __instance, ref string __result)
		{
			/*
			return "You have an extra set of arms.";
			*/
			__result = "여분의 무기 세트가 있습니다.";
		}
	}

	[HarmonyPatch(typeof(MultipleArms), nameof(MultipleArms.GetLevelText))]
	public static class MultipleArms_GetLevelText_Translate
	{
		static void Postfix(MultipleArms __instance, int __0, ref string __result)
		{
			/*
			return "{{rules|" + GetAttackChance(Level) + "%}} chance for each extra arm to deliver an additional melee attack whenever you make a melee attack";
			*/
			var value0 = __instance.GetAttackChance(__0);
			__result = $"{{{{rules|{value0}%}}}} 근접 공격을 할 때마다 각 추가 팔이 추가 근접 공격을 전달할 확률";
		}
	}

	[HarmonyPatch(typeof(MultipleLegs), nameof(MultipleLegs.GetDescription))]
	public static class MultipleLegs_GetDescription_Translate
	{
		static void Postfix(MultipleLegs __instance, ref string __result)
		{
			/*
			return "You have an extra set of legs.";
			*/
			__result = "여분의 다리 세트가 있습니다.";
		}
	}

	[HarmonyPatch(typeof(MultipleLegs), nameof(MultipleLegs.GetLevelText))]
	public static class MultipleLegs_GetLevelText_Translate
	{
		static void Postfix(MultipleLegs __instance, int __0, ref string __result)
		{
			/*
			return string.Concat("+{{rules|" + GetMoveSpeedBonus(Level) + "}} move speed\n", "+{{rules|", GetCarryCapacityBonus(Level).ToString(), "%}} carry capacity");
			*/
			var value0 = __instance.GetMoveSpeedBonus(__0);
			var value1 = __instance.GetCarryCapacityBonus(__0).ToString();
			__result = $"+{{{{rules|{value0}}}}} 이동 속도\n+{{{{rules|{value1}%}}}} 운반 능력";
		}
	}

	[HarmonyPatch(typeof(Myopia), nameof(Myopia.GetDescription))]
	public static class Myopia_GetDescription_Translate
	{
		static void Postfix(Myopia __instance, ref string __result)
		{
			/*
			return "You are nearsighted.\n\nYou can only see up to a radius of 10.";
			*/
			__result = "당신은 근시입니다.\n\n반경 10까지만 볼 수 있습니다.";
		}
	}

	[HarmonyPatch(typeof(Myopia), nameof(Myopia.GetLevelText))]
	public static class Myopia_GetLevelText_Translate
	{
		static void Postfix(Myopia __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Narcolepsy), nameof(Narcolepsy.GetDescription))]
	public static class Narcolepsy_GetDescription_Translate
	{
		static void Postfix(Narcolepsy __instance, ref string __result)
		{
			/*
			return "You fall asleep involuntarily from time to time.\n\nSmall chance each round you're in combat that you fall asleep for 20-29 rounds.";
			*/
			__result = "당신은 때때로 무의식적으로 잠이 듭니다.\n\n전투 중 매 라운드마다 20~29라운드 동안 잠들 가능성은 적습니다.";
		}
	}

	[HarmonyPatch(typeof(Narcolepsy), nameof(Narcolepsy.GetLevelText))]
	public static class Narcolepsy_GetLevelText_Translate
	{
		static void Postfix(Narcolepsy __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

[HarmonyPatch(typeof(MutationNightVision), nameof(MutationNightVision.GetDescription))]
public static class NightVision_GetDescription_Translate
{
	static void Postfix(MutationNightVision __instance, ref string __result)
	{
			/*
			return "";
			*/
			__result = "";
		}
	}

[HarmonyPatch(typeof(MutationNightVision), nameof(MutationNightVision.GetLevelText))]
public static class NightVision_GetLevelText_Translate
{
	static void Postfix(MutationNightVision __instance, int __0, ref string __result)
	{
			/*
			return "You see in the dark.\n";
			*/
			__result = "어둠 속에서도 보입니다.";
		}
	}

	[HarmonyPatch(typeof(NormalityBreather), nameof(NormalityBreather.GetDescription))]
	public static class NormalityBreather_GetDescription_Translate
	{
		static void Postfix(NormalityBreather __instance, ref string __result)
		{
			/*
			return "You breathe normality gas.";
			*/
			__result = "당신은 정상 가스를 흡입합니다.";
		}
	}

	[HarmonyPatch(typeof(NormalityBreather), nameof(NormalityBreather.GetLevelText))]
	public static class NormalityBreather_GetLevelText_Translate
	{
		static void Postfix(NormalityBreather __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat("Breathes normality gas in a cone.\n" + "Cone length: " + GetConeLength() + " tiles\n", "Cone angle: ", GetConeAngle().ToString(), " degrees\n"), "Cooldown: 15 rounds\n");
			*/
			var value0 = __instance.GetConeLength();
			var value1 = __instance.GetConeAngle().ToString();
			__result = $"원뿔 모양의 정상 가스를 호흡합니다.\n원뿔 길이: {value0} 타일\n원뿔 각도: {value1}도\n쿨타임: 15라운드";
		}
	}

	[HarmonyPatch(typeof(PackRat), nameof(PackRat.GetDescription))]
	public static class PackRat_GetDescription_Translate
	{
		static void Postfix(PackRat __instance, ref string __result)
		{
			/*
			return "You compulsively lug around everything you can.\n\nYou must maintain at least 90% of your carry capacity.\n\nYou cannot drop items if dropping them would reduce your weight beneath this requirement.\n\nYou suffer one point of damage each round you do not maintain this requirement.\n\nYou can only drop one set of items every 10 rounds.";
			*/
			__result = "당신은 당신이 할 수 있는 모든 것을 강박적으로 끌고 다닙니다.\n\n운반 능력의 최소 90%를 유지해야 합니다.\n\n아이템을 떨어뜨리면 무게가 이 요구 사항 이하로 줄어들 경우 아이템을 떨어뜨릴 수 없습니다.\n\n이 요구 사항을 유지하지 않으면 매 라운드마다 1점의 피해를 입습니다.\n\n10라운드마다 한 세트의 아이템만 떨어뜨릴 수 있습니다.";
		}
	}

	[HarmonyPatch(typeof(PackRat), nameof(PackRat.GetLevelText))]
	public static class PackRat_GetLevelText_Translate
	{
		static void Postfix(PackRat __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Phasing), nameof(Phasing.GetDescription))]
	public static class Phasing_GetDescription_Translate
	{
		static void Postfix(Phasing __instance, ref string __result)
		{
			/*
			return "You may phase through solid objects for brief periods of time.";
			*/
			__result = "짧은 시간 동안 고체 물체를 단계별로 통과할 수 있습니다.";
		}
	}

	[HarmonyPatch(typeof(Phasing), nameof(Phasing.GetLevelText))]
	public static class Phasing_GetLevelText_Translate
	{
		static void Postfix(Phasing __instance, int __0, ref string __result)
		{
			/*
			return string.Concat("Duration: {{rules|" + GetDuration(Level) + "}} rounds\n", "Cooldown: {{rules|", GetBaseCooldown(Level).ToString(), "}} rounds");
			*/
			var value0 = __instance.GetDuration(__0);
			var value1 = __instance.GetBaseCooldown(__0).ToString();
			__result = $"지속 시간: {{{{rules|{value0}}}}} 라운드\n쿨다운: {{{{rules|{value1}}}}} 라운드";
		}
	}

	[HarmonyPatch(typeof(PhotosyntheticSkin), nameof(PhotosyntheticSkin.GetDescription))]
	public static class PhotosyntheticSkin_GetDescription_Translate
	{
		static void Postfix(PhotosyntheticSkin __instance, ref string __result)
		{
			/*
			return "You replenish yourself by absorbing sunlight through your hearty green skin.";
			*/
			__result = "푸짐한 녹색 피부를 통해 햇빛을 흡수하여 에너지를 보충하세요.";
		}
	}

	[HarmonyPatch(typeof(PhotosyntheticSkin), nameof(PhotosyntheticSkin.GetLevelText))]
	public static class PhotosyntheticSkin_GetLevelText_Translate
	{
		static void Postfix(PhotosyntheticSkin __instance, int __0, ref string __result)
		{
			/*
			string text = "";
						text = text + "You can bask in the sunlight instead of eating a meal to gain a special metabolizing effect for {{rules|" + GetBonusDurationString(Level) + "}}: +{{rules|" + GetBonusRegeneration(Level) + "%}} to natural healing rate and +{{rules|" + GetBonusQuickness(Level) + "}} Quickness\n";
						text = text + "While in the sunlight, you accrue starch and lignin that you can use as ingredients in meals you cook (max {{rules|" + GetStarchServings(Level) + "}} of each).\n";
						text = text + "+{{rules|" + GetBonusCamouflage(Level) + "}} DV while occupying the same space as foliage\n";
						return text + "+200 reputation with {{w|roots}}, {{w|trees}}, {{w|vines}}, and {{w|the Consortium of Phyta}}";
			*/
			string text = "";
			string bonusDuration = PhotosyntheticSkin.GetBonusDurationString(__0);
			int bonusRegen = PhotosyntheticSkin.GetBonusRegeneration(__0);
			int bonusQuickness = PhotosyntheticSkin.GetBonusQuickness(__0);
			string starchServings = PhotosyntheticSkin.GetStarchServings(__0);
			int bonusCamouflage = PhotosyntheticSkin.GetBonusCamouflage(__0);
			text += $"햇빛을 쬐어 식사를 대신하고 {{{{rules|{bonusDuration}}}}} 동안 특별한 대사 효과를 얻을 수 있습니다: 자연 치유 속도 +{{{{rules|{bonusRegen}%}}}} 및 신속성 +{{{{rules|{bonusQuickness}}}}}\n";
			text += $"햇빛 아래에서는 전분과 리그닌을 축적하여 요리에 재료로 사용할 수 있습니다 (각각 최대 {{{{rules|{starchServings}}}}}).\n";
			text += $"+{{{{rules|{bonusCamouflage}}}}} DV (초목과 같은 칸에 있을 때)\n";
			__result = text + "{{w|뿌리}}, {{w|나무}}, {{w|덩굴}}, {{w|피타 컨소시엄}} 평판 +200";
		}
	}

	[HarmonyPatch(typeof(PoisonBreather), nameof(PoisonBreather.GetDescription))]
	public static class PoisonBreather_GetDescription_Translate
	{
		static void Postfix(PoisonBreather __instance, ref string __result)
		{
			/*
			return "You breathe poison gas.";
			*/
			__result = "당신은 독가스를 흡입합니다.";
		}
	}

	[HarmonyPatch(typeof(PoisonBreather), nameof(PoisonBreather.GetLevelText))]
	public static class PoisonBreather_GetLevelText_Translate
	{
		static void Postfix(PoisonBreather __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat("Breathes poison gas in a cone.\n" + "Cone length: " + GetConeLength() + " tiles\n", "Cone angle: ", GetConeAngle().ToString(), " degrees\n"), "Cooldown: 15 rounds\n");
			*/
			var value0 = __instance.GetConeLength();
			var value1 = __instance.GetConeAngle().ToString();
			__result = $"원뿔 모양의 독가스를 흡입합니다.\n원뿔 길이: {value0} 타일\n원뿔 각도: {value1}도\n쿨타임: 15라운드";
		}
	}

	[HarmonyPatch(typeof(Precognition), nameof(Precognition.GetDescription))]
	public static class Precognition_GetDescription_Translate
	{
		static void Postfix(Precognition __instance, ref string __result)
		{
			/*
			return "You peer into your near future.";
			*/
			__result = "당신은 가까운 미래를 들여다 봅니다.";
		}
	}

	[HarmonyPatch(typeof(Precognition), nameof(Precognition.GetLevelText))]
	public static class Precognition_GetLevelText_Translate
	{
		static void Postfix(Precognition __instance, int __0, ref string __result)
		{
			/*
			return string.Concat("You may activate this power and then later revert to the point in time when you activated it.\n" + "Duration between use and reversion: {{rules|" + GetDuration(Level) + "}} rounds\n", "Cooldown: ", GetCooldown(Level).ToString(), " rounds");
			*/
			var value0 = __instance.GetDuration(__0);
			var value1 = __instance.GetCooldown(__0).ToString();
			__result = $"당신은 이 힘을 활성화한 다음 나중에 활성화했던 시점으로 되돌릴 수 있습니다.\n사용과 복귀 사이의 기간: {{{{rules|{value0}}}}} 라운드\n쿨다운: {value1} 라운드";
		}
	}

	[HarmonyPatch(typeof(PsionicMigraines), nameof(PsionicMigraines.GetDescription))]
	public static class PsionicMigraines_GetDescription_Translate
	{
		static void Postfix(PsionicMigraines __instance, ref string __result)
		{
			/*
			return "You suffer from powerful psionic migraines that render your head extremely sensitive.\n\nYou can't wear hats or helmets.";
			*/
			__result = "당신은 머리를 극도로 민감하게 만드는 강력한 사이오닉 편두통으로 고통받고 있습니다.\n\n모자나 헬멧을 착용할 수 없습니다.";
		}
	}

	[HarmonyPatch(typeof(PsionicMigraines), nameof(PsionicMigraines.GetLevelText))]
	public static class PsionicMigraines_GetLevelText_Translate
	{
		static void Postfix(PsionicMigraines __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Psychometry), nameof(Psychometry.GetDescription))]
	public static class Psychometry_GetDescription_Translate
	{
		static void Postfix(Psychometry __instance, ref string __result)
		{
			/*
			return "You read the history of artifacts by touching them, learning what they do and how they were made.";
			*/
			__result = "유물을 만져보고, 그것이 무엇인지, 어떻게 만들어졌는지 배우면서 유물의 역사를 읽어보세요.";
		}
	}

	[HarmonyPatch(typeof(Pyrokinesis), nameof(Pyrokinesis.GetDescription))]
	public static class Pyrokinesis_GetDescription_Translate
	{
		static void Postfix(Pyrokinesis __instance, ref string __result)
		{
			/*
			return "You heat a nearby area with your mind.";
			*/
			__result = "당신은 마음으로 가까운 지역을 가열합니다.";
		}
	}

	[HarmonyPatch(typeof(QuantumJitters), nameof(QuantumJitters.GetDescription))]
	public static class QuantumJitters_GetDescription_Translate
	{
		static void Postfix(QuantumJitters __instance, ref string __result)
		{
			/*
			return "Your willful acts sometimes dent spacetime.\n\nWhenever you use an activated ability, there's a small chance your focus slips and you dent spacetime in the local region, causing 1-2 spacetime vortices to appear. This chance increases the longer you go without using an activated ability.";
			*/
			__result = "당신의 고의적인 행동이 때때로 시공간을 손상시킵니다.\n\n활성화 능력을 사용할 때마다 집중력이 흐트러지고 해당 지역의 시공간이 찌그러져 1~2개의 시공간 소용돌이가 나타날 가능성이 적습니다. 이 확률은 활성화 능력을 사용하지 않고 더 오래 갈수록 증가합니다.";
		}
	}

	[HarmonyPatch(typeof(QuantumJitters), nameof(QuantumJitters.GetLevelText))]
	public static class QuantumJitters_GetLevelText_Translate
	{
		static void Postfix(QuantumJitters __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Quills), nameof(Quills.GetDescription))]
	public static class Quills_GetDescription_Translate
	{
		static void Postfix(Quills __instance, ref string __result)
		{
			/*
			return Blueprint.GetTag("VariantDescription").Coalesce("Hundreds of needle-pointed quills cover your body.");
			*/
			var value0 = __instance.Blueprint.GetTag("VariantDescription").Coalesce("Hundreds of needle-pointed quills cover your body.");
			__result = $"{value0}";
		}
	}

	[HarmonyPatch(typeof(ReflectShame), nameof(ReflectShame.GetDescription))]
	public static class ReflectShame_GetDescription_Translate
	{
		static void Postfix(ReflectShame __instance, ref string __result)
		{
			/*
			return "You reflect the shameful countenance of nearby creatures.";
			*/
			__result = "당신은 근처 생물들의 부끄러운 표정을 반사합니다.";
		}
	}

	[HarmonyPatch(typeof(ReflectShame), nameof(ReflectShame.GetLevelText))]
	public static class ReflectShame_GetLevelText_Translate
	{
		static void Postfix(ReflectShame __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Regeneration), nameof(Regeneration.GetDescription))]
	public static class Regeneration_GetDescription_Translate
	{
		static void Postfix(Regeneration __instance, ref string __result)
		{
			/*
			return "Your wounds heal very quickly.";
			*/
			__result = "상처는 매우 빨리 치유됩니다.";
		}
	}

	[HarmonyPatch(typeof(RepellingForce), nameof(RepellingForce.GetDescription))]
	public static class RepellingForce_GetDescription_Translate
	{
		static void Postfix(RepellingForce __instance, ref string __result)
		{
			/*
			return "You invoke a repelling force in the surrounding area, throwing enemies back.";
			*/
			__result = "주변 지역에 반발력을 불러일으켜 적을 뒤로 밀어냅니다.";
		}
	}

	[HarmonyPatch(typeof(SensePsychic), nameof(SensePsychic.GetDescription))]
	public static class SensePsychic_GetDescription_Translate
	{
		static void Postfix(SensePsychic __instance, ref string __result)
		{
			/*
			return string.Concat("" + "You can sense other mental mutants through the psychic aether.\n\n", "You detect the presence of psychic enemies within a radius of ", Radius.ToString(), ".\nThere's a chance you identify detected enemies.");
			*/
			var value0 = __instance.Radius.ToString();
			__result = $"심령 에테르를 통해 다른 정신적 돌연변이를 감지할 수 있습니다.\n\n{value0} 반경 내에 정신적 적의 존재를 감지했습니다.\n감지된 적을 식별할 가능성이 있습니다.";
		}
	}

	[HarmonyPatch(typeof(SensePsychic), nameof(SensePsychic.GetLevelText))]
	public static class SensePsychic_GetLevelText_Translate
	{
		static void Postfix(SensePsychic __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(ShameBreather), nameof(ShameBreather.GetDescription))]
	public static class ShameBreather_GetDescription_Translate
	{
		static void Postfix(ShameBreather __instance, ref string __result)
		{
			/*
			return "You breathe shame gas.";
			*/
			__result = "당신은 수치스러운 가스를 들이마십니다.";
		}
	}

	[HarmonyPatch(typeof(ShameBreather), nameof(ShameBreather.GetLevelText))]
	public static class ShameBreather_GetLevelText_Translate
	{
		static void Postfix(ShameBreather __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat("Breathes shame gas in a cone.\n" + "Cone length: " + GetConeLength() + " tiles\n", "Cone angle: ", GetConeAngle().ToString(), " degrees\n"), "Cooldown: 15 rounds\n");
			*/
			var value0 = __instance.GetConeLength();
			var value1 = __instance.GetConeAngle().ToString();
			__result = $"원뿔 모양으로 수치 가스를 들이마십니다.\n원뿔 길이: {value0} 타일\n원뿔 각도: {value1}도\n쿨타임: 15라운드";
		}
	}

	[HarmonyPatch(typeof(Skittish), nameof(Skittish.GetDescription))]
	public static class Skittish_GetDescription_Translate
	{
		static void Postfix(Skittish __instance, ref string __result)
		{
			/*
			return "You startle easily and engage your defense mechanisms.";
			*/
			__result = "당신은 쉽게 놀라고 방어 메커니즘을 사용합니다.";
		}
	}

	[HarmonyPatch(typeof(Skittish), nameof(Skittish.GetLevelText))]
	public static class Skittish_GetLevelText_Translate
	{
		static void Postfix(Skittish __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(SleepBreather), nameof(SleepBreather.GetDescription))]
	public static class SleepBreather_GetDescription_Translate
	{
		static void Postfix(SleepBreather __instance, ref string __result)
		{
			/*
			return "You breathe sleep gas.";
			*/
			__result = "당신은 수면 가스를 흡입합니다.";
		}
	}

	[HarmonyPatch(typeof(SleepBreather), nameof(SleepBreather.GetLevelText))]
	public static class SleepBreather_GetLevelText_Translate
	{
		static void Postfix(SleepBreather __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat("Breathes sleep gas in a cone.\n" + "Cone length: " + GetConeLength() + " tiles\n", "Cone angle: ", GetConeAngle().ToString(), " degrees\n"), "Cooldown: 15 rounds\n");
			*/
			var value0 = __instance.GetConeLength();
			var value1 = __instance.GetConeAngle().ToString();
			__result = $"원뿔 형태로 수면 가스를 흡입합니다.\n원뿔 길이: {value0} 타일\n원뿔 각도: {value1}도\n쿨타임: 15라운드";
		}
	}

	[HarmonyPatch(typeof(SlimeGlands), nameof(SlimeGlands.GetDescription))]
	public static class SlimeGlands_GetDescription_Translate
	{
		static void Postfix(SlimeGlands __instance, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("" + "You produce a viscous slime that you can spit at things.\n\n", "Covers an area with slime\n"), "Range: 8\n"), "Area: 3x3\n"), "Cooldown: 40 rounds\n"), "You can walk over slime without slipping.");
			*/
			__result = "당신은 물건에 침을 뱉을 수 있는 점성 점액을 생성합니다.\n\n점액으로 해당 지역을 덮습니다.\n범위: 8\n면적: 3x3\n쿨타임: 40라운드\n미끄러지지 않고 슬라임 위를 걸을 수 있습니다.";
		}
	}

	[HarmonyPatch(typeof(SlimeGlands), nameof(SlimeGlands.GetLevelText))]
	public static class SlimeGlands_GetLevelText_Translate
	{
		static void Postfix(SlimeGlands __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(SlogGlands), nameof(SlogGlands.GetDescription))]
	public static class SlogGlands_GetDescription_Translate
	{
		static void Postfix(SlogGlands __instance, ref string __result)
		{
			/*
			return "You bear a sphincter-choked bilge hose that you use to slurp up nearby liquids and spew them at enemies, occasionally knocking them down.";
			*/
			__result = "당신은 괄약근이 막힌 빌지 호스를 가지고 있는데, 이 호스는 근처의 액체를 후루룩 마시고 적에게 토해 내며 가끔 쓰러뜨리는 데 사용됩니다.";
		}
	}

	[HarmonyPatch(typeof(SlogGlands), nameof(SlogGlands.GetLevelText))]
	public static class SlogGlands_GetLevelText_Translate
	{
		static void Postfix(SlogGlands __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("" + "+6 Strength\n", "+1 AV\n"), "+100 Acid Resistance\n"), "+300 reputation with mollusks\n"), "Bilge sphincter acts as a melee weapon.\n"), "+50 move speed when moving through tiles with 200+ drams of liquid\n"), "You can spew liquid from your tile into a nearby area.\n"), "Spew range: 10\n"), "Spew area: 3x3\n"), "Spew chance to knock the targets down: Strength/Agility save vs. character level\n"), "Spew cooldown: 10 rounds\n");
			*/
			__result = "+6 힘\n+1 AV\n산성 저항 +100\n연체동물 평판 +300\n빌지 괄약근은 근접 무기 역할을 합니다.\n200드람 이상의 액체가 있는 타일을 통과할 때 이동 속도 +50\n타일에서 근처 지역으로 액체를 뿜어낼 수 있습니다.\n분출 범위: 10\n분출 면적: 3x3\n목표물을 넘어뜨릴 기회를 뿜어냅니다: 힘/민첩 저장 대 캐릭터 레벨\n분출 쿨다운: 10라운드";
		}
	}

	[HarmonyPatch(typeof(SociallyRepugnant), nameof(SociallyRepugnant.GetDescription))]
	public static class SociallyRepugnant_GetDescription_Translate
	{
		static void Postfix(SociallyRepugnant __instance, ref string __result)
		{
			/*
			return "Others find it difficult to tolerate you in social settings.\n\n-50 reputation with every faction";
			*/
			__result = "다른 사람들은 사회적 환경에서 당신을 참는 것을 어려워합니다.\n\n-모든 세력에 대한 평판 50";
		}
	}

	[HarmonyPatch(typeof(SociallyRepugnant), nameof(SociallyRepugnant.GetLevelText))]
	public static class SociallyRepugnant_GetLevelText_Translate
	{
		static void Postfix(SociallyRepugnant __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(SpacetimeVortex), nameof(SpacetimeVortex.GetDescription))]
	public static class SpacetimeVortex_GetDescription_Translate
	{
		static void Postfix(SpacetimeVortex __instance, ref string __result)
		{
			/*
			return "You sunder spacetime, sending things nearby careening through a tear in the cosmic fabric.";
			*/
			__result = "당신은 시공간을 분리하여 우주 조직의 찢어진 틈을 통해 근처의 사물을 돌보게 합니다.";
		}
	}

	[HarmonyPatch(typeof(SpiderWebs), nameof(SpiderWebs.GetLevelText))]
	public static class SpiderWebs_GetLevelText_Translate
	{
		static void Postfix(SpiderWebs __instance, int __0, ref string __result)
		{
			/*
			return "You bear two spinnerets with which you spin a sticky silk.\n";
			*/
			__result = "당신은 끈적끈적한 실크를 뽑는 데 사용되는 두 개의 방적 돌기를 가지고 있습니다.";
		}
	}

	[HarmonyPatch(typeof(Spinnerets), nameof(Spinnerets.GetDescription))]
	public static class Spinnerets_GetDescription_Translate
	{
		static void Postfix(Spinnerets __instance, ref string __result)
		{
			/*
			return "You can spin sticky silk webs.";
			*/
			__result = "끈적끈적한 실크 거미줄을 뽑을 수 있습니다.";
		}
	}

	[HarmonyPatch(typeof(SpontaneousCombustion), nameof(SpontaneousCombustion.GetDescription))]
	public static class SpontaneousCombustion_GetDescription_Translate
	{
		static void Postfix(SpontaneousCombustion __instance, ref string __result)
		{
			/*
			return "You spontaneously erupt into flames.\n\nSmall chance each round you're in combat that you spontaneously erupt into flames.";
			*/
			__result = "당신은 저절로 화염에 휩싸입니다.\n\n전투 중 매 라운드마다 자발적으로 화염에 휩싸일 가능성은 거의 없습니다.";
		}
	}

	[HarmonyPatch(typeof(SpontaneousCombustion), nameof(SpontaneousCombustion.GetLevelText))]
	public static class SpontaneousCombustion_GetLevelText_Translate
	{
		static void Postfix(SpontaneousCombustion __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(SporePuffer), nameof(SporePuffer.GetLevelText))]
	public static class SporePuffer_GetLevelText_Translate
	{
		static void Postfix(SporePuffer __instance, int __0, ref string __result)
		{
			/*
			return "You puff with the best of them.\n";
			*/
			__result = "당신은 그들 중 최고의 것을 퍼프합니다.";
		}
	}

	[HarmonyPatch(typeof(StickyTongue), nameof(StickyTongue.GetDescription))]
	public static class StickyTongue_GetDescription_Translate
	{
		static void Postfix(StickyTongue __instance, ref string __result)
		{
			/*
			return "You capture prey with your sticky tongue.";
			*/
			__result = "끈적한 혀로 먹이를 잡습니다.";
		}
	}

	[HarmonyPatch(typeof(StickyTongue), nameof(StickyTongue.GetLevelText))]
	public static class StickyTongue_GetLevelText_Translate
	{
		static void Postfix(StickyTongue __instance, int __0, ref string __result)
		{
			/*
			return string.Concat("You pull the nearest creature toward you.\n" + "Range: " + GetRange(Level) + "\n", "Cooldown: ", GetCooldown(Level).ToString(), " rounds");
			*/
			var value0 = StickyTongue.GetRange(__0);
			var value1 = StickyTongue.GetCooldown(__0).ToString();
			__result = $"당신은 가장 가까운 생물을 당신쪽으로 끌어 당깁니다.\n범위: {value0}\n쿨다운: {value1} 라운드";
		}
	}

	[HarmonyPatch(typeof(Stinger), nameof(Stinger.GetDescription))]
	public static class Stinger_GetDescription_Translate
	{
		static void Postfix(Stinger __instance, ref string __result)
		{
			/*
			return StingerProperties.GetDescription();
			*/
			var value0 = __instance.StingerProperties.GetDescription();
			__result = $"{value0}";
		}
	}

	[HarmonyPatch(typeof(Stinger), nameof(Stinger.GetLevelText))]
	public static class Stinger_GetLevelText_Translate
	{
		static void Postfix(Stinger __instance, int __0, ref string __result)
		{
			/*
			StringBuilder stringBuilder = Event.NewStringBuilder("20% chance on melee attack to sting your opponent ({{c|\u001a}}{{rules|").Append(GetPenetration(Level) + RuleSettings.VISUAL_PENETRATION_BONUS).Append("}} {{r|\u0003}}{{rules|")
							.Append(GetDamage(Level))
							.Append("}})\n")
							.Append("Stinger is a long blade and can only penetrate once.\nAlways sting on charge or lunge.\nStinger applies venom on damage (only 20% chance if Stinger is your primary weapon).\nMay use Sting activated ability to strike with your stinger and automatically hit and penetrate.\nSting cooldown: ")
							.Append(GetCooldown(Level))
							.Append('\n');
						StingerProperties.AppendLevelText(stringBuilder, Level);
						stringBuilder.Append("+200 reputation with {{w|arachnids}}");
						return Event.FinalizeString(stringBuilder);
			*/
			int penetration = __instance.GetPenetration(__0) + RuleSettings.VISUAL_PENETRATION_BONUS;
			string damage = __instance.GetDamage(__0);
			int cooldown = __instance.GetCooldown(__0);
			StringBuilder stringBuilder = Event.NewStringBuilder();
			stringBuilder.Append("근접 공격 시 20% 확률로 상대를 찌릅니다 ({{c|\u001a}}{{rules|")
			    .Append(penetration)
			    .Append("}} {{r|\u0003}}{{rules|")
			    .Append(damage)
			    .Append("}})\n");
			stringBuilder.Append("침침은 장도이며 한 번만 관통할 수 있습니다.\n");
			stringBuilder.Append("충전 또는 돌진 시 항상 찌릅니다.\n");
			stringBuilder.Append("침침은 피해 시 독을 적용합니다 (침침이 주 무기라면 20% 확률만 적용).\n");
			stringBuilder.Append("Sting 활성화 능력을 사용해 침침으로 자동 명중 및 관통 공격을 할 수 있습니다.\n");
			stringBuilder.Append("Sting 쿨다운: ").Append(cooldown).Append('\n');
			__instance.StingerProperties.AppendLevelText(stringBuilder, __0);
			stringBuilder.Append("{{w|거미류}} 평판 +200");
			__result = Event.FinalizeString(stringBuilder);
		}
	}

	[HarmonyPatch(typeof(StoneGaze), nameof(StoneGaze.GetDescription))]
	public static class StoneGaze_GetDescription_Translate
	{
		static void Postfix(StoneGaze __instance, ref string __result)
		{
			/*
			return "You turn things to stone with your gaze.";
			*/
			__result = "당신은 시선으로 사물을 돌로 만듭니다.";
		}
	}

	[HarmonyPatch(typeof(StoneGaze), nameof(StoneGaze.GetLevelText))]
	public static class StoneGaze_GetLevelText_Translate
	{
		static void Postfix(StoneGaze __instance, int __0, ref string __result)
		{
			/*
			return "You can gaze {{rules|" + GetRange(Level) + "}} squares after a " + Grammar.Cardinal(GetDelay(Level)) + "-turn warmup and turn targets to stone.\nCooldown: {{rules|" + GetCooldown(Level) + "}} rounds";
			*/
			var value0 = __instance.GetRange(__0);
			var value1 = Grammar.Cardinal(__instance.GetDelay(__0));
			var value2 = __instance.GetCooldown(__0);
			__result = $"{value1}턴 워밍업 후 {{{{rules|{value0}}}}} 사각형을 응시하고 목표물을 돌로 만들 수 있습니다.\n쿨다운: {{{{rules|{value2}}}}} 라운드";
		}
	}

	[HarmonyPatch(typeof(StunBreather), nameof(StunBreather.GetDescription))]
	public static class StunBreather_GetDescription_Translate
	{
		static void Postfix(StunBreather __instance, ref string __result)
		{
			/*
			return "You breathe stun gas.";
			*/
			__result = "당신은 기절 가스를 흡입합니다.";
		}
	}

	[HarmonyPatch(typeof(StunBreather), nameof(StunBreather.GetLevelText))]
	public static class StunBreather_GetLevelText_Translate
	{
		static void Postfix(StunBreather __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat("Breathes stun gas in a cone.\n" + "Cone length: " + GetConeLength() + " tiles\n", "Cone angle: ", GetConeAngle().ToString(), " degrees\n"), "Cooldown: 15 rounds\n");
			*/
			var value0 = __instance.GetConeLength();
			var value1 = __instance.GetConeAngle().ToString();
			__result = $"원뿔 형태로 기절 가스를 흡입합니다.\n원뿔 길이: {value0} 타일\n원뿔 각도: {value1}도\n쿨타임: 15라운드";
		}
	}

	[HarmonyPatch(typeof(StunningForce), nameof(StunningForce.GetDescription))]
	public static class StunningForce_GetDescription_Translate
	{
		static void Postfix(StunningForce __instance, ref string __result)
		{
			/*
			return "You invoke a concussive force in a nearby area, throwing enemies back and stunning them.";
			*/
			__result = "근처 지역에 충격적인 힘을 불러일으켜 적을 뒤로 밀쳐내고 기절시킵니다.";
		}
	}

	[HarmonyPatch(typeof(StunningForce), nameof(StunningForce.GetLevelText))]
	public static class StunningForce_GetLevelText_Translate
	{
		static void Postfix(StunningForce __instance, int __0, ref string __result)
		{
			/*
			string text = "";
						string text2 = text;
						int rANGE = RANGE;
						text = text2 + "Range: " + rANGE + "\n";
						text += "Area: 7x7\n";
						text = ((Level != base.Level) ? (text + "{{rules|Increased stun save difficulty}}\n") : (text + "Creatures are pushed away from center of blast, stunned, and dealt crushing damage in up to 3 increments.\n"));
						text = text + "Damage increment: {{rules|" + GetDamageIncrement(Level) + "}}\n";
						return text + "Cooldown: 50 rounds";
			*/
			string text = "";
			text += $"사정거리: {StunningForce.RANGE}\n";
			text += "영역: 7x7\n";
			if (__0 != __instance.Level)
			{
			    text += "{{rules|기절 내성 난이도 증가}}\n";
			}
			else
			{
			    text += "생물은 폭발 중심에서 밀려나고 기절하며 최대 3회에 걸쳐 분쇄 피해를 받습니다.\n";
			}
			string damageIncrement = StunningForce.GetDamageIncrement(__0);
			text += $"피해 증가량: {{{{rules|{damageIncrement}}}}}\n";
			__result = text + "쿨다운: 50 라운드";
		}
	}

	[HarmonyPatch(typeof(SunderMind), nameof(SunderMind.GetDescription))]
	public static class SunderMind_GetDescription_Translate
	{
		static void Postfix(SunderMind __instance, ref string __result)
		{
			/*
			return "You sunder the mind of an enemy, leaving them reeling in pain.";
			*/
			__result = "당신은 적의 정신을 분열시켜 그들이 고통에 빠지게 만듭니다.";
		}
	}

	[HarmonyPatch(typeof(SunderMind), nameof(SunderMind.GetLevelText))]
	public static class SunderMind_GetLevelText_Translate
	{
		static void Postfix(SunderMind __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("" + "For up to 10 rounds, you engage in psychic combat with an opponent, dealing damage each round.\n", "Taking any action other than passing the turn will break the connection.\n"), "Each round you make a mental attack vs mental armor (MA).\n"), "Damage increment: {{rules|", GetDamageDice(Level), "}}\n"), "After the tenth round, you deal bonus damage equal to the total amount of damage you've done so far.\n"), "Range: sight\n"), "Cooldown: ", GetCooldown(Level).ToString(), " rounds");
			*/
			var value0 = __instance.GetDamageDice(__0);
			var value1 = __instance.GetCooldown(__0).ToString();
			__result = $"최대 10라운드 동안 상대와 심령 전투를 벌여 라운드마다 피해를 입힙니다.\n턴을 넘기는 것 이외의 행동을 하면 연결이 끊어집니다.\n매 라운드마다 정신 공격 대 정신 방어구(MA)를 수행합니다.\n피해 증가량: {{{{rules|{value0}}}}}\n10라운드가 지나면 지금까지 입힌 총 피해량에 해당하는 추가 피해를 입힙니다.\n범위: 시력\n쿨다운: {value1} 라운드";
		}
	}

	[HarmonyPatch(typeof(Telekinesis), nameof(Telekinesis.GetLevelText))]
	public static class Telekinesis_GetLevelText_Translate
	{
		static void Postfix(Telekinesis __instance, int __0, ref string __result)
		{
			/*
			string text = "You can manipulate objects at a distance and perform some physical tasks with your mind while immobilized.\n";
						int telekineticRange = GetTelekineticRange(Level);
						text = text + "Range: " + telekineticRange + " " + ((telekineticRange == 1) ? "square" : "squares") + "\n";
						return text + "Telekinetic Strength: " + GetTelekineticStrength(Level) + "\n";
			*/
			string text = "당신은 움직일 수 없을 때도 정신으로 원거리 물체를 조작하고 일부 육체적 작업을 수행할 수 있습니다.\n";
			int telekineticRange = Telekinesis.GetTelekineticRange(__0);
			text += $"사정거리: {telekineticRange} 칸\n";
			int telekineticStrength = Telekinesis.GetTelekineticStrength(__0);
			__result = text + $"염력: {telekineticStrength}\n";
		}
	}

	[HarmonyPatch(typeof(Telepathy), nameof(Telepathy.GetLevelText))]
	public static class Telepathy_GetLevelText_Translate
	{
		static void Postfix(Telepathy __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(Teleportation), nameof(Teleportation.GetDescription))]
	public static class Teleportation_GetDescription_Translate
	{
		static void Postfix(Teleportation __instance, ref string __result)
		{
			/*
			return "You teleport to a nearby location.";
			*/
			__result = "가까운 곳으로 순간이동합니다.";
		}
	}

	[HarmonyPatch(typeof(Teleportation), nameof(Teleportation.GetLevelText))]
	public static class Teleportation_GetLevelText_Translate
	{
		static void Postfix(Teleportation __instance, int __0, ref string __result)
		{
			/*
			return string.Concat("Teleport to a random location within a designated area.\n" + "Uncertainty radius: {{rules|" + GetRadius(Level) + "}}\n", "Cooldown: {{rules|", GetCooldown(Level).ToString(), "}} rounds");
			*/
			var value0 = __instance.GetRadius(__0);
			var value1 = Teleportation.GetCooldown(__0).ToString();
			__result = $"지정된 지역 내 임의의 위치로 순간이동합니다.\n불확도 반경: {{{{rules|{value0}}}}}\n쿨다운: {{{{rules|{value1}}}}} 라운드";
		}
	}

	[HarmonyPatch(typeof(TeleportOther), nameof(TeleportOther.GetDescription))]
	public static class TeleportOther_GetDescription_Translate
	{
		static void Postfix(TeleportOther __instance, ref string __result)
		{
			/*
			return "You teleport an adjacent creature to a random nearby location.";
			*/
			__result = "인접한 생물을 무작위 근처 위치로 순간이동시킵니다.";
		}
	}

	[HarmonyPatch(typeof(TeleportOther), nameof(TeleportOther.GetLevelText))]
	public static class TeleportOther_GetLevelText_Translate
	{
		static void Postfix(TeleportOther __instance, int __0, ref string __result)
		{
			/*
			return "Cooldown: {{rules|" + GetCooldownTurns(Level) + "}} rounds";
			*/
			var value0 = __instance.GetCooldownTurns(__0);
			__result = $"쿨다운: {{{{rules|{value0}}}}} 라운드";
		}
	}

	[HarmonyPatch(typeof(TemporalFugue), nameof(TemporalFugue.GetDescription))]
	public static class TemporalFugue_GetDescription_Translate
	{
		static void Postfix(TemporalFugue __instance, ref string __result)
		{
			/*
			return "You quickly pass back and forth through time creating multiple copies of yourself.";
			*/
			__result = "당신은 자신의 여러 사본을 생성하면서 시간을 빠르게 앞뒤로 이동합니다.";
		}
	}

	[HarmonyPatch(typeof(TemporalFugue), nameof(TemporalFugue.GetLevelText))]
	public static class TemporalFugue_GetLevelText_Translate
	{
		static void Postfix(TemporalFugue __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat("" + "Duration: {{rules|" + GetTemporalFugueDuration(Level) + "}} rounds\n", "Copies: {{rules|", GetTemporalFugueCopies(Level).ToString(), "}}\n"), "Cooldown: {{rules|", GetCooldown(Level).ToString(), "}} rounds");
			*/
			var value0 = TemporalFugue.GetTemporalFugueDuration(__0);
			var value1 = TemporalFugue.GetTemporalFugueCopies(__0).ToString();
			var value2 = __instance.GetCooldown(__0).ToString();
			__result = $"지속 시간: {{{{rules|{value0}}}}} 라운드\n사본: {{{{rules|{value1}}}}}\n쿨다운: {{{{rules|{value2}}}}} 라운드";
		}
	}

	[HarmonyPatch(typeof(ThickFur), nameof(ThickFur.GetDescription))]
	public static class ThickFur_GetDescription_Translate
	{
		static void Postfix(ThickFur __instance, ref string __result)
		{
			/*
			return string.Concat("" + "You are covered in a thick coat of fur, which protects you from the elements.\n\n", "+5 Heat Resistance\n+5 Cold Resistance\n+100 reputation with {{w|apes}}, {{w|baboons}}, {{w|bears}}, and {{w|grazing hedonists}}");
			*/
			__result = "당신은 두꺼운 모피 코트로 덮여있어 요소로부터 당신을 보호합니다.\n\n+5 열저항\n+5 냉기 저항\n{{w|유인원}}, {{w|비비}}, {{w|곰}} 및 {{w|방목하는 쾌락주의자}}에 대한 평판 +100";
		}
	}

	[HarmonyPatch(typeof(ThickFur), nameof(ThickFur.GetLevelText))]
	public static class ThickFur_GetLevelText_Translate
	{
		static void Postfix(ThickFur __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(TimeDilation), nameof(TimeDilation.GetDescription))]
	public static class TimeDilation_GetDescription_Translate
	{
		static void Postfix(TimeDilation __instance, ref string __result)
		{
			/*
			return "You distort time around your person in order to slow down your enemies.";
			*/
			__result = "적의 속도를 늦추기 위해 주변의 시간을 왜곡합니다.";
		}
	}

	[HarmonyPatch(typeof(TimeDilation), nameof(TimeDilation.GetLevelText))]
	public static class TimeDilation_GetLevelText_Translate
	{
		static void Postfix(TimeDilation __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("Creatures within " + Range + " tiles are slowed according to how close they are to you.\n", "Distance 1: creatures receive a {{rules|", ((int)(CalculateQuicknessPenaltyMultiplier(1.0, Range, Level) * 100.0)).ToString(), "%}} Quickness penalty\n"), "Distance 4: creatures receive a {{rules|", ((int)(CalculateQuicknessPenaltyMultiplier(4.0, Range, Level) * 100.0)).ToString(), "%}} Quickness penalty\n"), "Distance 7: creatures receive a {{rules|", ((int)(CalculateQuicknessPenaltyMultiplier(7.0, Range, Level) * 100.0)).ToString(), "%}} Quickness penalty\n"), "Duration: 15 rounds\n"), "Cooldown: ", GetCooldown(Level).ToString(), " rounds");
			*/
			var value0 = __instance.Range;
			var value1 = ((int)(TimeDilation.CalculateQuicknessPenaltyMultiplier(1.0, __instance.Range, __0) * 100.0)).ToString();
			var value2 = ((int)(TimeDilation.CalculateQuicknessPenaltyMultiplier(4.0, __instance.Range, __0) * 100.0)).ToString();
			var value3 = ((int)(TimeDilation.CalculateQuicknessPenaltyMultiplier(7.0, __instance.Range, __0) * 100.0)).ToString();
			var value4 = TimeDilation.GetCooldown(__0).ToString();
			__result = $"{value0} 타일 내의 생물은 자신에게 얼마나 가까이 있는지에 따라 속도가 느려집니다.\n거리 1: 생물은 {{{{rules|{value1}%}}}} 신속성 페널티를 받습니다.\n거리 4: 생물은 {{{{rules|{value2}%}}}} 신속성 페널티를 받습니다.\n거리 7: 생물은 {{{{rules|{value3}%}}}} 신속성 페널티를 받습니다.\n지속시간: 15라운드\n쿨다운: {value4} 라운드";
		}
	}

	[HarmonyPatch(typeof(TonicAllergy), nameof(TonicAllergy.GetDescription))]
	public static class TonicAllergy_GetDescription_Translate
	{
		static void Postfix(TonicAllergy __instance, ref string __result)
		{
			/*
			return "You are allergic to tonics.\n\nThe chance your mutant physiology reacts adversely to a tonic is increased to 33%.\nIf you react adversely this way to a salve or ubernostrum tonic, the adverse reaction effect is chosen randomly from among other tonic effects. You will still heal.";
			*/
			__result = "당신은 강장제에 알레르기가 있습니다.\n\n돌연변이 생리가 강장제에 부정적으로 반응할 확률이 33%로 증가합니다.\n고약이나 우버노스트럼 강장제에 이런 식으로 부정적 반응을 보이는 경우, 역반응 효과는 다른 강장제 효과 중에서 무작위로 선택됩니다. 당신은 여전히 ​​​​나아질 것입니다.";
		}
	}

	[HarmonyPatch(typeof(TonicAllergy), nameof(TonicAllergy.GetLevelText))]
	public static class TonicAllergy_GetLevelText_Translate
	{
		static void Postfix(TonicAllergy __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(TwoHeaded), nameof(TwoHeaded.GetDescription))]
	public static class TwoHeaded_GetDescription_Translate
	{
		static void Postfix(TwoHeaded __instance, ref string __result)
		{
			/*
			return "You have two heads.";
			*/
			__result = "당신에게는 머리가 두 개 있습니다.";
		}
	}

	[HarmonyPatch(typeof(TwoHeaded), nameof(TwoHeaded.GetLevelText))]
	public static class TwoHeaded_GetLevelText_Translate
	{
		static void Postfix(TwoHeaded __instance, int __0, ref string __result)
		{
			/*
			return string.Concat("Mental actions have {{rules|" + GetReducedMentalActionCost(Level) + "%}} lower action costs\n", "{{rules|", GetShakeOff(Level).ToString(), "%}} chance initially and each round to shake off a negative mental status effect");
			*/
			var value0 = __instance.GetReducedMentalActionCost(__0);
			var value1 = __instance.GetShakeOff(__0).ToString();
			__result = $"정신적 행동은 행동 비용이 {{{{rules|{value0}%}}}} 낮습니다\n{{{{rules|{value1}%}}}} 처음과 매 라운드마다 부정적인 정신 상태 효과를 떨쳐버릴 수 있는 기회";
		}
	}

	[HarmonyPatch(typeof(TwoHearted), nameof(TwoHearted.GetDescription))]
	public static class TwoHearted_GetDescription_Translate
	{
		static void Postfix(TwoHearted __instance, ref string __result)
		{
			/*
			return "You have two hearts.";
			*/
			__result = "당신은 두 개의 마음을 가지고 있습니다.";
		}
	}

	[HarmonyPatch(typeof(TwoHearted), nameof(TwoHearted.GetLevelText))]
	public static class TwoHearted_GetLevelText_Translate
	{
		static void Postfix(TwoHearted __instance, int __0, ref string __result)
		{
			/*
			StringBuilder stringBuilder = Event.NewStringBuilder();
						stringBuilder.Append("+{{rules|").Append(2 + (Level - 1) / 2).Append("}} Toughness\n")
							.Append("You can sprint for {{rules|")
							.Append(GetSprintBonus(Level))
							.Append("%}} longer.");
						return stringBuilder.ToString();
			*/
			int toughnessBonus = 2 + (__0 - 1) / 2;
			int sprintBonus = TwoHearted.GetSprintBonus(__0);
			__result = $"+{{{{rules|{toughnessBonus}}}}} 강인함\n{{{{rules|{sprintBonus}%}}}} 더 오래 전력질주할 수 있습니다.";
		}
	}

	[HarmonyPatch(typeof(UnstableGenome), nameof(UnstableGenome.GetDescription))]
	public static class UnstableGenome_GetDescription_Translate
	{
		static void Postfix(UnstableGenome __instance, ref string __result)
		{
			/*
			return "You gain one extra mutation each time you buy this, but the mutations don't manifest right away.\nWhenever you gain a level, there's a 33% chance that your genome destabilizes and you get to choose from 3 random mutations.";
			*/
			__result = "이것을 구매할 때마다 돌연변이가 하나 더 추가되지만, 돌연변이가 즉시 나타나지는 않습니다.\n레벨이 올라갈 때마다 게놈이 불안정해질 확률은 33%이며 무작위 돌연변이 3개 중에서 선택할 수 있습니다.";
		}
	}

	[HarmonyPatch(typeof(UnstableGenome), nameof(UnstableGenome.GetLevelText))]
	public static class UnstableGenome_GetLevelText_Translate
	{
		static void Postfix(UnstableGenome __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(UnwelcomeGermination), nameof(UnwelcomeGermination.GetDescription))]
	public static class UnwelcomeGermination_GetDescription_Translate
	{
		static void Postfix(UnwelcomeGermination __instance, ref string __result)
		{
			/*
			return "You spasmodically engender wild plant growth around yourself for a short period.\n\nThere is a small chance each round that you enter into a compulsive state of mind for 30-39 rounds.\n\nDuring this time, there is a 25% chance each round that you summon several hostile plants nearby.";
			*/
			__result = "당신은 짧은 기간 동안 주변에 야생 식물이 갑자기 자라나게 합니다.\n\n매 라운드마다 30~39라운드 동안 강박적인 정신 상태에 빠질 가능성이 적습니다.\n\n이 시간 동안 매 라운드마다 근처에 여러 적대적인 식물을 소환할 확률이 25%입니다.";
		}
	}

	[HarmonyPatch(typeof(UnwelcomeGermination), nameof(UnwelcomeGermination.GetLevelText))]
	public static class UnwelcomeGermination_GetLevelText_Translate
	{
		static void Postfix(UnwelcomeGermination __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(UrchinBelcher), nameof(UrchinBelcher.GetLevelText))]
	public static class UrchinBelcher_GetLevelText_Translate
	{
		static void Postfix(UrchinBelcher __instance, int __0, ref string __result)
		{
			/*
			return string.Concat(string.Concat(string.Concat("You belch urchins in a nearby area.\n" + "Number of urchins: 1d2+" + Level / 4 + " \n", "Range: ", GetRange(Level).ToString(), "\n"), "Radius: ", GetRadius().ToString(), "\n"), "Cooldown: ", GetCooldown(Level).ToString(), " rounds\n");
			*/
			var value0 = __0 / 4;
			var value1 = __instance.GetRange(__0).ToString();
			var value2 = __instance.GetRadius().ToString();
			var value3 = __instance.GetCooldown(__0).ToString();
			__result = $"근처에서 성게를 트림합니다.\n성게 수: 1d2+{value0} \n범위: {value1}\n반경: {value2}\n쿨다운: {value3} 라운드";
		}
	}

	[HarmonyPatch(typeof(WallWalker), nameof(WallWalker.GetDescription))]
	public static class WallWalker_GetDescription_Translate
	{
		static void Postfix(WallWalker __instance, ref string __result)
		{
			/*
			return "You can move across walls, and only across walls.";
			*/
			__result = "벽을 가로질러 이동할 수 있으며 벽을 통해서만 이동할 수 있습니다.";
		}
	}

	[HarmonyPatch(typeof(WallWalker), nameof(WallWalker.GetLevelText))]
	public static class WallWalker_GetLevelText_Translate
	{
		static void Postfix(WallWalker __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(WaveformWorm), nameof(WaveformWorm.GetDescription))]
	public static class WaveformWorm_GetDescription_Translate
	{
		static void Postfix(WaveformWorm __instance, ref string __result)
		{
			/*
			return "You dash along a waveform.";
			*/
			__result = "파형을 따라 돌진합니다.";
		}
	}

	[HarmonyPatch(typeof(WaveformWorm), nameof(WaveformWorm.GetLevelText))]
	public static class WaveformWorm_GetLevelText_Translate
	{
		static void Postfix(WaveformWorm __instance, int __0, ref string __result)
		{
			/*
			string text = "You dash in a direction, dealing damage to creatures you pass through.\n";
						text = text + "Range: " + GetRange(Level) + "\n";
						text = text + "Damage: " + GetDamage(Level) + "\n";
						int cooldownTurns = GetCooldownTurns(Level);
						return text + "Cooldown: " + cooldownTurns + " " + ((cooldownTurns == 1) ? "round" : "rounds") + "\n";
			*/
			string text = "당신은 한 방향으로 질주하며 지나치는 생물에게 피해를 줍니다.\n";
			int range = __instance.GetRange(__0);
			string damage = __instance.GetDamage(__0);
			int cooldownTurns = __instance.GetCooldownTurns(__0);
			text += $"사정거리: {range}\n";
			text += $"피해: {damage}\n";
			__result = text + $"쿨다운: {cooldownTurns} 라운드\n";
		}
	}

	[HarmonyPatch(typeof(WeakHeart), nameof(WeakHeart.GetDescription))]
	public static class WeakHeart_GetDescription_Translate
	{
		static void Postfix(WeakHeart __instance, ref string __result)
		{
			/*
			return "Your heart is weak.\n\n-5 to save vs. poison, disease, and cardiac arrest.\nSmall chance per turn of entering cardiac arrest.";
			*/
			__result = "당신의 마음은 약합니다.\n\n-5 저장 대 독, 질병 및 심장 마비.\n턴당 심장마비가 발생할 가능성이 적습니다.";
		}
	}

	[HarmonyPatch(typeof(WeakHeart), nameof(WeakHeart.GetLevelText))]
	public static class WeakHeart_GetLevelText_Translate
	{
		static void Postfix(WeakHeart __instance, int __0, ref string __result)
		{
			/*
			return "";
			*/
			__result = "";
		}
	}

	[HarmonyPatch(typeof(WillForce), nameof(WillForce.GetDescription))]
	public static class WillForce_GetDescription_Translate
	{
		static void Postfix(WillForce __instance, ref string __result)
		{
			/*
			return "Through sheer force of will, you perform uncanny physical feats.";
			*/
			__result = "순수한 의지력을 통해 당신은 놀라운 신체적 위업을 수행합니다.";
		}
	}

	[HarmonyPatch(typeof(WillForce), nameof(WillForce.GetLevelText))]
	public static class WillForce_GetLevelText_Translate
	{
		static void Postfix(WillForce __instance, int __0, ref string __result)
		{
			/*
			string text = "Augments one physical attribute by an amount equal to twice your Ego bonus\n";
						text = text + "Duration: {{rules|" + GetLowDuration(Level) + "-" + GetHighDuration(Level) + "}} rounds\n";
						return text + "Cooldown: 200 rounds";
			*/
			string text = "자아 보너스의 두 배만큼 하나의 신체 능력치를 강화합니다\n";
			int lowDuration = __instance.GetLowDuration(__0);
			int highDuration = __instance.GetHighDuration(__0);
			text += $"지속 시간: {{{{rules|{lowDuration}-{highDuration}}}}} 라운드\n";
			__result = text + "쿨다운: 200 라운드";
		}
	}

	[HarmonyPatch(typeof(Wings), nameof(Wings.GetDescription))]
	public static class Wings_GetDescription_Translate
	{
		static void Postfix(Wings __instance, ref string __result)
		{
			/*
			return Blueprint.GetTag("VariantDescription").Coalesce("You fly.");
			*/
			var value0 = __instance.Blueprint.GetTag("VariantDescription").Coalesce("You fly.");
			__result = $"{value0}";
		}
	}

	[HarmonyPatch(typeof(Beak), nameof(Beak.GetDescription))]
	public static class Beak_GetDescription_Translate
	{
		static void Postfix(Beak __instance, ref string __result)
		{
			/*
			if (!Variant.IsNullOrEmpty())
						{
							return "Your face bears a sightly " + GetVariantName().ToLowerInvariant() + ".\n\n+1 Ego\nYou occasionally peck at your opponents.\n+200 reputation with {{w|birds}}";
						}
						return "Your face bears a sightly beak.\n\n+1 Ego\nYou occasionally peck at your opponents.\n+200 reputation with {{w|birds}}";
			*/
			if (!__instance.Variant.IsNullOrEmpty())
			{
			    string variantName = __instance.GetVariantName().ToLowerInvariant();
			    __result = $"당신의 얼굴에는 눈에 띄는 {variantName}이(가) 돋아 있습니다.\n\n자아 +1\n당신은 때때로 상대를 쪼아댑니다.\n{{{{w|새}}}} 평판 +200";
			    return;
			}
			__result = "당신의 얼굴에는 눈에 띄는 부리가 돋아 있습니다.\n\n자아 +1\n당신은 때때로 상대를 쪼아댑니다.\n{{w|새}} 평판 +200";
		}
	}

	[HarmonyPatch(typeof(Burgeoning), nameof(Burgeoning.GetLevelText))]
	public static class Burgeoning_GetLevelText_Translate
	{
		static void Postfix(Burgeoning __instance, int __0, ref string __result)
		{
			/*
			int num = 115 - 10 * Level;
						if (num < 5)
						{
							num = 5;
						}
						string text = "";
						text += "Range: 8\n";
						text += "Area: 3x3 + growth into adjacent tiles\n";
						text = text + "Cooldown: {{rules|" + num + "}} rounds\n";
						if (Level != base.Level)
						{
							text += "More powerful plants summoned\n";
						}
						return text + "+200 reputation with {{w|the Consortium of Phyta}}";
			*/
			int cooldown = 115 - 10 * __0;
			if (cooldown < 5)
			{
			    cooldown = 5;
			}
			string text = "";
			text += "범위: 8\n";
			text += "영역: 3x3 + 인접 타일로 성장\n";
			text = text + $"쿨다운: {{{{rules|{cooldown}}}}} 라운드\n";
			if (__0 != __instance.Level)
			{
			    text += "더 강력한 식물이 소환됩니다.\n";
			}
			__result = text + "{{w|피타 컨소시엄}} 평판 +200";
		}
	}

	[HarmonyPatch(typeof(BurrowingClaws), nameof(BurrowingClaws.GetLevelText))]
	public static class BurrowingClaws_GetLevelText_Translate
	{
		static void Postfix(BurrowingClaws __instance, int __0, ref string __result)
		{
			/*
			string cachedDisplayNameStrippedTitleCase = Blueprint.CachedDisplayNameStrippedTitleCase;
						string value = Grammar.Pluralize(cachedDisplayNameStrippedTitleCase);
						int wallBonusPenetration = GetWallBonusPenetration(Level);
						StringBuilder stringBuilder = Event.NewStringBuilder().Append(cachedDisplayNameStrippedTitleCase).Append(" penetration vs. walls: {{rules|")
							.Append(wallBonusPenetration.Signed())
							.Append("}}\n");
						int wallHitsRequired = GetWallHitsRequired(Level, ParentObject);
						if (wallHitsRequired > 0)
						{
							stringBuilder.Append(value).Append(" destroy walls after ").Append(wallHitsRequired)
								.Append(" penetrating hits.\n");
						}
						if (Options.EnablePrereleaseContent)
						{
							stringBuilder.Append("Can dig passages up or down when outside of combat\n");
						}
						stringBuilder.Append(value).Append(" are also a ").Append(GetWeaponClass())
							.Append(" class natural weapon that deal {{rules|")
							.Append(GetClawsDamage(Level))
							.Append("}} base damage to non-walls.");
						return Event.FinalizeString(stringBuilder);
			*/
			string displayName = __instance.Blueprint.CachedDisplayNameStrippedTitleCase;
			int wallBonusPenetration = BurrowingClaws.GetWallBonusPenetration(__0);
			StringBuilder stringBuilder = XRL.World.Event.NewStringBuilder()
			    .Append(displayName)
			    .Append(" 벽 관통: {{rules|")
			    .Append(wallBonusPenetration.Signed())
			    .Append("}}\n");
			int wallHitsRequired = BurrowingClaws.GetWallHitsRequired(__0, __instance.ParentObject);
			if (wallHitsRequired > 0)
			{
			    stringBuilder.Append($"{displayName}(은)는 관통 공격 {wallHitsRequired}회 후 벽을 파괴합니다.\n");
			}
			if (Options.EnablePrereleaseContent)
			{
			    stringBuilder.Append("전투 중이 아닐 때 위아래로 통로를 팔 수 있습니다.\n");
			}
			string weaponClass = __instance.GetWeaponClass();
			string clawsDamage = __instance.GetClawsDamage(__0);
			stringBuilder.Append($"{displayName}(은)는 또한 {weaponClass} 계열의 자연 무기이며, 벽이 아닌 대상에게 {{{{rules|{clawsDamage}}}}} 기본 피해를 줍니다.");
			__result = XRL.World.Event.FinalizeString(stringBuilder);
		}
	}

	[HarmonyPatch(typeof(Carapace), nameof(Carapace.GetLevelText))]
	public static class Carapace_GetLevelText_Translate
	{
		static void Postfix(Carapace __instance, int __0, ref string __result)
		{
			/*
			string cachedDisplayNameStripped = Blueprint.CachedDisplayNameStripped;
						StringBuilder stringBuilder = Event.NewStringBuilder();
						stringBuilder.AppendSigned(GetAVModifier(Level), "rules").Append(" AV\n").AppendSigned(GetDVModifier(Level), "rules")
							.Append(" DV\n")
							.AppendSigned(GetHeatResistance(Level), "rules")
							.Append(" Heat Resistance\n")
							.AppendSigned(GetColdResistance(Level), "rules")
							.Append(" Cold Resistance");
						if (Blueprint.TryGetPartParameter<string>("AddsRep", "Faction", out var Result) && Blueprint.TryGetPartParameter<int>("AddsRep", "Value", out var Result2))
						{
							AddsRep.AppendDescription(stringBuilder, Result, Result2);
						}
						stringBuilder.Append("\nYou may tighten your ").Append(cachedDisplayNameStripped).Append(" to receive double the AV bonus at a -2 DV penalty as long as you remain still.")
							.Append("\nCannot wear body armor.");
						return Event.FinalizeString(stringBuilder);
			*/
			string displayName = __instance.Blueprint.CachedDisplayNameStripped;
			StringBuilder stringBuilder = XRL.World.Event.NewStringBuilder();
			stringBuilder.AppendSigned(Carapace.GetAVModifier(__0), "rules").Append(" AV\n")
			    .AppendSigned(Carapace.GetDVModifier(__0), "rules").Append(" DV\n")
			    .AppendSigned(Carapace.GetHeatResistance(__0), "rules").Append(" 열 저항\n")
			    .AppendSigned(Carapace.GetColdResistance(__0), "rules").Append(" 냉기 저항");
			if (__instance.Blueprint.TryGetPartParameter<string>("AddsRep", "Faction", out var faction) && __instance.Blueprint.TryGetPartParameter<int>("AddsRep", "Value", out var value))
			{
			    AddsRep.AppendDescription(stringBuilder, faction, value);
			}
			stringBuilder.Append($"\n가만히 있는 동안 {displayName}을(를) 조여 AV 보너스를 두 배로 받는 대신 DV -2 페널티를 받을 수 있습니다.")
			    .Append("\n몸통 방어구를 착용할 수 없습니다.");
			__result = XRL.World.Event.FinalizeString(stringBuilder);
		}
	}

	[HarmonyPatch(typeof(ColdAbsorption), nameof(ColdAbsorption.GetLevelText))]
	public static class ColdAbsorption_GetLevelText_Translate
	{
		static void Postfix(ColdAbsorption __instance, int __0, ref string __result)
		{
			/*
			string text = "Immune to heat damage\n";
						if (Level > 1)
						{
							text = text + "Whenever you would have taken cold damage, you heal for 0." + (Level - 1) + "% of that damage instead";
						}
						return text;
			*/
			string text = "열 피해 면역\n";
			if (__0 > 1)
			{
			    int percent = __0 - 1;
			    text = text + $"냉기 피해를 받을 때마다 그 피해의 0.{percent}%만큼 회복합니다.";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(Cryokinesis), nameof(Cryokinesis.GetLevelText))]
	public static class Cryokinesis_GetLevelText_Translate
	{
		static void Postfix(Cryokinesis __instance, int __0, ref string __result)
		{
			/*
			string text = "";
						text = ((Level == base.Level) ? ("Chills affected area over " + Duration.Things("round") + ", dealing damage and freezing things\n") : ((Level <= base.Level) ? "{{rules|Decreased chill temperature intensity}}\n" : "{{rules|Increased chill temperature intensity}}\n"));
						text = text + "Range: " + Range + "\n";
						int num = Radius * 2 + 1;
						text = text + "Area: " + num + "x" + num + "\n";
						for (int i = 1; i <= Duration; i++)
						{
							text = text + "Round " + i + " damage: {{rules|" + Level + "d" + GetDamageDieSize(i) + "}} divided by 2\n";
						}
						return text + "Cooldown: 50 rounds";
			*/
			string text = "";
			if (__0 == __instance.Level)
			{
			    string durationText = __instance.Duration.Things("round");
			    text = $"{durationText} 동안 영향을 받은 영역을 냉각하여 피해를 주고 얼립니다.\n";
			}
			else if (__0 <= __instance.Level)
			{
			    text = "{{rules|냉기 온도 강도 감소}}\n";
			}
			else
			{
			    text = "{{rules|냉기 온도 강도 증가}}\n";
			}
			text = text + $"사정거리: {__instance.Range}\n";
			int size = __instance.Radius * 2 + 1;
			text = text + $"영역: {size}x{size}\n";
			for (int i = 1; i <= __instance.Duration; i++)
			{
			    text = text + $"{i}라운드 피해: {{{{rules|{__0}d{Cryokinesis.GetDamageDieSize(i)}}}}} / 2\n";
			}
			__result = text + "쿨다운: 50 라운드";
		}
	}

	[HarmonyPatch(typeof(Dystechnia), nameof(Dystechnia.GetDescription))]
	public static class Dystechnia_GetDescription_Translate
	{
		static void Postfix(Dystechnia __instance, ref string __result)
		{
			/*
			string text = "";
						text += "You are befuddled by technological complexity.\n\n";
						text += "You're much worse at examining artifacts.\n";
						text += "You can't have artifacts identified for you because you don't understand their explanations.\n";
						text += "When you fail severely during artifact examination, the artifact explodes.\n";
						if (Options.AnySifrah)
						{
							text += "You lose a turn in most tinkering Sifrah games, and two turns in hacking Sifrah games.";
						}
						return text;
			*/
			string text = "";
			text += "당신은 기술적 복잡함에 어리둥절합니다.\n\n";
			text += "유물을 조사하는 데 훨씬 서툽니다.\n";
			text += "설명을 이해하지 못해 유물 감정을 받을 수 없습니다.\n";
			text += "유물 조사에서 심각하게 실패하면 유물이 폭발합니다.\n";
			if (Options.AnySifrah)
			{
			    text += "대부분의 땜질 시프라 게임에서 한 턴을 잃고, 해킹 시프라 게임에서는 두 턴을 잃습니다.";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(ElectricalGeneration), nameof(ElectricalGeneration.GetLevelText))]
	public static class ElectricalGeneration_GetLevelText_Translate
	{
		static void Postfix(ElectricalGeneration __instance, int __0, ref string __result)
		{
			/*
			StringBuilder stringBuilder = Event.NewStringBuilder();
						stringBuilder.Append("Maximum charge: {{C|").Append(GetMaxCharge(Level)).Append("}}");
						stringBuilder.Append("\nAccrue base {{C|").Append(GetBaseChargePerTurn(Level, BaseChargePerTurnPercent)).Append("}} charge per turn");
						stringBuilder.Append("\nCan discharge all held charge for 1d4 damage per ").Append(1000).Append(" charge");
						stringBuilder.Append("\nDischarge can arc to adjacent targets dealing reduced damage, up to 1 target per ").Append(1000).Append(" charge");
						stringBuilder.Append("\nEMP causes involuntary discharge (difficulty 18 Willpower save)");
						if (CanDrinkTransient)
						{
							stringBuilder.Append("\nYou can drink change from electrical power sources.");
						}
						else
						{
							stringBuilder.Append("\nYou can drink charge from energy cells and capacitors.");
						}
						stringBuilder.Append("\nYou gain ").Append(100).Append(" charge per point of electrical damage taken.");
						stringBuilder.Append("\nYou can provide charge to equipped devices that have integrated power systems.");
						return stringBuilder.ToString();
			*/
			int maxCharge = ElectricalGeneration.GetMaxCharge(__0);
			int baseCharge = ElectricalGeneration.GetBaseChargePerTurn(__0, __instance.BaseChargePerTurnPercent);
			int dischargeChunk = ElectricalGeneration.DISCHARGE_CHUNK;
			int absorbFactor = ElectricalGeneration.DAMAGE_ABSORB_FACTOR;
			StringBuilder stringBuilder = XRL.World.Event.NewStringBuilder();
			stringBuilder.Append($"최대 전하: {{{{C|{maxCharge}}}}}");
			stringBuilder.Append($"\n매 턴 기본 {{{{C|{baseCharge}}}}} 전하를 축적합니다");
			stringBuilder.Append($"\n보유한 전하를 모두 방출해 전하 {dischargeChunk}당 1d4 피해를 줍니다");
			stringBuilder.Append($"\n방출은 인접 대상에게 감쇄 피해로 튕겨 나갈 수 있으며, 전하 {dischargeChunk}당 최대 1명에게 연쇄됩니다");
			stringBuilder.Append("\nEMP는 비자발적 방전을 유발합니다 (의지력 내성 난이도 18)");
			if (__instance.CanDrinkTransient)
			{
			    stringBuilder.Append("\n전기 전원에서 전하를 마실 수 있습니다.");
			}
			else
			{
			    stringBuilder.Append("\n에너지 셀과 축전기에서 전하를 마실 수 있습니다.");
			}
			stringBuilder.Append($"\n전기 피해 1점당 전하 {absorbFactor}을 얻습니다.");
			stringBuilder.Append("\n내장 전원 시스템이 있는 장비에 전하를 공급할 수 있습니다.");
			__result = stringBuilder.ToString();
		}
	}

	[HarmonyPatch(typeof(Esper), nameof(Esper.GetDescription))]
	public static class Esper_GetDescription_Translate
	{
		static void Postfix(Esper __instance, ref string __result)
		{
			/*
			string text = "You only manifest mental mutations, and all of your mutation choices when manifesting a new mutation are mental.";
						if (Options.AnySifrah)
						{
							text += "\nAdds a bonus turn and improves performance in psionic Sifrah games.";
						}
						return text;
			*/
			string text = "당신은 정신 변이만 발현하며, 새로운 변이를 발현할 때의 선택지도 모두 정신 변이입니다.";
			if (Options.AnySifrah)
			{
			    text += "\n초능력 시프라 게임에서 보너스 턴을 추가하고 성능을 향상시킵니다.";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(FireBreather), nameof(FireBreather.GetLevelText))]
	public static class FireBreather_GetLevelText_Translate
	{
		static void Postfix(FireBreather __instance, int __0, ref string __result)
		{
			/*
			string text = "Breathes fire in a cone.\n";
						text = text + "Damage: " + ComputeDamage(Level) + "\n";
						text = text + "Cone length: " + GetConeLength() + " tiles\n";
						text = text + "Cone angle: " + GetConeAngle() + " degrees\n";
						text += "Cooldown: 15 rounds\n";
						if (Level != base.Level)
						{
							text += "{{rules|Increased temperature}}";
						}
						return text;
			*/
			string text = "원뿔 형태로 불을 뿜습니다.\n";
			string damage = __instance.ComputeDamage(__0);
			text = text + $"피해: {damage}\n";
			text = text + $"원뿔 길이: {__instance.GetConeLength()} 타일\n";
			text = text + $"원뿔 각도: {__instance.GetConeAngle()}도\n";
			text += "쿨다운: 15 라운드\n";
			if (__0 != __instance.Level)
			{
			    text += "{{rules|온도 증가}}";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(FlamingRay), nameof(FlamingRay.GetDescription))]
	public static class FlamingRay_GetDescription_Translate
	{
		static void Postfix(FlamingRay __instance, ref string __result)
		{
			/*
			BodyPart registeredSlot = GetRegisteredSlot(BodyPartType, evenIfDismembered: true);
						if (registeredSlot != null)
						{
							return "You emit a ray of flame from your " + registeredSlot.GetOrdinalName() + ".";
						}
						return "You emit a ray of flame.";
			*/
			BodyPart registeredSlot = __instance.GetRegisteredSlot(__instance.BodyPartType, evenIfDismembered: true);
			if (registeredSlot != null)
			{
			    string ordinalName = registeredSlot.GetOrdinalName();
			    __result = $"당신의 {ordinalName}에서 화염 광선을 방출합니다.";
			    return;
			}
			__result = "당신은 화염 광선을 방출합니다.";
		}
	}

	[HarmonyPatch(typeof(GasGeneration), nameof(GasGeneration.GetLevelText))]
	public static class GasGeneration_GetLevelText_Translate
	{
		static void Postfix(GasGeneration __instance, int __0, ref string __result)
		{
			/*
			string text = "";
						text = text + "Releases gas for {{rules|" + GetReleaseDuration(Level) + "}} rounds";
						if (Level != base.Level)
						{
							string tag = GeneratedGasBlueprint().GetTag("LevelEffectDescription");
							if (tag != null)
							{
								text = ((Level <= base.Level) ? (text + "\n{{rules|Decreased " + tag + "}}") : (text + "\n{{rules|Increased " + tag + "}}"));
							}
						}
						return text + "\nCooldown: " + GetReleaseCooldown(Level) + " rounds";
			*/
			string text = "";
			int duration = __instance.GetReleaseDuration(__0);
			text = text + $"가스를 {{{{rules|{duration}}}}} 라운드 동안 방출합니다";
			if (__0 != __instance.Level)
			{
			    GameObjectBlueprint blueprint = GameObjectFactory.Factory.GetBlueprint(__instance.GasObject);
			    string tag = blueprint.GetTag("LevelEffectDescription");
			    if (tag != null)
			    {
			        if (__0 <= __instance.Level)
			        {
			            text = text + $"\n{{{{rules|{tag} 감소}}}}";
			        }
			        else
			        {
			            text = text + $"\n{{{{rules|{tag} 증가}}}}";
			        }
			    }
			}
			int cooldown = __instance.GetReleaseCooldown(__0);
			__result = text + $"\n쿨다운: {cooldown} 라운드";
		}
	}

	[HarmonyPatch(typeof(HeatAbsorption), nameof(HeatAbsorption.GetLevelText))]
	public static class HeatAbsorption_GetLevelText_Translate
	{
		static void Postfix(HeatAbsorption __instance, int __0, ref string __result)
		{
			/*
			string text = "Immune to heat damage\n";
						if (Level > 1)
						{
							text = text + "Whenever you would have taken heat damage, you heal for " + (Level - 1) * 10 + "% of that damage instead";
						}
						return text;
			*/
			string text = "열 피해 면역\n";
			if (__0 > 1)
			{
			    int percent = (__0 - 1) * 10;
			    text = text + $"열 피해를 받을 때마다 그 피해의 {percent}%만큼 회복합니다.";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(HeightenedEgo), nameof(HeightenedEgo.GetLevelText))]
	public static class HeightenedEgo_GetLevelText_Translate
	{
		static void Postfix(HeightenedEgo __instance, int __0, ref string __result)
		{
			/*
			string text = (2 + (Level - 1) / 2).Signed() + " Ego\n";
						text = text + "Creatures within a radius of " + GetAlertRadius(Level) + " are alerted to your presence [unimplemented]\n";
						if (Level == BaseLevel)
						{
							text += "Small chance to frighten an adjacent enemy [unimplemented]";
						}
						else if (Level % 2 == 0)
						{
							text += "Increased chance to frighten an adjacent enemy [unimplemented]";
						}
						return text;
			*/
			int egoBonus = 2 + (__0 - 1) / 2;
			string text = $"{egoBonus.Signed()} 자아\n";
			int radius = __instance.GetAlertRadius(__0);
			text = text + $"반경 {radius} 내의 생물은 당신의 존재를 감지합니다 [미구현]\n";
			if (__0 == __instance.BaseLevel)
			{
			    text += "인접한 적을 겁주게 할 작은 확률 [미구현]";
			}
			else if (__0 % 2 == 0)
			{
			    text += "인접한 적을 겁주게 할 확률 증가 [미구현]";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(HeightenedHearing), nameof(HeightenedHearing.GetLevelText))]
	public static class HeightenedHearing_GetLevelText_Translate
	{
		static void Postfix(HeightenedHearing __instance, int __0, ref string __result)
		{
			/*
			string text = "You detect the presence of creatures within a radius of {{rules|" + GetRadius(Level) + "}}.\n";
						if (Level == base.Level)
						{
							return text + "Chance to identify nearby detected creatures";
						}
						return text + "{{rules|Increased chance to identify nearby detected creatures}}";
			*/
			int radius = HeightenedHearing.GetRadius(__0);
			string text = $"반경 {{{{rules|{radius}}}}} 내의 생물 존재를 감지합니다.\n";
			if (__0 == __instance.Level)
			{
			    text += "근처에서 감지한 생물을 식별할 확률";
			}
			else
			{
			    text += "{{rules|근처에서 감지한 생물을 식별할 확률 증가}}";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(HeightenedIntelligence), nameof(HeightenedIntelligence.GetLevelText))]
	public static class HeightenedIntelligence_GetLevelText_Translate
	{
		static void Postfix(HeightenedIntelligence __instance, int __0, ref string __result)
		{
			/*
			string text = (2 + (Level - 1) / 2).Signed() + " Intelligence\n";
						text = text + GetEgoPenalty(Level) + " Ego\n";
						if (Level == BaseLevel)
						{
							text += "Small chance to reveal the entire map in a flash of insight";
						}
						else if (Level % 2 == 0)
						{
							text += "Increased chance to reveal the entire map in a flash of insight";
						}
						return text;
			*/
			int intBonus = 2 + (__0 - 1) / 2;
			int egoPenalty = __instance.GetEgoPenalty(__0);
			string text = $"{intBonus.Signed()} 지능\n";
			text = text + $"{egoPenalty} 자아\n";
			if (__0 == __instance.BaseLevel)
			{
			    text += "통찰의 번뜩임으로 전체 지도를 밝혀낼 작은 확률";
			}
			else if (__0 % 2 == 0)
			{
			    text += "통찰의 번뜩임으로 전체 지도를 밝혀낼 확률 증가";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(HeightenedSmell), nameof(HeightenedSmell.GetLevelText))]
	public static class HeightenedSmell_GetLevelText_Translate
	{
		static void Postfix(HeightenedSmell __instance, int __0, ref string __result)
		{
			/*
			string text = "You detect the presence of creatures within a distance typically up to " + GetRadius(Level) + " squares depending on terrain\n";
						if (Level == base.Level)
						{
							return text + "Chance to identify nearby detected creatures";
						}
						return text + "{{rules|Increased chance to identify nearby detected creatures}}";
			*/
			int radius = HeightenedSmell.GetRadius(__0);
			string text = $"지형에 따라 보통 최대 {radius}칸 거리 내의 생물 존재를 감지합니다\n";
			if (__0 == __instance.Level)
			{
			    text += "근처에서 감지한 생물을 식별할 확률";
			}
			else
			{
			    text += "{{rules|근처에서 감지한 생물을 식별할 확률 증가}}";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(HeightenedWillpower), nameof(HeightenedWillpower.GetLevelText))]
	public static class HeightenedWillpower_GetLevelText_Translate
	{
		static void Postfix(HeightenedWillpower __instance, int __0, ref string __result)
		{
			/*
			string text = (2 + (Level - 1) / 2).Signed() + " Willpower\n";
						if (Level == BaseLevel)
						{
							text += "Small chance that you stubbornly refuse to flee from a fight [unimplemented]\n";
						}
						else if (Level % 2 != 0)
						{
							text += "Increased chance that you stubbornly refuse to flee from a flight\n";
						}
						if (Level == BaseLevel)
						{
							text += "Small chance when you are injured to ignore all damage for the next 5 rounds [unimplemented]";
						}
						else if (Level % 2 == 0)
						{
							text += "Increased chance when you are injured to ignore all damage for the next 5 rounds [unimplemented]";
						}
						return text;
			*/
			int wpBonus = 2 + (__0 - 1) / 2;
			string text = $"{wpBonus.Signed()} 의지력\n";
			if (__0 == __instance.BaseLevel)
			{
			    text += "전투에서 완강히 도망치지 않을 작은 확률 [미구현]\n";
			}
			else if (__0 % 2 != 0)
			{
			    text += "전투에서 완강히 도망치지 않을 확률 증가\n";
			}
			if (__0 == __instance.BaseLevel)
			{
			    text += "부상을 입었을 때 다음 5라운드 동안 모든 피해를 무시할 작은 확률 [미구현]";
			}
			else if (__0 % 2 == 0)
			{
			    text += "부상을 입었을 때 다음 5라운드 동안 모든 피해를 무시할 확률 증가 [미구현]";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(HooksForFeet), nameof(HooksForFeet.GetDescription))]
	public static class HooksForFeet_GetDescription_Translate
	{
		static void Postfix(HooksForFeet __instance, ref string __result)
		{
			/*
			string tag = Blueprint.GetTag("VariantDescription");
						if (tag.IsNullOrEmpty())
						{
							return "You have " + Blueprint.DisplayName() + " for feet.\n\nYou cannot wear shoes.";
						}
						return tag;
			*/
			string tag = __instance.Blueprint.GetTag("VariantDescription");
			if (tag.IsNullOrEmpty())
			{
			    string displayName = __instance.Blueprint.DisplayName();
			    __result = $"당신의 발은 {displayName}입니다.\n\n신발을 착용할 수 없습니다.";
			    return;
			}
			__result = tag;
		}
	}

	[HarmonyPatch(typeof(Horns), nameof(Horns.GetDescription))]
	public static class Horns_GetDescription_Translate
	{
		static void Postfix(Horns __instance, ref string __result)
		{
			/*
			if (Variant == null)
						{
							return "Horns jut out of your head.";
						}
						GameObjectBlueprint blueprint = GameObjectFactory.Factory.GetBlueprint(Variant);
						string propertyOrTag = blueprint.GetPropertyOrTag("Gender");
						string cachedDisplayNameStripped = blueprint.CachedDisplayNameStripped;
						if (propertyOrTag == "plural")
						{
							return Grammar.InitCap(cachedDisplayNameStripped) + " jut out of your head.";
						}
						return Grammar.A(cachedDisplayNameStripped, Capitalize: true) + " juts out of your head.";
			*/
			if (__instance.Variant == null)
			{
			    __result = "머리에서 뿔이 솟아납니다.";
			    return;
			}
			GameObjectBlueprint blueprint = GameObjectFactory.Factory.GetBlueprint(__instance.Variant);
			string hornName = blueprint.CachedDisplayNameStripped;
			__result = $"{hornName}이(가) 머리에서 솟아납니다.";
		}
	}

	[HarmonyPatch(typeof(Horns), nameof(Horns.GetLevelText))]
	public static class Horns_GetLevelText_Translate
	{
		static void Postfix(Horns __instance, int __0, ref string __result)
		{
			/*
			string baseDamage = GetBaseDamage(Level);
						int aV = GetAV(Level);
						string text = "20% chance on melee attack to gore your opponent\n";
						text = text + "Damage increment: {{rules|" + baseDamage + "}}\n";
						text = text + "To-hit bonus: {{rules|" + HornsProperties.GetToHitBonus(Level) + "}}\n";
						text = ((Level == base.Level) ? (text + "Goring attacks may cause bleeding\n") : ((Level % 4 != 1) ? (text + "{{rules|Increased bleeding save difficulty}}\n") : (text + "{{rules|Increased bleeding save difficulty and intensity}}\n")));
						string text2 = "plural";
						string word;
						if (Variant == null)
						{
							word = "horns";
						}
						else
						{
							GameObjectBlueprint blueprint = GameObjectFactory.Factory.GetBlueprint(Variant);
							text2 = blueprint.GetPropertyOrTag("Gender");
							word = blueprint.CachedDisplayNameStripped;
						}
						text = ((!(text2 == "plural")) ? (text + Grammar.InitCap(word) + " is a short-blade class natural weapon.\n") : (text + Grammar.InitCap(word) + " are a short-blade class natural weapon.\n"));
						text = text + "+{{rules|" + aV + " AV}}\n";
						text += "Cannot wear helmets\n";
						return text + "+100 reputation with {{w|antelopes}} and {{w|goatfolk}}";
			*/
			string baseDamage = __instance.GetBaseDamage(__0);
			int aV = __instance.GetAV(__0);
			string text = "근접 공격 시 20% 확률로 상대를 들이받습니다.\n";
			text = text + $"피해 증가량: {{{{rules|{baseDamage}}}}}\n";
			int toHitBonus = HornsProperties.GetToHitBonus(__0);
			text = text + $"명중 보너스: {{{{rules|{toHitBonus}}}}}\n";
			if (__0 == __instance.Level)
			{
			    text = text + "들이받기 공격은 출혈을 유발할 수 있습니다.\n";
			}
			else if (__0 % 4 != 1)
			{
			    text = text + "{{rules|출혈 내성 난이도 증가}}\n";
			}
			else
			{
			    text = text + "{{rules|출혈 내성 난이도 및 강도 증가}}\n";
			}
			string gender = "plural";
			string word;
			if (__instance.Variant == null)
			{
			    word = "뿔";
			}
			else
			{
			    GameObjectBlueprint blueprint = GameObjectFactory.Factory.GetBlueprint(__instance.Variant);
			    gender = blueprint.GetPropertyOrTag("Gender");
			    word = blueprint.CachedDisplayNameStripped;
			}
			text = text + $"{Grammar.InitCap(word)}은(는) 단검 계열 자연 무기입니다.\n";
			text = text + $"+{{{{rules|{aV} AV}}}}\n";
			text += "투구를 착용할 수 없습니다.\n";
			__result = text + "{{w|영양}} 및 {{w|염소족}} 평판 +100";
		}
	}

	[HarmonyPatch(typeof(IceBreather), nameof(IceBreather.GetLevelText))]
	public static class IceBreather_GetLevelText_Translate
	{
		static void Postfix(IceBreather __instance, int __0, ref string __result)
		{
			/*
			string text = "Breathes ice in a cone.\n";
						text = text + "Damage: " + ComputeDamage(Level) + "\n";
						text = text + "Cone length: " + GetConeLength() + " tiles\n";
						text = text + "Cone angle: " + GetConeAngle() + " degrees\n";
						text += "Cooldown: 15 rounds\n";
						if (Level != base.Level)
						{
							text += "Decreased temperature.";
						}
						return text;
			*/
			string text = "원뿔 형태로 얼음을 뿜습니다.\n";
			string damage = __instance.ComputeDamage(__0);
			text = text + $"피해: {damage}\n";
			text = text + $"원뿔 길이: {__instance.GetConeLength()} 타일\n";
			text = text + $"원뿔 각도: {__instance.GetConeAngle()}도\n";
			text += "쿨다운: 15 라운드\n";
			if (__0 != __instance.Level)
			{
			    text += "온도 감소.";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(MultiHorns), nameof(MultiHorns.GetLevelText))]
	public static class MultiHorns_GetLevelText_Translate
	{
		static void Postfix(MultiHorns __instance, int __0, ref string __result)
		{
			/*
			string text = "";
						int value = 0;
						if (Level == 1)
						{
							text = "2d3";
							value = 0;
						}
						if (Level == 2)
						{
							text = "2d4";
							value = 0;
						}
						if (Level == 3)
						{
							text = "2d4";
							value = 1;
						}
						if (Level == 4)
						{
							text = "2d5";
							value = 1;
						}
						if (Level == 5)
						{
							text = "2d5";
							value = 1;
						}
						if (Level == 6)
						{
							text = "2d6";
							value = 1;
						}
						if (Level == 7)
						{
							text = "2d6";
							value = 2;
						}
						if (Level == 8)
						{
							text = "2d7";
							value = 2;
						}
						if (Level == 9)
						{
							text = "2d7";
							value = 2;
						}
						if (Level >= 10)
						{
							text = "2d8";
							value = 2;
						}
						string text2 = "20% chance on melee attack to gore your opponent\n";
						text2 = text2 + "Damage increment: " + text + "\n";
						text2 = ((Level != base.Level) ? (text2 + "{{rules|Increased bleeding save difficulty and intensity}}\n") : (text2 + "Goring attacks may cause bleeding\n"));
						text2 = text2 + value.Signed() + " AV\n";
						text2 += "Cannot wear helmets\n";
						text2 += "Can launch into a destructive charge after a one round warm-up.\n";
						return text2 + "Charge distance: " + GetChargeDistance(Level);
			*/
			string damageText = "";
			int avBonus = 0;
			if (__0 == 1)
			{
			    damageText = "2d3";
			    avBonus = 0;
			}
			if (__0 == 2)
			{
			    damageText = "2d4";
			    avBonus = 0;
			}
			if (__0 == 3)
			{
			    damageText = "2d4";
			    avBonus = 1;
			}
			if (__0 == 4)
			{
			    damageText = "2d5";
			    avBonus = 1;
			}
			if (__0 == 5)
			{
			    damageText = "2d5";
			    avBonus = 1;
			}
			if (__0 == 6)
			{
			    damageText = "2d6";
			    avBonus = 1;
			}
			if (__0 == 7)
			{
			    damageText = "2d6";
			    avBonus = 2;
			}
			if (__0 == 8)
			{
			    damageText = "2d7";
			    avBonus = 2;
			}
			if (__0 == 9)
			{
			    damageText = "2d7";
			    avBonus = 2;
			}
			if (__0 >= 10)
			{
			    damageText = "2d8";
			    avBonus = 2;
			}
			string text = "근접 공격 시 20% 확률로 상대를 들이받습니다.\n";
			text = text + $"피해 증가량: {damageText}\n";
			text = (__0 != __instance.Level) ? (text + "{{rules|출혈 내성 난이도 및 강도 증가}}\n") : (text + "들이받기 공격은 출혈을 유발할 수 있습니다.\n");
			text = text + $"{avBonus.Signed()} AV\n";
			text += "투구를 착용할 수 없습니다.\n";
			text += "1라운드 준비 후 파괴적인 돌진을 할 수 있습니다.\n";
			__result = text + $"돌진 거리: {__instance.GetChargeDistance(__0)}";
		}
	}

	[HarmonyPatch(typeof(Psychometry), nameof(Psychometry.GetLevelText))]
	public static class Psychometry_GetLevelText_Translate
	{
		static void Postfix(Psychometry __instance, int __0, ref string __result)
		{
			/*
			StringBuilder stringBuilder = Event.NewStringBuilder();
						if (Options.SifrahExamine)
						{
							stringBuilder.Compound("Adds a bonus turn and is useful in many tinkering and some ritual Sifrah games.");
						}
						else
						{
							stringBuilder.Compound("Unerringly identify artifacts up to complexity tier {{rules|", "\n").Append(GetIdentifiableComplexity(Level)).Append("}}.");
							stringBuilder.Compound("Learn how to construct identified artifacts up to complexity tier {{rules|", "\n").Append(GetLearnableComplexity(Level)).Append("}} (must have the appropriate Tinker skill).");
						}
						stringBuilder.Compound("You may open security doors and use some secure devices by touching them.", "\n");
						return stringBuilder.ToString();
			*/
			StringBuilder stringBuilder = XRL.World.Event.NewStringBuilder();
			if (Options.SifrahExamine)
			{
			    stringBuilder.Compound("보너스 턴을 추가하며 많은 땜질 및 일부 의식 시프라 게임에 유용합니다.");
			}
			else
			{
			    stringBuilder.Compound("복잡도 등급 {{rules|", "\n").Append(Psychometry.GetIdentifiableComplexity(__0)).Append("}}까지의 유물을 정확히 감정합니다.");
			    stringBuilder.Compound("감정한 유물 중 복잡도 등급 {{rules|", "\n").Append(Psychometry.GetLearnableComplexity(__0)).Append("}}까지 제작 방법을 배웁니다 (적절한 땜질 기술 필요).");
			}
			stringBuilder.Compound("보안 문을 열고 일부 보안 장치를 접촉하여 사용할 수 있습니다.", "\n");
			__result = stringBuilder.ToString();
		}
	}

	[HarmonyPatch(typeof(Pyrokinesis), nameof(Pyrokinesis.GetLevelText))]
	public static class Pyrokinesis_GetLevelText_Translate
	{
		static void Postfix(Pyrokinesis __instance, int __0, ref string __result)
		{
			/*
			string text = "";
						text = ((Level == base.Level) ? ("Toasts affected area over " + Duration.Things("round") + "\n") : ((Level <= base.Level) ? "{{rules|Decreased toast temperature intensity}}\n" : "{{rules|Increased toast temperature intensity}}\n"));
						text = text + "Range: " + Range + "\n";
						int num = Radius * 2 + 1;
						text = text + "Area: " + num + "x" + num + "\n";
						for (int i = 1; i <= Duration; i++)
						{
							text = text + "Round " + i + " damage: {{rules|" + Level + "d" + GetDamageDieSize(i) + "}} divided by 2\n";
						}
						return text + "Cooldown: 50 rounds";
			*/
			string text = "";
			if (__0 == __instance.Level)
			{
			    string durationText = __instance.Duration.Things("round");
			    text = $"{durationText} 동안 영향을 받은 영역을 달굽니다.\n";
			}
			else if (__0 <= __instance.Level)
			{
			    text = "{{rules|열기 온도 강도 감소}}\n";
			}
			else
			{
			    text = "{{rules|열기 온도 강도 증가}}\n";
			}
			text = text + $"사정거리: {__instance.Range}\n";
			int size = __instance.Radius * 2 + 1;
			text = text + $"영역: {size}x{size}\n";
			for (int i = 1; i <= __instance.Duration; i++)
			{
			    text = text + $"{i}라운드 피해: {{{{rules|{__0}d{Pyrokinesis.GetDamageDieSize(i)}}}}} / 2\n";
			}
			__result = text + "쿨다운: 50 라운드";
		}
	}

	[HarmonyPatch(typeof(Quills), nameof(Quills.GetLevelText))]
	public static class Quills_GetLevelText_Translate
	{
		static void Postfix(Quills __instance, int __0, ref string __result)
		{
			/*
			string value = GetQuillPenetration(Level).ToString();
						int aVPenalty = GetAVPenalty(Level);
						StringBuilder stringBuilder = Event.NewStringBuilder();
						string objectName = ObjectName;
						if (Level == base.Level)
						{
							stringBuilder.Append("{{rules|").Append(nMaxQuills).Append("}} ")
								.Append(objectName)
								.Append('\n');
						}
						else
						{
							stringBuilder.Append("+{{rules|80-120}} ").Append(objectName).Append('\n');
						}
						stringBuilder.Append("May expel 10% of your ").Append(objectName).Append(" in a burst around yourself ({{c|\u001a}}{{rules|")
							.Append(value)
							.Append("}} {{r|\u0003}}1d3)\n")
							.Append("Regenerate ")
							.Append(objectName)
							.Append(" at the approximate rate of {{rules|")
							.Append((float)Level / 4f)
							.Append("}} per round\n")
							.Append("+{{rules|")
							.Append(GetAV(Level))
							.Append("}} AV as long as you retain half your ")
							.Append(objectName)
							.Append(" (+{{rules|")
							.Append(GetAV(Level) - aVPenalty)
							.Append("}} AV otherwise)\n")
							.Append("Creatures attacking you in melee may impale themselves on your ")
							.Append(objectName)
							.Append(", breaking roughly 1% of them and reflecting 3% damage per ")
							.Append(ObjectNameSingular)
							.Append(" broken.\n")
							.Append("Cannot wear body armor\n")
							.Append("Immune to other creatures' ")
							.Append(objectName);
						return Event.FinalizeString(stringBuilder);
			*/
			string value = __instance.GetQuillPenetration(__0).ToString();
			int aVPenalty = __instance.GetAVPenalty(__0);
			int av = __instance.GetAV(__0);
			StringBuilder stringBuilder = XRL.World.Event.NewStringBuilder();
			string objectName = __instance.ObjectName;
			string objectNameSingular = __instance.ObjectNameSingular;
			if (__0 == __instance.Level)
			{
			    stringBuilder.Append($"{{{{rules|{__instance.nMaxQuills}}}}} {objectName}\n");
			}
			else
			{
			    stringBuilder.Append($"+{{{{rules|80-120}}}} {objectName}\n");
			}
			stringBuilder.Append($"당신의 {objectName} 10%를 주변으로 폭발적으로 방출할 수 있습니다 ({{{{c|\u001a}}}}{{{{rules|{value}}}}} {{{{r|\u0003}}}}1d3)\n")
			    .Append("매 라운드 대략 {{rules|")
			    .Append((float)__0 / 4f)
			    .Append("}}개의 ")
			    .Append(objectName)
			    .Append("을(를) 재생합니다\n")
			    .Append($"+{{{{rules|{av}}}}} AV (그 외에는 +{{{{rules|{av - aVPenalty}}}}} AV)\n")
			    .Append("근접 공격하는 생물은 당신의 ")
			    .Append(objectName)
			    .Append("에 스스로 찔릴 수 있으며, 약 1%가 부러지고 부러진 ")
			    .Append(objectNameSingular)
			    .Append("당 3%의 피해를 반사합니다.\n")
			    .Append("몸통 방어구를 착용할 수 없습니다.\n")
			    .Append("다른 생물의 ")
			    .Append(objectName)
			    .Append("에 면역입니다.");
			__result = XRL.World.Event.FinalizeString(stringBuilder);
		}
	}

	[HarmonyPatch(typeof(Regeneration), nameof(Regeneration.GetLevelText))]
	public static class Regeneration_GetLevelText_Translate
	{
		static void Postfix(Regeneration __instance, int __0, ref string __result)
		{
			/*
			string text = "";
						text += "Your full natural healing rate applies in combat.\n";
						text = text + "+{{rules|" + (int)(100f * GetRegenerationBonus(Level)) + "%}} faster natural healing rate\n";
						text = text + "{{rules|" + GetRegenerationChance(Level) + "%}} chance to regrow a missing limb each round\n";
						if (GetRegenerationChance(Level) >= 100)
						{
							text += "{{rules|You cannot be decapitated.}}\n";
						}
						if (Level < 5)
						{
							return text + "{{rules|" + GetDebuffChance(Level) + "}}% chance to remove a {{rules|minor physical debuff}} at random each round";
						}
						return text + "{{rules|" + GetDebuffChance(Level) + "}}% chance to remove a {{rules|physical debuff}} at random each round";
			*/
			string text = "";
			text += "전투 중에도 완전한 자연 회복 속도가 적용됩니다.\n";
			int regenBonusPercent = (int)(100f * __instance.GetRegenerationBonus(__0));
			text = text + $"자연 회복 속도 {{{{rules|{regenBonusPercent}%}}}} 증가\n";
			int regenChance = __instance.GetRegenerationChance(__0);
			text = text + $"매 라운드 결손된 팔다리가 재생될 확률 {{{{rules|{regenChance}%}}}}\n";
			if (__instance.GetRegenerationChance(__0) >= 100)
			{
			    text += "{{rules|참수될 수 없습니다.}}\n";
			}
			int debuffChance = __instance.GetDebuffChance(__0);
			if (__0 < 5)
			{
			    __result = text + $"{{{{rules|{debuffChance}}}}}% 확률로 매 라운드 무작위 {{{{rules|경미한 신체 디버프}}}} 제거";
			    return;
			}
			__result = text + $"{{{{rules|{debuffChance}}}}}% 확률로 매 라운드 무작위 {{{{rules|신체 디버프}}}} 제거";
		}
	}

	[HarmonyPatch(typeof(RepellingForce), nameof(RepellingForce.GetLevelText))]
	public static class RepellingForce_GetLevelText_Translate
	{
		static void Postfix(RepellingForce __instance, int __0, ref string __result)
		{
			/*
			StringBuilder stringBuilder = Event.NewStringBuilder();
						stringBuilder.Compound("Area: 7x7", '\n');
						if (Level == base.Level)
						{
							stringBuilder.Compound("Creatures are pushed away from center of blast.", '\n');
						}
						else
						{
							stringBuilder.Compound("{{rules|Increased push force}}", '\n');
						}
						stringBuilder.Compound("Cooldown: ", '\n').Append(30).Append(" rounds");
						return stringBuilder.ToString();
			*/
			StringBuilder stringBuilder = XRL.World.Event.NewStringBuilder();
			stringBuilder.Compound("영역: 7x7", '\n');
			if (__0 == __instance.Level)
			{
			    stringBuilder.Compound("생물은 폭발 중심에서 밀려납니다.", '\n');
			}
			else
			{
			    stringBuilder.Compound("{{rules|밀어내는 힘 증가}}", '\n');
			}
			stringBuilder.Compound("쿨다운: ", '\n').Append(30).Append(" 라운드");
			__result = stringBuilder.ToString();
		}
	}

	[HarmonyPatch(typeof(SpacetimeVortex), nameof(SpacetimeVortex.GetLevelText))]
	public static class SpacetimeVortex_GetLevelText_Translate
	{
		static void Postfix(SpacetimeVortex __instance, int __0, ref string __result)
		{
			/*
			string text = "Summons a vortex that swallows everything in its path.\n";
						if (Level > 10)
						{
							text = text + "Bonus duration: {{rules|" + (Level - 10) + "}} rounds\n";
						}
						text = text + "Cooldown: {{rules|" + GetCooldown(Level) + "}} rounds\n";
						text += "You may enter the vortex to teleport to a random location in Qud.\n";
						return text + "+200 reputation with {{w|highly entropic beings}}";
			*/
			string text = "경로에 있는 모든 것을 집어삼키는 소용돌이를 소환합니다.\n";
			if (__0 > 10)
			{
			    text = text + $"추가 지속 시간: {{{{rules|{__0 - 10}}}}} 라운드\n";
			}
			int cooldown = __instance.GetCooldown(__0);
			text = text + $"쿨다운: {{{{rules|{cooldown}}}}} 라운드\n";
			text += "소용돌이에 들어가 쿠드의 무작위 위치로 순간이동할 수 있습니다.\n";
			__result = text + "{{w|고엔트로피 존재들}} 평판 +200";
		}
	}

	[HarmonyPatch(typeof(Spinnerets), nameof(Spinnerets.GetLevelText))]
	public static class Spinnerets_GetLevelText_Translate
	{
		static void Postfix(Spinnerets __instance, int __0, ref string __result)
		{
			/*
			StringBuilder stringBuilder = Event.NewStringBuilder();
						stringBuilder.Compound("While spinning, you leave webs in your wake as you move.", '\n');
						if (Level != base.Level)
						{
							stringBuilder.Compound("{{rules|Increased web strength}}", '\n');
						}
						stringBuilder.Compound("Duration: {{rules|", '\n').Append(GetDuration(Level)).Append("}} move actions");
						SavingThrows.AppendSaveBonusDescription(stringBuilder, GetMoveSaveModifier(), "Move", HighlightNumber: true);
						stringBuilder.Compound("Cooldown: 80 rounds", '\n');
						stringBuilder.Compound("You are immune to getting stuck.", '\n');
						stringBuilder.Compound("+300 reputation with {{w|arachnids}}", '\n');
						return stringBuilder.ToString();
			*/
			StringBuilder stringBuilder = XRL.World.Event.NewStringBuilder();
			stringBuilder.Compound("회전하는 동안 이동 경로에 거미줄을 남깁니다.", '\n');
			if (__0 != __instance.Level)
			{
			    stringBuilder.Compound("{{rules|거미줄 강도 증가}}", '\n');
			}
			stringBuilder.Compound("지속 시간: {{rules|", '\n').Append(__instance.GetDuration(__0)).Append("}} 이동 행동");
			SavingThrows.AppendSaveBonusDescription(stringBuilder, __instance.GetMoveSaveModifier(), "Move", HighlightNumber: true);
			stringBuilder.Compound("쿨다운: 80 라운드", '\n');
			stringBuilder.Compound("붙잡힘에 면역입니다.", '\n');
			stringBuilder.Compound("{{w|거미류}} 평판 +300", '\n');
			__result = stringBuilder.ToString();
		}
	}

	[HarmonyPatch(typeof(Telekinesis), nameof(Telekinesis.GetDescription))]
	public static class Telekinesis_GetDescription_Translate
	{
		static void Postfix(Telekinesis __instance, ref string __result)
		{
			/*
			string text = "You can move things with your mind.";
						if (Options.AnySifrah)
						{
							text += "\nUseful in many tinkering Sifrah games.";
						}
						return text;
			*/
			string text = "당신은 정신으로 사물을 움직일 수 있습니다.";
			if (Options.AnySifrah)
			{
			    text += "\n많은 땜질 시프라 게임에서 유용합니다.";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(Telepathy), nameof(Telepathy.GetDescription))]
	public static class Telepathy_GetDescription_Translate
	{
		static void Postfix(Telepathy __instance, ref string __result)
		{
			/*
			string text = "";
						text += "You may communicate with others through the psychic aether.\n\n";
						text += "Chat with anyone in vision\nTakes you much less time to issue orders to companions";
						if (Options.AnySifrah)
						{
							text += "\nUseful in many social and psionic Sifrah games.";
						}
						return text;
			*/
			string text = "";
			text += "당신은 정신 에테르를 통해 다른 이들과 소통할 수 있습니다.\n\n";
			text += "시야 내 누구와도 대화할 수 있습니다.\n동료에게 명령을 내리는 시간이 크게 줄어듭니다.";
			if (Options.AnySifrah)
			{
			    text += "\n많은 사회 및 초능력 시프라 게임에서 유용합니다.";
			}
			__result = text;
		}
	}

	[HarmonyPatch(typeof(Wings), nameof(Wings.GetLevelText))]
	public static class Wings_GetLevelText_Translate
	{
		static void Postfix(Wings __instance, int __0, ref string __result)
		{
			/*
			int num = Math.Max(0, FlightBaseFallChance - Level);
						StringBuilder stringBuilder = Event.NewStringBuilder();
						stringBuilder.Append("You travel on the world map at {{rules|").Append(1.5 + 0.5 * (double)Level).Append("x}} speed.\n");
						stringBuilder.Append("{{rules|" + (36 + Level * 4)).Append("%}} reduced chance of becoming lost\n");
						stringBuilder.Append("While outside, you may fly. You cannot be hit in melee by grounded creatures while flying.\n");
						stringBuilder.Append("{{rules|" + num).Append("%}} chance of falling clumsily to the ground\n");
						stringBuilder.Append("{{rules|" + ((int)(SprintingMoveSpeedBonus(Level) * 100f)).Signed() + "%}} move speed while sprinting\n");
						stringBuilder.Append("You can jump {{rules|" + GetJumpDistanceBonus(Level) + ((GetJumpDistanceBonus(Level) == 1) ? "}} square" : "
			*/
			int fallChance = Math.Max(0, __instance.FlightBaseFallChance - __0);
			double speed = 1.5 + 0.5 * (double)__0;
			int lostChance = 36 + __0 * 4;
			int sprintBonus = (int)(__instance.SprintingMoveSpeedBonus(__0) * 100f);
			int jumpBonus = __instance.GetJumpDistanceBonus(__0);
			int chargeBonus = __instance.GetChargeDistanceBonus(__0);
			StringBuilder stringBuilder = XRL.World.Event.NewStringBuilder();
			stringBuilder.Append($"월드맵에서 {{{{rules|{speed}}}}}x 속도로 이동합니다.\n");
			stringBuilder.Append($"{{{{rules|{lostChance}%}}}} 길 잃을 확률 감소\n");
			stringBuilder.Append("야외에서는 비행할 수 있습니다. 비행 중에는 지상에 있는 생물에게 근접 공격을 받지 않습니다.\n");
			stringBuilder.Append($"{{{{rules|{fallChance}%}}}} 확률로 서툴게 땅으로 떨어집니다.\n");
			stringBuilder.Append($"전력질주 시 이동 속도 {{{{rules|{sprintBonus.Signed()}%}}}}\n");
			stringBuilder.Append($"점프 거리가 {{{{rules|{jumpBonus}}}}}칸 증가합니다.\n");
			stringBuilder.Append($"돌진 거리가 {{{{rules|{chargeBonus}}}}}칸 증가합니다.\n");
			stringBuilder.Append("{{w|새}} 및 {{w|날개 달린 포유류}} 평판 +300");
			__result = stringBuilder.ToString();
		}
	}
}