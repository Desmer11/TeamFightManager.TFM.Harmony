using System;
using System.Collections.Generic;
using HarmonyLib;
using BepInEx.Logging;
using System.Linq;
using UnityEngine;
using System.Reflection;

namespace PatchNumberHarmony
{
	[HarmonyPatch]
	public static class PatchNumberPatch
	{
		private static ManualLogSource Logger;

		public static void InitializeLogger(ManualLogSource logger)
		{
			Logger = logger;
		}

		[HarmonyPatch(typeof(ChampionPatch), "Patch")]
		public static class ChampionPatch_Patch_Prefix
		{


			public static bool Prefix(ChampionPatch __instance, GameConfig config, ref List<PatchData> __result)
			{
				Logger?.LogInfo("PatchNumberPatch: Starting prefix...");

				List<ChampionInfo> list = (from c in config.GetChampionsByPatch(Store.Global.Get<TodayData>(0).Patch)
										   orderby __instance.GetWinRate(c.Name)
										   select c).ToList();

				Logger?.LogInfo($"Champion list count: {list.Count}");

				int min = 1 + (list.Count - 4) / 10;
				int max = 1 + list.Count / 6;
				int b = UnityEngine.Random.Range(min, max);
				int b2 = UnityEngine.Random.Range(min, max);

				//Logger?.LogInfo($"Random ranges: min={min}, max={max}, b={b}, b2={b2}");

				int num = Mathf.Max(list.Count(c => __instance.GetWinRate(c.Name) <= 0.45f), b);
				int num2 = Mathf.Max(list.Count(c => __instance.GetWinRate(c.Name) >= 0.55f), b2);

				Logger?.LogInfo($"Initial num (buffs): {num}, num2 (nerfs): {num2}");

				if (num + num2 >= 6)
				{
					num = Mathf.Clamp(Mathf.RoundToInt(10f * (float)num / (float)(num + num2)), 2, 4);
					num2 = 10 - num;
					Logger?.LogInfo($"Clamped num: {num}, num2: {num2}");
				}

				GamePlusData gamePlusData = Store.Global.Get<GamePlusData>(0);
				PatchIntensity patchIntensity = (gamePlusData != null) ? gamePlusData.Intensity : PatchIntensity.Weak;
				Logger?.LogInfo($"Patch intensity: {patchIntensity}");

				if (patchIntensity == PatchIntensity.Normal)
				{
					int num3 = UnityEngine.Random.Range(8, 12);
					num = UnityEngine.Random.Range(3, num3 - 1);
					num2 = num3 - num;
					Logger?.LogInfo($"-- IF:Normal --Normal intensity adjusted num: {num}, num2: {num2}");
				}

				if (patchIntensity == PatchIntensity.Weak)
				{
					int num4 = UnityEngine.Random.Range(10, Mathf.Min(list.Count + 10, 15));
					num = UnityEngine.Random.Range(4, num4 - 3);
					num2 = num4 - num;
					Logger?.LogInfo($"-- IF:Weak -- Strong intensity adjusted num: {num}, num2: {num2}");
				}
				if (patchIntensity == PatchIntensity.Strong)
				{
					Logger?.LogInfo($"-- IF:Strong -- List.Count{list.Count}");
					int num4 = UnityEngine.Random.Range(20, Mathf.Min(list.Count + 10, 15));
					num = UnityEngine.Random.Range(4, num4 - 3);
					num2 = num4 - num;
					Logger?.LogInfo($"-- IF:Strong -- Strong intensity adjusted num: {num}, num2: {num2}");
				}
				else
				{
					int num4 = UnityEngine.Random.Range(10, 20);
					num = UnityEngine.Random.Range(4, Mathf.Max(num4 - 3, 4)); // Ensure range is valid
					num2 = num4 - num;
					Logger?.LogInfo($"-- ELSE --Strong intensity adjusted num: {num}, num2: {num2}");
				}

				var changeMethod = typeof(ChampionPatch).GetMethod("Change", BindingFlags.NonPublic | BindingFlags.Instance);
				if (changeMethod == null)
				{
					//Logger?.LogError("Cannot find 'Change' method");
					throw new Exception("Cannot find 'Change' method");
				}

				Logger?.LogInfo("Invoking Change method for buffs...");
				List<PatchData> buffs = list.Take(num)
					.Select(c => (PatchData)changeMethod.Invoke(__instance, new object[] { config, c.Name, __instance.GetWinRate(c.Name) }))
					.ToList();
				Logger?.LogInfo($"Buffs created: {buffs.Count}");

				Logger?.LogInfo("Invoking Change method for nerfs...\n");
				List<PatchData> nerfs = list.Skip(list.Count - num2)
					.Select(c => (PatchData)changeMethod.Invoke(__instance, new object[] { config, c.Name, __instance.GetWinRate(c.Name) }))
					.ToList();
				Logger?.LogInfo($"Nerfs created: {nerfs.Count}");


				// Find champions with a win rate below 35%
				List<ChampionInfo> lowWinRateChampions = list.Where(c => __instance.GetWinRate(c.Name) < 0.35f).ToList();

								Logger?.LogInfo($"Adding {lowWinRateChampions.Count} additional entries for LOW-win-rate champions...");

								// Add low-win-rate champions twice
								foreach (var champion in lowWinRateChampions)
								{
									buffs.Add((PatchData)changeMethod.Invoke(__instance, new object[] { config, champion.Name, __instance.GetWinRate(champion.Name) }));
								}


				// Find champions with a win rate  75%
				List<ChampionInfo> highWinRate = list.Where(c => __instance.GetWinRate(c.Name) > 0.75f).ToList();

								Logger?.LogInfo($"Adding {highWinRate.Count} additional entries for HIGH-win-rate champions...");

								foreach (var champion in highWinRate)
								{
									buffs.Add((PatchData)changeMethod.Invoke(__instance, new object[] { config, champion.Name, __instance.GetWinRate(champion.Name) }));
								}

								Logger?.LogInfo($"-- HIGH-LOW WINRATE-- Total buffs after adding HIGH-LOW-win-rate champions: {buffs.Count}");


				//UNUSED CHAMPIONS Or 10 % WINRATE
				List<ChampionInfo> unused = list.Where(c => __instance.GetWinRate(c.Name) < 0.1f).ToList();

								Logger?.LogInfo($"Adding {unused.Count} additional entries for UNUSED Or 10% Winrate...");

								// Add low-win-rate champions twice
								foreach (var champion in unused)
								{
									buffs.Add((PatchData)changeMethod.Invoke(__instance, new object[] { config, champion.Name, __instance.GetWinRate(champion.Name) }));
								}

				buffs.AddRange(nerfs);
				Logger?.LogInfo($"-- TOTAL FINISHED LIST-- Total buffs after adding HIGH-LOW-win-rate champions: {buffs.Count}");

				// Use reflection to reset private fields
				var winCntsField = typeof(ChampionPatch).GetField("WinCnts", BindingFlags.NonPublic | BindingFlags.Instance);
				var loseCntsField = typeof(ChampionPatch).GetField("LoseCnts", BindingFlags.NonPublic | BindingFlags.Instance);

				winCntsField.SetValue(__instance, new int[50]);
				loseCntsField.SetValue(__instance, new int[50]);

				Logger?.LogInfo("WinCnts and LoseCnts reset.");

				__result = buffs;
























				Logger?.LogInfo("PatchNumberPatch: Prefix finished, skipping original method.");

				return false;
			}
		}
	}
}
