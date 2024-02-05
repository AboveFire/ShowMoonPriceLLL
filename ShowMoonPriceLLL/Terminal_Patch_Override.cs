using HarmonyLib;
using LethalLevelLoader;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace ShowMoonPriceLLL
{
    public class Terminal_Patch_Override
    {
        private static Terminal _terminal;

        internal static Terminal Terminal
        {
            get
            {
                if ((Object)(object)_terminal != (Object)null)
                {
                    return _terminal;
                }

                _terminal = GameObject.Find("TerminalScript").GetComponent<Terminal>();
                if ((Object)(object)_terminal != (Object)null)
                {
                    return _terminal;
                }

                Debug.LogError((object)"LethaLib: Failed To Grab Terminal Reference!");
                return null;
            }
        }

        [HarmonyPatch(typeof(Terminal), "TextPostProcess")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        internal static void TextPostProcess_Prefix(ref string modifiedDisplayText)
        {
            if (modifiedDisplayText.Contains("Welcome to the exomoons catalogue"))
            {
                string modifiedString = "";
                foreach (var line in modifiedDisplayText.Split("\n")){
                    if(line.StartsWith("* "))
                    {
                        ExtendedLevel level = getLevelFromLine(line);
                        if(level is not null)
                        {
                            if (level.routePrice == 0)
                            {
                                //Vanilla planet
                                modifiedString += line + (level.selectableLevel.riskLevel + "     $").PadLeft(40 - line.Length) + getRoutePriceForVanillaPlanet(level.NumberlessPlanetName) + "\n";
                            }
                            else
                            {
                                //Modded Planet
                                modifiedString += line + (level.selectableLevel.riskLevel + "     $").PadLeft(40 - line.Length) + level.routePrice + "\n";
                            }
                        }
                        else
                        {
                            modifiedString += line + "\n";
                        }
                    }
                    else
                    {
                        modifiedString += line + "\n";
                    }
                }

                modifiedDisplayText = modifiedString.Substring(0, modifiedString.Length);
            }
        }

        private static ExtendedLevel getLevelFromLine(string line)
        {
            ExtendedLevel extendedLevel = null;
            foreach (var level in Terminal.moonsCatalogueList.ToList())
            {
                if(SelectableLevel_Patch.TryGetExtendedLevel(level, out var returnExtendedLevel))
                {
                    if(returnExtendedLevel.NumberlessPlanetName == getNameFromLine(line))
                    {
                        extendedLevel = returnExtendedLevel;
                    }
                }
            }
            return extendedLevel;
        }

        private static int getRoutePriceForVanillaPlanet(string planetName)
        {
            if (planetName == "Rend")
            {
                return 550;
            }
            else if (planetName == "Dine")
            {
                return 600;
            }
            else if (planetName == "Titan")
            {
                return 700;
            }
            return 0;
        }

        private static string getNameFromLine(string line)
        {
            string name = line.Substring(2);
            name = name.Split('(')[0].Trim();

            return name;
        }
    }
}
