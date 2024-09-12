using MassTransit;

namespace Common;

public class KebabCaseFormatter :
    KebabCaseEndpointNameFormatter,
    IEntityNameFormatter
{
    private readonly string _topicSuffix;
    private readonly string _queueSuffix;

    public KebabCaseFormatter(
        string prefix,
        string topicSuffix = "topic",
        string queueSuffix = "queue",
        bool includeNamespace = false) :
        base(prefix, includeNamespace)
    {
        _topicSuffix = topicSuffix;
        _queueSuffix = queueSuffix;
    }

    public string FormatEntityName<T>()
    {
        var messageName = GetMessageName(typeof(T));
        return string.IsNullOrWhiteSpace(_topicSuffix)
            ? messageName
            : $"{messageName}-{_topicSuffix}";
    }

    public override string Consumer<T>()
    {
        var consumerName = base.Consumer<T>();
        return string.IsNullOrWhiteSpace(_queueSuffix)
            ? consumerName
            : $"{consumerName}-{_queueSuffix}";
    }
}