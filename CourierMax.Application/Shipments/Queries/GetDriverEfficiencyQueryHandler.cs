using CourierMax.Application.Shipments.DTOs;
using CourierMax.Application.Shipments.Interfaces;

namespace CourierMax.Application.Shipments.Queries
{
    public class GetDriverEfficiencyQueryHandler
    {
        private readonly IShipmentQueryService _queryService;

        public GetDriverEfficiencyQueryHandler(IShipmentQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<DriverEfficiencyReportDto> HandleAsync(GetDriverEfficiencyQuery query)
        {
            return await _queryService.GetDriverEfficiencyReportAsync(query.DriverId, query.StartDate, query.EndDate);
        }
    }
}
