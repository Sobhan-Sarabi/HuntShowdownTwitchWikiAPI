using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      var cmd = connection.CreateCommand();
      cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Weapons (
    FullWeaponName   TEXT PRIMARY KEY,
    WeaponAugment    TEXT,
    WeaponBase       TEXT,
    AmmoType         TEXT,
    Size             TEXT,
    SpecialType      TEXT,
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
    Note             TEXT
);

CREATE TABLE IF NOT EXISTS Traits (
    Name    TEXT PRIMARY KEY,
    Cost    TEXT,
    Unlock  TEXT,
    Type    TEXT,
    Effect  TEXT
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
    FullWeaponName,
    WeaponAugment,
    WeaponBase,
    AmmoType,
    Size,
    SpecialType,
    GeneralAmmoType,
    Price,
    Damage,
    DropRange,
    MuzzleVelocity,
    VerticalRecoil,
    RateOfFire,
    CycleTime,
    Spread,
    Sway,
    ReloadSpeed,
    Loaded,
    Extra,
    Note
) VALUES (
    $FullWeaponName,
    $WeaponAugment,
    $WeaponBase,
    $AmmoType,
    $Size,
    $SpecialType,
    $GeneralAmmoType,
    $Price,
    $Damage,
    $DropRange,
    $MuzzleVelocity,
    $VerticalRecoil,
    $RateOfFire,
    $CycleTime,
    $Spread,
    $Sway,
    $ReloadSpeed,
    $Loaded,
    $Extra,
    $Note
)
ON CONFLICT(FullWeaponName) DO NOTHING;  -- don’t overwrite if it’s already there
";

        cmd.Parameters.AddWithValue("$FullWeaponName", w.FullWeaponName ?? "");
        cmd.Parameters.AddWithValue("$WeaponAugment", w.WeaponAugment ?? "");
        cmd.Parameters.AddWithValue("$WeaponBase", w.WeaponBase ?? "");
        cmd.Parameters.AddWithValue("$AmmoType", w.AmmoType ?? "");
        cmd.Parameters.AddWithValue("$Size", w.Size ?? "");
        cmd.Parameters.AddWithValue("$SpecialType", w.SpecialType ?? "");
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
        cmd.Parameters.AddWithValue("$Note", w.Note ?? "");

        cmd.ExecuteNonQuery();
      }

      transaction.Commit();
    }

    public WeaponData? GetWeapon(string fullWeaponName)
    {
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      var cmd = connection.CreateCommand();
      cmd.CommandText = "SELECT * FROM Weapons WHERE FullWeaponName = $FullWeaponName;";
      cmd.Parameters.AddWithValue("$FullWeaponName", fullWeaponName);

      using var reader = cmd.ExecuteReader();
      if (!reader.Read())
        return null;

      return new WeaponData
      {
        FullWeaponName = reader["FullWeaponName"]?.ToString(),
        WeaponAugment = reader["WeaponAugment"]?.ToString(),
        WeaponBase = reader["WeaponBase"]?.ToString(),
        AmmoType = reader["AmmoType"]?.ToString(),
        Size = reader["Size"]?.ToString(),
        SpecialType = reader["SpecialType"]?.ToString(),
        GeneralAmmoType = reader["GeneralAmmoType"]?.ToString(),
        Price = reader["Price"]?.ToString(),
        Damage = reader["Damage"]?.ToString(),
        DropRange = reader["DropRange"]?.ToString(),
        MuzzleVelocity = reader["MuzzleVelocity"]?.ToString(),
        VerticalRecoil = reader["VerticalRecoil"]?.ToString(),
        RateOfFire = reader["RateOfFire"]?.ToString(),
        CycleTime = reader["CycleTime"]?.ToString(),
        Spread = reader["Spread"]?.ToString(),
        Sway = reader["Sway"]?.ToString(),
        ReloadSpeed = reader["ReloadSpeed"]?.ToString(),
        Loaded = reader["Loaded"]?.ToString(),
        Extra = reader["Extra"]?.ToString(),
        Note = reader["Note"]?.ToString()
      };
    }

    public Collection<WeaponData> GetWeaponsByAugment(string weaponAugment)
    {
      var results = new Collection<WeaponData>();

      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      var cmd = connection.CreateCommand();
      cmd.CommandText = @"
    SELECT * 
    FROM Weapons 
    WHERE WeaponAugment = $WeaponAugment COLLATE NOCASE;
  ";
      cmd.Parameters.AddWithValue("$WeaponAugment", weaponAugment);

      using var reader = cmd.ExecuteReader();
      while (reader.Read())
      {
        results.Add(new WeaponData
        {
          FullWeaponName = reader["FullWeaponName"]?.ToString(),
          WeaponAugment = reader["WeaponAugment"]?.ToString(),
          WeaponBase = reader["WeaponBase"]?.ToString(),
          AmmoType = reader["AmmoType"]?.ToString(),
          Size = reader["Size"]?.ToString(),
          SpecialType = reader["SpecialType"]?.ToString(),
          GeneralAmmoType = reader["GeneralAmmoType"]?.ToString(),
          Price = reader["Price"]?.ToString(),
          Damage = reader["Damage"]?.ToString(),
          DropRange = reader["DropRange"]?.ToString(),
          MuzzleVelocity = reader["MuzzleVelocity"]?.ToString(),
          VerticalRecoil = reader["VerticalRecoil"]?.ToString(),
          RateOfFire = reader["RateOfFire"]?.ToString(),
          CycleTime = reader["CycleTime"]?.ToString(),
          Spread = reader["Spread"]?.ToString(),
          Sway = reader["Sway"]?.ToString(),
          ReloadSpeed = reader["ReloadSpeed"]?.ToString(),
          Loaded = reader["Loaded"]?.ToString(),
          Extra = reader["Extra"]?.ToString(),
          Note = reader["Note"]?.ToString()
        });
      }

      return results;
    }

    public WeaponData? GetWeaponByKey(IReadOnlyList<string> keyParts)
    {
      if (keyParts == null || keyParts.Count != 3)
        throw new ArgumentException("Expected 3 elements: [WeaponAugment, WeaponBase, AmmoType]", nameof(keyParts));

      var augment = keyParts[1] ?? "";
      var weaponBase = keyParts[0] ?? "";
      var ammoType = keyParts[2] ?? "";

      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      var cmd = connection.CreateCommand();
      cmd.CommandText = @"
        SELECT *
        FROM Weapons
        WHERE WeaponAugment = $WeaponAugment
          AND WeaponBase    = $WeaponBase
          AND AmmoType      = $AmmoType;
    ";

      cmd.Parameters.AddWithValue("$WeaponAugment", augment);
      cmd.Parameters.AddWithValue("$WeaponBase", weaponBase);
      cmd.Parameters.AddWithValue("$AmmoType", ammoType);

      using var reader = cmd.ExecuteReader();
      if (!reader.Read())
        return null;

      WeaponData weaponData = new WeaponData
      {
        FullWeaponName = reader["FullWeaponName"]?.ToString(),
        WeaponAugment = reader["WeaponAugment"]?.ToString(),
        WeaponBase = reader["WeaponBase"]?.ToString(),
        AmmoType = reader["AmmoType"]?.ToString(),
        Size = reader["Size"]?.ToString(),
        SpecialType = reader["SpecialType"]?.ToString(),
        GeneralAmmoType = reader["GeneralAmmoType"]?.ToString(),
        Price = reader["Price"]?.ToString(),
        Damage = reader["Damage"]?.ToString(),
        DropRange = reader["DropRange"]?.ToString(),
        MuzzleVelocity = reader["MuzzleVelocity"]?.ToString(),
        VerticalRecoil = reader["VerticalRecoil"]?.ToString(),
        RateOfFire = reader["RateOfFire"]?.ToString(),
        CycleTime = reader["CycleTime"]?.ToString(),
        Spread = reader["Spread"]?.ToString(),
        Sway = reader["Sway"]?.ToString(),
        ReloadSpeed = reader["ReloadSpeed"]?.ToString(),
        Loaded = reader["Loaded"]?.ToString(),
        Extra = reader["Extra"]?.ToString(),
        Note = reader["Note"]?.ToString()
      };

      return weaponData;
    }

    public bool HasAnyTraits()
    {
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      var cmd = connection.CreateCommand();
      cmd.CommandText = "SELECT 1 FROM Traits LIMIT 1;";

      using var reader = cmd.ExecuteReader();
      return reader.Read();
    }

    public void BulkUpsertTraits(IEnumerable<TraitData> traits)
    {
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      using var transaction = connection.BeginTransaction();

      foreach (var t in traits)
      {
        var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;

        cmd.CommandText = @"
INSERT INTO Traits (
    Name,
    Cost,
    Unlock,
    Type,
    Effect
) VALUES (
    $Name,
    $Cost,
    $Unlock,
    $Type,
    $Effect
)
ON CONFLICT(Name) DO NOTHING;
";

        cmd.Parameters.AddWithValue("$Name", t.Name ?? "");
        cmd.Parameters.AddWithValue("$Cost", t.Cost ?? "");
        cmd.Parameters.AddWithValue("$Unlock", t.Unlock ?? "");
        cmd.Parameters.AddWithValue("$Type", t.Type ?? "");
        cmd.Parameters.AddWithValue("$Effect", t.Effect ?? "");

        cmd.ExecuteNonQuery();
      }

      transaction.Commit();
    }

    public TraitData? GetTrait(string name)
    {
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();

      var cmd = connection.CreateCommand();
      cmd.CommandText = "SELECT * FROM Traits WHERE Name = $Name;";
      cmd.Parameters.AddWithValue("$Name", name);

      using var reader = cmd.ExecuteReader();
      if (!reader.Read())
        return null;

      return new TraitData
      {
        Name = reader["Name"]?.ToString(),
        Cost = reader["Cost"]?.ToString(),
        Unlock = reader["Unlock"]?.ToString(),
        Type = reader["Type"]?.ToString(),
        Effect = reader["Effect"]?.ToString()
      };
    }
  }
}
