using GG40.Data;
using GG40.Core.Abstraction;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Microsoft.Win32;
using GG40.Pages;

namespace GG40.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogNuovoProgrammaTaglio.xaml
    /// </summary>
    public partial class DialogNuovoProgrammaTaglio : Window
    {
        private ViewModel _dataContext;

        public DialogNuovoProgrammaTaglio(ProgrammaTaglio programmaTaglio)
        {
            InitializeComponent();

            DataContext = _dataContext = new ViewModel(programmaTaglio);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _dataContext.ProgrammaTaglio.FileAllegato = _dataContext.FileAllegato;
            DialogResult = true;
            Close();
        }

        private void SelectAttachment_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Tutti i file (*.*)|*.*";
            var res = dlg.ShowDialog();
            if (res.HasValue && res.Value)
            {
                _dataContext.FileAllegato = dlg.FileName;
            }
        }
    }

    public class ViewModel : BindableModel
    {
        private string _fileAllegato;

        public ViewModel(ProgrammaTaglio programmaTaglio)
        {
            ProgrammaTaglio = programmaTaglio;
        }

        public ProgrammaTaglio ProgrammaTaglio { get; set; }

        public string FileAllegato
        {
            get => _fileAllegato;
            set => SetProperty(ref _fileAllegato, value);
        }
    }
}
