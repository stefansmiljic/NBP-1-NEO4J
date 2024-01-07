using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using System.Text.Json;
using Microsoft.VisualBasic;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        
        
        private readonly IDriver _driver;

        public GameController(IDriver driver)
        {
            _driver = driver;
        }
        [HttpPost("CreateGame")]
        public async Task<IActionResult> CreateGame(Game game)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                   
                    var query = @"
                    CREATE (n:Game 
                    { 
                        name: $name,
                        thumbnail: $thumbnail,
                        rating: $rating
                    })
                    WITH n
                    UNWIND $genres AS genreName
                    MERGE (g:Genre { name: genreName })
                    MERGE (n)-[:HAS_GENRE]->(g)
                    WITH n
                    UNWIND $publisher AS publisherName
                    MERGE (p:Publisher { name: publisherName })
                    MERGE (p)-[:DISTRIBUTES]->(n)";

                    var parameters = new
                    {
                        name = game.Name,
                        thumbnail = game.ThumbnailURL,
                        rating = game.Rating,
                        genres = game.Genres.Select(g=>g.ToString()).ToArray(),
                        publisher = game.Publisher.Name
                    };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("DeleteGame")]
        public async Task<IActionResult> RemoveGame(int gameId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (g:Game) where ID(g)=$gId
                                OPTIONAL MATCH (g)-[r]-()
                                DELETE r,g";
                    var parameters = new { gId = gameId };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteGenre")]
        public async Task<IActionResult> RemoveGenre(int genreId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (g:Genre) where ID(g)=$gId
                                OPTIONAL MATCH (g)-[r]-()
                                DELETE r,g";
                    var parameters = new { gId = genreId };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllGames")]
        public async Task<IActionResult> GetAllGames()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (g:Game) RETURN g";
                        var cursor = await tx.RunAsync(query);
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

        [HttpPost("GetGamesByGenre")]
        public async Task<IActionResult> GetGamesByGenre(List<string> genres)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = @"UNWIND $genres AS genre WITH genre MATCH (g:Game)-[:HAS_GENRE]->(gen:Genre WHERE gen.name = genre) RETURN DISTINCT g";
                        var parameters = new {
                            genres = genres
                        };
                        var cursor = await tx.RunAsync(query, parameters);
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