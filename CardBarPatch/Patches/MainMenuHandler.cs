using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;

namespace CardBarPatch.Patches
{
    [HarmonyPatch(typeof(MainMenuHandler), "Awake")]
    public class MainMenuHandlerAwakePatch
    {
        static void Prefix(MainMenuHandler __instance)
        {
            __instance.ExecuteAfterFrames(9, () => {
                CardBarHandler.instance?.ResetCardBards();
            });
        }
    }
}
