using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NN_Matrix.Models {
    [Serializable]
    public class Component : ModelBase {
        public string Name { get; set; }
        public BindingList<Parameter> Parameters { get; set; }
        public BindingList<Cell> ParamRelationMatrix { get; set; }

        public Component () {
            Parameters = new BindingList<Parameter>();
            ParamRelationMatrix = new BindingList<Cell>();
        }

        public void AddParam (Parameter param) {
            if (!Parameters.Any(x => x.Name == param.Name))
                Parameters.Add(param);
            UpdateMatrix(param);
        }

        void UpdateMatrix (Parameter p) {
            foreach(var param in Parameters) {
                ParamRelationMatrix.Add(new Cell { Param1 = p, Param2 = param, Value = "-" });
            }
        }
    }
}
