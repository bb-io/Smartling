using Apps.Smartling.Api;
using Apps.Smartling.Models.Dtos;
using Apps.Smartling.Models.Dtos.Issues;
using Apps.Smartling.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Smartling.DataSourceHandlers;

public class AssigneeDataSourceHandler : SmartlingInvocable, IAsyncDataSourceHandler
{
    public AssigneeDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var getProjectRequest = new SmartlingRequest($"/projects-api/v2/projects/{ProjectId}", Method.Get);
        var getProjectResponse = await Client.ExecuteWithErrorHandling<ResponseWrapper<ProjectDto>>(getProjectRequest);
        var accountUid = getProjectResponse.Response.Data.AccountUid;
    
        var getUsersRequest = new SmartlingRequest($"/people-api/v2/accounts/{accountUid}/users-general-list", 
            Method.Post);
        getUsersRequest.AddJsonBody(new
        {
            keyword = context.SearchString ?? "",
            limit = 20
        });
        
        var response = await Client.ExecuteWithErrorHandling<ResponseWrapper<ItemsWrapper<UserDto>>>(getUsersRequest);
        var users = response.Response.Data.Items
            .ToDictionary(user => user.UserUid, user => $"{user.FirstName} {user.LastName}");
        users["-1"] = "Unassigned";
        return users;
    }
}