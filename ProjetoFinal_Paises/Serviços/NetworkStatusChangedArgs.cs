namespace ProjetoFinal_Paises.Serviços;

/// <summary>
///     Describes the overall network connectivity of the machine.
/// </summary>
public class NetworkStatusChangedArgs
{
    /// <summary>
    ///     Instantiate a new instance with the given availability.
    /// </summary>
    /// <param name="isAvailable"></param>
    public NetworkStatusChangedArgs(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }

    /// <summary>
    ///     Gets a Boolean value indicating the current state of Internet connectivity.
    /// </summary>
    public bool IsAvailable { get; }
}

//********************************************************************************************
// delegate NetworkStatusChangedHandler
//********************************************************************************************