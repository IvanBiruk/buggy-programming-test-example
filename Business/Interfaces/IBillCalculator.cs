using Business.Models;

namespace Business.Interfaces
{
    public interface IBillCalculator
    {
        Bill CalculateBill(decimal pricePerPeriod, DateTime startDate, DateTime endDate);
    }
}
