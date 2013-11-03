using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HAckathon
{
    [DataContract]
    class Entry
    {
        [DataMember]
        public string CafeName;
        [DataMember]
        public string CafeUrl;
        [DataMember]
        public string DishName;
        [DataMember]
        public string Description;
        [DataMember]
        public string RestaurantName;
        [DataMember]
        public string Price;
    }
}
