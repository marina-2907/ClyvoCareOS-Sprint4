using ClyvoCareOSChallenge.Models;

namespace ClyvoCareOSChallenge.UnitTests;

public class PetTests
{
    [Fact]
    public void FaseDeVida_CachorroComMenosDeUmAno_RetornaFilhote()
    {
        // Arrange
        var pet = new Pet
        {
            Especie = "cachorro",
            DataNascimento =
                DateTime.UtcNow.AddMonths(-6)
        };

        // Act
        var faseDeVida = pet.FaseDeVida;

        // Assert
        Assert.Equal("Filhote", faseDeVida);
    }

    [Fact]
    public void FaseDeVida_CachorroComTresAnos_RetornaAdulto()
    {
        // Arrange
        var pet = new Pet
        {
            Especie = "cachorro",
            DataNascimento =
                DateTime.UtcNow.AddYears(-3)
        };

        // Act
        var faseDeVida = pet.FaseDeVida;

        // Assert
        Assert.Equal("Adulto", faseDeVida);
    }

    [Fact]
    public void FaseDeVida_CachorroComOitoAnos_RetornaIdoso()
    {
        // Arrange
        var pet = new Pet
        {
            Especie = "cachorro",
            DataNascimento =
                DateTime.UtcNow.AddYears(-8)
        };

        // Act
        var faseDeVida = pet.FaseDeVida;

        // Assert
        Assert.Equal("Idoso", faseDeVida);
    }

    [Fact]
    public void FaseDeVida_GatoComOitoAnos_RetornaAdulto()
    {
        // Arrange
        var pet = new Pet
        {
            Especie = "gato",
            DataNascimento =
                DateTime.UtcNow.AddYears(-8)
        };

        // Act
        var faseDeVida = pet.FaseDeVida;

        // Assert
        Assert.Equal("Adulto", faseDeVida);
    }

    [Fact]
    public void FaseDeVida_GatoComOnzeAnos_RetornaIdoso()
    {
        // Arrange
        var pet = new Pet
        {
            Especie = "gato",
            DataNascimento =
                DateTime.UtcNow.AddYears(-11)
        };

        // Act
        var faseDeVida = pet.FaseDeVida;

        // Assert
        Assert.Equal("Idoso", faseDeVida);
    }
}