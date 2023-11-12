using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RestaurantReservation.Core.Enums;
using RestaurantReservation.Data;
using RestaurantReservation.Data.Models;
using RestaurantReservation.Data.Models.Dto;
using RestaurantReservation.Data.Models.Entities;
using Serilog;
using System.Xml.Linq;

namespace RestaurantReservation.Api.Services
{
    public class ReservationService
    {
        private readonly ReservationDbContext _db;
        private readonly MailService _mailService;

        private List<TableDto> _tables;
        private SaveReservationDto _saveReservationDto;
        private TableDto _PreTable;
        private string _ReservationNo;

        public ReservationService(ReservationDbContext db, MailService mailService)
        {
            _db = db;
            _mailService = mailService;
        }

        public async Task<Result<List<ReservationSummary>>> GetAllReservation()
        {
            try
            {
                var vAllReservation = await _db.Reservations
                    .Where(reservation => reservation.ReservationDate.Date >= DateTime.Now.Date)
                    .Select(res => new ReservationSummary()
                    {
                        Id = res.Id,
                        ReservationDate = res.ReservationDate,
                        ApprovalDate = res.ApprovalDate,
                        CustomerName = res.CustomerName,
                        Status = res.Status,
                        NumberOfGuests = res.NumberOfGuests,
                        MailAddress = res.MailAddress,
                        PhoneNumber = res.PhoneNumber,
                        TableNo = res.Table.TableName

                    }).ToListAsync();

                return Result<List<ReservationSummary>>.PrepareSuccess(vAllReservation);
            }
            catch (Exception vEx)
            {
                Log.Error(vEx, "ReservationService GetAllReservation error");
                return Result<List<ReservationSummary>>.PrepareFailure("Rezervasyon Listesi alınırken hata oluştu.");
            }
        }
        public async Task<Result> UpdateReservation(ReservationInfo info)
        {
            try
            {
                var vReservationInfo = await _db.Reservations.FirstOrDefaultAsync(res => res.Id == info.Id);
                if (vReservationInfo == null)
                    return Result.PrepareFailure("Güncellenecek Rezervasyon Bulunamadı");

                _db.Reservations.Attach(vReservationInfo);

                vReservationInfo.ReservationDate = info.ReservationDate;
                vReservationInfo.ApprovalDate = info.ApprovalDate;
                vReservationInfo.Status = info.Status;
                vReservationInfo.CustomerName = info.CustomerName;
                vReservationInfo.PhoneNumber = info.PhoneNumber;
                vReservationInfo.MailAddress = info.MailAddress;


                await _db.SaveChangesAsync();

                return Result.PrepareSuccess("Rezervasyon Güncellendi.");
            }
            catch (Exception vEx)
            {
                Log.Error(vEx, "ReservationService UpdateReservation error");
                return Result.PrepareFailure("Rezervasyon Güncellemede Hata");
            }
        }
        public async Task<Result> DeleteReservation(int Id)
        {
            try
            {

                var vReservationInfo = await _db.Reservations.FirstOrDefaultAsync(res => res.Id == Id);
                if (vReservationInfo == null)
                    return Result.PrepareFailure("Silienecek Rezervasyon Bulunamadı");

                _db.Reservations.Attach(vReservationInfo);
                vReservationInfo.Deleted = true;

                await _db.SaveChangesAsync();


                return Result.PrepareSuccess("Rezervasyon Başarıyla Silindi");
            }
            catch (Exception vEx)
            {
                Log.Error(vEx, "ReservationService DeleteReservation error");
                return Result.PrepareFailure("Rezervasyon Silmede Hata");
            }
        }
        public async Task<Result> SaveReservation(SaveReservationDto dto)
        {
            try
            {
                _saveReservationDto = dto;

                var vResult = CheckReservationInformation();
                if (vResult.Failed)
                    return vResult;

                var vGetTables = await GetTables();
                if (vGetTables.Failed)
                    return vGetTables;

                var vCheckTable = await TableAvailabilityCheck();
                if (vCheckTable.Failed)
                    return vCheckTable;

                var vCreateReservation = await CreateReservation();
                if (vCreateReservation.Failed)
                    return vCreateReservation;

                await _db.SaveChangesAsync();

                var vSendReservationMail = await SendReservationMail();
                if (vSendReservationMail.Failed)
                    return vSendReservationMail;

                return Result.PrepareSuccess("Rezervasyon İşlemi Başarıyla Gerçekleştirildi");
            }
            catch (Exception vEx)
            {
                Log.Error(vEx, "ReservationService SaveReservation error");
                return Result.PrepareFailure("Rezervasyon işleminda hata oluştu.Lütfen restoran yetkilisi ile görüşün !");
            }
        }



        private async Task<Result> GetTables()
        {
            try
            {
                _tables = await _db.Tables
                    .Include(table => table.Reservations)
                    .Where(table =>

                        !table.Reservations
                        .Any(res => res.ReservationDate.Date == _saveReservationDto.ReservationDate.Date
                                      && res.Status == ReservationStatuses.Reservation
                                      && res.TableId == table.Id))
                    .Select(table => new TableDto
                    {
                        Id = table.Id,
                        TableName = table.TableName,
                        TableCapacity = table.TableCapacity
                    }).ToListAsync();

                return _tables.Count > 0 ? Result.PrepareSuccess()
                    : Result.PrepareFailure("Uygun masa Bulunamadı !");
            }
            catch (Exception vEx)
            {
                Log.Error(vEx, "ReservationService GetTables error");
                return Result.PrepareFailure("Masa Listsi alınırken hata oluştu");
            }
        }
        private async Task<Result> TableAvailabilityCheck()
        {
            try
            {
                foreach (var tableDto in _tables)
                {
                    if (tableDto.TableCapacity >= _saveReservationDto.NumberOfGuests)
                    {
                        _PreTable = tableDto;
                        break;
                    }
                }

                return _PreTable != null
                    ? Result.PrepareSuccess()
                    : Result.PrepareFailure("Belirtilen tarihte uygun masa bulunamadı !");
            }
            catch (Exception vEx)
            {
                Log.Error(vEx, "ReservationService TableAvailabilityCheck error");
                return Result.PrepareFailure("Uygun Masa kontrol edilirken hata oluştu");
            }
        }
        private async Task<Result> CreateReservation()
        {
            try
            {
                var vReservationNo = await _db.Set<DbGeneratedValue>()
                    .FromSqlRaw($"execute [dbo].[SP_ReservationNo]")
                    .ToListAsync();

                if (vReservationNo == null)
                    return Result.PrepareFailure("Rezervasyon No Oluşturulamadı");

                _ReservationNo = vReservationNo.FirstOrDefault().Value;
                var vNewReservation = new Data.Models.Entities.Reservation()
                {
                    CustomerName = _saveReservationDto.CustomerName,
                    ReservationDate = _saveReservationDto.ReservationDate,
                    MailAddress = _saveReservationDto.MailAddress,
                    NumberOfGuests = _saveReservationDto.NumberOfGuests,
                    ApprovalDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    PhoneNumber = _saveReservationDto.PhoneNumber,
                    ReservationNo = vReservationNo.FirstOrDefault().Value,
                    TableId = _PreTable.Id,
                    Status = ReservationStatuses.PreReservation
                };

                _db.Reservations.Add(vNewReservation);

                return Result.PrepareSuccess();
            }
            catch (Exception vEx)
            {
                Log.Error(vEx, "ReservationService CreateReservation error");
                return Result.PrepareFailure("Rezervasyon eklenirken bir hata oluştu");
            }
        }
        private async Task<Result> SendReservationMail()
        {
            try
            {
                var vMailInfo = new MailInfo()
                {
                    Subject = "Rezervayon İşleminiz",
                    To = _saveReservationDto.MailAddress,
                    Body = $"Sayın {_saveReservationDto.CustomerName}, rezervasyonunuz başarıyla alındı. Masa No: {_PreTable.TableName}, Tarih: {_saveReservationDto.ReservationDate.ToString("d")}, Kişi Sayısı: {_saveReservationDto.NumberOfGuests}"
                };

                var vResult = await _mailService.SendMail(vMailInfo);
                if (vResult.Failed)
                    return vResult;

                return Result.PrepareSuccess();
            }
            catch (Exception vEx)
            {
                Log.Error(vEx, "ReservationService SendReservationMail error");
                return Result.PrepareFailure("Mail GÖnderilirken bir hata oluştu !");
            }
        }



        private Result CheckReservationInformation()
        {
            try
            {
                if (_saveReservationDto.ReservationDate <= DateTime.Now.AddHours(-1))
                    return Result.PrepareFailure("Geçmiş tarihli rezervasyon yapamzsınız");

                if (_saveReservationDto.NumberOfGuests<=0)
                    return Result.PrepareFailure("Geçerli Misafir sayısı girin");

                if (!IsValidEmail())
                    return Result.PrepareFailure("Geçerli Mail Adresi girin");

                if (!IsNumeric())
                    return Result.PrepareFailure("Geçreli bir telefon numarası giriniz");

                return Result.PrepareSuccess();
            }
            catch (Exception vEx)
            {
                Log.Error(vEx, "ReservationService CheckReservationInformation error");
                return Result.PrepareFailure("Lütfen bilgilerini geçerli girin");
            }
        }


        public bool IsNumeric()
        {
            return _saveReservationDto.PhoneNumber.All(char.IsNumber);
        }
        private bool IsValidEmail()
        {
            var trimmedEmail = _saveReservationDto.MailAddress.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(_saveReservationDto.MailAddress);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
