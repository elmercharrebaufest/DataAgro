using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ExcelDetallePosicionDto
    {
        public string[] Headers { get; set; }
        public List<string[]> Data { get; set; }
        public string Name { get; set; }
        public string SheetName { get; set; }
    }
}



