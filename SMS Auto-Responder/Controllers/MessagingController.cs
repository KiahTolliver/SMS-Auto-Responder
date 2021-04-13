using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SMS_AutoResponder.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telnyx;

namespace SMS_AutoResponder.Controllers
{
    public class WebhookHelpers
    {
        public static async Task<InboundWebhook> deserializeInboundMessage(HttpRequest request)
        {
            string json;
            using (var reader = new StreamReader(request.Body))
            {
                json = await reader.ReadToEndAsync();
            }
            InboundWebhook myDeserializedClass = JsonConvert.DeserializeObject<InboundWebhook>(json);
            return myDeserializedClass;
        }

        public static async Task<OutboundWebhook> deserializeOutboundMessage(HttpRequest request)
        {
            string json;
            using (var reader = new StreamReader(request.Body))
            {
                json = await reader.ReadToEndAsync();
            }
            OutboundWebhook myDeserializedClass = JsonConvert.DeserializeObject<OutboundWebhook>(json);
            return myDeserializedClass;
        }
    }

    [ApiController]
    [Route("messaging/[controller]")]
    public class OutboundController : ControllerBase
    {
        // POST messaging/Outbound
        [HttpPost]
        [Consumes("application/json")]
        public async Task MessageDLRCallback()
        {
            OutboundWebhook webhook = await WebhookHelpers.deserializeOutboundMessage(this.Request);
            Console.WriteLine($"Received DLR for message with ID: {webhook.data.payload.id}");
        }
    }

    [ApiController]
    [Route("messaging/[controller]")]
    public class InboundController : ControllerBase
    {

        private string TELNYX_API_KEY = System.Environment.GetEnvironmentVariable("TELNYX_API_KEY");
        // POST messaging/Inbound
        [HttpPost]
        [Consumes("application/json")]
        public async Task MessageInboundCallback()
        {
            //Deserialize Inbound message

            InboundWebhook webhook = await WebhookHelpers.deserializeInboundMessage(this.Request);
            UriBuilder uriBuilder = new UriBuilder(Request.Scheme, Request.Host.ToString());
            uriBuilder.Path = "messaging/outbound";
            string dlrUri = uriBuilder.ToString();
            string to = webhook.data.payload.to[0].phone_number;
            string from = webhook.data.payload.from.phone_number;
            string inboundMessage = webhook.data.payload.text;
            string outboundMessage;

            //Inspect inbound message and prepare outbound response
            if (inboundMessage.Trim().ToLower().Equals("pizza"))
            {
                outboundMessage = "Chicago pizza is the best";
            }
            else if (inboundMessage.Trim().ToLower().Equals("ice cream"))
            {
                outboundMessage = "I prefer gelato";
            }
            else
            {
                outboundMessage = "Please send either the word ‘pizza’ or ‘ice cream’ for a different response";
            }
  
       
            TelnyxConfiguration.SetApiKey(TELNYX_API_KEY);
            MessagingSenderIdService service = new MessagingSenderIdService();
            NewMessagingSenderId options = new NewMessagingSenderId
            {
                From = to, //Reversing from and to numbers for response
                To = from,
                Text = outboundMessage,
                WebhookUrl = dlrUri,
                UseProfileWebhooks = false
            };
            try
            {
                MessagingSenderId messageResponse = await service.CreateAsync(options);
                Console.WriteLine($"Sent message with ID: {messageResponse.Id}");
            }
            catch (TelnyxException ex)
            {
                Console.WriteLine("exception");
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

        }
    }
}
