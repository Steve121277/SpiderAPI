using SpiderAPI.Spider.Engine;

namespace SpiderAPI.Spider
{
    public class APIUndoData
    {
        public string ID { get; set; } = "";
        public int DealNext10 { get; set; }
        public List<CardMovement> movement { get; set; } = new List<CardMovement>();
    }
}
