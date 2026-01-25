using System;
using System.Collections.Generic;
using ConsoleLib.Console;
using HarmonyLib;

namespace KorFontTest.Patches
{
	internal static class ActivatedAbilityNameTranslator
	{
		private struct TemplateRule
		{
			public string[] Parts;
			public string Format;

			public TemplateRule(string[] parts, string format)
			{
				Parts = parts;
				Format = format;
			}
		}

		private static readonly Dictionary<string, string> NameMap = new Dictionary<string, string>
		{
			{ "Swoop", "급강하" },
			{ "Sprint", "달리기" },
			{ "Stop Burrowing", "굴파기 중지" },
			{ "End Domination", "지배 중지" },
			{ "Phase", "물질 통과" },
			{ "Surface", "표면" },
			{ "Clone", "복제" },
			{ "Anchor Spikes", "닻 가시" },
			{ "Anomaly Fumigator", "변칙 훈증기" },
			{ "Wormhole", "웜홀" },
			{ "Pyrokinesis Field", "발화 능력 장" },
			{ "Stunning Force", "기절 충격" },
			{ "Glitter Bomb", "섬광 폭탄" },
			{ "Fire Suppression", "화재 진압" },
			{ "Medassist Module", "의료 지원 모듈" },
			{ "Night Vision", "야간 투시" },
			{ "Penetrating Radar", "투과 레이더" },
			{ "Phase-Adaptive Projectiles", "위상 적응 투사체" },
			{ "Phase Harmonic Modulator", "위상 조화 변조기" },
			{ "Fabricate Force Knife", "역장 단도 제작" },
			{ "Stasis Arena", "정지장 투기장" },
			{ "Stasis Entangler", "정지장 속박" },
			{ "Dig", "파기" },
			{ "Eject", "배출" },
			{ "Engulf", "삼키기" },
			{ "Change Grip", "손잡이 변경" },
			{ "Aggressive Stance", "공격 자세" },
			{ "Defensive Stance", "방어 자세" },
			// { "\"Lay Mine [\" + MineName + \"mk \" + Grammar.GetRomanNumeral(Mark) + \"]\"", "" },
			{ "Puff Spores", "포자 분출" },
			{ "Recoil", "반동" },
			{ "ActiveAbilityName", "능력 이름" },
			{ "Run Over", "짓밟기" },
			{ "Single Weapon Fighting", "단일 무기 전투" },
			{ "Activate Flume-Flier", "플룸-플라이어 활성화" },
			{ "Activate Stopsvalinn", "스톱스발린 활성화" },
			// { "\"Activate \" + Grammar.MakeTitleCase(ParentObject.BaseDisplayNameStripped)", "" },
			{ "Set Target Temperature", "목표 온도 설정" },
			{ "Rifle through Trash", "쓰레기 뒤지기" },
			// { "\"Tinker Turret  [\" + MaxTurretsPlaced + \" remaining]\"", "" },
			{ "Exit pilot seat", "조종석 나가기" },
			{ "Spit Acid", "산성 액체 발사" },
			{ "Release Adrenaline", "아드레날린 분비" },
			{ "Beguile Creature", "생명체 매혹" },
			{ "CommandName", "명령 이름" },
			{ "Burgeoning", "급성장" },
			{ "Burrow", "굴파기" },
			{ "Excavate up", "위로 굴착" },
			{ "Excavate down", "아래로 굴착" },
			// { "\"Tighten \" + GetDisplayName()", "" },
			{ "Clairvoyance", "투시" },
			{ "Confusion", "혼란" },
			{ "Chill", "냉기" },
			{ "Decarbonize", "탈탄소화" },
			{ "Scintillate", "섬광" },
			{ "Disintegration", "분해" },
			{ "Dominate Creature", "생명체 지배" },
			{ "Discharge", "방전" },
			{ "Power Devices", "장치 작동" },
			{ "Emit Pulse", "파동 방출" },
			{ "Teleport", "순간이동" },
			{ "Fear Aura", "공포 오라" },
			{ "Flaming Ray", "화염 광선" },
			{ "Force Bubble", "역장 방어막" },
			{ "Force Wall", "역장 벽" },
			{ "Freezing Ultraray", "냉동 초광선" },
			{ "Freezing Ray", "냉동 광선" },
			{ "Knit Frosty Webs", "서리 거미줄 생성" },
			{ "Label", "라벨" },
			{ "Infiltrate", "침투" },
			{ "Irisdual Beam", "홍채 이중 광선" },
			{ "Kindle", "점화" },
			{ "Ley Shift", "레이 이동" },
			{ "Syphon Vim", "활력 흡수" },
			{ "Lase", "레이저 발사" },
			{ "Ambient Light", "주변 조명" },
			{ "Spit Liquid", "액체 발사" },
			{ "Magnetic Pulse", "자기 펄스" },
			{ "Tap the Mass Mind", "집단 정신 접속" },
			{ "Mental Mirror", "정신 거울" },
			{ "End Metamorphosis", "변신 종료" },
			{ "Metamorphosis", "변신" },
			{ "Wrecking Charge", "파괴적 돌진" },
			{ "Bask", "휴식" },
			{ "Precognition - Start vision", "예지 - 환영 시작" },
			{ "Precognition - End vision", "예지 - 환영 종료" },
			{ "Psychometry", "잔상 독해" },
			{ "Toast", "소각" },
			{ "AbilityName", "능력 이름" },
			{ "Serenity", "평온" },
			{ "Spit Slime", "점액 발사" },
			{ "Spew", "분출" },
			{ "Spacetime Vortex", "시공간 소용돌이" },
			{ "Spin Webs", "거미줄 생성" },
			{ "Tongue", "혓바닥 공격" },
			{ "Sting", "침 공격" },
			{ "Sunder Mind", "정신 파괴" },
			{ "Telekinesis", "염동력" },
			{ "Telekinetic Throwing", "염동력 투척" },
			{ "Telepathy", "텔레파시" },
			{ "Teleport Other", "타인 순간이동" },
			{ "Time Dilation", "시간 왜곡" },
			{ "Waveform Dash", "파동 돌진" },
			{ "Boost Strength", "힘 강화" },
			{ "Boost Agility", "민첩 강화" },
			{ "Boost Toughness", "강인함 강화" },
			{ "Berserk!", "광폭화!" },
			{ "Decapitate", "참수" },
			{ "Dismember", "절단" },
			{ "Hook and Drag", "갈고리 끌어당기기" },
			{ "Butcher Corpses", "시체 해체" },
			{ "Harvest Plants", "식물 수확" },
			{ "Conk", "강타" },
			{ "Slam", "내리치기" },
			{ "Demolish", "파괴" },
			{ "Meditate", "명상" },
			{ "Sweep", "휩쓸기" },
			{ "En Garde!", "앙가르드!" },
			{ "Dueling Stance", "결투 자세" },
			{ "Lunge", "찌르기 돌진" },
			{ "Swipe", "휘두르기" },
			{ "Flurry", "난타" },
			{ "Berate", "호통" },
			{ "Intimidate", "위협" },
			{ "Menacing Stare", "위협적 응시" },
			{ "Proselytize", "개종 권유" },
			{ "Rebuke Robot", "로봇 질책" },
			{ "Amputate Limb", "사지 절단" },
			{ "Akimbo", "쌍권총" },
			{ "Empty the Clips", "탄창 비우기" },
			{ "Mark Target", "목표 표시" },
			{ "Shield Wall", "방패 벽" },
			{ "Shield Slam", "방패 강타" },
			{ "Hobble", "다리 절기" },
			{ "Rejoinder", "반격" },
			{ "Shank", "암살" },
			{ "Catapult", "투석기" },
			{ "Howl", "포효" },
			{ "Submerge", "잠수" },
			{ "Make Camp", "야영" },
			{ "Charge", "돌진" },
			{ "Death From Above", "급강하 공격" },
			{ "Juke", "속임 동작" },
			{ "Deploy Turret", "포탑 배치" },
			{ "Lay Mine", "지뢰 설치" },
			{ "Set Bomb", "폭탄 설치" },
			{ "Recharge", "재충전" },

		};

		private static readonly Dictionary<string, string> CommandMap = new Dictionary<string, string>
		{
			// Add command-based translations here (useful for templated names).
			// { "CommandCharge", "돌진" },
		};

		private static readonly TemplateRule[] TemplateRules = new TemplateRule[]
		{
			// Example for templated names:
			new TemplateRule(new[] { "Lay Mine [", "]" }, "{0} 지뢰 설치"),
			new TemplateRule(new[] { "Tinker Turret [", "]" }, "{0} 터렛 제작")
		};

		public static string Translate(string name, string command)
		{
			if (string.IsNullOrEmpty(name))
			{
				return name;
			}

			if (!string.IsNullOrEmpty(command) && CommandMap.TryGetValue(command, out string byCommand))
			{
				return byCommand;
			}

			if (NameMap.TryGetValue(name, out string byName))
			{
				return byName;
			}

			string templated = TryApplyTemplate(name);
			return templated ?? name;
		}

		private static string TryApplyTemplate(string message)
		{
			for (int i = 0; i < TemplateRules.Length; i++)
			{
				TemplateRule rule = TemplateRules[i];
				string[] parts = rule.Parts;
				if (parts == null || parts.Length == 0)
				{
					continue;
				}
				if (!message.StartsWith(parts[0], StringComparison.Ordinal))
				{
					continue;
				}
				int pos = parts[0].Length;
				string[] values = new string[parts.Length - 1];
				bool match = true;
				for (int partIndex = 1; partIndex < parts.Length; partIndex++)
				{
					string part = parts[partIndex] ?? string.Empty;
					if (part.Length == 0)
					{
						if (partIndex == parts.Length - 1)
						{
							values[partIndex - 1] = message.Substring(pos);
							pos = message.Length;
						}
						else
						{
							values[partIndex - 1] = string.Empty;
						}
						continue;
					}

					int next = message.IndexOf(part, pos, StringComparison.Ordinal);
					if (next < 0)
					{
						match = false;
						break;
					}
					if (partIndex == parts.Length - 1 && next + part.Length != message.Length)
					{
						match = false;
						break;
					}
					values[partIndex - 1] = message.Substring(pos, next - pos);
					pos = next + part.Length;
				}

				if (!match)
				{
					continue;
				}

				return string.Format(rule.Format, values);
			}

			return null;
		}
	}

	[HarmonyPatch(typeof(XRL.World.GameObject), nameof(XRL.World.GameObject.AddActivatedAbility))]
	[HarmonyPatch(new Type[]
	{
		typeof(string),
		typeof(string),
		typeof(string),
		typeof(string),
		typeof(string),
		typeof(string),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(int),
		typeof(string),
		typeof(Renderable),
		typeof(Renderable),
		typeof(Renderable),
		typeof(Renderable)
	})]
	public static class GameObject_AddActivatedAbility_NameTranslate
	{
		static void Prefix(ref string Name, string Command)
		{
			Name = ActivatedAbilityNameTranslator.Translate(Name, Command);
		}
	}
}
