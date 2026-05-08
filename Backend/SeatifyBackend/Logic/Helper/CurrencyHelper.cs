using Entities.Models;

namespace Logic.Helper
{
    public static class CurrencyHelper
    {
        public static string ResolveCurrency(EventOccurrence? occurrence, Auditorium? auditorium = null)
        {
            if (occurrence != null)
            {
                if (!string.IsNullOrWhiteSpace(occurrence.CurrencyOverride))
                    return occurrence.CurrencyOverride;

                if (occurrence.Event?.Appearance != null && !string.IsNullOrWhiteSpace(occurrence.Event.Appearance.Currency))
                    return occurrence.Event.Appearance.Currency;

                if (occurrence.Auditorium != null && !string.IsNullOrWhiteSpace(occurrence.Auditorium.Currency))
                    return occurrence.Auditorium.Currency;
            }

            if (auditorium != null && !string.IsNullOrWhiteSpace(auditorium.Currency))
                return auditorium.Currency;

            return "EUR";
        }

        public static string ResolveCurrency(Event? eventEntity, Auditorium? auditorium = null)
        {
            if (eventEntity?.Appearance != null && !string.IsNullOrWhiteSpace(eventEntity.Appearance.Currency))
                return eventEntity.Appearance.Currency;

            if (auditorium != null && !string.IsNullOrWhiteSpace(auditorium.Currency))
                return auditorium.Currency;

            return "EUR";
        }
    }
}
