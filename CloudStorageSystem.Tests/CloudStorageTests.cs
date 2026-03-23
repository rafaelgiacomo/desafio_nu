namespace CloudStorageSystem.Tests;

using CloudStorage;

public class CloudStorageTests
{
    [Fact]
    public void AddFile_ShouldReturnTrue_WhenFileIsNewAndWithinUserCapacity()
    {
        // Arrange
        var storage = new CloudStorage();
        storage.AddUser(1, 600);

        // Act
        bool result = storage.AddFile(1, "foto.png", 500);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AddFile_ShouldReturnFalse_WhenFileNameAlreadyExists()
    {
        // Arrange
        var storage = new CloudStorage();
        storage.AddUser(1, 600);
        storage.AddFile(1, "foto.png", 300);

        // Act
        bool result = storage.AddFile(1, "foto.png", 300);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AddFile_ShouldReturnFalse_WhenFileIsNewButAboveUserCapacity()
    {
        // Arrange
        var storage = new CloudStorage();
        storage.AddUser(1, 600);
        storage.AddFile(1, "foto.png", 500);

        // Act
        bool result = storage.AddFile(1, "foto2.png", 300);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AddFile_ShouldReturnFalse_WhenUserNotFound()
    {
        // Arrange
        var storage = new CloudStorage();

        // Act
        bool result = storage.AddFile(1, "foto2.png", 300);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AddFile_ShouldReturnTrue_WhenAddingSameFileForDifferentUsers()
    {
        // Arrange
        var storage = new CloudStorage();
        storage.AddUser(1, 600);
        storage.AddUser(2, 600);

        // Act
        var result1 = storage.AddFile(1, "foto1.png", 300);
        var result2 = storage.AddFile(2, "foto1.png", 300);

        // Assert
        Assert.True(result1);
        Assert.True(result2);
    }

    [Fact]
    public void DeleteFile_ShouldReturnFile_WhenFileFound()
    {
        var storage = new CloudStorage();
        storage.AddUser(1, 600);
        storage.AddFile(1, "foto.png", 500);

        var result = storage.DeleteFile(1, "foto.png");

        Assert.NotNull(result);
        Assert.Equal(500, result);
    }

    [Fact]
    public void DeleteFile_ShouldReturnNull_WhenFileNotFound()
    {
        var storage = new CloudStorage();
        storage.AddUser(1, 600);

        var result = storage.DeleteFile(1, "inexistente.txt");

        Assert.Null(result);
    }

    [Fact]
    public void DeleteFile_ShouldReturnNull_WhenUserNotFound()
    {
        var storage = new CloudStorage();

        var result = storage.DeleteFile(1, "inexistente.txt");

        Assert.Null(result);
    }

    [Fact]
    public void GetFileSize_ShouldReturnFileSize_WhenFileFound()
    {
        var storage = new CloudStorage();
        storage.AddUser(1, 600);
        storage.AddFile(1, "arquivo1.txt", 500);

        var result = storage.GetFileSize(1, "arquivo1.txt");

        Assert.NotNull(result);
        Assert.Equal(500, result);
    }

    [Fact]
    public void GetFileSize_ShouldReturnNull_WhenFileNotFound()
    {
        var storage = new CloudStorage();
        storage.AddUser(1, 600);

        var result = storage.GetFileSize(1, "inexistente.txt");
        Assert.Null(result);
    }

    [Fact]
    public void GetFileSize_ShouldReturnNull_WhenUserNotFound()
    {
        var storage = new CloudStorage();

        var result = storage.GetFileSize(1, "arquivo1.txt");
        Assert.Null(result);
    }

    [Fact]
    public void GetNLargest_ShouldReturnLargest_OneUser()
    {
        //Arrange
        var storage = new CloudStorage();
        storage.AddUser(1, 600);

        storage.AddFile(1, "/dir/file1.txt", 5);
        storage.AddFile(1, "/dir/file2", 20);
        storage.AddFile(1, "/deeper/file2.mov", 9);

        //Act
        List<string> result = storage.GetNLargest(1, "/dir", 2);

        //Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("/dir/file2(20)", result[0]);
        Assert.Equal("/dir/file1.txt(5)", result[1]);
    }

    [Fact]
    public void GetNLargest_ShouldReturnEmpty_WhenPrefixNotFound_OneUser()
    {
        //Arrange
        var storage = new CloudStorage();
        storage.AddUser(1, 600);

        storage.AddFile(1, "/dir/file1.txt", 5);
        storage.AddFile(1, "/dir/file2", 20);
        storage.AddFile(1, "/dir/deeper/file2.mov", 9);

        //Act
        List<string> result = storage.GetNLargest(1, "/anotherdir", 2);

        //Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MergeUsers_ShouldReturnMergedUserId_WhenUsersMergedSuccessfully()
    {
        //Arrange
        var storage = new CloudStorage();
        storage.AddUser(1, 600);
        storage.AddUser(2, 400);

        storage.AddFile(1, "/dir/file1.txt", 150);
        storage.AddFile(2, "/dir/file2", 200);

        //Act
        var mergedUser = storage.MergeUsers(1, 2);

        //Assert
        Assert.NotNull(mergedUser);
        Assert.Equal(3, mergedUser.Id);
        Assert.Equal(2, mergedUser.Files.Count);
        Assert.Equal(1000, mergedUser.Capacity);
        Assert.Equal(350, mergedUser.StorageUsed);
        Assert.Equal(650, mergedUser.StorageAvailable);

    }
}
