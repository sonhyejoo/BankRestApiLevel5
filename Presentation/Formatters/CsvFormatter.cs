using System.Text;
using BankRestApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace BankRestApi.Formatters;

public class CsvOutputFormatter : TextOutputFormatter
{
    public CsvOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
        SupportedEncodings.Add(Encoding.UTF8);
    }

    protected bool CanWriteResult(Type? type)
        => typeof(IEnumerable<Account>).IsAssignableFrom(type);

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var httpContext = context.HttpContext;
        var serviceProvider = httpContext.RequestServices;
    
        var logger = serviceProvider.GetService<ILogger<CsvOutputFormatter>>();
        var buffer = new StringBuilder();
    
        buffer.AppendLine("Id, Name, Balance");
    
        IEnumerable<Account> accounts = (IEnumerable<Account>)context.Object;
        foreach (var account in accounts)
        {
            FormatCsv(buffer, account, logger);
        }
        
        await httpContext.Response.WriteAsync(buffer.ToString(), selectedEncoding);    
    }
    
    private void FormatCsv(StringBuilder buffer, Account account, ILogger logger)
    {
        var newLine = $"{Escape(account.Id)}, {Escape(account.Name)}, {Escape(account.Balance)}";
        buffer.AppendLine(newLine);
        
        logger.LogInformation("Writing {Id}, {Name}, {Balance}", account.Id, account.Name, account.Balance);
    }

    private static readonly char[] SpecialChars = [',', '\n', '\r', '"'];

    private string Escape(object o)
    {
        if (o is null)
        {
            return "";
        }

        string field = o.ToString();
        if (field.IndexOfAny(SpecialChars) != -1)
        {
            return String.Format("\"{0}\"", field.Replace("\"", "\"\""));
        }
        else return field;
    }
}