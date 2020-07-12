using Detectify.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Detectify
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FaceList : ContentPage
    {
        public FaceList()
        {
            InitializeComponent();

            Faces.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            {
                if (e.Item == null) return;
                if (sender is ListView lv) lv.SelectedItem = null;
            };
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Navigation.PushAsync(new FaceDetails { BindingContext = ((FacesViewModel)BindingContext).SelectedFace});
        }
        
    }
}