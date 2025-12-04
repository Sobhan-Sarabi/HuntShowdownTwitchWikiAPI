using System;

namespace HuntShowdownTwitchWikiAPI.src
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);
      var app = builder.Build();

      app.UseDefaultFiles();
      app.UseStaticFiles();

      builder.WebHost.UseUrls("http://localhost:3536");



      //string weaponCsvPath = "G:/Sobhan/Database/WeponsData.csv";
      //string traitsCsvPath = "G:/Sobhan/Database/TtraitsData.csv";
      //string dbPath = "G:/Sobhan/Database/weapons.db";

      string weaponCsvPath = "D:/Sobhan/Programing/Csharp/dev/demo/HuntShowdownTwitchWikiAPIDataBase/WeponsData.csv";
      string traitsCsvPath = "D:/Sobhan/Programing/Csharp/dev/demo/HuntShowdownTwitchWikiAPIDataBase/TtraitsData.csv";
      string dbPath = "D:/Sobhan/Programing/Csharp/dev/demo/HuntShowdownTwitchWikiAPIDataBase/weapons.db"; 

      var repo = new WeaponRepository(dbPath);
      var helper = new Helper();

      if (!repo.HasAnyWeapons())
      {
        Console.WriteLine("Weapons table empty – importing from weapons CSV once...");

        helper.CsvOpener(weaponCsvPath);
        var weapons = helper.CsvLoadToWeapn();
        repo.BulkUpsert(weapons);

        Console.WriteLine("Weapon import done.");
      }
      else
      {
        Console.WriteLine("Weapons table already has data – skipping weapons CSV import.");
      }

      if (!repo.HasAnyTraits())
      {
        Console.WriteLine("Traits table empty – importing from traits CSV once...");

        helper.TraitsCsvOpener(traitsCsvPath);
        var traits = helper.CsvLoadToTraits();
        repo.BulkUpsertTraits(traits);

        Console.WriteLine("Trait import done.");
      }
      else
      {
        Console.WriteLine("Traits table already has data – skipping traits CSV import.");
      }

      Parser parser = new Parser(repo);

      app.MapGet("/huntwiki", (string q) =>
      {
        const string marker = " p ";
        int idx = q.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        var parts = q.Split(marker);

        string weaponName = parts[0];
        string parameters = parts.Length > 1 ? parts[1] : null;
        var parametersSplit = parameters?.Split(new[] { ' ' }, 2);


        if (parameters == null)
        {
          return parser.QueueParser(weaponName, ReturnString.NormalReturn);
        }
        else if (string.Equals(parametersSplit[0], "Extended", StringComparison.OrdinalIgnoreCase))
        {
          return parser.QueueParser(weaponName, ReturnString.ExtendedReturn);
        }
        else if (string.Equals(parametersSplit[0], "spc", StringComparison.OrdinalIgnoreCase))
        {
          if (parametersSplit.Length == 1)
          {
            return "No attribute given";
          }
          return parser.QueueParser(weaponName, 
            ReturnString.CustomReturn, parametersSplit[1]);
        }
        else
        {
          return "No pramters given";
        }

        return "dom?";
      });



      app.Run();
    }
  }
}
