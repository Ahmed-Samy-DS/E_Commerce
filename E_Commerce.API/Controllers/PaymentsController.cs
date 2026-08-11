using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace E_Commerce.API.Controllers
{
    

    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly PaymentGatewaySettings _paymentGatewaySettings;
        public PaymentsController(IPaymentService paymentService, IOptions<PaymentGatewaySettings> options)
        {
            _paymentService = paymentService;
            _paymentGatewaySettings = options.Value;

        }

        // Create or Update Payment Intent for the basket

        // Authorize => Only Authenticated Users can access this endpoint
        // POST baseurl/api/payments/{basketId}
        // basketId => Return BasketDto Updated with PaymentIntentId and ClientSecret

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent(string basketId, CancellationToken cancellationToken)
        {
            
            return ToActionResult(await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId, cancellationToken));
            
        }


        [HttpPost("webhook")]

        //post baseurl/api/payments/webhook

        public async Task<IActionResult> StripeWebhook()
        {
            var requestJson = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();


            try
            {
                var stripeEvent = EventUtility.ConstructEvent(requestJson,
                                Request.Headers["Stripe-Signature"],
                                _paymentGatewaySettings.WebhookSecret);


                switch(stripeEvent.Type) 
                {
                    case EventTypes.PaymentIntentSucceeded:
                        var paymentIntentSucceed = stripeEvent.Data.Object as PaymentIntent;
                        if(paymentIntentSucceed is not null)
                        {
                            await _paymentService.PaymentSuccededAsync(paymentIntentSucceed.Id);
                        }

                        break;

                    case EventTypes.PaymentIntentPaymentFailed:
                        var paymentIntentFailed = stripeEvent.Data.Object as PaymentIntent;
                        if (paymentIntentFailed is not null)
                        {
                            await _paymentService.PaymentFailedAsync(paymentIntentFailed.Id);
                        }

                        break;
                    default:
                        break;


                      
                }
                return Ok();
            }
            catch(StripeException ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }







        }



    }
}
