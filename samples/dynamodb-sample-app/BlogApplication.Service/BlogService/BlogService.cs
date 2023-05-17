using Amazon.DynamoDb.Wrapper.Interfaces;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using BlogApplication.DTO;
using BlogApplication.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BlogApplication.Service
{
    public class BlogService : IBlogService
    {
        private readonly IDynamoDBGenericRepository<BlogEntity> _blogRepository;
        private readonly IDynamoDBGenericRepository<GSI1Entity> _gsi1Repository;
        private readonly IDynamoDBGenericRepository<GSI2Entity> _gsi2Repository;
        private readonly IDynamoDBRepository _dynamoDBRepository;
        private readonly IDynamoDBContext _context;

        private readonly IAuthorService _authorService;
        private readonly IMapper _mapper;

        public BlogService(IDynamoDBGenericRepository<BlogEntity> blogRepository, IDynamoDBRepository dynamoDBRepository,
            IDynamoDBGenericRepository<GSI1Entity> gsi1Repository, IDynamoDBGenericRepository<GSI2Entity> gsi2Repository,
        IMapper mapper, IDynamoDBContext context, IAuthorService authorService)
        {
            _blogRepository = blogRepository;
            _gsi1Repository = gsi1Repository;
            _gsi2Repository = gsi2Repository;
            _dynamoDBRepository = dynamoDBRepository;
            _mapper = mapper;
            _context = context;

            _authorService = authorService;
        }

        public async Task Delete(string id)
        {
            await _blogRepository.DeleteAsync(AppConstants.BLOG_PARTITION_KEY, $"{AppConstants.BLOG_PARTITION_KEY}{AppConstants.DELIMITER}{id}");
        }

        public async Task<BlogDTO> GetBlogById(string id)
        {
            var blogEntity = await _blogRepository.GetByPrimaryKeyAsync(AppConstants.BLOG_PARTITION_KEY, $"{AppConstants.BLOG_PARTITION_KEY}{AppConstants.DELIMITER}{id}");

            return _mapper.Map<BlogDTO>(blogEntity);
        }

        public async Task<List<BlogDTO>> GetBlogList()
        {
            // Part 1: Get blogs Ids from GSI-1
            var queryFilter = new QueryFilter();
            queryFilter.AddCondition(nameof(BlogEntity.GSI2PK), QueryOperator.Equal, AppConstants.BLOG_PARTITION_KEY);

            List<string> attributesToGet = new List<string>();
            foreach (PropertyInfo prop in typeof(GSI2Entity).GetProperties())
            {
                attributesToGet.Add(prop.Name);
            }

            // here we get all blog ids sorted by date
            var gsi2Entities = await _gsi2Repository.QueryAsync(queryFilter, backwardSearch: true, indexName: AppConstants.GSI2_INDEX_NAME, attributesToGet: attributesToGet);


            // Part 2: Use batch get to get records from main table
            List<Tuple<object, object>> partitionAndSortKeys = new List<Tuple<object, object>>();

            foreach (var item in gsi2Entities)
            {
                partitionAndSortKeys.Add(new Tuple<object, object>(AppConstants.BLOG_PARTITION_KEY, item.SK));
            }

            var blogListEntities = await _blogRepository.BatchGetAsync(partitionAndSortKeys);

            var blogList = _mapper.Map<List<BlogDTO>>(blogListEntities);

            return blogList.OrderByDescending(item => item.CreatedDate).ToList();
        }

        public async Task<List<BlogDTO>> GetBlogListByAuthor(string authorId)
        {
            // Part 1: Get blogs Ids from GSI-2
            var queryFilter = new QueryFilter();
            queryFilter.AddCondition(nameof(BlogEntity.GSI1PK), QueryOperator.Equal, $"{AppConstants.AUTHOR_PARTITION_KEY}{AppConstants.DELIMITER}{authorId}");
            
            List<string> attributesToGet = new List<string>();
            foreach (PropertyInfo prop in typeof(GSI1Entity).GetProperties())
            {
                attributesToGet.Add(prop.Name);
            }

            // here we get all blog ids sorted by date filtered by author-id
            var gsi1Entities = await _gsi1Repository.QueryAsync(queryFilter, indexName: AppConstants.GSI1_INDEX_NAME, attributesToGet: attributesToGet);


            // Part 2: Use batch get to get records from main table
            List<Tuple<object, object>> partitionAndSortKeys = new List<Tuple<object, object>>();

            foreach (var item in gsi1Entities)
            {
                partitionAndSortKeys.Add(new Tuple<object, object>(AppConstants.BLOG_PARTITION_KEY, item.SK));
            }

            var blogListEntities = await _blogRepository.BatchGetAsync(partitionAndSortKeys);

            var blogList = _mapper.Map<List<BlogDTO>>(blogListEntities);

            return blogList.OrderByDescending(item => item.CreatedDate).ToList();
        }

        public async Task SaveBlog(BlogDTO blog)
        {
            var existingAuthor = await _authorService.GetAuthorById(blog.AuthorId);
            if (existingAuthor == null)
                throw new Exception("Invalid author id");

            var blogEntity = _mapper.Map<BlogEntity>(blog);

            await _blogRepository.SaveAsync(blogEntity);
        }

        private string GetSortKey(string blogId)
        {
            return $"{AppConstants.BLOG_PARTITION_KEY}{AppConstants.DELIMITER}{blogId}".ToUpper();
        }
    }
}