namespace SortingSample
{
    public class SortingMeasurements
    {
        public Dictionary<MeasurementType, List<Measurement>> Sort(DateTime startOfSampling, List<Measurement> unstapledMeasurements)
        {
            var sortedMeasurementsByDate = new SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>>();

            foreach (var measurement in unstapledMeasurements)
            {
                if (measurement.measurementTime < startOfSampling) continue;

                var sortedEntry = CreateSortedEntry(measurement);

                if (IsBetter(sortedEntry, sortedMeasurementsByDate))
                {
                    UpdateMeasurements(sortedEntry, sortedMeasurementsByDate);
                }
            }
            return MapToResult(sortedMeasurementsByDate);
        }

        public Dictionary<MeasurementType, List<Measurement>> MapToResult(SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>> sortedMeasurementsByDate)
        {
            var rez = new Dictionary<MeasurementType, List<Measurement>>();
            foreach (var measurements in sortedMeasurementsByDate)
            {
                var date = measurements.Key;
                foreach (var measurement in measurements.Value)
                {
                    if (rez.ContainsKey(measurement.Value.measurementType))
                    {
                        rez[measurement.Value.measurementType].Add(new Measurement()
                        {
                            measurementTime = date,
                            measurementType = measurement.Value.measurementType,
                            measurementValue = measurement.Value.measurementValue,
                        });
                    }
                    else
                    {
                        rez[measurement.Value.measurementType] = new List<Measurement>() {
                            new Measurement()
                            {
                            measurementTime = date,
                            measurementType = measurement.Value.measurementType,
                            measurementValue = measurement.Value.measurementValue,
                            }
                        };
                    }
                }
            }
            return rez;
        }

        public void UpdateMeasurements(KeyValuePair<DateTime, KeyValuePair<MeasurementType, Measurement>> sortedEntry, SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>> sortedMeasurementsByDate)
        {
            if (!sortedMeasurementsByDate.ContainsKey(sortedEntry.Key))
            {
                sortedMeasurementsByDate[sortedEntry.Key] = new Dictionary<MeasurementType, Measurement>() { { sortedEntry.Value.Key, sortedEntry.Value.Value } };
                return;
            }
            if (!sortedMeasurementsByDate[sortedEntry.Key].ContainsKey(sortedEntry.Value.Key))
            {
                sortedMeasurementsByDate[sortedEntry.Key].Add(sortedEntry.Value.Key, sortedEntry.Value.Value);
                return;
            }
            sortedMeasurementsByDate[sortedEntry.Key][sortedEntry.Value.Key] = sortedEntry.Value.Value;
        }

        public bool IsBetter(KeyValuePair<DateTime, KeyValuePair<MeasurementType, Measurement>> sortedEntry, SortedDictionary<DateTime, Dictionary<MeasurementType, Measurement>> sortedMeasurementsByDate)
        {
            if (!sortedMeasurementsByDate.ContainsKey(sortedEntry.Key)) { return true; }

            if (!sortedMeasurementsByDate[sortedEntry.Key].ContainsKey(sortedEntry.Value.Key)) { return true; }

            var t1 = sortedEntry.Value.Value.measurementTime;
            var t2 = from measurements in sortedMeasurementsByDate[sortedEntry.Key] where measurements.Key == sortedEntry.Value.Key select measurements.Value.measurementTime;

            return DateTime.Compare(t1, t2.First()) > 0;
        }

        public KeyValuePair<DateTime, KeyValuePair<MeasurementType, Measurement>> CreateSortedEntry(Measurement measurement)
        {
            var interval = RoundUpTime(measurement.measurementTime, TimeSpan.FromMinutes(5));

            var measurementEntry = new KeyValuePair<MeasurementType, Measurement>(measurement.measurementType, measurement);

            return new KeyValuePair<DateTime, KeyValuePair<MeasurementType, Measurement>>(interval, measurementEntry);

        }

        public DateTime RoundUpTime(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }
    }
}