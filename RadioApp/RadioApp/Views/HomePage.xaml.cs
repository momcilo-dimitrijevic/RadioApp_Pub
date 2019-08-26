using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using RadioApp.Models;
using RadioApp.ViewModels;
using RadioApp.Views.Base;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RadioApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : BaseContentPage<HomeViewModel>
	{
		public HomePage ()
		{
			InitializeComponent ();

		    this.WhenActivated(disposable =>
		    {
		        this.Bind(ViewModel, x => x.SearchTerm, x => x.StationSearch.Text).DisposeWith(disposable);
		        this.OneWayBind(ViewModel, x => x.BindingData, x => x.StationsList.ItemsSource).DisposeWith(disposable);
		        this.OneWayBind(ViewModel, x => x.IsBusy, x => x.ActivityIndicator.IsRunning).DisposeWith(disposable);
		        this.OneWayBind(ViewModel, x => x.PlaybackStatus, x => x.TitleStatus.Text, x => x.ToString()).DisposeWith(disposable);
            });
        }

	    private void StationsList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
	    {
	        var selectedStation = (StationModel) e.SelectedItem;
	        this.ViewModel.PlayCommand.Execute(selectedStation.M3Url);
	    }
	}
}