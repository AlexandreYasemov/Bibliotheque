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
    /// Interaction logic for FilterForm.xaml
    /// </summary>
    public partial class FilterForm : Page
    {

        public event EventHandler FilterAdded;
        public FilterForm(System.Reflection.PropertyInfo[] filterList)
        {
            InitializeComponent();

            foreach (System.Reflection.PropertyInfo item in filterList)
                cb_filters.Items.Add(item.Name);

            cb_filters.SelectedIndex = 0;
        }

        public void AddFilter_Click(object sender, RoutedEventArgs e)
        {
            if (FilterAdded != null)
            {
                FilterAdded(this, EventArgs.Empty);
            }
        }


        public string GetFilter()
        {
            return $"[{cb_filters.SelectedValue}]";
        }

        public string GetText()
        {
            return tb_filter.Text;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
