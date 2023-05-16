using GG40.Data;
using GG40.Core.Abstraction;
using GG40.Dialogs;
using GG40.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Diagnostics;

namespace GG40.Pages
{
    /// <summary>
    /// Interaction logic for ListaTagliSubPage.xaml
    /// </summary>
    public partial class ListaTagliSubPage : UserControl
    {
        private ListaTagliViewModel _dataContext;

        public ListaTagliSubPage()
        {
            InitializeComponent();

            var profile = Preferences.Instance.GetString("RunningProfile");
            DataContext = _dataContext = new ListaTagliViewModel
            {
                ListaTagli = new ObservableCollection<ProgrammaTaglioViewModel>(),
                ActionsVisibility = profile == "ADMIN" ? Visibility.Visible : Visibility.Collapsed,
                ActionsEnabled = true,// _isReachable,
                DeleteCommand = new Command(arg => Delete()),
                AddCommand = new Command(arg => Add()),
                OpenAttachmentCommand = new Command(arg => OpenAttachment((ProgrammaTaglioViewModel)arg))
            };

            Loaded += (sender, args) =>
            {
                Load();
            };
        }

        private async void Load()
        {
            lblWait.Visibility = Visibility.Visible;
            try
            {
                var listaTagli = await ProgrammaTaglioService.FindAllAsync();

                _dataContext.ListaTagli = new ObservableCollection<ProgrammaTaglioViewModel>(listaTagli.Select(t => new ProgrammaTaglioViewModel(t)));

                lblWait.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                lblWait.Visibility = Visibility.Collapsed;

                MessageBox.Show(ex.Message);
            }
        }

        private async void Delete()
        {
            //if (!_isReachable)
            //{
            //    MessageBox.Show("Attenzione, macchina non raggiungibile");
            //    return;
            //}

            var listDel = _dataContext.ListaTagli.Where(t => t.IsSelected).ToList();
            if (listDel.Count <= 0) return;

            lblWait.Visibility = Visibility.Visible;
            try
            {
                var conn = new DatabaseContext();

                Assembly curAssembly = Assembly.GetExecutingAssembly();
                var attachmentsLocation = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(curAssembly.Location), "Attachments");
                
                foreach (var toDel in listDel)
                {
                    var filePath = System.IO.Path.Combine(attachmentsLocation, toDel.ProgrammaTaglio.FileAllegato);
                    if (File.Exists(filePath)) File.Delete(filePath);

                    //filePath = System.IO.Path.Combine(Configuration.Instance.PlasmaRedNpgDir, toDel.ProgrammaTaglio.NomeProgramma);
                    //if (File.Exists(filePath)) File.Delete(filePath);

                    conn.Remove(toDel.ProgrammaTaglio);

                    _dataContext.ListaTagli.Remove(toDel);
                }

                conn.SaveChanges();

                lblWait.Visibility = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                lblWait.Visibility = Visibility.Collapsed;

                MessageBox.Show(e.Message);
            }
        }

        private async void Add()
        {
            //if (!_isReachable)
            //{
            //    MessageBox.Show("Attenzione, macchina non raggiungibile");
            //    return;
            //}

            var dlg = new OpenFileDialog();
            dlg.Filter = "File .npg (*.npg)|*.npg";
            var res = dlg.ShowDialog();
            if (res.HasValue && res.Value)
            {
                var fileName = DateTime.Now.ToString("yyMMddHHmmss") + "_" + System.IO.Path.GetFileName(dlg.FileName);

                var taglio = new ProgrammaTaglio 
                { 
                    Id = Guid.NewGuid().ToString(),
                    DataInserimento = DateTime.Now,
                    FileAllegato = "",
                    Descrizione = "",
                    NomeProgramma = fileName
                };

                var dlgData = new DialogNuovoProgrammaTaglio(taglio);
                dlgData.Owner = System.Windows.Window.GetWindow(this);
                res = dlgData.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    lblWait.Visibility = Visibility.Visible;
                    try
                    {
                        var path = System.IO.Path.Combine(Configuration.Instance.PlasmaRedNpgDir, fileName);
                        File.Copy(dlg.FileName, path, true);

                        if (!string.IsNullOrEmpty(taglio.FileAllegato))
                        {
                            Assembly curAssembly = Assembly.GetExecutingAssembly();
                            var attachmentsLocation = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(curAssembly.Location), "Attachments");
                            if (!Directory.Exists(attachmentsLocation))
                            {
                                Directory.CreateDirectory(attachmentsLocation);
                            }
                            
                            var fileExt = System.IO.Path.GetExtension(taglio.FileAllegato);
                            fileName = Guid.NewGuid().ToString();
                            fileName = System.IO.Path.ChangeExtension(fileName, fileExt);
                            var savePath = System.IO.Path.Combine(attachmentsLocation, fileName);
                            File.Copy(taglio.FileAllegato, savePath);
                            taglio.FileAllegato = fileName;
                        }

                        var conn = new DatabaseContext();
                        conn.Add(taglio);
                        conn.SaveChanges();

                        _dataContext.ListaTagli.Add(new ProgrammaTaglioViewModel(taglio));

                        lblWait.Visibility = Visibility.Collapsed;
                    }
                    catch (Exception ex)
                    {
                        lblWait.Visibility = Visibility.Collapsed;

                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void OpenAttachment(ProgrammaTaglioViewModel arg)
        {
            Assembly curAssembly = Assembly.GetExecutingAssembly();
            var attachmentsLocation = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(curAssembly.Location), "Attachments");
            var filePath = System.IO.Path.Combine(attachmentsLocation, arg.ProgrammaTaglio.FileAllegato);
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Attenzione, il file non è più disponibile");
                return;
            }

            try
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo(filePath)
                    {
                        UseShellExecute = true
                    }
                }.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void NotifyRefresh()
        {
            Load();
        }
    }

    public class ListaTagliViewModel : BindableModel
    {
        private ObservableCollection<ProgrammaTaglioViewModel> _listaTagli;
        private Visibility _plasmaRedReachableVisibility;
        private bool _actionsEnabled;

        public ListaTagliViewModel()
        {
            _plasmaRedReachableVisibility = Visibility.Collapsed;
        }

        public ObservableCollection<ProgrammaTaglioViewModel> ListaTagli
        {
            get => _listaTagli;
            set => SetProperty(ref _listaTagli, value);
        }

        public bool ActionsEnabled
        {
            get => _actionsEnabled;
            set => SetProperty(ref _actionsEnabled, value);
        }

        public Visibility PlasmaRedReachableVisibility
        {
            get => _plasmaRedReachableVisibility;
            set => SetProperty(ref _plasmaRedReachableVisibility, value);
        }

        public Visibility ActionsVisibility { get; set; }
        
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand OpenAttachmentCommand { get; set; }
    }

    public class ProgrammaTaglioViewModel : BindableModel
    {
        private bool _isSelected;

        public ProgrammaTaglioViewModel(ProgrammaTaglio programmaTaglio)
        {
            ProgrammaTaglio = programmaTaglio;
            IsMovedToMachine = !File.Exists(System.IO.Path.Combine(Configuration.Instance.PlasmaRedNpgDir, programmaTaglio.NomeProgramma));
            OpenAttachmentVisibility = !string.IsNullOrEmpty(programmaTaglio.FileAllegato) ? Visibility.Visible : Visibility.Collapsed;
        }

        public ProgrammaTaglio ProgrammaTaglio { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public bool IsMovedToMachine { get; set; }

        public Visibility OpenAttachmentVisibility { get; set; }
    }
}
