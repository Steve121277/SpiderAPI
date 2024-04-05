using SpiderAPI.Spider.Engine;

namespace SpiderAPI.Spider
{
    public class APIMoveData
    {
        public string ID { get; set; } = "";
        public List<CardMovement> movement { get; set; } = new List<CardMovement>();
    }
}
