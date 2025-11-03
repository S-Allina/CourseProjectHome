using Identity.Application.Dto;
using Identity.Application.Interfaces;
using Identity.Domain.Entity;
using Identity.Infrastructure.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Dto;

namespace Identity.Infrastructure.DataAccess.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateUsersRoleAsync(IEnumerable<string> userIds, string removeRole, string addRole, CancellationToken cancellationToken)
        {
            if (!userIds?.Any() == true) return;

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var roleIds = await _context.Roles
                    .Where(r => r.Name == removeRole || r.Name == addRole)
                    .Select(r => new { r.Name, r.Id })
                    .ToDictionaryAsync(r => r.Name, r => r.Id, cancellationToken);

                var roleIdToRemove = roleIds[removeRole];
                var roleIdToAdd = roleIds[addRole];

                var usersWithTargetRole = await _context.UserRoles
                    .Where(ur => userIds.Contains(ur.UserId) && ur.RoleId == roleIdToAdd)
                    .Select(ur => ur.UserId)
                    .ToListAsync(cancellationToken);

                if (usersWithTargetRole.Any())
                {
                    userIds = userIds.Except(usersWithTargetRole).ToList();

                    if (!userIds.Any())
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw new InvalidOperationException($"Некоторые пользователи уже имеют роль {addRole}");
                    }
                }

                var deletedCount = await _context.UserRoles
                    .Where(ur => userIds.Contains(ur.UserId) && ur.RoleId == roleIdToRemove)
                    .ExecuteDeleteAsync(cancellationToken);

                var newUserRoles = userIds.Select(userId => new IdentityUserRole<string>
                {
                    UserId = userId,
                    RoleId = roleIdToAdd
                });

                await _context.UserRoles.AddRangeAsync(newUserRoles, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                if (ex is InvalidOperationException)
                    throw;

                throw new DbUpdateConcurrencyException("Ошибка обновления. Попробуйте позже.");
            }
        }

        public async Task<ResponseDto> GetAllAsync(IEnumerable<ApplicationUser> users, CancellationToken cancellationToken)
        {
            var userIds = users.Select(u => u.Id).ToList();

            var userRoles = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, Role = r.Name })
                .ToListAsync(cancellationToken);

            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Status = user.Status,
                CreateAt = user.CreateAt,
                UpdateAt = user.UpdateAt,
                Role = userRoles.FirstOrDefault(ur => ur.UserId == user.Id).Role
            }).ToList();

            return new ResponseDto { Result = userDtos };
        }
    }
}
