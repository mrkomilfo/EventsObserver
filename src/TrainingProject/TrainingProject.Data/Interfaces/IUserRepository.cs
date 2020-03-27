﻿using System.Threading.Tasks;
using TrainingProject.Domain;
//using TrainingProject.Domain.Logic.Models;

namespace TrainingProject.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUser(string login);
        //Task<Page<User>> GetUsers(int index, int pageSize);
        Task<bool> IsUserExist(string login);
        Task AddUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(int id);
    }
}