using AutoMapper;
using BankMicroservices.Log.Data.ValueObjects;
using BankMicroservices.Log.Model;

namespace BankMicroservices.Log.Config
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config => {
                config.CreateMap<LogVO, LogModel>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
