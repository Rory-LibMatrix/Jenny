using Microsoft.Extensions.Configuration;

namespace Jenny;

public class JennyConfiguration {
    public JennyConfiguration(IConfiguration config) => config.GetRequiredSection("Jenny").Bind(this);

    public List<string> Admins { get; set; } = new();
}
