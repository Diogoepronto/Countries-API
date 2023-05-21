using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.Serviços;
using Syncfusion.Licensing;

namespace ProjetoFinal_Paises;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MenuArranque : Window
{
    #region Atributos

    private ObservableCollection<Country> CountryList { get; set; } = new();


    private readonly ApiService _apiService;
    private readonly DataService _dataService;

    private DialogService _dialogService;
    private NetworkService _networkService;

    private ICollectionView _dataView;

    #endregion

    public MenuArranque()
    {
        SyncfusionLicenseProvider.RegisterLicense(
            "MjA2Nzc2OUAzMjMxMmUzMjJlMzNHK1UvZmc1TzlONzFJYmdPYW54QTNXZk00ZytVOGtMUmU1eldxcCtZQ21FPQ==");

        InitializeComponent();

        _apiService = new ApiService();
        _dataService = new DataService();
        _networkService = new NetworkService();
        _dialogService = new DialogService();

        NetworkService.AvailabilityChanged +=
            DoAvailabilityChanged;

        InitializeData();

        // listBoxCountries.DataContext = this;
    }

    #region INITIALIZE APPLICATION

    private async void InitializeData()
    {
        var isConnected = await LoadCountries();


        InitializeDataView();


        DownloadFlags();


        if (isConnected)
            TxtStatus.Text =
                string.Format(
                    $"Country list loaded from server: " +
                    $"{DateTime.Now:g}");
        else
            TxtStatus.Text =
                string.Format(
                    $"Country list loaded from internal storage: " +
                    $"{DateTime.Now:g}");


        Close();
    }

    #endregion

    #region INITIALIZE UI

    private void InitializeDataView()
    {
        _dataView = CollectionViewSource.GetDefaultView(CountryList);
        _dataView.SortDescriptions.Add(
            new SortDescription("Name.Common",
                ListSortDirection.Ascending));

        // listBoxCountries.ItemsSource = _dataView;

        // var portugal =
        //     CountryList.FirstOrDefault(c => c.Name.Common == "Portugal");
        // listBoxCountries.SelectedItem = portugal;

        // gridSearchBar.IsEnabled = true;
    }

    #endregion

    #region MANAGE LOCAL DATA

    private async void DownloadFlags()
    {
        var progress = new Progress<int>(percentComplete =>
        {
            TxtStatusDownload.Text =
                $"Downloading flags: {percentComplete}%";
        });

        var downloadflags =
            await _dataService.DownloadFlags(CountryList, progress);

        TxtStatusDownload.Text = downloadflags.Message;

        _dataView.Refresh();

        await Task.Delay(8000);
        TxtStatusDownload.Text = string.Empty;
    }

    #endregion

    #region LOAD COUNTRY DATA

    private async Task<bool> LoadCountries()
    {
        if (!NetworkService.IsNetworkAvailable())
        {
            LoadCountriesLocal();
            return false;
        }

        await LoadCountriesApi();
        return true;
    }

    private void LoadCountriesLocal()
    {
        CountryList =
            (ObservableCollection<Country>)
            DataService.ReadData().Result;

        ProgressBar.Progress = 100;
        TxtProgressStep.Text = "Loading complete";

        Thread.Sleep(100);

        TxtProgressStep.Visibility = Visibility.Hidden;
        ProgressBarOverlay.Visibility = Visibility.Hidden;
    }

    private async Task LoadCountriesApi()
    {
        var progress = new Progress<int>(percentComplete =>
        {
            ProgressBar.Progress = percentComplete;

            TxtProgressStep.Text = percentComplete switch
            {
                25 => "Downloading countries data",
                50 => "Serializing data",
                75 => "Deserializing objects",
                100 => "Loading complete",
                _ => TxtProgressStep.Text
            };
        });

        var response =
            await _apiService.GetCountries(
                "https://restcountries.com",
                "v3.1/all", progress);

        CountryList = (ObservableCollection<Country>) response.Result;

        foreach (var country in CountryList)
            country.Flags.LocalImage =
                Directory.GetCurrentDirectory() +
                @"/Flags/" + $"{country.CCA3}.png";

        await Task.Delay(100);
        TxtProgressStep.Visibility = Visibility.Hidden;
        ProgressBarOverlay.Visibility = Visibility.Hidden;

        DataService.DeleteData();
        DataService.SaveData(CountryList);
    }

    #endregion


    // NETWORK CHECKING METHOD

    private void DoAvailabilityChanged(
        object sender, NetworkStatusChangedArgs e)
    {
        ReportAvailability();
    }

    /// <summary>
    ///     Report the current network availability.
    /// </summary>
    private void ReportAvailability()
    {
        var flagsDownloaded = Directory.GetFiles(@"Flags");

        if (NetworkService.IsAvailable &&
            flagsDownloaded.Length != CountryList.Count)
            Dispatcher.Invoke(() =>
            {
                TxtStatusDownload.Text = string.Empty;

                DownloadFlags();
            });
        else
            Dispatcher.Invoke(() =>
            {
                TxtStatusDownload.Text = "No internet connection";
            });
    }
}