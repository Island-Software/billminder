using System.Net;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Paybills.API.Application.Controllers;
using Paybills.API.Application.Controllers.Receivings;
using Paybills.API.Application.DTOs.ReceivingType;
using Paybills.API.Domain.Entities;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.API.Helpers;
using Paybills.UnitTests.Utils;

namespace Paybills.UnitTests.Controllers
{
    public class ReceivingTypeControllerUnitTest
    {
        [Fact]
        public async void CreateReceivingType_MustReturn201Created()
        {
            // Given
            Mock<IReceivingTypeService> mockReceivingTypeRepo = new Mock<IReceivingTypeService>();
            Mock<IMapper> mockMapper = new Mock<IMapper>();

            var controller = new ReceivingTypeController(mockReceivingTypeRepo.Object, mockMapper.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var receivingType = ModelUtils.GenerateRandomReceivingTypeRegisterDto();

            mockReceivingTypeRepo.Setup(repo => repo.Exists(receivingType.Description)).Returns(Task.FromResult(false));
            mockReceivingTypeRepo.Setup(repo => repo.Create(It.IsAny<ReceivingType>())).Returns(Task.FromResult(true));

            mockMapper.Setup(mapper => mapper.Map<ReceivingTypeDto>(It.IsAny<ReceivingType>())).Returns(
                ModelUtils.GenerateRandomReceivingTypeDto()
            );

            // When
            var result = await controller.Create(receivingType);

            // Then
            Assert.NotNull(result.Result);
            var resultCast = (CreatedAtActionResult)result.Result!;
            Assert.Equal("Get", resultCast.ActionName);    
            Assert.Equal((int)HttpStatusCode.Created, resultCast.StatusCode);
        }

        [Fact]
        public async void GetReceivingTypes_MustReturn200Ok()
        {
            var expectedSize = 2;

            // Given
            Mock<IReceivingTypeService> mockReceivingTypeRepo = new Mock<IReceivingTypeService>();
            Mock<IMapper> mockMapper = new Mock<IMapper>();

            var controller = new ReceivingTypeController(mockReceivingTypeRepo.Object, mockMapper.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var list = ModelUtils.GenerateRandomReceivingTypesList(expectedSize);

            mockReceivingTypeRepo.Setup(repo => repo.GetAsync()).Returns(Task.FromResult(list));

            mockMapper.Setup(mapper => mapper.Map<IEnumerable<ReceivingTypeDto>>(It.IsAny<IEnumerable<ReceivingType>>())).Returns(
                ModelUtils.GenerateRandomReceivingTypeDtoList(expectedSize)
            );

            // When
            var result = await controller.GetAll();

            // Then
            Assert.NotNull(result.Result);
            var resultCast = (OkObjectResult)result.Result!;
            var resultList = (IEnumerable<ReceivingTypeDto>)resultCast.Value!;
            Assert.True(resultList.Count() == expectedSize);
            result.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}