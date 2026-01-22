
﻿using AutoMapper;
using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;
using backend.Repository.interfaces;
using backend.Service.interfaces;

namespace backend.Service.implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IApiResponseFactory _apiResponseFactory;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IApiResponseFactory apiResponseFactory, IMapper mapper
            )
        {
            _userRepository = userRepository;
            _apiResponseFactory = apiResponseFactory;
            _mapper = mapper;
        }

        public User CreateStaff(User staff)
        {
            throw new NotImplementedException();
        }

        public bool DeleteStaff(int userId)
        {
            throw new NotImplementedException();
        }

        public List<User> getAllStaff()
        {
            throw new NotImplementedException();
        }

        public User Update(User user)
        {
            throw new NotImplementedException();
        }
        

                public ApiResponse<User> validateUser(LoginRequest request)
        {
            var user = _userRepository.ValidateUser(request.Email, request.Password);

            if (user == null)
            {
                return _apiResponseFactory.Fail<User>(
                    StatusCodes.Status401Unauthorized,
                    "Invalid credentials"
                );
            }
            return _apiResponseFactory.Success(
                user,
                "Login successful"
 
            );
        }

        public async Task<ApiResponse<bool>> ActiveUser(int userId)
        {
            var result = _userRepository.ActiveUser(userId);
            if (result)
            {
                return _apiResponseFactory.Success(true);
            }
            return _apiResponseFactory.Fail<bool>(
                StatusCodes.Status404NotFound,
                "User not found"
            );
        }

        public async Task<ApiResponse<UserResponse>> CreateEmployee(CreateEmployeeRequest user)
        {
            if(user == null)
            {
                return _apiResponseFactory.Fail<UserResponse>(
                        StatusCodes.Status400BadRequest,
                        "Employee is invalid!"
                    );
            }

            var today = DateOnly.FromDateTime(DateTime.Now);
            if (user.DateOfBirth > today.AddYears(-18))
            {
                return _apiResponseFactory.Fail<UserResponse>(
                        StatusCodes.Status400BadRequest,
                        "Employee must be at least 18 years old!"
                    );
            }

            user.PasswordHashing = BCrypt.Net.BCrypt.HashPassword(user.PasswordHashing); // Hash password 
            user.RoleId = true; // Set role to employee
            user.IsActive = true; // Set default active status
            user.CreatedAt = DateTime.Now;


            var userEntity = _mapper.Map<User>(user);
            var createdUser = await _userRepository.CreateEmployee(userEntity);
            var userResponse = _mapper.Map<UserResponse>(createdUser);
            return _apiResponseFactory.Success(userResponse);

            throw new NotImplementedException();
        }

        public async Task<ApiResponse<bool>> DeactiveUser(int userId)
        {
            var result = _userRepository.DeactiveUser(userId);
            if (result)
            {
                return _apiResponseFactory.Success(true);
            }
            return _apiResponseFactory.Fail<bool>(
                StatusCodes.Status404NotFound,
                "User not found"
            );
        }

        public Task<ApiResponse<bool>> DeleteEmployee(int userId)
        {
            if (userId <= 0)
            {
                return Task.FromResult(_apiResponseFactory.Fail<bool>(
                    StatusCodes.Status400BadRequest,
                    "Invalid userId"
                ));
            }
            var result = _userRepository.DeleteEmployee(userId);
            if (!result)
            {
                return Task.FromResult(_apiResponseFactory.Fail<bool>(
                    StatusCodes.Status404NotFound,
                    "User not found"
                ));
            }
            return Task.FromResult(_apiResponseFactory.Success(true));
        }

        public async Task<ApiResponse<List<UserResponse>>> GetAllUser()
        {
            var users = await _userRepository.GetAllUser();
            var userResponses = _mapper.Map<List<UserResponse>>(users);
            return _apiResponseFactory.Success(userResponses);
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<List<UserResponse>>> SearchEmployeeByFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return _apiResponseFactory.Fail<List<UserResponse>>(
                    StatusCodes.Status400BadRequest,
                    "FullName is required"
                );
            }

            var users = await _userRepository.SearchEmployeeByFullName(fullName);

            if (users == null || !users.Any())
            {
                return _apiResponseFactory.Fail<List<UserResponse>>(
                    StatusCodes.Status404NotFound,
                    "User not found"
                );
            }
            users = _mapper.Map<List<UserResponse>>(users);
            return _apiResponseFactory.Success(users);
        }

        public async Task<ApiResponse<List<UserResponse>>> SearchUserByFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return _apiResponseFactory.Fail<List<UserResponse>>(
                    StatusCodes.Status400BadRequest,
                    "FullName is required"
                );
            }

            var users = await _userRepository.SearchUserByFullName(fullName);
            if (users == null || !users.Any())
            {
                return _apiResponseFactory.Fail<List<UserResponse>>(
                    StatusCodes.Status404NotFound,
                    "User not found"
                );
            }
            users = _mapper.Map<List<UserResponse>>(users);
            return _apiResponseFactory.Success(users);
        }

        public async Task<ApiResponse<UserResponse>> UpdateEmployee(int id, UpdateEmployeeRequest user)
        {
            if (user == null) { 
                return _apiResponseFactory.Fail<UserResponse>(
                        StatusCodes.Status400BadRequest,
                        "Employee is invalid!"
                    );
            }

            var today = DateOnly.FromDateTime(DateTime.Now);
            if (user.DateOfBirth > today.AddYears(-18))
            {
                return _apiResponseFactory.Fail<UserResponse>(
                        StatusCodes.Status400BadRequest,
                        "Employee must be at least 18 years old!"
                    );
            }

            user.UpdatedAt = DateTime.Now;
            user.UserId = id;

            var userEntity = _mapper.Map<User>(user);
            var updatedUser = await _userRepository.UpdateEmployee(userEntity);
            

            var userResponse = _mapper.Map<UserResponse>(updatedUser);

            return _apiResponseFactory.Success(userResponse);

            throw new NotImplementedException();
        }
    }
}
