using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kokosoft.SimmingPoolTracker.API.Model
{
    public class Pool
    {
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public string ExitTime { get; set; }
        public int MaximumNumberOfLanes { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
    }
}
