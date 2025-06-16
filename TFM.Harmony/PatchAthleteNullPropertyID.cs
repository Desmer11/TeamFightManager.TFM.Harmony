using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;


namespace PatchCreateProperties
{
	[HarmonyPatch(typeof(PropertyConfig), "CreateProperties")]
	public static class PropertyConfig_CreateProperties_Patch
	{
		private static ManualLogSource Logger;

		public static void InitializeLogger(ManualLogSource logger)
		{
			Logger = logger;
		}
		static void Postfix(ref List<AthleteProperty> __result, PropertyConfig __instance, IRandomInterface rand)
		{
			Logger?.LogInfo("CreateProperties: Starting PostFix...");





			// Access the Properties field from PropertyConfig
			var properties = AccessTools.Field(typeof(PropertyConfig), "Properties").GetValue(__instance) as List<PropertyProb>;
			if (properties == null) return;

			// Filter out invalid properties
			var validProperties = properties.Where(p => p.Name != "null" && p.Name != "none" && p.Name != "ordinary").ToList();

			// Ensure the result contains exactly 3 valid properties
			while (__result.Count < 3 && validProperties.Any())
			{
				var randomProperty = validProperties.RandomSelect(rand);
				if (randomProperty != null)
				{
					__result.Add(AthleteProperty.Create(randomProperty.Name));
				}
			}

			// Trim to exactly 3 properties
			__result = __result.Take(3).ToList();





			Logger?.LogInfo($"CreateProperties: {__result}...");
		}
	}
}