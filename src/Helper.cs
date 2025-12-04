using System;
using System.Collections.Generic;
using System.IO;

namespace HuntShowdownTwitchWikiAPI.src
{
  public class Helper
  {
    private IEnumerable<string> m_WeponStatsCSVFile;
    private readonly List<WeaponData> m_WeponDataList = new List<WeaponData>();

    private IEnumerable<string> _traitStatsCsvFile;

    public void CsvOpener(string csvPath)
    {
      m_WeponStatsCSVFile = File.ReadLines(csvPath);
    }

    public void TraitsCsvOpener(string csvPath)
    {
      _traitStatsCsvFile = File.ReadLines(csvPath);
    }

    public List<WeaponData> CsvLoadToWeapn()
    {
      if (m_WeponStatsCSVFile == null)
        throw new InvalidOperationException("CSV file not opened. Call CsvOpener() first.");

      bool isFirstLine = true;

      foreach (var line in m_WeponStatsCSVFile)
      {
        // Skip header
        if (isFirstLine)
        {
          isFirstLine = false;
          continue;
        }

        if (string.IsNullOrWhiteSpace(line))
          continue;

        var parts = line.Split(',');

        // NOTE: you index parts[0..19], so you actually expect at least 20 columns here.
        if (parts.Length < 20)
        {
          Console.WriteLine("Skipping malformed line: " + line);
          continue;
        }

        WeaponData newWeapon = new WeaponData
        {
          FullWeaponName = parts[0].Trim(),
          WeaponAugment = parts[1].Trim(),
          WeaponBase = parts[2].Trim(),
          AmmoType = parts[3].Trim(),
          Size = parts[4].Trim(),
          SpecialType = parts[5].Trim(),
          GeneralAmmoType = parts[6].Trim(),
          Price = parts[7].Trim(),
          Damage = parts[8].Trim(),
          DropRange = parts[9].Trim(),
          MuzzleVelocity = parts[10].Trim(),
          VerticalRecoil = parts[11].Trim(),
          RateOfFire = parts[12].Trim(),
          CycleTime = parts[13].Trim(),
          Spread = parts[14].Trim(),
          Sway = parts[15].Trim(),
          ReloadSpeed = parts[16].Trim(),
          Loaded = parts[17].Trim(),
          Extra = parts[18].Trim(),
          Note = parts[19].Trim()
        };

        m_WeponDataList.Add(newWeapon);
      }

      return m_WeponDataList;
    }

    public List<TraitData> CsvLoadToTraits()
    {
      if (_traitStatsCsvFile == null)
        throw new InvalidOperationException("Traits CSV file not opened. Call TraitsCsvOpener() first.");

      var traits = new List<TraitData>();
      bool isFirstLine = true;

      foreach (var line in _traitStatsCsvFile)
      {
        if (isFirstLine)
        {
          isFirstLine = false;
          continue;
        }

        if (string.IsNullOrWhiteSpace(line))
          continue;

        var parts = SplitCsvLine(line);
        if (parts.Count < 5)
        {
          Console.WriteLine("Skipping malformed trait line: " + line);
          continue;
        }

        var trait = new TraitData
        {
          Name = parts[0].Trim(),
          Cost = parts[1].Trim(),
          Unlock = parts[2].Trim(),
          Type = parts[3].Trim(),
          Effect = parts[4].Trim()
        };

        traits.Add(trait);
      }

      return traits;
    }

    private static List<string> SplitCsvLine(string line)
    {
      var result = new List<string>();
      if (line == null)
        return result;

      var current = new System.Text.StringBuilder();
      bool inQuotes = false;

      for (int i = 0; i < line.Length; i++)
      {
        char c = line[i];

        if (c == '"')
        {
          if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
          {
            // Escaped quote
            current.Append('"');
            i++; // Skip the next quote
          }
          else
          {
            // Toggle in/out of quotes
            inQuotes = !inQuotes;
          }
        }
        else if (c == ',' && !inQuotes)
        {
          // Field separator
          result.Add(current.ToString());
          current.Clear();
        }
        else
        {
          current.Append(c);
        }
      }

      // Last field
      result.Add(current.ToString());

      return result;
    }
  }
}
