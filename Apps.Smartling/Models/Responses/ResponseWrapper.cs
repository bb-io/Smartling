namespace Apps.Smartling.Models.Responses;

public record ResponseWrapper<TData>(ResponseData<TData> Response);

public record ResponseData<TData>(string Code, TData Data);