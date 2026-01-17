using AutoMapper;
using backend.Dtos.Response;
using backend.Repository.interfaces;
using backend.Service.interfaces;

namespace backend.Service.implementations
{
    public class ClientService : IClientService
    {
        private readonly ILogger<ClientService> _logger;
        private readonly IClientRepository _clientRepository;
        private readonly IApiResponseFactory _apiResponseFactory;
        private readonly IMapper _mapper;

        public ClientService(ILogger<ClientService> logger, IClientRepository clientRepository, 
            IApiResponseFactory apiResponseFactory, IMapper mapper)
        {
            _logger = logger;
            _clientRepository = clientRepository;
            _apiResponseFactory = apiResponseFactory;
            _mapper = mapper;
        }

        public ApiResponse<bool> DeleteClient(int clientId)
        {
            var result = _clientRepository.DeleteClient(clientId);
            if (!result)
            {
                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status500InternalServerError,
                    "Failed to delete client"
                 );
            }else
            {
                return _apiResponseFactory.Success(true);
            }
        }

        public async Task<ApiResponse<List<ClientResponse>>> GetClientByFullNameOrEmailAsync(string information)
        {
            if (string.IsNullOrWhiteSpace(information))
            {
                return _apiResponseFactory.Fail<List<ClientResponse>>(
                        StatusCodes.Status404NotFound,
                        "Information cannot be null or empty"
                    );
            }

            var client = await _clientRepository.GetClientByFullNameOrEmailAsync(information);
            if (client == null)
            {
                return _apiResponseFactory.Fail<List<ClientResponse>>(
                        StatusCodes.Status404NotFound,
                        "Client not found"
                    );
            }
            var clientResponse = _mapper.Map<List<ClientResponse>>(client);
            return _apiResponseFactory.Success(clientResponse);
        }
    }
}
