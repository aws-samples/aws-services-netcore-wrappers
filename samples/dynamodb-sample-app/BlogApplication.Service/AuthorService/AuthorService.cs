using Amazon.DynamoDb.Wrapper.Interfaces;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using BlogApplication.DTO;
using BlogApplication.Repository.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogApplication.Service
{
    public class AuthorService : IAuthorService
    {
        private readonly IDynamoDBGenericRepository<AuthorEntity> _authorRepository;
        private readonly IDynamoDBRepository _dynamoDBRepository;
        private readonly IDynamoDBContext _context;

        private readonly IMapper _mapper;

        public AuthorService(IDynamoDBGenericRepository<AuthorEntity> authorRepository,
            IDynamoDBRepository dynamoDBRepository, IMapper mapper, IDynamoDBContext context)
        {
            _authorRepository = authorRepository;
            _dynamoDBRepository = dynamoDBRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task SaveAuthor(AuthorDTO author)
        {
            var authorEntity = _mapper.Map<AuthorEntity>(author);

            await _authorRepository.SaveAsync(authorEntity);
        }

        public async Task DeleteAuthor(string authorId)
        {
            await _authorRepository.DeleteAsync(AppConstants.AUTHOR_PARTITION_KEY, GetSortKey(authorId));
        }

        public async Task<AuthorDTO> GetAuthorById(string authorId)
        {
            var authorEntity = await _authorRepository.GetByPrimaryKeyAsync(AppConstants.AUTHOR_PARTITION_KEY, GetSortKey(authorId));
            return _mapper.Map<AuthorDTO>(authorEntity);
        }

        public async Task<List<AuthorDTO>> GetAuthorList()
        {
            var filter = new QueryFilter();
            filter.AddCondition(nameof(BaseEntity.PK), QueryOperator.Equal, AppConstants.AUTHOR_PARTITION_KEY);

            var authorList = await _authorRepository.QueryAsync(filter);
            return _mapper.Map<List<AuthorDTO>>(authorList);
        }

        private string GetSortKey(string authorId)
        {
            return $"{AppConstants.AUTHOR_PARTITION_KEY}{AppConstants.DELIMITER}{authorId}".ToUpper();
        }
    }
}
