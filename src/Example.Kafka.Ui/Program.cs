using Confluent.Kafka;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


var consumerConfig = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "test-group",
    // Add any additional configuration options as needed
};

var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

using (var producer = new ProducerBuilder<Null, string>(config).Build())
{
    try
    {
        var sendResult = producer
                            .ProduceAsync("fila_pedido", new Message<Null, string> { Value = "Mensagem" })
                                .GetAwaiter()
                                    .GetResult();

        Console.WriteLine($"Mensagem '{sendResult.Value}' de '{sendResult.TopicPartitionOffset}'");
    }
    catch (ProduceException<Null, string> e)
    {
        Console.WriteLine($"Delivery failed: {e.Error.Reason}");
    }
}

using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
{
    consumer.Subscribe("fila_pedido");

    while (true)
    {
        
        var consumeResult = consumer.Consume(2000);
        if (consumeResult != null)
            Console.WriteLine($"Received message: {consumeResult.Message.Value}");
    }
}