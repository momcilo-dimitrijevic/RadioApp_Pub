using System.Reactive.Disposables;
using System.Reactive.Linq;
using RadioApp.Models;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms.Xaml;

namespace RadioApp.Views.Cells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StationViewCell : ReactiveViewCell<StationModel>
    {
        protected readonly CompositeDisposable SubscriptionDisposables = new CompositeDisposable();

        public StationViewCell()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, x => x.Name, x => x.NameLabel.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.Genre, x => x.GenreLabel.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.BitRate, x => x.BitRateLabel.Text).DisposeWith(disposable);
                this.OneWayBind(ViewModel, x => x.LogoUrl, x => x.LogoImage.Source, x => x).DisposeWith(disposable);
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            SubscriptionDisposables.Clear();
        }
    }
}