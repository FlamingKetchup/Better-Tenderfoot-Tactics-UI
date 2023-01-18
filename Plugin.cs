using BepInEx;
using HarmonyLib;
using TT;
using System.Collections.Generic;

namespace BetterUITenderfootTactics;

[BepInPlugin("BetterUITenderfootTactics", "Better Combat UI", "0.0.1")]
public class Plugin : BaseUnityPlugin {
    private void Awake() {
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Harmony.CreateAndPatchAll(typeof(Plugin));
    }
    
    [HarmonyPatch(typeof(CombatNumberKeyShortcuts), "numberKey")]
    [HarmonyPrefix]
    static bool newNumberKey(Input.ButtonState buttonState,
                             int key, CombatNumberKeyShortcuts __instance) {
        var Log = BepInEx.Logging.Logger.CreateLogSource("Log");
        if (buttonState.pressedThisFrame) {
            Log.LogDebug($"{buttonState} pressed this frame");
            
            key--;
            if (key < 0) {
                Log.LogDebug($"Before: {key}");
                key += 10;
                Log.LogDebug($"After: {key}");
            }
            ActionMenuOption actionMenuOption = null;
            List<ActAction> actions = Traverse.Create(__instance)
                                              .Field("actions")
                                              .GetValue() as List<ActAction>;
            if (key >= 0 && key < actions.Count) {
                actionMenuOption = actions[key];
            }
            if (actionMenuOption && Combat.isActionAvailable(actionMenuOption)) {
                if (ActionMenu.onSelectionMade != null) {
                    ActionMenu.onSelectionMade(actionMenuOption);
                }
                actionMenuOption.activateMenuOption();
            }
        }
        return false;
    }
}
