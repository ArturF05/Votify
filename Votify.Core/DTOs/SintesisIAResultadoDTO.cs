using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.DTOs
{
    public class SintesisIAResultadoDTO
    {
        public string sintesis { get; set; } = "";
        public List<string> pros { get; set; } = new();
        public List<string> contras { get; set; } = new();
    }
}
