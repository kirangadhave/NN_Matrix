using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NN_Matrix.Models {
    [Serializable]
    public class Project : ModelBase {
        public BindingList<Component> Components { get; set; }
        public BindingList<Parameter> Parameters { get; set; }
        public string SavePath { get; set; }
        public bool Saved { get; set; }

        public Project () {
            Components = new BindingList<Component>();
            Parameters = new BindingList<Parameter>();
            Saved = false;
        }
    }
}
