using BookStation.Query.Abstractions;
using BookStation.Query.Common;
using BookStation.Query.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Query.Queries.AddressWallet;

public sealed record GetAllAddressQuery(Guid UserId, int Page = 1) : IRequest<PagedResult<AddressWalletDto>>;

public sealed class GetAllAddressQueryHandler : IRequestHandler<GetAllAddressQuery, PagedResult<AddressWalletDto>>
{
    private readonly IReadDbContext _db;
    private const int PageSize = 5;

    public GetAllAddressQueryHandler(IReadDbContext db) => _db = db;

    public async Task<PagedResult<AddressWalletDto>> Handle(GetAllAddressQuery request, CancellationToken cancellationToken)
    {
        var query = _db.AddressWallets
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.IsDefault)
            .Skip((request.Page - 1) * PageSize)
            .Take(PageSize)
            .Select(x => new AddressWalletDto
            {
                Id = x.Id,
                RecipientName = x.RecipientName,
                PhoneNumber = x.RecipientPhone.Value,
                Street = x.RecipientAddress.Street,
                Ward = x.RecipientAddress.Ward,
                City = x.RecipientAddress.City,
                Country = x.RecipientAddress.Country,
                PostalCode = x.RecipientAddress.PostalCode,
                Label = x.Label,
                IsDefault = x.IsDefault
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AddressWalletDto>(items, totalCount, request.Page, PageSize);
    }
}


//public record GetAllAddressQuery(Guid UserId) : IRequest<List<AddressWalletDto>>;
//public class GetAllAddressesQueryHandler : IRequestHandler<GetAllAddressQuery, List<AddressWalletDto>>
//{
//    private readonly IAddressWalletRepository _addressRepository;

//    public GetAllAddressesQueryHandler(IAddressWalletRepository addressRepository)
//    {
//        _addressRepository = addressRepository;
//    }

//    public async Task<List<AddressWalletDto>> Handle(GetAllAddressQuery request, CancellationToken cancellationToken)
//    {
//        var addresses = await _addressRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);

//        var results = new List<AddressWalletDto>();

//        // Map AddressWallet entities to AddressWalletDto
//        foreach (var address in addresses)
//        {
//            results.Add(new AddressWalletDto
//            {
//                Id = address.Id,
//                RecipientName = address.RecipientName,
//                PhoneNumber = address.RecipientPhone.Value,
//                Street = address.RecipientAddress.Street,
//                Ward = address.RecipientAddress.Ward,
//                City = address.RecipientAddress.City,
//                Country = address.RecipientAddress.Country,
//                PostalCode = address.RecipientAddress.PostalCode,
//                Label = address.Label,
//                IsDefault = address.IsDefault
//            });
//        }   

//        return results;
//    } 
//}



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