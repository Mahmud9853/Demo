using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using Core.Interfaces;
using API.Errors;
using Stripe;
using Core.Entities.OrderAggregate;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly string _whSecret;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger, IConfiguration config)
        {
            _logger = logger;
            _paymentService = paymentService;
            _whSecret = config.GetSection("StripeSettings:WhSecret").Value;

        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateorUpdatePaymentIntent(string basketId)
        {
            CustomerBasket basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if(basket == null) return BadRequest(new ApiResponse(400, "Problem with your basket"));
            
            return basket;
        }
        [HttpPost ("webhook")]
        public async Task<ActionResult> StripeWebHook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json,
                 Request.Headers["Stripe-Signature"], _whSecret);

            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent) stripeEvent.Data.Object;
                    _logger.LogInformation("Payment succeeded: {PaymentIntentId}", intent.Id);
                    //TODO: update the order with the new status
                    order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    _logger.LogInformation("Oder update to payment received: {OrderId}", order.Id);
                    break;
                case "payment_intent.payment_failed":
                    intent = (PaymentIntent) stripeEvent.Data.Object;
                    _logger.LogInformation("Payment failed: {PaymentIntentId}", intent.Id);
                    //TODO: update the order with the new status
                     order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                    _logger.LogInformation("Oder update to payment failed:  {OrderId}", order.Id);
                    break;
            }   

            return new EmptyResult();  

        }
        
    }
}