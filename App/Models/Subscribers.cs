namespace Connector.Models.Subscribers
{
    public enum IsSubscribed
    {
        undecided = 0,
        accepted = 1,
        declined = -1
    }

    /// <summary>
    /// Represents whether or not a local user 
    /// approved or declined a subscription request
    /// </summary>
    public class Subscribed
    {
        public IsSubscribed action { get; set; }
    }
}
