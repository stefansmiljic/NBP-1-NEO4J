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
    public class PublisherController : ControllerBase
    {
        private readonly IDriver _driver;

        public PublisherController(IDriver driver)
        {
            _driver = driver;
        }
        [HttpPost("CreatePublisher")]
        public async Task<IActionResult> CreatePublisher(Publisher publisher)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"CREATE (n:Publisher { name: $nameValue})";

                    var parameters = new
                    {
                        nameValue = publisher.Name
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
        
        [HttpPost("AddGamesToPublisher")]
        public async Task<IActionResult> AddGamesToPublisher(int gameId, int publisherId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Publisher) WHERE ID(n)=$pId
                                MATCH (m:Game) WHERE ID(m)=$gId
                                CREATE (n)-[:DISTRIBUTES]->(m)";

                    var parameters = new
                    {
                        gId = gameId,
                        pId = publisherId
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

        /*[HttpDelete]
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