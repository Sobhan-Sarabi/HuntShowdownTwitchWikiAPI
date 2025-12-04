namespace HuntShowdownTwitchWikiAPI.src
{
  public struct WeaponProcessee
  {
    public WeaponData Weapon { get; private set; }

    public string AttributeConfigurations { get; private set; }

    public WeaponProcessee(WeaponData Weapon, string? AttributeConfigurations) 
    {
      this.Weapon = Weapon;

      this.AttributeConfigurations = AttributeConfigurations;
    }
  }
}
