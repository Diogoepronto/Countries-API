namespace ProjetoFinal_Paises.Serviços;

/// <summary>
///     Define the method signature for network status changes.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
public delegate void NetworkStatusChangedHandler(object sender,
    NetworkStatusChangedArgs e);