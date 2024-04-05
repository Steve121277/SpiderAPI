using SpiderAPI.Spider.Engine;

namespace SpiderAPI.Spider
{
    public static class RuleManager
    {
        private static readonly object ruleLock = new object();

        private static Dictionary<string, PlayEngine> Engines { get; set; } = new Dictionary<string, PlayEngine>();

        internal static PlayEngine NewRule(int Difficulty)
        {
            string ID = Guid.NewGuid().ToString();

            PlayEngine engine = new PlayEngine(ID);

            engine.Difficulty = Difficulty;

            lock (ruleLock)
            {
                Engines.Add(ID, engine);
            }

            return engine;
        }

        internal static PlayEngine? GetEngine(string ID)
        {
            lock(ruleLock)
            {
                PlayEngine? engine = null;

                Engines.TryGetValue(ID, out engine);

                return engine;
            }
        }
    }
}
