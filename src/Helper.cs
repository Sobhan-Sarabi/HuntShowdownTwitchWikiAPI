using System;
using System.Collections.Generic;
using System.IO;

namespace HuntShowdownTwitchWikiAPI.src
{
  public class Helper
  {
    private IEnumerable<string> m_WeponStatsCSVFile;
    private readonly List<WeaponData> m_WeponDataList = new List<WeaponData>();

    public void CsvOpener(string csvPath)
    {
      m_WeponStatsCSVFile = File.ReadLines(csvPath);
    }

    public List<WeaponData> CsvLoadToWeapn()
    {
      if (m_WeponStatsCSVFile == null)
        throw new InvalidOperationException("CSV file not opened. Call CsvOpener() first.");

      bool isFirstLine = true;

      foreach (var weapon in m_WeponStatsCSVFile)
      {
        // Skip header
        if (isFirstLine)
        {
          isFirstLine = false;
          continue;
        }

        if (string.IsNullOrWhiteSpace(weapon))
          continue;

        var parts = weapon.Split(',');

        if (parts.Length < 18)
        {
          Console.WriteLine("Skipping malformed line: " + weapon);
          continue;
        }

        WeaponData newWeapon = new WeaponData
        {
          Weapon = parts[0],
          Size = parts[1],
          SpecialAmmoType = parts[2],
          GeneralAmmoType = parts[3],
          Price = parts[4],
          Damage = parts[5],
          DropRange = parts[6],
          MuzzleVelocity = parts[7],
          VerticalRecoil = parts[8],
          RateOfFire = parts[9],
          CycleTime = parts[10],
          Spread = parts[11],
          Sway = parts[12],
          ReloadSpeed = parts[13],
          Loaded = parts[14],
          Extra = parts[15],
          AmmoType = parts[16],
          Note = parts[17]
        };

        m_WeponDataList.Add(newWeapon);
      }

      return m_WeponDataList;
    }
  }
}
