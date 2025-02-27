using System.Threading.Tasks;
using AlloK8.DAL.Models;

namespace AlloK8.BLL.Common.Columns;

public interface IColumnService
{
    Task<Column> CreateColumnAsync(Column column);
}