using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using AutoFixture;
using Amazon.DynamoDb.Wrapper.Implementations;
using Amazon.DynamoDb.Wrapper.Interfaces;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Amazon.DynamoDb.Wrapper.Tests
{
    public class DynamoDBGenericRepositoryTest
    {
        private readonly Mock<IDynamoDBContext> _dynamoDBContextMock;
        private readonly Mock<IAmazonDynamoDB> _amazonDynamoDBClientMock;
        private readonly Mock<IDynamoDBRepository> _dynamoDBRepositoryMock;

        private readonly IDynamoDBGenericRepository<TestEntity> _testRepository;

        private readonly Fixture _fixture;

        private readonly string PARTITION_KEY;
        private readonly string SORT_KEY;

        public DynamoDBGenericRepositoryTest()
        {
            _dynamoDBContextMock = new Mock<IDynamoDBContext>();
            _amazonDynamoDBClientMock = new Mock<IAmazonDynamoDB>();
            _dynamoDBRepositoryMock = new Mock<IDynamoDBRepository>();

            _testRepository = new DynamoDBGenericRepository<TestEntity>(_dynamoDBContextMock.Object, _amazonDynamoDBClientMock.Object, _dynamoDBRepositoryMock.Object);

            _fixture = new Fixture();

            PARTITION_KEY = _fixture.Create<string>();
            SORT_KEY = _fixture.Create<string>();
        }

        [Fact]
        public async Task GetByPrimaryKey_IsSuccessful_WhenInputIsValid()
        {
            // arrange
            var loadAsyncResponse = _fixture.Create<TestEntity>();
            _dynamoDBContextMock.Setup(context => context.LoadAsync<TestEntity>(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(loadAsyncResponse));

            // act
            var response = await _testRepository.GetByPrimaryKeyAsync(PARTITION_KEY);

            // assert
            Assert.NotNull(response);
            _dynamoDBContextMock.Verify(context => context.LoadAsync<TestEntity>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByPrimaryKeyWithPartitionKeyAndSortKey_IsSuccessful_WhenInputIsValid()
        {
            // arrange
            var loadAsyncResponse = _fixture.Create<TestEntity>();
            _dynamoDBContextMock.Setup(context => context.LoadAsync<TestEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(loadAsyncResponse));

            // act
            var response = await _testRepository.GetByPrimaryKeyAsync(PARTITION_KEY, SORT_KEY);

            // assert
            Assert.NotNull(response);
            _dynamoDBContextMock.Verify(context => context.LoadAsync<TestEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Save_IsSuccessful_WhenInputIsValid()
        {
            // arrange
            _dynamoDBContextMock.Setup(context => context.SaveAsync(It.IsAny<TestEntity>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // act
            await _testRepository.SaveAsync(It.IsAny<TestEntity>());

            // assert
            _dynamoDBContextMock.Verify(context => context.SaveAsync(It.IsAny<TestEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_IsSuccessful_WhenInputIsValid()
        {
            // arrange
            _dynamoDBContextMock.Setup(context => context.DeleteAsync(It.IsAny<TestEntity>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // act
            await _testRepository.DeleteAsync(It.IsAny<TestEntity>());

            // assert
            _dynamoDBContextMock.Verify(context => context.DeleteAsync(It.IsAny<TestEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteWithPartitionKey_IsSuccessful_WhenInputIsValid()
        {
            // arrange
            _dynamoDBContextMock.Setup(context => context.DeleteAsync<TestEntity>(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // act
            await _testRepository.DeleteAsync(PARTITION_KEY);

            // assert
            _dynamoDBContextMock.Verify(context => context.DeleteAsync<TestEntity>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteWithPartitionAndSortKey_IsSuccessful_WhenInputIsValid()
        {
            // arrange
            _dynamoDBContextMock.Setup(context => context.DeleteAsync<TestEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // act
            await _testRepository.DeleteAsync(PARTITION_KEY, SORT_KEY);

            // assert
            _dynamoDBContextMock.Verify(context => context.DeleteAsync<TestEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SaveWithConditionaExpression_IsSuccessful_WhenInputIsValid()
        {
            // arrange
            var conditionExpression = _fixture.Create<string>();
            var tableName = _fixture.Create<string>();
            var testEntityRequest = _fixture.Create<TestEntity>();
            var putItemResponse = new PutItemResponse(); putItemResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            var document = new Document();

            _dynamoDBContextMock.Setup(item => item.ToDocument(It.IsAny<TestEntity>())).Returns(document);
            _amazonDynamoDBClientMock.Setup(context => context.PutItemAsync(It.IsAny<PutItemRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(putItemResponse));

            // act
            await _testRepository.SaveAsync(testEntityRequest, conditionExpression);

            // assert
            _amazonDynamoDBClientMock.Verify(context => context.PutItemAsync(It.IsAny<PutItemRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(System.Net.HttpStatusCode.OK, putItemResponse.HttpStatusCode);
        }
    }
}