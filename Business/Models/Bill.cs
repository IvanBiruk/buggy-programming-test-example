namespace Business.Models
{
    public class Bill
    {
        public Bill(decimal duration, decimal total)
        {
            Duration = duration;
            Total = total;
        }

        public decimal Duration { get; private set; }

        public decimal Total { get;private set; }

    }
}
