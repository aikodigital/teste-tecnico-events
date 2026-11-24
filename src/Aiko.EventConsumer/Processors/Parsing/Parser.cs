using System.Collections.Immutable;
using System.Globalization;

namespace Aiko.EventConsumer.Processors.Parsing;

internal abstract class Parser<T>
{
    private readonly Dictionary<string, string> _parameters;
    private readonly Dictionary<string, string[]> _propertyValues;
    private readonly DateTime _startDate;
    private readonly TimeSpan _period;
    private readonly int _rowCount;

    private int _current = 1;

    public Parser(string siteId, long equipmentId, ulong hash, string value)
    {
        SiteId = siteId;
        EquipmentId = equipmentId;
        Hash = hash;

        var lines = value.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        _parameters = lines[0].Split(',').Select(p => p.Split('=')).ToDictionary(p => p[0], p => p[1]);
        _propertyValues = lines[1..].Select(p => p.Split(',')).ToDictionary(p => p[0], p => p);
        Properties = _propertyValues.Keys.ToImmutableHashSet();

        long t0 = long.Parse(_parameters.GetValueOrDefault("t0", "0"), CultureInfo.InvariantCulture);
        double p = double.Parse(_parameters.GetValueOrDefault("p", "0"), CultureInfo.InvariantCulture);
        int n = int.Parse(_parameters.GetValueOrDefault("n", "0"), CultureInfo.InvariantCulture);

        _startDate = DateTimeOffset.FromUnixTimeMilliseconds(t0).DateTime;
        _period = p < 100 ? TimeSpan.FromSeconds(p) : TimeSpan.FromMilliseconds(p);
        _rowCount = n;
    }

    protected string SiteId { get; }

    protected long EquipmentId { get; }

    protected ulong Hash { get; }

    protected ImmutableHashSet<string> Properties { get; }

    public IEnumerable<T> Parse()
    {
        for (_current = 1; _current <= _rowCount; _current++)
        {
            yield return ParseRow(_current - 1);
        }
    }

    protected abstract T ParseRow(int index);

    protected int? GetInt(string property)
    {
        string? value = GetString(property);
        return string.IsNullOrEmpty(value) ? null : int.Parse(value, CultureInfo.InvariantCulture);
    }

    protected long? GetLong(string property)
    {
        string? value = GetString(property);
        return string.IsNullOrEmpty(value) ? null : long.Parse(value, CultureInfo.InvariantCulture);
    }

    protected double? GetDouble(string property)
    {
        string? value = GetString(property);
        return string.IsNullOrEmpty(value) ? null : double.Parse(value, CultureInfo.InvariantCulture);
    }

    protected float? GetFloat(string property)
    {
        string? value = GetString(property);
        return string.IsNullOrEmpty(value) ? null : float.Parse(value, CultureInfo.InvariantCulture);
    }

    protected bool? GetBool(string property)
    {
        string? value = GetString(property);
        return string.IsNullOrEmpty(value)
            ? null
            : "true".Equals(value, StringComparison.InvariantCultureIgnoreCase);
    }

    protected string? GetString(string property)
    {
        if (_propertyValues.TryGetValue(property, out string[]? values))
        {
            string? value = values[_current];

            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
        }

        return null;
    }

    protected DateTime GetDateTime()
    {
        return _startDate + (_current - 1) * _period;
    }

    protected DateTime? GetDateTime(string property)
    {
        double? deltaMs = GetDouble(property);

        return deltaMs.HasValue
            ? _startDate.AddMilliseconds(deltaMs.Value)
            : null;
    }
}
