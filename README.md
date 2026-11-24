# Event Consumer

Serviço que consome mensagens enviadas por equipamentos em campo, publicadas
em um exchange do RabbitMQ, e grava os eventos resultantes no ClickHouse.

Cada mensagem (`MdtMessage`) tem um tipo e um payload. O serviço converte o
payload em um ou mais eventos, acumula os eventos em lotes e insere cada lote
na tabela do tipo correspondente. A mensagem só é confirmada ao RabbitMQ depois
que seus eventos foram gravados; em caso de falha, ela vai para uma fila de
mensagens rejeitadas (`.dlq`).

São três tipos de mensagem: `Status`, `MachineState` e `Beacon`.

## Caminho de uma mensagem

```
RabbitMQ
  -> RabbitMqConsumerHostedService   recebe e desserializa a mensagem
  -> MdtMessageConsumer              escolhe o processor pelo tipo
  -> *Processor                      converte o payload em eventos
  -> EventSink                       enfileira e aguarda a gravação
  -> EventSinkWriter                 monta os lotes e os insere
  -> *EventAdapter                   grava o lote no ClickHouse
```

## Organização

```
src/
  Aiko.EventConsumer.Worker/         executável: Program.cs e appsettings.json
  Aiko.EventConsumer/                núcleo, sem dependência de infraestrutura
    Messages/                        mensagem de entrada e seus payloads
    Events/                          eventos, um por tabela
    Consumers/                       ponto de entrada de cada mensagem
    Processors/                      conversão de payload em eventos
      Parsing/                       leitura dos payloads em formato colunar
    Buffering/                       fila em memória e gravação em lotes
    MessagingExtensions.cs           registro das dependências do núcleo
  Aiko.EventConsumer.Infrastructure.RabbitMQ/     consumo da fila
  Aiko.EventConsumer.Infrastructure.ClickHouse/   inserção em lote
sql/tables/                          definição das tabelas
tests/                               testes de unidade
tools/                               gerador de mensagens para teste
```

## Como executar

Requisitos: .NET 10 e Docker. Os comandos partem da raiz do repositório.

```sh
# RabbitMQ e ClickHouse, já com as tabelas criadas
docker compose up -d --wait

# Aplicação (cria a fila ao iniciar; deve ser iniciada antes do gerador)
dotnet run --project src/Aiko.EventConsumer.Worker

# Gerador, em outro terminal: 50 mensagens a cada 1 segundo
dotnet run --project tools/Aiko.EventConsumer.MessageGenerator -- \
  --count 50 --interval 1

# Cliente do ClickHouse, para consultas
docker compose exec clickhouse \
  clickhouse-client -u dev --password dev -d aiko_mdt

# Testes
dotnet test
```

Consultas de exemplo:

```sql
SHOW TABLES;
SELECT count() FROM beacon_event;
SELECT * FROM status_event ORDER BY date DESC LIMIT 5 FORMAT Vertical;
```

Sem `--interval`, o gerador publica uma única rodada e encerra. Ele também
permite escolher os tipos (`--types`) e gerar mensagens com defeito
(`--invalid`); `--help` lista as opções.

O painel do RabbitMQ fica em http://localhost:15672 (usuário e senha `dev`).
