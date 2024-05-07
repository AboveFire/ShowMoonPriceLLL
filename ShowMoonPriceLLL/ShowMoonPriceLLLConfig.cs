using BepInEx.Configuration;

namespace ShowMoonPriceLLL
{
    public class ShowMoonPriceLLLConfig
    {
        public static ConfigEntry<bool> showRisk;
        public static ConfigEntry<bool> showPrice;

        public ShowMoonPriceLLLConfig(ConfigFile cfg)
        {
            showRisk = cfg.Bind(
                    "General.Toggles",
                    "ShowRisk",
                    true,
                    "To show the risk factor of the planet in moons menu"
            );

            showPrice = cfg.Bind(
                    "General.Toggles",
                    "ShowPrice",
                    true,
                    "To show the price of the planet in moons menu"
            );
        }
    }
}
