using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Seeding
{
    public static class UserSeed
    {
        public static readonly User[] Data = new[]
           {
            CreateUser(
                id: "783a64e0-efd9-4a60-b05a-65ee3ddafe95",
                email: "user1@example.com",
                passwordHash: "HASHED_PASSWORD_1",
                status: UserStatus.Active,
                role: UserRole.User
            ),
            CreateUser(
                id: "b89f7370-5365-4c7d-96e3-9093f4d4fa06",
                email: "user2@example.com",
                passwordHash: "HASHED_PASSWORD_2",
                status: UserStatus.Active,
                role: UserRole.User
            ),
            CreateUser(
                id: "a6c47fa6-75e9-4518-8ab9-505eed65c431",
                email: "user3@example.com",
                passwordHash: "HASHED_PASSWORD_3",
                status: UserStatus.Active,
                role: UserRole.User
            ),
            CreateUser(
                id: "c2e740d3-31ef-410f-875e-ce821ab4cbf2",
                email: "user20@example.com",
                passwordHash: "HASHED_PASSWORD_20",
                status: UserStatus.Active,
                role: UserRole.User
            )
        };

        private static User CreateUser(
            string id,
            string email,
            string passwordHash,
            UserStatus status,
            UserRole role)
        {
            return new User
            {
                Id = Guid.Parse(id),
                Email = email,
                PasswordHash = passwordHash,
                Status = status,
                Role = role,
            };
        }
    }
}