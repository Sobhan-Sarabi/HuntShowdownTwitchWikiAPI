namespace HuntShowdownTwitchWikiAPI.src
{
  public class WeaponData
  {
    public string Weapon { get; set; }
    public string Size { get; set; }
    public string SpecialAmmoType { get; set; }
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
    public string AmmoType { get; set; }
    public string Note { get; set; }

    public override string ToString()
    {
      return
$@"Weapon: {Weapon}
Size: {Size}
SpecialAmmoType: {SpecialAmmoType}
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
AmmoType: {AmmoType}
Note: {Note}";
    }
  }
}
