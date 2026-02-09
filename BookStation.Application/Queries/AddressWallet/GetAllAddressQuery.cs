using BookStation.Domain.Repositories;
using BookStation.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BookStation.Application.Queries.AddressWallet;

public record GetAllAddressQuery(Guid UserId) : IRequest<List<AddressWalletDto>>;

public record AddressWalletDto
{
    public Guid Id { get; init; }
    public string RecipientName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string Ward { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string? PostalCode { get; init; }
    public AddressLabel Label { get; init; }
    public bool IsDefault { get; init; }
}

public class GetAllAddressesQueryHandler : IRequestHandler<GetAllAddressQuery, List<AddressWalletDto>>
{
    private readonly IAddressWalletRepository _addressRepository;

    public GetAllAddressesQueryHandler(IAddressWalletRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<List<AddressWalletDto>> Handle(GetAllAddressQuery request, CancellationToken cancellationToken)
    {
        var addresses = await _addressRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);

        var results = new List<AddressWalletDto>();

        // Map AddressWallet entities to AddressWalletDto
        foreach (var address in addresses)
        {
            results.Add(new AddressWalletDto
            {
                Id = address.Id,
                RecipientName = address.RecipientName,
                PhoneNumber = address.RecipientPhone.Value,
                Street = address.RecipientAddress.Street,
                Ward = address.RecipientAddress.Ward,
                City = address.RecipientAddress.City,
                Country = address.RecipientAddress.Country,
                PostalCode = address.RecipientAddress.PostalCode,
                Label = address.Label,
                IsDefault = address.IsDefault
            });
        }   

        return results;
    } 
}



//Projection trực tiếp trong DB
//namespace Application.Queries.Locations;

//public class GetLocationsByOrganizationQuery
//{
//    public Guid OrganizationId { get; set; }
//    public CurrentOrganizationUser LoggedInUser { get; set; }

//    public class Result
//    {
//        public Guid LocationId { get; set; }
//        public string LocationName { get; set; }
//        public string Address { get; set; }
//        public string RegionName { get; set; }
//        public Guid? RegionId { get; set; }
//        public string PostalCode { get; set; }
//        public bool IsFromPlanday { get; set; }
//        public int? DepartmentId { get; set; }
//    }

//    public class Handler : IConsumer<GetLocationsByOrganizationQuery>
//    {
//        private readonly ApplicationDbContext _dbContext;

//        public Handler(ApplicationDbContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        public async Task Consume(ConsumeContext<GetLocationsByOrganizationQuery> context)
//        {
//            var query = context.Message;

//            List<Result> items = await _dbContext
//                .Locations
//                .Where(p =>
//                    p.OrganizationId == query.OrganizationId &&
//                    !p.IsDeleted &&
//                    p.MemberLocations.Any(ml =>
//                        ml.OrganizationMember.AccountId ==
//                        query.LoggedInUser.AccountId))
//                .Include(p => p.Region)
//                .Include(p => p.MemberLocations)
//                    .ThenInclude(ml => ml.OrganizationMember)
//                .OrderBy(p => p.LocationName)
//                .Select(p => new Result
//                {
//                    LocationId = p.Id,
//                    LocationName = p.LocationName,
//                    Address = p.Address.Address,
//                    PostalCode = p.Address.PostalCode,
//                    RegionName = p.Region.RegionName ?? string.Empty,
//                    RegionId = p.RegionId,
//                    DepartmentId = p.DepartmentId,
//                    IsFromPlanday = p.IsFromPlanday
//                })
//                .ToListAsync();

//            await context.RespondAsync(items.ToArray());
//        }
//    }
//}
