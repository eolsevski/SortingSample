using FluentAssertions;
using SortingSample;
using Xunit;

namespace SortingSampleTests
{
    public class SotringMeasurementsTests
    {
        SortingMeasurements sortingMeasurements = new SortingMeasurements();

        [Theory]
        [InlineData("2011-08-11 16:59", 5, "2011-08-11 17:00")]
        [InlineData("2011-08-11 17:00", 5, "2011-08-11 17:00")]
        [InlineData("2011-08-11 17:01", 5, "2011-08-11 17:05")]
        public void RoundUpTimeTests(string date, int timeSpan, string result)
        {
            var time = sortingMeasurements.RoundUpTime(DateTime.Parse(date), TimeSpan.FromMinutes(timeSpan));

            time.Should().Be(DateTime.Parse(result));
        }

        [Fact]
        public void CreateSortedEntryTest()
        {
            var measurement = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:01"), measurementValue = 36.6 };
            var result = new KeyValuePair<DateTime, KeyValuePair<MeasurementType, Measurement>>(DateTime.Parse("2011-08-11 17:05"), new KeyValuePair<MeasurementType, Measurement>(measurement.measurementType, measurement));
            var sut = sortingMeasurements.CreateSortedEntry(measurement);

            sut.Should().Be(result);
        }

        [Fact]
        public void IsBetterNoKeyTest()
        {
            var measurement = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:01"), measurementValue = 36.6 };
            var sortedEntry = sortingMeasurements.CreateSortedEntry(measurement);
            var emptySortedDictionary = new SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>>();

            var sut = sortingMeasurements.IsBetter(sortedEntry, emptySortedDictionary);

            sut.Should().BeTrue();
        }

        [Fact]
        public void IsBetterNoShuchTypeEntryTest()
        {
            var measurement = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:01"), measurementValue = 36.6 };
            var sortedEntry = sortingMeasurements.CreateSortedEntry(measurement);
            var emptySortedDictionary = new SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>>();
            emptySortedDictionary.Add(DateTime.Parse("2011-08-11 17:05"), new Dictionary<MeasurementType, Measurement>());

            var sut = sortingMeasurements.IsBetter(sortedEntry, emptySortedDictionary);

            sut.Should().BeTrue();
        }

        [Fact]
        public void IsBetterNotNeedUpdateEntryTest()
        {
            var measurement = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:01"), measurementValue = 36.6 };
            var measurementBetter = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:04"), measurementValue = 38.6 };
            var sortedEntry = sortingMeasurements.CreateSortedEntry(measurement);
            var emptySortedDictionary = new SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>>();
            emptySortedDictionary.Add(DateTime.Parse("2011-08-11 17:05"), new Dictionary<MeasurementType, Measurement>() { { MeasurementType.Temp, measurement } });

            var sut = sortingMeasurements.IsBetter(sortedEntry, emptySortedDictionary);

            sut.Should().BeFalse();
        }
        [Fact]
        public void IsBetterNeedUpdateEntryTest()
        {
            var measurement = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:01"), measurementValue = 36.6 };
            var measurementBetter = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:04"), measurementValue = 38.6 };
            var sortedEntry = sortingMeasurements.CreateSortedEntry(measurementBetter);
            var sortedDictionary = new SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>>();
            sortedDictionary.Add(DateTime.Parse("2011-08-11 17:05"), new Dictionary<MeasurementType, Measurement>() { { MeasurementType.Temp, measurement } });

            var sut = sortingMeasurements.IsBetter(sortedEntry, sortedDictionary);

            sut.Should().BeTrue();
        }
        [Fact]
        public void UpdateMeasurementsNoKeyTest()
        {
            var measurement = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:01"), measurementValue = 36.6 };
            var sortedEntry = sortingMeasurements.CreateSortedEntry(measurement);
            var emptySortedDictionary = new SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>>();

            sortingMeasurements.UpdateMeasurements(sortedEntry, emptySortedDictionary);

            var result = new SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>>();
            result.Add(DateTime.Parse("2011-08-11 17:05"), new Dictionary<MeasurementType, Measurement>() { { MeasurementType.Temp, measurement } });

            emptySortedDictionary.Should().BeEquivalentTo(result);
            emptySortedDictionary[DateTime.Parse("2011-08-11 17:05")][MeasurementType.Temp].measurementValue.Should().Be(36.6);
        }
        [Fact]
        public void UpdateMeasurementsNewEntryTests()
        {
            var measurement = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:01"), measurementValue = 36.6 };
            var measurementBetter = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:04"), measurementValue = 38.6 };
            var sortedEntry = sortingMeasurements.CreateSortedEntry(measurementBetter);
            var emptySortedDictionary = new SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>>();
            emptySortedDictionary.Add(DateTime.Parse("2011-08-11 17:05"), new Dictionary<MeasurementType, Measurement>());

            sortingMeasurements.UpdateMeasurements(sortedEntry, emptySortedDictionary);

            var result = new SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>>();
            result.Add(DateTime.Parse("2011-08-11 17:05"), new Dictionary<MeasurementType, Measurement>() { { MeasurementType.Temp, measurementBetter } });

            emptySortedDictionary.Should().BeEquivalentTo(result);
            emptySortedDictionary[DateTime.Parse("2011-08-11 17:05")][MeasurementType.Temp].measurementValue.Should().Be(38.6);
        }
        [Fact]
        public void UpdateMeasurementsUpdateExistingTests()
        {
            var measurement = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:01"), measurementValue = 36.6 };
            var measurementBetter = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:04"), measurementValue = 38.6 };
            var sortedEntry = sortingMeasurements.CreateSortedEntry(measurementBetter);
            var sortedDictionary = new SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>>();
            sortedDictionary.Add(DateTime.Parse("2011-08-11 17:05"), new Dictionary<MeasurementType, Measurement>() { { MeasurementType.Temp, measurement } });

            sortingMeasurements.UpdateMeasurements(sortedEntry,sortedDictionary);

            var result = new SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>>();
            result.Add(DateTime.Parse("2011-08-11 17:05"), new Dictionary<MeasurementType, Measurement>() { { MeasurementType.Temp, measurementBetter } });

            sortedDictionary.Should().BeEquivalentTo(result);
            sortedDictionary[DateTime.Parse("2011-08-11 17:05")][MeasurementType.Temp].measurementValue.Should().Be(38.6);
        }
        [Fact]
        public void MapToResultTest()
        {
            var measurementTemp1 = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:01"), measurementValue = 31.6 };
            var measurementSpo21 = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:01"), measurementValue = 98.71 };
            var sortedMeasurement1 = sortingMeasurements.CreateSortedEntry(measurementTemp1);
            var sortedMeasurement2 = sortingMeasurements.CreateSortedEntry(measurementSpo21);
            var sortedMeasurementsByDate = new SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>>();
            sortingMeasurements.UpdateMeasurements(sortedMeasurement1, sortedMeasurementsByDate);
            sortingMeasurements.UpdateMeasurements(sortedMeasurement2, sortedMeasurementsByDate);

            sortedMeasurementsByDate.Count().Should().Be(1);
            sortedMeasurementsByDate[DateTime.Parse("2011-08-11 17:05")].Values.Count.Should().Be(2);

            
            var sut = sortingMeasurements.MapToResult(sortedMeasurementsByDate);
            
            sut.Count.Should().Be(2);
            
        }
        [Fact]
        public void SortingTest()
        {
            var measurementTemp1 = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:01"), measurementValue = 31.6 };
            var measurementTemp2 = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:04"), measurementValue = 32.6 };
            var measurementTemp3 = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:05"), measurementValue = 33.6 };
            var measurementTemp4 = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:06"), measurementValue = 34.6 };
            var measurementTemp5 = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:11"), measurementValue = 35.6 };
            var measurementTemp6 = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:14"), measurementValue = 36.6 };
            var measurementTemp7 = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:16"), measurementValue = 37.6 };
            var measurementTemp8 = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:18"), measurementValue = 38.6 };
            var measurementTemp9 = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:20"), measurementValue = 39.6 };

            var measurementSpo21 = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:01"), measurementValue = 98.71 };
            var measurementSpo22 = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:04"), measurementValue = 98.72 };
            var measurementSpo23 = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:05"), measurementValue = 98.73 };
            var measurementSpo24 = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:06"), measurementValue = 98.74 };
            var measurementSpo25 = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:11"), measurementValue = 98.75 };
            var measurementSpo26 = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:14"), measurementValue = 98.76 };
            var measurementSpo27 = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:16"), measurementValue = 98.77 };
            var measurementSpo28 = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:18"), measurementValue = 98.78 };
            var measurementSpo29 = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:20"), measurementValue = 98.79 };

            var startOfSampling = DateTime.Parse("2011-08-11 17:02");

            var unstapledMeasurements = new List<Measurement>() { 
                measurementSpo21,measurementSpo22, measurementSpo23, measurementSpo24, measurementSpo25, measurementSpo26, measurementSpo27,measurementSpo28, measurementSpo29,
                measurementTemp1, measurementTemp2, measurementTemp3, measurementTemp4, measurementTemp5, measurementTemp6, measurementTemp7, measurementTemp8, measurementTemp9};

            var sut = sortingMeasurements.Sort(startOfSampling, unstapledMeasurements);

            var measurementTemp3Res = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:05"), measurementValue = 33.6 };
            var measurementTemp4Res = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:10"), measurementValue = 34.6 };
            var measurementTemp6Res = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:15"), measurementValue = 36.6 };
            var measurementTemp9Res = new Measurement() { measurementType = MeasurementType.Temp, measurementTime = DateTime.Parse("2011-08-11 17:20"), measurementValue = 39.6 };
            var measurementSpo23Res = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:05"), measurementValue = 98.73 };
            var measurementSpo24Res = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:10"), measurementValue = 98.74 };
            var measurementSpo26Res = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:15"), measurementValue = 98.76 };
            var measurementSpo29Res = new Measurement() { measurementType = MeasurementType.Spo2, measurementTime = DateTime.Parse("2011-08-11 17:20"), measurementValue = 98.79 };

            var rez = new Dictionary<MeasurementType, List<Measurement>>();
            var tempMeasurements = new List<Measurement>() { measurementTemp3Res, measurementTemp4Res, measurementTemp6Res, measurementTemp9Res };
            rez.Add(MeasurementType.Temp, tempMeasurements);
            var spoMeasurements = new List<Measurement>() { measurementSpo23Res, measurementSpo24Res, measurementSpo26Res, measurementSpo29Res };
            rez.Add(MeasurementType.Spo2, spoMeasurements);

            sut.Should().BeEquivalentTo(rez);

        }
    }
}
