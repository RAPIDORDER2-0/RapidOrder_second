namespace RapidOrder.Api.Controllers.Nigrum.Requests;

public sealed record EmployeeBreakCommand(long EmployeeId, EmployeeBreakRequest? Payload);
