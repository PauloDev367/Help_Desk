using Api.ViewModels;
using Application.Exceptions;
using Domain.DomainExceptions;
using System.Text.Json;

namespace Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate requestDelegate)
    {
        _next = requestDelegate;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var errorDetail = new ErrorDetailsVM
        {
            StatusCode = 500,
            Message = "Internal server error",
        };

        try
        {
            await _next(httpContext);
        }
        catch (ClientNotFoundException ex)
        {
            await HandleExceptionAsync(httpContext, ex, 404);
        }
        catch (InvalidSupportException ex)
        {
            await HandleExceptionAsync(httpContext, ex, 400);
        }
        catch (SupportNotFoundedException ex)
        {
            await HandleExceptionAsync(httpContext, ex, 404);
        }
        catch (TicketNotFoundedException ex)
        {
            await HandleExceptionAsync(httpContext, ex, 404);
        }
        catch (UserNotFoundedException ex)
        {
            await HandleExceptionAsync(httpContext, ex, 404);
        }
        catch (InvalidCommentException ex)
        {
            await HandleExceptionAsync(httpContext, ex, 400);
        }
        catch (SupportCannotCreateNewTicketException ex)
        {
            await HandleExceptionAsync(httpContext, ex, 400);
        }
        catch (TicketAlreadyCancelledException ex)
        {
            await HandleExceptionAsync(httpContext, ex, 400);
        }
        catch (TicketAlreadyFinishedException ex)
        {
            await HandleExceptionAsync(httpContext, ex, 400);
        }
        catch (TicketCancelledException ex)
        {
            await HandleExceptionAsync(httpContext, ex, 400);
        }
        catch (TicketFinishedException ex)
        {
            await HandleExceptionAsync(httpContext, ex, 400);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, 500);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, int statusCode)
    {
        var errorDetail = new ErrorDetailsVM
        {
            StatusCode = statusCode,
            Message = exception.Message
        };

        Console.WriteLine($"Exception: {exception.GetType().Name}, Message: {exception.Message}");
        await WriteHttpResponseAsync(errorDetail, context);
    }

    private async Task WriteHttpResponseAsync(ErrorDetailsVM errorDetail, HttpContext context)
    {
        context.Response.StatusCode = errorDetail.StatusCode;
        context.Response.ContentType = "application/json";
        var errorJson = JsonSerializer.Serialize(errorDetail);
        await context.Response.WriteAsync(errorJson);
    }
}
