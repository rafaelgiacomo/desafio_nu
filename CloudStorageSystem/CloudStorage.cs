

namespace CloudStorage;

public class CloudStorage
{
    private Dictionary<string, int> _files = new Dictionary<string, int>();

    public bool AddFile(string name, int size)
    {
        if(_files.ContainsKey(name))
            return false;

        _files.Add(name, size);

        return true;
    }

    public int? DeleteFile(string name)
    {
        if(!_files.ContainsKey(name))
            return null;

        var fileSize = _files.GetValueOrDefault(name);
        _files.Remove(name);

        return fileSize;
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
            .Where(x => prefix.Any(y => x.Key.StartsWith(y)));

        if(n != null)
            query = query.Take(n.Value);

        return query
            .OrderByDescending(x => x.Value)
            .ThenBy(x => x.Key)
            .Select(x => $"{x.Key}({x.Value})")
            .ToList();
    }
}

