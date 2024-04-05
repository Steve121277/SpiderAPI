using SpiderAPI.Spider.Engine;

namespace SpiderAPI.Spider
{
    public class APIStackData
    {
        public string ID { get; set; } = "";
        public int DealNext10 { get; set; }

        //public int[]? DCK { get; set; } = null;
        public List<CardMovement> movement { get; set; } = new List<CardMovement>();
    }
}
