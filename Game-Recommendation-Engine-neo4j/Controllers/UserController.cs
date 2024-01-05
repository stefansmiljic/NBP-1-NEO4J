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
    public class UserController : ControllerBase
    {
        private readonly IDriver _driver;

        public UserController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(User user)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"CREATE (n:User { name: $name})";

                    var parameters = new
                    {
                        name = user.Name
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

        [HttpPost("AddGamesToUser")]
        public async Task<IActionResult> AddGamesToUser(int gameId, int userId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:User) WHERE ID(n)=$uId
                                MATCH (m:Game) WHERE ID(m)=$gId
                                CREATE (n)-[:PLAYED]->(m)";

                    var parameters = new
                    {
                        gId = gameId,
                        uId = userId
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
    }
}