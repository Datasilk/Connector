namespace Connector.Common
{
    public static class Node
    {
        public static Models.Subscribe Subscribe(string username)
        {
            return new Models.Subscribe()
            {
                key = ""
            };
        }

        public static Models.Subscribed CompleteSubscription(string username, string password)
        {
            return new Models.Subscribed()
            {
                subscribed = true
            };
        }
    }
}
