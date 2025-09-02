using System.Text.Json.Serialization;

namespace ConstructionAPI.Models
{
    public class ConstructionObservation
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ConstructionData> Datas { get; set; } = new List<ConstructionData>();
    }

    public class ConstructionData
    {
        public DateTime SamplingTime { get; set; }
        public List<ConstructionProperty> Properties { get; set; } = new List<ConstructionProperty>();
    }

    public class ConstructionProperty
    {
        [JsonConverter(typeof(ObjectValueConverter))]
        public object Value { get; set; } = new object();
        public string Label { get; set; } = string.Empty;
    }

    public class UpdateConstructionDataRequest
    {
        public DateTime SamplingTime { get; set; }
        public List<ConstructionProperty> Properties { get; set; } = new List<ConstructionProperty>();
    }
}