using QuotationManagementApp.Models;

namespace QuotationManagementApp.Services
{
    public static class QuoteStatusValidator
    {
        private static readonly Dictionary<QuoteStatus, List<QuoteStatus>> AllowedTransitions =
            new()
            {
                { QuoteStatus.Draft, new() { QuoteStatus.Sent } },
                { QuoteStatus.Sent, new() { QuoteStatus.Viewed, QuoteStatus.Expired } },
                { QuoteStatus.Viewed, new() { QuoteStatus.Accepted, QuoteStatus.Rejected, QuoteStatus.Expired } },
                { QuoteStatus.Accepted, new() },
                { QuoteStatus.Rejected, new() },
                { QuoteStatus.Expired, new() }
            };

        public static bool CanTransition(QuoteStatus current, QuoteStatus next)
        {
            return AllowedTransitions.ContainsKey(current)
                && AllowedTransitions[current].Contains(next);
        }
    }
}