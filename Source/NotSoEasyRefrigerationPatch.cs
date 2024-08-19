using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using Verse;
using RimWorld;

namespace NotSoEasyRefrigeration
{
	[StaticConstructorOnStartup]
	static class HarmonyPatches
	{
		static HarmonyPatches()
		{
			Harmony harmony = new Harmony("Azuraal.NotSoEasyRefrigeration");
			Assembly assembly = Assembly.GetExecutingAssembly();
			harmony.PatchAll(assembly);
		}
	}

	[HarmonyPatch(typeof(Verse.GenTemperature), "RotRateAtTemperature")]
	class RotRatePatch
	{
		static NotSoEasyRefrigerationSettings Settings => NotSoEasyRefrigerationMod.Settings;
		public static Dictionary<int, float> RotRateDict = new Dictionary<int, float>();
		public static int MinRotRateThreshold;
		static RotRatePatch()
		{
			CasheRotRate();
		}
		public static void CasheRotRate()
		{
			RotRateDict = new Dictionary<int, float>();
			float normalisationFactor = (float)Math.Pow(Settings.RefrigerationCurveBase, Settings.RefrigerationMaxTemperature);
			int temperature = (int)Settings.RefrigerationMaxTemperature;
			float RotRate;
			while ((RotRate = (float)Math.Pow(Settings.RefrigerationCurveBase, temperature) / normalisationFactor) > Settings.RefrigerationMinRotRate && temperature > Settings.FreezingTemperature)
			{
				RotRateDict.Add(temperature, RotRate);
				temperature--;
			}
			MinRotRateThreshold = temperature;
		}
		static void Postfix(float temperature, ref float __result)
		{
			if (temperature >= Settings.RefrigerationMaxTemperature)
			{
				__result = 1f;
				return;
			}
			else if (temperature < Settings.FreezingTemperature)
			{
				__result = 0f;
				return;
			}
			else if (temperature <= MinRotRateThreshold)
			{
				__result = Settings.RefrigerationMinRotRate;
				return;
			}
			else
			{
				if (RotRateDict.ContainsKey((int)temperature)) __result = RotRateDict[(int)temperature];
				else Log.Error($"NotSoEasyRefrigeration: Dictionary does not contain key: {(int)temperature}.");
				return;
			}
		}
	}
}
