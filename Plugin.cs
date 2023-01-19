using BepInEx;
using HarmonyLib;
using TT;
using TT.UI;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace BetterUITenderfootTactics;

[BepInPlugin("BetterUITenderfootTactics", "Better Combat UI", "0.0.1")]
public class Plugin : BaseUnityPlugin {
    private void Awake() {
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Harmony.CreateAndPatchAll(typeof(Plugin));
    }
    
    [HarmonyPatch(typeof(CombatNumberKeyShortcuts), "Applicable", MethodType.Getter)]
    [HarmonyPrefix]
    static bool newApplicable(ref bool __result) {
        __result = Combat.active
                   && !CombatDeployment.active
                   && Combat.PlayerTurn
                   && (Combat.state == Combat.State.Surveying
                       || Combat.state == Combat.State.Acting);
        return false;
    }

    [HarmonyPatch(typeof(CombatNumberKeyShortcuts), "refresh")]
    [HarmonyPostfix]
    static void postRefresh(CombatNumberKeyShortcuts __instance) {
        Traverse.Create(__instance).Property("inActSubmenu").SetValue(true);
        if (!(Combat.activeUnit is null)) {
            ActionTooltipUI.SetNumbers(__instance, Combat.activeUnit.knownJobActions);
        }
    }

    [HarmonyPatch(typeof(CombatNumberKeyShortcuts), "cursor")]
    [HarmonyPrefix]
    static bool newCursor(Input.CursorState cursorState) {
        return false;
    }
}
