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
//		public static class ChampionPatch_Patch_Prefix
//		{
//			public static bool Prefix(ChampionPatch __instance, GameConfig config, ref List<PatchData> __result)
//			{
//				List<ChampionInfo> list = (from c in config.GetChampionsByPatch(Store.Global.Get<TodayData>(0).Patch)
//										   orderby __instance.GetWinRate(c.Name)
//										   select c).ToList();

//				int min = 1 + (list.Count - 4) / 10;
//				int max = 1 + list.Count / 6;
//				int b = UnityEngine.Random.Range(min, max);
//				int b2 = UnityEngine.Random.Range(min, max);

//				int num = Mathf.Max(list.Count(c => __instance.GetWinRate(c.Name) <= 0.45f), b) + 3;
//				int num2 = Mathf.Max(list.Count(c => __instance.GetWinRate(c.Name) >= 0.55f), b2) + 3;

//				if (num + num2 >= 10)
//				{
//					num = Mathf.Clamp(Mathf.RoundToInt(10f * (float)num / (float)(num + num2)), 2, 6);
//					num2 = 10 - num;
//				}

//				GamePlusData gamePlusData = Store.Global.Get<GamePlusData>(0);
//				PatchIntensity patchIntensity = (gamePlusData != null) ? gamePlusData.Intensity : PatchIntensity.Weak;

//				if (patchIntensity == PatchIntensity.Normal)
//				{
//					int num3 = UnityEngine.Random.Range(8, 12);
//					num = UnityEngine.Random.Range(3, num3 - 1);
//					num2 = num3 - num;
//				}

//				if (patchIntensity == PatchIntensity.Strong)
//				{
//					int num4 = UnityEngine.Random.Range(10, Mathf.Min(list.Count + 1, 15));
//					num = UnityEngine.Random.Range(4, num4 - 3);
//					num2 = num4 - num;
//				}


//				var changeMethod = typeof(ChampionPatch).GetMethod("Change", BindingFlags.NonPublic | BindingFlags.Instance);
//				if (changeMethod == null) throw new Exception("Cannot find 'Change' method");

//				List<PatchData> buffs = list.Take(num)
//					.Select(c => (PatchData)changeMethod.Invoke(__instance, new object[] { config, c.Name, __instance.GetWinRate(c.Name) }))
//					.ToList();

//				List<PatchData> nerfs = list.Skip(list.Count - num2)
//					.Select(c => (PatchData)changeMethod.Invoke(__instance, new object[] { config, c.Name, __instance.GetWinRate(c.Name) }))
//					.ToList();


//				buffs.AddRange(nerfs);

//				// Use reflection to reset private fields
//				var winCntsField = typeof(ChampionPatch).GetField("WinCnts", BindingFlags.NonPublic | BindingFlags.Instance);
//				var loseCntsField = typeof(ChampionPatch).GetField("LoseCnts", BindingFlags.NonPublic | BindingFlags.Instance);

//				winCntsField.SetValue(__instance, new int[50]);
//				loseCntsField.SetValue(__instance, new int[50]);

//				__result = buffs;
//				return false;  // Skip original method
//			}
//		}

//	}
//}
