namespace BPOI_HUB.model.client
{
    public class Client
    {
        public string? ClientCode { get; set; }
        public string? ClientName { get; set; }

        public Dictionary<string, string>? System { get; set; }

        public string? Division { get; set; }
    }
}
