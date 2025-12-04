using System.Linq;

namespace HuntShowdownTwitchWikiAPI.src
{
  public static class ReturnString
  {
    private static readonly Dictionary<string, string> attributeTranslationTable =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
        { "Special Ammo", "SpecialType" },
        { "General Ammo", "GeneralAmmoType" },
        { "Range", "DropRange" },
        { "Velocity", "MuzzleVelocity" },
        { "Recoil", "VerticalRecoil" },
        { "Fire rate", "RateOfFire" },
        { "FR", "RateOfFire" },
        { "Cycle", "CycleTime" },
        { "CT", "CycleTime" },
        { "Reload", "ReloadSpeed" },
        { "Ammo", "LoadedExtra" },
        { "Weapon Name", "FullWeaponName" },
        { "Augment", "WeaponAugment" },
        { "Weapon Base", "WeaponBase" },
        { "Ammo Type", "AmmoType" },
        { "Size", "Size" },
        { "Price", "Price" },
        { "Spread", "Spread" },
        { "Sway", "Sway" },
        { "Note", "Note" },
        { "Damage", "Damage" }
        };


    public static string NormalReturn(WeaponProcessee WeaponParameters)
    {
      WeaponData Weapon = WeaponParameters.Weapon;

      return
        $"|Price:{Weapon.Price.Replace(" ", "")}| " +
        $"|Damage:{Weapon.Damage.Replace(" ", "")}| " +
        $"|Velocity:{Weapon.MuzzleVelocity.Replace(" ", "")}| " +
        $"|Spread:{Weapon.Spread.Replace(" ", "")}| " +
        $"|Sway:{Weapon.Sway.Replace(" ", "")}| " +
        $"|Range:{Weapon.DropRange.Replace(" ", "")}| " +
        $"|Magazine:{Weapon.MergedAmmo}|"; ;
    }


    public static string ExtendedReturn(WeaponProcessee WeaponParameters)
    {
      WeaponData Weapon = WeaponParameters.Weapon;

      return
        $"|Size:{Weapon.Size}| " +
        $"|Ammo Type:{Weapon.GeneralAmmoType.Replace(" ", "")}| " +
        (string.IsNullOrWhiteSpace(Weapon.AmmoType)
          ? ""
          : $"|Special Ammo Type:{Weapon.AmmoType.Replace(" ", "")}| ") +
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
        $"|Magazine:{Weapon.MergedAmmo}| " +
        (string.IsNullOrWhiteSpace(Weapon.Note)
          ? ""
          : $"|Note:{Weapon.Note}|");
    }


    public static string CustomReturn(WeaponProcessee WeaponPrammters)
    {
      if (WeaponPrammters.AttributeConfigurations == null)
      {
        return "No Attributes given";
      }

      WeaponData weapon = WeaponPrammters.Weapon;

      Dictionary<string, string> attributes = 
        weapon.GetWeaponAttribute(WeaponPrammters.AttributeConfigurations);


      if (attributes.Count <= 0)
      {
        return $"Nothing found";
      }

      string returnString = "";

      foreach (KeyValuePair<string,string> attribute in attributes)
      {
        returnString += $"|{attribute.Key}:{attribute.Value}| ";
      }


      return returnString;
    }
  }
}
