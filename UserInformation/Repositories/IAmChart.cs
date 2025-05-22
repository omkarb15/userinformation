using Microsoft.AspNetCore.Mvc;
using UserInformation.Model;

namespace UserInformation.Repositories
{
    public interface IAmChart
    {
        Task<IEnumerable<SaleData>> GetAll();
        Task<IEnumerable<StackChart>> GetStack();
        Task<IEnumerable<StackChartDto>> GetGroupedStackDto();
        Task<IEnumerable<PieChart>> GetPieChart();
        Task<ActionResult<IEnumerable<SankeyFlow>>> GetSankeyChart();
        Task<IEnumerable<ColumnLine>>GetColumnLines();




    }


}
