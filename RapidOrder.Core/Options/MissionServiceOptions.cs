namespace RapidOrder.Core.Options;

public class MissionServiceOptions
{
    /// <summary>
    /// When true, finishing an ORDER mission automatically creates a SERVE mission for the assigned user/place.
    /// </summary>
    public bool TrackServeMission { get; set; } = false;
}
