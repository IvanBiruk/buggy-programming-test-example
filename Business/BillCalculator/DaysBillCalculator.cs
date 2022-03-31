using Business.Interfaces;
using Business.Models;

namespace Business.BillCalculator
{
    public class DaysBillCalculator: IBillCalculator
    {
        public Bill CalculateBill(decimal pricePerPeriod, DateTime startDate, DateTime endDate)
        {
            var dateDiff = endDate - startDate;
            var period = Convert.ToDecimal(dateDiff.TotalDays);
            return new Bill(period, period * pricePerPeriod);
        }
    }
}
