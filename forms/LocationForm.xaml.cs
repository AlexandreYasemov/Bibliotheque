using Bibliotheque.DataAccess.DataObjects;
using Bibliotheque.helpers;
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

namespace Bibliotheque.forms
{
    /// <summary>
    /// Interaction logic for LocationForm.xaml
    /// </summary>
    public partial class LocationForm : Page
    {
        Location currentLocation;
        PhpfribiblioContext context;

        public LocationForm()
        {
            context = Globals.databaseContext;

            InitializeComponent();
        }

        public LocationForm(Location location)
        {
            context = Globals.databaseContext;

            currentLocation = location;

            InitializeComponent();

            tb_header.Text = "Modifier lieu";

            FillField(location);
        }

        private void Click_AddModifyLocation(object sender, RoutedEventArgs e)
        {
            if (currentLocation == null)
            {
                currentLocation = new Location();
                context.Add(currentLocation);
                FillData();
            }
            else
            {
                FillData();
                context.Update(currentLocation);
            }

            context.SaveChanges();

            Globals.mainWindow.CloseForm();
        }

        private void FillData()
        {
            currentLocation.City = tb_city.Text;
            currentLocation.StreetName = tb_office.Text;
            currentLocation.RoomName = tb_room.Text;
        }
        private void FillField(Location location)
        {
            tb_city.Text = location.City;
            tb_office.Text = location.StreetName;
            tb_room.Text = location.RoomName;
        }
    }
}
