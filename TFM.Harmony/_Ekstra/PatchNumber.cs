
//using System;
//using System.Collections.Generic;
//using HarmonyLib;

//using BepInEx.Logging;
//using System.Linq;
//using UnityEngine;
//using System.Reflection;



//namespace PatchNumberHarmony
//{
//	[HarmonyPatch]
//	public static class PatchNumberPatch
//	{
//		private static ManualLogSource Logger;

//		public static void InitializeLogger(ManualLogSource logger)
//		{
//			Logger = logger;
//		}


//		[HarmonyPatch(typeof(ChampionPatch), "Patch")]

//		public static class ChampionPatch_Patch_Postfix
//		{
//			public static void Postfix(ChampionPatch __instance, GameConfig config, ref List<PatchData> __result)
//			{
//				Logger?.LogInfo("PatchNumberPatch: Starting postfix...");

//				if (__result == null)
//				{
//					Logger?.LogWarning("Original method returned null; no buffs or nerfs applied.");
//					return;
//				}

//				// Log the result from the original method
//				Logger?.LogInfo($"Original result count: {__result.Count}");

//				// Get champions and their win rates

//				List<ChampionInfo> list = (from c in config.GetChampionsByPatch(Store.Global.Get<TodayData>(0).Patch)
//										   orderby __instance.GetWinRate(c.Name)
//										   select c).ToList();


//				Logger?.LogInfo($"Champion list count: {list.Count}");

//				int min = 1 + (list.Count - 4) / 10;
//				int max = 1 + list.Count / 6;
//				int b = UnityEngine.Random.Range(min, max);
//				int b2 = UnityEngine.Random.Range(min, max);

//				Logger?.LogInfo($"Random ranges: min={min}, max={max}, b={b}, b2={b2}");

//				int num = Mathf.Max(list.Count(c => __instance.GetWinRate(c.Name) <= 0.40f), 10);
//				int num2 = Mathf.Max(list.Count(c => __instance.GetWinRate(c.Name) >= 0.60f), 10);

//				Logger?.LogInfo($"Initial num (buffs): {num}, num2 (nerfs): {num2}");

//				if (num + num2 >= 6)
//				{
//					//num = Mathf.Clamp(Mathf.RoundToInt(6f * (float)num / (float)(num + num2)), 10, 10);
//					//num2 = 6 - num;
//					Logger?.LogInfo($"Clamped num: {num}, num2: {num2}");
//				}

//				// Apply nerfs in addition to the original method's result
//				var changeMethod = typeof(ChampionPatch).GetMethod("Change", BindingFlags.NonPublic | BindingFlags.Instance);
//				if (changeMethod == null)
//				{
//					Logger?.LogError("Cannot find 'Change' method");
//					return;
//				}

//				Logger?.LogInfo("Invoking Change method for nerfs...");
//				List<PatchData> nerfs = list.Skip(list.Count - num2)
//					.Select(c => (PatchData)changeMethod.Invoke(__instance, new object[] { config, c.Name, __instance.GetWinRate(c.Name) }))
//					.ToList();
//				Logger?.LogInfo($"Nerfs created: {nerfs.Count}");

//				// Combine buffs and nerfs with the original result
//				__result.AddRange(nerfs);

//				Logger?.LogInfo("PatchNumberPatch: Postfix finished.");
//			}
//		}

//	}
//}
