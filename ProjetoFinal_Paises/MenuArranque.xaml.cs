using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.Serviços;
using Syncfusion.Licensing;

namespace ProjetoFinal_Paises;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MenuArranque : Window
{
    public MenuArranque()
    {
        // chaves que já não funcionam
        // Diogo
        // SyncfusionLicenseProvider.RegisterLicense("MjA2Nzc2OUAzMjMxMmUzMjJlMzNHK1UvZmc1TzlONzFJYmdPYW54QTNXZk00ZytVOGtMUmU1eldxcCtZQ21FPQ==");
        // Nuno
        SyncfusionLicenseProvider.RegisterLicense(
            "MjEyMzA1NEAzMjMxMmUzMjJlMzVtcEV4dGZ1Y0dJNnhtN0xNQWR1cHgxcXM3ZTFBRHZ0T21iOThpdVFoYm1RPQ==");


        InitializeComponent();

        _apiService = new ApiService();
        _dataService = new DataService();
        _networkService = new NetworkService();
        _dialogService = new DialogService();

        NetworkService.AvailabilityChanged += DoAvailabilityChanged;

        InitializeData();
    }


    #region INITIALIZE APPLICATION

    private async Task InitializeData()
    {
        var isConnected = await LoadCountries();

        InitializeDataView();

        DownloadFlags();

        txtStatus.Text = string.Format(isConnected
            ? $"Country list loaded from server: {DateTime.Now:g}"
            : $"Country list loaded from internal storage: {DateTime.Now:g}");


        // Aguarde 2 segundo(s)
        Thread.Sleep(2000);
        Close();
    }

    #endregion

    #region INITIALIZE UI

    private void InitializeDataView()
    {
        _dataView =
            CollectionViewSource.GetDefaultView(
                CountryList ??
                new ObservableCollection<Country>());

        _dataView.SortDescriptions.Add(
            new SortDescription("Name.Common",
                ListSortDirection.Ascending));
    }

    #endregion

    #region MANAGE LOCAL DATA

    private async void DownloadFlags()
    {
        var progress = new Progress<int>(percentComplete =>
        {
            txtStatusDownload.Text =
                $"Downloading flags: {percentComplete}%";
        });

        var downloadflags =
            await _dataService.DownloadFlags(CountryList, progress);

        txtStatusDownload.Text = downloadflags.Message;

        _dataView.Refresh();

        await Task.Delay(2000);
        txtStatusDownload.Text = string.Empty;
    }

    #endregion

    #region Propriedades

    private readonly ApiService _apiService;
    private readonly DataService _dataService;
    private DialogService _dialogService;
    private NetworkService _networkService;

    private ICollectionView? _dataView;

    private ObservableCollection<Country>? CountryList { get; set; } = new();

    #endregion

    #region LOAD COUNTRY DATA

    private async Task<bool> LoadCountries()
    {
        var resultado = NetworkService.IsNetworkAvailable();

        // resultado = false;
        if (!resultado)
        {
            LoadCountriesLocal();

            txtStatus.Text = Information.TextoStatus =
                $"Country list loaded from internal storage: " +
                $"{DateTime.Now:g}";

            Information.TextoIsSuccess = false.ToString();
            Information.APIorDB = false;

            return await Task.FromResult(false);
        }
        else
        {
            await LoadCountriesApi();
            // LoadCountriesLocal();

            txtStatus.Text = Information.TextoStatus =
                $"Country list loaded from server: {DateTime.Now:g}";

            Information.TextoIsSuccess = true.ToString();
            Information.APIorDB = true;
            
            return await Task.FromResult(true);
        }
    }

    private void LoadCountriesLocal()
    {
        CountryList =
            DataService.ReadData()?.Result as ObservableCollection<Country>;
        CountriesList.Countries = CountryList;

        progressBar.Progress = 100;
        txtProgressStep.Text = "Loading complete";

        Thread.Sleep(100);

        txtProgressStep.Visibility = Visibility.Hidden;
        progressBarOverlay.Visibility = Visibility.Hidden;
    }

    private async Task LoadCountriesApi()
    {
        var progress = new Progress<int>(percentComplete =>
        {
            progressBar.Progress = percentComplete;

            txtProgressStep.Text = percentComplete switch
            {
                25 => "Downloading countries data",
                50 => "Serializing data",
                75 => "Deserializing objects",
                100 => "Loading complete",
                _ => Information.TextoProgressSteps = txtProgressStep.Text
            };
        });

        var response =
            await _apiService.GetCountries(
                "https://restcountries.com",
                "v3.1/all", progress);

        CountryList = response.Result as ObservableCollection<Country>;

        if (CountryList != null)
        {
            foreach (var country in CountryList)
                country.Flags.LocalImage =
                    Directory.GetCurrentDirectory() +
                    @"/Flags/" + $"{country.CCA3}.png";


            await Task.Delay(100);
            txtProgressStep.Visibility = Visibility.Hidden;
            progressBarOverlay.Visibility = Visibility.Hidden;

            DataService.DeleteData();
            DataService.SaveData(CountryList);
        }
    }

    #endregion


    #region NETWORK_CHECKING_METHOD

    // NETWORK CHECKING METHOD

    private void DoAvailabilityChanged(object sender,
        NetworkStatusChangedArgs e)
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
                txtStatusDownload.Text = string.Empty;

                DownloadFlags();
            });
        else
            Dispatcher.Invoke(() =>
            {
                txtStatusDownload.Text = "No internet connection";
            });
    }

    #endregion
}