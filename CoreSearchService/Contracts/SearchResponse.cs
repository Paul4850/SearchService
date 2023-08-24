namespace SearchAPI.Contracts
{
#pragma warning disable CS8618
    public class SearchResponse
    {
        // Mandatory
        // Array of routes
        public Route[] Routes { get; set; }

        // Mandatory
        // The cheapest route
        public decimal MinPrice { get; set; }

        // Mandatory
        // Most expensive route
        public decimal MaxPrice { get; set; }

        // Mandatory
        // The fastest route
        public int MinMinutesRoute { get; set; }

        // Mandatory
        // The longest route
        public int MaxMinutesRoute { get; set; }
    }
#pragma warning restore CS8618
}