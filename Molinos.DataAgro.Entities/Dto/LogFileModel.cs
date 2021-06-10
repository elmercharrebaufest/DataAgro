using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class LogFileModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Size { get; set; }
        public DateTime LastWriteTime { get; set; }
    }
}


