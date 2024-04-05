using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiderAPI.Spider;
using SpiderAPI.Spider.Engine;

namespace SpiderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpiderController : ControllerBase
    {
        private readonly ILogger<SpiderController> _logger;

        public SpiderController(ILogger<SpiderController> logger)
        {
            _logger = logger;
        }

        [HttpGet("ping")]
        public int Ping()
        {
            return 1;
        }

        [HttpGet("new")]
        public APIInitData New(int Difficulty)
        {
            PlayEngine engine = RuleManager.NewRule(Difficulty);

            APIInitData data = new APIInitData() { ID = engine.ID };
            
            engine._New(data);

            return data;
        }

        [HttpGet("stack")]
        public APIStackData? Stack(string ID)
        {
            PlayEngine? engine = RuleManager.GetEngine(ID);

            if (engine != null)
            {
                APIStackData data = new APIStackData() { ID = engine.ID };

                engine.stackclick(data);

                return data;
            }

            return null;
        }

        [HttpGet("undo")]
        public APIUndoData? Undo(string ID)
        {
            PlayEngine? engine = RuleManager.GetEngine(ID);

            if (engine != null)
            {
                APIUndoData data = new APIUndoData() { ID = engine.ID };

                engine.undo(data);

                return data;
            }

            return null;
        }

        [HttpGet("solve")]
        public APIMoveData? Solve(string ID)
        {
            PlayEngine? engine = RuleManager.GetEngine(ID);

            if (engine != null)
            {
                APIMoveData data = new APIMoveData() { ID = engine.ID };

                engine.solve(data);

                return data;
            }

            return null;
        }

        [HttpGet("move")]
        public APIMoveData Move([FromQuery] string ID, [FromQuery] int X, [FromQuery] int Y)
        {
            PlayEngine? engine = RuleManager.GetEngine(ID);

            if (engine != null)
            {
                APIMoveData data = new APIMoveData() { ID = engine.ID };

                engine.OnLeftClick(X, Y, data);

                return data;
            }

            return null;
        }
    }
}
