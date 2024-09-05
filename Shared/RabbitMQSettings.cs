using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    static public class RabbitMQSettings
    {
        public static string Stock_OrderCreatedEventQueue = "stock-order-created-event-queue";
        public static string Payment_StockReservedEventQueue= "payment-stock-reserved-event-queue";
        public static string Order_PaymentCompletedEventQueue= "order-payment-completed-event-queue";
        public static string Order_PaymentFailedEventQueue = "order-payment-failed-event-queue";
        public static string Stock_PaymentFailedEventQueue = "stock-payment-failed-event-queue";
        public static string Order_StockNotReservedEventQueue = "order-stock-not-reserved-event-queue";

    }
}
