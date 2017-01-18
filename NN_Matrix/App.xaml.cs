using NN_Matrix.Main_Window;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NN_Matrix {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public App () {
            Bootstrapper.Init(new Container());
        }

        protected override void OnStartup ( StartupEventArgs e ) {
            MainWindow = (Main_Window.MainWindow)Bootstrapper.Container.GetInstance<IMainWindowViewModel>().Window;
            MainWindow.Show();
        }
    }
}
