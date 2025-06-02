using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;


namespace PatchHarmonyChange
{
	[HarmonyPatch(typeof(ChampionPatch))]
	[HarmonyPatch("Change")]
	public static class ChampionPatch_Change_Patch
	{
		private static ManualLogSource Logger;

		public static void InitializeLogger(ManualLogSource logger)
		{
			Logger = logger;
		}
		static void Postfix(GameConfig config, string name, float point, ref PatchData __result)
		{
			Logger.LogInfo($"[Harmony] Change called with name='{name}', point={point}");
			Logger.LogInfo($"[Harmony] PatchData result: Attack={__result.Attack}, Magic={__result.Magic}, AttackSpeed={__result.AttackSpeed}, Defence={__result.Defence}, MaxHp={__result.MaxHp}, SkillCool={__result.SkillCool}");
		}
	}
}