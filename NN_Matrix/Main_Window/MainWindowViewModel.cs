using IOC.IOCv2;
using NN_Matrix.Main_View;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NN_Matrix.Main_Window {
    public class MainWindowViewModel : WindowViewModelBase2, IMainWindowViewModel {

        public string Title => MainViewModel.This.Title;

        public MainWindowViewModel(IMainWindow window, IContainer container) : base(window, container) {
            ShowView<IMainViewModel>();
        }

    }
}
