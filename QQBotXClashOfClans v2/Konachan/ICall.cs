using Native.Csharp.App.Model;
using System;

namespace Native.Csharp.App.ApiCall
{
    public interface ICall: IDisposable
    {
        Post Request();
        string DownloadImage(Post json);
    }
}
