using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HuntShowdownTwitchWikiAPI.src
{
  public class Parser
  {
    private readonly string ammoPattern = string.Join("|", new[]
    {
    "Subsonic",
    "Flechette",
    "Penny Shot",
    "Slug",
    "Incendiary",
    "Spitzer",
    "Dragon Breath",
    "Harpoon",
    "Steel Ball",
    "Waxed Frag",
    "High Velocity",
    "Dumdum",
    "Poison",
    "Poison Arrows",
    "Explosive",
    "Shot Bolt",
    "Chaos Bolt",
    "Choke Bolt",
    "Starshell",
    "Concertina Arrows",
    "Frag Arrows",
    "Shredder",
    "DD",
    "DB",
    "HV",
    "FMJ",
    "INC",
    "POI",
    "SPI",
    "EX",
    "FL",
    "SL",
    "PS",
    "SUB",
    "fire"
});

    private readonly string augmentPattern = string.Join("|", new[]
    {
    "Alamo",
    "Aperture",
    "Avtomat",
    "Bayonet",
    "Brawler",
    "Bullseye",
    "(?<!^)Carbine",
    "Chain Pistol",
    "Claw",
    "Cyclone",
    "Deadeye",
    "Extended",
    "Hatchet",
    "Ironside",
    "Mace",
    "Marksman",
    "Match",
    "Pistol",
    "Pointman",
    "Precision",
    "Riposte",
    "Sharpeye",
    "Shorty",
    "Silencer",
    "Sniper",
    "Spitfire",
    "Striker",
    "Swift",
    "Talon",
    "Trauma",
    "Trueshot"
});

    private readonly Regex regexWeaponClassificationPattern;

    private static readonly Dictionary<string, string> ammoTranslationTable =
    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
    { "Subsonic",   "Subsonic" },
    { "Flechette",  "Flechette" },
    { "Penny",      "Penny Shot" },
    { "Slug",       "Slug" },
    { "Incendiary", "Incendiary" },
    { "Spitzer",    "Spitzer" },
    { "Dragon",     "Dragon Breath" },
    { "Harpoon",    "Harpoon" },
    { "SteelBall",  "Steel Ball" },
    { "WaxedFrag",  "Waxed Frag" },
    { "High",       "High Velocity" },
    { "Dumdum",     "Dumdum" },
    { "Poison",     "Poison" },
    { "Explosive",  "Explosive" },
    { "Shot",       "Shot Bolt" },
    { "Chaos",      "Chaos Bolt" },
    { "Choke",      "Choke Bolt" },
    { "Starshell",  "Starshell" },
    { "Concertina", "Concertina Arrows" },
    { "Frag",       "Frag Arrows" },
    { "Shredder",   "Shredder" },
    { "fmj", "FMJ" },
    { "DD",   "Dumdum" },
    { "DB", "Dragon Breath" },
    { "INC",  "Incendiary" },
    { "POI",  "Poison" },
    { "SPI",  "Spitzer" },
    { "EX",   "Explosive" },
    { "FL",   "Flechette" },
    { "SL",   "Slug" },
    { "PS",   "Penny Shot" },
    { "SUB",  "Subsonic" },
    { "fire", "Incendiary" }
    };

    private enum WeaponComponentParts
    {
      Weapon,
      Augment,
      Ammo
    }

    WeaponRepository Repo;


    public Parser(WeaponRepository repo)
    {
      regexWeaponClassificationPattern =
        new Regex($@"\b(?<ammo>{ammoPattern})\b|\b(?<augment>{augmentPattern})\b"
        , RegexOptions.IgnoreCase | RegexOptions.Compiled);

      this.Repo = repo;
    }


    private string Capitalize(string s)
    {
      if (string.IsNullOrWhiteSpace(s))
        return s;

      var sSplit = s.Split(' ');

      string reformedstring = "";

      for (int i = 0; i < sSplit.Length; i++)
      {
        reformedstring += char.ToUpper(sSplit[i][0]) + sSplit[i].Substring(1);
      }

      return reformedstring.Trim();
    }


    private List<string>? WeaponAmmoTransformer(List<string> weaponComponents)
    {
      if (!ammoTranslationTable.TryGetValue(weaponComponents[(int)WeaponComponentParts.Ammo], out string realAmmoName))
      {
        return null;
      }
      else
      {
        weaponComponents[(int)WeaponComponentParts.Ammo] = realAmmoName;
      }
      return weaponComponents;
    }


    private List<string> WeaponExtractor(string weaponName)
    {
      var matches = regexWeaponClassificationPattern.Matches(weaponName);

      var augments = matches
          .Cast<Match>()
          .Where(m => m.Groups["augment"].Success)
          .Select(m => m.Value)
          .ToList();

      var ammo = matches
          .Cast<Match>()
          .Where(m => m.Groups["ammo"].Success)
          .Select(m => m.Value)
          .LastOrDefault() ?? string.Empty;

      string weapon = regexWeaponClassificationPattern.Replace(weaponName, "");
      weapon = Regex.Replace(weapon, @"\s+", " ").Trim();

      string augmentCombined = augments.Count > 0
          ? string.Join(" ", augments)
          : string.Empty;

      return new List<string>
      {
          Capitalize(weapon),
          Capitalize(augmentCombined),
          Capitalize(ammo)
      };

    }


    private WeaponData? WeaponFinder(string weaponName)
    {
      WeaponData Weapon = Repo.GetWeapon(weaponName);

      if (Weapon == null)
      {
        var weaponComponents = WeaponExtractor(weaponName);

        List<string> weaponComponentsTransformered = WeaponAmmoTransformer(weaponComponents);

        Weapon = Repo.GetWeaponByKey(weaponComponents);

        if (Weapon == null)
        {
          return null;
        }

      }
      return Weapon;

    }


    public string QueueParser(
      string weaponName, 
      Func<WeaponProcessee, string> WeaponReturnStringFormater,
      string? weaponAugments = null)
    {


      WeaponData Weapon = WeaponFinder(weaponName);
      
      if (Weapon == null)
      {
        return $"{weaponName} Not found";
      }

      return WeaponReturnStringFormater(new WeaponProcessee(Weapon, weaponAugments));


    }
  }
}
