namespace Business.Models
{
    public class Subscription
    {
        public DateTime Start { get; private set; }
        public DateTime? End { get; set; }

        public decimal PricePerPeriod { get; set; }

        public Period Period { get; }

        public Subscription(DateTime start, DateTime? end, decimal pricePerPeriod)
        {
            Start = start;
            End = end;
            PricePerPeriod = pricePerPeriod;
            Period = End.HasValue ? 
                new Period(Start, End.Value) : 
                new Period(Start,DateTime.MaxValue);
        }
    }
}
