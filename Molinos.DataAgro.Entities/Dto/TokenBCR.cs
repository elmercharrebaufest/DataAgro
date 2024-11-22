namespace Molinos.DataAgro.Entities.Dto
{
    public partial class TokenBCR
    {
        public Status Status { get; set; }
        public Metadata Metadata { get; set; }
        public Data Data { get; set; }
    }

    public class Status
    {
        public string Code { get; set; }
        public string StatusString { get; set; }
    }

    public class Metadata { }

    public class Data
    {
        public string Token { get; set; }
    }

}
