using BookStation.Core.SharedKernel;
using BookStation.Domain.Enums;
using BookStation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace BookStation.Domain.Entities.UserAggregate;

public class SellerProfile : Entity<Guid>
{
    public SellerStatus Status { get; private set; }
    public int? OrganizationId { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public Gender? Gender { get; private set; }
    public Address? Address { get; private set; }
    public string? IdNumber { get; private set; } // CMND/CCCD
    public DateTime? ApprovedAt { get; private set; } 
    public User? User { get; private set; }

    private SellerProfile() { }

    /// <summary>
    /// Create a new seller profile. Tai day bat buoc phai su dung internal tai vi seller khong duoc phep tao truc tiep ma phai tao thong qua User (Seller nay la con cua User)
    /// </summary>
    internal static SellerProfile Create(Guid userId, int? organiztionId)
    {
        return new SellerProfile
        {
            Id = userId,
            Status = SellerStatus.Pending,
            OrganizationId = organiztionId,
            CreatedAt = DateTime.UtcNow
        };
    }
    public void UpdateProfile(DateTime? dateOfBirth, Gender? gender, string? idNumber, Address? address) 
    {
        DateOfBirth = dateOfBirth;
        Gender = gender;
        IdNumber = idNumber;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }
    public void ApproveSeller()
    {
        if (Status == SellerStatus.Active) return;
        Status = SellerStatus.Active;
        ApprovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public void RejectSeller()
    {
        Status = SellerStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }
    public void SuspendSeller(string reason)
    {
        Status = SellerStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }
    public void BanSeller(string reason)
    {
        Status = SellerStatus.Banned;
        UpdatedAt = DateTime.UtcNow;
    }
    public void SetOrganization(int? organizationId)
    {
        OrganizationId = organizationId;
        UpdatedAt = DateTime.UtcNow;
    }

}
