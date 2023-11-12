using Serilog;
using System.Net.Mail;
using RestaurantReservation.Data.Models.Dto;

namespace RestaurantReservation.Api.Services
{
    public class MailService
    {

        private readonly SmtpClient _client;

        public MailService(SmtpClient client)
        {
            _client = client;
        }

        public async Task<Result> SendMail(MailInfo info)
        {
            try
            {
                _client.UseDefaultCredentials = false;
                _client.EnableSsl = true;
                _client.Send(new MailMessage
                {
                    To = { info.To },
                    Subject = info.Subject,
                    Body = info.Body
                });

                return Result.PrepareSuccess();
            }
            catch (Exception vEx)
            {
                Log.Error(vEx, "MailService SendMail error");
                return Result.PrepareFailure("Mail gönderme  işleminde bir hata oluştu.Hata Kodu:" + vEx.Message);
            }
        }

    }
}
