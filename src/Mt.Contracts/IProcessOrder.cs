using System;

namespace Mt.Contracts
{
    public interface IProcessOrder
    {
        int OrderId { get; set; }
        DateTime Date { get; set; }
    }
}