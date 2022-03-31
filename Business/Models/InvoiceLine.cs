namespace Business.Models
{
    public class InvoiceLine
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public decimal PricePerPeriod { get; private set; }
        public decimal Duration { get; private set; }
        public decimal Total { get; private set; }

        public InvoiceLine(DateTime start, Bill bill, DateTime onDate, decimal pricePerPeriod)
        {
            Start = start;
            End = onDate;
            PricePerPeriod = pricePerPeriod;
            Total = bill.Total;
            Duration = bill.Duration;
        }


        public override string ToString()
        {
            return $"Invoice line for period {Start:s} - {End:s}({Duration}) PricePerPeriod = {PricePerPeriod} in total to pay : {Total}";
        }
    }
}
