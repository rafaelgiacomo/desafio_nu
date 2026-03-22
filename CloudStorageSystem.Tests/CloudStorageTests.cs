namespace CloudStorageSystem.Tests;

using CloudStorage;

public class CloudStorageTests
{
    [Fact]
    public void AddFile_ShouldReturnTrue_WhenFileIsNew()
    {
        // Arrange (Organizar)
        var storage = new CloudStorage();

        // Act (Agir)
        bool result = storage.AddFile("foto.png", 500);

        // Assert (Verificar)
        Assert.True(result);
    }

    [Fact]
    public void AddFile_ShouldReturnFalse_WhenFileNameAlreadyExists()
    {
        // Arrange (Organizar)
        var storage = new CloudStorage();
       storage.AddFile("foto.png", 500);

        // Act (Agir)
        bool result = storage.AddFile("foto.png", 300);

        // Assert (Verificar)
        Assert.False(result);
    }

    [Fact]
    public void DeleteFile_ShouldReturnFile_WhenFileFound()
    {
        var storage = new CloudStorage();
       storage.AddFile("foto.png", 500);
        
        var result = storage.DeleteFile("foto.png");

        Assert.NotNull(result);
        Assert.Equal(500, result);
    }

    [Fact]
    public void DeleteFile_ShouldReturnNull_WhenFileNotFound()
    {
        var storage = new CloudStorage();
        
        var result = storage.DeleteFile("inexistente.txt");

        Assert.Null(result);
    }

    [Fact]
    public void GetFileSize_ShouldReturnFileSize_WhenFileFound()
    {
        var storage = new CloudStorage();
        storage.AddFile("inexistente.txt", 500);

        var result = storage.GetFileSize("inexistente.txt");

        Assert.NotNull(result);
        Assert.Equal(500, result);
    }

    [Fact]
    public void GetFileSize_ShouldReturnNull_WhenFileNotFound()
    {
        var storage = new CloudStorage();
        var result = storage.GetFileSize("inexistente.txt");
        Assert.Null(result);
    }

    [Fact]
    public void GetNLargest_ShouldReturnLargest()
    {
        //Arrange
        var storage = new CloudStorage();

        storage.AddFile("/dir/file1.txt", 5);
        storage.AddFile("/dir/file2", 20);
        storage.AddFile("/dir/deeper/file2.mov", 9);

        //Act
        List<string> result = storage.GetNLargest("/dir", 2);

        //Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("/dir/file2(20)", result[0]);
        Assert.Equal("/dir/file1.txt(5)", result[1]);
    }

    [Fact]
    public void GetNLargest_ShouldReturnEmpty_WhenPrefixNotFound()
    {
        //Arrange
        var storage = new CloudStorage();

        storage.AddFile("/dir/file1.txt", 5);
        storage.AddFile("/dir/file2", 20);
        storage.AddFile("/dir/deeper/file2.mov", 9);

        //Act
        List<string> result = storage.GetNLargest("/anotherdir", 2);

        //Assert
        Assert.Empty(result);
    }
}
