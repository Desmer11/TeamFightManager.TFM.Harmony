﻿using System;

using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
using System.Reflection;

namespace PatchIntensityHarmony
{
	[HarmonyPatch]
	public static class PatchIntensityPatch
	{
		private static ManualLogSource Logger;

		public static void InitializeLogger(ManualLogSource logger)
		{
			Logger = logger;
		}

		// Patch AttackIntensity method
		[HarmonyPrefix]
		[HarmonyPatch(typeof(PatchIntensityExt), nameof(PatchIntensityExt.AttackIntensity))]
		public static bool PatchAttackIntensity(PatchIntensity intensity, ref int __result)
		{
			if (Logger == null)
			{
				Logger = BepInEx.Logging.Logger.CreateLogSource("PatchIntensityPatch");
			}

			//Logger.LogInfo($"Patching AttackIntensity for {intensity}");
			switch (intensity)
			{
				case PatchIntensity.Weak:
					__result = 100; // Increased from 80
					break;
				case PatchIntensity.Normal:
					__result = 150; // Increased from 100
					break;
				case PatchIntensity.Strong:
					__result = 200; // Increased from 130

					break;
				default:
					__result = 0;
					break;
			}

			//Logger.LogInfo($"New AttackIntensity: {__result}");
			return false; // Skip original method
		}

		// Patch DefenceIntensity method
		[HarmonyPrefix]
		[HarmonyPatch(typeof(PatchIntensityExt), nameof(PatchIntensityExt.DefenceIntensity))]
		public static bool PatchDefenceIntensity(PatchIntensity intensity, ref int __result)
		{

			//Logger.LogInfo($"Patching DefenceIntensity for {intensity}");
			switch (intensity)
			{
				case PatchIntensity.Weak:
					__result = 10; // Increased from 8
					break;
				case PatchIntensity.Normal:
					__result = 15; // Increased from 11
					break;
				case PatchIntensity.Strong:
					__result = 20; // Increased from 16

					break;
				default:
					__result = 0;
					break;
			}

			//Logger.LogInfo($"New DefenceIntensity: {__result}");

			return false;
		}

		// Patch HpIntensity method
		[HarmonyPrefix]
		[HarmonyPatch(typeof(PatchIntensityExt), nameof(PatchIntensityExt.HpIntensity))]
		public static bool PatchHpIntensity(PatchIntensity intensity, ref int __result)
		{

			//Logger.LogInfo($"Patching HpIntensity for {intensity}");
			switch (intensity)
			{
				case PatchIntensity.Weak:
					__result = 400; // Increased from 300
					break;
				case PatchIntensity.Normal:
					__result = 500; // Increased from 400
					break;
				case PatchIntensity.Strong:
					__result = 600; // Increased from 550

					break;
				default:
					__result = 0;
					break;
			}

			//Logger.LogInfo($"New HpIntensity: {__result}");

			return false;
		}

		// Patch SkillCoolIntensity method
		[HarmonyPrefix]
		[HarmonyPatch(typeof(PatchIntensityExt), nameof(PatchIntensityExt.SkillCoolIntensity))]
		public static bool PatchSkillCoolIntensity(PatchIntensity intensity, ref int __result)
		{

			//Logger.LogInfo($"Patching SkillCoolIntensity for {intensity}");
			switch (intensity)
			{
				case PatchIntensity.Weak:
					__result = 15; // Increased from 12
					break;
				case PatchIntensity.Normal:
					__result = 20; // Increased from 15
					break;
				case PatchIntensity.Strong:
					__result = 25; // Increased from 18

					break;
				default:
					__result = 0;
					break;
			}

			//Logger.LogInfo($"New SkillCoolIntensity: {__result}");

			return false;
		}

		// Patch AttackSpeedIntensity method
		[HarmonyPrefix]
		[HarmonyPatch(typeof(PatchIntensityExt), nameof(PatchIntensityExt.AttackSpeedIntensity))]
		public static bool PatchAttackSpeedIntensity(PatchIntensity intensity, ref int __result)
		{

			//Logger.LogInfo($"Patching AttackSpeedIntensity for {intensity}");
			switch (intensity)
			{
				case PatchIntensity.Weak:
					__result = 4; // Increased from 4
					break;
				case PatchIntensity.Normal:
					__result = 6; // Increased from 6
					break;
				case PatchIntensity.Strong:
					__result = 10; // Increased from 9

					break;
				default:
					__result = 0;
					break;
			}

			//Logger.LogInfo($"New AttackSpeedIntensity: {__result}");

		}
	}
}
