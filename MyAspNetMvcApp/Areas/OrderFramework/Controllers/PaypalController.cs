using MyAspNetMvcApp.Areas.OrderFramework.Paypal;
using MyAspNetMvcApp.Areas.OrderFramework.ViewModels;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.OrderFramework.Controllers
{
    public class PaypalController : Controller
    {
        public ActionResult GetPaypalVault(string CreditCardId)
        {
            var vault = PaypalVault.GetCreditCardDetailsFromVault(CreditCardId);
            return Json(vault, JsonRequestBehavior.AllowGet);

        }

        public ActionResult PaymentWithCreditCard(string CreditCardId = "")
        {
            var pvm = PaypalViewModel.GetSamplePayment("yuberto.gabon@outlook.com");

            var apiContext = PayPalConfig.GetAPIContext();

            // A transaction defines the contract of a payment.
            var transaction = new Transaction()
            {
                amount = new Amount()
                {
                    currency = pvm.Transaction.Amount.Currency,
                    total = pvm.Transaction.Amount.Total,
                    details = new Details()
                    {
                        shipping = pvm.Transaction.Amount.Detail.Shipping,
                        subtotal = pvm.Transaction.Amount.Detail.Subtotal,
                        tax = pvm.Transaction.Amount.Detail.Tax
                    }
                },
                description = pvm.Transaction.Description,
                item_list = new ItemList()
                {
                    items = pvm.Transaction.ItemList.Select(s => new Item
                    {
                        name = s.Name,
                        currency = s.Currency,
                        price = s.Price,
                        quantity = s.Quantity,
                        sku = s.Sku
                    }).ToList(),
                    shipping_address = new ShippingAddress
                    {
                        city = pvm.Transaction.ShippingAddress.City,
                        country_code = pvm.Transaction.ShippingAddress.CountryCodeDomain,
                        line1 = pvm.Transaction.ShippingAddress.AddressLine,
                        postal_code = pvm.Transaction.ShippingAddress.PostalCode,
                        state = pvm.Transaction.ShippingAddress.State,
                        recipient_name = pvm.Transaction.ShippingAddress.RecipientName
                    }
                },
                invoice_number = String.IsNullOrEmpty(pvm.Transaction.InvoiceNumber) ? GetRandomInvoiceNumber() : pvm.Transaction.InvoiceNumber
            };

            // A resource representing a Payer that funds a payment.
            var payer = new Payer()
            {
                payment_method = "credit_card",
                payer_info = new PayerInfo
                {
                    email = pvm.Payer.PayerInfo.Email
                }
            };

            if(String.IsNullOrEmpty(CreditCardId))
            {
                payer.funding_instruments = new List<FundingInstrument>()
                {
                    new FundingInstrument()
                    {
                        credit_card = new CreditCard()
                        {
                            billing_address = new Address()
                            {
                                city = pvm.Payer.FundingInstrument.CreditCard.BillingAddress.City,
                                country_code = pvm.Payer.FundingInstrument.CreditCard.BillingAddress.CountryCodeDomain,
                                line1 = pvm.Payer.FundingInstrument.CreditCard.BillingAddress.AddressLine,
                                postal_code = pvm.Payer.FundingInstrument.CreditCard.BillingAddress.PostalCode,
                                state = pvm.Payer.FundingInstrument.CreditCard.BillingAddress.State
                            },
                            cvv2 = pvm.Payer.FundingInstrument.CreditCard.Cvv2,
                            expire_month = pvm.Payer.FundingInstrument.CreditCard.ExpireMonth,
                            expire_year = pvm.Payer.FundingInstrument.CreditCard.ExpireYear,
                            first_name = pvm.Payer.FundingInstrument.CreditCard.FirstName,
                            last_name = pvm.Payer.FundingInstrument.CreditCard.LastName,
                            number = pvm.Payer.FundingInstrument.CreditCard.CcNumber,
                            type = pvm.Payer.FundingInstrument.CreditCard.CcType
                        }
                    }
                };

                CreditCardId = PaypalVault.StoreCreditCardInPaypal(pvm.Payer.FundingInstrument.CreditCard);
            }
            else
            {
                //Here, we are assigning the User's Credit Card ID which we saved in Database
                payer.funding_instruments = new List<FundingInstrument>()
                {
                    new FundingInstrument()
                    {
                        credit_card_token = new CreditCardToken
                        {
                            credit_card_id = CreditCardId
                        }
                    }
                };

            }


            // A Payment resource; create one using the above types and intent as `sale` or `authorize`
            var payment = new PayPal.Api.Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = new List<Transaction>() { transaction }
            };

            // ^ Ignore workflow code segment
            #region Track Workflow
            //this.flow.AddNewRequest("Create credit card payment", payment);
            #endregion

            // Create a payment using a valid APIContext
            var createdPayment = payment.Create(apiContext);

            // ^ Ignore workflow code segment
            #region Track Workflow
            //this.flow.RecordResponse(createdPayment);
            #endregion

            if (createdPayment.state.ToLower() != "approved")
            {
                //return View("FailureView");
                return Content("Failed");
            }

            return Content("Success Id: " + CreditCardId);

            // For more information, please visit [PayPal Developer REST API Reference](https://developer.paypal.com/docs/api/).
        }

        public ActionResult PaymentWithPaypal(PaypalViewModel pvm)
        {
            pvm = PaypalViewModel.GetSamplePayment();

            //getting the apiContext as earlier
            APIContext apiContext = PayPalConfig.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist
                    //it is returned by the create function call of the payment class

                    // Creating a payment
                    // baseURL is the url on which paypal sendsback the data.
                    // So we have provided URL of this controller only
                    // 
                    string baseURI = Request.Url.GetLeftPart(UriPartial.Authority)  // Request.Url.Scheme + "://" + Request.Url.Authority 
                        + "/" + Url.Action("PaymentWithPayPal","Paypal", new { area = "OrderFramework" }) + "?";

                    //guid we are generating for storing the paymentID received in session
                    //after calling the create function and it is used in the payment execution

                    var guid = Convert.ToString((new Random()).Next(100000));

                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment

                    var createdPayment = this.CreatePayment(pvm, apiContext, baseURI + "guid=" + guid);

                    //get links returned from paypal in response to Create function call

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This section is executed when we have received all the payments parameters

                    // from the previous call to the function Create

                    // Executing a payment

                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return Content("Failed");
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.log("Error" + ex.Message);
                //return View("FailureView");
                return Content("Failed");
            }

            //return View("SuccessView");
            return Content("Success");
        }

        #region PaypalMethod
        private Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(PaypalViewModel pvm, APIContext apiContext, string redirectUrl)
        {
            var payer = new Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };


            //similar to credit card create itemlist and add item objects to it
            var itemList = new ItemList()
            {
                items = pvm.Transaction.ItemList.Select(s => new Item
                {
                    name = s.Name,
                    currency = s.Currency,
                    price = s.Price,
                    quantity = s.Quantity,
                    sku = s.Sku
                }).ToList()
            };

            // similar as we did for credit card, do here and create details object
            var details = new Details()
            {
                tax = pvm.Transaction.Amount.Detail.Tax,
                shipping = pvm.Transaction.Amount.Detail.Shipping,
                subtotal = pvm.Transaction.Amount.Detail.Subtotal
            };

            // similar as we did for credit card, do here and create amount object
            var amount = new Amount()
            {
                currency = pvm.Transaction.Amount.Currency,
                total = pvm.Transaction.Amount.Total, // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = pvm.Transaction.Description,
                invoice_number = String.IsNullOrEmpty(pvm.Transaction.InvoiceNumber) ? GetRandomInvoiceNumber() : pvm.Transaction.InvoiceNumber,
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);

        }

        /// <summary>
        /// Gets a random invoice number to be used with a sample request that requires an invoice number.
        /// </summary>
        /// <returns>A random invoice number in the range of 0 to 999999</returns>
        private string GetRandomInvoiceNumber()
        {
            return new Random().Next(999999).ToString();
        }
        #endregion
    }

}

namespace MyAspNetMvcApp.Areas.OrderFramework.Paypal
{
    public static class PayPalConfig
    {
        public readonly static string ClientId;
        public readonly static string ClientSecret;

        // Static constructor for setting the readonly static members.
        static PayPalConfig()
        {
            ClientId = ConfigurationManager.AppSettings["PaypalClientId"];
            ClientSecret = ConfigurationManager.AppSettings["PaypalClientSecret"];
        }

        // Create the configuration map that contains mode and other optional configuration details.
        public static Dictionary<string, string> GetConfig()
        {
            return ConfigManager.Instance.GetProperties();
        }

        // Create accessToken
        private static string GetAccessToken()
        {
            // ###AccessToken
            // Retrieve the access token from
            // OAuthTokenCredential by passing in
            // ClientID and ClientSecret
            // It is not mandatory to generate Access Token on a per call basis.
            // Typically the access token can be generated once and
            // reused within the expiry window                
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }

        // Returns APIContext object
        public static APIContext GetAPIContext(string accessToken = "")
        {
            // ### Api Context
            // Pass in a `APIContext` object to authenticate 
            // the call and to send a unique request id 
            // (that ensures idempotency). The SDK generates
            // a request id if you do not pass one explicitly. 
            var apiContext = new APIContext(string.IsNullOrEmpty(accessToken) ? GetAccessToken() : accessToken);
            apiContext.Config = GetConfig();

            // Use this variant if you want to pass in a request id  
            // that is meaningful in your application, ideally 
            // a order id.
            // String requestId = Long.toString(System.nanoTime();
            // APIContext apiContext = new APIContext(GetAccessToken(), requestId ));

            return apiContext;
        }

    }

    public static class PaypalVault
    {
        // https://www.codeproject.com/Tips/886187/PayPal-REST-API-Recurring-Payment-via-Stored-Credi
        // https://code.tutsplus.com/articles/paypal-integration-part-2-paypal-rest-api--cms-22917

        public static string StoreCreditCardInPaypal(PaypalViewModel._Payer._FundingInstrument._CreditCard cc)
        {
            //Creating the CreditCard Object and assigning values
            var creditCard = new CreditCard
            {
                expire_month = cc.ExpireMonth,
                expire_year = cc.ExpireYear,
                number = cc.CcNumber,
                type = cc.CcType,
                cvv2 = cc.Cvv2
            };

            try
            {
                //Getting the API Context to authenticate the call to Paypal Server
                APIContext apiContext = PayPalConfig.GetAPIContext();
                // Storing the Credit Card Info in the PayPal Vault Server
                CreditCard createdCreditCard = creditCard.Create(apiContext);

                //Saving the User's Credit Card ID returned by the PayPal
                //You can use this ID for future payments via User's Credit Card
                //SaveCardID(User.Identity.Name, createdCreditCard.id);

                return createdCreditCard.id;

            }
            catch (PayPal.PayPalException ex)
            {
                //Logger.LogError("Error: " + ex.Message);
                return ex.Message;
            }
            catch (Exception ex)
            {
                //Logger.LogError("Error: " + ex.Message);
                return ex.Message;
            }

        }

        public static CreditCard GetCreditCardDetailsFromVault(string creditCardId)
        {
            try
            {
                //Getting the API Context to authenticate the call
                APIContext apiContext = PayPalConfig.GetAPIContext();

                //Getting the Credit Card Details from paypal
                //By sending the Card ID saved at our end
                CreditCard card = CreditCard.Get(apiContext, creditCardId); // "CARD-00N04036H5458422MKRIAWHY"
                return card;
            }
            catch (PayPal.PayPalException ex)
            {
                //Logger.LogError("Error: " + ex.Message);
            }

            return null;
        }

        public static bool DeleteCreditCardFromVault(string creditCardId)
        {
            try
            {
                // Getting the API Context for authentication the call to paypal server
                APIContext apiContext = PayPalConfig.GetAPIContext();

                //get the credit card from the vault to delete
                CreditCard card = CreditCard.Get(apiContext, creditCardId); // "CARD-00N04036H5458422MKRIAWHY"

                // Delete the credit card
                card.Delete(apiContext);

                return true;
            }
            catch (PayPal.PayPalException ex)
            {
                //Logger.LogError("Error: " + ex.Message);
                return false;
            }
        }
    }

}