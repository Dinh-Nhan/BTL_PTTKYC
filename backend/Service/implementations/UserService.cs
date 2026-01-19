using AutoMapper;
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
        public UserService(IUserRepository userRepository,
            IApiResponseFactory apiResponseFactory,
            IMapper mapper
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
    }
}
