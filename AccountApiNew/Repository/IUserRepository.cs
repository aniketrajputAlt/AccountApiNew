﻿using AccountApiNew.Model;

namespace AccountApiNew.Repository
{
    public interface IUserRepository
    {
       // public Task<User> GetUserByUsernameAndPasswordAsync(string username, string password);
       public  Task<bool> UpdatePasswordAsync(string username, string newPassword);
       //public Task<User> GetUserDetailsByIdAsync(int userId);
    }
}
