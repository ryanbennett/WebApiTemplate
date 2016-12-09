
using System;

namespace WebApiTemplate.Data.Interfaces
{
    public interface IHasTimestamp
    {
        string EditedBy { get; set; }
        DateTimeOffset? Timestamp { get; set; }
    }
}
