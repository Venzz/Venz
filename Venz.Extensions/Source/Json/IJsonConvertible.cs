using Windows.Data.Json;

namespace Venz.Extensions
{
    public interface IJsonConvertible
    {
        IJsonValue ToJsonValue();
    }
}
