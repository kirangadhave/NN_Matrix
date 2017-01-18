using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NN_Matrix.Models {
    [Serializable]
    public class Cell : ModelBase {
        public Parameter Param1 { get; set; }
        public Parameter Param2 { get; set; }
        public string Value { get; set; }
    }
}
