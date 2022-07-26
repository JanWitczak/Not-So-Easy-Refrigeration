using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using Verse;

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
		static Dictionary<int, float> RotRate = new Dictionary<int, float>();
		static RotRatePatch()
		{
			for (int t = -19; t < 10; t++)
			{
				RotRate.Add(t, (float)System.Math.Pow(1.16f, t) / 4.411435f); // 1.16^x curve, normalised to 1 at x=10
			}
		}
		static void Postfix(float temperature, ref float __result)
		{
			if (temperature >= 10f)
			{
				__result = 1f;
				return;
			}
			else if (temperature <= -20f && temperature > -100f)
			{
				__result = 0.01f;
				return;
			}
			else if (temperature <= -100f)
			{
				__result = 0f;
				return;
			}
			else
			{
				__result = RotRate[(int)temperature];
				return;
			}
		}
	}
}
