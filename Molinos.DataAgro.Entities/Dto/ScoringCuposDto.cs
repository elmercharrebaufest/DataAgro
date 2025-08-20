using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ConsultaScoringCuposDto
    {
        public List<ScoringCuposDto> data { get; set; }
    }

    public partial class ScoringCuposDto
    {
        public string score { get; set; }
        public string porcentaje_cumplimiento { get; set; }
        public string score_cluster { get; set; }
        public string cluster { get; set; }
        public string z_cuitda { get; set; }
    }
}
