// See https://aka.ms/new-console-template for more information
using EF_Core_Perf;
using EF_Core_Perf.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Xml.Linq;

var dbFile = Path.Combine(AppContext.BaseDirectory, "db.sqlite");

using var httpClient = new HttpClient();

var firstStart = !File.Exists(dbFile); 

var connection = new SqliteConnection($"Data Source={dbFile}");
connection.Open();

var options = new DbContextOptionsBuilder<HighwayDbContext>()
    .UseSqlite(connection)
    .Options;

// download data if not exists
if (firstStart)
{
    var client = new Api.ApiClient(httpClient);
    var highways = await client.ListAutobahnenAsync();
    var roadEntities = highways.Roads1.Select(x => new RoadEntity
    {
        Name = x
    });

    using var context = new HighwayDbContext(options);

    foreach (var roadEntity in roadEntities)
    {
        try
        {
            var restingAreas = await client.ListParkingLorriesAsync(roadEntity.Name);

            roadEntity.RestingAreas = restingAreas.Parking_lorry.Select(x => new RestingAreaEntity
            {
                Road = roadEntity,
                Future = x.Future,
                Longitude = x.Coordinate.Long,
                Latitude = x.Coordinate.Lat,
                IsBlocked = x.IsBlocked == "true",
                Title = x.Title,
                Subtitle = x.Subtitle,
                Features = x.LorryParkingFeatureIcons.Select(x => new RestingAreaFeature
                {
                    Name = x.Description
                }).ToList()
            }).ToList();

            context.Database.Migrate();
            context.Roads.Add(roadEntity);
            context.SaveChanges();
        }
        catch
        {
        }
    }
}

// Warmup DB - first call is always way slower
{ 
    using var dbContext = new HighwayDbContext(options);
    var result = dbContext
        .Roads
        .Include(x => x.RestingAreas)
            .ThenInclude(x => x.Features)
        .ToList();
}

const int sampleSize = 20;
var efPureTimes = new List<double>(sampleSize);
for (int i = 0; i < sampleSize; i++)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    using var dbContext = new HighwayDbContext(options);
    var result = dbContext
        .Roads
        .Include(x => x.RestingAreas)
            .ThenInclude(x => x.Features)
        .ToList();
    stopwatch.Stop();
    efPureTimes.Add(stopwatch.ElapsedMilliseconds);
}

Console.WriteLine($"{efPureTimes.Min()}ms {efPureTimes.Max()}ms {efPureTimes.Average()}ms (default)");


var efNoTrackingTimes = new List<double>(sampleSize);
for (int i = 0; i < sampleSize; i++)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    using var dbContext = new HighwayDbContext(options);
    var result = dbContext
        .Roads
        .Include(x => x.RestingAreas)
            .ThenInclude(x => x.Features)
        .AsNoTracking()
        .ToList();
    stopwatch.Stop();
    efNoTrackingTimes.Add(stopwatch.ElapsedMilliseconds);
}

Console.WriteLine($"{efNoTrackingTimes.Min()}ms {efNoTrackingTimes.Max()}ms {efNoTrackingTimes.Average()}ms (AsNoTracking)");

var efNoTrackingSplit = new List<double>(sampleSize);
for (int i = 0; i < sampleSize; i++)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    using var dbContext = new HighwayDbContext(options);
    var result = dbContext
        .Roads
        .Include(x => x.RestingAreas)
            .ThenInclude(x => x.Features)
        .AsNoTracking()
        .AsSplitQuery()
        .ToList();
    stopwatch.Stop();
    efNoTrackingSplit.Add(stopwatch.ElapsedMilliseconds);
}

Console.WriteLine($"{efNoTrackingSplit.Min()}ms {efNoTrackingSplit.Max()}ms {efNoTrackingSplit.Average()}ms (AsNoTracking+AsSplitQuery)");

var efOnlySplit = new List<double>(sampleSize);
for (int i = 0; i < sampleSize; i++)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    using var dbContext = new HighwayDbContext(options);
    var result = dbContext
        .Roads
        .Include(x => x.RestingAreas)
            .ThenInclude(x => x.Features)
        .AsSplitQuery()
        .ToList();
    stopwatch.Stop();
    efOnlySplit.Add(stopwatch.ElapsedMilliseconds);
}

Console.WriteLine($"{efOnlySplit.Min()}ms {efOnlySplit.Max()}ms {efOnlySplit.Average()}ms (AsSplitQuery)");

var efNoTrackingSplitProjection = new List<double>(sampleSize);
for (int i = 0; i < sampleSize; i++)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    using var dbContext = new HighwayDbContext(options);
    var result = dbContext
        .Roads
        .Include(x => x.RestingAreas)
            .ThenInclude(x => x.Features)
        .AsNoTracking()
        .Select(x => new
        {
            RoadName = x.Name,
            HasBlockedRestingAreas = x.RestingAreas.Select(r => r.IsBlocked),
            FeatureNames = x.RestingAreas.Select(r => r.Features.Select(f => f.Name))
        })
        .AsSplitQuery()
        .ToList();
    stopwatch.Stop();
    efNoTrackingSplitProjection.Add(stopwatch.ElapsedMilliseconds);
}

Console.WriteLine($"{efNoTrackingSplitProjection.Min()}ms {efNoTrackingSplitProjection.Max()}ms {efNoTrackingSplitProjection.Average()}ms (NoTracking+Split+Projection)");

var sqlProjection = new List<double>(sampleSize);
for (int i = 0; i < sampleSize; i++)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    using var dbContext = new HighwayDbContext(options);
    var result = dbContext
        .RawResult
        .FromSql($"SELECT r.Name, a.IsBlocked, f.Name as Feature FROM Roads r, RestingAreas a, RestingAreaFeature f WHERE a.RoadId = r.Id AND f.RestingAreaEntityId = a.Id ")
        .ToList();
    stopwatch.Stop();
    sqlProjection.Add(stopwatch.ElapsedMilliseconds);
}

Console.WriteLine($"{sqlProjection.Min()}ms {sqlProjection.Max()}ms {sqlProjection.Average()}ms (raw sql)");
