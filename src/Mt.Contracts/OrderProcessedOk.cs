using System;

namespace Mt.Contracts
{
    public interface OrderProcessedOk
    {
        int OrderId { get; set; }
        DateTime Date { get; set; }
        string Mensaje { get; set; }
    }
}