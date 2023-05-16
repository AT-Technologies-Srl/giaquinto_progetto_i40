using GG40.Core.Abstraction;
using GG40.Data;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GG40.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();

            DataContext = this;

            dbPass.Password = Configuration.Instance.DataSourcePassword;

            SaveCommand = new Command(arg => Save());
        }

        private async void Save()
        {
            try
            {
                Configuration.Instance.DataSourcePassword = dbPass.Password;

                await Configuration.Instance.Save();

                MessageBox.Show("Configurazione salvata");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnClose?.Invoke();
        }

        public ICommand SaveCommand { get; set; }

        public CallbackHandler OnClose { get; set; }
    }
}
