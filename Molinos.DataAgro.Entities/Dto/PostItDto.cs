using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PostItDto
    {
        public int ComercialId { get; set; }
        public string Texto { get; set; }
    }
}