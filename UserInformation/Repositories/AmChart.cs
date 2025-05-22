using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserInformation.Model;

namespace UserInformation.Repositories
{
    public class AmChart : IAmChart
    {
        private readonly UserContext _context;
        private readonly DbSet<SaleData> _saleData;
        private readonly DbSet<StackChart> _stackChart; 
        private readonly DbSet<PieChart> _pieChart;
        private readonly DbSet<SankeyFlow> _sankeyFlow;
        private readonly DbSet<ColumnLine>_columnLine;
        
        public AmChart(UserContext context)
        {
            _context = context;
            _saleData = _context.Set<SaleData>();
            _stackChart = _context.Set<StackChart>();
            _pieChart = _context.Set<PieChart>();
            _sankeyFlow= _context.Set<SankeyFlow>();
            _columnLine = _context.Set<ColumnLine>();
        }

        public async Task<IEnumerable<SaleData>> GetAll()
        {
            return await _saleData.ToListAsync();
        }

        public async Task<IEnumerable<StackChart>> GetStack()
        {
            return await _stackChart.ToListAsync();
        }

        public async Task<IEnumerable<StackChartDto>> GetGroupedStackDto()
        {
            var data = await _context.Set<StackChart>()
                .GroupBy(s => s.Region)
                .Select(g => new StackChartDto
                {
                    Region = g.Key,
                    ProductA = g.Where(x => x.Product == "Product A").Sum(x => x.SalesAmount),
                    ProductB = g.Where(x => x.Product == "Product B").Sum(x => x.SalesAmount),
                }).ToListAsync();

            return data;
        }
        public async Task<IEnumerable<PieChart>> GetPieChart()
        {
            return await _pieChart.ToListAsync();
        }
        public async Task<ActionResult<IEnumerable<SankeyFlow>>> GetSankeyChart()
        {
            //var data = await _sankeyFlow
            //     .Select(x => new { x.from, x.to, x.value })
            //     .ToListAsync();
            //return new OkObjectResult(data);

            var data = await _sankeyFlow.ToListAsync();
            return new OkObjectResult(data);
        }

        public async Task<IEnumerable<ColumnLine>> GetColumnLines()
        {
            return await _columnLine.ToListAsync();
        }

    }
}
