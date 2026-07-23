using System.Collections;
using Paybills.API.Application.DTOs.Bill;
using Paybills.API.Application.DTOs.Receiving;
using Paybills.API.Application.DTOs.ReceivingType;
using Paybills.API.Domain.Entities;
using Paybills.API.Infrastructure.Helpers;

namespace Paybills.UnitTests.Utils;

static class ModelUtils
{
    #region Bill
    public static Bill GenerateRandomBill()
    {
        return new Bill
        {
            Id = (int)new Random().NextInt64(1, 99999),
            Value = new Random().NextSingle() * 1000,
            Month = (int)new Random().NextInt64(1, 13),
            Year = (int)new Random().NextInt64(2020, 2030)
        };
    }

    public static List<Bill> GenerateRandomBillsList(int size)
    {
        var result = new List<Bill>();

        while (size > 0)
        {
            result.Add(GenerateRandomBill());
            size--;
        }

        return result;
    }

    public static PagedList<Bill> GenerateRandomPagedBillsList(int size)
    {
        return new PagedList<Bill>(GenerateRandomBillsList(size), size, 1, 10);
    }
    #endregion

    #region BillDto
    public static BillDto GenerateRandomBillDto()
    {
        return new BillDto
        {
            Id = (int)new Random().NextInt64(1, 99999),
            Value = new Random().NextSingle() * 1000,
            Month = (int)new Random().NextInt64(1, 13),
            Year = (int)new Random().NextInt64(2020, 2030)
        };
    }

    public static List<BillDto> GenerateRandomBillDtoList(int size)
    {
        var result = new List<BillDto>();

        while (size > 0)
        {
            result.Add(GenerateRandomBillDto());
            size--;
        }

        return result;
    }
    #endregion

    #region BillType
    public static BillType GenerateRandomBillType()
    {
        return new BillType
        {
            Id = DataUtils.RandomInt(1, 50),
            Description = DataUtils.RandomString(50),
            Active = true
        };
    }
    #endregion

    #region Receivings
    public static Receiving GenerateRandomReceiving()
    {
        return new Receiving
        {
            Id = DataUtils.RandomInt(1, 1000),
            Value = new Random().NextSingle() * 1000,
        };
    }

    public static List<Receiving> GenerateRandomReceivingsList(int size)
    {
        var result = new List<Receiving>();

        while (size > 0)
        {
            result.Add(GenerateRandomReceiving());
            size--;
        }

        return result;
    }

    public static PagedList<Receiving> GenerateRandomPagedReceivingsList(int size)
    {
        return new PagedList<Receiving>(GenerateRandomReceivingsList(size), size, 1, 10);
    }

    public static List<ReceivingDto> GenerateRandomReceivingDtoList(int size)
    {
        var result = new List<ReceivingDto>();

        while (size > 0)
        {
            result.Add(GenerateRandomReceivingDto());
            size--;
        }

        return result;
    }

    private static ReceivingDto GenerateRandomReceivingDto()
    {
        return new ReceivingDto
        {
            Id = DataUtils.RandomInt(1, 1000),
            Value = new Random().NextSingle() * 1000
        };
    }
    #endregion
    #region ReceivingType
    public static ReceivingType GenerateRandomReceivingType()
    {
        return new ReceivingType
        {
            Id = DataUtils.RandomInt(1, 50),
            Description = DataUtils.RandomString(50),
            Active = true
        };
    }
    public static IEnumerable<ReceivingType> GenerateRandomReceivingTypesList(int size)
    {
        var result = new List<ReceivingType>();

        while (size > 0)
        {
            result.Add(GenerateRandomReceivingType());
            size--;
        }

        return result;
    }
    public static PagedList<ReceivingType> GenerateRandomPagedReceivingTypesList(int size)
    {
        return new PagedList<ReceivingType>(GenerateRandomReceivingTypesList(size), size, 1, 10);
    }
    public static List<ReceivingTypeDto> GenerateRandomReceivingTypeDtoList(int size)
    {
        var result = new List<ReceivingTypeDto>();

        while (size > 0)
        {
            result.Add(GenerateRandomReceivingTypeDto());
            size--;
        }

        return result;
    }
    public static ReceivingTypeDto GenerateRandomReceivingTypeDto()
    {
        return new ReceivingTypeDto
        {
            Id = DataUtils.RandomInt(1, 50),
            Description = DataUtils.RandomString(50),
            Active = true
        };
    }

    internal static ReceivingTypeRegisterDto GenerateRandomReceivingTypeRegisterDto()
    {
        return new ReceivingTypeRegisterDto
        {
            Description = DataUtils.RandomString(50),
            Active = true
        };
    }
    #endregion
}