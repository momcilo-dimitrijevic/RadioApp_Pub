using System;
using RadioApp.ViewModels;
using RadioApp.Views;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace RadioApp
{
    public partial class App : Application
    {
        public AppBootstrapper AppBootstrapper;

        public App()
        {
            InitializeComponent();

            RxApp.DefaultExceptionHandler = new ObservableExceptionHandler();

            AppBootstrapper = new AppBootstrapper();

            AppBootstrapper
                .Router
                .NavigateAndReset
                .Execute(new HomeViewModel())
                .Subscribe();

            MainPage = new ReactiveUI.XamForms.RoutedViewHost();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
