using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace HuntShowdownTwitchWikiAPI.src
{
  static public class Parser
  {
    private static readonly Dictionary<string, string> weaponTranslationTable =
    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
      { "Carbine", "1865 Carbine" },
      { "Auto 5", "Auto-5"},
      { "Crown and king", "Auto-5" },
      { "Berthier", "Berthier 1892" },
      { "Bornheim", "Bornheim No. 3" },
      { "centenial", "Centennial" },
      { "Dolch", "Dolch 96" },
      { "Frontier", "Frontier 73C" },
      { "Homestead", "Homestead 78" },
      { "Bow", "Hunting Bow" },
      { "Infantry", "Infantry 73L" },
      { "Lemat", "LeMat" },
      { "Lebel", "Lebel 1886" },
      { "Mako", "Mako 1895" },
      { "Martini", "Martini-Henry" },
      { "Maynard", "Maynard Sniper" },
      { "Mosin", "Mosin-Nagant" },
      { "Nagant", "Nagant M1895" },
      { "Nitro", "Nitro Express" },
      { "Ranger", "Ranger 73" },
      { "Rival", "Rival 78" },
      { "Romero", "Romero 77" },
      { "Specter", "Specter 1882" },
      { "Springfield", "Springfield 1866" },
      { "Vandal", "Vandal 73C" },
      { "Veterli", "Vetterli 71" },
      { "Vetterli", "Vetterli 71" },
    };

    private enum WeaponParts
    {
      name,
      augment
    }

    private enum QueueParametersOptions
    {
      noPram = 0,
      extended = 0,
      custom = 0,
    }

    private enum GeneralAmmoTypes
    {
      Base,
      Alt
    }

    private static readonly HashSet<string> ammoTranslationList =
      new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
    "Subsonic",
    "Flechette",
    "Penny",
    "Slug",
    "Incendiary",
    "Spitzer",
    "Dragon",
    "Harpoon",
    "Steel",
    "Waxed",
    "High",
    "Dumdum",
    "Poison",
    "Explosive",
    "Shot",
    "Chaos",
    "Choke",
    "Starshell",
    "Concertina",
    "Frag",
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
    };

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
    { "Steel",      "Steel Ball" },
    { "Waxed",      "Waxed Frag" },
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


    static private string WeaponTranslator(string weaponToBeTranslated)
    {
      string translatedWeaponResult = "";

      translatedWeaponResult = weaponToBeTranslated;

      string[] translatedWeaponResultSplit = translatedWeaponResult.Split(' ');

      foreach (var pair in weaponTranslationTable)
      {
        if (pair.Key.Equals(translatedWeaponResultSplit[(int)WeaponParts.name], StringComparison.OrdinalIgnoreCase))
        {
          translatedWeaponResultSplit[(int)WeaponParts.name] = translatedWeaponResultSplit[(int)WeaponParts.name].Replace(
          pair.Key,
          pair.Value,
          StringComparison.OrdinalIgnoreCase);
        }
      }

      if (translatedWeaponResult == "")
      {
        return weaponToBeTranslated;
      }

      for (int i = 0; i < translatedWeaponResultSplit.Length; i++)
      {
        translatedWeaponResultSplit[i] = char.ToUpper(
          translatedWeaponResultSplit[i][0]) +
          translatedWeaponResultSplit[i].Substring(1);
      }

      //Console.WriteLine(string.Join(' ', translatedWeaponResultSplit));

      return string.Join(' ', translatedWeaponResultSplit);
    }

    static private string AmmoTranslator(string ammoToBeTranslated)
    {
      foreach (var pair in ammoTranslationTable)
      {
        if (pair.Key.Equals(ammoToBeTranslated, StringComparison.OrdinalIgnoreCase))
        {
          ammoToBeTranslated = ammoToBeTranslated.Replace(
          pair.Key,
          pair.Value,
          StringComparison.OrdinalIgnoreCase);
        }
      }

      //Console.WriteLine(ammoToBeTranslated);

      return ammoToBeTranslated;
    }


    static private WeaponData? WeaponFinder(WeaponRepository repo, string weaponName)
    {
      var weapon = repo.GetWeapon(weaponName);

      if (weapon != null)
      {
        return weapon;
      }
      else
      {
        string[] weaponNameParts = weaponName.Split(' ');


        for (int i = 0; i < weaponNameParts.Length; i++)
        {
          if (!ammoTranslationList.Contains(weaponNameParts[i]))
          {
            continue;
          }
          else
          {
            string[] weaponComponents = new string[i];
            Array.Copy(weaponNameParts, 0, weaponComponents, 0, i);

            weapon = repo.GetWeapon(
              WeaponTranslator(string.Join(' ', weaponComponents)) + ' ' +
              AmmoTranslator(weaponNameParts[i]));


            //Console.WriteLine(weapon.ToString());

            return weapon;
          }
        }

        weapon = repo.GetWeapon(WeaponTranslator(weaponName));
        return weapon;
      }
    }


    static private string MagazineMerger(string mag, string reserves)
    {
      if (!mag.Contains("/"))
      {
        return mag + "/" + reserves;
      }
      else
      {
        string[] magSplit = mag.Split("/");
        string[] reservesSplit = reserves.Split("/");

        return
          $"{magSplit[(int)GeneralAmmoTypes.Base].Trim()}/" +
          $"{reservesSplit[(int)GeneralAmmoTypes.Base].Trim()} Alt:" +
          $"{magSplit[(int)GeneralAmmoTypes.Alt].Trim()}/" +
          $"{reservesSplit[(int)GeneralAmmoTypes.Alt].Trim()}";
      }
    }


    static public string QueueParser(WeaponRepository repo, string weaponName, string[] parameters)
    {
      WeaponData Weapon = WeaponFinder(repo, weaponName);

      if (Weapon == null)
      {
        return $"{weaponName} Not Found.";
      }

      if (parameters.Length == (int)QueueParametersOptions.noPram)
      {
        return
          $"|Price:{Weapon.Price.Replace(" ", "")}| " +
          $"|Damage:{Weapon.Damage.Replace(" ", "")}| " +
          $"|Velocity:{Weapon.MuzzleVelocity.Replace(" ", "")}| " +
          $"|Spread:{Weapon.Spread.Replace(" ", "")}| " +
          $"|Sway:{Weapon.Sway.Replace(" ", "")}| " +
          $"|Range:{Weapon.DropRange.Replace(" ", "")}| " +
          $"|Magazine:{MagazineMerger(Weapon.Loaded, Weapon.Extra)}|";

      }

      else if (string.Equals("Extended", parameters[(int)QueueParametersOptions.extended], StringComparison.OrdinalIgnoreCase))
      {
        return
          $"|Size:{Weapon.Size}| " +
          $"|Ammo Type:{Weapon.GeneralAmmoType.Replace(" ", "")}| " +
          (string.IsNullOrWhiteSpace(Weapon.SpecialAmmoType)
            ? ""
            : $"|Special Ammo Type:{Weapon.SpecialAmmoType.Replace(" ", "")}| ") +
          $"|Price:{Weapon.Price.Replace(" ", "")}| " +
          $"|Damage:{Weapon.Damage.Replace(" ", "")}| " +
          $"|Range:{Weapon.DropRange.Replace(" ", "")}| " +
          $"|Velocity:{Weapon.MuzzleVelocity.Replace(" ", "")}| " +
          $"|Recoil:{Weapon.VerticalRecoil.Replace(" ", "")}| " +
          $"|Fire Rate:{Weapon.RateOfFire.Replace(" ", "")}| " +
          $"|Cycle Time:{Weapon.CycleTime.Replace(" ", "")}| " +
          $"|Spread:{Weapon.Spread.Replace(" ", "")}| " +
          $"|Sway:{Weapon.Sway.Replace(" ", "")}| " +
          $"|Reload:{Weapon.ReloadSpeed.Replace(" ", "")}| " +
          $"|Magazine:{MagazineMerger(Weapon.Loaded, Weapon.Extra)}| " +
          (string.IsNullOrWhiteSpace(Weapon.Note)
            ? ""
            : $"|Note:{Weapon.Note}|");
      }

      return "Something has gone wrong";
    }
  }
}
