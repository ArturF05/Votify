using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.API.DTOs
{
    public class Login
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
    }
}
