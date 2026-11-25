# Mensagens são perdidas quando o ClickHouse fica indisponível

## Descrição

Quando o ClickHouse está fora do ar, as mensagens consumidas nesse período
desaparecem: não são gravadas nas tabelas, não permanecem na fila
`mdt.event-consumer` e não chegam à fila de rejeitadas
`mdt.event-consumer.dlq`.

O log do Worker registra a falha e informa que as mensagens foram
rejeitadas, mas a fila de rejeitadas continua vazia:

```
"Level":"Error",
"MessageTemplate":"[Flush {EventType}] Insert FAILED. Nacking
{MessageCount} messages.",
"Exception":"System.Net.Http.HttpRequestException: Connection refused
(localhost:8123) ...",
"Properties":{"EventType":"Status","MessageCount":15, ...}
```

## Comportamento esperado

Uma mensagem cujos eventos não foram gravados deve ir para a fila de
rejeitadas, de onde pode ser reprocessada depois.

## Como reproduzir

Com os containers no ar e o Worker em execução (ver README):

```sh
# 1. Derrubar o ClickHouse
docker compose stop clickhouse

# 2. Publicar 30 mensagens válidas
dotnet run --project tools/Aiko.EventConsumer.MessageGenerator -- --count 30

# 3. Aguardar cerca de 15 segundos, até o Worker registrar "Insert FAILED"

# 4. Conferir as filas
docker compose exec rabbitmq gosu rabbitmq \
  rabbitmqctl -q list_queues name messages

# 5. Subir o ClickHouse novamente
docker compose start clickhouse
```

Resultado observado no passo 4:

```
mdt.event-consumer      0
mdt.event-consumer.dlq  0
```

Resultado esperado:

```
mdt.event-consumer      0
mdt.event-consumer.dlq  30
```

Após o passo 5, as tabelas não contêm os eventos das 30 mensagens.
