using Microsoft.AspNetCore.Mvc;
using Scai.Driver.Data;
using Scai.Driver.Web.Dto;

namespace Scai.Driver.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IAiCompiler _compiler;

        public DriverController(IAiCompiler compiler)
        {
            _compiler = compiler;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AiResult))]
        [ProducesResponseType(400, Type = typeof(CompilationResult))]
        public ActionResult<AiResult> Post([FromBody] RunRequest request)
        {
            var compilationResult = _compiler.Compile(request.Code);
            if (!compilationResult.IsOk)
            {
                return BadRequest(compilationResult);
            }

            var ai = compilationResult.Ai;
            var aiResult = ai.Run(request.TrackState);

            return Ok(aiResult);
        }
    }
}
