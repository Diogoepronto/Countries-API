using ProjetoFinal_Paises.Serviços;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using ProjetoFinal_Paises.Modelos;
using System.DirectoryServices.ActiveDirectory;
using Syncfusion.Licensing;

namespace ProjetoFinal_Paises;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Country> CountryList = new List<Country>();
        private ApiService apiService;
        private DataService dataService;
        private NetworkService networkService;
        private DialogService dialogService;

        #region Validates License

        public void RegisterSyncfusionLicense()
        {
            SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt+QHJqUU1hXk5Hd0BLVGpAblJ3T2ZQdVt5ZDU7a15RRnVfR1xhSHZXckZhUXZXcQ==;Mgo+DSMBPh8sVXJ1S0R+WFpFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF5jTH9Rd01nXXxceXVSRw==;ORg4AjUWIQA/Gnt2VFhiQlVPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSXtSdERrXXtdcn1WRWE=;MjAyNzA3NUAzMjMxMmUzMjJlMzRoSjNjN2VWSzlkSWpBTG5pNEZhaENjVG9Fcy9LV3k0d3I5aTFTdHRQRG00PQ==;MjAyNzA3NkAzMjMxMmUzMjJlMzRQcGdhUVVvRGF1Mmx2MXhZUXlZTkFvV1pjck92WDlmbmhvSEg3UTNOM3hNPQ==;NRAiBiAaIQQuGjN/V0d+Xk9AfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5Wd0diUXtac3dVQWNd;MjAyNzA3OEAzMjMxMmUzMjJlMzRSTHc3MmN6cnJIZXJaRXhjY3lyeW55aVlKb21VanY1enZPWnZsa2Y1dS9FPQ==;MjAyNzA3OUAzMjMxMmUzMjJlMzRtVkNqZStSMTY1VlhuV0ZhNlRaQU03ZmhySUUyV2lZd3l6V0RWbTU2d2NJPQ==;Mgo+DSMBMAY9C3t2VFhiQlVPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSXtSdERrXXtdc3VTQmE=;MjAyNzA4MUAzMjMxMmUzMjJlMzRBaGpHdzI5ZXJPY0dadmNpbFJzaisrOXR1azRNa0JDR2lIT0FhdzNvK2tnPQ==;MjAyNzA4MkAzMjMxMmUzMjJlMzRuUjd3SWJ0andvUEtBaisvMGRoeHZNYXhTQzJRa0Q1MEhndVdZZkErZmVrPQ==;MjAyNzA4M0AzMjMxMmUzMjJlMzRSTHc3MmN6cnJIZXJaRXhjY3lyeW55aVlKb21VanY1enZPWnZsa2Y1dS9FPQ==");
        }

        #endregion
        
        public MainWindow()
        {
            RegisterSyncfusionLicense();
            InitializeComponent();

        //var languageCodes = CultureInfo.GetCultures(CultureTypes.AllCultures)
        //                    .Where(c => c.ThreeLetterISOLanguageName != "")
        //                    .Select(c => c.ThreeLetterISOLanguageName)
        //                    .Distinct()
        //                    .ToList();


        //listBoxTeste.ItemsSource = languageCodes;

        apiService = new ApiService();
        dataService = new DataService();
        networkService = new NetworkService();
        dialogService = new DialogService();

        LoadCountries();
    }

    public async void LoadCountries()
    {
        bool load;

        var connection = networkService.CheckConnection();
        
        if (!connection.IsSuccess)
        {
            LoadCountriesLocal();
            load = false;
        }
        else
        {
            await LoadCountriesApi();
            load = true;
        }

        listBoxTeste.ItemsSource = CountryList;
        listBoxTeste.DisplayMemberPath = "Name.Common";

        KeyValuePair<string, Currency> keyValuePair = new KeyValuePair<string, Currency>();

        //keyValuePair.Value;
        //keyValuePair.Key;
    }

    private void LoadCountriesLocal()
    {
        throw new NotImplementedException();
    }

    private async Task LoadCountriesApi()
    {
        var response = await apiService.GetCountries("https://restcountries.com", "/v3.1/all?fields=name,capital,currencies,region,subregion,continents,population,gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps");

        CountryList = (List<Country>)response.Result;
    }
}
