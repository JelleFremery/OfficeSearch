using Microsoft.CodeAnalysis;
using OfficeSearch.Models;
using System.Text;

namespace OfficeSearch.Services.Indexing;

public static class DataGenerator
{
    private static readonly Random random = new();

    public static List<Office> GetOffices(long count)
    {
        List<Office> officeList = [];
        for (int i = 0; i < count; i++)
        {
            officeList.Add(GetNewOffice(i));
        }

        return officeList;
    }

    private static Office GetNewOffice(int id)
    {
        return new()
        {
            OfficeId = id.ToString(),
            OfficeName = GetOfficeNameById(id),
            Description = GetRandomString(100),
            DescriptionNl = GetRandomString(100),
            DescriptionSr = GetRandomString(100),
            Category = GetCategoryById(id),
            Tags = [GetRandomString(8), "tosti machine"],
            NrOfParkingSpots = random.Next(5, 100),
            LastRenovationDate = new DateTimeOffset(random.Next(1980, 2023), random.Next(1, 12), 1, 0, 0, 0, TimeSpan.Zero),
            Rating = random.NextDouble() * 5,
            Address = new Address()
            {
                StreetAddress = GetRandomString(20),
                City = GetCityNameById(id),
                StateProvince = GetRandomString(2),
                PostalCode = "1234 AB",
                Country = id == 1 ? "Serbia" : "Netherlands"
            },
            Desks = GetDesks(id).ToArray()
        };
    }

    private static string GetOfficeNameById(int id)
    {
        if (id <= 6)
        {
            return $"Betabit {GetCityNameById(id)}";
        }
        return $"Customer office #{id - 6}";
    }

    private static string GetCityNameById(int id)
    {
        return id.ToString() switch
        {
            "0" => "Rotterdam",
            "1" => "Belgrade",
            "2" => "Eindhoven",
            "3" => "Utrecht",
            "4" => "Amsterdam",
            "5" => "Den Haag",
            "6" => "Apeldoorn",
            _ => GetRandomString(12)
        };
    }

    private static string GetCategoryById(int id)
    {
        if (id >= 7) return "Customer";
        if (id == 0 || id == 1) return "Betabit HQ";
        return "Betabit";
    }

    private static string GetRandomString(int length)
    {
        StringBuilder sb = new();
        char letter;

        for (int i = 0; i < length; i++)
        {
            double flt = random.NextDouble();
            int shift = Convert.ToInt32(Math.Floor(25 * flt));
            letter = Convert.ToChar(shift + 65);
            sb.Append(letter);
        }
        return sb.ToString();
    }

    private static IEnumerable<Desk> GetDesks(int id)
    {
        int nrOfDesks = random.Next(2, 20);
        for (int i = 0; i < nrOfDesks; i++)
        {
            yield return new()
            {
                HeightAdjustable = id % 2 == 0,
                Location = id % 2 == 0 ? "Ground floor" : "First floor",
                Length = random.Next(50, 70),
                Width = random.Next(100, 150),
                Name = GetRandomString(10),
                Tags = ["coffee maker"]
            };
        }
    }
}