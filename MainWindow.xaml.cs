using GG40.Data;
using GG40.Core.Abstraction;
using GG40.Data;
using GG40.Pages;
using GG40.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
using System.Diagnostics;

namespace GG40
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _dataContext;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _dataContext = new MainViewModel
            {
                OnPageSelected = menuItem => PageSelected(menuItem)
            };

            Loaded += (sender, args) => OnLoaded();
        }

        private void PageSelected(MenuItemViewModel menuItem)
        {
            dockPanel.Children.Clear();

            if (menuItem == null) return;
            
            switch (menuItem.Route)
            {
                case MenuItemRoute.PlasmaRed:
                    dockPanel.Children.Add(new PlasmaRedPage
                    {
                        OnClose = () => ClosePage()
                    });
                    break;

                case MenuItemRoute.SegatriceNastro:
                    var pathExeSegatriceNastro = Preferences.Instance.GetString("PathExeSegatriceNastro");
                    if (!string.IsNullOrEmpty(pathExeSegatriceNastro))
                    {
                        Process.Start(pathExeSegatriceNastro);
                    }
                    break;

                case MenuItemRoute.StazioneSaldante:
                    dockPanel.Children.Add(new StazioneSaldantePage
                    {
                        OnClose = () => ClosePage()
                    });
                    break;

                case MenuItemRoute.MerloRoto50_26:
                    dockPanel.Children.Add(new WebBrowserPage("SOLLEVATORE MERLO ROTO 50.26S", "https://service.movimatica.com/jsp/vehicles/Map_LOGISTIC.jsp?vehicle=7073")
                    {
                        OnClose = () => ClosePage()
                    });
                    break;

                case MenuItemRoute.MerloRoto40_18:
                    dockPanel.Children.Add(new WebBrowserPage("SOLLEVATORE MERLO ROTO 40.18S", "https://service.movimatica.com/jsp/vehicles/Map_LOGISTIC.jsp?vehicle=6757")
                    {
                        OnClose = () => ClosePage()
                    });
                    break;

                case MenuItemRoute.Caterpillar305:
                    dockPanel.Children.Add(new WebBrowserPage("ESCAVATORE CATERPILLAR 305E2", "http://cgtspa.fleetmaster.it")
                    {
                        OnClose = () => ClosePage()
                    });
                    break;

                case MenuItemRoute.Caterpillar309:
                    dockPanel.Children.Add(new WebBrowserPage("ESCAVATORE CATERPILLAR 309CR", "https://cnew.mechanicallinesolutions.net/pages/home/(mappa//sidebar:listamezzi)")
                    {
                        OnClose = () => ClosePage()
                    });
                    break;

                case MenuItemRoute.GruEffer175:
                    dockPanel.Children.Add(new WebBrowserPage("GRU EFFER 175.2/4S", "https://dataportal.proemion.com/machines/details/159215")
                    {
                        OnClose = () => ClosePage()
                    });
                    break;

                case MenuItemRoute.GruEffer955:
                    dockPanel.Children.Add(new WebBrowserPage("GRU EFFER 955/8S", "https://dataportal.proemion.com/machines/details/159582")
                    {
                        OnClose = () => ClosePage()
                    });
                    break;

                case MenuItemRoute.AscensoreSalernoPonteggi:
                    dockPanel.Children.Add(new WebBrowserPage("ASCENSORE SALERNO PONTEGGI PT2000TD", "http://185.56.8.41/sp.telecontrollo/statomacchina/571")
                    {
                        OnClose = () => ClosePage()
                    });
                    break;

                case MenuItemRoute.Impostazioni:
                    dockPanel.Children.Add(new SettingsPage
                    {
                        OnClose = () => ClosePage()
                    });
                    break;
            }
        }

        private void ClosePage()
        {
            _dataContext.SelectedPage = null;
        }

        private async void OnLoaded()
        {
            try
            {
                var profile = Preferences.Instance.GetString("RunningProfile");

                await Configuration.Instance.Load();

                var plasma = new MenuItemViewModel { Label = "SOITAAB PLASMATECH RED", Route = MenuItemRoute.PlasmaRed };
                var segatrice = new MenuItemViewModel { Label = "SEGATRICE A NASTRO BTM AT303TCT210", Route = MenuItemRoute.SegatriceNastro };
                var saldatrice = new MenuItemViewModel { Label = "ISOLA DI SALDATURA OPM CNROBOT", Route = MenuItemRoute.StazioneSaldante };
                var merlo1 = new MenuItemViewModel { Label = "SOLLEVATORE MERLO ROTO 50.26S", Route = MenuItemRoute.MerloRoto50_26 };
                var merlo2 = new MenuItemViewModel { Label = "SOLLEVATORE MERLO ROTO 40.18S", Route = MenuItemRoute.MerloRoto40_18 };
                var cat1 = new MenuItemViewModel { Label = "ESCAVATORE CATERPILLAR 305E2", Route = MenuItemRoute.Caterpillar305 };
                var cat2 = new MenuItemViewModel { Label = "ESCAVATORE CATERPILLAR 309CR", Route = MenuItemRoute.Caterpillar309 };
                var gru1 = new MenuItemViewModel { Label = "GRU EFFER 175.2/4S", Route = MenuItemRoute.GruEffer175 };
                var gru2 = new MenuItemViewModel { Label = "GRU EFFER 955/8S", Route = MenuItemRoute.GruEffer955 };
                var ascensore = new MenuItemViewModel { Label = "ASCENSORE SALERNO PONTEGGI PT2000TD", Route = MenuItemRoute.AscensoreSalernoPonteggi };

                if ("admin".EqualsIgnoreCase(profile))
                {
                    _dataContext.MenuItems = new List<MenuItemViewModel>
                    {
                        plasma,
                        saldatrice,
                        segatrice,
                        merlo1,
                        merlo2,
                        cat1,
                        cat2,
                        gru1,
                        gru2,
                        ascensore
                    };
                }


                if ("plasma".EqualsIgnoreCase(profile))
                {
                    _dataContext.MenuItems = new List<MenuItemViewModel>
                    {
                        plasma
                    };
                }

                _dataContext.SelectedPage = _dataContext.MenuItems?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class MainViewModel : BaseViewModel
    {
        private List<MenuItemViewModel> _menuItems;
        private MenuItemViewModel _selectedPage;

        public List<MenuItemViewModel> MenuItems
        {
            get => _menuItems;
            set => SetProperty(ref _menuItems, value);
        }

        public MenuItemViewModel SelectedPage
        {
            get => _selectedPage;
            set => SetProperty(ref _selectedPage, value, () => OnPageSelected?.Invoke(_selectedPage));
        }

        public CallbackHandler<MenuItemViewModel> OnPageSelected { get; set; }
        public ICommand OnPageClose { get; set; }
    }
}
