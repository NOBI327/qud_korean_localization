using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using TMPro;
using UnityEngine;
using XRL;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(ControlManager), "Update")]
	public static class TMPFallbackFontBundle
	{
		private const string BundleFileName = "d2coding.bundle";
		private static bool Attempted;
		private static TMP_FontAsset FallbackFont;

		static void Postfix()
		{
			if (!Attempted)
			{
				Attempted = true;
				LoadFallbackFont();
			}

			EnsureFallbackAdded();
		}

		private static void LoadFallbackFont()
		{
			ModInfo mod = ModManager.GetMod(typeof(TMPFallbackFontBundle).Assembly);
			string modPath = mod?.Path;
			if (string.IsNullOrEmpty(modPath))
			{
				Debug.LogError("TMP fallback bundle: could not resolve mod path.");
				return;
			}

			string bundlePath = Path.Combine(modPath, BundleFileName);
			if (!File.Exists(bundlePath))
			{
				string altPath = Path.Combine(modPath, "Assets", BundleFileName);
				if (File.Exists(altPath))
				{
					bundlePath = altPath;
				}
			}

			if (!File.Exists(bundlePath))
			{
				Debug.LogError("TMP fallback bundle not found: " + bundlePath);
				return;
			}

			AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
			if (bundle == null)
			{
				Debug.LogError("TMP fallback bundle failed to load: " + bundlePath);
				return;
			}

			TMP_FontAsset[] fonts = bundle.LoadAllAssets<TMP_FontAsset>();
			if (fonts == null || fonts.Length == 0)
			{
				Debug.LogError("TMP fallback bundle has no TMP_FontAsset: " + bundlePath);
				return;
			}

			FallbackFont = fonts[0];
			Debug.LogError("TMP fallback bundle loaded: " + FallbackFont.name);
		}

		internal static TMP_FontAsset GetFallbackFont()
		{
			if (!Attempted)
			{
				Attempted = true;
				LoadFallbackFont();
			}
			EnsureFallbackAdded();
			return FallbackFont;
		}

		internal static void EnsureFallbackAdded()
		{
			if (FallbackFont == null)
			{
				return;
			}

			if (TMP_Settings.fallbackFontAssets == null)
			{
				TMP_Settings.fallbackFontAssets = new List<TMP_FontAsset>();
			}

			if (TMP_Settings.fallbackFontAssets.Contains(FallbackFont))
			{
				return;
			}

			TMP_Settings.fallbackFontAssets.Add(FallbackFont);
			TextMeshProUGUI[] texts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
			for (int i = 0; i < texts.Length; i++)
			{
				texts[i].ForceMeshUpdate(ignoreActiveState: false, forceTextReparsing: true);
			}
			Debug.LogError("TMP fallback font added: " + FallbackFont.name);
		}
	}
}
