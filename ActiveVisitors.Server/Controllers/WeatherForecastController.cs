using Common;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Driver.Linq;

namespace ActiveVisitors.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public List<ActiveVisitorsResult> Get(DateTime datetime, string cameras = "")
        {
            var conn = new MongoDBConnection("mongodb://localhost:27017", "reports_data_sightings");

            var mongoCollection = conn.GetCollection<Sightings>("reports_data_sightings");

            long startTime = ((DateTimeOffset)datetime).ToUnixTimeMilliseconds();
            long endTime = ((DateTimeOffset)datetime.AddDays(1).AddMilliseconds(-1)).ToUnixTimeMilliseconds();

            var startFilter = Builders<Sightings>.Filter.Gte(d => d.Timestamp, startTime);
            var endFilter = Builders<Sightings>.Filter.Lte(d => d.Timestamp, endTime);

            List<FilterDefinition<Sightings>> filterlist = new List<FilterDefinition<Sightings>>();
            filterlist.Add(startFilter);
            filterlist.Add(endFilter);
            try
            {
                foreach (var cam in cameras.Split(','))
                {
                    if (!String.IsNullOrEmpty(cam))
                    {
                        filterlist.Add(Builders<Sightings>.Filter.Eq("camera", ObjectId.Parse(cam)));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var combinedFilter = Builders<Sightings>.Filter.And(filterlist);

            var result = mongoCollection.AsQueryable().Where(document => combinedFilter.Inject()).ToList();

            var groupedByIdentity = result.GroupBy(doc => doc.Identity).ToList();

            var finalList = new List<Sightings>();

            foreach ( var group in groupedByIdentity)
            {
                var tempGroup = group.ToList();
                if(tempGroup.Count > 0)
                {
                    finalList.Add(tempGroup[0]);
                    long first10MinStart = tempGroup[0].Timestamp;

                    for (int i = 0; i < tempGroup.Count; i++)
                    {
                        var a = tempGroup.Where(x => x.Timestamp > first10MinStart && x.Timestamp < first10MinStart + (10 * 60 * 1000)).Count();
                        i += a;
                        if (i < tempGroup.Count)
                        {
                            finalList.Add(tempGroup[i]);
                            first10MinStart = tempGroup[i].Timestamp;
                        }
                    }
                }
            }

            List<ActiveVisitorsResult> results = new List<ActiveVisitorsResult>();

            for (int i = 0; i < 24; i++)
            {
                ActiveVisitorsResult oneResult = new ActiveVisitorsResult();

                oneResult.dateTime = ConvertToLocalDate(startTime.ToString());
                oneResult.total = finalList.Where(x => x.Timestamp > (startTime) && x.Timestamp < (startTime + 60 * 60 * 1000)).Count();

                results.Add(oneResult);

                startTime = (startTime +  60 * 60 * 1000);
            }

            return results;
        }

        public static DateTime ConvertToLocalDate(string timeInMilliseconds)
        {
            double timeInTicks = double.Parse(timeInMilliseconds);
            TimeSpan dateTimeSpan = TimeSpan.FromMilliseconds(timeInTicks);
            DateTime dateAfterEpoch = new DateTime(1970, 1, 1) + dateTimeSpan;
            DateTime dateInLocalTimeFormat = dateAfterEpoch.ToLocalTime();
            return dateInLocalTimeFormat;
        }
    }

    public class MongoDBConnection
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoDBConnection(string connectionString, string databaseName)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public void CloseConnection()
        {
            // MongoDB .NET Driver doesn't require explicit closing of the connection.
            // The connection is managed by the driver itself.
        }

    }

    public class Sightings
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("identity")]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Identity { get; set; }

        [BsonElement("camera")]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Camera { get; set; }

        [BsonElement("timestamp")]
        [BsonRepresentation(BsonType.Int64)]
        public long Timestamp { get; set; }
        // Add other properties as needed
    }

    public class ActiveVisitorsResult
    {
        public DateTime dateTime { get; set; }
        public long total { get; set; }
    }



}