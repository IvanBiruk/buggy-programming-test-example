using Business.Interfaces;
using Business.Models;

namespace Business.Services
{
    public class BillManager
    {
        private readonly IBillCalculator _calculator;

        public BillManager(IBillCalculator calculator)
        {
            _calculator = calculator;
        }
        /// <summary>
        /// Problem 2
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="billingEnd"></param>
        /// <returns></returns>
        public InvoiceLine BillSubscription(Subscription subscription, DateTime billingEnd)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            if (billingEnd == DateTime.MinValue || billingEnd < subscription.Start)
            {
                throw new ArgumentOutOfRangeException(nameof(billingEnd));
            }

            var endDate = GetBillEndDate(subscription, billingEnd);
            return CreateInvoice(subscription.Start, endDate, subscription.PricePerPeriod);
        }

        /// <summary>
        /// Problem 2
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="discounts"></param>
        /// <param name="billingEnd"></param>
        /// <returns></returns>
        public IEnumerable<InvoiceLine> BillSubscriptionWithDiscounts(Subscription subscription, List<Discount> discounts, DateTime billingEnd)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            if (discounts == null)
            {
                throw new ArgumentNullException(nameof(discounts)); 
            }
            if (billingEnd == DateTime.MinValue || billingEnd < subscription.Start)
            {
                throw new ArgumentOutOfRangeException(nameof(billingEnd));
            }

            var endDate = GetBillEndDate(subscription, billingEnd);
            
            var discountsForTimeFrame = discounts.Where(discount => discount.Period.Start < endDate && subscription.Start < discount.Period.End);
            if (discountsForTimeFrame.Any() == false)
            {
                return new List<InvoiceLine> {BillSubscription(subscription, billingEnd)};
            }

            var orderDiscounts = NormalizeDiscounts(new Period(subscription.Start, endDate), discountsForTimeFrame.ToList());

            return orderDiscounts.Select(x => GetDiscountedInvoice(x.Period.Start, subscription, x));
        }

        private InvoiceLine GetDiscountedInvoice(DateTime startDate,Subscription subscription, Discount discount)
        {
            var pricePerPeriod = subscription.PricePerPeriod - subscription.PricePerPeriod * (discount.PercentReduction / 100);
            var discountBillEndDate = GetBillEndDate(subscription, discount.Period.End);
            return CreateInvoice(startDate, discountBillEndDate, pricePerPeriod);
        }
        
        private Bill GetBill(DateTime start, DateTime endDate, decimal pricePerPeriod)
        {
            var isIncorrectSubscription = start == endDate ;

            return isIncorrectSubscription
                ? new Bill(0, 0)
                : _calculator.CalculateBill(pricePerPeriod, start, endDate);
        }

        private InvoiceLine CreateInvoice(DateTime billStart, DateTime endDate, decimal pricePerPeriod)
        {
            var bill = GetBill(billStart, endDate, pricePerPeriod);

            return new InvoiceLine(billStart, bill, endDate, pricePerPeriod);
        }

        private static DateTime GetBillEndDate(Subscription subscription, DateTime billingEnd)
        {
            var endDate = subscription.End.HasValue == false || subscription.End.Value >= billingEnd
                ? billingEnd
                : subscription.End.Value;
            return endDate;
        }

        private List<Discount> NormalizeDiscounts(Period period, List<Discount> discounts, decimal maxDiscount = decimal.MaxValue)
        {
            var res = new List<Discount>();
            
            var overlapped = discounts.Where(x => x.Period.OverlapsWith(period) 
                && x.PercentReduction<maxDiscount);

            if (overlapped.Any() == false)
            {
                if (period.IsMoment == false)
                {
                    res.Add(new Discount(period.Start, period.End, 0));
                }
                
                return res;
            }

            var max = overlapped.Max(x => x.PercentReduction);

            var periodsWithMax = overlapped.Where(x => x.PercentReduction == max);

            do
            {
                var discount = periodsWithMax.First();
                var overlaps = periodsWithMax
                    .Where(x => x.Period.OverlapsWith(discount.Period));

                if (overlaps.Any())
                {
                    discount.Period = new Period(periodsWithMax.Min(x => x.Period.Start), overlaps.Max(x => x.Period.End));

                    foreach (var overlap in overlaps.ToList())
                    {
                        discounts.Remove(overlap);
                    }
                }

                if (discount.Period.Start < period.Start)
                {
                    discounts.Add(new Discount(discount.Period.Start, period.Start, discount.PercentReduction));
                    discount.Period.Setup(period.Start, discount.Period.End);
                }
                if (discount.Period.End > period.End)
                {
                    discounts.Add(new Discount(period.End, discount.Period.End, discount.PercentReduction));
                    discount.Period.Setup(discount.Period.Start, period.End);
                }

                res.Add(discount);

                var before = discount.Period.Start > period.Start
                    ? new Period(period.Start, discount.Period.Start)
                    : new Period(period.Start);
                
                res.AddRange(NormalizeDiscounts(before, discounts, discount.PercentReduction));

                var after = new Period(discount.Period.End, period.End);
                res.AddRange(NormalizeDiscounts(after, discounts));

            } while (periodsWithMax.Any());
            
            return res.Where(x=>x.Period.IsMoment == false).OrderBy(x=>x.Period.Start).ToList();
        }
    }
}
