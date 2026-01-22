using AutoMapper;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Domain.Entities;

namespace WhatsAppBusiness.Application.Mappings;

/// <summary>
/// AutoMapper profile for entity to DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value));

        // Operator mappings
        CreateMap<Operator, OperatorDto>();

        // Message mappings
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content.Value));

        // Conversation mappings
        CreateMap<Conversation, ConversationDto>()
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedOperator, opt => opt.Ignore())
            .ForMember(dest => dest.Messages, opt => opt.Ignore());
    }
}
