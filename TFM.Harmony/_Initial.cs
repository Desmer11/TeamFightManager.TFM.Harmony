using System;

using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using PatchIntensityHarmony;
using PatchNumberHarmony;
using PatchHarmonyChange;
using System;

namespace Initial
{
    [BepInPlugin("com.System.patch", "System Patch", "1.0.0")]
    public class InitialPatch : BaseUnityPlugin
    {
        private static ManualLogSource Logger;

        private void Awake()
        {
			// Initialize the logger
			Logger = BepInEx.Logging.Logger.CreateLogSource("System Patch");
            Logger.LogInfo("Initializing System Patch...");

			// Initialize Harmony and apply patches
			try
			{
                Harmony.DEBUG = true; // Set debug mode
                var harmony = new Harmony("com.system.patch");

                Logger.LogInfo("Applying Harmony patches...");
                InitializePatches(harmony);

                Logger.LogInfo("AWAKE: System Patch initialized successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"AWAKE: Initialization failed: {ex.Message}");
            }

            Logger.LogInfo("\n");
        }

        private void InitializePatches(Harmony harmony)
        {
			// Apply all Harmony patches
			try
			{
				Logger.LogInfo("Starting harmony.PatchAll()");
				harmony.PatchAll();
				Logger.LogInfo("Finished harmony.PatchAll()");

				var targetMethod = AccessTools.Method(typeof(ChampionPatch), "Patch");
				var prefixMethod = AccessTools.Method(typeof(PatchNumberPatch.ChampionPatch_Patch_Prefix), "Postfix");

				if (targetMethod == null)
				{
					Logger.LogError("Target method ChampionPatch.Patch not found");
				}
				else
				{
					Logger.LogInfo("Target method found, applying manual patch...");
					harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefixMethod));
					Logger.LogInfo("Manual patch applied");
				}
			}
			catch (Exception ex)
			{
				Logger.LogError($"Error applying Harmony patches: {ex}");
			}
			// Initialize individual patches with the logger
			try
			{
                PatchIntensityPatch.InitializeLogger(Logger);
                Logger.LogInfo("Logger initialized for PatchIntensityPatch.");

				PatchNumberPatch.InitializeLogger(Logger);
				Logger.LogInfo("Logger initialized for PatchNumberPatch.");

				ChampionPatch_Change_Patch.InitializeLogger(Logger);
				Logger.LogInfo("Logger initialized for PatchHarmonyChange.");


			}
            catch (Exception ex)
            {
                Logger.LogError($"Error during patch initialization: {ex.Message}");
            }
        }

        private void OnDestroy()
        {
            // Release logger resource on plugin unload
            BepInEx.Logging.Logger.Sources.Remove(Logger);
            Logger.LogInfo("System Patch disposed.");
        }
    }
}
