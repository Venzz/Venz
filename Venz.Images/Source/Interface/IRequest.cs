using System;
using System.Threading.Tasks;

namespace Venz.Images
{
    internal interface IRequest
    {
        Object Tag { get; }
        Task<Boolean> ProcessAsync(TaskFactory taskFactory);
        Boolean IsRequestFor(Picture picture);
    }
}
