using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_IMC.Models.Entities
{
    public class resMeasuringBodyMass
    {
        public int status { get; set; }
        public string message { get; set; }
        public double fltBodyMass { get; set; }
    }
}