using AutoMapper;
using BankMicroservices.Client.Data.ValueObjects;
using BankMicroservices.Client.Model;

namespace BankMicroservices.Client.Config
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config => {
                config.CreateMap<UserVO, User>()
                .ForPath(dest => dest.AccountNumber, input => input.MapFrom(i => Int32.Parse(i.BankingDetails.AccountNumber)))
                .ForPath(dest => dest.Agency, input => input.MapFrom(i => Int32.Parse(i.BankingDetails.Agency)))
                .ForPath(dest => dest.Balance, input => input.MapFrom(i => i.BankingDetails.Balance))
                .ReverseMap()
                .ForPath(dest => dest.BankingDetails.AccountNumber, input => input.MapFrom(i => i.AccountNumber.ToString()))
                .ForPath(dest => dest.BankingDetails.Balance, input => input.MapFrom(i => i.Balance))
                .ForPath(dest => dest.BankingDetails.Agency, input => input.MapFrom(i => i.Agency.ToString().PadLeft(3, '0')));
                config.CreateMap<UserProfilePictureVO, UserProfilePicture>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
