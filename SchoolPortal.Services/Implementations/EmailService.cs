using DnsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class EmailService : IEmailService
    {
        public Task<bool> IsEmailValidAsync(string email, bool checkDnsEntries = true)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                var host = mailAddress.Host;
                if (checkDnsEntries)
                    return CheckDnsEntriesAsync(host);

                return Task.FromResult(true);
            }
            catch (FormatException)
            {
                return Task.FromResult(false);
            }
        }

        async Task<bool> CheckDnsEntriesAsync(string domain)
        {
            try
            {
                var options = new LookupClientOptions { Timeout = TimeSpan.FromSeconds(5) };
                var lookup = new LookupClient(options);
                var result = await lookup.QueryAsync(domain, QueryType.ANY).ConfigureAwait(false);

                var records = result.Answers.Where(record => record.RecordType == DnsClient.Protocol.ResourceRecordType.A ||
                                                             record.RecordType == DnsClient.Protocol.ResourceRecordType.AAAA ||
                                                             record.RecordType == DnsClient.Protocol.ResourceRecordType.MX);
                return records.Any();
            }
            catch (DnsResponseException)
            {
                return false;
            }
        }
    }
}
