using Infrastructure.Audilog;

namespace Application.Logs.Querys
{
    public interface IGetLogsQueryHandler
    {
        Task<IEnumerable<LogDto>> Handle(GetLogsQuery request, CancellationToken cancellationToken);
    }
}