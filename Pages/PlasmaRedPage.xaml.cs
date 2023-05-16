using GG40.Core.Abstraction;
using GG40.Data;
using GG40.Support;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for PlasmaRedPage.xaml
    /// </summary>
    public partial class PlasmaRedPage : UserControl
    {
        private PlasmaRedViewModel _viewModel;
        private UIElement _currentPage;

        public PlasmaRedPage()
        {
            InitializeComponent();

            DataContext = _viewModel = new PlasmaRedViewModel();

            Loaded += (sender, arg) =>
            {
                OnLoaded();
            };
        }

        private async void OnLoaded()
        {
            _viewModel.IsReachable = await Network.PingAddress(Configuration.Instance.PlasmaRedIndirizzoIp);

            dockPanel.Children.Clear();

            _currentPage = new ListaTagliSubPage();

            dockPanel.Children.Add(_currentPage);

            btnPogrammi.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eee"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnClose?.Invoke();
        }

        private void StartCadCam_Click(object sender, RoutedEventArgs e)
        {
            var pathExePlasmaCadCam = Preferences.Instance.GetString("PathExePlasmaCadCam");
            if (!string.IsNullOrEmpty(pathExePlasmaCadCam))
            {
                Process.Start(pathExePlasmaCadCam);
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            //if (!_isReachable)
            //{
            //    MessageBox.Show("Attenzione, macchina non raggiungibile");
            //    return;
            //}

            //lblWait.Visibility = Visibility.Visible;
            try
            {
                await PlasmaRedLogService.ImportAll();

                //lblWait.Visibility = Visibility.Collapsed;

                if (_currentPage is ListaTagliSubPage)
                {
                    ((ListaTagliSubPage)_currentPage).NotifyRefresh();
                }
                else if (_currentPage is StatistichePlasmaRedPage)
                {
                    ((StatistichePlasmaRedPage)_currentPage).NotifyRefresh();
                }
            }
            catch (Exception ex)
            {
                //lblWait.Visibility = Visibility.Collapsed;

                MessageBox.Show(ex.Message);
            }
        }

        private void Statistiche_Click(object sender, RoutedEventArgs e)
        {
            btnPogrammi.Background = new SolidColorBrush(Colors.Transparent);
            btnStatistiche.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eee"));

            if (dockPanel.Children.Cast<object>().Where(e => e.GetType() == typeof(StatistichePlasmaRedPage)).Count() == 0)
            {
                dockPanel.Children.Clear();

                dockPanel.Children.Add(new StatistichePlasmaRedPage());
            }
        }

        private void ProgrammiTaglio_Click(object sender, RoutedEventArgs e)
        {
            btnStatistiche.Background = new SolidColorBrush(Colors.Transparent);
            btnPogrammi.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eee"));

            if (dockPanel.Children.Cast<object>().Where(e => e.GetType() == typeof(ListaTagliSubPage)).Count() == 0)
            {
                dockPanel.Children.Clear();

                dockPanel.Children.Add(new ListaTagliSubPage());
            }
        }

        public CallbackHandler OnClose { get; set; }
    }

    public class PlasmaRedViewModel : BindableModel
    {
        private SolidColorBrush _statusColor;
        private bool _isReachable;

        public PlasmaRedViewModel()
        {
            var profile = Preferences.Instance.GetString("RunningProfile");
            _statusColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            ActionsVisibility = /*"admin".EqualsIgnoreCase(profile) ? Visibility.Visible : */Visibility.Collapsed;
            RefreshVisibility = Visibility.Visible;
        }

        public bool IsReachable
        {
            get => _isReachable;
            set => SetProperty(ref _isReachable, value, () =>
            {
                StatusColor = _isReachable 
                ? new SolidColorBrush(Color.FromRgb(0, 255, 0))
                : new SolidColorBrush(Color.FromRgb(255, 0, 0));
            });
        }

        public SolidColorBrush StatusColor
        {
            get => _statusColor;
            set => SetProperty(ref _statusColor, value);
        }

        public Visibility ActionsVisibility { get; private set; }
        public Visibility RefreshVisibility { get; private set; }
    }
}
