using System.Net;
using ProjetoFinal_Paises.Modelos;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal_Paises.Serviços;

public class NetworkService
{
    public static Response CheckConnection()
    {
        var client = new WebClient();

        try
        {
            using (
                client.OpenRead(
                    "http://clients3.google.com/generate_204"))
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = "Há ligação a Internet."
                };
            }
        }
        catch
        {
            return new Response
            {
                IsSuccess = false,
                Message =
                    "Não há ligação a Internet.\n" +
                    "Configure sua ligação à Internet."
            };
        }
    }

    #region Network magic

    /// <summary>
    /// Provides notification of status changes related to Internet-specific network
    /// adapters on this machine.  All other adpaters such as tunneling and loopbacks
    /// are ignored.  Only connected IP adapters are considered.
    /// </summary>
    /// <remarks>
    /// <i>Implementation Note:</i>
    /// <para>
    /// Since we'll likely invoke the IsAvailable property very frequently, that should
    /// be very efficient.  So we wire up handlers for both NetworkAvailabilityChanged
    /// and NetworkAddressChanged and capture the state in the local isAvailable variable. 
    /// </para>
    /// </remarks>

    private static bool _isAvailable;
    private static NetworkStatusChangedHandler _handler;

    //========================================================================================
    // Constructor
    //========================================================================================

    /// <summary>
    /// Initialize the class by detecting the start condition.
    /// </summary>

    static NetworkService()
    {
        _isAvailable = IsNetworkAvailable();
    }


    //========================================================================================
    // Properties
    //========================================================================================

    /// <summary>
    /// This event is fired when the overall Internet connectivity changes.  All
    /// non-Internet adpaters are ignored.  If at least one valid Internet connection
    /// is "up" then we consider the Internet "available".
    /// </summary>

    public static event NetworkStatusChangedHandler AvailabilityChanged
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        add
        {
            if (_handler == null)
            {
                NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(DoNetworkAvailabilityChanged);

                NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(DoNetworkAddressChanged);
            }

            _handler = (NetworkStatusChangedHandler)Delegate.Combine(_handler, value);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        remove
        {
            _handler = (NetworkStatusChangedHandler)Delegate.Remove(_handler, value);

            if (_handler == null)
            {
                NetworkChange.NetworkAvailabilityChanged -= new NetworkAvailabilityChangedEventHandler(DoNetworkAvailabilityChanged);

                NetworkChange.NetworkAddressChanged -= new NetworkAddressChangedEventHandler(DoNetworkAddressChanged);
            }
        }
    }

    /// <summary>
    /// Gets a Boolean value indicating the current state of Internet connectivity.
    /// </summary>

    public static bool IsAvailable
    {
        get { return _isAvailable; }
    }

    //========================================================================================
    // Methods
    //========================================================================================

    /// <summary>
    /// Evaluate the online network adapters to determine if at least one of them
    /// is capable of connecting to the Internet.
    /// </summary>
    /// <returns></returns>

    public static bool IsNetworkAvailable()
    {
        // only recognizes changes related to Internet adapters
        if (NetworkInterface.GetIsNetworkAvailable())
        {
            // however, this will include all adapters
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface face in interfaces)
            {
                // filter so we see only Internet adapters
                if (face.OperationalStatus == OperationalStatus.Up)
                {
                    if ((face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) &&
                        (face.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                    {
                        IPv4InterfaceStatistics statistics = face.GetIPv4Statistics();

                        // all testing seems to prove that once an interface comes online
                        // it has already accrued statistics for both received and sent...

                        if ((statistics.BytesReceived > 0) &&
                            (statistics.BytesSent > 0))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }


    private static void DoNetworkAddressChanged(object sender, EventArgs e)
    {
        SignalAvailabilityChange(sender);
    }


    private static void DoNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
    {
        SignalAvailabilityChange(sender);
    }


    private static void SignalAvailabilityChange(object sender)
    {
        bool change = IsNetworkAvailable();

        if (change != _isAvailable)
        {
            _isAvailable = change;

            if (_handler != null)
            {
                _handler(sender, new NetworkStatusChangedArgs(_isAvailable));
            }
        }
    }

    #endregion
}