using LibMatrix.Homeservers;
using LibMatrix.RoomTypes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jenny;

public class JennyBot(AuthenticatedHomeserverGeneric hs, ILogger<JennyBot> logger, JennyConfiguration configuration) : IHostedService {
    private Task _listenerTask;

    // private GenericRoom _policyRoom;
    private GenericRoom? _logRoom;
    private GenericRoom? _controlRoom;

    /// <summary>Triggered when the application host is ready to start the service.</summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken) {
        _listenerTask = Run(cancellationToken);
        logger.LogInformation("Bot started!");
    }

    private async Task Run(CancellationToken cancellationToken) {
 
    }

    /// <summary>Triggered when the application host is performing a graceful shutdown.</summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public async Task StopAsync(CancellationToken cancellationToken) {
        logger.LogInformation("Shutting down bot!");
    }

}
