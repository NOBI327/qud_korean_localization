using System;
using System.Collections.Generic;

namespace KorFontTest.Patches
{
	internal static class TextTranslator
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

			public TemplateRule(string prefix, string suffix, string format)
				: this(new[] { prefix, suffix }, format)
			{
			}
		}


		private static readonly Dictionary<string, string> Replacements = new Dictionary<string, string>
		{
			// Popup.Show
			// TutorialStep:22
			{ "We're not quite ready to leave yet.", "아직 떠날 준비가 되지 않았습니다." },
			// BattleRemains:34
			{ "You should view the tooltip before equipping the axe.", "도끼를 장착하기 전에, 툴팁을 확인해야 합니다." },
			// ExploreWorldMap:25
			{ "You'll be able to explore the world freely after the tutorial.\n\nFor now, let's visit Joppa.", "튜토리얼 후에 세계를 자유롭게 탐험할 수 있습니다. 지금은, 조파를 방문합시다." },
			// FightBear
			{ "Wait for the bear to take a step towards you.", "곰이 당신에게 가까이 올 때 까지 기다립시다." },
			{ "Make sure the bear is in the path of your freezing ray.", "빙결 광선 범위 내에 곰이 올 때 까지 기다리세요." },
			{ "It's quite dangerous to fight this bear in melee combat! Try backing away and using Freezing Ray.", "근접 전투로 곰과 싸우는 것은 꽤 위험합니다! 뒤로 물러나서 빙결 광선을 시도하세요." },
			{ "Let's take a look at the bear before we continue." , "진행하기 전에 곰을 살펴봅시다." },
			// FightSnapJaw
			{ "Before you move on, you should loot the corpse of the snapjaw.", "" },
			{ "Take a look at the snapjaw before we continue.", "" },
			// MoveToChest
			{ "Before you move on, you should equip yourself from the nearby chest.", "" },
			{ "Huh, you destroyed the tutorial chest we were going to teach you how to use.\n\nGo ahead and pick of the torch and dagger from the floor.", "" },
			// EquipmentAPI
			{ "You cannot do that from here.", "여기에서 할 수 없습니다." },
			// QudSpecificBootHandlersModule
			{ "You embark for the caves of Qud.", "당신은 커드의 동굴로 향합니다."},
			// Add or edit translations here.
			{ "You can only set your checkpoint in settlements.", "정착지에서만 체크포인트를 설정할 수 있습니다." }
		};

		private static readonly TemplateRule[] TemplateRules = new TemplateRule[]
		{
			// HistoricEvent:458
			new TemplateRule("You discover the location of ", ".", "당신은 {0}의 위치를 발견했습니다."),
			// EquipmentAPI
			// Popup.Show(GO.Does("are") + " out of your telekinetic range.");
			new TemplateRule(
				new[]
				{
					"You note the location of ",
					" in the {{W|",
					"}} section of your journal."
				},
				"{0}의 위치를 일지의 {{{{W|{1}}}}} 부분에 기록했습니다"
			),
			
			new TemplateRule("You discover ", "!", "{0}을(를) 발견했습니다!"),
			new TemplateRule("You pass by ", ".", "{0}을(를) 지나칩니다."),
			new TemplateRule("You have received a new quest, ", "!", "새로운 퀘스트 {0}을(를) 수락했습니다!"),
			new TemplateRule("You have failed the quest ", "!", "퀘스트 {0}을(를) 실패했습니다!"),
			new TemplateRule(
				new[] { "You have failed the step, {{R|", "}}, of the quest ", "!" },
				"퀘스트 {1}의 단계 {{{{R|{0}}}}}을(를) 실패했습니다!"
			),
			new TemplateRule(
				new[] { "You have finished the step, {{G|", "}}, of the quest ", "!\nYou gain {{C|", "}} XP!"},
				"퀘스트 {1}의 단계 {{{{G|{0}}}}}을(를) 완료했습니다!\n{2}XP를 획득했습니다!"
			),
			new TemplateRule(
				new[] { "You have finished the step, {{G|", "}}, of the quest ", "!"},
				"퀘스트 {1}의 단계 {{{{G|{0}}}}}을(를) 완료했습니다!"
			),
		};

		public static string Translate(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return message;
			}

			if (Replacements.TryGetValue(message, out string replacement))
			{
				return replacement;
			}

			string templated = TryApplyTemplate(message);
			if (templated != null)
			{
				return templated;
			}

			return message;
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

				try
				{
					return string.Format(rule.Format, values);
				}
				catch (FormatException)
				{
					continue;
				}
			}
			return null;
		}
	}
}
