namespace Business.Models
{
    public class Discount
    {
        public Discount(DateTime start, DateTime end, decimal percentReduction)
        {
            PercentReduction = percentReduction;
            Period = new Period(start, end);
        }
        
        public Period Period { get; set; }

        public decimal PercentReduction { get; set; }

        public override string ToString()
        {
            return $"Discount for period {Period.Start:s} - {Period.End:s} PercentReduction:{PercentReduction}";
        }
    }
}
