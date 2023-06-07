using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Example.Kafka.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult Post([FromQuery] string msg)
        {
            return Created("", SendMessageByKafka(msg));
        }

        private string SendMessageByKafka(string message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "kafka:29092", //usually of the form cell-1.streaming.[region code].oci.oraclecloud.com:9092
                                                     //  SslCaLocation = "<path\to\root\ca\certificate\*.pem>",
                                                     // SecurityProtocol = SecurityProtocol.SaslSsl,
                                                     //   SaslMechanism = SaslMechanism.Plain,
                                                     // SaslUsername = "<OCI_tenancy_name>/<your_OCI_username>/<stream_pool_OCID>",
                                                     //SaslPassword = "<your_OCI_user_auth_token>", // use the auth-token you created step 5 of Prerequisites section 
            };
            Produce("fila_pedido", config);
            //using (var producer = new ProducerBuilder<Null, string>(config).Build())
            //{
            //    try
            //    {
            //        var sendResult = producer.ProduceAsync("fila_pedido", new Message<Null, string> { Value = message })/*.ConfigureAwait(false);*/
            //                                .GetAwaiter()
            //                                .GetResult();

            //                        // return "OK";
            //        return $"Mensagem '{sendResult.Value}' de '{sendResult.TopicPartitionOffset}'";
            //    }
            //    catch (ProduceException<Null, string> e)
            //    {
            //        Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            //    }
            //}

            return string.Empty;
        }



        static void Produce(string topic, ClientConfig config)
        {
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                int numProduced = 0;
                int numMessages = 5000;
                for (int i = 0; i < numMessages; ++i)
                {
                    var key = "API_messageKey" + i;
                    var val = "API_messageVal" + i;

                    Console.WriteLine($"Producing record: {key} {val}");

                    producer.Produce(topic, new Message<string, string> { Key = key, Value = val },
                        (deliveryReport) =>
                        {
                            if (deliveryReport.Error.Code != ErrorCode.NoError)
                            {
                                Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                            }
                            else
                            {
                                Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                                numProduced += 1;
                            }
                        });
                }

                producer.Flush(TimeSpan.FromSeconds(10));

                Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
            }
        }

    }
}
