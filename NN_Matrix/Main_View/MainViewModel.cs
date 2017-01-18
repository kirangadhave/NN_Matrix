using IOC.IOCv2;
using Microsoft.Win32;
using NN_Matrix.Models;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfMvvmLibrary;

namespace NN_Matrix.Main_View {
    public class MainViewModel : ViewModelBase2, IMainViewModel {

        public static MainViewModel This { get; set; }

        public string Title => "N x N Matrix Tool";

        public string FileName => BaseFileName + SaveStatus;

        public string BaseFileName { get; set; }

        public string SaveStatus { get; set; }

        public Project Project { get; set; }
        public Component LinkSelComp { get; set; }
        public Parameter LinkAllParamSel { get; set; }
        public Component MatSelComp { get; set; }
        public DataTable Data { get; set; }

        public MainViewModel(IMainView view, IContainer container) : base(view, container) {
            if (This == null)
                This = this;
            BaseFileName = "";
        }

        public ICommand NewProject => new Commander(o => startNewProject(), o => true);
        public ICommand Add => new Commander(o => addComponent(o as string), o=> { return Project != null; });
        public ICommand SaveProject => new Commander(o => saveProject());
        public ICommand OpenProject => new Commander(o => loadProject());
        public ICommand AddParam => new Commander(o => addParam(o as string));
        public ICommand AddParamToComp => new Commander(o => addPtoC());
        public ICommand Refresh => new Commander(o => refresh());

        void refresh () {
            Data = new DataTable();

            try {
                Data.Columns.Add(new DataColumn { ColumnName = "-" });

                foreach (var p in MatSelComp.Parameters) {
                    var dc = new DataColumn { ColumnName = p.Name };
                    Data.Columns.Add(dc);
                    var dr = Data.NewRow();
                    dr["-"] = p.Name;
                    Data.Rows.Add(dr);
                }

                foreach (var c in MatSelComp.ParamRelationMatrix) {
                    var index = MatSelComp.Parameters.IndexOf(c.Param1);
                    Data.Rows[index][c.Param2.Name] = c.Value;
                }

                Data.RowChanged += Data_RowChanged;
            }catch(Exception ex) { Console.WriteLine(ex); }
            OnPropertyChanged(nameof(Data));
        }

        void Data_RowChanged ( object sender, DataRowChangeEventArgs e ) {
            var p1 = Project.Parameters.FirstOrDefault(x => x.Name == e.Row["-"].ToString());
            if (p1 == null) return;
            foreach(var p in MatSelComp.Parameters) {
                var p2 = Project.Parameters.FirstOrDefault(x => x.Name == p.Name);
                if(p2 != null) {
                    foreach(var c in MatSelComp.ParamRelationMatrix) {
                        if(c.Param1.Name == p1.Name && c.Param2.Name == p2.Name) {
                            c.Value = e.Row[p2.Name].ToString();
                        }
                    }
                }
            }
            SaveStatus = "*";
            OnPropertyChanged(nameof(FileName));
        }

        void addPtoC () {
            if (LinkAllParamSel != null)
                LinkSelComp.AddParam(LinkAllParamSel);
            SaveStatus = "*";
            OnPropertyChanged(nameof(FileName));
        }

        void addParam (string name) {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
                if (!Project.Parameters.Any(x => x.Name == name))
                    Project.Parameters.Add(new Parameter { Name = name });
            OnPropertyChanged(nameof(Project));
            SaveStatus = "*";
            OnPropertyChanged(nameof(FileName));
        }

        void addComponent ( string name ) {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
                if (!Project.Components.Any(x => x.Name == name))
                    Project.Components.Add(new Component { Name = name });
            OnPropertyChanged(nameof(Project));
            SaveStatus = "*";
            OnPropertyChanged(nameof(FileName));
        }

        void saveProject () {
            if (!Project.Saved) {
                var sfd = new SaveFileDialog {
                    DefaultExt = ".nnm"
                };
                sfd.ShowDialog();
                if (sfd.FileName.Length < 2)
                    return;
                using (var stream = File.OpenWrite(sfd.FileName)) {
                    var bf = new BinaryFormatter();
                    bf.Serialize(stream, Project);
                    stream.Close();
                }
                Project.Saved = true;
                Project.SavePath = sfd.FileName;
                BaseFileName = sfd.SafeFileName;
            } else {
                if (Project.SavePath.Length < 2)
                    throw new Exception("Save File Corrupted");
                using (var stream = File.OpenWrite(Project.SavePath)) {
                    var bf = new BinaryFormatter();
                    bf.Serialize(stream, Project);
                    stream.Close();
                }
            }

            SaveStatus = "";
            OnPropertyChanged(nameof(FileName));
        }

        void loadProject () {
            var ofd = new OpenFileDialog();
            ofd.ShowDialog();
            if (ofd.FileName.Length < 2)
                return;
            using(var str = File.OpenRead(ofd.FileName)) {
                var bf = new BinaryFormatter();
                var o = bf.Deserialize(str);
                Project = o as Project;
                if (Project != null) {
                    BaseFileName = ofd.SafeFileName;
                    Project.SavePath = ofd.FileName;
                    OnPropertyChanged(nameof(FileName));
                    OnPropertyChanged(nameof(Project));
                }
            }
        }

        void startNewProject () {
            Project = new Project();
            OnPropertyChanged(nameof(Project));
            BaseFileName = "Untitled";
            SaveStatus = "*";
            OnPropertyChanged(nameof(FileName));
        }

        public void KeyPress(object sender, KeyEventArgs e ) {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) {
                if (e.Key == Key.S)
                    SaveKeyPress(sender, e);
                else if (e.Key == Key.O)
                    OpenKeyPress(sender, e);
            }
        }

        public void SaveKeyPress(object sender, KeyEventArgs e ) {
            if (Project != null)
                if (e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    SaveProject.Execute(null);
        }

        public void OpenKeyPress ( object sender, KeyEventArgs e ) {
            if (e.Key == Key.O && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) {
                if (Project != null) {
                    var dr = MessageBox.Show("Do you want to save current project first?", "", MessageBoxButton.YesNoCancel);
                    if (dr == MessageBoxResult.Cancel)
                        return;
                    if (dr == MessageBoxResult.Yes)
                        SaveProject.Execute(null);
                    OpenProject.Execute(null);
                } else {
                        OpenProject.Execute(null);
                }
            }
        }
    }
}
