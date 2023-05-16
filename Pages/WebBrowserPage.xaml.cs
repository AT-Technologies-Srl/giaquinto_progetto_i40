using Accessibility;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GG40.Pages
{
    /// <summary>
    /// Interaction logic for WebBrowserPage.xaml
    /// </summary>
    public partial class WebBrowserPage : UserControl
    {
        private WebBrowserViewModel _viewModel;

        public WebBrowserPage(string title, string address)
        {
            InitializeComponent();

            DataContext = _viewModel = new WebBrowserViewModel(title, address);

            Loaded += (s, a) => OnLoaded();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnClose?.Invoke();
        }

        private void OnLoaded()
        {
            
        }

        public CallbackHandler OnClose { get; set; }
    }

    public class WebBrowserViewModel : BindableModel
    {
        private string _title;
        private string _address;

        public WebBrowserViewModel(string title, string address)
        {
            _title = title;
            _address = address;
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }
    }
}
