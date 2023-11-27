using System.Diagnostics;

namespace MicroCommunication.Api;

public static class Observability
{
    // Define a default ActivitySource
    public static readonly ActivitySource DefaultActivities = new ActivitySource("ServiceName");
}
