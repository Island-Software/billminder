using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Paybills.API.Application.Controllers;
using Paybills.API.Application.DTOs.Receiving;
using Paybills.API.Domain.Entities;
using Paybills.API.Domain.Services.Interfaces;
using Paybills.UnitTests.Utils;

namespace Paybills.UnitTests.Controllers
{
    public class ReceivingsControllerUnitTest
    {
        // [Fact]
        // public async void GetReceivings_MustReturn200Ok()
        // {
            // var expectedSize = 2;
            // var userName = DataUtils.RandomString(20);

            // // Given
            // Mock<IReceivingService> mockReceivingRepo = new Mock<IReceivingService>();
            // Mock<IMapper> mockMapper = new Mock<IMapper>();
            // Mock<SESService> sesService = new Mock<SESService>();

            // var controller = new ReceivingsController(mockReceivingRepo.Object, mockMapper.Object);
            // controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // var list = ModelUtils.GenerateRandomPagedReceivingsList(expectedSize);

            // mockReceivingRepo.Setup(repo => repo.GetAsync(
            //     userName, It.IsAny<UserParams>())).Returns(Task.FromResult(list));

            // mockMapper.Setup(mapper => mapper.Map<IEnumerable<ReceivingDto>>(It.IsAny<PagedList<Receiving>>())).Returns(
            //     ModelUtils.GenerateRandomReceivingDtoList(expectedSize)
            // );

            // // When
            // var result = await controller.GetReceivings(userName, new Mock<UserParams>().Object);

            // // Then
            // Assert.NotNull(result.Result);
            // var resultCast = (OkObjectResult)result.Result!;
            // var resultList = (IEnumerable<ReceivingDto>)resultCast.Value!;
            // Assert.True(resultList.Count() == expectedSize);
            // result.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
        // }
    }
}