using GG40.Core.Abstraction;
using GG40.Data;
using GG40.Models;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class StazioneSaldantePage : UserControl
    {
        private StazioneSaldanteViewModel _dataContext;

        public StazioneSaldantePage()
        {
            InitializeComponent();

            DataContext = _dataContext = new StazioneSaldanteViewModel
            {
                RefreshCommand = new Command(arg => Refresh()),
                ApplyFilterCommand = new Command(arg => ApplyFilter())
            };

            Loaded += (sender, args) =>
            {
                BuildChart();
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnClose?.Invoke();
        }

        private async void Refresh()
        {
            lblWait.Visibility = Visibility.Visible;
            try
            {
                await StazioneSaldanteLogService.ImportAll();

                lblWait.Visibility = Visibility.Collapsed;

                ApplyFilter();
            }
            catch (Exception ex)
            {
                lblWait.Visibility = Visibility.Collapsed;

                MessageBox.Show(ex.Message);
            }
        }

        private async void BuildChart()
        {
            lblWait.Visibility = Visibility.Visible;
            try
            {
                var durataProduzioneGiornaliera = await StazioneSaldanteLogService.LoadTotaleProduzioneGiornaliera(_dataContext.DateFrom, _dataContext.DateTo);

                lblWait.Visibility = Visibility.Collapsed;

                _dataContext.DataSource = durataProduzioneGiornaliera;
            }
            catch (Exception ex)
            {
                lblWait.Visibility = Visibility.Collapsed;

                MessageBox.Show(ex.Message);
            }
        }

        private void ApplyFilter()
        {
            BuildChart();
        }

        public CallbackHandler OnClose { get; set; }
    }

    public class StazioneSaldanteViewModel : BindableModel
    {
        private List<GraficoDurataProduzioneGiornalieraStazioneSaldante> _dataSource;
        private SeriesCollection _seriesCollection;
        private string[] _labels;
        private Visibility _plasmaRedReachableVisibility;
        private bool _actionsEnabled;
        private DateTime _dateFrom;
        private DateTime _dateTo;

        public StazioneSaldanteViewModel()
        {
            var dtToday = DateTime.Today;
            _plasmaRedReachableVisibility = Visibility.Collapsed;
            Formatter = value => value.ToString();
            _dateFrom = DateTime.Today.AddDays(-7);
            _dateTo = new DateTime(dtToday.Year, dtToday.Month, dtToday.Day, 23, 59, 59);
        }

        private void OnDataSourceSet()
        {
            var chartValues = new ChartValues<double>();
            var labels = new List<string>();

            foreach (var durata in _dataSource)
            {
                chartValues.Add(durata.TotaleTralicci);
                labels.Add(durata.Data.ToString("dd/MM/yyyy"));
            }

            SeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Totale tralicci",
                        Values = chartValues
                    }
                };

            Labels = labels.ToArray();
        }

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set => SetProperty(ref _seriesCollection, value);
        }

        public string[] Labels
        {
            get => _labels;
            set => SetProperty(ref _labels, value);
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

        public DateTime DateFrom
        {
            get => _dateFrom;
            set => SetProperty(ref _dateFrom, value, () =>
            {
                if (_dateFrom > _dateTo)
                {
                    DateTo = _dateFrom;
                }
            });
        }

        public DateTime DateTo
        {
            get => _dateTo;
            set => SetProperty(ref _dateTo, value.AddDays(1).AddSeconds(-1), () =>
            {
                if (_dateTo < _dateFrom)
                {
                    DateFrom = _dateTo.AddSeconds(1).AddDays(-1);
                }
            });
        }

        public List<GraficoDurataProduzioneGiornalieraStazioneSaldante> DataSource
        {
            get => _dataSource;
            set => SetProperty(ref _dataSource, value, () => OnDataSourceSet());
        }

        public Func<double, string> Formatter { get; set; }

        public ICommand ApplyFilterCommand { get; set; }

        public ICommand RefreshCommand { get; set; }
    }
}
