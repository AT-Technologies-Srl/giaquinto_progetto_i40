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
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : UserControl
    {
        public MenuPage()
        {
            InitializeComponent();
        }
    }

    public class MenuItemViewModel : BindableModel
    {
        public string Label { get; set; }
        public MenuItemRoute Route { get; set; }
    }

    public enum MenuItemRoute
    {
        Undefined,
        PlasmaRed,
        SegatriceNastro,
        StazioneSaldante,
        MerloRoto50_26,
        MerloRoto40_18,
        Caterpillar305,
        Caterpillar309,
        GruEffer175,
        GruEffer955,
        AscensoreSalernoPonteggi,
        Impostazioni
    }
}
