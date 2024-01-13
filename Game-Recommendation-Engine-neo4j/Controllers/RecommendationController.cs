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
        [HttpGet("GetRecommendations")]
        public async Task<IActionResult> GetRecommendations(int game1Id)
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

                    var addGamesParameters = new
                    {
                        name = "user",
                        g1Id = game1Id
                    };
                    await session.RunAsync(addGamesQuery, addGamesParameters);

                    var recommendationQuery = @"
                            MATCH (u:User { name: $name })
                            WITH u
                            MATCH (g1:Game)-[:HAS_GENRE]->(genre1:Genre)
                            WHERE ID(g1) = $g1Id
                            WITH COLLECT(DISTINCT genre1) AS genres1, u
                            MATCH (g:Game)-[:HAS_GENRE]->(genre)
                            WHERE genre IN genres1 
                            AND NOT (u)-[:PLAYED]->(g) RETURN DISTINCT g
                            UNION 
                            MATCH(p:Publisher)-[:DISTRIBUTES]->(g1:Game)
                            WHERE ID(g1) = $g1Id
                            WITH p
                            MATCH (p:Publisher)-[:DISTRIBUTES]->(g:Game)
                            WHERE ID(g) <> $g1Id
                            RETURN g";

                    var recommendationParameters = new
                    {
                        name = "user",
                        g1Id = game1Id
                    };

                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var cursor = await tx.RunAsync(recommendationQuery, recommendationParameters);
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
    }
}