using System.Diagnostics.Metrics;

namespace HuntShowdownTwitchWikiAPI.src
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);
      var app = builder.Build();

      builder.WebHost.UseUrls("http://localhost:5000");

      string csvPath = "D:/Sobhan/Programing/C#/dev/demo/HuntShowdownTwitchWikiAPIDataBase/WeponsData.csv";
      string dbPath = "D:/Sobhan/Programing/C#/dev/demo/HuntShowdownTwitchWikiAPIDataBase/weapons.db";

      var repo = new WeaponRepository(dbPath);

      if (!repo.HasAnyWeapons())
      {
        Console.WriteLine("DB empty – importing from CSV once...");

        var helper = new Helper();
        helper.CsvOpener(csvPath);
        var weapons = helper.CsvLoadToWeapn();

        repo.BulkUpsert(weapons);

        Console.WriteLine("Import done. From now on we only read from DB.");
      }
      else
      {
        Console.WriteLine("DB already has data – skipping CSV import.");
      }


      app.MapGet("/huntwiki", (string q) =>
      {
        const string marker = " p ";
        int idx = q.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        string weaponName;
        string paramPart = "";

        if (idx <= 0)
        {
          weaponName = q.Trim();
        }
        else
        {
          weaponName = q[..idx].Trim();
          int startParams = idx + marker.Length;
          if (startParams < q.Length)
          {
            paramPart = q[startParams..].Trim();
          }
        }

        string[] parameters = Array.Empty<string>();
        if (!string.IsNullOrEmpty(paramPart))
        {
          parameters = paramPart.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        return Parser.QueueParser(repo, weaponName, parameters);

      });

      app.Run();
    }
  }
}
