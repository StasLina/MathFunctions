using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathFunctions.Models
{
    internal class FunctionSourceData
    {
        public string Formula
        {
            get; set;
        } = "";

        public double XStart { get; set; } = 0;
        public double XEnd { get; set; } = 0;
    }
}
