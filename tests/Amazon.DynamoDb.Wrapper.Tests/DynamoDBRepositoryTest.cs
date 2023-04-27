using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AutoFixture;
using Amazon.DynamoDb.Wrapper.Implementations;
using Amazon.DynamoDb.Wrapper.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Amazon.DynamoDb.Wrapper.Tests
{
    public class DynamoDBRepositoryTest
    {
        private readonly Mock<IAmazonDynamoDB> _amazonDynamoDBClientMock;
        private readonly IDynamoDBRepository _repository;
        private readonly Fixture _fixture;


        public DynamoDBRepositoryTest()
        {
            _amazonDynamoDBClientMock = new Mock<IAmazonDynamoDB>();

            _repository = new DynamoDBRepository(_amazonDynamoDBClientMock.Object);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task RunTransaction_IsSuccessful_WhenInputIsValid()
        {
            // arrange
            var input = new List<TransactWriteItem>();
            input.Add(new TransactWriteItem());
            input.Add(new TransactWriteItem());

            var transactionResponse = new TransactWriteItemsResponse
            {
                HttpStatusCode = HttpStatusCode.OK
            };

            _amazonDynamoDBClientMock.Setup(client => client.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(transactionResponse));

            // act
            await _repository.RunTransactionAsync(input);

            // assert
            _amazonDynamoDBClientMock.Verify(client => client.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BatchWrite_IsSuccessful_WhenInputIsValid()
        {
            // arrange
            var batchRequests = new Dictionary<string, List<WriteRequest>>();
            batchRequests.Add(_fixture.Create<string>(), new List<WriteRequest>());
            batchRequests.Add(_fixture.Create<string>(), new List<WriteRequest>());

            var batchWriteItemResponse = new BatchWriteItemResponse
            {
                UnprocessedItems = new Dictionary<string, List<WriteRequest>>(),
                HttpStatusCode = HttpStatusCode.OK
            };

            _amazonDynamoDBClientMock.Setup(client => client.BatchWriteItemAsync(It.IsAny<BatchWriteItemRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(batchWriteItemResponse));

            // act
            await _repository.BatchWriteAsync(batchRequests);

            // assert
            _amazonDynamoDBClientMock.Verify(client => client.BatchWriteItemAsync(It.IsAny<BatchWriteItemRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Update_IsSuccessful_WhenInputIsValid()
        {
            // arrange
            var updateItemRequest = new UpdateItemRequest();
            var updateItemResponse = new UpdateItemResponse
            {
                HttpStatusCode = HttpStatusCode.OK
            };

            _amazonDynamoDBClientMock.Setup(client => client.UpdateItemAsync(It.IsAny<UpdateItemRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(updateItemResponse));

            // act
            await _repository.UpdateAsync(updateItemRequest);

            // assert
            _amazonDynamoDBClientMock.Verify(client => client.UpdateItemAsync(It.IsAny<UpdateItemRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}