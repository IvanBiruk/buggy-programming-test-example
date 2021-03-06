# Buggy programming test example
    
## Disclaimer
This repository contains an example of a solution for the billwerk programming test. 
The solution includes errors, is not an ideal algorithm and doesn't follow clean code or any other ideas about code with good quality.
## Task
billwerk is a subscription management platform. One of the tasks billwerk performs is billing subscriptions.

A subscription could be modelled like this:

	public class Subscription
	{
		public DateTime Start { get; set; }
		public DateTime? End { get; set; }
		public decimal PricePerPeriod { get; set; }
	}
`End` is `null` if the subscription has not been cancelled yet.

For simplicity's sake, we assume that the fee period is one day. Thus the price could be 2.00 EUR per day.

As output we want to produce invoice lines. An invoice line could look like this:

	public class InvoiceLine
	{
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		public decimal PricePerPeriod { get; set; }
		public decimal Duration { get; set; } // in periods
		public decimal Total { get; set; }
	}

`Start` and `End` describe the billed interval. The Duration is measured in Periods, so this could be `30` for the `30` day interval between `01. Jan 00:00` and the `31. Jan 00:00`. `Total` is the total price for the billed interval.

Since subscriptions are generally repeatedly billed for fixed time intervals, we limit billing to an end date. If the subscription ends before `billingEnd`, bill the full subscription.
### Problem 1
Complete the following function to produce an invoice line:

	public InvoiceLine BillSubscription(Subscription subscription, DateTime billingEnd)
	{
	}

You can make minor changes to the above classes where appropriate, such as replacing the setter of a property by a computation in the getter.

### Problem 2
To complicate matters we now add time limited discounts.

	public class Discount
	{
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		public decimal PercentReduction { get; set; }
	}

The interval between Start and End specifies when the subscription is cheaper, not when the discount can be redeemed.

There can be several discounts active at the same time. In that case the discount with the highest reduction should be applied to the overlapping time interval.

Complete the following function to produce invoice lines.

    public IEnumerable<InvoiceLine> BillSubscriptionWithDiscounts(Subscription subscription, List<Discount> discounts, DateTime billingEnd)
    {
    }

#### Example:

    var subscription = new Subscription { Start = new DateTime(2017, 03, 03), End = null, PricePerPeriod = 10.00m };
    var discounts = new List<Discount>
    {
        new Discount { Start = new DateTime(2017, 03, 03), End = new DateTime(2017, 03, 17), PercentReduction = 50},
        new Discount { Start = new DateTime(2017, 03, 10, 12, 0, 0), End = new DateTime(2017, 04, 10, 12, 0, 0), PercentReduction = 20},
    };

    var billingEnd = new DateTime(2017, 05, 03);

    var invoiceLines = BillingHelper.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd);

#### Expected output

	03.03.2017 00:00:00 - 17.03.2017 00:00:00 | PricePerPeriod:  5.00 | Duration: 14.0 | Total:  70.00
	17.03.2017 00:00:00 - 10.04.2017 12:00:00 | PricePerPeriod:  8.00 | Duration: 24.5 | Total: 196.00
	10.04.2017 12:00:00 - 03.05.2017 00:00:00 | PricePerPeriod: 10.00 | Duration: 22.5 | Total: 225.00
