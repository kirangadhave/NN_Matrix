using NN_Matrix.Main_View;
using NN_Matrix.Main_Window;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NN_Matrix {
    public static class Bootstrapper {

        public static IContainer Container { get; set; }

        public static void Init(IContainer c ) {
            Container = c;
            Container.Configure(x => {
                x.For<IMainWindow>().Use<MainWindow>();
                x.For<IMainWindowViewModel>().Use<MainWindowViewModel>();

                x.For<IMainView>().Use<MainView>();
                x.For<IMainViewModel>().Use<MainViewModel>();
            });
        }

    }
}
