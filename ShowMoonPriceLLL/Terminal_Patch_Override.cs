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
            bool showRisk = ShowMoonPriceLLLConfig.showRisk.Value;
            bool showPrice = ShowMoonPriceLLLConfig.showPrice.Value;
            if (modifiedDisplayText.Contains("Welcome to the exomoons catalogue"))
            {
                string modifiedString = "";
                foreach (var line in modifiedDisplayText.Split("\n")){
                    if(line.StartsWith("* "))
                    {
                        ExtendedLevel level = getLevelFromLine(line);
                        modifiedString += getFormattedLine(line, level, showRisk, showPrice) + "\n";
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
                extendedLevel = LevelManager.GetExtendedLevel(level);
                if (extendedLevel is not null && extendedLevel.NumberlessPlanetName is not null)
                {
                    if (System.String.Equals(extendedLevel.NumberlessPlanetName.Trim(), getNameFromLine(line)))
                    {
                        return extendedLevel;
                    }
                }
            }
            return null;
        }

        private static string getNameFromLine(string line)
        {
            string name = line.Substring(2);
            name = name.Split('(')[0].Trim();

            //Trim to be compatible with WeatherTweaks
            name = name.Split('[')[0].Trim();

            return name;
        }

        private static string getFormattedLine(string pLine, ExtendedLevel level, bool pShowRisk, bool pShowPrice )
        {
            if (level is not null && level.SelectableLevel is not null)
            {
                string formattedLine = pLine;
                if (pShowRisk && pLine.Length < 33 && level.SelectableLevel.riskLevel is not null)
                {
                    formattedLine += level.SelectableLevel.riskLevel.PadLeft(33 - formattedLine.Length + level.SelectableLevel.riskLevel.Length);
                }
                if (pShowPrice && pLine.Length < 44 && level.SelectableLevel.riskLevel is not null)
                {
                    formattedLine += ("$" + level.RoutePrice).PadLeft(44 - formattedLine.Length);
                }
                return formattedLine;
            }
            return pLine;
        }
    }
}
