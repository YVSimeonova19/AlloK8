using System.Collections.Generic;
using System.Threading.Tasks;
using AlloK8.Common.Models.User;

namespace AlloK8.BLL.Common.Search;

public interface ISearchService
{
    Task<List<UserVM>> SearchUsersByEmailAsync(string email);

    Task<List<UserVM>> SearchTaskUsersByEmailAsync(string email, int projectId);
}