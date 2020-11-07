﻿using System;
using System.Collections.Generic;

namespace HandCarftBaseServer.Models
{
    public partial class Location
    {
        public Location()
        {
            Customer = new HashSet<Customer>();
            CustomerAddressCity = new HashSet<CustomerAddress>();
            CustomerAddressProvince = new HashSet<CustomerAddress>();
            InverseCountry = new HashSet<Location>();
            InverseP = new HashSet<Location>();
            InverseProvince = new HashSet<Location>();
            PaymentTypeLocation = new HashSet<PaymentTypeLocation>();
            Seller = new HashSet<Seller>();
        }

        public long Id { get; set; }
        public long? Pid { get; set; }
        public string Name { get; set; }
        public string EnName { get; set; }
        public long? LocationCode { get; set; }
        public long? PostCode { get; set; }
        public long? CountryId { get; set; }
        public long? ProvinceId { get; set; }
        public long? CuserId { get; set; }
        public long? Cdate { get; set; }
        public long? DuserId { get; set; }
        public long? Ddate { get; set; }
        public long? MuserId { get; set; }
        public long? Mdate { get; set; }
        public long? DaUserId { get; set; }
        public long? DaDate { get; set; }

        public virtual Location Country { get; set; }
        public virtual Location P { get; set; }
        public virtual Location Province { get; set; }
        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<CustomerAddress> CustomerAddressCity { get; set; }
        public virtual ICollection<CustomerAddress> CustomerAddressProvince { get; set; }
        public virtual ICollection<Location> InverseCountry { get; set; }
        public virtual ICollection<Location> InverseP { get; set; }
        public virtual ICollection<Location> InverseProvince { get; set; }
        public virtual ICollection<PaymentTypeLocation> PaymentTypeLocation { get; set; }
        public virtual ICollection<Seller> Seller { get; set; }
    }
}
