using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
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

		[HarmonyPostfix]
		static void Postfix(ChampionPatch __instance, GameConfig config, string name, float point, ref PatchData __result)
		{
			// Log before modification
			Logger.LogInfo($"[Harmony] INITIALIZED: Change for name='{name}', point={point}");

			// Calculate current max HP
			ChampionInfo entity = config.GetEntity<ChampionInfo>(name);
			if (entity == null)
			{
				Logger.LogError($"[Harmony] Failed to find champion with name '{name}'");
				return;
			}





			// Apply buffs to HP, Attack, Magic, and Defense
			//if (entity.MaxHp < 800)
			//{
			//	Logger.LogInfo($"[Harmony] Buffing HP: entity.MaxHp={entity.MaxHp}  Setting patch MaxHp to 80");
			//	entity.MaxHp = 800;
			//}
			//if (entity.MaxHp > 4000)
			//{
			//	Logger.LogInfo($"[Harmony] Buffing HP: entity.MaxHp={entity.MaxHp}  Setting patch MaxHp to 400");
			//	entity.MaxHp = 4000;
			//}


			//if (entity.Attack < 0)
			//{
			//	Logger.LogInfo($"[Harmony] Buffing Attack: entity.Attack={entity.Attack}  Setting patch Attack to 0");
			//	entity.Attack = 0;
			//}
			//if (entity.Attack > 150)
			//{
			//	Logger.LogInfo($"[Harmony] Buffing Attack: entity.Attack={entity.Attack}  Setting patch Attack to +150.");
			//	entity.Attack = 150;
			//}
			//if (entity.Magic < 0)
			//{
			//	Logger.LogInfo($"[Harmony] Buffing Magic: entity.Magic={entity.Magic}  Setting patch Magic to 0");
			//	entity.Magic = 0;
			//}
			//if (entity.Magic > 150)
			//{
			//	Logger.LogInfo($"[Harmony] Buffing Magic: entity.Magic={entity.Magic}  Setting patch Magic to 150");
			//	entity.Magic = 150;
			//}

			//if (entity.Defence < 5)
			//{
			//	Logger.LogInfo($"[Harmony] Buffing Defence: entity.Defence={entity.Defence} Setting patch Defence to 5");
			//	entity.Defence = 5;
			//}
			//if (entity.Defence > 100)
			//{
			//	Logger.LogInfo($"[Harmony] Buffing Defence: entity.Defence={entity.Defence} Setting patch Defence to 100");
			//	entity.Defence = 100;
			//}



			//______________________________________________________________________________________________________________________


			PatchData existingPatch = __instance.GetPatch(name);
			// Get base attack speed and skill cooldown values
			global::Action baseAttackAction = entity.GetAction("BaseAttack");
			ActionSwitch baseAttackActionSwitch = entity.GetAction<ActionSwitch>("BaseAttack");
			int baseAttackDelay = (baseAttackAction != null) ?
				baseAttackAction.Delay :
				(baseAttackActionSwitch != null && baseAttackActionSwitch.Actions.Count > 0) ?
					baseAttackActionSwitch.Actions[0].Delay :
					0;


			// Calculate the base attack speed
			int currentAttackSpeed = baseAttackDelay + (existingPatch?.AttackSpeed ?? 0);
			// Log current attack speed
			Logger.LogInfo($"[Harmony] Current AttackSpeed before patch: {currentAttackSpeed}");
			int addedAttackSpeed = __result.AttackSpeed;
			int calculatedAttackSpeed = currentAttackSpeed + addedAttackSpeed;
			// Attack speed boundaries (0.50 = 5 in game units, 2.0 = 20 in game units)
			const int minAttackSpeed = 5;  // Corresponds to 0.50 in game units
			const int maxAttackSpeed = 20; // Corresponds to 2.0 in game units

			// Clamp to boundaries
			int finalAttackSpeed = Mathf.Clamp(calculatedAttackSpeed, minAttackSpeed, maxAttackSpeed);

			// Adjust the result to reflect clamping
			if (finalAttackSpeed < calculatedAttackSpeed)
			{
				Logger.LogInfo($"[Harmony] AttackSpeed {calculatedAttackSpeed} exceeded max boundary {maxAttackSpeed}. Clamped to {maxAttackSpeed}.");
				__result.AttackSpeed = maxAttackSpeed - currentAttackSpeed; // Adjust the patch to match clamping
			}
			else if (finalAttackSpeed > calculatedAttackSpeed)
			{
				Logger.LogInfo($"[Harmony] AttackSpeed {calculatedAttackSpeed} was below min boundary {minAttackSpeed}. Clamped to {minAttackSpeed}.");
				__result.AttackSpeed = minAttackSpeed - currentAttackSpeed; // Adjust the patch to match clamping
			}
			else
			{
				Logger.LogInfo($"[Harmony] AttackSpeed within boundaries. No clamping applied.");
			}
			//if (finalAttackSpeed < 0)
			//{
			//	Logger.LogInfo($"[Harmony] AttackSpeed {calculatedAttackSpeed} exceeded max boundary {maxAttackSpeed}. Clamped to {maxAttackSpeed}.");
			//	finalAttackSpeed = Mathf.Clamp(calculatedAttackSpeed, minAttackSpeed, maxAttackSpeed);
			//}

			// Log final result
			Logger.LogInfo($"[Harmony] FINAL: AttackSpeed={__result.AttackSpeed}");


			//______________________________________________________________________________________________________________________



			// Get base skill cooldown value
			// Get base skill cooldown value
			//global::Action skillAction = entity.GetAction("Skill");
			//int baseSkillCooldown = skillAction?.Delay ?? 0;

			//// Current skill cooldown with existing patch
			//int currentSkillCooldown = baseSkillCooldown + (existingPatch?.SkillCool ?? 0);

			//// Log current values
			//Logger.LogInfo($"[Harmony] Base SkillCool: {baseSkillCooldown}, Current SkillCool: {currentSkillCooldown}");

			//// Skill cooldown boundaries (1.0 = 10 in game units, 10.0 = 100 in game units)
			//const int minSkillCooldown = 20;  // Corresponds to 1.0 in game units
			//const int maxSkillCooldown = 100; // Corresponds to 10.0 in game units

			//// Calculate final skill cooldown with new patch
			//int addedSkillCool = __result.SkillCool;
			//int finalSkillCooldown = currentSkillCooldown + addedSkillCool;

			//// Clamp final cooldown to boundaries
			//if (finalSkillCooldown < minSkillCooldown)
			//{
			//	__result.SkillCool += (minSkillCooldown - finalSkillCooldown); // Adjust to meet the minimum
			//	Logger.LogInfo($"[Harmony] Clamping SkillCool: {finalSkillCooldown} < {minSkillCooldown}. Adjusted to {minSkillCooldown}.");
			//}
			//else if (finalSkillCooldown > maxSkillCooldown)
			//{
			//	__result.SkillCool -= (finalSkillCooldown - maxSkillCooldown); // Adjust to meet the maximum
			//	Logger.LogInfo($"[Harmony] Clamping SkillCool: {finalSkillCooldown} > {maxSkillCooldown}. Adjusted to {maxSkillCooldown}.");
			//}
			//else
			//{
			//	Logger.LogInfo($"[Harmony] SkillCool within boundaries. No clamping applied.");
			//}



			// Log final result
			Logger.LogInfo($"[Harmony] FINAL: SkillCool={__result.SkillCool}");


			// Log after modification
			Logger.LogInfo($"[Harmony] FINAL: Attack={__result.Attack}, Magic={__result.Magic}, Defence={__result.Defence}, " +
						   $"MaxHp={__result.MaxHp}, AttackSpeed={__result.AttackSpeed}, SkillCool={__result.SkillCool} \n");
		}

	}
}