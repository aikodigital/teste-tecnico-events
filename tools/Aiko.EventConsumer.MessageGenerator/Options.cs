using System.Globalization;
using Aiko.EventConsumer.Messages;

namespace Aiko.EventConsumer.MessageGenerator;

internal record Options
{
    public const string Usage = """
        Uso: dotnet run -- [opções]

          --count <n>         mensagens por rodada (padrão: 10)
          --interval <s>      repete a rodada a cada <s> segundos;
                              sem esta opção, publica uma rodada e encerra
          --types <a,b,...>   tipos a gerar: Status, MachineState, Beacon (padrão: todos)
          --invalid           gera mensagens com o payload truncado, que falham ao serem
                              convertidas em eventos

        Exemplos:
          dotnet run -- --count 50 --interval 1
          dotnet run -- --count 5 --types Status,Beacon
          dotnet run -- --count 5 --invalid
        """;

    public int Count { get; private init; } = 10;

    public TimeSpan? Interval { get; private init; }

    public MdtMessageType[] Types { get; private init; } = Enum.GetValues<MdtMessageType>();

    public bool Invalid { get; private init; }

    public static Options? Parse(string[] args)
    {
        var options = new Options();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--invalid")
            {
                options = options with { Invalid = true };
                continue;
            }

            if (i + 1 >= args.Length)
            {
                return null;
            }

            string value = args[i + 1];

            switch (args[i++])
            {
                case "--count" when int.TryParse(value, out int count) && count > 0:
                    options = options with { Count = count };
                    break;

                case "--interval" when TryParseSeconds(value, out double seconds):
                    options = options with { Interval = TimeSpan.FromSeconds(seconds) };
                    break;

                case "--types" when TryParseTypes(value, out var types):
                    options = options with { Types = types };
                    break;

                default:
                    return null;
            }
        }

        return options;
    }

    private static bool TryParseSeconds(string value, out double seconds)
    {
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out seconds)
            && seconds > 0;
    }

    private static bool TryParseTypes(string value, out MdtMessageType[] types)
    {
        var parsed = new List<MdtMessageType>();
        var names = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (string name in names)
        {
            if (int.TryParse(name, out _) || !Enum.TryParse(name, ignoreCase: true, out MdtMessageType type))
            {
                types = [];
                return false;
            }

            parsed.Add(type);
        }

        types = parsed.Distinct().ToArray();
        return types.Length > 0;
    }
}
