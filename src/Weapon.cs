using System.Text.RegularExpressions;

namespace HuntShowdownTwitchWikiAPI.src
{
  public class WeaponData
  {
    public string FullWeaponName { get; set; }
    public string WeaponAugment { get; set; }
    public string WeaponBase { get; set; }
    public string AmmoType { get; set; }
    public string Size { get; set; }
    public string SpecialType { get; set; }
    public string GeneralAmmoType { get; set; }
    public string Price { get; set; }
    public string Damage { get; set; }
    public string DropRange { get; set; }
    public string MuzzleVelocity { get; set; }
    public string VerticalRecoil { get; set; }
    public string RateOfFire { get; set; }
    public string CycleTime { get; set; }
    public string Spread { get; set; }
    public string Sway { get; set; }
    public string ReloadSpeed { get; set; }
    public string Loaded { get; set; }
    public string Extra { get; set; }
    public string Note { get; set; }
    public string MergedAmmo => MagazineMerger(this.Loaded, this.Extra);

    private enum GeneralAmmoTypes
    {
      Base,
      Alt
    }

    private readonly string attributePattern = string.Join("|", new[]
{
    "Special Ammo",
    "General Ammo",
    "Range",
    "Muzzle Velocity",
    "Velocity",
    "Vertical Recoil",
    "Recoil",
    "Rate Of Fire",
    "Fire rate",
    "FR",
    "Cycle Time",
    "Cycle",
    "CT",
    "Reload",
    "Ammo",
    "Weapon Name",
    "Augment",
    "Weapon Base",
    "Ammo Type",
    "Size",
    "Price",
    "Spread",
    "Sway",
    "Note",
    "Damage"
});

    private Dictionary<string, (string InternalName, string Value)> attributeTranslationTable;

    private readonly Regex regexAttributePattern;

    public WeaponData()
    {
      regexAttributePattern =
    new Regex($@"\b(?<field>{attributePattern})\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }


    private string MagazineMerger(string mag, string reserves)
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


    private List<string> AttributeExtractor(string attributes)
    {
      List<string> returnList = new();

      var matches = regexAttributePattern.Matches(attributes);

      foreach (Match match in matches)
      {
        returnList.Add(match.Value);
      }

      return returnList;
    }


    public Dictionary<string, string> GetWeaponAttribute(string attributes)
    {
      attributeTranslationTable =
        new Dictionary<string, (string InternalName, string Value)>(StringComparer.OrdinalIgnoreCase)
        {
        { "Special Ammo", ("Special Ammo", this.SpecialType) },
        { "General Ammo", ("General Ammo", this.GeneralAmmoType) },
        { "Range", ("Drop Range", this.DropRange) },
        { "Muzzle Velocity", ("Muzzle Velocity", this.MuzzleVelocity) },
        { "Velocity", ("Muzzle Velocity", this.MuzzleVelocity) },
        { "Vertical Recoil", ("Vertical Recoil", this.VerticalRecoil) },
        { "Recoil", ("Vertical Recoil", this.VerticalRecoil) },
        { "Rate Of Fire", ("Rate Of Fire", this.RateOfFire) },
        { "Fire rate", ("Rate Of Fire", this.RateOfFire) },
        { "FR", ("Rate Of Fire", this.RateOfFire) },
        { "Cycle Time", ("Cycle Time", this.CycleTime) },
        { "Cycle", ("Cycle Time", this.CycleTime) },
        { "CT", ("Cycle Time", this.CycleTime) },
        { "Reload", ("Reload Speed", this.ReloadSpeed) },
        { "Ammo", ("Merged Ammo", this.MergedAmmo) },
        { "Weapon Name", ("Full Weapon Name", this.FullWeaponName) },
        { "Augment", ("Weapon Augment", this.WeaponAugment) },
        { "Weapon Base", ("Weapon Base", this.WeaponBase) },
        { "Ammo Type", ("Ammo Type", this.AmmoType) },
        { "Size", ("Size", this.Size) },
        { "Price", ("Price", this.Price) },
        { "Spread", ("Spread", this.Spread) },
        { "Sway", ("Sway", this.Sway) },
        { "Note", ("Note", this.Note) },
        { "Damage", ("Damage", this.Damage) }
        };

      Dictionary<string, string> returnDirct = new();

      List<string> attributesExtracted = AttributeExtractor(attributes);

      for (int i = 0; i < attributesExtracted.Count(); i++)
      {
        attributeTranslationTable.TryGetValue(
          attributesExtracted[i], out (string InternalName, string Value) realattributesName);
        if (realattributesName != (null, null))
        {
          returnDirct[realattributesName.InternalName] = realattributesName.Value;
        }
      }

      return returnDirct;
    }



    public override string ToString()
    {
      return
$@"Weapon: {FullWeaponName}
WeaponAugment: {WeaponAugment}
WeaponBase: {WeaponBase}
AmmoType: {AmmoType}
Size: {Size}
SpecialType: {SpecialType}
GeneralAmmoType: {GeneralAmmoType}
Price: {Price}
Damage: {Damage}
DropRange: {DropRange}
MuzzleVelocity: {MuzzleVelocity}
VerticalRecoil: {VerticalRecoil}
RateOfFire: {RateOfFire}
CycleTime: {CycleTime}
Spread: {Spread}
Sway: {Sway}
ReloadSpeed: {ReloadSpeed}
Loaded: {Loaded}
Extra: {Extra}
Note: {Note}";
    }
  }
}
