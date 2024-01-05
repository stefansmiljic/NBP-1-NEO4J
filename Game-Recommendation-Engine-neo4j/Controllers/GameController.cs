using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

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
                    var query = @"CREATE (n:Game { name: $name,
                                                    genre: $genre,
                                                    thumbnail: $thumbnail,
                                                    rating: $rating})";

                    var parameters = new
                    {
                        name = game.Name,
                        genre = game.Genres,
                        thumbnail = game.ThumbnailURL,
                        rating = game.Rating
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
        /*[HttpPost("AssignClass")]
        public async Task<IActionResult> AssignClass(int classId, int playerId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Class) WHERE ID(n)=$cId
                                MATCH (m:Player) WHERE ID(m)=$pId
                                CREATE (m)-[:IS]->(n)";

                    var parameters = new
                    {
                        cId = classId,
                        pId = playerId
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

        [HttpDelete]
        public async Task<IActionResult> RemoveClass(int classId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (c:Class) where ID(c)=$cId
                                OPTIONAL MATCH (c)-[r]-()
                                DELETE r,c";
                    var parameters = new { cId = classId };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateClass")]
        public async Task<IActionResult> UpdateClass(int classId, string newName)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Class) WHERE ID(n)=$cId
                                SET n.name=$name
                                RETURN n";
                    var parameters = new { cId = classId,
                                        name = newName };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllClasses")]
        public async Task<IActionResult> GetAllClasses()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Class) RETURN n";
                        var cursor = await tx.RunAsync(query);
                        var nodes = new List<INode>();

                        await cursor.ForEachAsync(record =>
                        {
                            var node = record["n"].As<INode>();
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
        }*/
    }
}