using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.OrderFramework.ViewModels
{
	public class PaypalViewModel
	{
        public _Payer Payer { get; set; }
        public _Transaction Transaction { get; set; }

        public class _Payer
        {
            public _PayerInfo PayerInfo { get; set; }
            public _FundingInstrument FundingInstrument { get; set; }

            public class _PayerInfo
            {
                public string Email { get; set; }
            }

            public class _FundingInstrument
            {
                public _CreditCard CreditCard { get; set; }
                public _CreditCardToken CreditCardToken { get; set; }
                public class _CreditCard
                {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public string CcType { get; set; }
                    public string CcNumber { get; set; }
                    public string Cvv2 { get; set; }
                    public int ExpireMonth { get; set; }
                    public int ExpireYear { get; set; }
                    public _BillingAddress BillingAddress { get; set; }

                    public class _BillingAddress
                    {
                        public string City { get; set; }
                        public string CountryCodeDomain { get; set; }
                        public string AddressLine { get; set; }
                        public string PostalCode { get; set; }
                        public string State { get; set; }
                    }
                }
                public class _CreditCardToken
                {
                    public string CreditCardId { get; set; }
                }
            }

        }

        public class _Transaction
        {
            public string Description { get; set; }
            public string InvoiceNumber { get; set; }
            public _Amount Amount { get; set; }
            public List<_Item> ItemList { get; set; }
            public _ShippingAddress ShippingAddress { get; set; }

            public class _Amount
            {
                public string Currency { get; set; }
                public string Total { get; set; }
                public _Detail Detail { get; set; }
                public class _Detail
                {
                    public string Shipping { get; set; }
                    public string Subtotal { get; set; }
                    public string Tax { get; set; }
                }
            }
            public class _Item
            {
                public string Name { get; set; }
                public string Currency { get; set; }
                public string Price { get; set; }
                public string Quantity { get; set; }
                public string Sku { get; set; }
            }
            public class _ShippingAddress
            {
                public string City { get; set; }
                public string CountryCodeDomain { get; set; }
                public string AddressLine { get; set; }
                public string PostalCode { get; set; }
                public string State { get; set; }
                public string RecipientName { get; set; }
            }

        }

        public static PaypalViewModel GetSamplePayment(string buyerEmail = "")
        {
            var pvm = new PaypalViewModel
            {
                Payer = new _Payer
                {
                    PayerInfo = new _Payer._PayerInfo
                    {
                        Email = buyerEmail
                    },
                    FundingInstrument = new _Payer._FundingInstrument
                    {
                        CreditCard = new _Payer._FundingInstrument._CreditCard
                        {
                            FirstName = "Yuberto",
                            LastName = "Gabs",
                            CcType = "visa",
                            CcNumber = "4032031018735885",
                            ExpireMonth = 02,
                            ExpireYear = 2022,
                            Cvv2 = "000",
                            BillingAddress = new _Payer._FundingInstrument._CreditCard._BillingAddress
                            {
                                AddressLine = "Marigold st., Carmen",
                                City = "Cagayan De Oro",
                                State = "Misamis Oriental",
                                CountryCodeDomain = "PH",
                                PostalCode = "9000"
                            }
                        }                        
                    }                    
                },
                Transaction = new _Transaction
                {
                    Amount = new _Transaction._Amount
                    {
                        Currency = "USD",
                        Total = "28",
                        Detail = new _Transaction._Amount._Detail
                        {
                            Shipping = "1",
                            Subtotal = "26",
                            Tax = "1"
                        }                         
                    },
                    Description = "Gabs Sari-sari Sample Transaction",
                    ItemList = new List<_Transaction._Item>
                    {
                        new _Transaction._Item
                        {
                             Currency = "USD",
                             Name = "Banana Cue",
                             Price = "8",
                             Quantity = "2",
                             Sku = "44443333"
                        },
                        new _Transaction._Item
                        {
                             Currency = "USD",
                             Name = "Binignit",
                             Price = "10",
                             Quantity = "1",
                             Sku = "44445555"
                        }
                    },
                    ShippingAddress = new _Transaction._ShippingAddress
                    {
                        RecipientName = "Julieta Dela Saluta",
                        AddressLine = "RCPA Road, Purok 6-A, North Poblacion",
                        City = "Maramag",
                        State = "Bukidnon",
                        CountryCodeDomain = "PH",
                        PostalCode = "8714",                        
                    }                     
                }
            };

            return pvm;
        }

    }
}