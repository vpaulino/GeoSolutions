using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountryApi.Repositories.Models
{

    public class Geometry
    {
        public string type { get; set; }
        public float[][][][] coordinates { get; set; }
    }
}
