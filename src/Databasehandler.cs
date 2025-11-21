using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace HuntShowdownTwitchWikiAPI.src
{
  public class WeaponRepository
  {
    private readonly string _databasePath;
    private readonly string _connectionString;

    public WeaponRepository(string databasePath)
    {
      _databasePath = databasePath;
      _connectionString = $"Data Source={databasePath}";
      EnsureCreated();
    }

    private void EnsureCreated()
    {
      // Just opening this path will create the file if it doesn’t exist
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      var cmd = connection.CreateCommand();
      cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Weapons (
    Weapon           TEXT PRIMARY KEY,
    Size             TEXT,
    SpecialAmmoType  TEXT,
    GeneralAmmoType  TEXT,
    Price            TEXT,
    Damage           TEXT,
    DropRange        TEXT,
    MuzzleVelocity   TEXT,
    VerticalRecoil   TEXT,
    RateOfFire       TEXT,
    CycleTime        TEXT,
    Spread           TEXT,
    Sway             TEXT,
    ReloadSpeed      TEXT,
    Loaded           TEXT,
    Extra            TEXT,
    AmmoType         TEXT,
    Note             TEXT
);";
      cmd.ExecuteNonQuery();
    }

    public bool HasAnyWeapons()
    {
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      var cmd = connection.CreateCommand();
      cmd.CommandText = "SELECT 1 FROM Weapons LIMIT 1;";

      using var reader = cmd.ExecuteReader();
      return reader.Read();
    }

    public void BulkUpsert(IEnumerable<WeaponData> weapons)
    {
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      using var transaction = connection.BeginTransaction();

      foreach (var w in weapons)
      {
        var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;

        cmd.CommandText = @"
INSERT INTO Weapons (
    Weapon, Size, SpecialAmmoType, GeneralAmmoType, Price, Damage, DropRange,
    MuzzleVelocity, VerticalRecoil, RateOfFire, CycleTime, Spread, Sway,
    ReloadSpeed, Loaded, Extra, AmmoType, Note
) VALUES (
    $Weapon, $Size, $SpecialAmmoType, $GeneralAmmoType, $Price, $Damage, $DropRange,
    $MuzzleVelocity, $VerticalRecoil, $RateOfFire, $CycleTime, $Spread, $Sway,
    $ReloadSpeed, $Loaded, $Extra, $AmmoType, $Note
)
ON CONFLICT(Weapon) DO NOTHING;  -- don’t overwrite if it’s already there
";

        cmd.Parameters.AddWithValue("$Weapon", w.Weapon ?? "");
        cmd.Parameters.AddWithValue("$Size", w.Size ?? "");
        cmd.Parameters.AddWithValue("$SpecialAmmoType", w.SpecialAmmoType ?? "");
        cmd.Parameters.AddWithValue("$GeneralAmmoType", w.GeneralAmmoType ?? "");
        cmd.Parameters.AddWithValue("$Price", w.Price ?? "");
        cmd.Parameters.AddWithValue("$Damage", w.Damage ?? "");
        cmd.Parameters.AddWithValue("$DropRange", w.DropRange ?? "");
        cmd.Parameters.AddWithValue("$MuzzleVelocity", w.MuzzleVelocity ?? "");
        cmd.Parameters.AddWithValue("$VerticalRecoil", w.VerticalRecoil ?? "");
        cmd.Parameters.AddWithValue("$RateOfFire", w.RateOfFire ?? "");
        cmd.Parameters.AddWithValue("$CycleTime", w.CycleTime ?? "");
        cmd.Parameters.AddWithValue("$Spread", w.Spread ?? "");
        cmd.Parameters.AddWithValue("$Sway", w.Sway ?? "");
        cmd.Parameters.AddWithValue("$ReloadSpeed", w.ReloadSpeed ?? "");
        cmd.Parameters.AddWithValue("$Loaded", w.Loaded ?? "");
        cmd.Parameters.AddWithValue("$Extra", w.Extra ?? "");
        cmd.Parameters.AddWithValue("$AmmoType", w.AmmoType ?? "");
        cmd.Parameters.AddWithValue("$Note", w.Note ?? "");

        cmd.ExecuteNonQuery();
      }

      transaction.Commit();
    }

    public WeaponData? GetWeapon(string weaponName)
    {
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      var cmd = connection.CreateCommand();
      cmd.CommandText = "SELECT * FROM Weapons WHERE Weapon = $Weapon;";
      cmd.Parameters.AddWithValue("$Weapon", weaponName);

      using var reader = cmd.ExecuteReader();
      if (!reader.Read())
        return null;

      return new WeaponData
      {
        Weapon = reader["Weapon"].ToString(),
        Size = reader["Size"].ToString(),
        SpecialAmmoType = reader["SpecialAmmoType"].ToString(),
        GeneralAmmoType = reader["GeneralAmmoType"].ToString(),
        Price = reader["Price"].ToString(),
        Damage = reader["Damage"].ToString(),
        DropRange = reader["DropRange"].ToString(),
        MuzzleVelocity = reader["MuzzleVelocity"].ToString(),
        VerticalRecoil = reader["VerticalRecoil"].ToString(),
        RateOfFire = reader["RateOfFire"].ToString(),
        CycleTime = reader["CycleTime"].ToString(),
        Spread = reader["Spread"].ToString(),
        Sway = reader["Sway"].ToString(),
        ReloadSpeed = reader["ReloadSpeed"].ToString(),
        Loaded = reader["Loaded"].ToString(),
        Extra = reader["Extra"].ToString(),
        AmmoType = reader["AmmoType"].ToString(),
        Note = reader["Note"].ToString()
      };
    }
  }
}
