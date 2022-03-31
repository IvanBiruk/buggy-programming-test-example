using System.Globalization;
using Business.BillCalculator;
using Business.Models;
using Business.Services;

namespace TestTask
{
    public class Program
    {

        private const string DateFormat = "dd.MM.yyyy HH:ss";
        private static BillManager _billManager;
        static void Main(string[] args)
        {
            _billManager = new BillManager(new DaysBillCalculator());//here we could provide any other billing calculator


            Console.WriteLine("Task2_1 with skipped period");
            Task2_1();
            Console.WriteLine("Task2_2 with touching");
            Task2_2();
            Console.WriteLine("Task2_3 with overlapping");
            Task2_3();

            Console.ReadLine();
        }

        private static void Task2_1()
        {
            

            var subscription = new Subscription(new DateTime(2017, 03, 03), null, 10.00m);
            var discounts = new List<Discount>
            {
                new Discount(new DateTime(2017, 03, 03),new DateTime(2017, 03, 14),50),
                new Discount(new DateTime(2017, 03, 16),new DateTime(2017, 04, 10, 00, 0, 0),20),
                new Discount(new DateTime(2017, 04, 10, 00, 0, 0),new DateTime(2017, 5, 17),30)
            };
            var billingEnd = new DateTime(2017, 05, 03);
            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd);

            foreach (var discount in discounts)
            {
                Console.WriteLine(discount);
            }
            foreach (var invoiceLine in invoiceLines)
            {
                Console.WriteLine(invoiceLine);
            }
        }

        private static void Task2_2()
        {


            var subscription = new Subscription(new DateTime(2017, 03, 03), null, 10.00m);
            var discounts = new List<Discount>
            {
                new Discount(new DateTime(2017, 03, 03),new DateTime(2017, 03, 16),50),
                new Discount(new DateTime(2017, 03, 16),new DateTime(2017, 04, 10, 00, 0, 0),20),
                new Discount(new DateTime(2017, 04, 10, 00, 0, 0),new DateTime(2017, 5, 17),30)
            };
            var billingEnd = new DateTime(2017, 05, 03);
            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd);

            foreach (var discount in discounts)
            {
                Console.WriteLine(discount);
            }
            foreach (var invoiceLine in invoiceLines)
            {
                Console.WriteLine(invoiceLine);
            }
        }

        private static void Task2_3()
        {


            var subscription = new Subscription(new DateTime(2017, 03, 03), null, 10.00m);
            var discounts = new List<Discount>
            {
                new Discount(new DateTime(2017, 03, 03),new DateTime(2017, 03, 20),50),
                new Discount(new DateTime(2017, 03, 16),new DateTime(2017, 04, 10, 00, 0, 0),20),
                new Discount(new DateTime(2017, 04, 10, 00, 0, 0),new DateTime(2017, 5, 17),30)
            };
            var billingEnd = new DateTime(2017, 05, 03);
            var invoiceLines = _billManager.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd);

            foreach (var discount in discounts)
            {
                Console.WriteLine(discount);
            }
            foreach (var invoiceLine in invoiceLines)
            {
                Console.WriteLine(invoiceLine);
            }
        }

        private static void Task1()
        {
            bool isExit;
            do
            {
                var start = ReadDate("subscription start date:");
                var end = ReadDate("subscription end date:", true);

                var subscription = new Subscription(start.Value, end, 2);

                var billEnd = ReadDate("billing on date:");

                try
                {
                    var bill = _billManager.BillSubscription(subscription, billEnd.Value);
                    Console.WriteLine(bill);
                }
                catch (ArgumentNullException exception)
                {
                    HandleError(exception);
                }
                catch (ArgumentOutOfRangeException outOfRangeException)
                {
                    HandleError(outOfRangeException);
                }

                Console.WriteLine("Enter 'exit' for close app or any other key to continue.");
                isExit = Console.ReadLine() != "exit";
            } while (isExit);
        }

        private static void HandleError(Exception exception)
        {
            Console.WriteLine("Error: {0}", exception.Message);
        }

        private static DateTime? ReadDate(string expectedInfo, bool isAllowNull= false)
        {
            var nullInfo = isAllowNull ? "(null allowed)" : String.Empty;
            Console.WriteLine("Enter {0} in format {1} {2} :", expectedInfo, DateFormat, nullInfo);
            while (true)
            {
                var consoleData = Console.ReadLine();
                if (String.IsNullOrEmpty(consoleData) && isAllowNull)
                {
                    return null;
                }

                DateTime date;
                var isSuccess = DateTime.TryParseExact(consoleData, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                if (isSuccess)
                {
                    return date;
                }
                Console.WriteLine("Incorrect data entered. Please try one more time;");
            }
        }
    }
}
