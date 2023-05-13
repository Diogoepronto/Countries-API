using System.Collections.Generic;
using ProjetoFinal_Paises.Modelos;

namespace ProjetoFinal_Paises.Serviços;

public class ProgressReportService
{
    public int Percentage { get; set; } = 0;

    public List<Country> CountriesDownloaded { get; set; } = new List<Country>();
}
