using BepInEx;
using HarmonyLib;

namespace ShowMoonPriceLLL
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Config MoonPriceConfig { get; internal set; }

        private void Awake()
        {
            MoonPriceConfig = new(base.Config);
            var harmony = new Harmony("ShowMoonPriceLLL");
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            harmony.PatchAll(typeof(Terminal_Patch_Override));
        }
    }
}
