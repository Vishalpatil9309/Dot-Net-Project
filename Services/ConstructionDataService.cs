using ConstructionAPI.Models;
using Newtonsoft.Json;

namespace ConstructionAPI.Services
{
    public interface IConstructionDataService
    {
        Task<ConstructionObservation?> GetConstructionDataAsync();
        Task<bool> UpdateConstructionDataAsync(DateTime samplingTime, List<ConstructionProperty> properties);
        Task<ConstructionData?> GetConstructionDataBySamplingTimeAsync(DateTime samplingTime);
    }

    public class ConstructionDataService : IConstructionDataService
    {
        private readonly string _dataFilePath;
        private readonly ILogger<ConstructionDataService> _logger;

        public ConstructionDataService(ILogger<ConstructionDataService> logger)
        {
            _logger = logger;
            _dataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "data.json");
            
            // Ensure the Data directory exists
            var dataDirectory = Path.GetDirectoryName(_dataFilePath);
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory!);
            }
        }

        public async Task<ConstructionObservation?> GetConstructionDataAsync()
        {
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    _logger.LogWarning("Data file not found at {FilePath}", _dataFilePath);
                    return null;
                }

                var jsonContent = await File.ReadAllTextAsync(_dataFilePath);
                var data = JsonConvert.DeserializeObject<ConstructionObservation>(jsonContent);
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading construction data from file");
                return null;
            }
        }

        public async Task<ConstructionData?> GetConstructionDataBySamplingTimeAsync(DateTime samplingTime)
        {
            var observation = await GetConstructionDataAsync();
            if (observation == null) return null;

            return observation.Datas.FirstOrDefault(d => d.SamplingTime.Date == samplingTime.Date);
        }

        public async Task<bool> UpdateConstructionDataAsync(DateTime samplingTime, List<ConstructionProperty> properties)
        {
            try
            {
                var observation = await GetConstructionDataAsync();
                if (observation == null) return false;

                var existingData = observation.Datas.FirstOrDefault(d => d.SamplingTime == samplingTime);
                if (existingData != null)
                {
                    existingData.Properties = properties;
                }
                else
                {
                    observation.Datas.Add(new ConstructionData
                    {
                        SamplingTime = samplingTime,
                        Properties = properties
                    });
                }

                var jsonContent = JsonConvert.SerializeObject(observation, Formatting.Indented);
                await File.WriteAllTextAsync(_dataFilePath, jsonContent);
                
                _logger.LogInformation("Construction data updated successfully for sampling time {SamplingTime}", samplingTime);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating construction data");
                return false;
            }
        }
    }
}