using CountryApi.Repositories.Models;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CountryApi.Repositories
{
    public  class AdministrativeAreasRepository
    {
        private ElasticClient client;
        private string fileName, indexName;
        private ILogger<AdministrativeAreasRepository> logger;

        public AdministrativeAreasRepository(ElasticClient client, string fileName, string indexName, ILogger<AdministrativeAreasRepository> logger)
        {
            this.client = client;
            this.fileName = fileName;
            this.indexName = indexName;
            this.logger = logger;
        }

        public async Task SetupAsync() {

            var createIndex01Response = client.Indices.Create(indexName, index =>
            
                index.Map<Feature>(map =>
                                            map.AutoMap().
                                        Properties(props => props.GeoShape(geo => geo.Name(p => p.geometry))))
            );

            // Read GeoJSON file
            var geoJsonContent = File.ReadAllText(fileName);

            // Deserialize GeoJSON to a list of your GeoShapeType
            var mapAreas = JsonConvert.DeserializeObject<Feature[]>(geoJsonContent);

            // Execute the bulk request
            var bulkResponse = client.Bulk(b => b
                    .Index(indexName)
                    .IndexMany<Feature>(mapAreas)
                );

            if (bulkResponse.IsValid)
            {
                logger.LogInformation($"Bulk insert successful {bulkResponse.DebugInformation}");
                
            }
            else
            {
                logger.LogWarning($"Bulk insert failed: {bulkResponse.DebugInformation}");
            }

        }

        public async Task<Properties1> SearchAsync(double latitude, double longitude, CancellationToken token) 
        {

            
            var queryGeoPoint = new GeoLocation(latitude, longitude);

            
            var searchResponse = client.Search<Feature>(s => s
                       .Index(this.indexName)
                       .Query(q => q
                                .GeoShape(g => g
                                    .Field( f => f.geometry)
                                    .Shape(shape => shape
                                        .Point(new GeoCoordinate(latitude, longitude)
                                     )
                                    ).Relation(GeoShapeRelation.Contains)
                                  )
                                )
                   );

            if (!searchResponse.IsValid)
            {
                return null;
            }


            foreach (var hit in searchResponse.Hits)
            {

                logger.LogInformation($"Document found: {hit.Id}");
            }

            return searchResponse.Documents.FirstOrDefault()?.properties;

        }
    }
}
