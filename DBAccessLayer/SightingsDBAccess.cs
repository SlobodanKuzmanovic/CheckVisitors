using Common;
using DBAccessLayer.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccessLayer
{
    public class SightingsDBAccess
    {
        public SightingsDBAccess()
        {
                
        }
        public List<Visitors> ReadData(DateTime datetime, string? cameras = null)
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

            foreach (var group in groupedByIdentity)
            {
                var tempGroup = group.ToList();
                if (tempGroup.Count > 0)
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

            List<Visitors> results = new List<Visitors>();

            for (int i = 0; i < 24; i++)
            {
                Visitors oneResult = new Visitors();

                oneResult.dateTime = Helper.ConvertToLocalDate(startTime.ToString()).ToString("yyyy-MM-dd'T'HH:mm:ss");
                oneResult.total = finalList.Where(x => x.Timestamp > (startTime) && x.Timestamp < (startTime + 60 * 60 * 1000)).Count();

                results.Add(oneResult);

                startTime = (startTime + 60 * 60 * 1000);
            }

            return results;
        }
    }
}
