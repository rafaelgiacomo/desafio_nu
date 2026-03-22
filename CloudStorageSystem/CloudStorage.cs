

namespace CloudStorage;

public class CloudStorage
{
    private Dictionary<int, CloudUser> _users = new Dictionary<int, CloudUser>();
    private Dictionary<string, int> _files = new Dictionary<string, int>();

    public bool AddUser(int id, int capacity)
    {
        if (_users.ContainsKey(id))
            return false;

        _users.Add(id, new CloudUser(id, capacity));

        return true;
    }

    public bool AddFile(int userId, string name, int size)
    {
        if (!_users.ContainsKey(userId))
            return false;

        var user = _users.GetValueOrDefault(userId);

        return user!.AddFile(name, size);
    }

    public int? DeleteFile(int userId, string name)
    {
        if (!_users.ContainsKey(userId))
            return null;

        var user = _users.GetValueOrDefault(userId);

        return user!.DeleteFile(name);
    }

    public int? GetFileSize(int userId, string name)
    {
        if (!_users.ContainsKey(userId))
            return null;

        var user = _users.GetValueOrDefault(userId);

        return user!.GetFileSize(name);
    }

    public List<string> GetNLargest(int userId, params string[] prefix)
    {
        if (!_users.ContainsKey(userId))
            return new List<string>();

        var user = _users.GetValueOrDefault(userId);

        return user!.GetNLargest(prefix);
    }

    public List<string> GetNLargest(int userId, string prefix, int n)
    {
        if (!_users.ContainsKey(userId))
            return new List<string>();

        var user = _users.GetValueOrDefault(userId);

        return user!.GetNLargest(prefix, n);
    }

    public int? MergeUsers(int userId1, int userId2)
    {
        var user1 = _users.GetValueOrDefault(userId1);
        var user2 = _users.GetValueOrDefault(userId2);

        if(user1 == null || user2 == null)
            return null;

        int newUserId = _users.Count + 1;
        int newUserCapacity = user1.Capacity + user2.Capacity;
        
        var newUser = new CloudUser(newUserId, newUserCapacity)
        {
            Files = user1.MergeFiles(user2.Files, newUserId.ToString())
        };

        _users.Add(newUserId, newUser);

        return newUser.Id;
    }
}

public class CloudUser
{
    public int Id { get; set; }
    public int Capacity { get; set; }
    public Dictionary<string, int> Files { get; set; } = new Dictionary<string, int>();

    public int StorageUsed => Files.Sum(x => x.Value);
    public int StorageAvailable => Capacity - Files.Sum(x => x.Value);


    public CloudUser(int id, int capacity)
    {
        Id = id;
        Capacity = capacity;
    }

    public bool AddFile(string name, int size)
    {
        if (Files.ContainsKey(name) || size > StorageAvailable)
            return false;

        Files.Add(name, size);

        return true;
    }

    public int? DeleteFile(string name)
    {
        if (!Files.ContainsKey(name))
            return null;

        var fileSize = Files.GetValueOrDefault(name);
        Files.Remove(name);

        return fileSize;
    }

    public Dictionary<string, int> MergeFiles(Dictionary<string, int> mergingFiles, string? diffText = "1")
    {
        var mergedFiles = Files.ToDictionary();

        foreach(var file in mergingFiles)
        {
            if(!mergedFiles.ContainsKey(file.Key))
                mergedFiles.Add(file.Key, file.Value);
            else
                mergedFiles.Add($"({diffText}) {file.Key}", file.Value);
        }

        return mergedFiles;
    }

    public int? GetFileSize(string name)
    {
        return Files.ContainsKey(name) ? Files.GetValueOrDefault(name) : null;
    }

    public List<string> GetNLargest(params string[] prefix)
    {
        return GetNLargest(prefix, null);
    }

    public List<string> GetNLargest(string prefix, int n)
    {
        return GetNLargest([prefix], n);
    }

    private List<string> GetNLargest(string[] prefix, int? n = null)
    {
        var query = Files
            .Where(x => prefix.Any(y => x.Key.StartsWith(y)));

        if (n != null)
            query = query.Take(n.Value);

        return query
            .OrderByDescending(x => x.Value)
            .ThenBy(x => x.Key)
            .Select(x => $"{x.Key}({x.Value})")
            .ToList();
    }
}