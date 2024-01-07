using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IDriver _driver;

        public RecommendationController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpPost("GetRecommendations")]
        public async Task<IActionResult> GetRecommendations(int game1Id, int? game2Id, int? game3Id)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    
                    var deletionQuery = @"MATCH (u:User { name: 'user' })
                                OPTIONAL MATCH (u)-[r]-()
                                DELETE r,u";
                    await session.RunAsync(deletionQuery);

                    var createUserQuery = @"CREATE (n:User { name: $name })";
                    var createUserParameters = new
                    {
                        name = "user"
                    };
                    await session.RunAsync(createUserQuery, createUserParameters);

                    var addGamesQuery = @"
                        MATCH (n:User { name: $name }) 
                        MATCH (g1:Game) WHERE ID(g1)=$g1Id
                        CREATE (n)-[:PLAYED]->(g1)
                        ";

                    if(game2Id.HasValue)
                    {
                        addGamesQuery+="WITH n MATCH (g2:Game) WHERE ID(g2)=$g2Id CREATE (n)-[:PLAYED]->(g2) ";
                    }
                    if(game3Id.HasValue)
                    {
                        addGamesQuery+="WITH n MATCH (g3:Game) WHERE ID(g3)=$g3Id CREATE (n)-[:PLAYED]->(g3) ";
                    }
                                
                    var addGamesParameters = new
                    {
                        name = "user",
                        g1Id = game1Id,
                        g2Id = game2Id ?? -1,
                        g3Id = game3Id ?? -1,
                    };
                    await session.RunAsync(addGamesQuery, addGamesParameters);//odavde na dole brisem ako ne valja

                    var recommendationQuery = @"
                        MATCH (u:User { name: $name })
                        WITH u
                        MATCH (g1:Game)-[:HAS_GENRE]->(genre1:Genre)
                        WHERE ID(g1) = $g1Id
                        WITH COLLECT(DISTINCT genre1) AS genres1, u
                        ";
                    if (game2Id.HasValue)
                    {
                        recommendationQuery += @"
                            MATCH (g2:Game)-[:HAS_GENRE]->(genre2:Genre)
                            WHERE ID(g2) = $g2Id
                            WITH COLLECT(DISTINCT genre2) AS genres2, u, genres1
                        ";
                    }
                    else
                    {
                        recommendationQuery += "WITH u, genres1 ";
                    }
                    if (game3Id.HasValue)
                    {
                        recommendationQuery += @"
                            MATCH (g3:Game)-[:HAS_GENRE]->(genre3:Genre)
                            WHERE ID(g3) = $g3Id
                            WITH COLLECT(DISTINCT genre3) AS genres3, u, genres1, genres2
                        ";
                    }
                    else if(game2Id.HasValue)
                    {
                        recommendationQuery += "WITH u, genres1, genres2 ";
                    }
                    else
                    {
                        recommendationQuery += "WITH u, genres1 ";
                    }

                    recommendationQuery += @"
                        MATCH (g:Game)-[:HAS_GENRE]->(genre)
                        WHERE genre IN genres1";

                    if (game2Id.HasValue)
                    {
                        recommendationQuery += " AND genre IN genres2";
                    }

                    if (game3Id.HasValue)
                    {
                        recommendationQuery += " AND genre IN genres3";
                    }

                    recommendationQuery += " AND NOT (u)-[:PLAYED]->(g) RETURN DISTINCT g";

                    var recommendationParameters = new
                    {
                        name = "user",
                        g1Id = game1Id,
                        g2Id = game2Id ?? -1,
                        g3Id = game3Id ?? -1,
                    };

                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = recommendationQuery;
                        var parametri = recommendationParameters;
                        var cursor = await tx.RunAsync(query, parametri);
                        var nodes = new List<INode>();

                        await cursor.ForEachAsync(record =>
                        {
                            var node = record["g"].As<INode>();
                            nodes.Add(node);
                        });

                        return nodes;
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //UNWIND ["RPG", "Stealth"] AS genre WITH genre MATCH (g:Game)-[:HAS_GENRE]->(gen:Genre WHERE gen.name = genre) WITH g MATCH (g)-[:DISTRIBUTES]-(p:Publisher)  WITH p MATCH (ga:Game)-[:DISTRIBUTES]-(p) RETURN ga
        
    }
}