

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

    public CloudUser? MergeUsers(int userId1, int userId2)
    {
        var user1 = _users.GetValueOrDefault(userId1);
        var user2 = _users.GetValueOrDefault(userId2);

        if (user1 == null || user2 == null)
            return null;

        int newUserId = _users.Count + 1;
        var newUser = user1.MergeUser(user2, newUserId);

        _users.Add(newUserId, newUser);

        return newUser;
    }
}

public class CloudUser
{

    public int Id { get; private set; }
    public int Capacity { get; private set; }

    public int StorageUsed { get; private set; } = 0;
    public int StorageAvailable => Capacity - StorageUsed;

    private Dictionary<string, int> _files = new();
    public IReadOnlyDictionary<string, int> Files => _files;

    public CloudUser(int id, int capacity)
    {
        Id = id;
        Capacity = capacity;
    }

    public bool AddFile(string name, int size)
    {
        if (_files.ContainsKey(name) || size > StorageAvailable)
            return false;

        _files.Add(name, size);

        StorageUsed = StorageUsed + size;

        return true;
    }

    public int? DeleteFile(string name)
    {
        if (!_files.ContainsKey(name))
            return null;

        var fileSize = _files.GetValueOrDefault(name);
        
        _files.Remove(name);
        StorageUsed = StorageUsed - fileSize;

        return fileSize;
    }

    public CloudUser MergeUser(CloudUser user, int newUserId)
    {
        var mergedUser = new CloudUser(newUserId, Capacity + user.Capacity);

        foreach (var file in _files.Concat(user.Files))
        {
            if (!mergedUser.Files.ContainsKey(file.Key))
                mergedUser.AddFile(file.Key, file.Value);
            else
                mergedUser.AddFile($"({user.Id}) {file.Key}", file.Value);
        }

        return mergedUser;
    }

    public Dictionary<string, int> MergeFiles(Dictionary<string, int> mergingFiles, string? diffText = "1")
    {
        var mergedFiles = _files.ToDictionary();

        foreach (var file in mergingFiles)
        {
            if (!mergedFiles.ContainsKey(file.Key))
                mergedFiles.Add(file.Key, file.Value);
            else
                mergedFiles.Add($"({diffText}) {file.Key}", file.Value);
        }

        return mergedFiles;
    }

    public int? GetFileSize(string name)
    {
        return _files.ContainsKey(name) ? _files.GetValueOrDefault(name) : null;
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
        var query = _files
            .Where(x => prefix.Any(y => x.Key.StartsWith(y)))
            .OrderByDescending(x => x.Value)
            .ThenBy(x => x.Key)
            .AsQueryable();

        if (n != null)
            query = query.Take(n.Value);

        return query
            .Select(x => $"{x.Key}({x.Value})")
            .ToList();
    }
}