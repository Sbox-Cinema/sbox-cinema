using Sandbox;
using System.Collections.Generic;

namespace Cinema;

/// <summary>
/// Stores data related to a media request that may be queued.
/// </summary>
public partial class MediaRequest : BaseNetworkable
{
    /// <summary>
    /// Contains information about this media that shall be displayed to the user.
    /// </summary>
    [Net]
    public MediaInfo Information { get; set; }
    /// <summary>
    /// The client that requested this media. If null, this media was
    /// requested by the server.
    /// </summary>
    [Net]
    public IClient Requestor { get; set; }

    [Net]
    private IDictionary<string, string> RequestData { get; set; }

    /// <summary>
    /// Get or set the value of a key in <c>RequestData</c>. <br/>
    /// If setting a key that does not yet exist, the key shall be created with the specified value. <br/>
    /// If getting a key that does not yet exist, <c>null</c> shall be returned.
    /// </summary>
    /// <param name="key">The key in <c>RequestData</c> that shall be get or set.</param>
    /// <returns>
    /// If getting, returns the value of the <paramref name="key"/> in that was specified, or <c>null</c>
    /// if no such key exists.
    /// </returns>
    public string this[string key]
    {
        get => GetRequestData(key);
        set => SetRequestData(key, value);
    }

    /// <summary>
    /// Retrieves a value from <c>RequestData</c> for the given key.
    /// </summary>
    /// <param name="key">A key that may exist in <c>RequestData</c>.</param>
    /// <returns>The value of the specified <c>RequestData</c> key, or <c>null</c> if the key does not exist.</returns>
    public string GetRequestData(string key)
    {
        if (!RequestData.ContainsKey(key))
            return null;

        return RequestData[key];
    }

    /// <summary>
    /// Sets a value in <c>RequestData</c> for the given key. Creates the key if
    /// it does not yet exist.
    /// </summary>
    /// <param name="key">A key that will be set or created in <c>RequestData</c>.</param>
    /// <param name="value">The value that will be assigned to the specified key in <c>RequestData</c>.</param>
    public void SetRequestData(string key, string value)
        => RequestData[key] = value;

}
