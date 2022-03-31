using System;
using System.Collections.Generic;
using System.Linq;
using Business.BillCalculator;
using Business.Models;
using Business.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class DiscountTests
    {
        private BillManager _billManager;

        [TestInitialize]
        public void Init()
        {
            _billManager = new BillManager(new DaysBillCalculator()); //todo: change special calculator to mock
        }


        [TestMethod]
        public void NotOverlappedDiscount()
        {
            var subscription = new Subscription(new DateTime(2017, 05, 12), null, 10.00m);
            var discounts = new List<Discount>
            {
                new Discount(new DateTime(2017, 03, 03),new DateTime(2017, 03, 14),50),
                new Discount(new DateTime(2017, 03, 16),new DateTime(2017, 04, 10, 00, 0, 0),20),
                new Discount(new DateTime(2017, 04, 10, 00, 0, 0),new DateTime(2017, 5, 03),30)
            };
            var billingEnd = new DateTime(2017, 05, 17);


            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd);

            Assert.IsTrue(invoiceLines.Count()==1);
            var invoice = invoiceLines.First();

            Assert.IsTrue(invoice.Start == subscription.Start);
            Assert.IsTrue(invoice.End == billingEnd);
            Assert.IsTrue(invoice.Total == 50);
        }

        [TestMethod]
        public void OneDiscountInMiddleOfPeriod()
        {
            var subscription = new Subscription(new DateTime(2017, 3, 3), null, 10.00m);
            var discount = new Discount(new DateTime(2017, 03, 04), new DateTime(2017, 03, 14), 50);
            var discounts = new List<Discount>{discount};
            var billingEnd = new DateTime(2017, 3, 15);


            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd).ToList();

            Assert.IsTrue(invoiceLines.Count == 3);

            Assert.IsTrue(invoiceLines[0].Start == subscription.Start);
            Assert.IsTrue(invoiceLines[0].End == discount.Period.Start);
            Assert.IsTrue(invoiceLines[0].Total == 10);

            Assert.IsTrue(invoiceLines[1].Start == discount.Period.Start);
            Assert.IsTrue(invoiceLines[1].End == discount.Period.End);
            Assert.IsTrue(invoiceLines[1].Total == 50);

            Assert.IsTrue(invoiceLines[2].Start == discount.Period.End);
            Assert.IsTrue(invoiceLines[2].End == billingEnd);
            Assert.IsTrue(invoiceLines[2].Total == 10);
        }

        [TestMethod]
        public void OneDiscountStartTouchingfPeriod()
        {
            var subscription = new Subscription(new DateTime(2017, 3, 3), null, 10.00m);
            var discount = new Discount(new DateTime(2017, 03, 03), new DateTime(2017, 03, 14), 50);
            var discounts = new List<Discount> { discount };
            var billingEnd = new DateTime(2017, 3, 15);


            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd).ToList();

            Assert.IsTrue(invoiceLines.Count == 2);
            
            Assert.IsTrue(invoiceLines[0].Start == discount.Period.Start);
            Assert.IsTrue(invoiceLines[0].End == discount.Period.End);
            Assert.IsTrue(invoiceLines[0].Total == 55);

            Assert.IsTrue(invoiceLines[1].Start == discount.Period.End);
            Assert.IsTrue(invoiceLines[1].End == billingEnd);
            Assert.IsTrue(invoiceLines[1].Total == 10);
        }

        [TestMethod]
        public void OneDiscountEndTouchingfPeriod()
        {
            var subscription = new Subscription(new DateTime(2017, 3, 3), null, 10.00m);
            var discount = new Discount(new DateTime(2017, 03, 05), new DateTime(2017, 03, 15), 50);
            var discounts = new List<Discount> { discount };
            var billingEnd = new DateTime(2017, 3, 15);


            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd).ToList();

            Assert.IsTrue(invoiceLines.Count == 2);

            Assert.IsTrue(invoiceLines[0].Start == subscription.Start);
            Assert.IsTrue(invoiceLines[0].End == discount.Period.Start);
            Assert.IsTrue(invoiceLines[0].Total == 20);

            Assert.IsTrue(invoiceLines[1].Start == discount.Period.Start);
            Assert.IsTrue(invoiceLines[1].End == billingEnd);
            Assert.IsTrue(invoiceLines[1].Total == 50);
        }

        [TestMethod]
        public void TwoDiscountsWithScippedPeriodInTheMiddle()
        {
            var subscription = new Subscription(new DateTime(2017, 3, 1), null, 10.00m);
            var discount = new Discount(new DateTime(2017, 03, 05), new DateTime(2017, 03, 10), 50);
            var discount2 = new Discount(new DateTime(2017, 03, 15), new DateTime(2017, 03, 20), 40);
            var discounts = new List<Discount> { discount,discount2 };
            var billingEnd = new DateTime(2017, 3, 30);


            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd).ToList();

            Assert.IsTrue(invoiceLines.Count == 5);

            Assert.IsTrue(invoiceLines[0].Start == subscription.Start);
            Assert.IsTrue(invoiceLines[0].End == discount.Period.Start);
            Assert.IsTrue(invoiceLines[0].Total == 40);

            Assert.IsTrue(invoiceLines[1].Start == discount.Period.Start);
            Assert.IsTrue(invoiceLines[1].End == discount.Period.End);
            Assert.IsTrue(invoiceLines[1].Total == 25);

            Assert.IsTrue(invoiceLines[2].Start == discount.Period.End);
            Assert.IsTrue(invoiceLines[2].End == discount2.Period.Start);
            Assert.IsTrue(invoiceLines[2].Total == 50);

            Assert.IsTrue(invoiceLines[3].Start == discount2.Period.Start);
            Assert.IsTrue(invoiceLines[3].End == discount2.Period.End);
            Assert.IsTrue(invoiceLines[3].Total == 30);

            Assert.IsTrue(invoiceLines[4].Start == discount2.Period.End);
            Assert.IsTrue(invoiceLines[4].End == billingEnd);
            Assert.IsTrue(invoiceLines[4].Total == 100);
        }

        [TestMethod]
        public void TwoSameDiscountsWithScippedPeriodInTheMiddle()
        {
            var subscription = new Subscription(new DateTime(2017, 3, 1), null, 10.00m);
            var discount = new Discount(new DateTime(2017, 03, 05), new DateTime(2017, 03, 10), 50);
            var discount2 = new Discount(new DateTime(2017, 03, 15), new DateTime(2017, 03, 20), 50);
            var discounts = new List<Discount> { discount, discount2 };
            var billingEnd = new DateTime(2017, 3, 30);


            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd).ToList();

            Assert.IsTrue(invoiceLines.Count() == 5);

            Assert.IsTrue(invoiceLines[0].Start == subscription.Start);
            Assert.IsTrue(invoiceLines[0].End == discount.Period.Start);
            Assert.IsTrue(invoiceLines[0].Total == 40);

            Assert.IsTrue(invoiceLines[1].Start == discount.Period.Start);
            Assert.IsTrue(invoiceLines[1].End == discount.Period.End);
            Assert.IsTrue(invoiceLines[1].Total == 25);

            Assert.IsTrue(invoiceLines[2].Start == discount.Period.End);
            Assert.IsTrue(invoiceLines[2].End == discount2.Period.Start);
            Assert.IsTrue(invoiceLines[2].Total == 50);

            Assert.IsTrue(invoiceLines[3].Start == discount2.Period.Start);
            Assert.IsTrue(invoiceLines[3].End == discount2.Period.End);
            Assert.IsTrue(invoiceLines[3].Total == 25);

            Assert.IsTrue(invoiceLines[4].Start == discount2.Period.End);
            Assert.IsTrue(invoiceLines[4].End == billingEnd);
            Assert.IsTrue(invoiceLines[4].Total == 100);
        }

        [TestMethod]
        public void TwoSameOverlappedDiscountsInTheMiddle()
        {
            var subscription = new Subscription(new DateTime(2017, 3, 1), null, 10.00m);
            var discount = new Discount(new DateTime(2017, 03, 05), new DateTime(2017, 03, 10), 50);
            var discount2 = new Discount(new DateTime(2017, 03, 09), new DateTime(2017, 03, 20), 50);
            var discounts = new List<Discount> { discount, discount2 };
            var billingEnd = new DateTime(2017, 3, 30);


            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd).ToList();

            Assert.IsTrue(invoiceLines.Count() == 3);

            Assert.IsTrue(invoiceLines[0].Start == subscription.Start);
            Assert.IsTrue(invoiceLines[0].End == discount.Period.Start);
            Assert.IsTrue(invoiceLines[0].Total == 40);

            Assert.IsTrue(invoiceLines[1].Start == discount.Period.Start);
            Assert.IsTrue(invoiceLines[1].End == discount2.Period.End);
            Assert.IsTrue(invoiceLines[1].Total == 75);
            
            Assert.IsTrue(invoiceLines[2].Start == discount2.Period.End);
            Assert.IsTrue(invoiceLines[2].End == billingEnd);
            Assert.IsTrue(invoiceLines[2].Total == 100);
        }

        [TestMethod]
        public void ThreeOverlappedDiscounts()
        {
            var subscription = new Subscription(new DateTime(2017, 3, 1), null, 10.00m);
            var discount = new Discount(new DateTime(2017, 01, 01), new DateTime(2017, 5, 01), 50);
            var discount2 = new Discount(new DateTime(2017, 03, 10), new DateTime(2017, 03, 20), 70);
            var discount3 = new Discount(new DateTime(2017, 03, 20), new DateTime(2017, 03, 30), 90);
            var discounts = new List<Discount> { discount, discount2, discount3 };
            var billingEnd = new DateTime(2017, 3, 30);


            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd).ToList();

            Assert.IsTrue(invoiceLines.Count() == 3);

            Assert.IsTrue(invoiceLines[0].Start == subscription.Start);
            Assert.IsTrue(invoiceLines[0].End == discount2.Period.Start);
            Assert.IsTrue(invoiceLines[0].Total == 45);

            Assert.IsTrue(invoiceLines[1].Start == discount2.Period.Start);
            Assert.IsTrue(invoiceLines[1].End == discount2.Period.End);
            Assert.IsTrue(invoiceLines[1].Total == 30);

            Assert.IsTrue(invoiceLines[2].Start == discount2.Period.End);
            Assert.IsTrue(invoiceLines[2].End == billingEnd);
            Assert.IsTrue(invoiceLines[2].Total == 10);
        }

        [TestMethod]
        public void OneLongDiscountOverlapedByBiggerAndLowerDiscount()
        {
            var subscription = new Subscription(new DateTime(2017, 3, 1), null, 10.00m);
            var discount = new Discount(new DateTime(2017, 01, 01), new DateTime(2017, 5, 01), 50);
            var discount2 = new Discount(new DateTime(2017, 03, 10), new DateTime(2017, 03, 20), 70);
            var discount3 = new Discount(new DateTime(2017, 03, 20), new DateTime(2017, 03, 30), 20);
            var discounts = new List<Discount> { discount, discount2, discount3 };
            var billingEnd = new DateTime(2017, 3, 30);


            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd).ToList();

            Assert.IsTrue(invoiceLines.Count() == 3);

            Assert.IsTrue(invoiceLines[0].Start == subscription.Start);
            Assert.IsTrue(invoiceLines[0].End == discount2.Period.Start);
            Assert.IsTrue(invoiceLines[0].Total == 45);

            Assert.IsTrue(invoiceLines[1].Start == discount2.Period.Start);
            Assert.IsTrue(invoiceLines[1].End == discount2.Period.End);
            Assert.IsTrue(invoiceLines[1].Total == 30);

            Assert.IsTrue(invoiceLines[2].Start == discount2.Period.End);
            Assert.IsTrue(invoiceLines[2].End == billingEnd);
            Assert.IsTrue(invoiceLines[2].Total == 50);
        }

        [TestMethod]
        public void LongDiscountOverlaedByTwoLowerDiscounts()
        {
            var subscription = new Subscription(new DateTime(2017, 3, 1), null, 10.00m);
            var discount = new Discount(new DateTime(2017, 01, 01), new DateTime(2017, 5, 01), 50);
            var discount2 = new Discount(new DateTime(2017, 03, 10), new DateTime(2017, 03, 20), 20);
            var discount3 = new Discount(new DateTime(2017, 03, 20), new DateTime(2017, 03, 30), 30);
            var discounts = new List<Discount> { discount, discount2, discount3 };
            var billingEnd = new DateTime(2017, 3, 30);


            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd).ToList();

            Assert.IsTrue(invoiceLines.Count() == 1);

            Assert.IsTrue(invoiceLines[0].Start == subscription.Start);
            Assert.IsTrue(invoiceLines[0].End == billingEnd);
            Assert.IsTrue(invoiceLines[0].Total == 145);
        }
    }
}
