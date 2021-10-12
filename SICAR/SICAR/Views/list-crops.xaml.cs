using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SICAR.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class list_crops : ContentPage
    {

        public IList<Crop> Crops { get; private set; }
        public list_crops()
        {
            InitializeComponent();
            Crops = new List<Crop>();

            Crops.Add(new Crop
            {
                Name = "Tomate",
                Url = "https://cdn2.vectorstock.com/i/thumbs/99/26/tomato-red-vegetable-icon-cartoon-vector-38309926.jpg"
            });


            Crops.Add(new Crop
            {
                Name = "Caña de azúcar",
                Url = "https://cdn5.vectorstock.com/i/thumbs/46/74/sugar-cane-icon-vector-20494674.jpg"
            });

            Crops.Add(new Crop
            {
                Name = "Manzana",
                Url = "https://cdn3.vectorstock.com/i/thumbs/99/52/red-apple-vector-709952.jpg"
            });

            BindingContext = this;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Crop selectedItem = e.SelectedItem as Crop;
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Crop tappedItem = e.Item as Crop;
        }
    }
}