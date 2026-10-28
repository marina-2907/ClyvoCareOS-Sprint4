using ClyvoCareOSChallenge.Application;
using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.DTOs;
using Moq;

namespace ClyvoCareOSChallenge.UnitTests;

public sealed class PetQueryServiceTests
{
    [Fact]
    public async Task GetPageAsync_DelegaFiltroEPaginacaoAoRepositorio()
    {
        // Arrange
        var query = new PaginationQuery { Page = 2, PageSize = 5, Nome = "Mel" };
        var repo = new Mock<IPetReadRepository>();
        repo.Setup(x => x.FindPageAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Array.Empty<PetResponseDto>(), 0));
        var service = new PetQueryService(repo.Object);
        // Act
        var result = await service.GetPageAsync(query);
        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
        repo.Verify(x => x.FindPageAsync(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_QuandoNaoExiste_RetornaNulo()
    {
        // Arrange
        var repo = new Mock<IPetReadRepository>();
        repo.Setup(x => x.FindByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PetResponseDto?)null);
        var service = new PetQueryService(repo.Object);
        // Act
        var result = await service.GetByIdAsync(999);
        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(0, 1, 10)]
    [InlineData(-2, 1, 10)]
    [InlineData(5, 5, 10)]
    public void PaginationQuery_ValidaPagina(int value, int expected, int pageSize)
    {
        // Arrange
        var query = new PaginationQuery();
        // Act
        query.Page = value;
        // Assert
        Assert.Equal(expected, query.Page);
        Assert.Equal(pageSize, query.PageSize);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(101, 100)]
    [InlineData(25, 25)]
    public void PaginationQuery_LimitaTamanhoDaPagina(int value, int expected)
    {
        // Arrange
        var query = new PaginationQuery();
        // Act
        query.PageSize = value;
        // Assert
        Assert.Equal(expected, query.PageSize);
    }
}
