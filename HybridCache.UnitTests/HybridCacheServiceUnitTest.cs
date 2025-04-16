using HybridCache.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HybridCache.UnitTests;

public class HybridCacheServiceUnitTest
{
    private IMemoryCache _memCache;
    private IDistributedCache _distributedCache;
    private HybridCacheOptions _options;
    private HybridCacheService _hybridCacheService;
    
    private readonly string _key = "super-hero-mash";
    private readonly string _value = "Some awesome secret message for super hero's";
    
    [SetUp]
    public void Setup()
    {
        _distributedCache = new MemoryDistributedCache(new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        _memCache = new MemoryCache(new MemoryCacheOptions());
        _options = new HybridCacheOptions();
        _hybridCacheService = new HybridCacheService(_options, _memCache, _distributedCache);
    }

    [TearDown]
    public void TearDown() => _memCache.Dispose();
    
    [Test]
    public async Task TestDefaultCache_Test_Success()
    {
        var result = await _hybridCacheService.GetOrSetAsync(_key, async () => { await Task.Delay(1); return _value; });
        
        Assert.That(result, Is.EqualTo(_value));
    }

    [Test]
    public void TestExceptionStore()
    {
        Assert.DoesNotThrowAsync(async () =>
            await  _hybridCacheService.StoreHandledExceptionAsync("some_exception", new Exception("your exception thrown up", new AggregateException("pookie-ghost-hopper"))));
    }
}