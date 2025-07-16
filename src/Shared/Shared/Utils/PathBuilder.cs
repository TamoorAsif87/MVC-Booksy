using Microsoft.AspNetCore.Http;
namespace Shared.Utils;

public class BuildPath
{
    public List<BuildPath> Paths = new List<BuildPath>();
    private string _outPutPath = string.Empty;

    private string _path;

    public BuildPath(string path)
    {
        _path = path;
    }

    public BuildPath()
    {
        
    }

    private string GetFullPath()
    {
        _outPutPath = string.Empty;
        foreach (var e in Paths)
        {
            _outPutPath = Path.Combine(_outPutPath, e._path);
        }

        return _outPutPath;
    }

    public override string ToString()
    {
        return GetFullPath();
    }
}


public class PathBuilder
{
    private readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();
    private Dictionary<string,string> _keyPaths = new Dictionary<string, string>();
    private BuildPath buildPath = new BuildPath();

    public PathBuilder AddScheme()
    {
        var scheme = _httpContextAccessor.HttpContext?.Request.Scheme;
        if (string.IsNullOrEmpty(scheme))
        {
            throw new ArgumentException("Scheme cannot be null or empty.");
        }
        buildPath.Paths.Add(new BuildPath(scheme));
        _keyPaths.Add("scheme", scheme);
        return this;
    }

    public PathBuilder AddHost()
    {
        var host = _httpContextAccessor.HttpContext?.Request.Host.Value;
        if (string.IsNullOrEmpty(host))
        {
            throw new ArgumentException("Host cannot be null or empty.");
        }
        _keyPaths.Add("host", host);
        buildPath.Paths.Add(new BuildPath(host));
      
        return this;
    }

    public PathBuilder AddRootPath(string rootPath)
    {
        if (string.IsNullOrEmpty(rootPath))
        {
            throw new ArgumentException("Root Path cannot be null or empty.");
        }
        _keyPaths.Add("rootPath", rootPath);
        buildPath.Paths.Add(new BuildPath(rootPath));
       
        return this;
    }

    public PathBuilder AddRootDirectory(string directory)
    {
        if (string.IsNullOrEmpty(directory))
        {
            throw new ArgumentException("Directory Path cannot be null or empty.");
        }
        _keyPaths.Add("directoryPath", directory);
        buildPath.Paths.Add(new BuildPath(directory));

        return this;
    }

    public PathBuilder AddFileName(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentException("File name cannot be null or empty.");
        }
        _keyPaths.Add("fileName", filename);
        buildPath.Paths.Add(new BuildPath(filename));

        return this;
    }

    public override string ToString()
    {

        if(_keyPaths.ContainsKey("scheme") && _keyPaths.ContainsKey("host"))
        {
            buildPath.Paths.Clear();
            buildPath.Paths.Insert(0, new BuildPath($"{_keyPaths["scheme"]}://{_keyPaths["host"]}"));
            buildPath.Paths.Insert(1, new BuildPath($"{_keyPaths["directoryPath"]}"));
            buildPath.Paths.Insert(2, new BuildPath($"{_keyPaths["fileName"]}"));
        }
       
        return buildPath.ToString();
    }
}
