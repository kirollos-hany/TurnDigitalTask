using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using TurnDigital.Application.Common;
using TurnDigital.Application.Features.Account.Responses;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Query.Pagination;
using TurnDigital.Web.Features.Account.Requests;

namespace TurnDigital.IntegrationTests;

public class ProductsIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ProductsIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private async Task<string> GetJwt()
    {
        var client = _factory.CreateClient();

        var loginRequest = new LoginRequest
        {
            Email = Constants.TurnDigitalAdmin.Email,
            Password = Constants.TurnDigitalAdmin.Password
        };

        var response = await client.PostAsJsonAsync("/api/account/login", loginRequest);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<LoginResponse>();

        return responseContent!.AccessToken;
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    public async Task Delete_Product_Returns_NoContent(int id)
    {
        // Arrange
        var client = _factory.CreateClient();
        var accessToken = await GetJwt();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Act
        var responseMessage = await client.DeleteAsync($"/api/products/{id}");

        //Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);

        //Verify product deleted
        var response = await client.GetAsync($"/api/products/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Product_Returns_NotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var accessToken = await GetJwt();
        var productId = int.MaxValue;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Act
        var responseMessage = await client.DeleteAsync($"/api/products/{productId}");

        //Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public async Task Get_Product_By_Id_Returns_Success(int id)
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var responseMessage = await client.GetAsync($"/api/products/{id}");

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        (await responseMessage.Content.ReadFromJsonAsync<ProductDto>()).Should().NotBeNull();
    }

    [Fact]
    public async Task Get_Product_By_Id_Returns_NotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var productId = int.MaxValue;

        // Act
        var responseMessage = await client.GetAsync($"/api/products/{productId}");

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task Get_Products_ByCategory_Returns_Success(int categoryId)
    {
        // Arrange
        var client = _factory.CreateClient();
        var queryString = $"?categoryId={categoryId}";

        // Act
        var responseMessage = await client.GetAsync($"/api/products{queryString}");

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        (await responseMessage.Content.ReadFromJsonAsync<PaginatedResult<ProductDto>>()).Should().NotBeNull();
    }

    [Theory]
    [InlineData(1, "name 1")]
    [InlineData(2, "name 2")]
    [InlineData(3, "name 3")]
    public async Task Get_Products_ByCategoryAndProductName_Returns_Success(int categoryId, string productName)
    {
        // Arrange
        var client = _factory.CreateClient();
        var queryString = $"?categoryId={categoryId}&name={productName}";

        // Act
        var responseMessage = await client.GetAsync($"/api/products{queryString}");

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        (await responseMessage.Content.ReadFromJsonAsync<PaginatedResult<ProductDto>>()).Should().NotBeNull();
    }

    [Fact]
    public async Task Get_Products_Returns_Success()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var responseMessage = await client.GetAsync("/api/products");

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        (await responseMessage.Content.ReadFromJsonAsync<PaginatedResult<ProductDto>>()).Should().NotBeNull();
    }

    [Fact]
    public async Task Create_Product_Returns_BadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var jwt = await GetJwt();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var request = new MultipartFormDataContent
        {

        };

        // Act
        var responseMessage = await client.PostAsync("/api/v2/products", request);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await responseMessage.Content.ReadFromJsonAsync<ValidationFailureResponse>()).Should().NotBeNull();
    }

    [Fact]
    public async Task Create_Product_Returns_Created()
    {
        // Arrange
        var client = _factory.CreateClient();
        var jwt = await GetJwt();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var request = new MultipartFormDataContent
        {
            { new StringContent("Product Test"), "Name" },
            { new StringContent("Some Description"), "Description" },
            { new StringContent("200"), "Price" },
            { new StringContent("1"), "CategoryId" }
        };

        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Dummy file content"));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        request.Add(fileContent, "Image", "test-image.jpg");

        // Act
        var responseMessage = await client.PostAsync("/api/v2/products", request);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Created);
        var product = await responseMessage.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product?.Id.Should().BePositive();
        product?.Category.Id.Should().BePositive();
        product?.Category.Name.Should().NotBeNullOrEmpty();
        product?.Name.Should().Be("Product Test");
        product?.Description.Should().Be("Some Description");
        product?.Price.Should().Be(200);

        // Make sure it is created 
        var getByIdResponseMessage = await client.GetAsync(responseMessage.Headers.Location);

        getByIdResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var productResult = (await getByIdResponseMessage.Content.ReadFromJsonAsync<ProductDto>());
        productResult.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Update_Product_Returns_NotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var jwt = await GetJwt();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var request = new MultipartFormDataContent
        {
            { new StringContent("Product Test"), "Name" },
            { new StringContent("Some Description"), "Description" },
            { new StringContent("200"), "Price" },
            { new StringContent("1"), "CategoryId" }
        };

        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Dummy file content"));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        request.Add(fileContent, "Image", "test-image.jpg");

        // Act
        var responseMessage = await client.PatchAsync($"/api/v2/products/{int.MaxValue}", request);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Update_Product_Returns_BadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var jwt = await GetJwt();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var request = new MultipartFormDataContent
        {
            { new StringContent("Product Test"), "Name" },
            { new StringContent("Some Description"), "Description" },
            { new StringContent("0"), "Price" },
            { new StringContent("1"), "CategoryId" }
        };

        // Act
        var responseMessage = await client.PatchAsync("/api/v2/products/1", request);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationFailure = await responseMessage.Content.ReadFromJsonAsync<ValidationFailureResponse>();
        validationFailure.Should().NotBeNull();
        validationFailure?.ValidationMap.Should().ContainKeys("Price");
    }
    
    [Fact]
    public async Task Update_Product_Returns_Ok()
    {
        // Arrange
        var client = _factory.CreateClient();
        var jwt = await GetJwt();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var request = new MultipartFormDataContent
        {
            { new StringContent("500"), "Price" },
        };

        // Act
        var responseMessage = await client.PatchAsync("/api/v2/products/1", request);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await responseMessage.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product?.Price.Should().Be(500);
    }
}